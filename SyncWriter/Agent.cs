using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonClass;
using APILib;
using System.Data;



namespace SyncWriter
{
    class Agent
    {
        //Default logs to the program data folder
        Logger _clsLogger = new Logger("");
        ConfigManager _clsConfig = new ConfigManager();
        APIClass _clsAPI = new APIClass();

        //Parameters with defaults
        string APIURL= "";
        string APIENDPOINT = "";
        string APIFILTER = "";
        string APIKEY = "";
        string ERPTYPE = "SAGE";
        string APPID = "XX";
        string PROGRAMNAME = "XX1000";
        string APPVERSION = "55A";
        string ERPUSER = "ADMIN";
        string ERPPASSWORD = "ADMIN";
        string COMPANYID = "SAMINC";
        string SYSTEMID = "SAMSYS";
        string PRINTOPTION = "1";

        //Testing only
        int TestRowCount = 0;
        bool Debug = false;

        


        string ConfigFolder;
        DataTable dtConfigSettings = new DataTable();

        public Agent(string folder, int TestRows = 0, bool TestDebug = false)
        {
            ConfigFolder = folder;
            if (TestRows > 0)
                TestRowCount = TestRows;
            if (TestDebug)
                Debug = true;
        }

        public void RunAgent()
        {
            try
            {
                LogInfo("Starting the agent", true, true);

                //Get default folder if not supplied (Program Data)
                if (ConfigFolder == "")
                {
                    ConfigFolder = _clsConfig.GetAppFolder("");

                }


                    LogInfo("Fetching config settings from folder:" + ConfigFolder, true, Debug);
                GetConfigSettings();
                if (dtConfigSettings.Rows.Count == 0)
                {
                    LogError("No config files found. Shutting down.", true, true);
                    return;
                }

                LogInfo("Fetching new write-back events:", true, true);
                DataTable dtEvents = GetAPIEvents();
                if(dtEvents.Rows.Count == 0)
                {
                    LogWarning("No events to process.", true, true);
                }
                else
                {
                    LogInfo(dtEvents.Rows.Count.ToString() +  " new events found.", true, true);
                    LogInfo("Processing write-back events:", true, true);
                    ProcessWriteBackEvents(dtEvents);
                }

                LogInfo("Completed write-back events. Shutting down.", true, true);



            }
            catch (Exception ex)
            {
                LogError("Agent general failure: " + ex.Message, true, true);

            }

        }


#region API get events
        // ***********************************************************
        //**   API get events ********************************
        //************************************************************

        private DataTable GetAPIEvents()
        {
            DataTable dtAPIData = new DataTable();
            try
            {
                

                string validation = "";
                if (APIURL == "")
                    validation = "API URL setting not found. ";
                if (APIENDPOINT == "")
                    validation += "API endpoint setting not found. ";
                if (APIKEY == "")
                    validation = "API key setting not found. ";

                if (validation != "")
                    LogError(validation, true, true);
                else
                {
                    
                    dtAPIData = _clsAPI.ReadAPI(APIURL, APIENDPOINT, APIFILTER, APIKEY, APIClass.HttpVerb.GET,0,1);

                }
            }
            catch (Exception ex)
            {

            }

            return dtAPIData;

        }
#endregion


#region Processing of write-back commands
        // ***********************************************************
        //**  Processing of Write-Back *******************************
        //************************************************************
        private string ProcessWriteBackEvents(DataTable dtEvents)
        {
            string Response = "";
            try
            {
                if(dtEvents.Rows.Count > 0)
                {
                    string LastPaymentID = "";
                    string LastPaymentResult = "";
                    int OverallSuccessCount = 0;
                    int OverallErrorCount = 0;
                    int SuccessCount = 0;
                    int ErrorCount = 0;
                    int Paymentcount = 0;
                    foreach(DataRow drEvent in dtEvents.Rows)
                    {
                        //testing
                        if (TestRowCount == 0 || Paymentcount < TestRowCount)
                        {
                            try
                            {
                                string PaymentResult = "";
                                string PaymentId = drEvent["id"].ToString();
                                string CustomerId = drEvent["data_customerId"].ToString();
                                string InvoiceNumber = "";
                                try
                                {
                                    InvoiceNumber = drEvent["paymentAllocations_invoiceNumber"].ToString();
                                }
                                catch { }
                                string AmountString = "";
                                try
                                {
                                    AmountString = drEvent["paymentAllocations_amount"].ToString();
                                }
                                catch { }
                                string TotalAmountString = "";
                                try
                                {
                                    TotalAmountString = drEvent["data_totalAmountPaid"].ToString();
                                }
                                catch { }

                                // Write to API if the id changed
                                if (LastPaymentID != PaymentId && LastPaymentID != "")
                                {
                                    LogInfo("Writing back API:" + LastPaymentID + " . ProcessMessage : " + LastPaymentResult, true, Debug);

                                    WritebackAPIResult(LastPaymentID, SuccessCount, ErrorCount, LastPaymentResult);

                                    OverallSuccessCount += SuccessCount;
                                    OverallErrorCount += ErrorCount;
                                    ErrorCount = 0;
                                    SuccessCount = 0;
                                    Paymentcount++;

                                }


                                LogInfo("Processing Id:" + PaymentId + ", Customer: " + CustomerId + ", Invoice : " + InvoiceNumber + ", Amount: " + AmountString, true, Debug);

                                decimal InvoicePayment = -999;
                                //check if unapplied amount is to be used for the whole payment (only process once)
                                if(AmountString == "" && InvoiceNumber == "" && LastPaymentID != PaymentId)
                                    decimal.TryParse(TotalAmountString, out InvoicePayment);
                                else
                                    decimal.TryParse(AmountString, out InvoicePayment);
                                


                                if (InvoicePayment > 0)
                                    PaymentResult = WriteERPData(PaymentId, ERPTYPE, CustomerId, InvoiceNumber, InvoicePayment);
                                else
                                    PaymentResult = "Error: invalid payment amount";


                                if (PaymentResult.ToUpper().Contains("ERROR"))
                                    ErrorCount++;
                                else
                                    SuccessCount++;

                                LastPaymentID = PaymentId;
                                LastPaymentResult = PaymentResult;
                            }
                            catch (Exception ex1)
                            {
                                LastPaymentResult = ex1.Message;
                                ErrorCount++;

                            }
                        }

                    }
                    WritebackAPIResult(LastPaymentID, SuccessCount, ErrorCount, LastPaymentResult);
                    OverallSuccessCount += SuccessCount;
                    OverallErrorCount += ErrorCount;
                    LogInfo("Completed payment write-backs with " + OverallSuccessCount.ToString() + " successful writes and " + OverallErrorCount.ToString() + " errors.", true, true);
                }
            }
            catch(Exception ex)
            {
                Response = "Error: " + ex.Message;
            }

            return Response;
        }
#endregion


#region ERP write-back process
        // ***********************************************************
        //**  ERP writes              ********************************
        //************************************************************
        private string WriteERPData(string ID, string ERPType, string CustomerCode, string Invoice, decimal Value)
        {
            string Response = "";
            try
            {
                switch (ERPType.ToUpper())
                {
                    case "SAGE":
                        
                        SAGEModule _clsSAGE = new SAGEModule(APPID, PROGRAMNAME, APPVERSION, ERPUSER, ERPPASSWORD, COMPANYID, SYSTEMID,PRINTOPTION);
                        Response = _clsSAGE.InsertReceipt(CustomerCode, Invoice, Value);
                        //get log events
                        if(Debug)
                        {
                            List<string> DebugLog = _clsSAGE.LogEvents;
                            if (DebugLog.Count > 0)
                            {
                                foreach(string LogEvent in DebugLog)
                                {
                                    _clsLogger.WriteEvent(LogEvent);
                                }
                            }
                        }


                        break;

                }
                
            }
            catch (Exception ex)
            {
                Response = "Error: " + ex.Message;
            }

            return Response;
        }
#endregion

#region API write-back operation
        // ***********************************************************
        //**  API response write-back ********************************
        //************************************************************
        private string WritebackAPIResult(string ID, int SuccessCount, int ErrorCount, string Result)
        {
            string Response = "";
            try
            {
                string Status = "CONSUMED";
                if (ErrorCount > 0)
                    Status = "FAILED";

                //Result = Result.Replace("\"", " ").Replace(":", " ").Replace("{", " ").Replace("}", " ");
                string Message = "";
                if (SuccessCount > 0 && ErrorCount == 0)
                    Message = "Successfully updated the transaction. " ;
                if(SuccessCount > 0 && ErrorCount > 0)
                    Message = "Partially updated some transactions with errors. " ;
                if (SuccessCount == 0 && ErrorCount > 0)
                    Message = "ERP Errors encountered while trying to write back. " ;
                string json = "{ \"status\": \"" + Status + "\",  \"message\": \"" + Message + "\", \"Segment\": \"string\"}";
                LogInfo("API Patch payload: " + json, true, Debug);
                if (!APIENDPOINT.EndsWith("/"))
                    APIENDPOINT += "/";
                Response = _clsAPI.WriteAPI(APIURL, APIENDPOINT , ID , APIKEY, APIClass.HttpVerb.PATCH, json);
                if(Response.ToUpper().Contains("ERROR"))
                    LogError("API Patch response error: " + Response, true, Debug);
            }
            catch (Exception ex)
            {
                Response = "Error: " + ex.Message;
                LogError("API Patch Update Error: " + Response, true, Debug);
            }
            return Response;
        }
#endregion


#region Configuration manager


        // ***********************************************************
        //**  config data manager ************************************
        //************************************************************
        private void GetConfigSettings()
        {
            try
            {
                if (!ConfigFolder.EndsWith("\\"))
                    ConfigFolder += "\\";
                ConfigFolder += "Config.json";
                dtConfigSettings = _clsConfig.ReadTablefromJsonFile(ConfigFolder);
                if (dtConfigSettings.Rows.Count == 0)
                    LogError("Error - no config settings were found in folder:  " + ConfigFolder + ". " + _clsConfig.LastError, true, true);

                APIURL = ReadConfigSettingByName("URL", APIURL,false);
                APIENDPOINT = ReadConfigSettingByName("ENDPOINT", APIENDPOINT,false);
                APIFILTER = ReadConfigSettingByName("APIFILTER", APIFILTER,false);
                APIKEY = ReadConfigSettingByName("APIKEY", APIKEY,true);
                ERPTYPE = ReadConfigSettingByName("ERPTYPE", ERPTYPE, false);
                APPID = ReadConfigSettingByName("APPID", APPID,false);
                PROGRAMNAME = ReadConfigSettingByName("PROGRAMNAME", PROGRAMNAME,false);
                APPVERSION = ReadConfigSettingByName("APPVERSION", APPVERSION,false);
                ERPUSER = ReadConfigSettingByName("ERPUSER", ERPUSER,false);
                ERPPASSWORD = ReadConfigSettingByName("ERPPASSWORD", ERPPASSWORD,true);
                COMPANYID = ReadConfigSettingByName("COMPANYID", COMPANYID,false);
                SYSTEMID = ReadConfigSettingByName("SYSTEMID", SYSTEMID,false);
                PRINTOPTION = ReadConfigSettingByName("PRINTOPTION", PRINTOPTION, false);



            }
            catch (Exception ex)
            {
                LogError("Config load error: " + ex.Message, true, true);
            }

        }

        private string ReadConfigSettingByName(string ConfigName, string DefaultValue, bool Decrypt)
        {
            string Setting = "";
            try
            {
                if(dtConfigSettings.Rows.Count > 0)
                {
                    foreach(DataRow dr in dtConfigSettings.Rows)
                    {
                        if (dr[0].ToString().ToUpper() == ConfigName)
                            Setting = dr[1].ToString();
                    }
                }

                if (Decrypt)
                    Setting = _clsConfig.ConvertToDecrypted(Setting);

            }
            catch { }
            if (Setting == "" && DefaultValue != "")
                Setting = DefaultValue;
            return Setting;
        }

#endregion

        // ***********************************************************
        //**  Logging ************************************************
        //************************************************************
        private void LogError(string ErrorMessage,bool Show,bool Log)
        {
            try
            {
                if(Show)
                    Console.WriteLine("Error:" + ErrorMessage);
                if (Log)
                    _clsLogger.WriteErrorLog(ErrorMessage);
            }
            catch { }
        }
        private void LogInfo(string ErrorMessage, bool Show, bool Log)
        {
            try
            {
                if (Show)
                    Console.WriteLine(ErrorMessage);
                if (Log)
                    _clsLogger.WriteInfoLog(ErrorMessage);
            }
            catch { }
        }

        private void LogWarning(string ErrorMessage, bool Show, bool Log)
        {
            try
            {
                if (Show)
                    Console.WriteLine(ErrorMessage);
                if (Log)
                    _clsLogger.WriteWarningLog(ErrorMessage);
            }
            catch { }
        }

    }
}
