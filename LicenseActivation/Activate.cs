using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using EncryptionLib;
using UserSettingsLib;
using ErrorLoggingLib;
using System.Net;
using System.Diagnostics;
using System.Reflection;
using ApplicationDataClass;

namespace LicenseActivation
{
    public partial class ActivateLicense : UserControl
    {
        string m_SKU;

        public ActivateLicense(string sku, APPLICATION_DATA appData)
        {
            InitializeComponent();

            m_AppData = appData;
            m_Log = (ErrorLog) m_AppData.Logger;

            m_SKU = sku;
            m_NeedToRestart = false;
        }

        bool m_NeedToRestart;
        ErrorLog m_Log;
        APPLICATION_DATA m_AppData;

        private void Activator_Load(object sender, EventArgs e)
        {
            string productName;
            string serialNumber;
            int CameraCount;

            openFileDialog1.Filter = "Text Files|*.txt;";
            openFileDialog1.FileName = " ";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if (GetActivationCode(openFileDialog1.FileName, out productName, out serialNumber, out CameraCount))
                {
                    MarkThisInstanceAsActivated();
                    UserSettings.Set("serialnumber", serialNumber);
                    MessageBox.Show("Success ! " + productName + ",  SN: " + serialNumber + "\r\n\r\nThe Plate Surveillance Software must be restarted now...please close the app and restart");
                    m_NeedToRestart = true;
                }
            }
        }

        public bool NeedToRestart()
        {
            return (m_NeedToRestart);
        }

        void MarkThisInstanceAsActivated()
        {

            try{

            string path = (new FileInfo(System.Windows.Forms.Application.ExecutablePath)).DirectoryName;

            DateTime creationTime = Directory.GetCreationTime(path);

            string encrypted = Encryption.EncryptText(creationTime.ToString());

            UserSettings.Set("plateOCRLibDefaultConfig", encrypted);
            }
            catch (Exception ex)
            {
                m_Log.Log("IsActivated error : " + ex.Message,ErrorLog.LOG_TYPE.FATAL);
            }

        }

        public enum LICENSE_STATE {DEMO_MODE, LICENSED_MODE, STOPWORKING_MODE }


        // security by obfuscation - because the UserSettings field tags are stored in plain text,
        // choosing names that do not give away their true purpose

        // if first time run, we need to go to demo mode, from demo mode the user can
        //   install the license key file via the activate button
        // if we are in demo mode - check the time since installation to determine if demo perdio has expired.


        public static LICENSE_STATE IsActivated()
        {
            if (haveValidLicenseKey())
            {
                return LICENSE_STATE.LICENSED_MODE;
            }
            else if (inDemoPeriod())
            {
                return LICENSE_STATE.DEMO_MODE;
            }
            else if (firstTimeRun())
            {
                return LICENSE_STATE.DEMO_MODE;
            }
            else
            {
                return LICENSE_STATE.STOPWORKING_MODE;
            }

        }

        static bool firstTimeRun()
        {
            return false;
        }

        static bool inDemoPeriod()
        {
            return false;
        }

        static bool haveValidLicenseKey()
        {
            return false;
        }

        //public static LICENSE_STATE IsActivated()
        //{
        //    try
        //    {
        //        string path = (new FileInfo(System.Windows.Forms.Application.ExecutablePath)).DirectoryName;
             
        //        DateTime creationTime = Directory.GetCreationTime(path);

              

        //        string encrypted = UserSettings.Get("plateOCRLibDefaultConfig");
        //        if (encrypted == null)
        //        {
        //            // the string is not there, either this is the first time being run or the user is cheating
        //            if (UserSettings.Get("plateRollTwistDefault") != null)
        //            {
        //                // user is cheating - this was run before but the secret key string is missing
        //                return LICENSE_STATE.STOPWORKING_MODE;

        //            }

        //            // this is the first time running
        //            UserSettings.Set("plateRollTwistDefaults", "0.0385"); 

        //            // write the current time, this being the first time run
        //            // this will be used by the demo mode to determine if the demo perdiod has expired.
                  


        //        }

        //        string decrypted = Encryption.DecryptText(encrypted);

        //        if (creationTime.ToString().Equals(decrypted))
        //        {
        //            return (true);
        //        }
        //        else
        //            return (false);
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorLoggingLib.ErrorLog.Trace(ex);
        //        return (false);
        //    }
        //}

        public static int GetCameraCount()
        {

            int CameraCount = 0;

            try
            {
               
                string encrypted = UserSettings.Get("cameracount");
                string decrypted = Encryption.DecryptText(encrypted);

                CameraCount = Convert.ToInt32(decrypted);
               
            }
            catch (Exception ex)
            {
                ErrorLoggingLib.ErrorLog.Trace(ex);
                return (0);
            }

            return (CameraCount);
        }


        bool GetActivationCode(string file, out string productName, out string serialNumber, out int CameraCount)
        {

            FileInfo fi = new FileInfo(file);
            productName = null;
            serialNumber = null;
            CameraCount = 0;

            //string fileSerialNum = (fi.Name.Split('.'))[0];
            string filenamenoext = (fi.Name.Split('.'))[0];
            string fileSerialNum = (filenamenoext.Split('_'))[0];
            if (!filenamenoext.Contains("_"))
            {
                m_Log.Log("Activation error on file open, file name format not correct", ErrorLog.LOG_TYPE.FATAL);
                MessageBox.Show("Activation error on file open, file name format not correct. Possible cause: incorrect file");
                return (false);
            }

            string sku = (filenamenoext.Split('_'))[1];
            
            if (sku.Contains("PSS"))
            {
//                FE-PSS-C3-2.0.0.1

                try
                {
                    string[] ss1 = sku.Split('-');
                    string[] psssku =ss1[2].Split('C');
                    CameraCount = Convert.ToInt32(psssku[1]);
                    string encryptedCount = Encryption.EncryptText(CameraCount.ToString());
                    UserSettings.Set("cameracount", encryptedCount);
                }
                catch (Exception ex)
                {
                    m_Log.Log("Activation error on file open: " + ex.Message, ErrorLog.LOG_TYPE.FATAL);
                    MessageBox.Show("Activation error PSS SKU " + ex.Message);
                    return (false);
                }
            }
          

           
            string[] lines;
            try
            {
                lines = File.ReadAllLines(file);
            }
            catch (Exception ex)
            {
                m_Log.Log("Activation error on file open: " + ex.Message, ErrorLog.LOG_TYPE.FATAL);
                MessageBox.Show("Activation error on file open: " + ex.Message);
                return (false);
            }

            string line = Encryption.DecryptText(lines[0]);

            string[] ss = line.Split(',');

            if (!ss[2].Contains("ftp"))
            {
                m_Log.Log("Activation file format error", ErrorLog.LOG_TYPE.FATAL);
                MessageBox.Show("Activation file format error");

                return (false);
            }    

            // decrpyt(encryptedkeystring) = 
            //    "username, password, ftpFileUri, PRODUCTSKU, SERIALNUMBER"

            string un = ss[0];
            string pw = ss[1];
            string ftpuri = ss[2];
            productName = ss[3];
            serialNumber = ss[4];

            if (!productName.Contains(m_SKU))
            {
                MessageBox.Show("Product SKU does not match license key");
                return (false);
            }


            if ( ! serialNumber.Equals(fileSerialNum))
            {
                return (false);
            }

            if (!GetKeyFileFromServer( un, pw, ftpuri ))
                return (false);

            return (true);
        }



        bool GetKeyFileFromServer(string un, string pw, string uriStr)
        {
            MemoryStream stream;
          
            try{
              //  Uri uri = new Uri(uriStr);

                stream = new MemoryStream();

            DownloadFile(uriStr,un, pw, stream);
            }
            catch (Exception ex )
            {
                MessageBox.Show ("GetKeyFileFromServerftp exception: "+ex.Message);
                m_Log.Log ( "GetKeyFileFromServer ftp exception: "+ex.Message, ErrorLog.LOG_TYPE.FATAL);
                return (false);
            }

            if (stream.Length < 10) // all we need to know is that the file with this name was there
                return (false);

            bool retVal = false;
            try
            {
               retVal = DeleteFile(uriStr , un, pw);
            }
            catch (Exception ex)
            {
                MessageBox.Show("GetKeyFileFromServerftp exception: " + ex.Message);
                m_Log.Log("GetKeyFileFromServer ftp exception: " + ex.Message, ErrorLog.LOG_TYPE.FATAL);
                return (false);
            }

            return (retVal);
        }



        bool DeleteFile(string uri, string un, string pw)
        {

            //create the ftp request
         
            FtpWebRequest LRequest = (FtpWebRequest)WebRequest.Create(uri);
            LRequest.Credentials = new NetworkCredential(un, pw);
            LRequest.Method = WebRequestMethods.Ftp.DeleteFile;
            FtpWebResponse LResponse = (FtpWebResponse)LRequest.GetResponse();
            if ( LResponse.StatusCode !=  FtpStatusCode.FileActionOK)
            {
                return(false);
            }

            return (true);
        }

        /// <summary>
        /// Downloads the contents of a file on the ftp server into a stream.
        /// </summary>
        /// <param name="AServerUri"> The file to download. </param>
        /// <param name="AOutStream"> The stream in which to write the contents of the file. </param>
        public static void DownloadFile(string AServerUri, string un, string pw, MemoryStream AOutStream)
        {

            //ftp://user:password@ftp.myserver.com:21/path/to/upload/folder/autoexec.bat

        
       
            // The serverUri should start with the ftp:// scheme.
       //     if (AServerUri.Scheme != Uri.UriSchemeFtp)
       //         throw new Exception("A valid FTP server URI is required.");

            if (AOutStream == null)
                throw new Exception("ftp://ftp.downloadfile/: A valid stream is required to down load a file.");

            // Get the object used to communicate with the server.
            FtpWebRequest LRequest = (FtpWebRequest)WebRequest.Create(AServerUri);
            LRequest.Credentials = new NetworkCredential(un, pw);
            LRequest.Method = WebRequestMethods.Ftp.DownloadFile;

            // Get the ServicePoint object used for this request, and limit it to one connection.
            // In a real-world application you might use the default number of connections (2),
            // or select a value that works best for your application.

            ServicePoint LServicePoint = LRequest.ServicePoint;
            LServicePoint.ConnectionLimit = 1;

            Stream LResponseStream = null;

            FtpWebResponse LResponse = (FtpWebResponse)LRequest.GetResponse();
            try
            {
                // The following streams are used to read the data returned from the server.
                StreamReader LReadStream = null;

                LResponseStream = LResponse.GetResponseStream();
                if (LResponseStream != null)
                {
                    Console.WriteLine("File Download status: {0}", LResponse.StatusDescription);
                    LReadStream = new StreamReader(LResponseStream, System.Text.Encoding.UTF8);
                    try
                    {
                        if (LReadStream != null)
                        {
                            StreamWriter LWriter = new StreamWriter(AOutStream);
                            LWriter.AutoFlush = true;
                            LWriter.Write(LReadStream.ReadToEnd());
                            //AOutStream.Seek(0, SeekOrigin.Begin);
                        }
                    }
                    finally
                    {
                        if (LReadStream != null)
                            LReadStream.Close();
                    }
                }

            }
            finally
            {
                if (LResponse != null)
                    LResponse.Close();
            }
        }
    }
}
