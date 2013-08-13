using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using S2255Controller;
using LPROCR_Wrapper;
using System.Drawing;
using RCS_Protocol;
using RemoteConnectionServer;
using ApplicationDataClass;
using UserSettingsLib;
using ErrorLoggingLib;
using EmailServicesLib;
using FrameGeneratorLib;
using DVRLib;
using LPREngineLib;
using WatchlistLib;

namespace LPRServiceCore
{
    public class LPRServiceEntryPoint
    {

        public LPRServiceEntryPoint()
        {
            m_AppData = new APPLICATION_DATA();

            SetConfigPath();

            m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.LPRServiceVersionString.LPRServiceVersionString_version].StatString.RegisterForUse(true);
            
            string ver = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            ver = ver.Replace('.', '^');// the . is used as a delimeter in the stats reporting

            m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.LPRServiceVersionString.LPRServiceVersionString_version].StatString.SetValue=ver;


            m_AppData.OnCloseServiceLoop += HandleSelfDestructEvent;

        }

        RemoteConnectionServer.RemoteConnectionServer m_RCServer;
        Thread m_StopProgram;
        APPLICATION_DATA m_AppData;
        EmailServices m_Email;
        ErrorLog m_Log;
        LPREngineLib.LPREngine  m_LPREngine;
        DVR m_DVR;
        WatchLists m_WatchList;

        public APPLICATION_DATA GetAppData() {return (m_AppData);}

        public delegate void OnSelfDestructEvent();
        /// <summary>
        /// Add the top level Main loop close event handler to this event to do the final close of the main loop 
        /// </summary>
        public OnSelfDestructEvent OnSelfDestruct;
        void HandleSelfDestructEvent()
        { // one of our threads called SelfDestruct in AppData, AppData called this, now send the event to the Main Loop to stop the service.
           if ( OnSelfDestruct != null) OnSelfDestruct();
        }

    

        public void Start(bool AsService)
        {
            try
            {
                m_AppData.RunninAsService = AsService;


                //////////////////////////////////////
                //
                // setup system wide health statistics 

                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.System.System_Drive].StatString.RegisterForUse(true);
                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.System.System_frameGrabber_2].StatString.RegisterForUse(true);
                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.System.System_frameGrabber_1].StatString.RegisterForUse(true);
                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.System.System_GPS].StatString.RegisterForUse(true);
                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.System.System_Hotswap].StatString.RegisterForUse(true);
                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.System.System_Service].StatString.RegisterForUse(true);
                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.System.System_videoChannel1].StatString.RegisterForUse(true);
                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.System.System_videoChannel2].StatString.RegisterForUse(true);
                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.System.System_videoChannel3].StatString.RegisterForUse(true);
                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.System.System_videoChannel4].StatString.RegisterForUse(true);

                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.QueueOverruns.QueueOverruns_DVR_DirectyToStorageQ].Accumulator.RegisterForUse(true);
                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.QueueOverruns.QueueOverruns_DVR_MotionDetectedQ].Accumulator.RegisterForUse(true);
                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.QueueOverruns.QueueOverruns_DVR_NewFrameQ].Accumulator.RegisterForUse(true);
                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.QueueOverruns.QueueOverruns_DVR_NewLPRRecordQ].Accumulator.RegisterForUse(true);
                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.QueueOverruns.QueueOverruns_FG_AllFramesConsumerPushQ].Accumulator.RegisterForUse(true);
                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.QueueOverruns.QueueOverruns_FG_MotionDetectedConsumerPushQ].Accumulator.RegisterForUse(true);
                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.QueueOverruns.QueueOverruns_FG_MotionDetectionQ].Accumulator.RegisterForUse(true);
                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.QueueOverruns.QueueOverruns_LPR_LPRFinalPlateGroupOutputQ].Accumulator.RegisterForUse(true);
                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.QueueOverruns.QueueOverruns_LPR_LPRPerFrameReadingQ].Accumulator.RegisterForUse(true);
                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.QueueOverruns.QueueOverruns_LPR_LPRProcessQ].Accumulator.RegisterForUse(true);
         

                //////////////////////////////////////
                //
                // start error reporting lib  ( decides if local and/or remote reporting)

                m_AppData.Logger = new ErrorLog(m_AppData);

                m_Log = (ErrorLog)m_AppData.Logger;

                m_Log.Log("Starting LPR Services", ErrorLog.LOG_TYPE.INFORMATIONAL);
                m_Log.Log("using file in path: " + UserSettings.GetAppPath(), ErrorLog.LOG_TYPE.INFORMATIONAL);

                //////////////////////////////////////
                //
                // start email lib (used by error reporting lib for remote error notifications and by the watch list processor)

                m_Log.Log("Loading Email module", ErrorLog.LOG_TYPE.INFORMATIONAL);

                m_AppData.EmailServices = new EmailServices(m_AppData);
                m_Email = (EmailServices)m_AppData.EmailServices;



                //////////////////////////////////////
                //
                // load the Frame Generator
            
                m_Log.Log("Loading Frame Generator module", ErrorLog.LOG_TYPE.INFORMATIONAL);
                m_AppData.FrameGenerator = (object)new FrameGenerator(m_AppData, AsService);


                //////////////////////////////////////
                //
                // load the DVR 

                m_AppData.DVRMode = APPLICATION_DATA.DVR_MODE.STORE_ON_MOTION;
                m_Log.Log("Loading DVR module", ErrorLog.LOG_TYPE.INFORMATIONAL);
                m_DVR = new DVR(m_AppData);
                m_AppData.DVR = (object)m_DVR;


                //////////////////////////////////////
                //
                // start the TCP Server
                if (m_AppData.RunninAsService)
                {
                    m_Log.Log("Loading TCP module", ErrorLog.LOG_TYPE.INFORMATIONAL);
                    m_RCServer = new RemoteConnectionServer.RemoteConnectionServer(m_AppData);
                }

            

                //////////////////////////////////////
                //
                // load the LPR Engine

                m_Log.Log("Loading LPR Engine", ErrorLog.LOG_TYPE.INFORMATIONAL);
                m_LPREngine = new LPREngine(m_AppData);
                m_AppData.LPREngine = m_LPREngine;

                //////////////////////////////////////
                //
                // load the Watch List Processor

                m_Log.Log("Loading Watch List module", ErrorLog.LOG_TYPE.INFORMATIONAL);

                m_WatchList = new WatchLists(m_AppData);


                //  now that all modules are loaded, let them register with each other for event communications
                m_Log.Log("Starting registrations", ErrorLog.LOG_TYPE.INFORMATIONAL);
                if (m_AppData.RunninAsService)
                {
                    m_RCServer.StartRegistration();
                }
                m_DVR.StartRegistration();
                m_LPREngine.StartRegistration();
                m_WatchList.StartRegistration();



                // now let all modules start their threads
                if (m_AppData.RunninAsService)
                {
                    m_Log.Log("Starting TCP Server", ErrorLog.LOG_TYPE.INFORMATIONAL);
                    m_RCServer.StartThreads();
                }

                m_Log.Log("Starting DVR", ErrorLog.LOG_TYPE.INFORMATIONAL);
                m_DVR.StartThreads();

                m_Log.Log("Starting LPR Engine", ErrorLog.LOG_TYPE.INFORMATIONAL);
                m_LPREngine.StartThreads();

                m_Log.Log("Starting Watch list processor", ErrorLog.LOG_TYPE.INFORMATIONAL);
                m_WatchList.StartThreads();

                m_Log.Log("Starting Email Services", ErrorLog.LOG_TYPE.INFORMATIONAL);
                m_Email.StartThreads();

                // is everyone happy?
                //if (!m_DVR.GetDVRReady || !((FrameGenerator)m_AppData.FrameGenerator).GetReadyStatus)
                //{
                //    m_Log.Log("Error, self destruct", ErrorLog.LOG_TYPE.FATAL);
                //    m_AppData.SelfDestruct();
                //}
            }
            catch (Exception ex) { m_Log.Trace(ex, ErrorLog.LOG_TYPE.FATAL); }
        }


        void OnGPSDeviceFailure()
        {
        }

        public void Stop()
        {
            m_Log.Log("LPRService received close notification, closing program", ErrorLog.LOG_TYPE.FATAL);


         //  m_AppData.CloseApplication();

            m_StopProgram = new Thread(StopProgram);
            m_StopProgram.Start();

        }

        void StopProgram()
        {

            m_Log.Log("LPR service stopping", ErrorLog.LOG_TYPE.FATAL);
            m_AppData.CloseApplication();
           
        }


     
        void SetConfigPath()
        {
            // where user.config and logfiles will be stored.

            string firstEvidenceDataStoragePath = "C:/FirstEvidence";
            if (!Directory.Exists(firstEvidenceDataStoragePath))
                Directory.CreateDirectory(firstEvidenceDataStoragePath);
        }

       
    }
}
