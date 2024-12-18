using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

using ACCPAC.Advantage;

namespace SAGE_Drivers
{
    public class SAGEWriter
    {
        
        private ACCPAC.Advantage.Session sessionObj;
        private ACCPAC.Advantage.DBLink DBLinkCmpRW;
        //private AccpacCOMAPI.AccpacSessionClass sessionObj;

        //private AccpacCOMAPI.AccpacDBLinkClass DBLinkCmpRW;
        private string sessionHandle = "";
        private string appID = "XX";
        private string programName = "XX1000";
        private string appVersion = "55A";
        private string userID = "ADMIN";
        private string userPassword = "ADMIN";
        private string companyID = "SAMINC";
        private string systemID = "SAMSYS";

        //private ACCPAC.Advantage.View OEORD1header;


        public string LastErrorMessage { get; set; }

        public SAGEWriter(string InpappID, string InpprogramName, string InpappVersion, string InpuserID, string InpuserPassword, string InpcompanyID, string InpsystemID)
        {
            appID = InpappID;
            programName = InpprogramName;
            appVersion = InpappVersion;
            userID = InpuserID;
            userPassword = InpuserPassword;
            companyID = InpcompanyID;
            systemID = InpsystemID;
        }

        public string OpenSession()
        {
            string SessionResponse = "";
            string Step = "";
            string SessionErrors = "";
            LastErrorMessage = "";
            try
            {

                //using ACCPAC.Advantage;
                if (sessionObj == null)
                {
                    Step = "Defining new session";
                    sessionObj = new ACCPAC.Advantage.Session();
                    Step = "Initialising session";
                    sessionObj.Init(sessionHandle, appID, programName, appVersion);
                    Step = "Opening session";
                    sessionObj.Open(userID, userPassword, companyID, DateTime.Today, 0);
                }
                SessionResponse = "Successfully opened session";
            }
            catch (SessionException sx)
            {
                SessionResponse = "Open Session ERROR at step : " + Step + ". " + sx.Message;
                LastErrorMessage = SessionResponse;
            }
            catch (Exception ex)
            {
                if (sessionObj != null && sessionObj.Errors != null)
                {
                    SessionErrors = " ErrorCount = " + sessionObj.Errors.Count.ToString() + " : ";
                    for (int e = 0; e < sessionObj.Errors.Count; e++)
                    {
                        SessionErrors += "\n - " + sessionObj.Errors[e].Message;
                    }
                }

                SessionResponse = "ERROR opening session at step : " + Step + ". " + SessionErrors + " " + ex.Message;
                LastErrorMessage = SessionResponse;
            }
            //session.    .Init("", "XY", "XY1000", "62A");
            
            return SessionResponse;
        }

        /// <summary>
        ///initDBLinks creates an Accpac session object, initialize and the open the session.
        ///This session object is also used to obtain an Accpac DBLink with read & write access
        /// permissions.
        /// </summary>
        public string initDBLinks()
        {
            string Response = "";
            try
            {
                if (sessionObj == null)
                    OpenSession();
                DBLinkCmpRW = sessionObj.OpenDBLink(ACCPAC.Advantage.DBLinkType.Company, ACCPAC.Advantage.DBLinkFlags.ReadWrite);

                Response = "Successfully opened link";


            }
            catch (Exception ex)
            {
                string SessionErrors = "";
                if (sessionObj != null && sessionObj.Errors != null)
                {
                    SessionErrors = " ErrorCount = " + sessionObj.Errors.Count.ToString() + " : ";
                    for (int e = 0; e < sessionObj.Errors.Count; e++)
                    {
                        SessionErrors += "\n - " + sessionObj.Errors[e].Message + ". " + SessionErrors;
                    }
                }
                Response = "ERROR on initialise " + ex.Message;
            }
            return Response;
        }

        public List<string> ReturnData()
        {
            List<string> CustomerList = new List<string>();
            string Step = "";
            try
            {
                View vARBTA = DBLinkCmpRW.OpenView("AR0041");
                Step = "Fetching first";
                bool gotOne = vARBTA.GoTop();
                String BatchNo;
                String BatchStatus;
                while (gotOne)
                {
                    Step = "Reading batch number";
                    BatchNo = vARBTA.Fields.FieldByName("CNTBTCH").Value.ToString();
                    Step = "Reading batch status";
                    BatchStatus = vARBTA.Fields.FieldByName("BATCHSTAT").Value.ToString();

                    CustomerList.Add(BatchNo + ", " + BatchStatus);
                    gotOne = vARBTA.GoNext();
                }





                //ACCPAC.Advantage.View arCustView = DBLinkCmpRW.OpenView("AR0024");
                //bool gotOne = arCustView.GoTop();
                //String custNum;
                //String custName;
                //while (gotOne)
                //{
                //    custNum = (String)arCustView.Fields.FieldByName("IDCUST").Value;
                //    custName = (String)arCustView.Fields.FieldByName("NAMECUST").Value;
                //    string CustomerRecord = "Customer Number: " + custNum + ",  Customer Name: " + custName;
                //    CustomerList.Add(CustomerRecord);
                //    gotOne = arCustView.GoNext();
                //}


            }
            catch (Exception ex)
            {
                CustomerList.Add("ERROR at step: " + Step + " - " + ex.Message);
            }

            return CustomerList;
        }

        public decimal ReturnInvoiceDue(string FilterCustomer, string FilterInvoice)
        {
            string Step = "";
            String InvoiceDueStr = "";
            string InvAmountStr = "";
            string InvPaidStr = "";
            decimal InvoiceDue = -9999;
            decimal InvAmount;
            decimal InvPaid;
            string InvoiceNo = "";
            bool gotOne = false;
            int RecordCount = 0;
            LastErrorMessage = "";
            try
            {
                
                try
                {
                    View vAROBL = DBLinkCmpRW.OpenView("AR0036");
                    Step = "Setting filter";
                    vAROBL.Browse("IDINVC = " + FilterInvoice.Trim() + " And TRXTYPETXT = 1", true);
                    Step = "Fetching first";
                    InvoiceNo = "";
                    gotOne = vAROBL.GoTop();
                    RecordCount = 0;
                    while (gotOne)
                    {
                        Step = "Reading invoice number";

                        InvoiceNo = vAROBL.Fields.FieldByName("IDINVC").Value.ToString();
                        string InvoiceType = vAROBL.Fields.FieldByName("TRXTYPETXT").Value.ToString();
                        if (InvoiceNo.ToUpper().Trim() == FilterInvoice.ToUpper().Trim() && InvoiceType.Trim() == "1")
                        {
                            Step = "Found invoice, reading invoice due";
                            InvoiceDueStr = vAROBL.Fields.FieldByName("AMTDUETC").Value.ToString();
                            InvAmountStr = vAROBL.Fields.FieldByName("AMTINVCTC").Value.ToString(); 

                            try
                            {
                                if(vAROBL.Fields.FieldByName("AMTREMIT").Value != null)
                                    InvPaidStr = vAROBL.Fields.FieldByName("AMTREMIT").Value.ToString();
                            }
                            catch { }
                            gotOne = false;
                        }
                        else
                            gotOne = vAROBL.GoNext();

                        if(RecordCount > 10000)
                        {
                            //Timeout - too many searches
                            gotOne = false;
                        }
                        RecordCount++;
                    }

                    //Check due
                    
                    if (InvoiceDueStr == "" && InvAmountStr != "")
                    {
                        decimal.TryParse(InvAmountStr, out InvAmount);
                        if (InvPaidStr != "")
                            decimal.TryParse(InvPaidStr, out InvPaid);
                        else
                            InvPaid = 0;
                        
                        InvoiceDue = InvAmount - InvPaid;

                        InvoiceDueStr = InvAmountStr;
                    }
                    else
                        decimal.TryParse(InvoiceDueStr, out InvoiceDue);
                }
                catch(Exception ex1)
                {
                    LastErrorMessage = "Error at step: " + Step + ". " + ex1.Message + ". ";
                }

                if (InvoiceDueStr == "")
                {

                    View vARBTA = DBLinkCmpRW.OpenView("AR0032");
                    Step = "Getting invoice ARBTA";
                    vARBTA.Browse("IDINVC = " + FilterInvoice.Trim(), true);
                    gotOne = vARBTA.GoTop();
                    string CustomerNumber;
                    InvoiceNo = "";
                    RecordCount = 0;
                    while (gotOne)
                    {
                        Step = "Reading ARBTA invoice number";
                        CustomerNumber = vARBTA.Fields.FieldByName("IDCUST").Value.ToString();
                        InvoiceNo = vARBTA.Fields.FieldByName("IDINVC").Value.ToString();
                        if (InvoiceNo.ToUpper().Trim() == FilterInvoice.ToUpper().Trim())
                        {
                            Step = "Found ARBTA invoice, reading invoice due";
                            InvoiceDueStr = vARBTA.Fields.FieldByName("AMTDUE").Value.ToString();
                            InvAmountStr = vARBTA.Fields.FieldByName("AMTNETTOT").Value.ToString();
                            gotOne = false;
                        }
                        else
                            gotOne = vARBTA.GoNext();

                        if (RecordCount > 10000)
                        {
                            //Timeout - too many searches
                            gotOne = false;
                        }
                        RecordCount++;
                    }
                    //Check due

                    if (InvoiceDueStr == "" && InvAmountStr != "")
                    {
                        decimal.TryParse(InvAmountStr, out InvAmount);

                        InvoiceDue = InvAmount;

                    }
                    else
                        decimal.TryParse(InvoiceDueStr, out InvoiceDue);
                }

            }
            catch (Exception ex)
            {
                LastErrorMessage += "Error at step: " + Step + ". " + ex.Message;
            }

            return InvoiceDue;
        }

        public string CreateCustomer()
        {
            string Response = "";
            string Step = "";
            try
            {
                ACCPAC.Advantage.View arCustView = DBLinkCmpRW.OpenView("AR0024");
                Step = "1";
                arCustView.Compose(new View[] { arCustView });
                Step = "2";
                arCustView.RecordCreate(ViewRecordCreate.Insert);
                Step = "3";
                arCustView.Fields.FieldByName("IDCUST").SetValue("TEST01", false);
                arCustView.Fields.FieldByName("NAMECUST").SetValue("Test Customer Add", false);
                Step = "4";
                arCustView.Insert();

                Response = "Customer Added";
            }
            catch (Exception ex)
            {
                Response = "ERROR in customer add at step:  " + Step + " - " + ex.Message;
            }

            return Response;
        }
        public string InsertBatchWithPrint(string CustomerCode, string InvoiceNumber, decimal PaymentAmount, decimal AppliedAmount)
        {
            string Response = "";
            String Step = "";
            try
            {
                Step = "Setting up views";
                Step += " vARBTA";
                View vARBTA = DBLinkCmpRW.OpenView("AR0041");
                Step += " vARTCR";
                View vARTCR = DBLinkCmpRW.OpenView("AR0042");
                Step += " vARTCP";
                View vARTCP = DBLinkCmpRW.OpenView("AR0044");
                Step += " vARTCU";
                View vARTCU = DBLinkCmpRW.OpenView("AR0045");
                Step += " vARPOOP";
                View vARPOOP = DBLinkCmpRW.OpenView("AR0061");
                //Step += " vARTCRO";
                //View vARTCRO = DBLinkCmpRW.OpenView("AR406");
                Step += " vARTCN";
                View vARTCN = DBLinkCmpRW.OpenView("AR0043");
                Step += " vARPYPT";
                View vARPYPT = DBLinkCmpRW.OpenView("AR0049");

                Step = "Composing views";
                Step += " vARBTA";
                vARBTA.Compose(new View[] { vARTCR });
                Step += " vARTCR";
                vARTCR.Compose(new View[] { vARBTA, vARTCN, vARTCP }); // Array(vARBTA, vARTCN, vARTCP, vARTCRO)
                Step += " vARTCP";
                vARTCP.Compose(new View[] { vARTCR, vARTCU, vARPOOP });// Array(vARTCR, vARTCU, vARPOOP)
                Step += " vARTCU";
                vARTCU.Compose(new View[] { vARTCP });//Array(vARTCP)
                Step += " vARTCN";
                vARTCN.Compose(new View[] { vARTCR });//Array(vARTCR)
                Step += " vARPOOP";
                vARPOOP.Compose(new View[] { vARBTA, vARTCR, vARTCN, vARTCP, vARTCU });//Array(vARBTA, vARTCR, vARTCN, vARTCP, vARTCU)
                //Step += " vARTCRO";
                //vARTCRO.Compose(new View[] { vARTCR });//Array(vARTCR)

                Step = "Creating Batch";
                // Set entry values for batch
                Step += " 1";

                // 
                //vARBTA.Unlock();
                //

                Step += " 2";
                vARBTA.Fields.FieldByName("CODEPYMTYP").SetValue("CA", false);   // Batch Type
                Step += " 3";

                vARBTA.RecordCreate(ViewRecordCreate.Insert);
                Step += " 4";
                vARBTA.Fields.FieldByName("BATCHDESC").SetValue("AUTOMATED 1", false);

                Step += " 5";
                vARBTA.Update();

                Step = "Creating Receipt";
                Step += " 1";
                vARTCR.RecordCreate(ViewRecordCreate.DelayKey);
                Step += " 2";
                vARTCR.Fields.FieldByName("IDCUST").SetValue(CustomerCode, false);   //  Customer Number
                //Step += " 3";
                //vARTCR.Fields.FieldByName("AMTPAYMTC").SetValue(PaymentAmount, false);
                Step += " 4";
                vARTCR.Fields.FieldByName("AMTRMIT").SetValue(PaymentAmount, false);
                Step += " 5";
                vARTCR.Fields.FieldByName("RMITTYPE").SetValue(int.Parse("1"), false);
                Step += " 6";
                vARTCR.Fields.FieldByName("TXTRMITREF").SetValue("TEST AUT", false);
                //vARTCR.Process();

                Step = "Creating Receipt Detail. ";
                vARTCP.RecordCreate(ViewRecordCreate.DelayKey);
                vARTCP.Fields.FieldByName("IDCUST").SetValue(CustomerCode, false);   //  Customer Number
                vARTCP.Fields.FieldByName("IDINVC").SetValue(InvoiceNumber, false);   //  Invoice
                vARTCP.Fields.FieldByName("AMTPAYM").SetValue(PaymentAmount, false);
                vARTCP.Process();
                vARTCP.Insert();
                vARTCR.Insert();

                Response = "Payment receipt created. ";

                //Aug 24 - Print receipt requirement
                //See ref: http://sageaom.kcsvar.com/AOM2020/AR0041.xml
                //SWPRINTED   25  Integer Batch Printed Flag  E A     List: 2 entries
                //0 = No
                //1 = Yes
                //e.g. vARBTA.Fields.FieldByName("SWPRINTED").SetValue("1", false);

                //Also investigate if print is required in receipt level:
                // See http://sageaom.kcsvar.com/AOM2020/AR0042.xml
                //SWPRINTED	68	Integer	Receipt Printed	E A 	List:2 entries
                // 0 = No
                //1 = Yes
                //e.g. vARTCR.Fields.FieldByName("SWPRINTED").SetValue("1", false);

                //Get batch number
                string BatchNo = "";
                Step = "Getting batch number.";
                BatchNo = vARBTA.Fields.FieldByName("CNTBTCH").Value.ToString();
                
                Step = "Getting deposit slip number.";
                string DepSlipNumber = vARBTA.Fields.FieldByName("DEPSTNBR").Value.ToString();
                
                Step = "Getting Bank code.";
                string BankCode = vARBTA.Fields.FieldByName("IDBANK").Value.ToString();
                

                //Print receipt

                string YearString = DateTime.Today.Year.ToString();
                string MonthString = DateTime.Today.Month.ToString();
                if (MonthString.Length < 2)
                    MonthString = "0" + MonthString;
                string DayString = DateTime.Today.Day.ToString();
                if (DayString.Length < 2)
                    DayString = "0" + DayString;
                string DateToday = YearString + "-" + MonthString + "-" + DayString;

                Step = "Setting up ARPRTUPDT view";
                
                View vARPRTUPDT = DBLinkCmpRW.OpenView("AR0076");
                Step = "Adding ARPRTUPDT values";
                vARPRTUPDT.Fields.FieldByName("CMNDCODE").SetValue("30", false);
                vARPRTUPDT.Fields.FieldByName("CNTBTCHFRM").SetValue(BatchNo, false);
                vARPRTUPDT.Fields.FieldByName("CNTBTCHTO").SetValue(BatchNo, false);
                Step = "Adding date";
                //if (IncludeDate)
                    vARPRTUPDT.Fields.FieldByName("DATEFROM").SetValue(DateToday, false);
                vARPRTUPDT.Fields.FieldByName("TYPEENTER").SetValue("1", false);
                vARPRTUPDT.Fields.FieldByName("TYPEIMPORT").SetValue("1", false);
                vARPRTUPDT.Fields.FieldByName("TYPEGENRTD").SetValue("1", false);
                vARPRTUPDT.Fields.FieldByName("TYPEEXTERN").SetValue("1", false);
                vARPRTUPDT.Fields.FieldByName("STTSOPEN").SetValue("1", false);
                vARPRTUPDT.Fields.FieldByName("STTSRDYTPT").SetValue("1", false);
                vARPRTUPDT.Fields.FieldByName("STTSPOSTED").SetValue("1", false);
                Step = "Processing ARPRTUPDT";
                
                vARPRTUPDT.Process();

                //Print deposit slip
                Step = "Setting up BKPRDEP view";
                

                View vBKPRDEP = DBLinkCmpRW.OpenView("BK0765");
                Step = "Adding values to  BKPRDEP";
                

                vBKPRDEP.Fields.FieldByName("SRCEAPP").SetValue("AR", false);
                vBKPRDEP.Fields.FieldByName("BANK").SetValue(BankCode, false);
                vBKPRDEP.Fields.FieldByName("DEPFROM").SetValue(DepSlipNumber, false);
                vBKPRDEP.Fields.FieldByName("DEPTO").SetValue(DepSlipNumber, false);

                Step = "Processing BKPRDEP";
                vBKPRDEP.Process();

                Step = "Updating Ready to Post";
                string BatchUpdateResult = UpdateBatch(BatchNo, true);
                if(BatchUpdateResult != null)
                    Response = "Completed all steps. Batch Number = " + BatchNo;
                

            }
            catch (Exception ex)
            {
                if (sessionObj == null)
                {
                    Response = "ERROR: Session not open. " + ex.Message;
                }
                else
                {
                    string StrErrors = " ErrorCount = " + sessionObj.Errors.Count.ToString() + " : ";
                    for (int e = 0; e < sessionObj.Errors.Count; e++)
                    {
                        StrErrors += "\n - " + sessionObj.Errors[e].Message;
                    }
                    Response = "ERROR: at " + Step + " - " + ex.GetBaseException().Message + " Session Error = " + StrErrors;
                }

            }

            return Response;

        }
            public string InsertBatch(string CustomerCode, string InvoiceNumber, decimal PaymentAmount, decimal AppliedAmount)
        {
            string Response = "";
            String Step = "";
            try
            {
                
                decimal UnappliedAmount = PaymentAmount - AppliedAmount;
                if (InvoiceNumber == "")
                    UnappliedAmount = PaymentAmount;
                Step = "Setting up views";

                Step += " vARBTA";
                View vARBTA = DBLinkCmpRW.OpenView("AR0041");
                Step += " vARTCR";
                View vARTCR = DBLinkCmpRW.OpenView("AR0042");
                Step += " vARTCP";
                View vARTCP = DBLinkCmpRW.OpenView("AR0044");
                Step += " vARTCU";
                View vARTCU = DBLinkCmpRW.OpenView("AR0045");
                Step += " vARPOOP";
                View vARPOOP = DBLinkCmpRW.OpenView("AR0061");
                //Step += " vARTCRO";
                //View vARTCRO = DBLinkCmpRW.OpenView("AR406");
                Step += " vARTCN";
                View vARTCN = DBLinkCmpRW.OpenView("AR0043");
                Step += " vARPYPT";
                View vARPYPT = DBLinkCmpRW.OpenView("AR0049");

                Step = "Composing views";
                Step += " vARBTA";
                vARBTA.Compose(new View[] { vARTCR });
                
                Step += " vARTCR";
                vARTCR.Compose(new View[] { vARBTA, vARTCN, vARTCP }); // Array(vARBTA, vARTCN, vARTCP, vARTCRO)
                Step += " vARTCP";
                vARTCP.Compose(new View[] { vARTCR, vARTCU, vARPOOP });// Array(vARTCR, vARTCU, vARPOOP)
                Step += " vARTCU";
                vARTCU.Compose(new View[] { vARTCP });//Array(vARTCP)
                Step += " vARTCN";
                vARTCN.Compose(new View[] { vARTCR });//Array(vARTCR)
                Step += " vARPOOP";
                vARPOOP.Compose(new View[] { vARBTA, vARTCR, vARTCN, vARTCP, vARTCU });//Array(vARBTA, vARTCR, vARTCN, vARTCP, vARTCU)
                
                
                //Step += " vARTCRO";
                //vARTCRO.Compose(new View[] { vARTCR });//Array(vARTCR)

                Step = "Creating Batch";
                // Set entry values for batch
                Step += " 1";

                // 
                //vARBTA.Unlock();
                //

                Step += " 2";
                vARBTA.Fields.FieldByName("CODEPYMTYP").SetValue("CA", false);   // Batch Type
                Step += " 3";

                vARBTA.RecordCreate(ViewRecordCreate.Insert);
                Step += " 4";
                vARBTA.Fields.FieldByName("BATCHDESC").SetValue("AUTOMATED 1", false);

                Step += " 5";
                vARBTA.Update();

                Step = "Creating Receipt";
                Step += " 1";
                vARTCR.RecordCreate(ViewRecordCreate.DelayKey);
                Step += " 2";
                vARTCR.Fields.FieldByName("IDCUST").SetValue(CustomerCode, false);   //  Customer Number
                //Step += " 3";
                //vARTCR.Fields.FieldByName("AMTPAYMTC").SetValue(PaymentAmount, false);
                Step += " 4";
                vARTCR.Fields.FieldByName("AMTRMIT").SetValue(PaymentAmount , false);
                Step += " 5";
                //if (InvoiceNumber.Length > 1)
                vARTCR.Fields.FieldByName("RMITTYPE").SetValue(int.Parse("1"), false); //Receipt
                //else
                //    vARTCR.Fields.FieldByName("RMITTYPE").SetValue(int.Parse("3"), false); //Unapplied cash
                Step += " 6";
                vARTCR.Fields.FieldByName("TXTRMITREF").SetValue("EZYCOLLECT", false);
                //vARTCR.Process();
                if (InvoiceNumber.Length > 1 && AppliedAmount > 0)
                {
                    //Only if an invoice needs to be applied
                    Step = "Creating Receipt Detail";
                    vARTCP.RecordCreate(ViewRecordCreate.DelayKey);
                    vARTCP.Fields.FieldByName("IDCUST").SetValue(CustomerCode, false);   //  Customer Number
                    vARTCP.Fields.FieldByName("IDINVC").SetValue(InvoiceNumber, false);   //  Invoice
                    vARTCP.Fields.FieldByName("AMTPAYM").SetValue(AppliedAmount, false);
                    vARTCP.Process();
                    vARTCP.Insert();
                }
                else
                {
                    Step = "Setting up unapplied handling";
                    vARTCP.RecordClear();
                    vARTCP.Fields.FieldByName("CNTLINE").SetValue(-1, false);   //  Empty lines
                    
                }

                if (UnappliedAmount > 0)
                {
                    Step += " 6.5";
                    vARTCR.Fields.FieldByName("REMUNAPL").SetValue(UnappliedAmount, false);
                }

                Step += " 7";
                vARTCR.Insert();

                Step = "Updating Batch To Ready To Process";
                vARBTA.Fields.FieldByName("BATCHSTAT").SetValue("7", false);
                vARBTA.Update();

                var vBatchNo = vARBTA.Fields.FieldByName("CNTBTCH").Value;
                Step = "Setting Post batch range";
                vARPYPT.Fields.FieldByName("BATCHIDFR").SetValue(vBatchNo, false);
                vARPYPT.Fields.FieldByName("BATCHIDTO").SetValue(vBatchNo, false);
                //Step = "Updating batch";
                //vARBTA.Update();

                Step = "Processing Batch ";
                vARPYPT.Process();



                Response = "Completed all steps";

                /*
                vARPOOP.Fields("PAYMTYPE").Value = vARBTA.Fields("CODEPYMTYP").Value ' Batch Type
               vARPOOP.Fields("CNTBTCH").Value = vARBTA.Fields("CNTBTCH").Value     ' Batch Number
               vARPOOP.Fields("CNTITEM").Value = vARTCR.Fields("CNTITEM").Value     ' Entry Number
               vARPOOP.Fields("IDCUST").Value = "1550"                      ' ID Customer
               vARPOOP.Fields("SHOWTYPE").Value = "2"                       ' Invoices only.
               vARPOOP.Process

                If False = vARPOOP.GoTop Then
                    Exit Sub
                End If

                Dim i As Long

                For i = 1 To numEntries
                    If vARPOOP.Fields("AMTDUE").Value <> 0 Then
                        vARPOOP.Fields("APPLY").Value = "Y"                           ' Apply
                       vARPOOP.Update
                    End If
                    If False = vARPOOP.GoNext Then
                        Exit For
                    End If
                Next i

                vARTCR.Fields("AMTRMIT").Value = -1 * vARTCR.Fields("REMUNAPL").Value                    ' Bank Receipt Amount
               vARTCR.Insert

                */

            }
            catch (Exception ex)
            {
                if (sessionObj == null)
                {
                    Response = "ERROR: Session not open. " + ex.Message;
                }
                else
                {
                    string StrErrors = " ErrorCount = " + sessionObj.Errors.Count.ToString() + " : ";
                    for (int e = 0; e < sessionObj.Errors.Count; e++)
                    {
                        StrErrors += "\n - " + sessionObj.Errors[e].Message;
                    }
                    Response = "ERROR: at " + Step + " - " + ex.GetBaseException().Message + " Session Error = " + StrErrors;
                }

            }

            return Response;
        }

        public string InsertCreditBatch(string CustomerCode, string InvoiceNumber, string CreditNoteNumber, decimal AppliedAmount)
        {
            string Response = "";
            String Step = "";
            try
            {

                
                Step = "Setting up views";

                Step += " vARBTA";
                View vARBTA = DBLinkCmpRW.OpenView("AR0041");
                Step += " vARTCR";
                View vARTCR = DBLinkCmpRW.OpenView("AR0042");
                Step += " vARTCP";
                View vARTCP = DBLinkCmpRW.OpenView("AR0044");
                Step += " vARTCU";
                View vARTCU = DBLinkCmpRW.OpenView("AR0045");
                Step += " vARPOOP";
                View vARPOOP = DBLinkCmpRW.OpenView("AR0061");
                //Step += " vARTCRO";
                //View vARTCRO = DBLinkCmpRW.OpenView("AR406");
                Step += " vARTCN";
                View vARTCN = DBLinkCmpRW.OpenView("AR0043");
                Step += " vARPYPT";
                View vARPYPT = DBLinkCmpRW.OpenView("AR0049");

                Step = "Composing views";
                Step += " vARBTA";
                vARBTA.Compose(new View[] { vARTCR });

                Step += " vARTCR";
                vARTCR.Compose(new View[] { vARBTA, vARTCN, vARTCP }); // Array(vARBTA, vARTCN, vARTCP, vARTCRO)
                Step += " vARTCP";
                vARTCP.Compose(new View[] { vARTCR, vARTCU, vARPOOP });// Array(vARTCR, vARTCU, vARPOOP)
                Step += " vARTCU";
                vARTCU.Compose(new View[] { vARTCP });//Array(vARTCP)
                Step += " vARTCN";
                vARTCN.Compose(new View[] { vARTCR });//Array(vARTCR)
                Step += " vARPOOP";
                vARPOOP.Compose(new View[] { vARBTA, vARTCR, vARTCN, vARTCP, vARTCU });//Array(vARBTA, vARTCR, vARTCN, vARTCP, vARTCU)


                //Step += " vARTCRO";
                //vARTCRO.Compose(new View[] { vARTCR });//Array(vARTCR)

                Step = "Creating Batch";
                // Set entry values for batch
                Step += " 1";

                // 
                //vARBTA.Unlock();
                //

                Step += " 2";
                vARBTA.Fields.FieldByName("CODEPYMTYP").SetValue("CA", false);   // Batch Type
                Step += " 3";

                vARBTA.RecordCreate(ViewRecordCreate.Insert);
                Step += " 4";
                vARBTA.Fields.FieldByName("BATCHDESC").SetValue("AUTOMATED 1", false);

                Step += " 5";
                vARBTA.Update();

                Step = "Creating Receipt";
                Step += " 1";
                vARTCR.RecordCreate(ViewRecordCreate.DelayKey);
                Step += " 2";
                vARTCR.Fields.FieldByName("IDCUST").SetValue(CustomerCode, false);   //  Customer Number
                //Step += " 3";
                //vARTCR.Fields.FieldByName("AMTPAYMTC").SetValue(PaymentAmount, false);
                //Step += " 4";
                //vARTCR.Fields.FieldByName("AMTRMIT").SetValue(PaymentAmount, false);
                Step += " 5";
                //if (InvoiceNumber.Length > 1)
                vARTCR.Fields.FieldByName("RMITTYPE").SetValue(int.Parse("1"), false); //Receipt
                //else
                //    vARTCR.Fields.FieldByName("RMITTYPE").SetValue(int.Parse("3"), false); //Unapplied cash
                Step += " 6";
                vARTCR.Fields.FieldByName("TXTRMITREF").SetValue("EZYCOLLECT", false);
                //vARTCR.Process();
                if (CreditNoteNumber.Length > 1 )
                {
                    //Only if an invoice needs to be applied
                    Step += "Creating Credit Line";
                    vARTCP.RecordCreate(ViewRecordCreate.DelayKey);
                    vARTCP.Fields.FieldByName("IDCUST").SetValue(CustomerCode, false);   //  Customer Number
                    vARTCP.Fields.FieldByName("IDINVC").SetValue(CreditNoteNumber, false);   //  Invoice
                    vARTCP.Fields.FieldByName("AMTPAYM").SetValue(AppliedAmount * -1, false);
                    vARTCP.Process();
                    vARTCP.Insert();
                }
                if (InvoiceNumber.Length > 1)
                {
                    //Only if an invoice needs to be applied
                    Step += "Creating Invoice Line";
                    vARTCP.RecordCreate(ViewRecordCreate.DelayKey);
                    vARTCP.Fields.FieldByName("IDCUST").SetValue(CustomerCode, false);   //  Customer Number
                    vARTCP.Fields.FieldByName("IDINVC").SetValue(InvoiceNumber, false);   //  Invoice
                    vARTCP.Fields.FieldByName("AMTPAYM").SetValue(AppliedAmount, false);
                    vARTCP.Process();
                    vARTCP.Insert();
                }

                Step += " 7";
                vARTCR.Insert();

                Step = "Updating Batch To Ready To Process";
                vARBTA.Fields.FieldByName("BATCHSTAT").SetValue("7", false);
                vARBTA.Update();

                var vBatchNo = vARBTA.Fields.FieldByName("CNTBTCH").Value;
                Step = "Setting Post batch range";
                vARPYPT.Fields.FieldByName("BATCHIDFR").SetValue(vBatchNo, false);
                vARPYPT.Fields.FieldByName("BATCHIDTO").SetValue(vBatchNo, false);
                //Step = "Updating batch";
                //vARBTA.Update();

                Step = "Processing Batch ";
                vARPYPT.Process();



                Response = "Completed all steps";

                /*
                vARPOOP.Fields("PAYMTYPE").Value = vARBTA.Fields("CODEPYMTYP").Value ' Batch Type
               vARPOOP.Fields("CNTBTCH").Value = vARBTA.Fields("CNTBTCH").Value     ' Batch Number
               vARPOOP.Fields("CNTITEM").Value = vARTCR.Fields("CNTITEM").Value     ' Entry Number
               vARPOOP.Fields("IDCUST").Value = "1550"                      ' ID Customer
               vARPOOP.Fields("SHOWTYPE").Value = "2"                       ' Invoices only.
               vARPOOP.Process

                If False = vARPOOP.GoTop Then
                    Exit Sub
                End If

                Dim i As Long

                For i = 1 To numEntries
                    If vARPOOP.Fields("AMTDUE").Value <> 0 Then
                        vARPOOP.Fields("APPLY").Value = "Y"                           ' Apply
                       vARPOOP.Update
                    End If
                    If False = vARPOOP.GoNext Then
                        Exit For
                    End If
                Next i

                vARTCR.Fields("AMTRMIT").Value = -1 * vARTCR.Fields("REMUNAPL").Value                    ' Bank Receipt Amount
               vARTCR.Insert

                */

            }
            catch (Exception ex)
            {
                if (sessionObj == null)
                {
                    Response = "ERROR: Session not open. " + ex.Message;
                }
                else
                {
                    string StrErrors = " ErrorCount = " + sessionObj.Errors.Count.ToString() + " : ";
                    for (int e = 0; e < sessionObj.Errors.Count; e++)
                    {
                        StrErrors += "\n - " + sessionObj.Errors[e].Message;
                    }
                    Response = "ERROR: at " + Step + " - " + ex.GetBaseException().Message + " Session Error = " + StrErrors;
                }

            }

            return Response;
        }

        public string UpdateBatch(string BatchNumber, bool PostBatch = false)
        {
            string Response = "";
            String Step = "";
            
            try
            {
                Step = "Setting up views";
                
                Step += " vARBTA";
                View vARBTA = DBLinkCmpRW.OpenView("AR0041");

                //vARBTA.FilterSelect("CNTBTCH = 47", true, 0, ViewFilterOrigin.FromStart);
                Step = "Looking for batch";
                bool gotOne = vARBTA.GoTop();

                while (gotOne)
                {
                    string BatchNo = vARBTA.Fields.FieldByName("CNTBTCH").Value.ToString();
                    if (BatchNo == BatchNumber)
                    {

                        Step = "Setting batch field";
                        vARBTA.Fields.FieldByName("BATCHSTAT").SetValue(7, false);
                        Step = "Updating batch";
                        vARBTA.Update();
                        
                        gotOne = false;
                        if (PostBatch)
                        {
                            Step += " vARPYPT";
                            View vARPYPT = DBLinkCmpRW.OpenView("AR0049");
                            vARPYPT.Fields.FieldByName("BATCHIDFR").SetValue(BatchNumber, false);
                            vARPYPT.Fields.FieldByName("BATCHIDTO").SetValue(BatchNumber, false);
                            //Step = "Updating batch";
                            //vARBTA.Update();

                            Step = "Processing Batch ";
                            vARPYPT.Process();
                            Step = "Batch posted successfully. ";
                            Response = "Batch posted successfully. ";
                        }

                    }

                    if (gotOne)
                        gotOne = vARBTA.GoNext();
                }

                if (Step == "Looking for batch")
                    Response = "ERROR: Batch record not found";



            }
            catch (Exception ex)
            {
                Response = "ERROR: Failed at step " + Step + " - " + ex.GetBaseException().Message;
            }

            return Response ;
        }



        public string PostBatch(string BatchNumber)
        {
            string Response = "";
            String Step = "";
            try
            {
                Step = "Setting up views";

                Step += " vARBTA";
                View vARBTA = DBLinkCmpRW.OpenView("AR0041");
                Step += " vARPYPT";
                View vARPYPT = DBLinkCmpRW.OpenView("AR0049");

                //vARBTA.FilterSelect("CNTBTCH = 47", true, 0, ViewFilterOrigin.FromStart);
                Step = "Looking for batch";
                bool gotOne = vARBTA.GoTop();

                while (gotOne)
                {
                    string BatchNo = vARBTA.Fields.FieldByName("CNTBTCH").Value.ToString();
                    if (BatchNo == BatchNumber)
                    {
                        var vBatchNo = vARBTA.Fields.FieldByName("CNTBTCH").Value;
                        vARPYPT.Fields.FieldByName("BATCHIDFR").SetValue(vBatchNo, false);
                        vARPYPT.Fields.FieldByName("BATCHIDTO").SetValue(vBatchNo, false);
                        //Step = "Updating batch";
                        //vARBTA.Update();

                        Step = "Processing Batch ";
                        vARPYPT.Process();
                        Step = "Completed ";
                    }

                    gotOne = vARBTA.GoNext();
                }

                if (Step == "Looking for batch")
                    Response = "Record not found";
                else
                    Response = Step;



            }
            catch (Exception ex)
            {
                string StrErrors = " ErrorCount = " + sessionObj.Errors.Count.ToString() + " : ";
                for (int e = 0; e < sessionObj.Errors.Count; e++)
                {
                    StrErrors += "\n - " + sessionObj.Errors[e].Message;
                }
                Response = "ERROR: Failed at step " + Step + " - " + ex.GetBaseException().Message + StrErrors;
            }

            return Response;
        }


        // Set up credit note payments
        public string InsertCreditNote(string CustomerCode, string InvoiceNumber,string CreditNoteRef)
        {
            string Response = "";
            String Step = "";
            try
            {

                
                Step = "Setting up views";

                Step += " vOEPOSTC";
                View vOEPOSTC = DBLinkCmpRW.OpenView("OE0560");
                Step += " vOECRDH";
                View vOECRDH = DBLinkCmpRW.OpenView("OE0240");
                Step += " vOECRDD";
                View vOECRDD = DBLinkCmpRW.OpenView("OE0220");
                Step += " vOECOINC";
                View vOECOINC = DBLinkCmpRW.OpenView("OE0140");
                Step += " vOECRDHO";
                View vOECRDHO = DBLinkCmpRW.OpenView("OE0242");

                /*
                 * OEPOSTC   OE0560
                 * 
                AR0031	ARIBC	Invoice Batches	ARIBC
AR0032	ARIBH	Invoices	ARIBH
AR0033	ARIBD	Invoice Details	ARIBD
AR0034	ARIBS	Invoice Payment Schedules	ARIBS
AR0402	ARIBHO	Invoice Optional Fields	ARIBHO
AR0401	ARIBDO	Invoice Detail Optional Fields	ARIBDO
GP0950		Transaction Posting	GPGLPO
                */
                Step = "Composing views";
                Step += " vOECRDH";
                vOECRDH.Compose(new View[] { vOECRDD, vOECOINC, vOECRDHO });
                


                Step = "Write fields";
                // Set entry values for batch
                Step += " 1 start create";

                vOECRDH.RecordCreate(ViewRecordCreate.DelayKey);

                Step += " 2";
                vOECRDH.Fields.FieldByName("INVNUMBER").SetValue(InvoiceNumber, false);   // Batch Type
                Step += " 3";

                Step += " 4";
                vOECRDH.Fields.FieldByName("CUSTOMER").SetValue(CustomerCode, false);

                Step += " 5";
                vOECRDH.Fields.FieldByName("CRDNUMBER").SetValue(CreditNoteRef, false);

                Step += " 6";
                vOECRDH.Fields.FieldByName("TAXGROUP").SetValue("CALIF", false);

                Step += " 7";
                vOECRDH.Fields.FieldByName("CUSACCTSET").SetValue("TRADE", false);
                

                Step += " Process";
                vOECRDH.Process();

                
                Step += " Insert";
                vOECRDH.Insert();

                

                Step = "Posting Batch ";
                vOECRDH.Post();

                


                Response = "Completed all steps";

                /*
                vARPOOP.Fields("PAYMTYPE").Value = vARBTA.Fields("CODEPYMTYP").Value ' Batch Type
                vARPOOP.Fields("CNTBTCH").Value = vARBTA.Fields("CNTBTCH").Value     ' Batch Number
                vARPOOP.Fields("CNTITEM").Value = vARTCR.Fields("CNTITEM").Value     ' Entry Number
                vARPOOP.Fields("IDCUST").Value = "1550"                      ' ID Customer
                vARPOOP.Fields("SHOWTYPE").Value = "2"                       ' Invoices only.
                vARPOOP.Process

                If False = vARPOOP.GoTop Then
                    Exit Sub
                End If

                Dim i As Long

                For i = 1 To numEntries
                    If vARPOOP.Fields("AMTDUE").Value <> 0 Then
                        vARPOOP.Fields("APPLY").Value = "Y"                           ' Apply
                        vARPOOP.Update
                    End If
                    If False = vARPOOP.GoNext Then
                        Exit For
                    End If
                Next i

                vARTCR.Fields("AMTRMIT").Value = -1 * vARTCR.Fields("REMUNAPL").Value                    ' Bank Receipt Amount
                vARTCR.Insert

                */

            }
            catch (Exception ex)
            {
                if (sessionObj == null)
                {
                    Response = "ERROR: Session not open. " + ex.Message;
                }
                else
                {
                    string StrErrors = " ErrorCount = " + sessionObj.Errors.Count.ToString() + " : ";
                    for (int e = 0; e < sessionObj.Errors.Count; e++)
                    {
                        StrErrors += "\n - " + sessionObj.Errors[e].Message;
                    }
                    Response = "ERROR: at " + Step + " - " + ex.GetBaseException().Message + " Session Error = " + StrErrors;
                }

            }

            return Response;
        }

        /// <summary>
        /// openAndComposeViews opens each required view for the order entry operation. Required views
        /// are referenced in the Accpac Object Model or recorded macro. These views must be opened
        /// and composed in the order defined. Do not change ordering of statements in this procedure.
        /// </summary>
        public string openAndComposeViews()
        {
            string Response = "";

            try
            {

                // Open the A/R Invoice Entry Views
                ACCPAC.Advantage.View arInvoiceBatch = DBLinkCmpRW.OpenView("AR0031");
                ACCPAC.Advantage.View arInvoiceHeader = DBLinkCmpRW.OpenView("AR0032");
                ACCPAC.Advantage.View arInvoiceDetail = DBLinkCmpRW.OpenView("AR0033");
                ACCPAC.Advantage.View arInvoicePaymentSchedules = DBLinkCmpRW.OpenView("AR0034");
                ACCPAC.Advantage.View arInvoiceHeaderOptFields = DBLinkCmpRW.OpenView("AR0402");
                ACCPAC.Advantage.View arInvoiceDetailOptFields = DBLinkCmpRW.OpenView("AR0401");

                // Compose the Batch, Header and Detail views together.
                arInvoiceBatch.Compose(new ACCPAC.Advantage.View[] { arInvoiceHeader });
                arInvoiceHeader.Compose(new ACCPAC.Advantage.View[] { arInvoiceBatch, arInvoiceDetail,
      arInvoicePaymentSchedules, arInvoiceHeaderOptFields });
                arInvoiceDetail.Compose(new ACCPAC.Advantage.View[] { arInvoiceHeader,
      arInvoiceBatch, arInvoiceDetailOptFields });
                arInvoicePaymentSchedules.Compose(new ACCPAC.Advantage.View[] {
      arInvoiceHeader });
                arInvoiceHeaderOptFields.Compose(new ACCPAC.Advantage.View[] { arInvoiceHeader });
                arInvoiceDetailOptFields.Compose(new ACCPAC.Advantage.View[] { arInvoiceDetail });




            }
            catch (Exception e)
            {
                Response = "ERROR: " + e.Message;
            }
            return Response;
        }



    }
}
