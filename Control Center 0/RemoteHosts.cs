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
    
    class RemoteHosts
    {
        public RemoteHosts(APPLICATION_DATA appData)
        {
            m_AppData = appData;
        //    TCPClient =(RCSClient.RCSClient) m_AppData.TCPClient; // for the primary server the user is interacting with

            MessageEventHandlers = new MessageEventHandlersClass(this);
            MessageEventGenerators = new MessageEventGeneratorsClass(this);

            TCPClient = new RCSClientLib.RCSClient(null,m_AppData);// each host gets its own client connection

            // load up handlers for receiving messages from the TCP Client object

            TCPClient.MessageEventGenerators.OnRxHostName += MessageEventHandlers.OnRxServerName;
            TCPClient.MessageEventGenerators.OnRxValidAdminPW += MessageEventHandlers.OnRxValidAdminPassword;
            TCPClient.MessageEventGenerators.OnRxValidViewerPW += MessageEventHandlers.OnRxValidViewerPassword;
            TCPClient.MessageEventGenerators.OnRxInvalidPassword += MessageEventHandlers.OnRxInvalidPassword;

            RCSProtocol = (RCS_Protocol.RCS_Protocol) m_AppData.RCSProtocol;
           
        }


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
        public enum HOST_STATUS {PING_SUCCESS, PING_FAILURE, CONNECT_SUCCESS, CONNECT_FAILURE}

        public enum VALIDATION_STATE { NONE, STARTING, COMPLETED };
        public VALIDATION_STATE ValidationState = VALIDATION_STATE.NONE;

        Ping m_pinger;
        public bool LoggedIn=false;
        public const string NOT_LOGGED_IN = "Not logged in";
        public const string LOGGED_IN = "Logged in as ";
        public string LoggedInStatusMessage = "Not logged in";
        bool m_RunningTestConnection = false;

        // methods
        //
        //   ping - return results to callback, then start test server connection
        //
        //   test server connection, request host system name from remote host(via RCS protocol) - return results to callback   
        //
        //   

        bool m_Stop = false;

        public void Stop()
        {
            m_Stop = true;
            
            TCPClient.Stop();

            LoggedIn = false;
            LoggedInStatusMessage = NOT_LOGGED_IN;

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
        public delegate void LoginResult(RemoteHosts host, LOGIN_TYPE ltype);
    //    public LoginResult OnRxValidAdminPassword;

        public void LoginAsAdmin(string adminPW)
        {
          
            m_RunningTestConnection = false;

            ValidationState = VALIDATION_STATE.NONE;

            StartConnection();

            TCPClient.SendVerifyAdminPassword(adminPW);

        }

        public void LoginAsViewer(string viewerPW)
        {
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

            TCPClient.Connect(hostIPstr, false, MessageEventHandlers.OnTCPConnectStatusChange);

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

          
        }


        public class MessageEventHandlersClass
        {
            RemoteHosts Parent;

            public MessageEventHandlersClass(RemoteHosts parent)
            {
                Parent = parent;
            }

            public void OnRxValidAdminPassword()
            {
                Parent.LoggedIn = true;
                Parent.LoggedInStatusMessage = RemoteHosts.LOGGED_IN + LOGIN_TYPE.ADMIN.ToString();
                
                if (Parent.MessageEventGenerators.OnRxValidAdminPassword != null) Parent.MessageEventGenerators.OnRxValidAdminPassword();
            }

            public void OnRxValidViewerPassword()
            {
                Parent.LoggedIn = true;
                Parent.LoggedInStatusMessage = RemoteHosts.LOGGED_IN + LOGIN_TYPE.VIEWER.ToString();

                if (Parent.MessageEventGenerators.OnRxValidViewerPassword != null) Parent.MessageEventGenerators.OnRxValidViewerPassword();
            }

            public void OnRxInvalidPassword(string message)
            {
                Parent.LoggedIn = false;
                Parent.LoggedInStatusMessage = RemoteHosts.NOT_LOGGED_IN + LOGIN_TYPE.VIEWER.ToString();

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
                        Parent.StatusColorCode = Color.Red;
                        Parent.MessageEventGenerators.OnStatusChange(Parent, HOST_STATUS.CONNECT_FAILURE, details);
                    }
                }
            }


         

        }
    }

}
