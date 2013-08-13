using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ApplicationDataClass;
using ErrorLoggingLib;
using LPREngineLib;
using System.Threading;
using Utilities;
using FrameGeneratorLib;
using System.IO;
using UserSettingsLib;
using LPROCR_Wrapper;
using EmailServicesLib;
using DVRLib;

namespace WatchlistLib
{
   

    //
    //  add new match events in this method:  void ComparePlateNumbersToAllWatchLists( FRAME frame)
    //
    //

    public  partial class WatchLists
    {
        /// constructor
        /// 
        public WatchLists(APPLICATION_DATA appData)
        {
            try
            {
                // load stored data
                mapURL = UserSettings.Get(UserSettingTags.GPSMAPURL);
                if (mapURL == null)
                    mapURL = "http://maps.google.com/maps?q=";

                m_AppData = appData;
                m_Log = (ErrorLog)m_AppData.Logger;
                m_AppData.AddOnClosing(Stop, APPLICATION_DATA.CLOSE_ORDER.MIDDLE);

                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.WatchList.WatchList_LastAlert].StatString.RegisterForUse(true);
                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.WatchList.WatchList_NumAlerts].Accumulator.RegisterForUse(true);
                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.WatchList.Watchlist_NumListsLoaded].Accumulator.RegisterForUse(true);

                m_Stop = false;

                m_NewLPRResultQ = new ThreadSafeQueue<FRAME>(60);
                m_AlertsToBeGeneratedQ = new ThreadSafeQueue<FRAME>(60);
    
                m_SettingsTags = new WatchListDynamicTags();

                m_LoadListSingleton = new object();

                m_WatchLists = LoadListsFromUserConfig();

                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.WatchList.Watchlist_NumListsLoaded].Accumulator.SetValue = m_WatchLists.Count;
            }
            catch (Exception ex) { m_Log.Trace(ex, ErrorLog.LOG_TYPE.FATAL); }

        }

        UserSettingsLib.WatchListDynamicTags m_SettingsTags;

        

        List<WatchListControl> m_WatchLists; // all the user generated hot lists
        ThreadSafeQueue<FRAME> m_NewLPRResultQ; // holds all new LPR results until the watch thread can get around to checking them
        ThreadSafeQueue<FRAME> m_AlertsToBeGeneratedQ; // alerts ready to be consumed by the email service or the remote client server
 
        private static ThreadSafeList<String>  m_WatchFileChangedList;
        EmailServices m_EmailService;

        Thread m_WatchThread;
        Thread m_SendAlertsThread;
  
        DVR m_DVR;

        string mapURL;

        LPREngine m_LPREngine;
        APPLICATION_DATA m_AppData;
        ErrorLog m_Log;
    
      
        bool m_Stop;
        void Stop()
        {
            m_Stop = true;
        }

        public void StartRegistration()
        {
            m_LPREngine = (LPREngine)m_AppData.LPREngine;
            m_LPREngine.OnNewFilteredPlateGroupEvent += new LPREngine.NewPlateEvent(NewLPRResultsEvent_OnNewPlateEvent);

            m_EmailService = (EmailServices) m_AppData.EmailServices;
            m_DVR = (DVR)m_AppData.DVR;
        }
     
        void NewLPRResultsEvent_OnNewPlateEvent(FRAME frame)
        {
            Console.WriteLine("WL recevied new plate");

            // put this on the que to be handeled by the watch list thread
            m_NewLPRResultQ.Enqueue(frame);
        }

        public void StartThreads()
        {

            m_WatchThread = new Thread(WatchThreadLoop);
            m_WatchThread.Priority = ThreadPriority.AboveNormal;
            m_WatchThread.Start();

            m_SendAlertsThread = new Thread(SendAlertsLoop);
            m_SendAlertsThread.Start();

        
        }

        void  GenAlertMessages(FRAME frame)
        {

            m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.WatchList.WatchList_NumAlerts].Accumulator.Increment(1);
            m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.WatchList.WatchList_LastAlert].StatString.SetValue =(string) frame.WatchListMatchingNumber;

            if ( ((WatchListControl)frame.ParentWatchList).WatchEmailAddresses == null ) return;


        
            foreach (string to in ((WatchListControl)frame.ParentWatchList).WatchEmailAddresses)
            {
                EmailServices.SEND_MESSAGE message = new EmailServices.SEND_MESSAGE();
                message.to = to;

                string attachement =  m_DVR.Paths.GetCompleteFilePath(frame.JpegFileRelativePath);
               

                /// the email server has a problem that if you attach a file to a new mail, and then
                /// send multiple emails with the same attachment, you get an exception of file open by another process.
                ///  so create a new file copy for each mail that is sent. Don't keep this file around forever.
                ///  

                message.attachment = m_DVR.GetUniqueCopy(attachement);

                message.from = m_EmailService.FromAddress;
                message.subject = "ALERT: " + frame.WatchListMatchingNumber + " Sent at " + DateTime.UtcNow.ToString();
                message.body = message.subject + Environment.NewLine + Environment.NewLine +
                    "From PSS: " + frame.PSSName + Environment.NewLine + Environment.NewLine +
                    "Camera Source: " + frame.SourceName + Environment.NewLine + Environment.NewLine +
                    "Hot list entry: " + frame.WatchListMatchingNumber +",   " +frame.WatchListMatchingNumberUserComment + Environment.NewLine + Environment.NewLine +
                    "Detected Nunmber: " + frame.BestMatchingString + Environment.NewLine + Environment.NewLine +
                    "Match Probability: " + frame.MatchScore.ToString() + "%" + Environment.NewLine + Environment.NewLine +
                    "Jpeg Image: " + frame.JpegFileRelativePath + Environment.NewLine + Environment.NewLine +
                    "GPS Location: " + frame.GPSPosition + Environment.NewLine + Environment.NewLine +
                    "Plot location URL: " + mapURL + frame.GPSPosition + Environment.NewLine + Environment.NewLine +
                    "Observation time: " + frame.TimeStamp.ToString();

                message.sendResultCallBack = OnSendMessageSendStatus;
                
                m_EmailService.SendMessage(message);

                m_Log.Log("WL sent message : " + message.subject, ErrorLog.LOG_TYPE.INFORMATIONAL);

              

              
            }
        }

        void OnSendMessageSendStatus(EmailServices.SEND_RESULT result)
        {
            m_Log.Log("Watch Send Email Result : " + result.Commentary, ErrorLog.LOG_TYPE.INFORMATIONAL);
        }

        void SendAlertsLoop()
        {
            while (!m_Stop)
            {
                try
                {
                    Thread.Sleep(10);
                    FRAME frame = m_AlertsToBeGeneratedQ.Dequeue();
                    if (frame != null)
                    {
                        Console.WriteLine("WL dequeued alert");
                        GenAlertMessages(frame);
                    }
                }
                catch (Exception ex) { m_Log.Trace(ex, ErrorLog.LOG_TYPE.FATAL); }
            }
        }


        
        void WatchThreadLoop()
        {
            int count = 0;

            while (!m_Stop)
            {
                Thread.Sleep(10);

                try
                {
                    FRAME frame = m_NewLPRResultQ.Dequeue();
                    if (frame != null)
                    {
                        Console.WriteLine("WL comparing new plate to lists");

                        ComparePlateNumbersToAllWatchLists(frame);
                    }
                }
                catch (Exception ex) { m_Log.Trace(ex, ErrorLog.LOG_TYPE.FATAL); }

                // check watch list files to see if the user has updated a list
                count++;
                try
                {
                    if (count > 100)
                    {
                        count = 0;
                        checkLists();
                    }
                }
                catch (Exception ex) { m_Log.Trace(ex, ErrorLog.LOG_TYPE.FATAL); }

            }
        }

       
        void BuidlAlertFrame(FRAME frame, string matchingPlateString, string watchNumber, string watchNumberComment, WatchListControl list, int score)
        {
            frame.BestMatchingString = matchingPlateString;
            frame.MatchScore = score;
            frame.ParentWatchList = (object)list;
            frame.WatchListMatchingNumber = watchNumber;
            frame.WatchListMatchingNumberUserComment = watchNumberComment;
        }

        //
        //  add new match events in this method
        //
        void ComparePlateNumbersToAllWatchLists( FRAME frame)
        {
            foreach (WatchListControl list in m_WatchLists)
            {
                for (int i = 0; i < list.WatchEntrys.Length; i++ )
                {
                    string watchNumber = list.WatchEntrys[i].Number;
                    foreach (string plate in frame.PlateNumberLatin)
                    {
                        int score = LPROCR_Lib.scoreMatch(plate, watchNumber);
                        if (score >= list.AlertThreshold)
                        {
                            Console.WriteLine("WL found intermediate match");

                            BuidlAlertFrame(frame, plate, watchNumber, list.WatchEntrys[i].UserComment, list, score);


                            //
                            //  Add new match events here
                            //

                            m_AlertsToBeGeneratedQ.Enqueue(frame);

                        }
                    }
                }
            }
        }

    

    }



}
