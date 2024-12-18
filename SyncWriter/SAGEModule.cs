using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SAGE_Drivers;

namespace SyncWriter
{
    public class SAGEModule
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
        string PRINTOPTION = "1";

        SAGEWriter _clsSAGE;

        public List<string> LogEvents { get; set; }

        public SAGEModule(string appid, string programnane, string appversion, string erpuser, string erppassword, string companyid, string systemid,string printoption)
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
                PRINTOPTION = printoption;
                LogEvents = new List<string>();
            }
            catch
            {

            }
        }

        public string GetLastError()
        {
            try
            {
                return _clsSAGE.LastErrorMessage;

            }
            catch (Exception ex)
            {
                return "Last error not accessable " + ex.Message;
            }
        }

        public decimal GetInvoiceOutstanding(string Customer, string Invoice)
        {
            decimal Return = -9999;
            try
            {
                if (_clsSAGE == null)
                    OpenReader();

                Return = _clsSAGE.ReturnInvoiceDue(Customer, Invoice);
                

            }
            catch (Exception ex)
            {

            }
            return Return;
        }

        public string InsertReceipt(string CustomerCode, string InvoiceNumber, decimal PaymentAmount)
        {
            string Response = "";
            if (LogEvents == null)
                LogEvents = new List<string>();
            else
                LogEvents.Clear();
            try
            {
                if (_clsSAGE == null)
                    OpenReader();


                // get outstanding
                decimal AppliedAmount = 0;
                decimal Outstanding = -9999;
                if(InvoiceNumber != "")
                    Outstanding = GetInvoiceOutstanding(CustomerCode, InvoiceNumber);
                   

                if(Outstanding < 0)
                {
                    //can't find amount to apply, make unapplied
                    AppliedAmount = 0;
                    if (InvoiceNumber != "")
                    {
                        string ReadOutstandingResult = GetLastError();
                        AddLogMessage(true, "Error reading invoice due amount: " + ReadOutstandingResult);
                    }
                }
                else
                {
                    //get the balance to apply
                    if (PaymentAmount > Outstanding)
                        AppliedAmount = Outstanding;
                    else
                        AppliedAmount = PaymentAmount;

                    AddLogMessage(false, "Getting amount to apply: " + AppliedAmount.ToString());
                }



                if(PRINTOPTION == "1")
                    Response += _clsSAGE.InsertBatchWithPrint(CustomerCode, InvoiceNumber, PaymentAmount, AppliedAmount);
                else
                    Response += _clsSAGE.InsertBatch(CustomerCode, InvoiceNumber, PaymentAmount,AppliedAmount);
                if (Response.ToUpper().Contains("ERROR"))
                    AddLogMessage(true, "Inserting batch: " +  Response);
                else
                    AddLogMessage(false, "Inserting batch: " + Response);
            }
            catch (Exception ex)
            {
                Response = "Local ERROR on main function : " + ex.Message;
                AddLogMessage(true, Response);
            }
            return Response;
        }

        public string OpenReader()
        {
            string Response = "";
            try
            {
                if (_clsSAGE == null)
                    InitialiseSession();
                
                Response = _clsSAGE.initDBLinks();
                if (Response.ToUpper().Contains("ERROR"))
                    AddLogMessage(true, "Creating DB links: " + Response);
                else
                    AddLogMessage(false, "Creating DB links: " + Response);
            }
            catch (Exception ex)
            {
                Response = "Local ERROR on open reader: " + ex.Message;
                AddLogMessage(false, Response);
            }
            return Response;
        }


        public string InitialiseSession()
        {
            string Response = "";
            try
            {
                AddLogMessage( false,"Initialising SAGE session for AppID: " + APPID + "; Programme: " + PROGRAMNAME + "; AppVersion: " + APPVERSION + "; User: " + ERPUSER + "; Company: " + COMPANYID + "; SystemId: " + SYSTEMID);
                if (_clsSAGE == null)
                    _clsSAGE = new SAGEWriter(APPID, PROGRAMNAME, APPVERSION, ERPUSER, ERPPASSWORD, COMPANYID, SYSTEMID);
                Response = _clsSAGE.OpenSession();
                if (Response.ToUpper().Contains("ERROR"))
                    AddLogMessage(true, "Opening session: " + Response);
                else
                    AddLogMessage(false, "Opening session: " + Response);


            }
            catch (Exception ex)
            {
                Response = "Local ERROR on session open: " + ex.Message;
                AddLogMessage(true, Response);
            }
            return Response;
        }

        

        public List<string> ReadViews()
        {
            List<string> ReturnList = new List<string>();
            try
            {
                if (_clsSAGE == null)
                    OpenReader();

                ReturnList = _clsSAGE.ReturnData();
                string Response = ReturnList.Count.ToString() + " rows returned.";
                if (ReturnList.Count == 1)
                    Response = ReturnList[0];
                if (Response.ToUpper().Contains("ERROR"))
                    AddLogMessage(true, "Read views: " + Response);
                else
                    AddLogMessage(false, "Read views: " + Response);

            }
            catch (Exception ex)
            {
                AddLogMessage(true, "Read views: " + ex.Message);
            }
            return ReturnList;
        }

        public string InsertCustomer()
        {
            string Response = "";
            try
            {
                if (_clsSAGE == null)
                    OpenReader();
                Response = _clsSAGE.CreateCustomer();
            }
            catch (Exception ex)
            {
                Response = "Local ERROR: " + ex.Message;
            }
            return Response;
        }


       

        public string UpdateBatch(string BatchNumber)
        {
            string Response = "";
            try
            {
                if (_clsSAGE == null)
                    OpenReader();

                Response = _clsSAGE.UpdateBatch(BatchNumber);

            }
            catch (Exception ex)
            {
                Response = "Local ERROR updating batch: " + ex.Message;
            }
            if (Response.ToUpper().Contains("ERROR"))
                AddLogMessage(true, "Update batch: " + Response);
            else
                AddLogMessage(false, "update batch: " + Response);
            return Response;
        }

        public string PostBatch(string BatchNumber)
        {
            string Response = "";
            try
            {
                if (_clsSAGE == null)
                    OpenReader();

                Response = _clsSAGE.PostBatch(BatchNumber);

            }
            catch (Exception ex)
            {
                Response = "Local ERROR posting batch: " + ex.Message;
            }
            if (Response.ToUpper().Contains("ERROR"))
                AddLogMessage(true, "Post batch: " + Response);
            else
                AddLogMessage(false, "Post batch: " + Response);
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
