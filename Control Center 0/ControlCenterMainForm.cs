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
using ApplicationDataClass;
using ScreenLoggerLib;
using RCSClientLib;
using RCS_Protocol;
using ErrorLoggingLib;

namespace Control_Center
{

    public partial class ControlCenterMainForm : Form
    {
        public ControlCenterMainForm()
        {
            InitializeComponent();

            m_ScreenLogger = new ScreenLogger();// debug tool
         

            // load up the AppData object so the rest of the app has acccess to global resources

            m_AppData = new APPLICATION_DATA();
            m_AppData.LogToScreen = m_ScreenLogger;
            
            m_Log = new ErrorLog(m_AppData);
            m_AppData.Logger = m_Log;

            m_ScreenLogger.Show();

            m_AppData.RCSProtocol = (object)new RCS_Protocol.RCS_Protocol(m_AppData,null);
            m_AppData.TCPClient = (object)new RCSClientLib.RCSClient(null, m_AppData);

        

            this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);

            tabControlMain.Click += new EventHandler(tabControlMain_Click);

            // not logged in

            m_AppData.LoggedIn = APPLICATION_DATA.LoggedInAs.NOT_LOGGED_IN;


            // start on the select server tab
            tabControlMain.SelectedTab = tabPageSelectServer;
            m_CurrentPage = tabControlMain.SelectedTab;
            LoadSelectServerUC();
        }

        // ////////////////////////////////////////////////////////////////////
        //
        //   Global variables
        //

        APPLICATION_DATA m_AppData; // share will entire application
        ScreenLogger m_ScreenLogger; // debug tool
        ErrorLog m_Log;
        

        // ////////////////////////////////////////////////////////////////////
        //
        //   main tab control tab click processing
        //

        void tabControlMain_Click(object sender, EventArgs e)
        {
            // clean up pages being navigated from
            //if (m_CurrentPage == tabPageLogin)
            //{
            //    for (int i = m_CurrentPage.Controls.Count-1; i >= 0 ; i--)
            //    {
            //        m_CurrentPage.Controls.RemoveAt(i);
            //    }
            //}

            // process new page selection

            if (tabControlMain.SelectedTab == tabPageSelectServer)
            {
                m_CurrentPage = tabControlMain.SelectedTab;
                LoadSelectServerUC();
            }

            //if (tabControlMain.SelectedTab == tabPageLogin)
            //{
            //    m_CurrentPage = tabPageLogin;
            //    tabPageLogin_Click(sender, e);
            //}
            //else if (tabControlMain.SelectedTab == tabPageWorkRemotely_SelectRemoteSystem)
            //{
            //    m_SelectRemoteSystem = new SelectRemoteSystem(m_AppData);
            //    m_CurrentPage.Controls.Add(m_SelectRemoteSystem);
            //}
        }

        SelectRemoteSystem m_SelectRemoteSystem;
        TabPage m_CurrentPage;

        void LoadSelectServerUC()
        {
            m_SelectRemoteSystem = new SelectRemoteSystem(m_AppData);
            m_CurrentPage.Controls.Add(m_SelectRemoteSystem);
        }


        // ////////////////////////////////////////////////////////////////////
        //
        //   Ordered shutown
        //

        void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_AppData.CloseApplication();
        }

     
       
       
        private void tabPageWorkLocally_Click(object sender, EventArgs e)
        {

        }

        // ////////////////////////////////////////////////////////////////////
        //
        //   Login and    Passwords
        //


        //  Main Login Tab Page

    

        private void tabPageLogin_Click(object sender, EventArgs e)
        {
            //// are we logged in?
            //if (m_AppData.LoggedIn == APPLICATION_DATA.LoggedInAs.NOT_LOGGED_IN)
            //{
            //    notLoggedIn = new NotLoggedInUC(OnChooseLogin);
            //    notLoggedIn.Location = new Point(100, 100);
            //    notLoggedIn.Visible = true;
            //    tabPageLogin.Controls.Add(notLoggedIn);
            //}
            //else
            //{
            //    PaintLogoutOption();
            //}
        }


        //  receieved new Admin Password

        void OnNewAdminPasswordEntered(string pw)
        {
            string pwEncrypted = EncryptString.encryptString(pw);
            UserSettings.Set(UserSettingTags.PWControlCenterAdmin, pwEncrypted);
            m_AppData.LoggedIn = APPLICATION_DATA.LoggedInAs.AS_ADMIN;
        }

        //   Log in As Admininstrator

        private void LoadPanelLoginAsAdministrator()
        {
          // is this the first time login?

            //string adminPwEncrypted = UserSettings.Get(UserSettingTags.PWControlCenterAdmin);
            //if (adminPwEncrypted == null)
            //{
            //    FirstTimeLogin firstLogin = new FirstTimeLogin(OnNewAdminPasswordEntered, "Login as Administrator");
            //    firstLogin.Location = new Point(100, 100);
            //    tabPageLogin.Controls.Add(firstLogin);
            //}
        }


    

        void OnChooseLogin(NotLoggedInUC.LOGIN_AS loginAs)
        {
            //if (loginAs == NotLoggedInUC.LOGIN_AS.ADMIN)
            //{
            //    tabPageLogin.Controls.Remove(notLoggedIn);
            //    notLoggedIn = null;

            //    LoadPanelLoginAsAdministrator();
            //}
        }

        void PaintLogoutOption()
        {

        }

    
    }
}
