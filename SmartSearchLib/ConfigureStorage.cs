using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using UserSettingsLib;





namespace SmartSearchLib
{
    public partial class ConfigureStorageForm : Form
    {

        UserSettings UserSettings;



        // file system strings

     

        // user.config setting key strings

        const string STORAGE_LOCATION_PATH = "storage location path";

        // 
        public delegate void NotifyStorageChangedDel();
        NotifyStorageChangedDel m_StorageChangedCB;

        public ConfigureStorageForm(NotifyStorageChangedDel storageChangedCallBack)
        {
            InitializeComponent();

            m_StorageChangedCB = storageChangedCallBack;

            UserSettings = new UserSettings();


        }
        


        private void ConfigureStorageForm_Load(object sender, EventArgs e)
        {


            string storageLocation = getStorageLocation();
            if (storageLocation != null)
            {
                storageLocationPathLabel.Text = storageLocation;
            }
        }








        private void finishedButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void folderBrowserDialog1_HelpRequest(object sender, EventArgs e)
        {

        }


        private void selectStorageLocationButton_Click(object sender, EventArgs e)
        {

            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {

                // test the path


                StreamWriter fileWriter;

                string fileNamePath = folderBrowserDialog1.SelectedPath.ToString() + "\\testfile";

                try
                {
                    fileWriter = new StreamWriter(fileNamePath);

                    fileWriter.WriteLine("teststring");

                    fileWriter.Close();

                    File.Delete(fileNamePath);
                }
                catch (ArgumentException)
                {
                    MessageBox.Show("Problem with selected location..Argument Exception");
                    clearStorageLocation();
                    return;
                } // end catch
                catch (IOException)
                {
                    MessageBox.Show("Problem with selected location....IOException ");
                    clearStorageLocation();
                    return;
                } // end catch
                catch (UnauthorizedAccessException)
                {
                    MessageBox.Show("Problem with selected location....UnauthorizedAccessException ");
                    clearStorageLocation();
                    return;
                } // end catch


                setStorageLocation(folderBrowserDialog1.SelectedPath);
                m_StorageChangedCB();
            }
        }

        void clearStorageLocation()
        {
            UserSettings.Set(STORAGE_LOCATION_PATH, "");
            storageLocationPathLabel.Text = "Storage Location Not Set, Storage Disabled";

            m_StorageChangedCB();
        }

        void setStorageLocation(string location)
        {
            UserSettings.Set(STORAGE_LOCATION_PATH, location);

            storageLocationPathLabel.Text = location;

            // create subdirectories

            if (!Directory.Exists(location ))
            {
                Directory.CreateDirectory(location );
            }

            m_StorageChangedCB();
        }


        private void storageLocationPathLabel_Click(object sender, EventArgs e)
        {

        }

        private void disableStorageButton_Click(object sender, EventArgs e)
        {
            clearStorageLocation();
        }

        public static string getStorageLocation()
        {
            if (UserSettings.Get(STORAGE_LOCATION_PATH) == null)
                return (null);

            if (!Directory.Exists(UserSettings.Get(STORAGE_LOCATION_PATH)))
                return (null);

            return (UserSettings.Get(STORAGE_LOCATION_PATH));
        }

        public static string getImageStorageLocation()
        {
            if (UserSettings.Get(STORAGE_LOCATION_PATH) == null)
                return (null);

            if (!Directory.Exists(UserSettings.Get(STORAGE_LOCATION_PATH)))
                return (null);

            if (Directory.Exists(UserSettings.Get(STORAGE_LOCATION_PATH) ))
            {
                return (UserSettings.Get(STORAGE_LOCATION_PATH) );
            }
            else
            {
                return (null);
            }
        }



    }
}
