using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;

namespace CommonClass
{

    
    public class Logger
    {
        public string LogFolder { get; set; }

        ConfigManager _clsConfig = new ConfigManager();

        

        public Logger(string logfolder)
        {
            try
            {
                if (logfolder != null && logfolder != "")
                {
                    try
                    {
                        if (Directory.Exists(logfolder))
                            LogFolder = logfolder;
                        else
                        {
                            Directory.CreateDirectory(logfolder);
                            if (Directory.Exists(logfolder))
                                LogFolder = logfolder;
                        }
                        
                        
                    }
                    catch
                    {
                        LogFolder = "";
                    }                
                }
                // Default log to app data
                if (LogFolder == null || LogFolder == "")
                    LogFolder = _clsConfig.GetAppFolder("");
                //create folder if it doesnt exist
                if (!Directory.Exists(LogFolder))
                    Directory.CreateDirectory(LogFolder);
            }
            catch{ }
        }

        public void WriteErrorLog(string LogMessage)
        {
            try
            {
                WriteLog("ERROR", LogMessage);
            }
            catch{}
        }
        public void WriteInfoLog(string LogMessage)
        {
            try
            {
                WriteLog("INFORMATION", LogMessage);
            }
            catch { }
        }
        public void WriteWarningLog(string LogMessage)
        {
            try
            {
                WriteLog("WARNING", LogMessage);
            }
            catch { }
        }
        public void WriteLog(string EventType,string LogMessage)
        {
            try
            {
                string strFile = "Event_Log_" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
                //check if the folder exist
                if (!LogFolder.EndsWith("\\"))
                    LogFolder += "\\";
                bool NewFile = false;
                if (!File.Exists(LogFolder + strFile))
                    NewFile = true;
                System.IO.StreamWriter file = new System.IO.StreamWriter(LogFolder + strFile,true);
                //create header if new
                if (NewFile)
                    file.WriteLine("DateTime,EventType,Message");
                //get file line to write

                if (LogMessage.Contains("\"") || LogMessage.Contains(","))
                    LogMessage = "\"" + LogMessage + "\"";
                string strFileLine = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + EventType + "," + LogMessage;
                file.WriteLine(strFileLine);

                file.Close();

            }
            catch { }
        }

        public void WriteEvent(string LogMessage)
        {
            try
            {
                string strFile = "Event_Log_" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
                //check if the folder exist
                if (!LogFolder.EndsWith("\\"))
                    LogFolder += "\\";
                bool NewFile = false;
                if (!File.Exists(LogFolder + strFile))
                    NewFile = true;
                System.IO.StreamWriter file = new System.IO.StreamWriter(LogFolder + strFile, true);
                //create header if new
                if (NewFile)
                    file.WriteLine("DateTime,EventType,Message");
                file.WriteLine(LogMessage);

                file.Close();



            }
            catch { }
        }

        public void WriteServiceLog(string EventType, string LogMessage)
        {
            try
            {
                string strFile = "Service_Log_" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
                //check if the folder exist
                if (!LogFolder.EndsWith("\\"))
                    LogFolder += "\\";
                bool NewFile = false;
                if (!File.Exists(LogFolder + strFile))
                    NewFile = true;
                System.IO.StreamWriter file = new System.IO.StreamWriter(LogFolder + strFile, true);
                //create header if new
                if (NewFile)
                    file.WriteLine("DateTime,EventType,Message");
                //get file line to write

                if (LogMessage.Contains("\"") || LogMessage.Contains(","))
                    LogMessage = "\"" + LogMessage + "\"";
                string strFileLine = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + EventType + "," + LogMessage;
                file.WriteLine(strFileLine);

                file.Close();

            }
            catch { }
        }

        public void WriteServiceEvent(string LogMessage)
        {
            try
            {
                string strFile = "Service_Log_" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
                //check if the folder exist
                if (!LogFolder.EndsWith("\\"))
                    LogFolder += "\\";
                bool NewFile = false;
                if (!File.Exists(LogFolder + strFile))
                    NewFile = true;
                System.IO.StreamWriter file = new System.IO.StreamWriter(LogFolder + strFile, true);
                //create header if new
                if (NewFile)
                    file.WriteLine("DateTime,EventType,Message");
                file.WriteLine(LogMessage);

                file.Close();



            }
            catch { }
        }
    }
}
