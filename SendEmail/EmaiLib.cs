using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using UserSettingsLib;
using EncryptionLib;
using ErrorLoggingLib;
using ApplicationDataClass;
using System.Threading;
using Utilities;
using DVRLib;

namespace EmailServicesLib
{
    public partial class EmailServices
    {
        public EmailServices(APPLICATION_DATA appData)
        {
            m_AppData = appData;
            m_AppData.AddOnClosing(Stop, APPLICATION_DATA.CLOSE_ORDER.MIDDLE);
            m_Log = (ErrorLog) m_AppData.Logger;
            m_SendMessagesQ = new ThreadSafeQueue<SEND_MESSAGE>(40);
            m_SendResultEvents = new ThreadSafeHashTable(40);
            m_SendResultQ = new ThreadSafeQueue<SEND_RESULT>(40);
          
        }

        public void StartThreads()
        {
            EmaiSettings = new EMAIL_SETTINGS();
            EmaiSettings.LoadSettings();

            m_PushResultsThread = new Thread(PushResultsLoop);
            m_PushResultsThread.Start();

            m_SendMessagesThread = new Thread(SendMessagesLoop);
            m_SendMessagesThread.Start();
        }

        public EMAIL_SETTINGS EmaiSettings;
        public class SEND_MESSAGE
        {
            public string subject;
            public string to;
            public string from;
            public string body;
            public string attachment;
            public SEND_RESULT_EVENT sendResultCallBack;
            public string Mail_Key // unique identifier to tie the send message to the send-status callback
            {
                get
                {
                    return(subject + ", to: " + to);
                }
            }
        }

        public class SEND_RESULT
        {
            public string Mail_Key;
            public string Commentary;
            public bool Success;
        }

        APPLICATION_DATA m_AppData;
        ErrorLog m_Log;
        ThreadSafeHashTable m_SendResultEvents;
        ThreadSafeQueue<SEND_MESSAGE> m_SendMessagesQ;
        ThreadSafeQueue<SEND_RESULT> m_SendResultQ;
        public delegate void SEND_RESULT_EVENT(SEND_RESULT result);
       

        /// <summary>
        /// Send a composed message (message composition must include a results callback
        /// </summary>
        /// <param name="message"></param>
        public void SendMessage (SEND_MESSAGE message)
        {
            if (!EmaiSettings.EmailDataLoaded) return;

            m_SendMessagesQ.Enqueue(message);
        }

        bool m_Stop;

        void Stop(){m_Stop = true;}

        Thread m_SendMessagesThread;
        Thread m_PushResultsThread;

        public string FromAddress
        {
            get
            {
               
                return EmaiSettings.FromAddress;
            }
        }

        void SendMessagesLoop()
        {
            SEND_MESSAGE message;

            while (!m_Stop)
            {
                Thread.Sleep(10);

                message = m_SendMessagesQ.Dequeue();
                if (message != null)
                {
                    m_SendResultEvents.Add(message.Mail_Key,(object) message.sendResultCallBack);
                    SendMessageToDotNet(message);
                }

            }
        }

        void PushResultsLoop()
        {
            SEND_RESULT result;

            while (!m_Stop)
            {
                Thread.Sleep(10);

                result = m_SendResultQ.Dequeue();
                if (result != null)
                {
                    // look up the callback function to use in the hash table
                    object cb = m_SendResultEvents[result.Mail_Key];
                    if (cb != null)
                    {
                        SEND_RESULT_EVENT sendEvent = (SEND_RESULT_EVENT)cb;
                        
                        // raise the event - notify the message originator of the result
                        sendEvent(result);

                        // remove the event handler from the hash table for this message
                        m_SendResultEvents.Remove(result.Mail_Key);
                    }
                }   
            }
        }

        void SendMessageToDotNet(SEND_MESSAGE message)
        {
            try
            {
                DateTime timeNow = DateTime.Now;

                // from, to
                MailMessage theMailMessage = new MailMessage(message.from,message.to);


                if (message.attachment != null)
                {
                   
                    Attachment newAttachement = new Attachment(message.attachment);

                    theMailMessage.Attachments.Add(newAttachement);
                }

                theMailMessage.Body = message.body;
                theMailMessage.Subject = message.subject;
                SmtpClient theClient = new SmtpClient(EmaiSettings.OutBoundServer);
                theClient.UseDefaultCredentials = false;
                System.Net.NetworkCredential theCredential = new System.Net.NetworkCredential(EmaiSettings.UserName, EmaiSettings.Password);
                theClient.Credentials = theCredential;
                theClient.SendCompleted += new SendCompletedEventHandler(theClient_SendCompleted);


                // The userState can be any object that allows your callback 
                // method to identify this send operation.

                string userState = message.Mail_Key;

                theClient.SendAsync(theMailMessage, userState);
               
            }
            catch (Exception ex)
            {
                SEND_RESULT result = new SEND_RESULT();
                result.Commentary = ex.Message ;
                result.Success = false;
                result.Mail_Key = message.Mail_Key;
                m_SendResultQ.Enqueue(result);
                m_Log.Log("SendMessageToDotNet ex: " + result.Commentary, ErrorLog.LOG_TYPE.INFORMATIONAL);
            }
        }

        void theClient_SendCompleted(object sender, AsyncCompletedEventArgs e)
        {
            try
            {
                string mailKey =(string) e.UserState;

                object callBack = m_SendResultEvents[mailKey];

                if (callBack != null)
                {
                    SEND_RESULT result = new SEND_RESULT();

                    result.Mail_Key = mailKey;
                    if (e.Error != null || e.Cancelled)
                    {
                        result.Success = false;
                        if (e.Cancelled) result.Commentary = "Send Cancelled";
                        else result.Commentary = e.Error.Message;
                    }
                    else
                    {
                        result.Commentary = "Send Message Sucessful";
                        result.Success = true;
                    }

                    m_SendResultQ.Enqueue(result);
                }
            }
            catch (Exception ex)
            {
                m_Log.Log("theClient_SendCompleted ex: " + ex.Message, ErrorLog.LOG_TYPE.INFORMATIONAL);
            }
        }

    

    }
}
