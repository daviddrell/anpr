using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using RCS_Protocol;
using FileLoggingLib;
using System.Threading;
using System.Runtime.InteropServices;
using ApplicationDataClass;
using UserSettingsLib;
using EncryptionLib;
using ErrorLoggingLib;

namespace RemoteConnectionServer
{
    class ConnectionServer
    {
           
        public delegate void MessageReceivedCallback( RCS_Protocol.RCS_Protocol.PACKET_TYPES messageType, byte[] data, ClientConnection connection, object packetHdr);
        public int m_ip_port;
        
        const int MAX_CONNECTIONS = 5;
        MessageReceivedCallback m_MessageReceviedCallback;
        private TcpListener listner;

        bool m_Stop = false;

        IPAddress m_IPAddr;

        APPLICATION_DATA m_AppData;

        List<ClientConnection> m_Clients;

        public delegate void ConnectionClosedCallBack ( ClientConnection connection);
        public delegate void ConnectionEstablishedCallBack ( ClientConnection connection);

        Thread m_StartClientConnectThread;
        ErrorLog m_Log;
      
        public ConnectionServer( IPAddress addr, int port, MessageReceivedCallback callback, APPLICATION_DATA appData)
        {
            
        
            m_MessageReceviedCallback = callback;
            m_ip_port = port;
            m_IPAddr = addr;
            m_AppData = appData;
            m_Log = (ErrorLog)m_AppData.Logger;

            string ViewerPassword;
            string AdminPassword;

            try
            {
                ViewerPassword = UserSettings.Get(UserSettingTags.PWLPRServiceViewer);
                if (ViewerPassword == null) m_Log.Log("ViewerPW is null", ErrorLog.LOG_TYPE.INFORMATIONAL);
                m_Log.Log("viewer pw = " + ViewerPassword, ErrorLog.LOG_TYPE.INFORMATIONAL);

                m_Log.Log("user path = " + UserSettings.GetAppPath(), ErrorLog.LOG_TYPE.INFORMATIONAL);

                if (ViewerPassword == null) 
                    ViewerPassword = " ";
                else                
                    ViewerPassword = Encryption.DecryptText(ViewerPassword);

                AdminPassword = UserSettings.Get(UserSettingTags.PWLPRServiceAdmin);
                if (AdminPassword == null)
                    AdminPassword = " ";
                else
                    AdminPassword = Encryption.DecryptText(AdminPassword);

                m_AppData.ServiceAdminPW = AdminPassword;
                m_AppData.ServiceViewPW = ViewerPassword;

                m_Clients = new List<ClientConnection>(); ;

                listner = new TcpListener(addr, 13000);
                listner.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);// this line is key, or else you get an exception because the OS does not release the socket if you re-start 
          
                listner.Start();
               

                StartClientConnectWaitThread();
            }
            catch (Exception ex)
            {
                m_Log.Trace(ex, ErrorLog.LOG_TYPE.FATAL);
                Stop();  
            }
        }


        public void Stop ()
        {
            try
            {
                m_Stop = true;


                foreach (ClientConnection client in m_Clients)
                {
                    client.Close();
                }

                listner.Stop();
            }
            catch (Exception ex)
            {
                m_Log.Trace(ex, ErrorLog.LOG_TYPE.FATAL);
            }
        }



        public static IPAddress[] GetValidLocalAddress()
        {
            try
            {
                //IPAddress.Parse("127.0.0.1")

                // get all valid IP addresses for this machine

                string strHostName = Dns.GetHostName();
                IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);
                IPAddress[] addresses = ipEntry.AddressList;
                if (addresses == null) return null;
                if (addresses.Length < 1) return null;

                List<IPAddress> validAddresses = new List<IPAddress>();

                foreach (IPAddress addr in addresses)
                {
                    if (addr.AddressFamily == AddressFamily.InterNetwork)
                    {
                        validAddresses.Add(addr);
                    }
                }

                return (validAddresses.ToArray());
            }
            catch (Exception ex)
            {
                ErrorLog.Trace(ex );
                return (default(IPAddress[]));
            }
        }

        // there are two threads, the first blocks on waiting for a client to connect, the second(s) services the client after connection
        // after a client connects (and a service thread is started), start another blocking thread waiting for the next client to connect

        void StartClientConnectWaitThread()
        {
            if (m_Stop) return;

            m_StartClientConnectThread = new Thread(BlockOnClientConnect);
            m_StartClientConnectThread.Start();
        }
       
        void BlockOnClientConnect( )
        {
            try
            {
                if (m_Clients.Count >= MAX_CONNECTIONS) return;

                // block on waiting for a client to connect, ClientConnection spins a new internal thread on connect to service the connecton
                ClientConnection client = new ClientConnection(listner.AcceptSocket(), m_IPAddr.ToString(), m_MessageReceviedCallback, OnConnectionClosed, m_AppData);

                client.StartCommunicating();

                // a client connected, add it to the list of active clients
                m_Clients.Add(client);


                // start listening for another client to connected
                StartClientConnectWaitThread();
            }
            catch (Exception ex)
            {
                m_Log.Trace(ex, ErrorLog.LOG_TYPE.FATAL);
            }
        
        }

      
    
        
        public void OnConnectionClosed(ClientConnection connection)
        {
            // this connection thread has ended

            try
            {
                lock (connection)
                {
                    if (m_Clients.Contains(connection)) m_Clients.Remove(connection);
                }

                if (m_Stop) return;

                // we may have been at MAX connections, and one just freed up, so see if we need to start a new client thread
                StartClientConnectWaitThread();
            }
            catch (Exception ex)
            {
                m_Log.Trace(ex, ErrorLog.LOG_TYPE.FATAL);
            }
        }

    


        public class ClientConnection
        {
            internal Socket connection;
            private NetworkStream socketStream;

            private BinaryWriter writer;
            private BinaryReader reader;
        
            private bool stop = false;
            private RCS_Protocol.RCS_Protocol protocol;
            private MessageReceivedCallback MessageReceived;
            ConnectionClosedCallBack ConnectionClosed;

            string ipAddress;

            APPLICATION_DATA m_AppData;

            private Thread thread;
            ErrorLog m_Log;

            public ClientConnection(Socket socket, string ipAddr, MessageReceivedCallback messageReceived,ConnectionClosedCallBack closedCB, APPLICATION_DATA appData )
            {
                ipAddress = ipAddr;
                m_AppData = appData;
                m_Log = (ErrorLog)m_AppData.Logger;

                try
                {
                    protocol = new RCS_Protocol.RCS_Protocol(m_AppData, ipAddr);

                    MessageReceived = messageReceived;
                    ConnectionClosed = closedCB;

                    connection = socket;

                    connection.Blocking = true;
          

                }
                catch (Exception ex)
                {

                    m_Log.Trace(ex, ErrorLog.LOG_TYPE.FATAL);
                    Close();
                    ConnectionClosed(this);
                }
            }

            public void StartCommunicating ()
            {

                try
                {
                    socketStream = new NetworkStream(connection, true);

                    writer = new BinaryWriter(socketStream);
                    reader = new BinaryReader(socketStream);

                    // a client has connected, spin a thread to handle this connected client

                    thread = new Thread(ProcessClientRequests);
                    thread.Start();
                }
                catch (Exception ex)
                {
                    m_Log.Trace(ex, ErrorLog.LOG_TYPE.FATAL);
                }
            }

            public void Close()
            {
                try
                {
                    stop = true;
                    connection.Shutdown(SocketShutdown.Both);
                   // writer.Close();
                  //  reader.Close();
                  //  socketStream.Close();
                    connection.Disconnect(true);
                }
                catch (Exception ex)
                {
                    m_Log.Trace(ex, ErrorLog.LOG_TYPE.FATAL);
                }
            }


         
            public void ProcessClientRequests()
            {

                byte[] payload;
                RCS_Protocol.RCS_Protocol.PACKET_HEADER packetHeader;
                RCS_Protocol.RCS_Protocol.PACKET_TYPES type;
                RCS_Protocol.RCS_Protocol.LIVE_VIEW_HEADER JPEG_Header = null;

                int bytes = 0;
             
                while (!stop)
                {
                    try
                    {
                        Byte[] header = new byte[protocol.HeaderLessSOPLength];

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

                        if (!headerFound) continue;

                        // block on  reading header length bytes from the socket

                        int cnt = reader.Read(header, 0, header.Length); // read out the header
                        if (cnt == 0)
                        {
                            // the connection was closed by the other end
                            writer.Close();
                            reader.Close();
                            socketStream.Close();
                            connection.Close();
                            ConnectionClosed(this);
                            break;
                        }

                        int payLoadLength = 0;

                        RCS_Protocol.RCS_Protocol.ERROR_CODES error = RCS_Protocol.RCS_Protocol.ERROR_CODES.NO_ERROR;

                        type = protocol.GetPacketType(header, ref payLoadLength, ref error, out packetHeader);

                        payload = null;

                        if (type != RCS_Protocol.RCS_Protocol.PACKET_TYPES.INVALID_PACKET)
                        {
                            if (payLoadLength > 0)
                            {
                                payload = new byte[payLoadLength];

                                // read out the payload
                                bytes = 0;
                                while (bytes < payLoadLength)
                                {
                                    b = reader.ReadByte();

                                    payload[bytes++] = b;
                                }
                            }
                        }
                        else
                        {
                            // invalid packet, flush the buffer
                           
                            payload = null;
                        }
                       

                        
                        switch (type)
                        {
                            case RCS_Protocol.RCS_Protocol.PACKET_TYPES.REQUEST_STATS:
                                {
                                    if (ValidViewerPW(packetHeader.PasswordHash, packetHeader.SequenceNumber) || ValidAdminPW(packetHeader.PasswordHash, packetHeader.SequenceNumber))
                                    {
                                        MessageReceived(type, payload, this, packetHeader);
                                    }
                                    else
                                        ReplyInvalidViewerPW();
                                }
                                break;
                            case RCS_Protocol.RCS_Protocol.PACKET_TYPES.REQUEST_CHANNEL_LIST:
                                {
                                    if (ValidViewerPW(packetHeader.PasswordHash, packetHeader.SequenceNumber) || ValidAdminPW(packetHeader.PasswordHash, packetHeader.SequenceNumber))
                                    {
                                        MessageReceived(type, payload, this, packetHeader);
                                    }
                                    else
                                        ReplyInvalidViewerPW();
                                }
                                break;

                            case RCS_Protocol.RCS_Protocol.PACKET_TYPES.REQUEST_CHECK_ADMIN_PASSWORD:
                                Console.Write("received REQUEST_CHECK_ADMIN_PASSWORD");
                                if (!ValidAdminPW(packetHeader.PasswordHash, packetHeader.SequenceNumber))
                                    ReplyInvalidAdminPW();
                                else
                                    ReplyValidAdminPW();
                                break;

                            case RCS_Protocol.RCS_Protocol.PACKET_TYPES.REQUEST_CHECK_VIEWER_PASSWORD:
                                Console.Write("received REQUEST_CHECK_VIEWER_PASSWORD");
                                if (!ValidViewerPW(packetHeader.PasswordHash, packetHeader.SequenceNumber))
                                    ReplyInvalidViewerPW();
                                else
                                    ReplyValidViewerPW();
                                break;

                            case RCS_Protocol.RCS_Protocol.PACKET_TYPES.REQUEST_LIVE_VIEW:
                                if (ValidViewerPW(packetHeader.PasswordHash, packetHeader.SequenceNumber) || ValidAdminPW(packetHeader.PasswordHash, packetHeader.SequenceNumber))
                                {
                                    protocol.BreakOutJpegHeader(payload, out JPEG_Header);
                                    MessageReceived(type, null, this, (object)JPEG_Header);
                                }
                                else
                                    ReplyInvalidViewerPW();
                                break;

                            case RCS_Protocol.RCS_Protocol.PACKET_TYPES.REQUEST_HOST_NAME:

                                // the REQUEST_HOST_NAME is special, its used by the client to verify the initial connection.
                                //    the password for this request is simply a hash of the server's IP address.
                                //    this is checked at the protocol level.

                                if (error == RCS_Protocol.RCS_Protocol.ERROR_CODES.INVALID_PASSWORD)
                                {
                                    ReplyInvalidHostNamePassword(RCS_Protocol.RCS_Protocol.PASSWORD_TYPES.REQUEST_HOST_NAME.ToString());
                                    break;
                                }
                                MessageReceived(type, null, this, packetHeader);

                                break;
                        }

                    }
                    catch (Exception ex)
                    {
                        Close();
                        ConnectionClosed(this);
                        if (ex.Message.Contains("Unable to"))
                            m_Log.Log("remote closed connection", ErrorLog.LOG_TYPE.INFORMATIONAL);// normal far end socket close
                        else
                            m_Log.Trace(ex, ErrorLog.LOG_TYPE.INFORMATIONAL);
                        break;
                    }


                }
            }

            bool ValidViewerPW(string ReceivedPasswordHash, int sequenceNumber)
            {
                return (true);

                // navy wants no passwords used

                //try
                //{
                //    byte checkNum = (byte)(sequenceNumber & 0xff);

                //    string receivedPW = Encryption.DecryptText(ReceivedPasswordHash);

                //    string[] ss = receivedPW.Split(',');
                //    if (ss.Length != 2) return (false);

                //    int receivedSeqNum = Convert.ToByte(ss[1]);

                //    if (ss[0].Equals(m_AppData.ServiceViewPW) && receivedSeqNum == checkNum) return (true);

                //    return (false);
                //}
                //catch
                //{
                    
                //    return (false);
                //}
            }

            bool ValidAdminPW(string ReceivedPasswordHash, int sequenceNumber)
            {
                return (true);
                // navy wants no passwords used

                //try
                //{
                //    byte checkNum = (byte)(sequenceNumber & 0xff);

                //    string receivedPW = Encryption.DecryptText(ReceivedPasswordHash);

                //    string[] ss = receivedPW.Split(',');
                //    if (ss.Length != 2) return (false);

                //    int receivedSeqNum = Convert.ToByte(ss[1]);

                //    if (ss[0].Equals(m_AppData.ServiceAdminPW) && receivedSeqNum == checkNum)
                //    {
                //        return (true);
                //    }
                //    m_Log.Log("admin pw failed, was " + ss[0] + "/"+m_AppData.ServiceAdminPW + ", seqnum =" + receivedSeqNum.ToString() + "/" + checkNum.ToString(), ErrorLog.LOG_TYPE.INFORMATIONAL);
                //    return (false);
                //}
                //catch 
                //{
                //    return (false);
                //}
            }

            void ReplyInvalidViewerPW()
            {
                try
                {
                    byte[] packet = protocol.CreatePacket(RCS_Protocol.RCS_Protocol.PACKET_TYPES.REPLY_INVALID_VIEWER_PASSWORD_ERROR, "invalid viewer password");
                    writer.Write(packet, 0, packet.Length);

         
                    mPacketCount++;
                }
                catch (Exception ex)
                {
                    writer.Close();
                    reader.Close();
                    socketStream.Close();
                    connection.Close();
                    ConnectionClosed(this);
                    m_Log.Trace(ex, ErrorLog.LOG_TYPE.INFORMATIONAL);
                    
                }
            }

            void ReplyInvalidAdminPW()
            {
                try
                {
                    byte[] packet = protocol.CreatePacket(RCS_Protocol.RCS_Protocol.PACKET_TYPES.REPLY_INVALID_ADMIN_PASSWORD_ERROR, "invalid admin password");
                    writer.Write(packet, 0, packet.Length);

                    mPacketCount++;
                }
                catch (Exception ex)
                {
                    writer.Close();
                    reader.Close();
                    socketStream.Close();
                    connection.Close();
                    ConnectionClosed(this);
                    m_Log.Trace(ex, ErrorLog.LOG_TYPE.INFORMATIONAL);
                }
            }

            void ReplyValidAdminPW()
            {
                try
                {
                    Console.Write("sending REPLY_VALID_ADMIN_PASSWORD");

                    byte[] packet = protocol.CreatePacket(RCS_Protocol.RCS_Protocol.PACKET_TYPES.REPLY_VALID_ADMIN_PASSWORD);
                    writer.Write(packet, 0, packet.Length);

                    mPacketCount++;
                }
                catch (Exception ex)
                {
                    writer.Close();
                    reader.Close();
                    socketStream.Close();
                    connection.Close();
                    ConnectionClosed(this);
                    m_Log.Trace(ex, ErrorLog.LOG_TYPE.INFORMATIONAL);
                }
            }

            void ReplyValidViewerPW()
            {
                try
                {
                    Console.Write("sending REPLY_VALID_VIEWER_PASSWORD");

                    byte[] packet = protocol.CreatePacket(RCS_Protocol.RCS_Protocol.PACKET_TYPES.REPLY_VALID_VIEWER_PASSWORD );
                    writer.Write(packet, 0, packet.Length);

                    mPacketCount++;
                }
                catch (Exception ex)
                {
                    writer.Close();
                    reader.Close();
                    socketStream.Close();
                    connection.Close();
                    ConnectionClosed(this);
                    m_Log.Trace(ex, ErrorLog.LOG_TYPE.INFORMATIONAL);
                }
            }

            void ReplyInvalidHostNamePassword(string passwordType)
            {
                try
                {
                    byte[] packet = protocol.CreatePacket(RCS_Protocol.RCS_Protocol.PACKET_TYPES.REPLY_INVALID_HOSTNAME_PASSWORD_ERROR, RCS_Protocol.RCS_Protocol.PASSWORD_TYPES.REQUEST_HOST_NAME.ToString());
                    writer.Write(packet, 0, packet.Length);

                    mPacketCount++;
                }
                catch (Exception ex)
                {
                    writer.Close();
                    reader.Close();
                    socketStream.Close();
                    connection.Close();
                    ConnectionClosed(this);
                    m_Log.Trace(ex, ErrorLog.LOG_TYPE.INFORMATIONAL);
                }
            }



            public void SendHealthStats( APPLICATION_DATA.HEALTH_STATISTICS stats)
            {
                try
                {
                    string info = protocol.BuildStatsString(stats);
                    byte[] packet = protocol.CreatePacket( RCS_Protocol.RCS_Protocol.PACKET_TYPES.SEND_STATS, info);

                    writer.Write(packet, 0, packet.Length);
                
                    mPacketCount++;
                }
                catch (Exception ex)
                {
                    writer.Close();
                    reader.Close();
                    socketStream.Close();
                    connection.Close();
                    ConnectionClosed(this);
                    m_Log.Trace(ex, ErrorLog.LOG_TYPE.INFORMATIONAL);
                }
            }

            public void SendChannelList(string[] list)
            {
                try
                {
                    byte[] packet = protocol.CreatePacket(" ", RCS_Protocol.RCS_Protocol.PACKET_TYPES.REPLY_CHANNEL_LIST, (string[])list);

                    writer.Write(packet, 0, packet.Length);

                    mPacketCount++;
                }
                catch (Exception ex)
                {
                    writer.Close();
                    reader.Close();
                    socketStream.Close();
                    connection.Close();
                    ConnectionClosed(this);
                    m_Log.Trace(ex, ErrorLog.LOG_TYPE.INFORMATIONAL);
                }
            }

            int mPacketCount = 0;
            public void SendJpeg(byte[] jpeg, string channel, string timeStamp, string plateNumber)
            {
                try
                {
                    RCS_Protocol.RCS_Protocol.LIVE_VIEW_HEADER header =new RCS_Protocol.RCS_Protocol.LIVE_VIEW_HEADER();
                    header.cameraName = channel;
                    header.timeStamp = timeStamp;
                    header.plateNumber = plateNumber;
                    string jpegInfo = protocol.BuildInfoString(header);
                    byte[] packet = protocol.CreatePacket(RCS_Protocol.RCS_Protocol.PACKET_TYPES.SEND_LIVE_VIEW, jpeg, jpegInfo);

               //     Console.WriteLine("writing " + packet.Length.ToString() + " bytes to socket, packet number: " + mPacketCount.ToString());
                 
                    writer.Write(packet, 0, packet.Length);

                    mPacketCount++;
                }
                catch (Exception ex)
                {
                    writer.Close();
                    reader.Close();
                    socketStream.Close();
                    connection.Close();
                    ConnectionClosed(this);
                    m_Log.Trace(ex, ErrorLog.LOG_TYPE.INFORMATIONAL);
                }
            }

            public void SendHostName(string hostName)
            {
                try
                {
                    byte[] packet = protocol.CreatePacket(RCS_Protocol.RCS_Protocol.PACKET_TYPES.SEND_HOST_NAME, hostName);

                    //     Console.WriteLine("writing " + packet.Length.ToString() + " bytes to socket, packet number: " + mPacketCount.ToString());

                    writer.Write(packet, 0, packet.Length);

                    mPacketCount++;
                }
                catch (Exception ex)
                {
                    writer.Close();
                    reader.Close();
                    socketStream.Close();
                    connection.Close();
                    ConnectionClosed(this);
                    m_Log.Trace(ex, ErrorLog.LOG_TYPE.INFORMATIONAL);
                }
            }

        }
   


    }


}
