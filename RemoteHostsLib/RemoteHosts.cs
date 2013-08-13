using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using ApplicationDataClass;
using RCSClientLib;
using RCS_Protocol;
using System.Drawing;

namespace Control_Center
{
    
    public class RemoteHosts
    {
        public RemoteHosts(APPLICATION_DATA appData, bool IsNullHost)
        {
            m_AppData = appData;
            m_AppData.AddOnClosing(Stop, APPLICATION_DATA.CLOSE_ORDER.FIRST);
            
            singleton = new object();

            m_IsNullHost = IsNullHost;

            MessageEventHandlers = new MessageEventHandlersClass(this);
            MessageEventGenerators = new MessageEventGeneratorsClass(this);

            TCPClient = new RCSClientLib.RCSClient(m_AppData);// each host gets its own client connection

            // load up handlers for receiving messages from the TCP Client object

            TCPClient.MessageEventGenerators.OnRxHostName += MessageEventHandlers.OnRxServerName;
            TCPClient.MessageEventGenerators.OnRxValidAdminPW += MessageEventHandlers.OnRxValidAdminPassword;
            TCPClient.MessageEventGenerators.OnRxValidViewerPW += MessageEventHandlers.OnRxValidViewerPassword;
            TCPClient.MessageEventGenerators.OnRxInvalidPassword += MessageEventHandlers.OnRxInvalidPassword;
            TCPClient.MessageEventGenerators.OnRxChannelList += OnReceiveChannels;
            TCPClient.MessageEventGenerators.OnRxJpeg += MessageEventHandlers.OnRxJpeg;
            TCPClient.MessageEventGenerators.OnRxHealthStatus += MessageEventHandlers.OnRxStats;

            RCSProtocol = (RCS_Protocol.RCS_Protocol) m_AppData.RCSProtocol;

            StartHeatlthStatusPolling();
           
        }
        object singleton;
        bool m_IsNullHost;
        public bool NullHost { get { lock (singleton) { return m_IsNullHost; } } }
        public MessageEventHandlersClass MessageEventHandlers;
        public MessageEventGeneratorsClass MessageEventGenerators;

        RCS_Protocol.RCS_Protocol RCSProtocol;
        RCSClientLib.RCSClient TCPClient;
        APPLICATION_DATA m_AppData;

        public Color StatusColorCode;
        public int datagridRowIndex;
        public string hostIPstr;
        public IPAddress hostIP;
    //    public string HostDNS;
        public string hostDescription;
        public string hostName;
        
     
        public Thread NetThread;
        public enum HOST_STATUS {PING_SUCCESS, PING_FAILURE, CONNECT_SUCCESS, CONNECT_FAILURE, LOGGEDOUT}

        public enum VALIDATION_STATE { NONE, STARTING, COMPLETED };
        public VALIDATION_STATE ValidationState = VALIDATION_STATE.NONE;

        Ping m_pinger;
        public bool LoggedIn=false;
        public const string NOT_LOGGED_IN = "Not logged in";
        public const string LOGGED_IN = "Logged in as ";
        public string LoggedInStatusMessage = "Not logged in";
        bool m_RunningTestConnection = false;



        Thread m_RequestHealthStatusThread;
        void StartHeatlthStatusPolling()
        {
            m_RequestHealthStatusThread = new Thread(PollForHealthStatus);
            m_RequestHealthStatusThread.Start();
        }

        void PollForHealthStatus()
        {
            while (!m_Stop)
            {
                while (LoggedIn && ! m_RunningTestConnection )
                {

                    string pw = GetCurrentPW();
                    if (pw == null) continue;

                    TCPClient.SendGetHealthStatsRequest(pw);

                    if (m_Stop) break;
                    Thread.Sleep(500);
                }

                Thread.Sleep(1);
            }
        }


        string GetCurrentPW()
        {
            string pw=null;
            if (LoggedInAs == LOGIN_TYPE.ADMIN)
                pw = AdminPW;
            else if (LoggedInAs == LOGIN_TYPE.VIEWER)
                pw = ViewerPW;

            return (pw);
        }

        public string[] GetChannelList()
        {
            m_ChannelsReceived = false;

            string pw = GetCurrentPW();
            if (pw == null) return (null);

            TCPClient.SendGetChannelsRequest(pw );

            int timeout = 20000;
            while (!m_ChannelsReceived && timeout > 0)
            {
                timeout--;
                Thread.Sleep(1);
            }

            if (m_ChannelsReceived) return (m_Channellist);
            else return (null);
        }

        bool m_ChannelsReceived = false;
        string[] m_Channellist;
        void OnReceiveChannels(string[] channels)
        {
            m_Channellist = channels;
            m_ChannelsReceived = true;
        }

        bool m_Stop = false;

        public void Stop()
        {
            m_Stop = true;
            
            TCPClient.Stop();

            LoggedIn = false;
            LoggedInStatusMessage = NOT_LOGGED_IN;

        }

        public void CloseConnection()
        {
            TCPClient.Stop();
          
        }

        public string HostNameStatus
        {
            set { }
            get
            {
                if (hostName == null) hostName = "";
                
                return (hostName+" : " + LoggedInStatusMessage);
            }
        }

        public enum LOGIN_TYPE { ADMIN, VIEWER }
        private LOGIN_TYPE LoggedInAs;
      //  public delegate void LoginResult(RemoteHosts host, LOGIN_TYPE ltype);
   
        private string AdminPW;
        private string ViewerPW;


        public void LoginAsAdmin(string adminPW)
        {
            AdminPW = adminPW;
            ViewerPW = "";
          
            m_RunningTestConnection = false;

            ValidationState = VALIDATION_STATE.NONE;

            StartConnection();

            TCPClient.SendVerifyAdminPassword(adminPW);

        }

        public void LoginAsViewer(string viewerPW)
        {
            AdminPW = "";
            ViewerPW = viewerPW;
            m_RunningTestConnection = false;

            ValidationState = VALIDATION_STATE.NONE;

            StartConnection();

            TCPClient.SendVerifyViewerPassword(viewerPW);
        }

       // public void StartValidation( OnStatusChange callback, OnGetHostName nameCallback)
        public void StartValidation( )
        {
            m_RunningTestConnection = true;

            ValidationState = VALIDATION_STATE.STARTING;

            StatusColorCode = Color.Red;

            NetThread = new Thread(WaitOnPing);
            NetThread.Start();
        }

        void WaitOnPing()
        {
            m_pinger = new Ping();
            PingReply reply = m_pinger.Send(hostIP);
            if (reply.Status == IPStatus.Success)
            {
                StatusColorCode = Color.FromArgb(255, 255, 0);
                MessageEventGenerators.OnStatusChange(this, HOST_STATUS.CONNECT_SUCCESS, "ping success");
                
                StartTestConnection();
            }
            else
            {
                StatusColorCode = Color.Red;
                MessageEventGenerators.OnStatusChange(this, HOST_STATUS.PING_FAILURE, "cannot reach remote system");
            }
        }

        void StartTestConnection()
        {
            NetThread = new Thread(WaitOnTestConnection);
            NetThread.Start();
        }

        void WaitOnTestConnection()
        {
          
            TCPClient.Connect(hostIPstr, false, MessageEventHandlers.OnTCPConnectStatusChange);
            
        }

        void StartConnection()
        {
            NetThread = new Thread(WaitOnConnection);
            NetThread.Start();
        }

        void WaitOnConnection()
        {

            TCPClient.Connect(hostIPstr, true, MessageEventHandlers.OnTCPConnectStatusChange);

        }


        public void SendLiveViewRequest(string channel)
        {
            string pw ;
            if ( LoggedInAs == LOGIN_TYPE.ADMIN ) pw = AdminPW;
            else pw = ViewerPW;

            TCPClient.SendLiveViewRequest(channel, pw);
        }

       
     
        public class MessageEventGeneratorsClass
        {

            public MessageEventGeneratorsClass(RemoteHosts parent)
            {
                Parent = parent;
            }
            RemoteHosts Parent;

            // host connection status has changed (validation states such as ping, connect, getHostName )

            public delegate void OnStatusChangeDel(RemoteHosts host, HOST_STATUS status, string details);
            public OnStatusChangeDel OnStatusChange;

            // notify upper layer of new host name received from the remote host IP address
            public delegate void OnGetHostNameDel(RemoteHosts host, string hostName);
            public OnGetHostNameDel OnGetHostName;

            // notify upper layer we received confirmation the admin password was accepted by the remote host
            public delegate void OnRxValidAdminPasswordEvent( );
            public OnRxValidAdminPasswordEvent OnRxValidAdminPassword;

            // notify upper layer we received confirmation the viewer password was accepted by the remote host
            public delegate void OnRxValidViewerPasswordEvent();
            public OnRxValidViewerPasswordEvent OnRxValidViewerPassword;

            // notify upper layer we received confirmation the viewer password was not accepted by the remote host
            public delegate void OnRxInvalidPasswordEvent();
            public OnRxInvalidPasswordEvent OnRxInvalidPassword;


            public delegate void ProcessRxJpegEvent(byte[] jpeg, string channel, string timeStamp, string plateReading);
            public ProcessRxJpegEvent OnRxJpeg;

            public delegate void ProcessRxHealthStatus(string statusInfo);
            public ProcessRxHealthStatus OnRxHealthStatus;
          
        }


        public class MessageEventHandlersClass
        {
            RemoteHosts Parent;

            public MessageEventHandlersClass(RemoteHosts parent)
            {
                Parent = parent;
            }

            public void OnRxStats(string statusInfo)
            {
                if (Parent.MessageEventGenerators.OnRxHealthStatus != null) Parent.MessageEventGenerators.OnRxHealthStatus(statusInfo);
            }

            public void OnRxJpeg(byte[] jpeg, string channel, string timeStamp, string plateReading)
            {
                if (Parent.MessageEventGenerators.OnRxJpeg != null) Parent.MessageEventGenerators.OnRxJpeg(jpeg, channel, timeStamp, plateReading );
            }

            public void OnRxValidAdminPassword()
            {
                Parent.LoggedIn = true;
                Parent.LoggedInAs = LOGIN_TYPE.ADMIN;
                
                Parent.LoggedInStatusMessage = RemoteHosts.LOGGED_IN + LOGIN_TYPE.ADMIN.ToString();
                Parent.StatusColorCode = Color.Green;
                if (Parent.MessageEventGenerators.OnRxValidAdminPassword != null) Parent.MessageEventGenerators.OnRxValidAdminPassword();
            }

            public void OnRxValidViewerPassword()
            {
                Parent.LoggedIn = true;
                Parent.LoggedInAs = LOGIN_TYPE.VIEWER;

                Parent.StatusColorCode = Color.Green;
                Parent.LoggedInStatusMessage = RemoteHosts.LOGGED_IN + LOGIN_TYPE.VIEWER.ToString();

                if (Parent.MessageEventGenerators.OnRxValidViewerPassword != null) Parent.MessageEventGenerators.OnRxValidViewerPassword();
            }

            public void OnRxInvalidPassword(string message)
            {
                Parent.LoggedIn = false;
                Parent.LoggedInStatusMessage = RemoteHosts.NOT_LOGGED_IN + LOGIN_TYPE.VIEWER.ToString();

                Parent.StatusColorCode = Color.Yellow;

                if (Parent.MessageEventGenerators.OnRxInvalidPassword != null) Parent.MessageEventGenerators.OnRxInvalidPassword();

                Parent.StatusColorCode = Color.Red;
                Parent.MessageEventGenerators.OnStatusChange(Parent, HOST_STATUS.CONNECT_FAILURE, "Connect failed because of Invalid Password: " + message);
            }


            public void OnRxServerName(string name)
            {
                if (Parent.m_Stop) return;

                if (Parent.m_RunningTestConnection)
                {
                    Parent.TCPClient.Stop();
                    Parent.hostName = name;
                    Parent.MessageEventGenerators.OnGetHostName(Parent, Parent.hostName);
                }
                else
                {
                    // attempting a permement connection
                    Parent.hostName = name;
                    Parent.MessageEventGenerators.OnGetHostName(Parent, Parent.hostName);
                }
            }


            public void OnTCPConnectStatusChange(RCSClientLib.RCSClient.STATE state, string details)
            {
               
                
                if (Parent.m_RunningTestConnection)
                {
                    if (state == RCSClientLib.RCSClient.STATE.CONNECTED)
                    {

                        Parent.StatusColorCode = Color.Green;
                        Parent.MessageEventGenerators.OnStatusChange(Parent, HOST_STATUS.CONNECT_SUCCESS, "connect success");
                        Parent.TCPClient.SendServerNameRequest(Parent.hostIPstr);

                        if (Parent.ValidationState == VALIDATION_STATE.STARTING)
                            Parent.ValidationState = VALIDATION_STATE.COMPLETED;// this is the end of the validation process

                    }
                    else
                    {
                            // connection is closed
                        if (Parent.LoggedIn)
                        {
                            Parent.MessageEventGenerators.OnStatusChange(Parent, HOST_STATUS.LOGGEDOUT, "logout due to connection closed");
                        }
                        Parent.LoggedIn = false;

                        Parent.StatusColorCode = Color.Red;
                     
                        Parent.MessageEventGenerators.OnStatusChange(Parent, HOST_STATUS.CONNECT_FAILURE, details);

                        if (Parent.ValidationState == VALIDATION_STATE.STARTING)
                            Parent.ValidationState = VALIDATION_STATE.COMPLETED;// this is the end of the validation process
                    }
                }
                else
                {
                    // attempting a permement connection
                    if (state == RCSClientLib.RCSClient.STATE.CONNECTED)
                    {

                        Parent.StatusColorCode = Color.Green;
                        Parent.MessageEventGenerators.OnStatusChange(Parent, HOST_STATUS.CONNECT_SUCCESS, "connect success");
                     
                    }
                    else
                    {
                        if (Parent.LoggedIn)
                        {
                            Parent.MessageEventGenerators.OnStatusChange(Parent, HOST_STATUS.LOGGEDOUT, "logout due to connection closed");
                        }
                        Parent.LoggedIn = false;

                        Parent.StatusColorCode = Color.Red;
                        Parent.MessageEventGenerators.OnStatusChange(Parent, HOST_STATUS.CONNECT_FAILURE, details);
                    }
                }
            }



        }
    }

}

