using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UserSettingsLib;
using EncryptionLib;
using System.IO;
using System.Net;

namespace KeyFileGenerator
{
    public partial class KeyFileGenMainForm : Form
    {
        public KeyFileGenMainForm()
        {
            InitializeComponent();

            m_KeyOwner = KEY_OWNER.A1_SECURITY;

            if (m_KeyOwner == KEY_OWNER.A1_SECURITY)
            {
                labelKeyOwnerString.Text = "A1 Security Demo Key Generator";
                radioButtonDemoKey.Checked = true;
                radioButtonProductionKey.Checked = false;
                radioButtonProductionKey.Enabled = false;
            }
            else
            {
                labelKeyOwnerString.Text = "First Evidence Key Generator";
                radioButtonDemoKey.Checked = false;
                radioButtonProductionKey.Checked = true; 
                radioButtonProductionKey.Enabled = true;

            }

            comboBoxProductTypeList.Items.Add("PSS_V_3.0_C1");
            comboBoxProductTypeList.Items.Add("PSS_V_3.0_C2");
            comboBoxProductTypeList.Items.Add("PSS_V_3.0_C3");
            comboBoxProductTypeList.Items.Add("PSS_V_3.0_C4");

        }

        enum KEY_OWNER {FIRST_EVIDENCE, A1_SECURITY }
        KEY_OWNER m_KeyOwner;
        
        string ftpusername = "activation1";
        string ftppw = "0435*%$fgpq";
        string ftpurl = "ftp://ehost-services221.com:21/";

 
        string m_storageLocation;

        private void buttonGenFile_Click(object sender, EventArgs e)
        {
            if (m_storageLocation == null)
            {
                MessageBox.Show(" storage location not set");
                return;
            }

            labelStatus.Text = "Starting";

            string ftpstr = BuildFTP_Str(textBoxSerialNumber.Text, comboBoxProductTypeList.SelectedText);

            string keystring = ftpstr + "," + comboBoxProductTypeList.SelectedText + "," + textBoxSerialNumber.Text;

            string encryptedString = Encryption.EncryptText(keystring);

            string directorypath = m_storageLocation + "\\" + textBoxEnterOrderNumber.Text;
            string filepath = null;

            try
            {
                if (! Directory.Exists(directorypath))
                {
                    Directory.CreateDirectory(directorypath);
                }

                filepath = directorypath + "\\" + textBoxSerialNumber.Text + "_" + comboBoxProductTypeList.SelectedText + ".txt";

                File.WriteAllText(filepath, encryptedString);
            }
            catch (Exception ex)
            {
                MessageBox.Show("file exception: " + ex.Message);
            }

            labelStatus.Text = "Local file written, starting FTP";

            // put it out on the ftp server:

            string ftpuri = null;

            try
            {
                ftpuri = BuildFTP_URI(textBoxSerialNumber.Text, comboBoxProductTypeList.SelectedText);

                Upload(filepath, ftpuri, ftpusername, ftppw);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception : " + ex.Message);
            }

            labelStatus.Text = "Done";
        }


 

        private void buttonFileLocation_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                m_storageLocation = folderBrowserDialog1.SelectedPath;
                UserSettings.Set("location", m_storageLocation);
                labelFileLocation.Text = m_storageLocation;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            m_storageLocation = UserSettings.Get("location");
            labelFileLocation.Text = m_storageLocation;
            labelStatus.Text = "Stating New";
        }

        string BuildFTP_URI(string serialNum, string sku)
        { 
          
            serialNum= serialNum.Trim();
            sku = sku.Trim();
            string ftpstring = ftpurl + serialNum + "_"+ sku + ".txt";

            return (ftpstring);
        }

        string BuildFTP_Str(string serialNum, string sku)
        {


            serialNum = serialNum.Trim();
            sku = sku.Trim();
        //    string ftpstring = "activation@firstevidence.com,s7g8s09d8f7,ftp://ftp.firstevidence.com:21/" + serialNum + "_"+ sku + ".txt";
            string ftpstring = ftpusername+ "," + ftppw+ "," + ftpurl + serialNum + "_" + sku + ".txt";

            return (ftpstring);
        }


        private void Upload(string filename, string uri, string un, string pw)
        {
            FileInfo fileInf = new FileInfo(filename);

            FtpWebRequest reqFTP;

            // Create FtpWebRequest object from the Uri provided
            reqFTP = (FtpWebRequest)FtpWebRequest.Create(uri);

            // Provide the WebPermission Credintials
            reqFTP.Credentials = new NetworkCredential(un, pw);

            // By default KeepAlive is true, where the control connection is 
            // not closed after a command is executed.
            reqFTP.KeepAlive = false;

            // Specify the command to be executed.
            reqFTP.Method = WebRequestMethods.Ftp.UploadFile;

            // Specify the data transfer type.
            reqFTP.UseBinary = true;

            // Notify the server about the size of the uploaded file
            reqFTP.ContentLength = fileInf.Length;

            // The buffer size is set to 2kb
            int buffLength = 2048;
            byte[] buff = new byte[buffLength];
            int contentLen;

            // Opens a file stream (System.IO.FileStream) to read 
            //  the file to be uploaded
            FileStream fs = fileInf.OpenRead();

            try
            {
                // Stream to which the file to be upload is written
                Stream strm = reqFTP.GetRequestStream();

                // Read from the file stream 2kb at a time
                contentLen = fs.Read(buff, 0, buffLength);

                // Till Stream content ends
                while (contentLen != 0)
                {
                    // Write Content from the file stream to the 
                    // FTP Upload Stream
                    strm.Write(buff, 0, contentLen);
                    contentLen = fs.Read(buff, 0, buffLength);
                }

                // Close the file stream and the Request Stream
                strm.Close();
                fs.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Upload Error");
            }
        }

        private void textBoxEnterOrderNumber_TextChanged(object sender, EventArgs e)
        {
            labelStatus.Text = "Stating New";

        }

        private void textBoxCustomerName_TextChanged(object sender, EventArgs e)
        {
            labelStatus.Text = "Stating New";

        }

        private void textBoxSerialNumber_TextChanged(object sender, EventArgs e)
        {
            labelStatus.Text = "Stating New";

        }


        private void textBoxProductSKU_TextChanged(object sender, EventArgs e)
        {
            labelStatus.Text = "Stating New";
        }

        private void labelKeyOwnerString_Click(object sender, EventArgs e)
        {

        }


    }
}
