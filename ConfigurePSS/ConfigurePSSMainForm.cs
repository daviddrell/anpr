using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ConfigWatchListsUC;
using ApplicationDataClass;
using ErrorLoggingLib;
using EmailServicesLib;
using ConfigureEmailUC;
using UserSettingsLib;
using ConfigureGPS_UC;
using EncryptionLib;
using System.ServiceProcess;
using System.Diagnostics;
using System.IO;
using System.Threading;
using LicenseActivation;

namespace ConfigurePSS
{
    public partial class ConfigurePSSMainForm : Form
    {
        public ConfigurePSSMainForm()
        {
            InitializeComponent();

            try
            {
                this.Text = "First Evidence LPR Service Control, version " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

                m_AppData = new APPLICATION_DATA();
                m_AppData.Logger = new ErrorLog(m_AppData, true);
                m_Log = (ErrorLog) m_AppData.Logger;

                SetConfigPath();
               
                m_AppData.AddOnClosing(Stop, APPLICATION_DATA.CLOSE_ORDER.LAST);

                m_ConfigWatchListsUC = new ConfigWatchListsUC.ConfigWatchListsUC(m_AppData);

                m_EmailServices = new EmailServices(m_AppData);
                m_AppData.EmailServices = (object)m_EmailServices;
                m_EmailServices.StartThreads();

                m_ConfigureEmailUC = new ConfigureEmailUC.ConfigureEmailUC(m_AppData);

                m_ConfigureGPS_UC = new ConfigureGPS_UC.ConfigureGPS_UC();
                m_ConfigureGPS_UC.AppData = m_AppData;
                m_ConfigureGPS_UC.Location = new Point(50, 60);

                m_ConfigureSourceChannels = new ConfigureSourceChannels.ConfigSourceChannels(m_AppData);

                tabPageConfigureWatchLists.Controls.Add(m_ConfigWatchListsUC);
                tabPageConfigureEmail.Controls.Add(m_ConfigureEmailUC);
                tabPageConfigureGPS.Controls.Add(m_ConfigureGPS_UC);
                tabPageChannels.Controls.Add(m_ConfigureSourceChannels);

                this.FormClosing += new FormClosingEventHandler(ConfigurePSSMainForm_FormClosing);


                bool serviceInstalled = IsServiceInstalled();
                ServiceControllerStatus status;
                bool serviceRunning = IsServiceRunning(out status);

                SetServiceStatus(serviceRunning, serviceInstalled);


                m_CheckServiceStatusThread = new Thread(CheckServiceStatusLoop);
                m_CheckServiceStatusThread.Start();

       
                ////////////   remove unwanted pages

                // get rid of the password and activationt tabs

                tabControlMain.TabPages.Remove(tabPageEnterPasswords);
                tabControlMain.TabPages.Remove(tabPageActivation);

                
                //if ( ActivateLicense.IsActivated())
                //{
                //    tabControlMain.TabPages.Remove(tabPageActivation);
                //}

            }
            catch (Exception ex) { m_Log.Trace(ex, ErrorLog.LOG_TYPE.FATAL); }
        }

    
       
        ErrorLog m_Log;

        ActivateLicense m_LicenseActivator;
     
        Thread m_CheckServiceStatusThread;

        ConfigureGPS_UC.ConfigureGPS_UC m_ConfigureGPS_UC;

        EmailServices m_EmailServices;
        APPLICATION_DATA m_AppData;
        ConfigWatchListsUC.ConfigWatchListsUC m_ConfigWatchListsUC;
        ConfigureEmailUC.ConfigureEmailUC m_ConfigureEmailUC;
        ConfigureSourceChannels.ConfigSourceChannels m_ConfigureSourceChannels;



        void SetConfigPath()
        {
            // where user.config and logfiles will be stored.

            string firstEvidenceDataStoragePath = "C:/FirstEvidence";
            if (!Directory.Exists(firstEvidenceDataStoragePath))
                Directory.CreateDirectory(firstEvidenceDataStoragePath);
        }



        bool m_Stop;
        void Stop()
        {
            m_Stop = true;
        }

        void SetServiceStatus(bool isRunning, bool isInstalled)
        {
            if ( ! isInstalled)
            {
                textBoxServiceInstallStatus.Text = "Not Installed";
                textBoxServiceRunningStatus.Text = "Not Running";
                buttonStartStopService.Text = " ";
                buttonStartStopService.Enabled = false;
            }
            else
            {
              
                buttonStartStopService.Enabled = true;

                textBoxServiceInstallStatus.Text = "Installed";

                if (isRunning)
                {
                    buttonStartStopService.Text = "Stop Service";
                    textBoxServiceRunningStatus.Text = "Running";
                }
                else
                {
                    buttonStartStopService.Text = "Start Service";
                    textBoxServiceRunningStatus.Text = "Not Running";
                }
            }
        }

        void ConfigurePSSMainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_AppData.LPRServiceControl.ConfigChangedServiceNeedsRestarting  &&  IsServiceInstalled())
            {
                string message = "You have changes one or more configuration settings. The service must be stopped and restarted. To automatically restart the service, choose Yes. To exit without restarting choose No. To cancel the exit, choose Cancel.";
                string caption = "Service Must Be Re-started";
                DialogResult result = MessageBox.Show(message, caption, MessageBoxButtons.YesNoCancel);

                switch (result)
                {
                    case DialogResult.Cancel:
                        e.Cancel = true; // cancel exit
                        break;

                    case   DialogResult.Yes:
                        RestartService();// restart service, then exit
                        break;

                    case DialogResult.No:
                        break; // just exit
                }

            }
            m_AppData.CloseApplication();// stop all threads
        }

       

        private void tabPageConfigureGPS_Click(object sender, EventArgs e)
        {

        }

        private void buttonSaveAdmin_Click(object sender, EventArgs e)
        {
            if (textBoxAdminPassword.Text != textBoxAdminPasswordVerify.Text)
            {
                MessageBox.Show("Admin password does not match Admin password verify");
                textBoxAdminPassword.Text = "";
                textBoxAdminPasswordVerify.Text = "";
                return;
            }
            m_AppData.LPRServiceControl.ConfigChangedServiceNeedsRestarting = true;
            UserSettings.Set(UserSettingTags.PWLPRServiceAdmin,Encryption.EncryptText( textBoxAdminPassword.Text));
        }

        private void buttonSaveViewerPassword_Click(object sender, EventArgs e)
        {
            if (textBoxViewerPassword.Text != textBoxViewerPasswordVerify.Text)
            {
                MessageBox.Show("Viewer password does not match Viewer password verify");
                textBoxViewerPassword.Text = "";
                textBoxViewerPasswordVerify.Text = "";
                return;
            }
            m_AppData.LPRServiceControl.ConfigChangedServiceNeedsRestarting = true;
            UserSettings.Set(UserSettingTags.PWLPRServiceViewer, Encryption.EncryptText(textBoxViewerPassword.Text));
        }

        void CheckServiceStatusLoop()
        {

            while (!m_Stop)
            {
                try
                {
                    bool serviceInstalled = IsServiceInstalled();
                    ServiceControllerStatus status;
                    bool serviceRunning = IsServiceRunning(out status);

                    this.BeginInvoke((MethodInvoker)delegate { this.SetServiceStatus(serviceRunning, serviceInstalled); });

                }
                catch { }

                Thread.Sleep(1000);
            }
        }

        void RestartService()
        {
            StopService();

            Thread.Sleep(2000);

            StartService();

            int count = 10;
            bool serviceIsRunning = IsServiceRunning();

            while (count-- > 0 && !serviceIsRunning)
            {
                serviceIsRunning = IsServiceRunning();
                Thread.Sleep(500);
            }

            string message = (serviceIsRunning) ? "Service Restarted and running." : "Service Restart Failed";
            
            MessageBox.Show(message);
        }

        bool IsServiceInstalled()
        {
            try
            {
                // get list of Windows services
                System.ServiceProcess.ServiceController[] services = ServiceController.GetServices();

                // try to find service name
              
                foreach (ServiceController service in services)
                {
                    if (service.ServiceName == "LPRService")
                    {
                        return (true);
                    }
                }
            }
            catch { }
            return (false);
          
        }

        bool IsServiceRunning(out ServiceControllerStatus status)
        {
            bool isRunning = false;
            status = default(ServiceControllerStatus);
            try
            {
      
                // get list of Windows services
                System.ServiceProcess.ServiceController[] services = ServiceController.GetServices();

                // try to find service name

                foreach (ServiceController service in services)
                {
                    if (service.ServiceName == "LPRService")
                    {
                        if (service.Status == ServiceControllerStatus.Running)
                        {
                            isRunning = true;
                        }
                        status = service.Status;
                    }
                }
            }
            catch { }
            return (isRunning);
           
        }

        bool IsServiceRunning()
        {
            bool isRunning = false;
            try
            {
               

                // get list of Windows services
                System.ServiceProcess.ServiceController[] services = ServiceController.GetServices();

                // try to find service name

                foreach (ServiceController service in services)
                {
                    if (service.ServiceName == "LPRService")
                    {
                        if (service.Status == ServiceControllerStatus.Running)
                        {
                            isRunning = true;
                        }
                    }
                }
            }
            catch { }
            return (isRunning);
           
        }

        bool StartService()
        {

            try
            {
                // get list of Windows services
                System.ServiceProcess.ServiceController[] services = ServiceController.GetServices();

                // try to find service name

                foreach (ServiceController service in services)
                {
                    if (service.ServiceName == "LPRService")
                    {
                        if (service.Status != ServiceControllerStatus.Running)
                        {
                            service.Start();
                        }
                    }
                }
            }
            catch { }
            return (false);
           
        }

        bool StopService()
        {
            try
            {
               m_AppData.LPRServiceControl.ConfigChangedServiceNeedsRestarting = false;

                // get list of Windows services
                System.ServiceProcess.ServiceController[] services = ServiceController.GetServices();

                // try to find service name

                foreach (ServiceController service in services)
                {
                    if (service.ServiceName == "LPRService")
                    {
                        service.Stop();
                    }
                }
            }
            catch { }
            return (false);

        }

        private void buttonInstallService_Click(object sender, EventArgs e)
        {
            try
            {
                string appPath = null;

                if (Application.ExecutablePath.Contains("Debug") || Application.ExecutablePath.Contains("Release"))
                {
                    // dev environment
                    appPath = Application.ExecutablePath.Replace("ConfigurePSS.exe", "LPRService.exe");
                    appPath = appPath.Replace("ConfigurePSS.EXE", "LPRService.exe");// cant figure out why the EXE is sometimes exe
                }
                else
                {    // deployed
                    appPath = Application.ExecutablePath.Replace("FE LPR Service Control\\ConfigurePSS.exe", "FE LPR Service\\LPRService.exe");
                
                }

                FileInfo fi = new FileInfo(appPath);
                string workingDir = fi.DirectoryName;
                string serviceName = fi.Name;
                
                System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo();

                psi.UseShellExecute = true;
                psi.Verb = "runas";
              //  psi.RedirectStandardOutput = true;

                psi.Arguments = serviceName;
                psi.FileName = "C:\\Windows\\Microsoft.NET\\Framework\\v2.0.50727\\InstallUtil.exe";
                psi.WorkingDirectory = workingDir;
                Process p = System.Diagnostics.Process.Start(psi);

                p.WaitForExit();


                // to capture output need to set UseShellExecute to false, but then will not get elevated user permissions and will not get sucesss
                //System.IO.StreamReader oReader2 = p.StandardOutput;
                //string sRes = oReader2.ReadToEnd();
                //oReader2.Close();
                //MessageBox.Show(sRes); 


                bool foundService = IsServiceInstalled();

                if (foundService)
                {
                    StartService();
                    MessageBox.Show("installed");
                }
                else
                    MessageBox.Show("failed");

                

            }
            catch (Exception ex)
            {
                MessageBox.Show("installer ex : " + ex.Message);
            }
        }

        private void buttonUninstallService_Click(object sender, EventArgs e)
        {
            try
            {
                string appPath =null;

                if (Application.ExecutablePath.Contains("Debug") || Application.ExecutablePath.Contains("Release"))
                {
                    appPath = Application.ExecutablePath.Replace("ConfigurePSS.exe", "LPRService.exe");
                    appPath = appPath.Replace("ConfigurePSS.EXE", "LPRService.exe");// cant figure out why the EXE is sometimes exe
                }
                else
                {
                    appPath = Application.ExecutablePath.Replace("FE LPR Service Control\\ConfigurePSS.exe", "FE LPR Service\\LPRService.exe");
                   
                }

                FileInfo fi = new FileInfo(appPath);
                string workingDir = fi.DirectoryName;
                string serviceName = fi.Name;

                System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo();

                psi.UseShellExecute = true;
                psi.Verb = "runas";
                //  psi.RedirectStandardOutput = true;

                psi.Arguments = "/u " + serviceName;
                psi.FileName = "C:\\Windows\\Microsoft.NET\\Framework\\v2.0.50727\\InstallUtil.exe";
                psi.WorkingDirectory = workingDir;
                Process p = System.Diagnostics.Process.Start(psi);

                p.WaitForExit();

                // to capture output need to set UseShellExecute to false, but then will not get elevated user permissions and will not get sucesss
                //System.IO.StreamReader oReader2 = p.StandardOutput;
                //string sRes = oReader2.ReadToEnd();
                //oReader2.Close();
                //MessageBox.Show(sRes); 

                bool foundService = IsServiceInstalled();


                if (! foundService)
                    MessageBox.Show("un-installed");
                else
                    MessageBox.Show("failed");
            }
            catch (Exception ex)
            {
                MessageBox.Show("un-installer ex : " + ex.Message);
            }
        }

        private void tabPageService_Click(object sender, EventArgs e)
        {

        }

        private void buttonStartStopService_Click(object sender, EventArgs e)
        {
            if (IsServiceRunning())
            {
                StopService();
            }
            else
            {
                StartService();
            }
        }

       

        private void buttonActivate_Click(object sender, EventArgs e)
        {
            m_LicenseActivator = new ActivateLicense("PSS_V3", m_AppData);
            tabPageActivation.Controls.Add(m_LicenseActivator);

            // license/ demo mode behavoir
            //
            // the LPR Service control is used to set the license key into the config file, either
            //      the demo mode key or the purchased license key can be installed.
            //      At a minimu, the user must obtain a demo mode key to operate.
            
            // The demo mode key will contain (once decrpyted) a demo termination date. The service
            // will stop itself on this date. If the service attempts to re-start, it will not unless
            // a new key is installed first.

            // if the user attempts to start the service with no demo mode key or no service key installed,
            // the service will not start, the Service Control should check for these conditions to warn the user.
            //  so there will be two checks, one in the service itself, and one in the Service Control ( the user
            //  could attempt to start the service from the control panel).

            // 
            // 
        }
    }

    

}
