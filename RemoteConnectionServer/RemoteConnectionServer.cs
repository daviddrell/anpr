using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RCS_Protocol;
using ApplicationDataClass;
using ErrorLoggingLib;
using System.Net;
using FrameGeneratorLib;
using Utilities;
using LPREngineLib;

namespace RemoteConnectionServer
{
    public class RemoteConnectionServer
    {
        List<ConnectionServer> m_Server;
       // RCS_Protocol.RCS_Protocol m_Protocol;
        APPLICATION_DATA m_AppData;
        ErrorLog m_Log;
        FrameGenerator m_FrameGenerator;
        int m_ConsumerID;
        int m_NumberChannels;
        object m_FrameLock;
        ThreadSafeQueue<FRAME>[] m_CurrentImageQ;
        ThreadSafeQueue<FRAME>[] m_CurrentPlateNumberQ;
        ThreadSafeHashTable m_LocalHostPortsTable;
        LPREngine m_LPREngine;

        public RemoteConnectionServer( APPLICATION_DATA appData )
        {
            try
            {
                m_AppData = appData;
                m_AppData.AddOnClosing(OnClose, APPLICATION_DATA.CLOSE_ORDER.MIDDLE);
                m_Log = (ErrorLog)m_AppData.Logger;

                m_FrameLock = new object();

                m_FrameGenerator = (FrameGenerator)m_AppData.FrameGenerator;
                m_NumberChannels = m_FrameGenerator.GetNumberOfPhysicalChannels();
                m_ConsumerID = m_FrameGenerator.GetNewConsumerID();
                m_CurrentImageQ = new ThreadSafeQueue<FRAME>[m_NumberChannels];
                m_CurrentPlateNumberQ = new ThreadSafeQueue<FRAME>[m_NumberChannels];

                m_Log = (ErrorLog)m_AppData.Logger;

                m_LocalHostPortsTable = new ThreadSafeHashTable(5);

                m_Server = new List<ConnectionServer>();
            }
            catch (Exception ex) { m_Log.Trace(ex, ErrorLog.LOG_TYPE.FATAL); }
         
            
        }


        public void StartRegistration()
        {
            m_LPREngine = (LPREngine)m_AppData.LPREngine;

            for (int c = 0; c < m_NumberChannels; c++)
            {

                try
                {
                    m_CurrentImageQ[c] = new ThreadSafeQueue<FRAME>(3);
                    m_FrameGenerator.RegisterToConsumeChannel(m_ConsumerID, c, (FrameGenerator.NotificationOfNewFrameReady)NewImageCallBack);

                    m_CurrentPlateNumberQ[c] = new ThreadSafeQueue<FRAME>(3);
                    m_LPREngine.OnNewUnfilteredPlateEvent += new LPREngine.NewPlateEvent(m_LPREngine_OnNewPlateEvent); // get unfiltered plate readings for user display
                }

                catch (Exception ex) { m_Log.Trace(ex, ErrorLog.LOG_TYPE.FATAL); }

            }

        }

        public void StartThreads()
        {
            IPAddress[] ipAddresses = ConnectionServer.GetValidLocalAddress();
            if (ipAddresses == null)
            {
                m_Log.Log(" no local IP addresses found, exiting", ErrorLog.LOG_TYPE.FATAL);
                return;
            }

            bool loopBackAddressFound = false;
      

            foreach (IPAddress addr in ipAddresses)
            {
                try
                {
                    // eliminate duplicates
                    if (!m_LocalHostPortsTable.Contains(addr.ToString()))
                    {
                        if (addr.ToString().Contains("127.0")) loopBackAddressFound = true;

                        m_LocalHostPortsTable.Add(addr.ToString(), addr.ToString());
                        
                        m_Log.Log("IP Server listening on host addr: " + addr.ToString(), ErrorLog.LOG_TYPE.INFORMATIONAL);

                        ConnectionServer con = new ConnectionServer(addr, 13000, HandleReceivedMessage, m_AppData);
                        m_Server.Add(con);
                    }
                }
                catch (Exception ex) { m_Log.Trace(ex, ErrorLog.LOG_TYPE.FATAL); }

            }

            // ensure we are listening to the loop back 127.0.0.1
            if (!loopBackAddressFound)
            {
                try
                {
                    ConnectionServer con = new ConnectionServer(IPAddress.Loopback, 13000, HandleReceivedMessage, m_AppData);
                    m_Server.Add(con);
                }
                catch (Exception ex) { m_Log.Trace(ex, ErrorLog.LOG_TYPE.FATAL); }

            }
        }

        void m_LPREngine_OnNewPlateEvent(FRAME frame)
        {
            try
            {
                int c = frame.SourceChannel;

                m_CurrentPlateNumberQ[c].Dequeue();// this queue is just to act as a single unit buffer

                m_CurrentPlateNumberQ[c].Enqueue(frame);
            }
            catch (Exception ex) { m_Log.Trace(ex, ErrorLog.LOG_TYPE.FATAL); }
        }

        void NewImageCallBack(FRAME frame)
        {
            try
            {
                int c = frame.SourceChannel;

                if (m_CurrentImageQ[c].Count > 2) m_CurrentImageQ[c].Dequeue();// this queue is just to act as a single unit buffer

                m_CurrentImageQ[c].Enqueue(frame);


             }
            catch (Exception ex) { m_Log.Trace(ex, ErrorLog.LOG_TYPE.FATAL); }
        }

        byte[] GetCurrentJpeg(string channel, out string timeStamp, out string currentPlateReading, out int channelIndex)
        {
            timeStamp = " ";
            currentPlateReading = " ";
            channelIndex = 0;
            int c;
            try
            {

                c = m_FrameGenerator.GetChannelIndex(channel);
                if (c < 0)
                {
                    m_Log.Log("GetCurrentJpeg received bad channel index: " + c.ToString(), ErrorLog.LOG_TYPE.FATAL);
                    return null;
                }

                channelIndex = c;

                FRAME currentFrame = null;

                lock (m_FrameLock)
                {
                    if (m_CurrentImageQ[c].Count > 0)
                    {
                        currentFrame = m_CurrentImageQ[c].Dequeue();
                        timeStamp = currentFrame.TimeStamp.ToString(m_AppData.TimeFormatStringForFileNames);

                        // is there an LPR result available at this time?
                        FRAME lprResultFrame = m_CurrentPlateNumberQ[c].Dequeue();
                        if (lprResultFrame != null)
                        {
                            StringBuilder sb = new StringBuilder();
                            for (int i = 0; i < lprResultFrame.PlateNumberLatin.Length; i++ )
                            {
                                string s = lprResultFrame.PlateNumberLatin[i];
                                if ( i < lprResultFrame.PlateNumberLatin.Length-1)
                                    sb.Append(s + "^ ");  // use the ^ to seperate strings, the comma  is a parse field delimeter so do not use that
                                else
                                    sb.Append(s );  // do not put a delimeter after the last string
                            }
                            currentPlateReading = sb.ToString();
                        }

                        return (currentFrame.Jpeg);
                    }
                    else return null;
                }
            }
            catch (Exception ex) { m_Log.Trace(ex, ErrorLog.LOG_TYPE.FATAL); return (null); }

        }


        void OnClose()
        {
            try
            {
                foreach (ConnectionServer con in m_Server)
                {
                    con.Stop();
                }

            }
            catch (Exception ex) { m_Log.Trace(ex, ErrorLog.LOG_TYPE.FATAL); }

        }

        void HandleReceivedMessage(RCS_Protocol.RCS_Protocol.PACKET_TYPES type, byte[] data, ConnectionServer.ClientConnection connection, object packetHeader)
        {
            try
            {
                switch (type)
                {
                    case RCS_Protocol.RCS_Protocol.PACKET_TYPES.REQUEST_STATS:
                        {
                            // CreatePacket(PACKET_TYPES type, string data)
                            connection.SendHealthStats(m_AppData.HealthStatistics);

                        }
                        break;
                    case RCS_Protocol.RCS_Protocol.PACKET_TYPES.REQUEST_CHANNEL_LIST:
                        {
                            FrameGenerator fg = (FrameGenerator)m_AppData.FrameGenerator;
                            string[] list = fg.GetChannelList();
                            connection.SendChannelList(list);
                        }
                        break;

                    case RCS_Protocol.RCS_Protocol.PACKET_TYPES.REQUEST_LIVE_VIEW:
                        {
                            RCS_Protocol.RCS_Protocol.LIVE_VIEW_HEADER jpegHeader = (RCS_Protocol.RCS_Protocol.LIVE_VIEW_HEADER)packetHeader;


                            // parse out the requested channel ID from the info string

                            // int channel = Convert.ToInt32(jpegHeader.cameraName);
                            string timeStamp = null;
                            int channelIndex = 0;
                            string lastPlateReading = null;
                            byte[] jpeg = GetCurrentJpeg(jpegHeader.cameraName, out timeStamp, out lastPlateReading, out channelIndex);
                            if (jpeg == null)
                            {
                                jpeg = new byte[10]; // will be treated as null image on receiving end, but keep the state machine going
                            }
                            connection.SendJpeg(jpeg, jpegHeader.cameraName, timeStamp, lastPlateReading);
                            jpeg = null;
                        }
                        break;

                    case RCS_Protocol.RCS_Protocol.PACKET_TYPES.REQUEST_HOST_NAME:
                        {
                            connection.SendHostName(m_AppData.ThisComputerName);
                        }
                        break;
                }
            }
            catch (Exception ex) { m_Log.Trace(ex, ErrorLog.LOG_TYPE.FATAL); }

        }

       
    }
}
