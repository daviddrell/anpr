using System;
using System.Collections.Generic;
using System.Collections;
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
using System.Reflection;
using System.Runtime.InteropServices;

namespace RCSClientLib
{
    // top part of file has message parsers for specific Rx messages

    // bottom part of this file has the class definitionfor the class the handles pushing messages to registered client's message handlers-->> public RxMesgHandlers ProcessRxMesgs;


    public partial class RCSClient
    {

       

        //  ///////////////////////////////////////////////////
        //
        //   pull message bodies from the socket and then push to the registered clients.
        // 
        void HandleValidViewerPassword(RCS_Protocol.RCS_Protocol.PACKET_HEADER  header, byte [] payload)
        {
            m_Log.Log("HandleValidAdminPassword", ErrorLog.LOG_TYPE.INFORMATIONAL);

            if (MessageEventGenerators.OnRxValidViewerPW != null) MessageEventGenerators.OnRxValidViewerPW();
        }

        void HandleValidAdminPassword(RCS_Protocol.RCS_Protocol.PACKET_HEADER header, byte[] payload)
        {
            m_Log.Log("HandleValidAdminPassword", ErrorLog.LOG_TYPE.INFORMATIONAL);

            if (MessageEventGenerators.OnRxValidAdminPW != null) MessageEventGenerators.OnRxValidAdminPW();
        }



        void HandleInvalidViewerPassword(RCS_Protocol.RCS_Protocol.PACKET_HEADER header, byte[] payload)
        {
            m_Log.Log("HandleInvalidViewerPassword", ErrorLog.LOG_TYPE.INFORMATIONAL);

            string message = ExtractMessage(payload);
            if (MessageEventGenerators.OnRxInvalidPassword != null) MessageEventGenerators.OnRxInvalidPassword(message);

        }

        void HandleInvalidAdminPassword(RCS_Protocol.RCS_Protocol.PACKET_HEADER header, byte[] payload)
        {
            m_Log.Log("HandleinInvalidAdminPassword", ErrorLog.LOG_TYPE.INFORMATIONAL);

            string message = ExtractMessage(payload);
            if (MessageEventGenerators.OnRxInvalidPassword != null) MessageEventGenerators.OnRxInvalidPassword(message);
        }

        void HandleInvalidHostnamePassword(RCS_Protocol.RCS_Protocol.PACKET_HEADER header, byte[] payload)
        {
            m_Log.Log("HandleInvalidHostnamePassword", ErrorLog.LOG_TYPE.INFORMATIONAL);

            string message = ExtractMessage(payload);
            if (MessageEventGenerators.OnRxInvalidPassword != null) MessageEventGenerators.OnRxInvalidPassword(message);
        }

        string ExtractMessage(byte[] pkt)
        {
            string message = "";
            try
            {
                message = System.Text.ASCIIEncoding.ASCII.GetString(pkt);
            }
            catch (Exception ex)
            {
                m_Log.Log("ExtractMessage ex:" + ex.Message, ErrorLog.LOG_TYPE.INFORMATIONAL);
                return "";
            }

            return (message);
        }



        void HandleReceiveStats(RCS_Protocol.RCS_Protocol.PACKET_HEADER header, byte[] payload)
        {

             string s = System.Text.ASCIIEncoding.ASCII.GetString(payload);
             //APPLICATION_DATA.HEALTH_STATISTICS stats =  m_RCSProtocol.ParseStats(s);
             //if (stats == null) return;

             MessageEventGenerators.OnRxHealthStatus(s);
        }


        void HandleReceiveChannelList(RCS_Protocol.RCS_Protocol.PACKET_HEADER header, byte[] payload)
        {
            if (payload == null) return;
            if (header == null) return;

            try
            {
                List<string> nameslist = new List<string>();

                int offset = 0;
                for (int i = 0; i < payload.Length; i++)
                {
                    if (payload[i] == 0)
                    {
                        string s = System.Text.ASCIIEncoding.ASCII.GetString(payload, offset, i - offset);
                        offset = i + 1;
                        nameslist.Add(s);
                    }
                }
                string[] list = nameslist.ToArray();

                if (MessageEventGenerators.OnRxChannelList != null) MessageEventGenerators.OnRxChannelList(list);
                
            }
            catch (Exception ex)
            {
                m_Log.Log("HandleReceiveChannelList ex:" + ex.Message, ErrorLog.LOG_TYPE.INFORMATIONAL);
            }
        }


        void HandleReceiveHostName(RCS_Protocol.RCS_Protocol.PACKET_HEADER header, byte[] payload)
        {
           

            string name = "";
            try
            {
                name = System.Text.ASCIIEncoding.ASCII.GetString(payload);
            }
            catch (Exception ex)
            {
                m_Log.Log("HandleReceiveHostName ex:" + ex.Message, ErrorLog.LOG_TYPE.INFORMATIONAL);
                return;
            }

            if (MessageEventGenerators.OnRxHostName != null) MessageEventGenerators.OnRxHostName(name);
             
        }



        //  ///////////////////////////////////////////////////
        //
        //   Receive JPEG images
        //


        void HandleReceiveJPeg(RCS_Protocol.RCS_Protocol.PACKET_HEADER header, byte[] payload, string channel, string timeStamp, string plateReading)
        {
            MessageEventGenerators.OnRxJpeg(payload, channel, timeStamp, plateReading);
        }


    
        

        public MessageEventGeneratorClass MessageEventGenerators;

        public class MessageEventGeneratorClass
        {

            public delegate void ProcessRxHealthStatus(string statusinfo);
            public ProcessRxHealthStatus OnRxHealthStatus;

            public delegate void ProcessRxHostName(string hostName);
            public ProcessRxHostName OnRxHostName;

            public delegate void ProcessRxJpeg(byte[] jpeg, string channel, string timeStamp, string plateReading);
            public ProcessRxJpeg OnRxJpeg;

            public delegate void ProcessRxInvalidPassword(string message);
            public ProcessRxInvalidPassword OnRxInvalidPassword;

            public delegate void ProcessRxValidAdminPW( );
            public ProcessRxValidAdminPW OnRxValidAdminPW;

            public delegate void ProcessRxValidViewerPW( );
            public ProcessRxValidViewerPW OnRxValidViewerPW;

            public delegate void ProcessRxChannelList(string[] list);
            public ProcessRxChannelList OnRxChannelList;

        }
    }
}