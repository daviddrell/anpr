using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using ApplicationDataClass;
using ErrorLoggingLib;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using UserSettingsLib;

namespace Control_Center
{
    public partial class SelectRemoteSystem : UserControl
    {
        public SelectRemoteSystem(APPLICATION_DATA appData)
        {
            InitializeComponent();
            m_AppData = appData;
            m_AppData.AddOnClosing(OnClose, APPLICATION_DATA.CLOSE_ORDER.FIRST);
            m_Log = (ErrorLog) m_AppData.Logger;

            m_AppData.CurrentlyLoggedInRemoteServer = (object)new RemoteHosts(m_AppData, true);// assign a nullHost as a place holder until the use selects an actual server

            MessageGenerators = new MessageGeneratorEvents(this);


            buttonLogout.Visible = false;
            buttonLogout.Enabled = false;
            
         

            m_StatsViewer = new StatsViewer(m_AppData);
            //744, 57
            m_StatsViewer.Location = new Point(744, 57);
            this.Controls.Add(m_StatsViewer);

            bool hostsLoaded = false;
            string hostfile = UserSettings.Get(UserSettingTags.REMOTEHOSTS_LastHostFileUsed);
            if (hostfile != null)
            {
                if (File.Exists(hostfile))
                {
                    m_HostFile = hostfile;
                    LoadHostsFromFile(hostfile);
                    hostsLoaded = true;
                }
            }
            if ( ! hostsLoaded)
                AddLocalDefaultHost();

        }



        // ////////////////////////////////////////////////
        //
        //  globals
        APPLICATION_DATA m_AppData;
        ErrorLog m_Log;


        StatsViewer m_StatsViewer;

        public MessageGeneratorEvents MessageGenerators;
        string m_HostFile;
        LoginPWPrompt m_PWPrompt;

        List<RemoteHosts> m_Hosts;
        Hashtable m_HostsLookup;


        // ////////////////////////////////////////////////
        //
        //    Close the app
        //
        void OnClose()
        {
            if (m_Hosts == null) return;
            foreach (RemoteHosts h in m_Hosts)
            {
                h.Stop();
            }
        }

        // ////////////////////////////////////////////////
        //
        //    Open Hosts File
        //

  

        private void buttonLoadHosts_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                m_HostFile = openFileDialog1.FileName;
                UserSettings.Set(UserSettingTags.REMOTEHOSTS_LastHostFileUsed, m_HostFile);
                LoadHostsFromFile(m_HostFile);
            }
        }


        void AddLocalDefaultHost()
        {
            ClearTable();

            m_Hosts = new List<RemoteHosts>();
            m_HostsLookup = new Hashtable();

         
            RemoteHosts h = new RemoteHosts(m_AppData, false);
       
            h.hostIPstr = "127.0.0.1";

            h.hostDescription = "This Machine";

            h.MessageEventGenerators.OnRxInvalidPassword += OnInvalidPassword;

            h.MessageEventGenerators.OnStatusChange += OnHostsStatusChange;
            h.MessageEventGenerators.OnGetHostName += OnHostsRxHostName;
            h.MessageEventGenerators.OnRxValidAdminPassword += OnValidPassword;
            h.MessageEventGenerators.OnRxValidViewerPassword += OnValidPassword;
            h.MessageEventGenerators.OnRxHealthStatus += OnRxHealthStats;

            // this entry to the host display table 
            h.datagridRowIndex = AddToDataGridTable(h);
            m_HostsLookup.Add(h.datagridRowIndex, h);

            // spin off a validation thread for this potential host
            ValidateHostInfo(h);
        }

        void OnRxHealthStats(string statusInfo)
        {
            this.BeginInvoke((MethodInvoker)delegate { this.PushHealthStats(statusInfo); });
        }

        

        void PushHealthStats(string statusInfo)
        {

            m_StatsViewer.PushStatusInfo(statusInfo);

        }


        void LoadHostsFromFile(string hostsFile)
        {
            ClearTable();

            m_Hosts = new List<RemoteHosts>();
            m_HostsLookup = new Hashtable();

            try
            {
                string[] lines = File.ReadAllLines(hostsFile);
                for (int i =0; i < lines.Count(); i++)
                {
                    // hostIP,  hostDecription
                    string[] bits = lines[i].Split(',');
                    if (bits.Length != 2)
                    {
                        m_Log.Log("found bad line ("+ (i+1).ToString()+") in file :", ErrorLog.LOG_TYPE.INFORMATIONAL);
                        continue;
                    }
                    RemoteHosts h = new RemoteHosts(m_AppData, false);
                    h.hostIPstr = bits[0];

                    // load up handlers for receiving messages from the remote host

                 //   h.MessageEventGenerators. OnLoginResult += HandlePasswordTestResult;

                    string test = h.hostIPstr.ToLower();
                    if (test.Equals("localhost"))
                        h.hostIPstr = "127.0.0.1";

                    h.hostDescription = bits[1].Trim(' '); ;

                    h.MessageEventGenerators.OnRxInvalidPassword += OnInvalidPassword;

                    h.MessageEventGenerators.OnStatusChange += OnHostsStatusChange;
                    h.MessageEventGenerators.OnGetHostName += OnHostsRxHostName;
                    h.MessageEventGenerators.OnRxValidAdminPassword += OnValidPassword;
                    h.MessageEventGenerators.OnRxValidViewerPassword += OnValidPassword;
                    h.MessageEventGenerators.OnRxHealthStatus += OnRxHealthStats;

                   
                    // this entry to the host display table 
                    h.datagridRowIndex = AddToDataGridTable(h);
                    m_HostsLookup.Add(h.datagridRowIndex, h);

                    // spin off a validation thread for this potential host
                    ValidateHostInfo(h);
                }
            }
            catch(Exception ex)
            {
                m_Log.Log(" LoadHostsFromFile ex: " + ex.Message, ErrorLog.LOG_TYPE.INFORMATIONAL);
            }
        }

        void ClearTable()
        {
            for (int i = dataGridViewRemoteHostList.Rows.Count - 1; i >= 0; i--)
            {
                if (dataGridViewRemoteHostList.Rows[i].IsNewRow) continue;

                dataGridViewRemoteHostList.Rows.RemoveAt(i);
            }
        }

        bool ValidateHostInfo(RemoteHosts h)
        {

            try
            {
                // 1. is the supplied IR address a valid IP address or is it a host name?
                System.Net.IPAddress remoteIPAddress = null;
                if (!System.Net.IPAddress.TryParse(h.hostIPstr, out remoteIPAddress))
                {
                    // if it was not a valid ip address, perhaps it was a host name instead
                    System.Net.IPHostEntry GetIPHost = System.Net.Dns.GetHostEntry(h.hostIPstr);
                    h.hostIP = GetIPHost.AddressList[0];
                }
                else
                {
                    h.hostIP = System.Net.IPAddress.Parse(h.hostIPstr);
                }

                // 2. is the supplied host IP reachable via ping?

                h.StartValidation();

//                h.StartValidation(OnHostsStatusChange, OnHostsRxHostName);

            }
            catch(Exception ex)
            {
                m_Log.Log("ValidateHostInfo ex: " + ex.Message, ErrorLog.LOG_TYPE.INFORMATIONAL);
            }
            return (true);
        }

        void OnHostsRxHostName(RemoteHosts host, string name)
        {
          
                UpdateGridHostNameCell(host.datagridRowIndex, name);

        }

        void OnHostsStatusChange(RemoteHosts host, RemoteHosts.HOST_STATUS status, string details)
        {
            if (status == RemoteHosts.HOST_STATUS.LOGGEDOUT)
            {
                changeStateToLoggedOut();
                return;
            }

            if (details.Contains("actively refused")) details = "LPR Service not running on server";

            if (host.ValidationState == RemoteHosts.VALIDATION_STATE.COMPLETED) return;// dont print the "not connected" status, for validation connections leave the "connected"

            UpdateGridStatusCell(host.datagridRowIndex, details, host.StatusColorCode);

        }

        void UpdateGridHostNameCell(int row, string name)
        {
            if (dataGridViewRemoteHostList.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate { this.UpdateGridHostNameCell(row, name); });
            }
            else
            {
                dataGridViewRemoteHostList.Rows[row].Cells[0].Value = name;
            }
        }

        void UpdateGridStatusCell(int row, string status, Color code)
        {
            if (dataGridViewRemoteHostList.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate { this.UpdateGridStatusCell(row, status, code); });
            }
            else
            {

                dataGridViewRemoteHostList.Rows[row].Cells[3].Style.ForeColor = code;
                dataGridViewRemoteHostList.Rows[row].Cells[3].Value = status;

            }
        }

        int AddToDataGridTable(RemoteHosts h)
        {
            // colRemoteName, colIPAddress,colDescription, colStatus


            // do we have a host name?
            string hostName;
            if (h.hostName == null) hostName = " ";
            else hostName = h.hostName;

            // do we have an IP address?
            string ipaddress;
            if (h.hostIPstr != null) ipaddress = h.hostIPstr;
            else ipaddress = " ";

            string description;
            if (h.hostDescription == null) description = " ";
            else description = h.hostDescription;

            string status = "starting validation...";

            //
            // n is the new index. The cells must also be accessed by an index.
            // In this example, there are four cells in each row.
            //
            int n = dataGridViewRemoteHostList.Rows.Add();

            dataGridViewRemoteHostList.Rows[n].Cells[0].Value = hostName;
            dataGridViewRemoteHostList.Rows[n].Cells[1].Value = ipaddress;


            dataGridViewRemoteHostList.Rows[n].Cells[2].Value = description;

            dataGridViewRemoteHostList.Rows[n].Cells[3].Style.WrapMode = DataGridViewTriState.True;

            dataGridViewRemoteHostList.Rows[n].Cells[3].Value = status;

            return (n);
        }


        private void dataGridViewRemoteHostList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

            
            if (((RemoteHosts)m_AppData.CurrentlyLoggedInRemoteServer).LoggedIn) return; // don't select a new server until the user logs out of the current one
            

            RemoteHosts host =(RemoteHosts) m_HostsLookup[e.RowIndex];
            if (host == null) return;
            
            if (host.hostName != null)
            {
                // its a verified host, select it

                m_AppData.CurrentlyLoggedInRemoteServer = (RemoteHosts)host;

                textBoxSelectedSystem.ForeColor = Color.Black;
                textBoxSelectedSystem.Text = host.HostNameStatus;

                SetLoginButtonStatus(host);
            }
        }

      
        void SetLoginButtonStatus(RemoteHosts host)
        {
            textBoxSelectedSystem.ForeColor = Color.Black;
            textBoxSelectedSystem.Text = host.HostNameStatus;

            if (host.LoggedIn)
            {
                // allow logout
             

                buttonLogout.Location = buttonLoginAsAdmin.Location;
                buttonLogout.Enabled = true;
                buttonLogout.Visible = true;

                buttonLoginAsAdmin.Visible = false;
                buttonLoginAsAdmin.Enabled = false;
                
                buttonLoginAsViewer.Visible = false;
                buttonLoginAsViewer.Enabled = false;
            }
            else
            {
                // allow login
          

                buttonLogout.Enabled = false;
                buttonLogout.Visible = false;

                buttonLoginAsAdmin.Visible = true;
                buttonLoginAsAdmin.Enabled = true;

                buttonLoginAsViewer.Visible = true;
                buttonLoginAsViewer.Enabled = true;

            }
        }

      

      

        void OnValidPassword()
        {

            // this runs on the network thread


            this.BeginInvoke((MethodInvoker)delegate { this.SetPasswordPromptResult(true); });
            this.BeginInvoke((MethodInvoker)delegate { this.SetLoginButtonStatus(((RemoteHosts) m_AppData.CurrentlyLoggedInRemoteServer)); });
            this.BeginInvoke((MethodInvoker)delegate { this.SetCurrentHostGridTableStatusCellLoginStatus(); });

            // notify consumers of the change in status
            MessageGenerators.OnNewServerSelected(((RemoteHosts)m_AppData.CurrentlyLoggedInRemoteServer));

            Thread.Sleep(2000);
        
            this.BeginInvoke((MethodInvoker)delegate { this.ClosePWPrompt(); });

        }

        void SetPasswordPromptResult(bool result)
        {

            if (result == true) // we are logged in
            {
            }

            if (m_PWPrompt == null) return;


            m_PWPrompt.PasswordValidationResult(result);
        }

        void SetCurrentHostGridTableStatusCellLoginStatus()
        {

           int row = ((RemoteHosts) m_AppData.CurrentlyLoggedInRemoteServer).datagridRowIndex;

           dataGridViewRemoteHostList.Rows[row].Cells[3].Style.ForeColor = ((RemoteHosts)m_AppData.CurrentlyLoggedInRemoteServer).StatusColorCode;
           dataGridViewRemoteHostList.Rows[row].Cells[3].Value = ((RemoteHosts)m_AppData.CurrentlyLoggedInRemoteServer).LoggedInStatusMessage;

        }

        void ClosePWPrompt()
        {
            if (m_PWPrompt == null) return;

            if (m_PWPrompt.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate { this.ClosePWPrompt(); });
            }
            else
            {
                m_PWPrompt.Dispose();
                m_PWPrompt = null;
            }
        }

        void OnInvalidPassword()
        {
            this.BeginInvoke((MethodInvoker)delegate { this.SetPasswordPromptResult(false); });
        }

        private void buttonLoginAsAdmin_Click(object sender, EventArgs e)
        {
            if (((RemoteHosts)m_AppData.CurrentlyLoggedInRemoteServer).NullHost)
            {
                textBoxSelectedSystem.ForeColor = Color.Red;
                textBoxSelectedSystem.Text = "Please select a remote system to log into first";
                System.Media.SystemSounds.Beep.Play();
                return;
            }
           

            if (m_PWPrompt != null)
            {
                if (m_PWPrompt.Visible) return;
            }

            m_PWPrompt = new LoginPWPrompt("Login as Admin");

            m_PWPrompt.StartPosition = FormStartPosition.CenterScreen;

            m_PWPrompt.OnPasswordEntered -= ((RemoteHosts) m_AppData.CurrentlyLoggedInRemoteServer).LoginAsViewer;
          
            m_PWPrompt.OnPasswordEntered += ((RemoteHosts) m_AppData.CurrentlyLoggedInRemoteServer).LoginAsAdmin;
            m_PWPrompt.Show();
        }

        private void buttonLoginAsViewer_Click(object sender, EventArgs e)
        {
            if (((RemoteHosts) m_AppData.CurrentlyLoggedInRemoteServer).NullHost )
            {
                textBoxSelectedSystem.ForeColor = Color.Red;
                textBoxSelectedSystem.Text = "Please select a remote system to log into first";
                System.Media.SystemSounds.Beep.Play();
                return;
            }
           

            if (m_PWPrompt != null)
            {
                if (m_PWPrompt.Visible) return;
            }

            m_PWPrompt = new LoginPWPrompt("Login as Viewer");

            m_PWPrompt.StartPosition = FormStartPosition.CenterScreen;

            m_PWPrompt.OnPasswordEntered -= ((RemoteHosts) m_AppData.CurrentlyLoggedInRemoteServer).LoginAsAdmin;

            m_PWPrompt.OnPasswordEntered +=  ((RemoteHosts) m_AppData.CurrentlyLoggedInRemoteServer).LoginAsViewer;
            m_PWPrompt.Show();
        }

        private void buttonLogout_Click(object sender, EventArgs e)
        {
            ((RemoteHosts)m_AppData.CurrentlyLoggedInRemoteServer).CloseConnection();
            
            changeStateToLoggedOut();

            // no currently logged in server
            //m_AppData.CurrentlyLoggedInRemoteServer = null;
            m_AppData.CurrentlyLoggedInRemoteServer = (object)new RemoteHosts(m_AppData, true);

            textBoxSelectedSystem.ForeColor = Color.Red;
            textBoxSelectedSystem.Text = "No server selected";
        }

        void changeStateToLoggedOut()
        {

            // notify all consumers that this host is now disconnected
            MessageGenerators.OnCurrentServerLoggedOut((RemoteHosts)m_AppData.CurrentlyLoggedInRemoteServer);

            // change the status in the hosts table
           
            this.BeginInvoke((MethodInvoker)delegate { this.SetLoginButtonStatus(((RemoteHosts)m_AppData.CurrentlyLoggedInRemoteServer)); });
           
        }

        private void textBoxSelectedSystem_TextChanged(object sender, EventArgs e)
        {

        }


        public class MessageGeneratorEvents
        {
            public MessageGeneratorEvents(SelectRemoteSystem parent)
            {
                Parent = parent;
            }

            SelectRemoteSystem Parent;

            public delegate void OnNewServerSelectedEvent (RemoteHosts host);
            public OnNewServerSelectedEvent OnNewServerSelected;

            public delegate void OnCurrentServerLoggedoutEvent (RemoteHosts host);
            public OnCurrentServerLoggedoutEvent OnCurrentServerLoggedOut;

        }

        private void buttonUnloadServerList_Click(object sender, EventArgs e)
        {




            if (((RemoteHosts)m_AppData.CurrentlyLoggedInRemoteServer).LoggedIn)
                buttonLogout_Click(sender, e);
         

            ClearTable();
            UserSettings.Remove (UserSettingTags.REMOTEHOSTS_LastHostFileUsed);
            m_HostFile = null;
            m_Hosts = null;
            m_HostsLookup = null;
        }

        private void buttonReValidateConnections_Click(object sender, EventArgs e)
        {
            if (m_HostFile != null)
                LoadHostsFromFile(m_HostFile);
            else
                AddLocalDefaultHost();
        }

        private void dataGridViewStatistics_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

    }
}
