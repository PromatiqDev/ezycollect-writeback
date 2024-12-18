using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;


namespace ezyCollect_Writeback_Service
{
    class EventLogManager
    {
         
        // Constants
        private const string EVENT_LOG_NAME = "Application";
        private const string EVENT_LOG_SOURCE_NAME = "ezyCollectWriteBackService";

        private static string strLastMessage = "";

        private static string AppLogFolder = "";





        /*
            * Output an information log message (Viewed in "Event Viewer").
            */
        public static void WriteInfoLog(string logMessage)
        {
            WriteLog(logMessage, EventLogEntryType.Information);
            WriteFileLog("INFORMATION", logMessage);
        }

        /*
            * Output a warning log message (Viewed in "Event Viewer").
            */
        public static void WriteWarningLog(string logMessage)
        {
            WriteLog(logMessage, EventLogEntryType.Warning);
            WriteFileLog("WARNING", logMessage);
        }

        /*
            * Output an error log message (Viewed in "Event Viewer").
            */
        public static void WriteErrorLog(string logMessage)
        {
            WriteLog(logMessage, EventLogEntryType.Error);
            WriteFileLog("ERROR", logMessage);
        }

        /*
            * Output a success audit log message (Viewed in "Event Viewer").
            */
        public static void WriteSuccessAuditLog(string logMessage)
        {
            WriteLog(logMessage, EventLogEntryType.SuccessAudit);
        }

        /*
            * Output a failure audit log message (Viewed in "Event Viewer").
            */
        public static void WriteFailureAuditLog(string logMessage)
        {
            WriteLog(logMessage, EventLogEntryType.FailureAudit);
        }


        // Log file writer


        /*
            * Private helper method which outputs log messages with the correct 
            * log entry type and create custom log event source if required.
            */
        private static void WriteLog(string logMessage, EventLogEntryType logType)
        {
            bool bLogSuccess = false;
            try
            {
                if (strLastMessage == null || strLastMessage != logMessage)
                {
                    strLastMessage = logMessage;
                    // Check if the event source exists
                    if (!(EventLog.SourceExists(EVENT_LOG_SOURCE_NAME)))
                    {
                        // Create new event log source
                        EventLog.CreateEventSource(EVENT_LOG_SOURCE_NAME, EVENT_LOG_NAME);
                    }

                    // Output new event log
                    EventLog log = new EventLog();
                    
                    log.Source = EVENT_LOG_SOURCE_NAME;
                    log.WriteEntry(logMessage, logType);
                    bLogSuccess = true;
                }
            }
            catch
            {
                bLogSuccess = false;
            }

        }

        private static void WriteFileLog(string EventType, string LogMessage)
        {
            try
            {
                try
                {
                    string strFile = "Service_Log_" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
                    if (AppLogFolder == "")
                        AppLogFolder = GetAppFolder(GetAppName());
                    //check if the folder exist
                    if (!AppLogFolder.EndsWith("\\"))
                        AppLogFolder += "\\";
                    bool NewFile = false;
                    if (!File.Exists(AppLogFolder + strFile))
                        NewFile = true;
                    System.IO.StreamWriter file = new System.IO.StreamWriter(AppLogFolder + strFile, true);
                    //create header if new
                    if (NewFile)
                        file.WriteLine("DateTime,EventType,Message");

                    LogMessage = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + EventType + "," + LogMessage;
                    file.WriteLine(LogMessage);

                    file.Close();



                }
                catch { }
            }
            catch { }
        }

        public static string GetAppName()
        {
            return "ezyCollect\\SyncWriteBack";
        }

        //fetches the app data location
        public static string GetAppFolder(string AppName)
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


    }
}
