using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;


namespace FileLoggingLib
{
   
 
    public class FileLogging
    {

        static string logFileNameService = "Service_logfile.txt";
        static string logFileNameWorkstationApp = "WorkstationApp_logfile.txt";
        public static string logFileName()
        {

            string caller = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;
           


            if (caller.Contains("Service")) return (logFileNameService);
            else return (logFileNameWorkstationApp);
        }

        const int MAX_ENTRIES = 10000;

        static object m_Singleton=null;

        static void GetSingleton()
        {
            if (m_Singleton == null) m_Singleton = new object();
        }

        // 
        // method Set
        //

        public static void Set(string value)
        {
            GetSingleton();

            lock (m_Singleton)
            {

                StreamWriter fileWriter;

                value = DateTime.UtcNow.ToString() + " UTC: " + value;
                string userData;

                try
                {
                    // Create a file that the application will store user specific data in.
                    //userData = Application.UserAppDataPath + "\\" + logFileName;
                    userData = "C:\\FirstEvidence\\" + logFileName();
                    
                }
                catch (IOException e)
                {
                    // Inform the user that an error occurred.
                    MessageBox.Show("Logging internal error on Application.UserAppDataPath" +
                                    "The error is:" + e.ToString());
                    return;
                }



                int numLines = 0;
                int i = 0;
           

                try
                {
                    //does file exist already ?

                    if (File.Exists(userData))
                    {
                        

                        string[] lines = File.ReadAllLines(userData);

                

                        // too many entires?

                        if (lines.Count() > MAX_ENTRIES)
                        {
                            // dump the first 10% of MAX number of entries and re-write the rest
        
                           
                            int offset = (10 * MAX_ENTRIES) / 100;

                             // delete the old file and write the new one

                            File.Delete(userData);

                            fileWriter = new StreamWriter(userData);

                            for (i = offset; i < numLines; i++)
                            {
                                fileWriter.WriteLine(lines[i]);
                            }

                            fileWriter.Close();

                        }// end if (i > MAX_ENTRIES

                    }// end if file exists


                    // now append the new entry to the file

                    // open with append
                    fileWriter = new StreamWriter(userData, true);

                    fileWriter.WriteLine(value);

                    fileWriter.Close();


                }// end try

                catch (ArgumentException)
                {
                    MessageBox.Show("LoggingLib ArgumentException.. " + logFileName());

                } // end catch
                catch (IOException)
                {
                    MessageBox.Show("LoggingLib IOException.. " + logFileName());

                } // end catch
                catch (UnauthorizedAccessException)
                {
                    MessageBox.Show("LoggingLib UnauthorizedAccessException.. " + logFileName());

                } // end catch

            }
        } // end Set



    }
}
