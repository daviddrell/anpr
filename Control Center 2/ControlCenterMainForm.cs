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
using System.Threading;

namespace Control_Center
{

    public partial class ControlCenterMainForm : Form
    {
        public ControlCenterMainForm()
        {
            InitializeComponent();

       //     m_ScreenLogger = new ScreenLogger();// debug tool


            // load up the AppData object so the rest of the app has acccess to global resources

            m_AppData = new APPLICATION_DATA();
           // m_AppData.LogToScreen = m_ScreenLogger;

            m_Log = new ErrorLog(m_AppData);
            m_AppData.Logger = m_Log;

        //    if ( m_ScreenLogger != null)  m_ScreenLogger.Show(); // for debug

            // network communication stack
            m_AppData.RCSProtocol = (object)new RCS_Protocol.RCS_Protocol(m_AppData, null);
            m_AppData.TCPClient = (object)new RCSClientLib.RCSClient( m_AppData);


            // ordered shutdown

            this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);
            

            // actions on entering a tab page
            tabControlMain.Click += new EventHandler(tabControlMain_Click);

            tabControlMain.Size = this.Size;

            // instantiate and install the Load SelectServer User control
            LoadSelectServerUC();

            // instantiate and install the LiveView page components
            LoadLiveViewUC();

            // load server changes event handlers

            LoadServerEventHandlers();
       

            // start on the select server tab
            tabControlMain.SelectedTab = tabPageSelectServer;
            m_CurrentPage = tabControlMain.SelectedTab;
        }


        // ////////////////////////////////////////////////////////////////////
        //
        //   Global variables
        //

        APPLICATION_DATA m_AppData; // share will entire application
     //   ScreenLogger m_ScreenLogger; // debug tool
        ErrorLog m_Log;


        protected override void OnPaint(PaintEventArgs e)
        {
            tabControlMain.Size = this.Size;

            base.OnPaint(e);
           
        }



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
               // LoadSelectServerUC();
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
            tabPageSelectServer.Controls.Add(m_SelectRemoteSystem);
            m_AppData.SelectServer = (object)m_SelectRemoteSystem;
        }



        // ////////////////////////////////////////////////////////////////////
        //
        //   remote server changes
        //
 
      
        void LoadServerEventHandlers()
        {
          
            m_SelectRemoteSystem.MessageGenerators.OnCurrentServerLoggedOut += OnCurrentServerLoggedOut;
            m_SelectRemoteSystem.MessageGenerators.OnNewServerSelected += OnNewServerSelected;
        }

        void OnNewServerSelected(RemoteHosts host)
        {         
                // LiveView has registered its own notfication on host logged in

        }

        void OnCurrentServerLoggedOut(RemoteHosts host)
        {
            this.BeginInvoke((MethodInvoker)delegate { this.UnLoadHostFromControls(); });
        }

        void UnLoadHostFromControls()
        {
          // LiveView has registered its own notification on host logged out           
        }

        // ////////////////////////////////////////////////////////////////////
        //
        //   Ordered shutown
        //

        void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_Log.Log("ControlCenterMainForm closing application", ErrorLog.LOG_TYPE.FATAL);
            m_AppData.CloseApplication();
        }


        // ////////////////////////////////////////////////////////////////////
        //
        //   Live View controls
        //

        LiveView m_LiveView;
        void LoadLiveViewUC()
        {
            m_LiveView = new LiveView(m_AppData);
            tabPageViewLive.Controls.Add(m_LiveView);
        }

        private void tabPageViewLive_Click(object sender, EventArgs e)
        {

        }

        private void tabPageViewOffline_Click(object sender, EventArgs e)
        {

        }

     

    }
}
