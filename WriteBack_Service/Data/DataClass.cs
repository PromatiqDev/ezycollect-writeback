using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.SqlClient;
using System.Data;
using Newtonsoft.Json;

namespace ezyCollect_Writeback_Service.Data
{

    class DataClass
    {

        static SqlConnection _SQLConnection = new SqlConnection();
        static string _SQLConnectionString;
        static string strLastErrorMessage;


        //Last Error properties
        public string getLastErrorMessage()
        {
            return strLastErrorMessage;

        }

        public string ConstructSQLConnectionString(string SQLServer, string Database, string Username, string Password)
        {
            if (Password != "")
                Password = ConvertToDecrypted(Password);
            if (Username == "")
                return "Data Source = " + SQLServer + "; Initial Catalog = " + Database + "; Integrated Security = True";
            else
                return "Data Source = " + SQLServer + "; Initial Catalog = " + Database + "; Integrated Security = False; User ID = " + Username + "; Password = " + Password;
        }




        public string ConvertToDecrypted(string Encrypted)
        {
            string Password = "";
            try
            {
                if (Encrypted.StartsWith("#***"))
                {

                    Password = DecryptPassword(Encrypted);
                }
                else
                    Password = Encrypted;
            }
            catch
            {
                Password = "ERROR";
            }
            return Password;
        }

        public string DecryptPassword(string Encrypted)
        {
            string Password = "";
            try
            {
                int NumChars = 3;
                int StartChar = 0;
                Encrypted = Encrypted.Replace("#***", "");
                while (StartChar < Encrypted.Length)
                {
                    char DecryptChar = Convert.ToChar((Convert.ToInt32(Encrypted.Substring(StartChar, NumChars)) - 3));
                    Password += DecryptChar.ToString();
                    StartChar = StartChar + NumChars;
                }
            }
            catch (Exception ex)
            {
                Password = "ERROR:  " + ex.Message;
            }
            return Password;

        }



        #region SQL Server
        #region Common Global Connection Handler
        /* Function OpenConnection ************************************************/
        /* Creates a connection object, opens and returns it **********************/

        //global connection state handler
        public void SetConnectionString(string SQLConnectionString)
        {
            if (_SQLConnectionString != SQLConnectionString)
            {
                _SQLConnectionString = SQLConnectionString;
                try
                {
                    if (_SQLConnection.State == ConnectionState.Open)
                    {
                        _SQLConnection.Close();
                    }
                }
                catch
                {

                }
            }

        }


        public bool OpenGlobalConnection(bool ConfirmConnection)
        {
            bool bConnectionState = false;
            try
            {
                if (_SQLConnection.State != ConnectionState.Open)
                {
                    try
                    {
                        if (_SQLConnection.State != ConnectionState.Closed)
                            _SQLConnection.Close();
                    }
                    catch { }

                    _SQLConnection.ConnectionString = _SQLConnectionString;
                    _SQLConnection.Open();
                    bConnectionState = true;
                }
                else
                {
                    if (ConfirmConnection)
                    {
                        //check to see if the connection is available else close it and reopen it
                        bool bConnectionHealthy = false;
                        try
                        {
                            //check connection string
                            if (_SQLConnection.ConnectionString == _SQLConnectionString)
                            {
                                SqlCommand cmdUserCommand = new SqlCommand("Select 'TEST' as TEST", _SQLConnection);
                                int iRecords;
                                iRecords = cmdUserCommand.ExecuteNonQuery();
                                cmdUserCommand.Dispose();
                                bConnectionHealthy = true;
                                bConnectionState = true;
                            }
                            else
                            {
                                //reconnect with correct string
                                _SQLConnection.Close();
                                _SQLConnection.ConnectionString = _SQLConnectionString;
                                _SQLConnection.Open();
                                SqlCommand cmdUserCommand = new SqlCommand("Select 'TEST' as TEST", _SQLConnection);
                                int iRecords;
                                iRecords = cmdUserCommand.ExecuteNonQuery();
                                cmdUserCommand.Dispose();
                                bConnectionHealthy = true;
                                bConnectionState = true;
                            }
                        }
                        catch
                        {
                            bConnectionState = false;
                        }
                        if (!bConnectionHealthy)
                        {
                            try
                            {
                                //close and reopen connection
                                if (_SQLConnection.State == ConnectionState.Open)
                                {
                                    _SQLConnection.Close();
                                    _SQLConnection.Open();
                                    bConnectionState = true;
                                }


                            }
                            catch (Exception ex1)
                            {
                                bConnectionState = false;
                                strLastErrorMessage = ex1.Message;
                            }
                        }
                    }
                    else
                        bConnectionState = true;
                }


            }
            catch (Exception ex)
            {
                bConnectionState = false;
                strLastErrorMessage = ex.Message;
            }
            return bConnectionState;
        }

        //global connection state handler
        public bool CloseGlobalConnection()
        {
            bool bConnectionState = false;
            try
            {
                if (_SQLConnection.State == ConnectionState.Open)
                {
                    _SQLConnection.Close();
                }

            }
            catch
            {

            }
            return bConnectionState;
        }
        #endregion


        #region External Connection Handler
        /* Function OpenConnection ************************************************/
        /* Creates a connection object, opens and returns it **********************/
        public SqlConnection OpenConnection(SqlConnection dcReadWrite, string DataConnectionString)
        {

            try
            {
                dcReadWrite = new SqlConnection(DataConnectionString);
                dcReadWrite.Open();

            }
            catch //( System.Exception ex)
            {

            }
            return dcReadWrite;
        }

        public SqlConnection CloseConnection(SqlConnection dcReadWrite)
        {

            try
            {
                dcReadWrite.Close();

            }
            catch //( System.Exception ex)
            {

            }
            return dcReadWrite;
        }
        #endregion
               

        /* Function ProcessQuery ************************************************/
        /* Returns string Errormessage or Empty String if No Errors**************/
        public string ProcessQuery(string QueryString, bool WithResult)
        {

            string Result = "";
            try
            {
                if (OpenGlobalConnection(false))
                {

                    SqlCommand cmdUserCommand = new SqlCommand(QueryString, _SQLConnection);

                    cmdUserCommand.CommandTimeout = 200;
                    cmdUserCommand.ExecuteNonQuery();

                    cmdUserCommand.Dispose();
                    CloseGlobalConnection();
                }
                else
                    Result = "Connection cannot be opened";
            }
            catch (Exception e)
            {
                if (WithResult == true) Result = e.Message;
                CloseGlobalConnection();
            }
            return Result;
        }


        //Overload Returning Actual Failure
        /* Function ProcessQuery Overload 3**************************************/
        /* Uses the supplied connection strting *********************************/
        /* Returns string Errormessage or Empty String if No Errors**************/
        public string ProcessQuery(string QueryString, string strConnectionstring)
        {

            string Result = "";
            try
            {
                SqlConnection conn = new SqlConnection(strConnectionstring);
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    SqlCommand cmdUserCommand = new SqlCommand(QueryString, conn);

                    cmdUserCommand.CommandTimeout = 200;
                    cmdUserCommand.ExecuteNonQuery();

                    cmdUserCommand.Dispose();
                    conn.Close();
                }
                else
                    Result = "Connection cannot be opened";
            }
            catch (Exception e)
            {
                Result = e.Message;

            }
            return Result;
        }

        /* Function ReturnQuery   ***********************************************/
        /* Returns string Query Result ******************************************/
        public string ReturnQuery(string QueryString)
        {
            string strReturnData = "";
            strLastErrorMessage = "";
            try
            {
                if (OpenGlobalConnection(false))
                {
                    SqlCommand cmdUserCommand = new SqlCommand(QueryString, _SQLConnection);

                    SqlDataReader drRunQuery;

                    cmdUserCommand.CommandTimeout = 200;
                    drRunQuery = cmdUserCommand.ExecuteReader();
                    drRunQuery.Read();
                    if (drRunQuery.HasRows == true)
                    {
                        strReturnData = drRunQuery.GetValue(0).ToString();

                    }
                    else
                    {
                        strReturnData = "";

                    }
                    //dcReadWrite.Close();
                    if (strReturnData.Length == 0)
                    {
                        strReturnData = "";
                    }
                    drRunQuery.Close();
                    drRunQuery.Dispose();
                    cmdUserCommand.Dispose();
                    CloseGlobalConnection();
                }
                else
                {
                    strReturnData = "Connection Error - " + strLastErrorMessage;
                }

            }
            catch (System.Exception ex)
            {
                strReturnData = "Error:" + ex.Message;
                strLastErrorMessage = ex.Message;
                CloseGlobalConnection();
            }
            return strReturnData;

        }

        /* Overload using connectionstring Returns string Query Result ******************************************/
        public string ReturnQuery(string QueryString, string strConnectionstring)
        {
            string strReturnData = "";
            strLastErrorMessage = "";
            try
            {
                SqlConnection conn = new SqlConnection(strConnectionstring);
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    SqlCommand cmdUserCommand = new SqlCommand(QueryString, conn);

                    SqlDataReader drRunQuery;

                    cmdUserCommand.CommandTimeout = 200;
                    drRunQuery = cmdUserCommand.ExecuteReader();
                    drRunQuery.Read();
                    if (drRunQuery.HasRows == true)
                    {
                        strReturnData = drRunQuery.GetValue(0).ToString();

                    }
                    else
                    {
                        strReturnData = "";

                    }
                    //dcReadWrite.Close();
                    if (strReturnData.Length == 0)
                    {
                        strReturnData = "";
                    }
                    drRunQuery.Close();
                    drRunQuery.Dispose();
                    cmdUserCommand.Dispose();
                    conn.Close();
                }
                else
                {
                    strReturnData = "Connection Error - " + strLastErrorMessage;
                }

            }
            catch (System.Exception ex)
            {
                strReturnData = "Error:" + ex.Message;
                strLastErrorMessage = ex.Message;
                CloseGlobalConnection();
            }
            return strReturnData;

        }

        /* Function ReturnTable   ***********************************************/
        /* Returns DataTable  ***************************************************/

        public DataTable ReturnTable(string QueryString)
        {

            DataTable dtTableData = new DataTable();
            strLastErrorMessage = "";
            try
            {
                if (OpenGlobalConnection(false))
                {
                    SqlCommand cmdDataCommand = new SqlCommand(QueryString, _SQLConnection);

                    SqlDataAdapter daDatafetch = new SqlDataAdapter(cmdDataCommand);
                    DataSet dsDataFile = new DataSet();

                    cmdDataCommand.CommandTimeout = 200;

                    daDatafetch.Fill(dsDataFile, "Table1");
                    dtTableData = dsDataFile.Tables[0];

                    cmdDataCommand.Dispose();


                }
            }
            catch (Exception ex)
            {

                strLastErrorMessage = ex.Message;
                CloseGlobalConnection();
            }
            return dtTableData;


        }

        public DataTable ReturnTable(string QueryString, string strConnectionstring)
        {

            DataTable dtTableData = new DataTable();
            strLastErrorMessage = "";
            try
            {
                SqlConnection conn = new SqlConnection(strConnectionstring);
                conn.Open();
                if (conn.State == ConnectionState.Open)
                {
                    SqlCommand cmdDataCommand = new SqlCommand(QueryString, conn);

                    SqlDataAdapter daDatafetch = new SqlDataAdapter(cmdDataCommand);
                    DataSet dsDataFile = new DataSet();

                    cmdDataCommand.CommandTimeout = 200;

                    daDatafetch.Fill(dsDataFile, "Table1");
                    dtTableData = dsDataFile.Tables[0];

                    cmdDataCommand.Dispose();

                    conn.Close();
                    conn.Dispose();

                }
                else
                    strLastErrorMessage = "Unable to open the connection";
            }
            catch (Exception ex)
            {

                strLastErrorMessage = ex.Message;

            }
            return dtTableData;


        }

        #endregion

        #region json file read and writes
        /* Function  TabletoJsonFile************************************************/
        public string WriteTabletoJsonFile(DataTable dtInput, string FileName)
        {
            string Result = "";
            try
            {
                string json = @"{" + "\"Data\":";
                json += JsonConvert.SerializeObject(dtInput, Newtonsoft.Json.Formatting.Indented);
                json += "}";

                string[] lines = json.Split('\n');


                System.IO.StreamWriter file = new System.IO.StreamWriter(FileName, false);

                {
                    foreach (string line in lines)
                    {
                        file.WriteLine(line);

                    }
                }

                file.Close();
            }
            catch (Exception ex)
            {
                Result = "ERROR : " + ex.ToString();
            }
            return Result;

        }

        public DataTable ReadTablefromJsonFile(string FileName)
        {
            DataTable Result = new DataTable();
            try
            {


                string json = @"";


                System.IO.StreamReader file = new System.IO.StreamReader(FileName);
                while (!file.EndOfStream)
                {
                    json += file.ReadLine();
                }


                file.Close();

                DataSet dsData = JsonConvert.DeserializeObject<DataSet>(json);
                Result = dsData.Tables[0];
            }
            catch (Exception ex)
            {
                strLastErrorMessage = ex.Message;
            }
            return Result;

        }

        public DataTable ReadTablefromJson(string json)
        {
            DataTable Result = new DataTable();
            try
            {


                DataSet dsData = JsonConvert.DeserializeObject<DataSet>(json);
                Result = dsData.Tables[0];
            }
            catch (Exception ex)
            {
                strLastErrorMessage = ex.Message;
            }
            return Result;

        }

        #endregion

        #region file read and writes
        /* Function  Read delimited file ************************************************/
        public DataTable ReadDelimitedToTable(string Folder, string FileName, string Delimiter, string DelimitedEscape, bool HasHeader, int SkipRows)
        {
            DataTable dtReturn = new DataTable();
            try
            {
                Char[] DelimiterChar = Delimiter.ToCharArray();
                Char[] EscapeChar = DelimitedEscape.ToCharArray();
                if (!Folder.EndsWith("\\") && FileName != "")
                    Folder += "\\";
                string DelimiterReplace = "#|#";

                

                string[] SearchFiles = Directory.GetFiles(Folder, FileName);
                if (SearchFiles.Length > 0)
                {
                    FileName = SearchFiles[0];
                }
                else
                    FileName = Folder + FileName;

                if (File.Exists(FileName))
                {
                    List<string> lines = File.ReadLines(FileName).ToList();
                    int RowNumber = 0;
                    foreach (string Line in lines)
                    {
                        string InsertLine = Line;
                        int ExcapeFound = 0;
                        //Replace escape char encapsulated by quotes
                        if (DelimitedEscape != "" && Line.Contains(DelimitedEscape))
                        {
                            ExcapeFound = 1;
                            string Templine = "";
                            int EscapeStart = -2;
                            int EscapeEnd = -1;
                            int LastPoint = -1;
                            int Cyclecount = 0;
                            while (EscapeStart < Line.Length)
                            {
                                if (EscapeStart < EscapeEnd)
                                    EscapeStart = Line.IndexOf(EscapeChar[0], EscapeEnd + 1);
                                else
                                    EscapeEnd = Line.IndexOf(EscapeChar[0], EscapeStart + 1);

                                if (EscapeStart < EscapeEnd && EscapeStart > LastPoint && EscapeStart > -1)
                                {
                                    if (LastPoint < 0)
                                        LastPoint = 0;
                                    if (EscapeStart > LastPoint)
                                        Templine += Line.Substring(LastPoint, EscapeStart - LastPoint).Replace(DelimitedEscape, "");

                                    Templine += Line.Substring(EscapeStart, EscapeEnd - EscapeStart).Replace(DelimitedEscape, "").Replace(Delimiter, DelimiterReplace);
                                    LastPoint = EscapeEnd;
                                }
                                //ensure it doesn't get stuck
                                if (Cyclecount > Line.Length || EscapeStart == -1)
                                    EscapeStart = Line.Length;
                                Cyclecount++;
                            }
                            if (Templine != "")
                            {
                                if (LastPoint < Line.Length - 1)
                                    Templine += Line.Substring(LastPoint + 1);
                                InsertLine = Templine;
                            }
                        }


                        string[] SplitRow = InsertLine.Split(DelimiterChar);
                        if (RowNumber == 0)
                        {
                            for (int c = 0; c < SplitRow.Length; c++)
                            {
                                if (HasHeader)
                                {
                                    dtReturn.Columns.Add(SplitRow[c]);
                                }
                                else
                                {
                                    dtReturn.Columns.Add("COLUMN" + (c + 1).ToString());
                                }

                            }
                            if (HasHeader)
                                SkipRows = 1;
                        }
                        if (RowNumber >= SkipRows && SplitRow.Length > 0)
                        {
                            DataRow drInsert = dtReturn.NewRow();
                            for (int c = 0; c < SplitRow.Length; c++)
                            {
                                //Check for escape chars re-insert delimiter
                                if (ExcapeFound > 0)
                                    SplitRow[c] = SplitRow[c].Replace(DelimiterReplace, Delimiter);
                                if (c < dtReturn.Columns.Count)
                                    drInsert[c] = SplitRow[c];
                            }
                            dtReturn.Rows.Add(drInsert);
                        }
                        RowNumber++;
                    }

                    
                }
                else
                    strLastErrorMessage = "The file was not found";


            }
            catch (Exception ex)
            {
                strLastErrorMessage = "Read Error: " + ex.Message;
            }
            return dtReturn;
        }


        public string WriteFileLine(string Folder, string FileName, string FileLine, bool Append)
        {
            string Result = "";
            try
            {
                if (!Folder.EndsWith("\\"))
                    Folder += "\\";
                System.IO.StreamWriter file = new System.IO.StreamWriter(Folder + FileName, Append);
                file.WriteLine(FileLine);


                file.Close();
            }
            catch (Exception ex)
            {
                Result = "ERROR : " + ex.ToString();
            }
            return Result;

        }

        //Inserts multiple linesin a query
        /* Function ProcessTable           **************************************/
        /* Uses the supplied connection strting *********************************/
        /* Returns string Errormessage or Empty String if No Errors**************/
        public string ProcessTableToDelimitedFile(DataTable dtOutput, string Folder, string FileName, string Delimiter, string DelimitedEscape, bool IncludeHeadings, bool Append)
        {
            string Fileline = "";
            string Result = "";
            int successcount = 0;
            int errorcount = 0;
            int ColCount = 0;
            try
            {
                if (dtOutput == null || dtOutput.Rows.Count == 0)
                    return "Error: No data to process";

                //set defaults
                if (DelimitedEscape == "")
                    DelimitedEscape = "\"";

                if (Delimiter.ToUpper() == "CSV")
                    Delimiter = ",";

                if (!Folder.EndsWith("\\"))
                    Folder += "\\";
                System.IO.StreamWriter file = new System.IO.StreamWriter(Folder + FileName, Append);


                if (IncludeHeadings)
                {

                    foreach (DataColumn col in dtOutput.Columns)
                    {
                        string ColName = col.ColumnName;
                        if (ColName.Contains(Delimiter))
                            ColName = DelimitedEscape + ColName + DelimitedEscape;
                        if (ColCount > 0)
                            Fileline += Delimiter;
                        ColCount++;
                        Fileline = Fileline + ColName;

                    }
                    file.WriteLine(Fileline);
                }

                foreach (DataRow dr in dtOutput.Rows)
                {
                    try
                    {
                        Fileline = "";

                        for (int c = 0; c < dr.ItemArray.Length; c++)
                        {
                            string CellValue = dr[c].ToString();
                            if (CellValue.Contains(Delimiter))
                                CellValue = DelimitedEscape + CellValue + DelimitedEscape;
                            if (c > 0)
                                Fileline += ",";
                            Fileline += CellValue;
                        }
                        file.WriteLine(Fileline);
                        successcount++;
                    }
                    catch (Exception ex)
                    {
                        Result = "Error: " + ex.Message;
                        errorcount++;
                    }

                }
                file.Close();

            }
            catch (Exception e)
            {
                Result = "Overall Error: " + e.Message;
            }
            Result += successcount.ToString() + " rows inserted successfully with " + errorcount.ToString() + " exceptions.";
            return Result;
        }

        public string DeleteFiles(string Folder, string FileSearch, string FileType, int AgeDays)
        {
            string Result = "";
            try
            {
                string[] Files = Directory.GetFiles(Folder, "*" + FileSearch + "*." + FileType);
                foreach (string FileName in Files)
                {
                    try
                    {
                        FileInfo fi = new FileInfo(FileName);
                        if (fi.LastAccessTime < DateTime.Now.AddDays(-1 * AgeDays))
                            fi.Delete();
                    }
                    catch
                    {

                    }

                }


            }
            catch (Exception ex)
            {
                Result = "ERROR : " + ex.ToString();
            }
            return Result;

        }
        #endregion



    }


}
