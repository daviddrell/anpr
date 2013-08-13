using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using EncryptionLib;
using ApplicationDataClass;
using ErrorLoggingLib;


namespace RCS_Protocol
{
    public class RCS_Protocol
    {
        // this constructor used by the client app
        public RCS_Protocol(APPLICATION_DATA appData, string thisIP)
        {
            m_AppData = appData;
            m_Log = (ErrorLog) m_AppData.Logger;

            m_ThisHostsIP = thisIP;

            if (m_ThisHostsIP == null) m_ThisHostsIP = " ";

            HeaderLength = Marshal.SizeOf(typeof(PACKET_HEADER_STRUCT));
            HeaderLessSOPLength = Marshal.SizeOf(typeof(PACKET_HEADER_LESS_SOP_STRUCT));

        }

      

        ErrorLog m_Log;
        APPLICATION_DATA m_AppData;

        string m_ThisHostsIP;

        public int HeaderLength;
        public int HeaderLessSOPLength;

      
        public enum PACKET_TYPES {  
        REQUEST_LIVE_VIEW = 1,
        SEND_LIVE_VIEW =  2,
        REQUEST_HOST_NAME =  3,
        SEND_HOST_NAME =  4,
        REPLY_INVALID_ADMIN_PASSWORD_ERROR =  5,
        REQUEST_CHECK_VIEWER_PASSWORD =  6,
        REQUEST_CHECK_ADMIN_PASSWORD =  7,
        REPLY_VALID_VIEWER_PASSWORD =  8,
        REPLY_VALID_ADMIN_PASSWORD =  9,
        REPLY_INVALID_VIEWER_PASSWORD_ERROR =  10,
        REPLY_INVALID_HOSTNAME_PASSWORD_ERROR = 11,
        REQUEST_CHANNEL_LIST =  12,
        REPLY_CHANNEL_LIST = 13,
        REQUEST_STATS = 14,
        SEND_STATS = 15,
        INVALID_PACKET = 0
        }

        const int STRING_LEN = 256;

        public const int PASS_WORD_HASH_LENGTH = 64;

        public enum ERROR_CODES { NO_ERROR, INVALID_PASSWORD, BAD_PACKET }
        public enum PASSWORD_TYPES { VIEWER, ADMINISTRATOR, REQUEST_HOST_NAME }

        [StructLayout(LayoutKind.Explicit, Size = 80, Pack = 1)]
        struct PACKET_HEADER_STRUCT
        {
            [MarshalAs(UnmanagedType.U4)]
            [FieldOffset(0)]
            public uint START_OF_PACKET;

            [MarshalAs(UnmanagedType.U4)]
            [FieldOffset(4)]
            public uint PACKET_TYPE;

            [MarshalAs(UnmanagedType.U4)]
            [FieldOffset(8)]
            public uint PACKET_LEN;// len of data not including this header
            
            [MarshalAs(UnmanagedType.U4)]
            [FieldOffset(12)]
            public uint SEQUENCE_NUMBER;// packet sequence number

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            [FieldOffset(16)]
            public string PasswordHash;
        }

        [StructLayout(LayoutKind.Explicit, Size = 76, Pack = 1)]
        struct PACKET_HEADER_LESS_SOP_STRUCT
        {
            [MarshalAs(UnmanagedType.U4)]
            [FieldOffset(0)]
            public uint PACKET_TYPE;

            [MarshalAs(UnmanagedType.U4)]
            [FieldOffset(4)]
            public uint PACKET_LEN;// len of data not including this header

            [MarshalAs(UnmanagedType.U4)]
            [FieldOffset(8)]
            public uint SEQUENCE_NUMBER;// packet sequence number

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
            [FieldOffset(12)]
            public string PasswordHash;
        }

        public class PACKET_HEADER
        {
            public PACKET_TYPES Type;
            public int Length;
            public int SequenceNumber;
            public string PasswordHash;
            public int channel;
        }

        //   LIVE VIEW


        public class LIVE_VIEW_HEADER
        {
            public string InfoString;
            public int JPEG_LENGTH;
            public string cameraName;
            public string timeStamp;
            public string plateNumber;
        }

        [StructLayout(LayoutKind.Explicit, Size = 1108, Pack = 1)]
        struct LIVE_VIEW_COMBINED_PACKET_STRUCT
        {
            [MarshalAs(UnmanagedType.Struct, SizeConst = 80)]
            [FieldOffset(0)]
            public PACKET_HEADER_STRUCT header;

            [MarshalAs(UnmanagedType.U4)]
            [FieldOffset(80)]
            public uint JPEG_LENGTH;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
            [FieldOffset(84)]
            public string InfoString;
        }

        [StructLayout(LayoutKind.Explicit, Size = 1104, Pack = 1)]
        struct LIVE_VIEW_COMBINED_PACKET_LESS_SOP_STRUCT
        {
            [MarshalAs(UnmanagedType.Struct, SizeConst = 76)]
            [FieldOffset(0)]
            public PACKET_HEADER_LESS_SOP_STRUCT header;

            [MarshalAs(UnmanagedType.U4)]
            [FieldOffset(76)]
            public uint JPEG_LENGTH;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
            [FieldOffset(80)]
            public string InfoString;
        }

        [StructLayout(LayoutKind.Explicit, Size = 1028, Pack = 1)]
        struct LIVE_VIEW_ONLY_PACKET_STRUCT
        {
            [MarshalAs(UnmanagedType.U4)]
            [FieldOffset(0)]
            public uint JPEG_LENGTH;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
            [FieldOffset(4)]
            public string InfoString;
        }

        //     HOST NAME

        [StructLayout(LayoutKind.Explicit, Size = 80, Pack = 1)]
        struct REQUEST_HOST_NAME
        {
            [MarshalAs(UnmanagedType.Struct, SizeConst = 80)]
            [FieldOffset(0)]
            public PACKET_HEADER_STRUCT header;
        }

        [StructLayout(LayoutKind.Explicit, Size = 336, Pack = 1)]
        struct SEND_HOST_NAME_PACKET
        {
            [MarshalAs(UnmanagedType.Struct, SizeConst = 80)]
            [FieldOffset(0)]
            public PACKET_HEADER_STRUCT header;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            [FieldOffset(76)]
            public string HostName;
        }

        T ByteArrayToStructure<T>(byte[] bytes) where T : struct
        {
            try
            {
                GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
                T stuff = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(),
                    typeof(T));
                handle.Free();
                return stuff;
            }
            catch (Exception ex)
            {
                m_Log.Log("ByteArrayToStructure ex: " + ex.Message, ErrorLog.LOG_TYPE.INFORMATIONAL);
                return (default(T));
            }
        }

        string EncyptGetHostPassword(string remoteHostIPAddress)
        {
            try
            {
                string coded = Encryption.EncryptText(remoteHostIPAddress);
                return (coded);
            }
            catch (Exception ex)
            {
                m_Log.Log("EncyptGetHostPassword ex: " + ex.Message, ErrorLog.LOG_TYPE.INFORMATIONAL);
                return (null);
            }
            
        }

        bool DoesGetHostPWMatch(string encoded)
        {
            try
            {
                if (m_ThisHostsIP == null) return false;

                string code = DecryptGetHostPassword(encoded);
                // "hostIP,seqNum"
                string[] ss = code.Split(',');
                if (ss.Length != 2)
                {
                    m_Log.Log("DoesGetHostPWMatch, ss.Length != 2, code = " + code, ErrorLog.LOG_TYPE.INFORMATIONAL);

                    return (false);
                }

                if (ss[0].Equals(m_ThisHostsIP))
                {
                    m_Log.Log("DoesGetHostPWMatch, good pw", ErrorLog.LOG_TYPE.INFORMATIONAL);

                    return (true);
                }
                else
                {
                    m_Log.Log("DoesGetHostPWMatch, pw does not match: received: " + ss[0]+", this host ip = "+m_ThisHostsIP, ErrorLog.LOG_TYPE.INFORMATIONAL);
                }


                return (false);
            }
            catch (Exception ex)
            {
                m_Log.Log("DoesGetHostPWMatch ex: " + ex.Message, ErrorLog.LOG_TYPE.INFORMATIONAL);
                return (false);
            }
        }

        string DecryptGetHostPassword(string coded)
        {
            try
            {
                string remoteHostIPAddress = Encryption.DecryptText(coded);
                return (remoteHostIPAddress);
            }
            catch (Exception ex)
            {
                m_Log.Log("DecryptGetHostPassword ex: " + ex.Message, ErrorLog.LOG_TYPE.INFORMATIONAL);
                return (null);
            }
        }

        public void BreakOutJpegHeader(byte[] packet,  out LIVE_VIEW_HEADER header)
        {
            header = new LIVE_VIEW_HEADER(); ;


            LIVE_VIEW_ONLY_PACKET_STRUCT pktHdr = new LIVE_VIEW_ONLY_PACKET_STRUCT();

            int size = Marshal.SizeOf(typeof(LIVE_VIEW_ONLY_PACKET_STRUCT));

            GCHandle handle = GCHandle.Alloc(packet, GCHandleType.Pinned);
            pktHdr = (LIVE_VIEW_ONLY_PACKET_STRUCT)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(LIVE_VIEW_ONLY_PACKET_STRUCT));
            handle.Free();

            //header.JPEG_LENGTH = (int)pktHdr.JPEG_LENGTH;
            //header.InfoString = pktHdr.InfoString;

            header = ParseInfoString(pktHdr.InfoString);
            header.JPEG_LENGTH = (int)pktHdr.JPEG_LENGTH;
        }




        public PACKET_TYPES GetPacketType(byte[] packet, ref int payLoadLength, ref ERROR_CODES error, out PACKET_HEADER managedHeader)
        {
            managedHeader = new PACKET_HEADER();

            try
            {

                if (packet.Length < HeaderLessSOPLength) return PACKET_TYPES.INVALID_PACKET;

                PACKET_HEADER_LESS_SOP_STRUCT pktHdr = new PACKET_HEADER_LESS_SOP_STRUCT();

                int size = Marshal.SizeOf(typeof(PACKET_HEADER_LESS_SOP_STRUCT));
                byte[] headerPart = new byte[size];
                headerPart = (byte[])Slice(packet, 0, size);
                GCHandle handle = GCHandle.Alloc(headerPart, GCHandleType.Pinned);
                pktHdr = (PACKET_HEADER_LESS_SOP_STRUCT)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(PACKET_HEADER_LESS_SOP_STRUCT));
                handle.Free();

                payLoadLength = (int)pktHdr.PACKET_LEN;

                PACKET_TYPES pktTpye = (PACKET_TYPES)pktHdr.PACKET_TYPE;

                error = ERROR_CODES.NO_ERROR;

                if (!CheckAllPacketTypes(pktTpye))
                {
                    error = ERROR_CODES.BAD_PACKET;
                    return PACKET_TYPES.INVALID_PACKET;
                }

                switch ((PACKET_TYPES)pktTpye)
                {
                    case PACKET_TYPES.REQUEST_HOST_NAME:
                        m_Log.Log("GetPacketType, received REQUEST_HOST_NAME ", ErrorLog.LOG_TYPE.INFORMATIONAL);

                        if (!DoesGetHostPWMatch(pktHdr.PasswordHash))
                        {
                            m_Log.Log("GetPacketType, ERROR_CODES.INVALID_PASSWORD", ErrorLog.LOG_TYPE.INFORMATIONAL);

                            error = ERROR_CODES.INVALID_PASSWORD;
                        }

                        break;

                  

                    default:

                        managedHeader.Type = (PACKET_TYPES)pktHdr.PACKET_TYPE;
                        managedHeader.PasswordHash = pktHdr.PasswordHash;
                        managedHeader.SequenceNumber = (int)pktHdr.SEQUENCE_NUMBER;
                        managedHeader.Length = (int)pktHdr.PACKET_LEN;
                        break;
                }

                
              

                return ((PACKET_TYPES)pktHdr.PACKET_TYPE);
            }
            catch (Exception ex)
            {
                
                m_Log.Log("GetPacketTypeex: " + ex.Message, ErrorLog.LOG_TYPE.INFORMATIONAL);
                return ((PACKET_TYPES)PACKET_TYPES.INVALID_PACKET);
            }

        }


        bool CheckAllPacketTypes(PACKET_TYPES pktTpye)
        {
            foreach (PACKET_TYPES item in Enum.GetValues(typeof(PACKET_TYPES)))
            {
                if (item == PACKET_TYPES.INVALID_PACKET) continue;

                if (pktTpye == item) return true;
            }

            return (false);
        }


        byte[] Slice(byte[] inArray, int start, int count)
        {

            try
            {
                byte[] outArray = new byte[count];
                for (int i = 0; i < count; i++)
                {
                    outArray[i] = inArray[start + i];
                }
                return (outArray);
            }
            catch (Exception ex)
            {
                m_Log.Log("Slice: " + ex.Message, ErrorLog.LOG_TYPE.INFORMATIONAL);
                return (null);
            }
        }

        void InsertStartOfPacket(byte[] pkt)
        {
            
        }

        uint m_OutBoundSequenceNumber = 0;

        public byte[] CreatePacket(PACKET_TYPES type, byte[] data, string jpegInfo)
        {
           
            try
            {
                byte[] hdrBytes = null;
                byte[] packet = null;
                switch (type)
                {
                    case PACKET_TYPES.SEND_LIVE_VIEW:
                        {
                            LIVE_VIEW_COMBINED_PACKET_STRUCT pkt = new LIVE_VIEW_COMBINED_PACKET_STRUCT(); // live view packet includes the common header

                            pkt.InfoString = jpegInfo;
                            pkt.header.PACKET_TYPE = (uint)type;

                            pkt.header.START_OF_PACKET = 0xaa55aa55;

                            pkt.header.PACKET_LEN =(uint) (Marshal.SizeOf(typeof(LIVE_VIEW_COMBINED_PACKET_STRUCT)) - Marshal.SizeOf(typeof(PACKET_HEADER_STRUCT))); // just the live view specific header bytes

                            pkt.JPEG_LENGTH = (uint)data.Length; // the rest of the payload after the live view specific header bytes

                       //     pkt.header.PACKET_LEN = (uint)(data.Length + Marshal.SizeOf(typeof(LIVE_VIEW_PACKET)) - Marshal.SizeOf(typeof(PACKET_HEADER_STRUCT)));

                            pkt.header.SEQUENCE_NUMBER = (uint)m_OutBoundSequenceNumber++;

                            byte seqNum = (byte)(pkt.header.SEQUENCE_NUMBER & 0xff);
                            string seqNumS = seqNum.ToString();
                            pkt.header.PasswordHash = " "; // no outbound pw


                                   // copy the combined headers  to the bytes array

                            hdrBytes = new byte[Marshal.SizeOf(typeof(LIVE_VIEW_COMBINED_PACKET_STRUCT))];

                            GCHandle handle = GCHandle.Alloc(hdrBytes, GCHandleType.Pinned);
                            Marshal.StructureToPtr(pkt, handle.AddrOfPinnedObject(), true);
                            handle.Free();

                            packet = new byte[hdrBytes.Length + data.Length];

                            int i =0;
                            for (i = 0; i < hdrBytes.Length; i++)
                            {
                                packet[i] = hdrBytes[i];
                            }

                            for (i =  hdrBytes.Length; i < hdrBytes.Length + data.Length; i++)
                            {
                                packet[i] = data[i - hdrBytes.Length];
                            }
                        }
                        break;

                }
                return (packet);
            }
            catch (Exception ex)
            {
                m_Log.Log("CreatePacket: " + ex.Message, ErrorLog.LOG_TYPE.INFORMATIONAL);
                return (null);
            }
        }

        public byte[] CreatePacket(string password, PACKET_TYPES type, string[] data)
        {


            try
            {
                byte[] hdrBytes = null;
                byte[] packet = null;
                switch (type)
                {

                    case PACKET_TYPES.REPLY_CHANNEL_LIST:
                        {
                            // count the number of bytes needed for the payload

                            int count = 0;
                            foreach (string s in data)
                            {
                                count += (s.Length+1);  // add one for the null terminator
                            }


                            // header
                            PACKET_HEADER_STRUCT hdr = new PACKET_HEADER_STRUCT();
                            hdr.PACKET_TYPE = (uint)type;
                            hdr.PACKET_LEN = (uint)count;
                            hdr.SEQUENCE_NUMBER = (uint)m_OutBoundSequenceNumber++;
                            hdrBytes = new byte[Marshal.SizeOf(typeof(PACKET_HEADER_STRUCT))];
                            string seqNumS = ((byte)hdr.SEQUENCE_NUMBER).ToString();
                            hdr.PasswordHash = EncyptGetHostPassword(password + "," + seqNumS);

                            hdr.START_OF_PACKET = 0xaa55aa55;

                            hdrBytes = new byte[hdrBytes.Length];

                            GCHandle handle = GCHandle.Alloc(hdrBytes, GCHandleType.Pinned);
                            Marshal.StructureToPtr(hdr, handle.AddrOfPinnedObject(), true);
                            handle.Free();
                   
                         
                           
                            packet = new byte[hdrBytes.Length + count];

                            // now build the packet

                            int i =0;
                            int cnt = 0;
                            for (i = 0; i < hdrBytes.Length; i++)
                            {
                                packet[i] = hdrBytes[i];
                            }

                            foreach (string s in data)
                            {
                                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
                                byte[] dataBytes = encoding.GetBytes(s);

                                int end =  s.Length + i;
                                cnt = 0;
                                for ( ; i < end; i++)
                                {
                                    packet[i] = dataBytes[cnt];
                                   
                                    cnt++;
                                }
                                packet[i++] = 0;// null terminator for each string
                            }
                        }
                        break;

                  
                }
                return (packet);
            }
            catch (Exception ex)
            {
                m_Log.Log("CreatePacket: " + ex.Message, ErrorLog.LOG_TYPE.INFORMATIONAL);
                return (null);
            }
        }


        //REQUEST_CHANNEL_LIST:
        public byte[] CreatePacket(string password, PACKET_TYPES type)
        {


            try
            {
                byte[] hdrBytes = null;
                byte[] packet = null;
                switch (type)
                {

                    case PACKET_TYPES.REQUEST_CHANNEL_LIST:
                        {

                            PACKET_HEADER_STRUCT hdr = new PACKET_HEADER_STRUCT();

                            hdr.PACKET_TYPE = (uint)type;
                            hdr.PACKET_LEN = (uint)0;
                            hdr.SEQUENCE_NUMBER = (uint)m_OutBoundSequenceNumber++;
                            hdrBytes = new byte[Marshal.SizeOf(typeof(PACKET_HEADER_STRUCT))];
                            string seqNumS = ((byte)hdr.SEQUENCE_NUMBER).ToString();
                            hdr.PasswordHash = EncyptGetHostPassword(password + "," + seqNumS);
                            hdr.START_OF_PACKET = 0xaa55aa55;

                            packet = new byte[Marshal.SizeOf(typeof(PACKET_HEADER_STRUCT))];

                            GCHandle handle = GCHandle.Alloc(packet, GCHandleType.Pinned);
                            Marshal.StructureToPtr(hdr, handle.AddrOfPinnedObject(), true);
                            handle.Free();
                        }
                        break;

                    case PACKET_TYPES.REQUEST_STATS:
                        {

                            PACKET_HEADER_STRUCT hdr = new PACKET_HEADER_STRUCT();

                            hdr.PACKET_TYPE = (uint)type;
                            hdr.PACKET_LEN = (uint)0;
                            hdr.SEQUENCE_NUMBER = (uint)m_OutBoundSequenceNumber++;
                            hdrBytes = new byte[Marshal.SizeOf(typeof(PACKET_HEADER_STRUCT))];
                            string seqNumS = ((byte)hdr.SEQUENCE_NUMBER).ToString();
                            hdr.PasswordHash = EncyptGetHostPassword(password + "," + seqNumS);
                            hdr.START_OF_PACKET = 0xaa55aa55;

                            packet = new byte[Marshal.SizeOf(typeof(PACKET_HEADER_STRUCT))];

                            GCHandle handle = GCHandle.Alloc(packet, GCHandleType.Pinned);
                            Marshal.StructureToPtr(hdr, handle.AddrOfPinnedObject(), true);
                            handle.Free();
                        }
                        break;
                }
                return (packet);
            }
            catch (Exception ex)
            {
                m_Log.Log("CreatePacket: " + ex.Message, ErrorLog.LOG_TYPE.INFORMATIONAL);
                return (null);
            }
        }

        public byte[] CreatePacket(PACKET_TYPES type, string data)
        {
            

            try
            {
                byte[] hdrBytes = null;
                byte[] packet = null;
                switch (type)
                {

                    case PACKET_TYPES.REQUEST_CHECK_VIEWER_PASSWORD:
                    case PACKET_TYPES.REQUEST_CHECK_ADMIN_PASSWORD:
                        {

                            PACKET_HEADER_STRUCT hdr = new PACKET_HEADER_STRUCT();

                            hdr.PACKET_TYPE = (uint)type;
                            hdr.PACKET_LEN = (uint)0;
                            hdr.SEQUENCE_NUMBER = (uint)m_OutBoundSequenceNumber++;
                            hdrBytes = new byte[Marshal.SizeOf(typeof(PACKET_HEADER_STRUCT))];
                            string seqNumS = ((byte)hdr.SEQUENCE_NUMBER).ToString();
                            hdr.PasswordHash = EncyptGetHostPassword(data + "," + seqNumS);
                            hdr.START_OF_PACKET = 0xaa55aa55;

                            packet = new byte[Marshal.SizeOf(typeof(PACKET_HEADER_STRUCT))];

                            GCHandle handle = GCHandle.Alloc(packet, GCHandleType.Pinned);
                            Marshal.StructureToPtr(hdr, handle.AddrOfPinnedObject(), true);
                            handle.Free();


                        }
                        break;
                    case PACKET_TYPES.SEND_HOST_NAME:
                    case PACKET_TYPES.SEND_STATS:
                        {
                            PACKET_HEADER_STRUCT hdr = new PACKET_HEADER_STRUCT();

                            hdr.PACKET_TYPE = (uint)type;
                            hdr.PACKET_LEN = (uint)data.Length;
                            hdr.SEQUENCE_NUMBER = (uint)m_OutBoundSequenceNumber++;
                            hdrBytes = new byte[Marshal.SizeOf(typeof(PACKET_HEADER_STRUCT))];
                            hdr.START_OF_PACKET = 0xaa55aa55;

                            GCHandle handle = GCHandle.Alloc(hdrBytes, GCHandleType.Pinned);
                            Marshal.StructureToPtr(hdr, handle.AddrOfPinnedObject(), true);
                            handle.Free();

                            if (data == null) data = "0";

                            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
                            byte[] dataBytes = encoding.GetBytes(data);

                            packet = new byte[hdrBytes.Length + dataBytes.Length];

                           
                            for (int i = 0; i < hdrBytes.Length; i++)
                            {
                                packet[i] = hdrBytes[i];
                            }
                            for (int i = hdrBytes.Length; i < hdrBytes.Length + dataBytes.Length; i++)
                            {
                                packet[i] = dataBytes[i - hdrBytes.Length];
                            }
                        }
                        break;

                    case PACKET_TYPES.REPLY_INVALID_ADMIN_PASSWORD_ERROR:
                    case PACKET_TYPES.REPLY_INVALID_VIEWER_PASSWORD_ERROR:
                    case PACKET_TYPES.REPLY_INVALID_HOSTNAME_PASSWORD_ERROR : 
                        {
                            PACKET_HEADER_STRUCT hdr = new PACKET_HEADER_STRUCT();

                            hdr.PACKET_TYPE = (uint)type;
                            hdr.PACKET_LEN = (uint)data.Length;
                            hdr.SEQUENCE_NUMBER = (uint)m_OutBoundSequenceNumber++;
                            hdrBytes = new byte[Marshal.SizeOf(typeof(PACKET_HEADER_STRUCT))];
                            hdr.START_OF_PACKET = 0xaa55aa55;

                            GCHandle handle = GCHandle.Alloc(hdrBytes, GCHandleType.Pinned);
                            Marshal.StructureToPtr(hdr, handle.AddrOfPinnedObject(), true);

                            if (data == null) data = "0";

                            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
                            byte[] dataBytes = encoding.GetBytes(data);

                            packet = new byte[hdrBytes.Length + dataBytes.Length];

                            for (int i = 0; i < hdrBytes.Length; i++)
                            {
                                packet[i] = hdrBytes[i];
                            }
                            for (int i = hdrBytes.Length; i < hdrBytes.Length + dataBytes.Length; i++)
                            {
                                packet[i] = dataBytes[i - hdrBytes.Length];
                            }

                        }
                        break;


                }
                return (packet);
            }
            catch (Exception ex)
            {
                m_Log.Log("CreatePacket: " + ex.Message, ErrorLog.LOG_TYPE.INFORMATIONAL);
                return (null);
            }
        }


        public byte[] CreatePacket(string remoteHostIP, PACKET_TYPES  type, string ViewerPassword, string jpegInfo)
        {
            if (remoteHostIP == null) remoteHostIP = " ";
            if (ViewerPassword == null) ViewerPassword = " ";

            try
            {
                byte[] bytes = null;

                switch (type)
                {
                    case PACKET_TYPES.REQUEST_LIVE_VIEW:
                        {
                            LIVE_VIEW_COMBINED_PACKET_STRUCT pkt = new LIVE_VIEW_COMBINED_PACKET_STRUCT();

                            pkt.InfoString = jpegInfo;
                            pkt.header.PACKET_TYPE = (uint)type;
                            pkt.header.PACKET_LEN = (uint)(Marshal.SizeOf(typeof(LIVE_VIEW_COMBINED_PACKET_STRUCT)) - Marshal.SizeOf(typeof(PACKET_HEADER_STRUCT)));

                            pkt.header.SEQUENCE_NUMBER = (uint)m_OutBoundSequenceNumber++;

                            byte seqNum = (byte)(pkt.header.SEQUENCE_NUMBER & 0xff);
                            string seqNumS = seqNum.ToString();
                            pkt.header.PasswordHash = EncyptGetHostPassword(ViewerPassword + "," + seqNumS);
                            pkt.header.START_OF_PACKET = 0xaa55aa55;

                         

                            bytes = new byte[Marshal.SizeOf(typeof(LIVE_VIEW_COMBINED_PACKET_STRUCT))];

                            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
                            Marshal.StructureToPtr(pkt, handle.AddrOfPinnedObject(), true);
                            handle.Free();


                        }
                        break;

                    case PACKET_TYPES.REQUEST_HOST_NAME:
                        {
                            REQUEST_HOST_NAME pkt = new REQUEST_HOST_NAME();


                            pkt.header.PACKET_TYPE = (int)PACKET_TYPES.REQUEST_HOST_NAME;
                            pkt.header.PACKET_LEN = (uint)0;
                            pkt.header.SEQUENCE_NUMBER = (uint)m_OutBoundSequenceNumber++;
                            byte seqNum = (byte)(pkt.header.SEQUENCE_NUMBER & 0xff);
                            string seqNumS = seqNum.ToString();
                            pkt.header.PasswordHash = EncyptGetHostPassword(remoteHostIP+","+seqNumS);
                            pkt.header.START_OF_PACKET = 0xaa55aa55;

                            bytes = new byte[Marshal.SizeOf(typeof(REQUEST_HOST_NAME))];

                            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
                            Marshal.StructureToPtr(pkt, handle.AddrOfPinnedObject(), true);
                            handle.Free();

                        }
                        break;
                }
                return (bytes);
            }
            catch (Exception ex)
            {
                m_Log.Log("CreatePacket: " + ex.Message, ErrorLog.LOG_TYPE.INFORMATIONAL);
                return (null);
            }
        }


        public byte[] CreatePacket( PACKET_TYPES type)
        {
            try
            {
                byte[] hdrBytes = null;
          
                switch (type)
                {
                    case PACKET_TYPES.REPLY_VALID_ADMIN_PASSWORD:
                        {
                            PACKET_HEADER_STRUCT hdr = new PACKET_HEADER_STRUCT();


                            hdr.PACKET_TYPE = (uint)type;
                            hdr.PACKET_LEN = (uint)0;
                            hdr.SEQUENCE_NUMBER = (uint)m_OutBoundSequenceNumber++;
                            hdr.START_OF_PACKET = 0xaa55aa55;

                            hdrBytes = new byte[Marshal.SizeOf(typeof(PACKET_HEADER_STRUCT))];

                            GCHandle handle = GCHandle.Alloc(hdrBytes, GCHandleType.Pinned);
                            Marshal.StructureToPtr(hdr, handle.AddrOfPinnedObject(), true);
                            handle.Free();

                        }
                        break;

                    case PACKET_TYPES.REPLY_VALID_VIEWER_PASSWORD:
                        {
                            PACKET_HEADER_STRUCT hdr = new PACKET_HEADER_STRUCT();


                            hdr.PACKET_TYPE = (uint)type;
                            hdr.PACKET_LEN = (uint)0;
                            hdr.SEQUENCE_NUMBER = (uint)m_OutBoundSequenceNumber++;
                            hdr.START_OF_PACKET = 0xaa55aa55;
                            hdrBytes = new byte[Marshal.SizeOf(typeof(PACKET_HEADER_STRUCT))];

                            GCHandle handle = GCHandle.Alloc(hdrBytes, GCHandleType.Pinned);
                            Marshal.StructureToPtr(hdr, handle.AddrOfPinnedObject(), true);
                            handle.Free();

                        }
                        break;
                }
                return (hdrBytes);
            }
            catch (Exception ex)
            {
                m_Log.Log("CreatePacket: " + ex.Message, ErrorLog.LOG_TYPE.INFORMATIONAL);
                return (null);
            }
        }


        public byte[] CreateTestPacket()
        {
            try
            {
                LIVE_VIEW_COMBINED_PACKET_STRUCT pkt = new LIVE_VIEW_COMBINED_PACKET_STRUCT();

                pkt.InfoString = "kjhaksjdfhalksjdfhlaksjdf";
                pkt.header.PACKET_TYPE = (int)PACKET_TYPES.SEND_LIVE_VIEW;
                pkt.header.SEQUENCE_NUMBER = (uint)m_OutBoundSequenceNumber++;
                pkt.header.PasswordHash = "502834750293845";
                pkt.header.START_OF_PACKET = 0xaa55aa55;

                byte[] bytes = new byte[Marshal.SizeOf(typeof(LIVE_VIEW_COMBINED_PACKET_STRUCT))];

                GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
                Marshal.StructureToPtr(pkt, handle.AddrOfPinnedObject(), true);
                handle.Free();

                return (bytes);
            }
            catch (Exception ex)
            {
                m_Log.Log("CreatePacket: " + ex.Message, ErrorLog.LOG_TYPE.INFORMATIONAL);
                return (null);
            }
        }



        ///////////////////////////////////////////////////
        ///
        ///
        //   LIVE VIEW INFO STRING PARSING

        public string BuildInfoString(LIVE_VIEW_HEADER header)
        {
            string info;

            info ="CN:" + header.cameraName;
            info += ",";
            info += "TS:" + header.timeStamp;
            info += ",";
            info += "PN:" + header.plateNumber;

            return (info);
        }

        public LIVE_VIEW_HEADER ParseInfoString(string info)
        {
            try
            {
                LIVE_VIEW_HEADER header = new LIVE_VIEW_HEADER();

                string[] sp1 = info.Split(',');

                foreach (string s1 in sp1)
                {
                    string[] sp2 = s1.Split(':');

                    if (sp2[0].Equals("CN"))
                        header.cameraName = sp2[1];
                    if (sp2[0].Equals("TS"))
                        header.timeStamp = sp2[1];
                    if (sp2[0].Equals("PN"))
                        header.plateNumber = sp2[1];
                    
                }
                return (header);
            }
            catch (Exception ex)
            {
                m_Log.Log("ParseInfoString ex: " + ex.Message, ErrorLog.LOG_TYPE.INFORMATIONAL);
                return (null);
            }
        }

        ///////////////////////////////////////////////////
        ///
        ///
        //   STATS STRING PARSING

        public string BuildStatsString(APPLICATION_DATA.HEALTH_STATISTICS stats)
        {
            string info=null;

            string[] statNames = stats.GetStatList();
            string s;

            foreach (string name in statNames)
            {
                s = stats[name].PerSecond.GetNameValue();
                if (s != null)
                {
                    info += s;
                    info += ",";
                }

                s = stats[name].RunningAverage.GetNameValue();
                 if (s != null)
                 {
                     info += s;
                     info += ",";
                 }

                 s = stats[name].Accumulator.GetNameValue();
                 if (s != null)
                 {
                     info += s;
                     info += ",";
                 }

                 s = stats[name].Snapshot.GetNameValue();
                 if (s != null)
                 {
                     info += s;
                     info += ",";
                 }

                 s = stats[name].Peak.GetNameValue();
                 if (s != null)
                 {
                     info += s;
                     info += ",";
                 }


                 s = stats[name].StatString.GetNameValue();
                 if (s != null)
                 {
                     info += s;
                     info += ",";
                 }


                 s = stats[name].boolean.GetNameValue();
                 if (s != null)
                 {
                     info += s;
                     info += ",";
                 }



                 s = stats[name].SnapshotDouble.GetNameValue();
                 if (s != null)
                 {
                     info += s;
                     info += ",";
                 }

            }
          

            return (info);
        }


    }
}
