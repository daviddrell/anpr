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

        //  ///////////////////////////////////////////////////
        //
        //   ReceiveThread
        //    

        Thread m_ReceiveThread;
        void StartReceiveThread()
        {
            m_ReceiveThread = new Thread(ReceiveThread);
            m_ReceiveThread.Start();
        }

        void ReceiveThread()
        {
            int bytes = 0;
            byte[] payload;
            RCS_Protocol.RCS_Protocol.PACKET_HEADER packetHeader;
            RCS_Protocol.RCS_Protocol.PACKET_TYPES type;
            RCS_Protocol.RCS_Protocol.LIVE_VIEW_HEADER JPEG_Header=null;

        //    m_Stream.ReadTimeout = 10000;

            BinaryReader reader = default(BinaryReader);

            try
            {
                reader = new BinaryReader(m_Stream);
            }
            catch (Exception ex) { m_Log.Trace(ex, ErrorLog.LOG_TYPE.FATAL); }

            while (!m_CloseConnection)
            {
                try
                {
                    Byte[] header = new byte[m_RCSProtocol.HeaderLessSOPLength];

                    bool headerFound = false;

                    // look for the start-of-packet sequence

                    byte b = reader.ReadByte();// b 0
                    if (b == 0x55)
                    {
                        b = reader.ReadByte(); // b 1
                        if (b == 0xaa)
                        {
                            b = reader.ReadByte(); // b 2
                            if (b == 0x55)
                            {
                                b = reader.ReadByte(); // b 3
                                if (b == 0xaa)
                                {
                                    headerFound = true;
                                }
                            }
                        }
                    }

                    if (headerFound)
                    {
                        bytes = reader.Read(header, 0, header.Length);
                        if (bytes != header.Length)
                        {
                            int jj = bytes;//breakpoint
                        }

                        int payLoadLength = 0;

                        RCS_Protocol.RCS_Protocol.ERROR_CODES error = RCS_Protocol.RCS_Protocol.ERROR_CODES.NO_ERROR;

                        type = m_RCSProtocol.GetPacketType(header, ref payLoadLength, ref error, out packetHeader);
                        
                        payload = null;

                        if (type != RCS_Protocol.RCS_Protocol.PACKET_TYPES.INVALID_PACKET  )
                        {
                            if ( payLoadLength > 0 )
                            {
                                try
                                {
                                    payload = new byte[payLoadLength];
                                    bytes = 0;
                                    while (bytes < payLoadLength)
                                    {
                                        b = reader.ReadByte();

                                        payload[bytes++] = b;
                                    }

                                    if (bytes != payload.Length)
                                    {
                                        int jj = bytes;//breakpoint
                                    }
                                }
                                catch (Exception ex)
                                {
                                    m_Log.Log("first payload receive ex " + ex.Message, ErrorLog.LOG_TYPE.INFORMATIONAL);
                                    CloseConnection();
                                }
                            }
                        }
                        else
                        {
                            // invalid packet, flush the buffer
                         //   while (m_Stream.DataAvailable) m_Stream.ReadByte();
                          //  payload = null;
                        }
                    }

                    else
                    {
                        Thread.Sleep(1);
                        continue;
                    }

            
                    switch (type)
                    {
                        case RCS_Protocol.RCS_Protocol.PACKET_TYPES.SEND_STATS:
                            HandleReceiveStats(packetHeader, payload);
                            break;

                        case RCS_Protocol.RCS_Protocol.PACKET_TYPES.REPLY_CHANNEL_LIST:
                            HandleReceiveChannelList(packetHeader, payload);
                            break;

                        case RCS_Protocol.RCS_Protocol.PACKET_TYPES.SEND_LIVE_VIEW:

                            JPEG_Header = null;

                            try
                            {
                                m_RCSProtocol.BreakOutJpegHeader(payload, out JPEG_Header);

                                if (JPEG_Header.JPEG_LENGTH <= 10)
                                {
                                    // a null frame was sent
                                    payload = new byte[JPEG_Header.JPEG_LENGTH];

                                    payload = new byte[JPEG_Header.JPEG_LENGTH];
                                    bytes = 0;
                                    while (bytes < JPEG_Header.JPEG_LENGTH)
                                    {
                                        b = reader.ReadByte();

                                        payload[bytes++] = b;
                                    }

                                    break; // drop it
                                }

                                payload = new byte[JPEG_Header.JPEG_LENGTH];

                                // read in the jpeg header
                                bytes = 0;
                                while (bytes < JPEG_Header.JPEG_LENGTH)
                                {
                                    b = reader.ReadByte();

                                    payload[bytes++] = b;
                                }


                             //   int channel = Convert.ToInt32(JPEG_Header.cameraName);
                                string channel = JPEG_Header.cameraName;
                                HandleReceiveJPeg(packetHeader, (byte[])payload.Clone(), channel, JPEG_Header.timeStamp, JPEG_Header.plateNumber);
                            }
                            catch (Exception ex)
                            {
                                m_Log.Log("jpeg receive ex " + ex.Message, ErrorLog.LOG_TYPE.INFORMATIONAL);
                                CloseConnection();
                            }

                            break;

                        case RCS_Protocol.RCS_Protocol.PACKET_TYPES.SEND_HOST_NAME:

                            HandleReceiveHostName(packetHeader, payload);
                            break;

                        case RCS_Protocol.RCS_Protocol.PACKET_TYPES.REPLY_INVALID_HOSTNAME_PASSWORD_ERROR:

                            HandleInvalidHostnamePassword(packetHeader, payload);
                            break;

                        case RCS_Protocol.RCS_Protocol.PACKET_TYPES.REPLY_VALID_ADMIN_PASSWORD:

                            HandleValidAdminPassword(packetHeader, payload);
                            break;

                        case RCS_Protocol.RCS_Protocol.PACKET_TYPES.REPLY_VALID_VIEWER_PASSWORD:

                            HandleValidViewerPassword(packetHeader, payload);
                            break;

                        case RCS_Protocol.RCS_Protocol.PACKET_TYPES.REPLY_INVALID_VIEWER_PASSWORD_ERROR:

                            HandleInvalidViewerPassword(packetHeader, payload);
                            break;

                        case RCS_Protocol.RCS_Protocol.PACKET_TYPES.REPLY_INVALID_ADMIN_PASSWORD_ERROR:

                            HandleInvalidAdminPassword(packetHeader, payload);
                            break;

                        case RCS_Protocol.RCS_Protocol.PACKET_TYPES.REQUEST_CHECK_ADMIN_PASSWORD:

                            HandleInvalidAdminPassword(packetHeader, payload);
                            break;

                        
                    }
                }
                catch (Exception ex)
                {
                    m_Log.Trace(ex, ErrorLog.LOG_TYPE.FATAL); 

                    CloseConnection();
                }

                Thread.Sleep(1);
            }
            CloseConnection();
        }


    }
}