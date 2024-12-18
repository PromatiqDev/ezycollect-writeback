using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace SageComAPI
{
    public partial class Form1 : Form
    {

        //Parameters with defaults
        string APIURL = "";
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

        TestSageModule _clsSAGE;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnInsertReceipt_Click(object sender, EventArgs e)
        {
            APIURL = txtAppId.Text;
            string APIENDPOINT = "";
            string APIFILTER = "";
            string APIKEY = "";
            string ERPTYPE = "SAGE";
            string APPID = txtAppId.Text;
            string PROGRAMNAME = txtProgramName.Text;
            string APPVERSION = txtAppVersion.Text;
            string ERPUSER = txtUserId.Text;
            string ERPPASSWORD = txtPassword.Text;
            string COMPANYID = txtCompanyId.Text;
            string SYSTEMID = txtSystemId.Text;

            Decimal AmountToPay = 0;
            Decimal.TryParse(txtAmount.Text, out AmountToPay);
            if (AmountToPay == 0)
                MessageBox.Show("Amount is not valid");
            else
                WriteERPData("SAGE", txtCustomer.Text, txtInvoice.Text, AmountToPay);
        }

        

        private void btnMessageClear_Click(object sender, EventArgs e)
        {
            MessageList.Items.Clear();
            ClearErrorLog();
        }

        private void btnInitialise_Click(object sender, EventArgs e)
        {
            ClearErrorLog();
            InitialiseSession();
            DumpErrorLog();
        }

        private void btnOpenReader_Click(object sender, EventArgs e)
        {
            ClearErrorLog();
            OpenReader();
            DumpErrorLog();
        }

        private void btnReadViews_Click(object sender, EventArgs e)
        {
            ClearErrorLog();
            ReadViews();
            DumpErrorLog();
        }

        private void DumpErrorLog()
        {
            try
            {
                foreach(string Message in _clsSAGE.LogEvents)
                {
                    MessageList.Items.Add(Message);
                }
            }
            catch
            {

            }
        }

        private void ClearErrorLog()
        {
            try
            {
                _clsSAGE.LogEvents.Clear();

            }
            catch
            {

            }
        }

        private void InitialiseSession()
        {
            string Response = "";
            try
            {
                _clsSAGE = null;
                _clsSAGE = new TestSageModule(txtAppId.Text, txtProgramName.Text, txtAppVersion.Text, txtUserId.Text, txtPassword.Text, txtCompanyId.Text, txtSystemId.Text);
                Response = _clsSAGE.InitialiseSession();

            }
            catch (Exception ex)
            {
                Response = "Local ERROR: " + ex.Message;
            }
            MessageList.Items.Add(Response);
        }

        private void OpenReader()
        {
            string Response = "";
            try
            {
                if (_clsSAGE == null)
                    InitialiseSession();
                Response = _clsSAGE.OpenReader();
            }
            catch (Exception ex)
            {
                Response = "Local ERROR: " + ex.Message;
            }
            MessageList.Items.Add(Response);
        }


        private void ReadViews()
        {
            string Response = "";
            try
            {
                if (_clsSAGE == null)
                    OpenReader();

                List<string> ReturnList = _clsSAGE.LogEvents;
                foreach (string ListRow in ReturnList)
                {
                    MessageList.Items.Add(ListRow);


                }

            }
            catch (Exception ex)
            {
                Response = "Local ERROR: " + ex.Message;
            }
            MessageList.Items.Add(Response);
        }

        private void InsertCustomer()
        {
            string Response = "";
            try
            {
                if (_clsSAGE == null)
                    OpenReader();
                Response = _clsSAGE.InsertCustomer();
            }
            catch (Exception ex)
            {
                Response = "Local ERROR: " + ex.Message;
            }
            MessageList.Items.Add(Response);
        }


        // ***********************************************************
        //**  ERP writes              ********************************
        //************************************************************
        private string WriteERPData(string ERPType, string CustomerCode, string Invoice, decimal Value)
        {
            string Response = "";
            try
            {
                ClearErrorLog();
                switch (ERPType.ToUpper())
                {
                    case "SAGE":

                        _clsSAGE = new TestSageModule(APPID, PROGRAMNAME, APPVERSION, ERPUSER, ERPPASSWORD, COMPANYID, SYSTEMID);
                        Response = _clsSAGE.InsertReceipt(CustomerCode, Invoice, Value, chkPrint.Checked);

                        List<string> DebugLog = _clsSAGE.LogEvents;
                        if (DebugLog.Count > 0)
                        {
                            foreach (string LogEvent in DebugLog)
                            {
                                MessageList.Items.Add(LogEvent);
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

        private void btnReadInvoice_Click(object sender, EventArgs e)
        {
            string Response = "";
            try
            {
                if (_clsSAGE == null)
                    OpenReader();

                decimal DueAmount = _clsSAGE.GetInvoiceOutstanding(txtCustomer.Text, txtInvoice.Text);
                if (DueAmount < -999)
                    Response = "Unable to get the due amount: " + _clsSAGE.GetLastError();
                else
                    Response = "Outstanding = " + DueAmount.ToString();

            }
            catch (Exception ex)
            {
                Response = "Local ERROR: " + ex.Message;
            }
            MessageList.Items.Add(Response);
        }

        private void btnInsertCreditNote_Click(object sender, EventArgs e)
        {
            string Response = "";
            try
            {
                if (_clsSAGE == null)
                    OpenReader();

                decimal DueAmount = 0;
                decimal.TryParse(txtAmount.Text,out DueAmount);

                Response = _clsSAGE.InsertCreditNote(txtCustomer.Text, txtInvoice.Text,txtCreditNote.Text, DueAmount);
                

            }
            catch (Exception ex)
            {
                Response = "Local ERROR: " + ex.Message;
            }
            MessageList.Items.Add(Response);
        }

        private void btnApplyCN_Click(object sender, EventArgs e)
        {
            APIURL = txtAppId.Text;
            string APIENDPOINT = "";
            string APIFILTER = "";
            string APIKEY = "";
            string ERPTYPE = "SAGE";
            string APPID = txtAppId.Text;
            string PROGRAMNAME = txtProgramName.Text;
            string APPVERSION = txtAppVersion.Text;
            string ERPUSER = txtUserId.Text;
            string ERPPASSWORD = txtPassword.Text;
            string COMPANYID = txtCompanyId.Text;
            string SYSTEMID = txtSystemId.Text;

            Decimal AmountToPay = 0;
            Decimal.TryParse(txtAmount.Text, out AmountToPay);
            if (AmountToPay == 0)
                MessageBox.Show("Amount is not valid");
            else
                WriteERPCNReceipt("SAGE", txtCustomer.Text, txtInvoice.Text,txtCN.Text, AmountToPay);

        }

        private string WriteERPCNReceipt(string ERPType, string CustomerCode, string Invoice,string CreditNoteNumber, decimal Value)
        {
            string Response = "";
            try
            {
                ClearErrorLog();
                switch (ERPType.ToUpper())
                {
                    case "SAGE":

                        _clsSAGE = new TestSageModule(APPID, PROGRAMNAME, APPVERSION, ERPUSER, ERPPASSWORD, COMPANYID, SYSTEMID);
                        Response = _clsSAGE.InsertCreditReceipt(CustomerCode, Invoice, CreditNoteNumber, Value);

                        List<string> DebugLog = _clsSAGE.LogEvents;
                        if (DebugLog.Count > 0)
                        {
                            foreach (string LogEvent in DebugLog)
                            {
                                MessageList.Items.Add(LogEvent);
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
    }
}
