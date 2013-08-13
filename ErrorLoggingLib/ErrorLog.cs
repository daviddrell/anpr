using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ApplicationDataClass;
using ScreenLoggerLib;
using FileLoggingLib;
using System.Windows.Forms;
using System.Diagnostics;


namespace ErrorLoggingLib
{
    public class ErrorLog
    {

        APPLICATION_DATA m_AppData;
        ScreenLogger m_ScreenLogger;
        object singleton;
        bool ScreenLoggerShowing;

        public ErrorLog(APPLICATION_DATA appData)
        {
            m_AppData = appData;
            singleton = new object();

            if (m_AppData.LogToScreen != null)
                m_ScreenLogger = (ScreenLogger) m_AppData.LogToScreen;
        }

        public ErrorLog(APPLICATION_DATA appData, bool LogToScreen)
        {
            m_AppData = appData;
            singleton = new object();
            if (LogToScreen)
            {
                m_ScreenLogger = new ScreenLogger();
                ScreenLoggerShowing = false;
            }
        }

        public enum LOG_TYPE { INFORMATIONAL, FATAL }

        public static void WriteToLog (string error, LOG_TYPE type)
        {
            error = DateTime.UtcNow.ToString() + ": " + error;

            Console.WriteLine(error);

            FileLogging.Set(error);
        }

        //System.Reflection.MethodBase.GetCurrentMethod()

        public void Trace(Exception ex, LOG_TYPE type)
        {
            List<string> lines = new List<string>();

            string time = DateTime.UtcNow.ToString() + " UTC:";

            Console.WriteLine(time + " ex: " + ex.Message + " stack trace: " + Environment.NewLine);
            Console.WriteLine(ex.StackTrace + Environment.NewLine);

            FileLogging.Set(time + " ex: " + ex.Message + " stack trace: " + Environment.NewLine);
            FileLogging.Set(ex.StackTrace + Environment.NewLine);
          

        }

        public static void Trace(Exception ex)
        {
            List<string> lines = new List<string>();

            string time = DateTime.UtcNow.ToString() + " UTC:";

            FileLogging.Set(time + " ex: " + ex.Message + " stack trace: " + Environment.NewLine);
            FileLogging.Set(ex.StackTrace + Environment.NewLine);

        }
       
    
       
        public void Log(string error, LOG_TYPE type)
        {
            lock (singleton)
            {          

               if (!ScreenLoggerShowing && m_ScreenLogger != null)
                {
                    ScreenLoggerShowing = true;
                    m_ScreenLogger.ShowForm();
                }

                Console.WriteLine( error);

                if (m_ScreenLogger != null)
                     m_ScreenLogger.Log(error);
                   

                FileLogging.Set(error);
            }

        }

    }

  
}
