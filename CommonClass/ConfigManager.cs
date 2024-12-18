using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft;
using Newtonsoft.Json;
using System.Data;
using System.IO;

namespace CommonClass
{
    public class ConfigManager
    {

        public string LastError { get; set; }

        public string GetAppName()
        {
            return "ezyCollect\\SyncWriteBack";
        }

        //fetches the app data location
        public string GetAppFolder(string AppName)
        {
            string ReturnFolderName = "";
            try
            {
                if (AppName == "")
                    AppName = GetAppName();
                string commonpath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                if (!commonpath.EndsWith("\\"))
                    commonpath += "\\";
                ReturnFolderName = commonpath + AppName;
            }
            catch (Exception ex)
            {
                ReturnFolderName = "ERROR: " + ex.Message;
            }
            return ReturnFolderName;
        }



        public DataTable ReadTablefromJsonFile(string FileName)
        {
            LastError = "";
            DataTable Result = new DataTable();
            try
            {


                string json = @"";


                StreamReader file = new StreamReader(FileName);
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
                LastError = ex.Message;
            }
            return Result;

        }


        public string WriteTableToConfigFile(DataTable dtConfig,string ConfigFolder, string FileName)
        {
            string Response = "";
            try
            {
                if (dtConfig.Rows.Count > 0)
                {
                    if (!Directory.Exists(ConfigFolder))
                        Directory.CreateDirectory(ConfigFolder);


                    //check if the folder exist
                    if (!ConfigFolder.EndsWith("\\"))
                    ConfigFolder += "\\";
                    

                    System.IO.StreamWriter file = new System.IO.StreamWriter(ConfigFolder + FileName, false);
                    //get file line to write
                    string strFileLine = "{\"Settings\":[";
                    file.WriteLine(strFileLine);
                    int iRowcount = 0;
                    foreach (DataRow dr in dtConfig.Rows)
                    {
                        string strKey = dr[0].ToString();
                        if(strKey.Length > 0)
                        {
                            string strValue = dr[1].ToString();
                            strFileLine = "{ \"KeyName\":\"" + strKey + "\",\"KeyValue\":\"" + strValue + "\"}";
                            if (iRowcount > 0)
                                strFileLine = "," + strFileLine;
                            file.WriteLine(strFileLine);
                            iRowcount++;
                        }
                        
                    }

                    strFileLine = "]}";
                    file.WriteLine(strFileLine);
                    file.Close();
                    Response = "Config settings written successfully";
                }
                else
                {
                    Response = "Error: No settings to save}";
                }


            }
            catch(Exception ex) 
            {
                Response = "Error: " + ex.Message;
            }
            return Response;
        }


        #region Password Encryption

        public string ConvertToEncrypted(string Password)
        {
            string Encrypted = "";
            try
            {
                if (Password != null && Password != "")
                {
                    if (Password.StartsWith("##**"))
                    {
                        Encrypted = Password;
                    }
                    else
                        Encrypted = EncryptPassword(Password);
                }
            }
            catch
            {
                Encrypted = "ERROR";
            }
            return Encrypted;
        }

        public string ConvertToDecrypted(string Encrypted)
        {
            string Password = "";
            try
            {
                if (Encrypted.StartsWith("##**"))
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


        public string EncryptPassword(string Password)
        {
            string Encrypted = "";
            
            try
            {                
                char[] inputchar = Password.ToCharArray();
                byte[] data = new byte[Password.Length];

                for (int i = 0; i < Password.Length; i++)
                {
                    Encrypted += PadString((Convert.ToInt32(Convert.ToByte(inputchar[i])) + 2).ToString(), 3);
                }
                Encrypted = "##**" + Encrypted;
               
            }
            catch (Exception ex)
            {
                Encrypted = "ERROR:  " + ex.Message;
            }
            
            return Encrypted;

        }

        public string DecryptPassword(string Encrypted)
        {
            string Password = "";
            try
            {
                int NumChars = 3;
                int StartChar = 0;
                Encrypted = Encrypted.Replace("##**", "");
                while (StartChar < Encrypted.Length)
                {
                    char DecryptChar = Convert.ToChar((Convert.ToInt32(Encrypted.Substring(StartChar, NumChars)) - 2));
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

        public string PadString(string OriginalString, int PadLength)
        {
            string Result = OriginalString;
            try
            {
                while (Result.Length < PadLength)
                {
                    Result = "0" + Result;
                }
            }
            catch
            { }
            return Result;

        }

        #endregion

    }
}
