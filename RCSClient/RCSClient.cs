using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using RCS_Protocol;
using System.Windows.Forms;
using ApplicationDataClass;
using ErrorLoggingLib;


namespace RCSClientLib
{

    // handles the TCP connection to the remote server

    // 
    //  Connect - starts a connection to the remote server
    //
    //  Stop - ends a connection to the remote server
    //
    //  SendLiveViewRequest - sends a request to remote server for the next jpeg image in a live view
    //
    // 

    public partial class RCSClient
    {
        TcpClient m_client;

        enum DESIRED_STATE { WANT_TO_BE_CONNECTED, DONT_WANT_TO_BE_CONNECTED }
        public enum STATE { CONNECTED, NOT_CONNECTED }

        public delegate void handleRxJpeg(byte[] jpeg);
     //   handleRxJpeg m_SendNewJpeg;
        string m_AdminPW;
        string m_ViewerPW;
        
        STATE           m_STATE;
        DESIRED_STATE m_DesiredState;

        RCS_Protocol.RCS_Protocol m_RCSProtocol;

        APPLICATION_DATA m_AppData;
        ErrorLog m_Log;
      

        public RCSClient(/* handleRxJpeg callback, */ APPLICATION_DATA appData)
        {
            m_AppData = appData;
            m_AppData.AddOnClosing(OnAppClose, APPLICATION_DATA.CLOSE_ORDER.MIDDLE);
            m_Log = (ErrorLog) m_AppData.Logger;

            m_AdminPW = "";
            m_ViewerPW ="";

            MessageEventGenerators = new MessageEventGeneratorClass();

          //  m_SendNewJpeg = callback;

            m_SendPacketRequests = new SEND_PACKET_REQUESTS();
          
            m_STATE = STATE.NOT_CONNECTED;
            m_DesiredState = DESIRED_STATE.DONT_WANT_TO_BE_CONNECTED;

            m_RCSProtocol = new RCS_Protocol.RCS_Protocol(m_AppData,null);

        }


        public void OnAppClose()
        {
            m_DesiredState = DESIRED_STATE.DONT_WANT_TO_BE_CONNECTED;
            CloseConnection();
        }

        public void Stop()
        {
            m_DesiredState = DESIRED_STATE.DONT_WANT_TO_BE_CONNECTED;
            m_ViewerPW = "";
            m_AdminPW = "";
          
            CloseConnection();

            if (m_OnConnectedStatusChange != null) m_OnConnectedStatusChange(STATE.NOT_CONNECTED, "Connection Closed");
        }

        NetworkStream m_Stream = null;
        public delegate void OnConnectStatusChange(STATE st, string details);
        OnConnectStatusChange m_OnConnectedStatusChange;

        public void Connect(String server, bool retryConnection, OnConnectStatusChange del)
        {
            m_DesiredState = DESIRED_STATE.WANT_TO_BE_CONNECTED;

            m_OnConnectedStatusChange = del;

            if (retryConnection)
                StartMaintainConnectionThread(server);
            else
                StartOneConnectionAttemptThread(server);
        }

        ParameterizedThreadStart m_OneAttemptPTS;
        Thread m_OneAttemptConnectionThread;
        void StartOneConnectionAttemptThread(string server)
        {
            m_OneAttemptPTS = new ParameterizedThreadStart(ConnectOnce);
            m_OneAttemptConnectionThread = new Thread(m_OneAttemptPTS);
            m_OneAttemptConnectionThread.Start(server);
        }

        void ConnectOnce(object serverObj)
        {
            string server = (string)serverObj;

            try
            {
                m_CloseConnection = false;

                Int32 port = 13000;
                m_client = new TcpClient(server, port);

                m_Stream = m_client.GetStream();

                // we are connected, start  send & receive threads
                m_STATE = STATE.CONNECTED;
                StartReceiveThread();
                StartSendThread();
                m_OnConnectedStatusChange(STATE.CONNECTED, "connected");
            }
            catch (Exception ex)
            {
                CloseConnection();
                if (m_OnConnectedStatusChange != null) m_OnConnectedStatusChange(STATE.NOT_CONNECTED, ex.Message);
                m_Log.Log("Connect " + ex.Message, ErrorLog.LOG_TYPE.INFORMATIONAL);
            }

           
        }



        ParameterizedThreadStart m_MaintainConnectionPTS;
        Thread                   m_MaintainConnectionThread;
        void StartMaintainConnectionThread(string server)
        {
            m_MaintainConnectionPTS = new ParameterizedThreadStart(MaintainConnection);
            m_MaintainConnectionThread = new Thread(m_MaintainConnectionPTS);
            m_MaintainConnectionThread.Start(server);
        }

        void MaintainConnection(object serverObj)
        {
            string server = (string)serverObj;

            // keep re-trying as long as want to be connected

            while (m_DesiredState == DESIRED_STATE.WANT_TO_BE_CONNECTED)
            {
                if (m_STATE == STATE.CONNECTED)
                {
                    Thread.Sleep(1000);
                    continue;
                }

                // Create a TcpClient.
                // Note, for this client to work you need to have a TcpServer 
                // connected to the same address as specified by the server, port
                // combination.
                try
                {
                    m_CloseConnection = false;

                    Int32 port = 13000;
                    m_client = new TcpClient(server, port);

                    m_Stream = m_client.GetStream();

                    // we are connected, start  send & receive threads
                    m_STATE = STATE.CONNECTED;
                    
                    if (m_OnConnectedStatusChange != null) m_OnConnectedStatusChange(STATE.CONNECTED, "connected");

                    StartReceiveThread();
                    StartSendThread();
                }
                catch (Exception ex)
                {
                    CloseConnection();
                    if (m_OnConnectedStatusChange != null) m_OnConnectedStatusChange(STATE.NOT_CONNECTED, ex.Message);
                    m_Log.Log("Connect " + ex.Message, ErrorLog.LOG_TYPE.INFORMATIONAL);
                }

                Thread.Sleep(1000);// re-try once per second
            }
        }



        bool m_CloseConnection = false;

        void CloseConnection()
        {
            try
            {
                if (m_client != null) m_client.Client.Shutdown(SocketShutdown.Both);
                if (m_client != null) m_client.Client.Disconnect(false);

                m_CloseConnection = true;
                if (m_Stream != null) m_Stream.Close();
                if (m_client != null) m_client.Close();

                m_STATE = STATE.NOT_CONNECTED;

                m_Stream = null;
                m_client = null;
            }
            catch { }

        }
    }
}
