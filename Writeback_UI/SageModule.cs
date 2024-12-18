using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SAGE_Drivers;

namespace Writeback_UI
{
    class SageModule
    {
        //Parameters
        string APIURL = "";
        string APIENDPOINT = "";
        string APIFILTER = "";
        string APIKEY = "";
        string APPID = "XX";
        string PROGRAMNAME = "XX1000";
        string APPVERSION = "55A";
        string ERPUSER = "ADMIN";
        string ERPPASSWORD = "ADMIN";
        string COMPANYID = "SAMINC";
        string SYSTEMID = "SAMSYS";

        SAGEWriter _clsSAGE;

        public List<string> LogEvents { get; set; }

        public SageModule(string appid, string programnane, string appversion, string erpuser, string erppassword, string companyid, string systemid)
        {
            try
            {
                APPID = appid;
                PROGRAMNAME = programnane;
                APPVERSION = appversion;
                ERPUSER = erpuser;
                ERPPASSWORD = erppassword;
                COMPANYID = companyid;
                SYSTEMID = systemid;
                LogEvents = new List<string>();
            }
            catch
            {

            }
        }

        public string TestConnection()
        {
            string Response = "";
            
            try
            {
                Response = OpenReader();
                
                
            }
            catch (Exception ex)
            {
                Response = "ERROR on main function : " + ex.Message;
                
            }
            return Response;
        }

        public string OpenReader()
        {
            string Response = "";
            try
            {
                if (_clsSAGE == null)
                    Response = InitialiseSession();
                if (Response != "")
                    Response += ". ";

                Response += _clsSAGE.initDBLinks();
                
            }
            catch (Exception ex)
            {
                Response = "Local ERROR on open reader: " + ex.Message;
                
            }
            return Response;
        }


        public string InitialiseSession()
        {
            string Response = "";
            try
            {
                if (_clsSAGE == null)
                    _clsSAGE = new SAGEWriter(APPID, PROGRAMNAME, APPVERSION, ERPUSER, ERPPASSWORD, COMPANYID, SYSTEMID);

                Response = _clsSAGE.OpenSession();
                

            }
            catch (Exception ex)
            {
                Response = "Initialise session error: " + ex.Message;
                
            }
            return Response;
        }



        public string ReadViews()
        {
            string Response = "";
            List<string> ReturnList = new List<string>();
            try
            {
                if (_clsSAGE == null)
                    OpenReader();

                ReturnList = _clsSAGE.ReturnData();
                Response = ReturnList.Count.ToString() + " rows returned. ";
                if (ReturnList.Count == 1)
                {
                    string ResponseItem = ReturnList[0];
                    if (ResponseItem.ToUpper().Contains("ERROR"))
                        Response +=  "Read views: " + ResponseItem;
                }

            }
            catch (Exception ex)
            {
                Response = "Read views: " + ex.Message;
            }
            return Response;
        }


        private void AddLogMessage(bool IsError, string Message)
        {
            try
            {
                string EventType = "INFORMATION";
                if (IsError)
                    EventType = "ERROR";
                string strFileLine = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + EventType + "," + Message.Replace(",", ";");
                LogEvents.Add(strFileLine);
            }
            catch { }
        }
    }
}
