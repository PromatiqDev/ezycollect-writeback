using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;

using CommonClass;
using APILib;
using System.Drawing.Printing;



namespace Writeback_UI
{
    public partial class Mainform : Form
    {
        ConfigManager _clsConfig = new ConfigManager();
        APIClass _clsAPI = new APIClass();
        DataTable dtConfigSettings = new DataTable();

        //Parameters with defaults
        string APIURL = "";
        string APIENDPOINT = "events/consume";
        string APIFILTER = "status=FINAL&type=ACCEPTED&skip=0&limit=50";
        string APIKEY = "";
        string ERPTYPE = "SAGE";
        string APPID = "XX";
        string PROGRAMNAME = "XX1000";
        string APPVERSION = "55A";
        string ERPUSER = "";
        string ERPPASSWORD = "";
        string COMPANYID = "SAMINC";
        string SYSTEMID = "SAMSYS";
        string PRINTOPTION = "1";

        string ConfigFolder = "";
        string LastRunCommand = "";

        BackgroundWorker BackgroundTask = new BackgroundWorker();

        public Mainform()
        {
            InitializeComponent();
            try
            {
                BackgroundTask.DoWork +=
                new DoWorkEventHandler(BackgroundTask_DoWork);
                BackgroundTask.RunWorkerCompleted +=
                        new RunWorkerCompletedEventHandler(
                    BackgroundTask_RunWorkerCompleted);
            }
            catch
            {

            }
        }

        private void Mainform_Load(object sender, EventArgs e)
        {
            try
            {
                //Get default folder if not supplied (Program Data)
                if (ConfigFolder == "")
                {
                    ConfigFolder = _clsConfig.GetAppFolder("");

                }

                GetConfigSettings();
                toolStripStatusLabel1.Text = "V " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
            catch(Exception ex) 
            {
                MessageBox.Show(ex.Message);
                
            }
            

        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            CheckAPISettings();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveConfigSettings();
        }

        




        #region Configuration manager


        // ***********************************************************
        //**  config data manager ************************************
        //************************************************************
        private void GetConfigSettings()
        {
            try
            {
                string ConfigFile = ConfigFolder;
                if (!ConfigFile.EndsWith("\\"))
                    ConfigFile += "\\";
                ConfigFile += "Config.json";
                dtConfigSettings = _clsConfig.ReadTablefromJsonFile(ConfigFile);
                if (dtConfigSettings.Rows.Count == 0)
                    MessageBox.Show("No config settings were found. Please enter credentials and save them.");

                txtAPIURL.Text = ReadConfigSettingByName("URL", APIURL,false);                
                txtAPIKEY.Text = ReadConfigSettingByName("APIKEY", APIKEY,true);
                cboERPType.Text = ReadConfigSettingByName("ERPTYPE", ERPTYPE, false);                
                txtERPUser.Text = ReadConfigSettingByName("ERPUSER", ERPUSER, false);
                txtERPPassword.Text = ReadConfigSettingByName("ERPPASSWORD", ERPPASSWORD,true);
                txtERPCompany.Text = ReadConfigSettingByName("COMPANYID", COMPANYID, false);
                APIURL = txtAPIURL.Text;


                //Populate custom settings
                string tempAPIENDPOINT = ReadConfigSettingByName("ENDPOINT", APIENDPOINT, false);
                if (tempAPIENDPOINT != "")
                    APIENDPOINT = tempAPIENDPOINT;
                string tempAPIFILTER = ReadConfigSettingByName("FILTER", APIFILTER, false);
                if (tempAPIFILTER != "")
                    APIFILTER = tempAPIFILTER;
                string tempAPPID = ReadConfigSettingByName("APPID", APPID, false);
                if (tempAPPID != "")
                    APPID = tempAPPID;
                string tempPROGRAMNAME = ReadConfigSettingByName("PROGRAMNAME", PROGRAMNAME, false);
                if (tempPROGRAMNAME != "")
                    PROGRAMNAME = tempPROGRAMNAME;
                string tempAPPVERSION = ReadConfigSettingByName("APPVERSION", APPVERSION, false);
                if (tempAPPVERSION != "")
                    APPVERSION = tempAPPVERSION;
                string tempSYSTEMID = ReadConfigSettingByName("SYSTEMID", SYSTEMID, false);
                if (tempSYSTEMID != "")
                    SYSTEMID = tempSYSTEMID;
                string tempPRINTOPTION = ReadConfigSettingByName("PRINTOPTION", PRINTOPTION, false);
                if (tempPRINTOPTION != "" && (tempPRINTOPTION == "0" || tempPRINTOPTION == "1"))
                    PRINTOPTION = tempPRINTOPTION;
                

                DataTable dtCustomSettings = new DataTable();
                dtCustomSettings.Columns.Add("KeyName");
                dtCustomSettings.Columns.Add("KeyValue");
                dtCustomSettings.Rows.Add(InsertRow(dtCustomSettings.NewRow(), "ENDPOINT", APIENDPOINT));
                dtCustomSettings.Rows.Add(InsertRow(dtCustomSettings.NewRow(), "APIFILTER", APIFILTER));
                dtCustomSettings.Rows.Add(InsertRow(dtCustomSettings.NewRow(), "APPID", APPID));
                dtCustomSettings.Rows.Add(InsertRow(dtCustomSettings.NewRow(), "PROGRAMNAME", PROGRAMNAME));
                dtCustomSettings.Rows.Add(InsertRow(dtCustomSettings.NewRow(), "APPVERSION", APPVERSION));
                dtCustomSettings.Rows.Add(InsertRow(dtCustomSettings.NewRow(), "SYSTEMID", SYSTEMID));
                dtCustomSettings.Rows.Add(InsertRow(dtCustomSettings.NewRow(), "PRINTOPTION", PRINTOPTION));
                dgSettings.DataSource = dtCustomSettings;


            }
            catch (Exception ex)
            {
                MessageBox.Show("Config load error: " + ex.Message);
            }

        }

        private void SaveConfigSettings()
        {
            try
            {
                string ConfigFile = ConfigFolder;
                if (!ConfigFile.EndsWith("\\"))
                    ConfigFile += "\\";
                ConfigFile += "Config.json";
                              

                DataTable dtSaveSettings = new DataTable();
                dtSaveSettings.Columns.Add("KeyName");
                dtSaveSettings.Columns.Add("KeyValue");
                APIURL = txtAPIURL.Text;
                APIKEY = _clsConfig.ConvertToEncrypted(txtAPIKEY.Text);
                ERPTYPE = cboERPType.Text;
                ERPUSER = txtERPUser.Text;
                ERPPASSWORD = _clsConfig.ConvertToEncrypted(txtERPPassword.Text);
                COMPANYID = txtERPCompany.Text;

                dtSaveSettings.Rows.Add(InsertRow(dtSaveSettings.NewRow(), "URL", APIURL));
                dtSaveSettings.Rows.Add(InsertRow(dtSaveSettings.NewRow(), "APIKEY", APIKEY));
                dtSaveSettings.Rows.Add(InsertRow(dtSaveSettings.NewRow(), "ERPTYPE", ERPTYPE));
                dtSaveSettings.Rows.Add(InsertRow(dtSaveSettings.NewRow(), "ERPUSER", ERPUSER));
                dtSaveSettings.Rows.Add(InsertRow(dtSaveSettings.NewRow(), "ERPPASSWORD", ERPPASSWORD));
                dtSaveSettings.Rows.Add(InsertRow(dtSaveSettings.NewRow(), "COMPANYID", COMPANYID));

                //custom settings         

                APIENDPOINT = ReadConfigSettingByName("ENDPOINT", APIENDPOINT, false);
                APIFILTER = ReadConfigSettingByName("APIFILTER", APIFILTER, false);
                APPID = ReadConfigSettingByName("APPID", APPID, false);
                PROGRAMNAME = ReadConfigSettingByName("PROGRAMNAME", PROGRAMNAME, false);
                APPVERSION = ReadConfigSettingByName("APPVERSION", APPVERSION, false);
                SYSTEMID = ReadConfigSettingByName("SYSTEMID", SYSTEMID, false);
                PRINTOPTION = ReadConfigSettingByName("PRINTOPTION", PRINTOPTION, false);

                dtSaveSettings.Rows.Add(InsertRow(dtSaveSettings.NewRow(), "ENDPOINT", APIENDPOINT));
                dtSaveSettings.Rows.Add(InsertRow(dtSaveSettings.NewRow(), "APIFILTER", APIFILTER));
                dtSaveSettings.Rows.Add(InsertRow(dtSaveSettings.NewRow(), "APPID", APPID));
                dtSaveSettings.Rows.Add(InsertRow(dtSaveSettings.NewRow(), "PROGRAMNAME", PROGRAMNAME));
                dtSaveSettings.Rows.Add(InsertRow(dtSaveSettings.NewRow(), "APPVERSION", APPVERSION));
                dtSaveSettings.Rows.Add(InsertRow(dtSaveSettings.NewRow(), "SYSTEMID", SYSTEMID));
                dtSaveSettings.Rows.Add(InsertRow(dtSaveSettings.NewRow(), "PRINTOPTION", PRINTOPTION));


                string SaveResults = _clsConfig.WriteTableToConfigFile(dtSaveSettings, ConfigFolder, "Config.json");
                MessageBox.Show(SaveResults);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving config settings: " + ex.Message);
            }

        }

        private DataRow InsertRow(DataRow drSetting, string KeyName, string KeyVal)
        {
            try
            {
                drSetting[0] = KeyName;
                drSetting[1] = KeyVal;
            }
            catch { }

            return drSetting;
        }

        private string ReadConfigSettingByName(string ConfigName, string DefaultValue, bool Decrypt)
        {
            string Setting = "";
            try
            {
                if (dgSettings.Rows.Count > 4)
                {
                    for(int i = 0; i < dgSettings.Rows.Count;i++)
                    {
                        if (dgSettings.Rows[i].Cells[0].Value != null && dgSettings.Rows[i].Cells[0].Value.ToString().ToUpper() == ConfigName)
                            Setting = dgSettings.Rows[i].Cells[1].Value.ToString();

                    }
                }
                else
                {
                    if (dtConfigSettings.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dtConfigSettings.Rows)
                        {
                            if (dr[0].ToString().ToUpper() == ConfigName)
                                Setting = dr[1].ToString();
                        }
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


        //Test API Settings
        private void CheckAPISettings()
        {
            DataTable dtAPIData = new DataTable();
            try
            {

                APIURL = txtAPIURL.Text;
                string validation = "";
                if (APIURL == "")
                    validation = "API URL setting not found. ";
                
                if (APIENDPOINT == "")
                    validation += "API endpoint setting not found. ";
                APIKEY = txtAPIKEY.Text;
                if (APIKEY == "")
                    validation = "API key setting not found. ";

                if (validation != "")
                    MessageBox.Show(validation);
                else
                {
                    this.Cursor = Cursors.WaitCursor;
                    dtAPIData = _clsAPI.ReadAPI(APIURL, APIENDPOINT, APIFILTER, APIKEY, APIClass.HttpVerb.GET, 0, 1);
                    if (dtAPIData.Rows.Count > 0)
                        MessageBox.Show("Successfully connected to the API endpoint. " + dtAPIData.Rows.Count.ToString() + " events queued");
                    else
                    {
                        
                        string LastStatus = _clsAPI.LastStatus;
                        
                        if (LastStatus.ToUpper().Contains("OK") || LastStatus.ToUpper().Contains("ACCEPTED"))
                            MessageBox.Show("Successfully connected to the API endpoint with no queued events found.");                        
                        else
                            MessageBox.Show("Errors encountered. " + _clsAPI.LastError);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            this.Cursor = Cursors.Default;
        }

        private void btnTestERP_Click(object sender, EventArgs e)
        {
            TestERPConnection();
        }

        private void TestERPConnection()
        {
            try
            {
                switch (cboERPType.Text.ToUpper())
                {
                    case "SAGE":

                        SageModule _clsSAGE = new SageModule(APPID, PROGRAMNAME, APPVERSION, txtERPUser.Text, txtERPPassword.Text, txtERPCompany.Text, SYSTEMID);
                        string Response = _clsSAGE.TestConnection();
                        MessageBox.Show(Response);


                        break;

                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }



        private void btnTestRun_Click(object sender, EventArgs e)
        {
            try
            {
                if (APIURL == "")
                {
                    MessageBox.Show("Please save the configuration before running.");
                    return;
                }
                else
                    RunWriteBack();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to run: " + ex.Message);
            }
        }

        private void RunWriteBack()
        {
            try
            {
                BackgroundTask.WorkerReportsProgress = true;
                BackgroundTask.WorkerSupportsCancellation = true;
                BackgroundTask.RunWorkerAsync();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while starting the write-back process: " + ex.Message);
            }

        }



        private void BackgroundTask_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            
            try
            {
                if (worker.CancellationPending == true)
                {
                    e.Cancel = true;
                    MessageBox.Show("Process still active. Please wait for it to complete.");
                }
                else
                {
                    string RunFolder = Application.StartupPath;
                    if (RunFolder.Substring(RunFolder.Length - 1) != "\\")
                        RunFolder += "\\";
                    
                    string RunCommand = $"SyncWriter.exe";
                    string Arguments = $"-t 1 -d";
                    //string Arguments = $"-f \"{RunProjectFolder}\" -i {RunTransformId.ToString()}";
                    LastRunCommand = RunCommand + " " + Arguments ;

                    var si = new System.Diagnostics.ProcessStartInfo();
                    si.CreateNoWindow = true;
                    si.FileName = RunCommand;
                    si.Arguments = Arguments;
                    System.Diagnostics.Process.Start(si);

                    //System.Diagnostics.Process.Start(RunCommand,Arguments);
                    MessageBox.Show("Running command " + LastRunCommand);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Start error: " + ex.Message);
            }
        }

        private void BackgroundTask_RunWorkerCompleted(
            object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                // First, handle the case where an exception was thrown.
                if (e.Error != null)
                {
                    MessageBox.Show("Run error: " + e.Error);
                }
                else if (e.Cancelled)
                {
                    // Next, handle the case where the user canceled 
                    // the operation.
                    // Note that due to a race condition in 
                    // the DoWork event handler, the Cancelled
                    // flag may not have been set, even though
                    // CancelAsync was called.
                    MessageBox.Show("Write-back run canceled");
                }
                else
                {
                    // Finally, handle the case where the operation 
                    // succeeded.
                    MessageBox.Show("Write-back process successfully. Last Command was: " + LastRunCommand);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Transform run completed with errors: " + ex.Message);
            }
        }

       
    }
}
