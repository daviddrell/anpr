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
using Utilities;

namespace RCSClientLib
{

    public partial class RCSClient
    {

        //  ///////////////////////////////////////////////////
        //
        //   form specific send messages
        //    



        public void SendGetHealthStatsRequest(string pw)
        {

            byte[] pkt = m_RCSProtocol.CreatePacket(pw, RCS_Protocol.RCS_Protocol.PACKET_TYPES.REQUEST_STATS);

            m_SendPacketRequests.AddRequest(pkt);
        }


        public void SendGetChannelsRequest(string pw )
        {
            
            byte[] pkt = m_RCSProtocol.CreatePacket(pw, RCS_Protocol.RCS_Protocol.PACKET_TYPES.REQUEST_CHANNEL_LIST);

            m_SendPacketRequests.AddRequest(pkt);
        }



        //   Verify Admin Password

      

        public void SendVerifyAdminPassword( string pw)
        {
        //    if (m_STATE != STATE.CONNECTED) return;

            m_AdminPW = pw;

            byte[] pkt = m_RCSProtocol.CreatePacket( RCS_Protocol.RCS_Protocol.PACKET_TYPES.REQUEST_CHECK_ADMIN_PASSWORD, pw);

            m_SendPacketRequests.AddRequest(pkt);
        }


        //   Verify Viewer Password

        public void SendVerifyViewerPassword(string pw)
        {
        //    if (m_STATE != STATE.CONNECTED) return;

            byte[] pkt = m_RCSProtocol.CreatePacket(RCS_Protocol.RCS_Protocol.PACKET_TYPES.REQUEST_CHECK_VIEWER_PASSWORD, pw);

            m_SendPacketRequests.AddRequest(pkt);
        }

      
         //   ServerName Request

        public void SendServerNameRequest( string remoteHostIP)
        {
         //   if (m_STATE != STATE.CONNECTED) return;



            byte[] pkt = m_RCSProtocol.CreatePacket(remoteHostIP, RCS_Protocol.RCS_Protocol.PACKET_TYPES.REQUEST_HOST_NAME, (string)null, (string)null);

            m_SendPacketRequests.AddRequest(pkt);
        }


        //   LiveView Request


        public void SendLiveViewRequest(string channel, string pw)
        {
       //     if (m_STATE != STATE.CONNECTED) return;

            RCS_Protocol.RCS_Protocol.LIVE_VIEW_HEADER header = new RCS_Protocol.RCS_Protocol.LIVE_VIEW_HEADER();
            header.cameraName = channel;
            header.timeStamp = " ";
            string jpegInfo = m_RCSProtocol.BuildInfoString(header);
            byte[] pkt = m_RCSProtocol.CreatePacket(null, RCS_Protocol.RCS_Protocol.PACKET_TYPES.REQUEST_LIVE_VIEW, pw, jpegInfo);

            m_SendPacketRequests.AddRequest(pkt);
        }

        //  ///////////////////////////////////////////////////
        //
        //   Send Queue
        //   
        SEND_PACKET_REQUESTS m_SendPacketRequests;

        class SEND_PACKET_REQUESTS
        {
            //Queue<RCS_Protocol.RCS_Protocol.PACKET_TYPES> m_SendPacketRequests;
            ThreadSafeQueue<byte[]> m_SendPacketRequests;

            public SEND_PACKET_REQUESTS()
            {
                // m_SendPacketRequests = new Queue<RCS_Protocol.RCS_Protocol.PACKET_TYPES>();
                m_SendPacketRequests = new ThreadSafeQueue<byte[]>(30);
            }

            public void AddRequest(byte[] packet)
            {
                lock (m_SendPacketRequests)
                {
                    m_SendPacketRequests.Enqueue(packet);
                }
            }

            public byte[] GetRequest()
            {

                return m_SendPacketRequests.Dequeue();
          
            }

            public int GetRequestCount()
            {

                return m_SendPacketRequests.Count;
             
            }

        }



        Thread m_SendThread;
        void StartSendThread()
        {
            m_SendThread = new Thread(SendThread);
            m_SendThread.Start();
        }

        void SendThread()
        {

            try
            {
                while (!m_CloseConnection)
                {

                    if (m_SendPacketRequests.GetRequestCount() > 0)
                    {
                        byte[] pkt = m_SendPacketRequests.GetRequest();


                        if (pkt != null)
                        {
                            //  Send the message to the connected TcpServer. 
                            m_Stream.Write(pkt, 0, pkt.Length);

                            //if (m_Stream.CanWrite)
                            //{
                            //    WaitingOnSendToComplete = true;
                            //    m_Stream.BeginWrite(pkt, 0, pkt.Length, new AsyncCallback(writeDoneCallBack), m_Stream);
                                
                            //    while (WaitingOnSendToComplete) Thread.Sleep(1);
                            //}
                          

                            //m_Log.Log("wrote request to socket " + pkt.Length.ToString() + "  bytes", ErrorLog.LOG_TYPE.INFORMATIONAL);
                        }
                    }
                    Thread.Sleep(1);
                }
            }
            catch (Exception ex)
            {
                CloseConnection();
                m_Log.Log("SendThread  ex:" + ex.Message, ErrorLog.LOG_TYPE.INFORMATIONAL);
            }


        }

   

       void  writeDoneCallBack(IAsyncResult ar)
        { m_Stream.EndWrite(ar);  }
    }
}