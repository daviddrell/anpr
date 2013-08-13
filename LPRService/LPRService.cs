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

namespace LPRService
{
    public class LPRServiceBody
    {

        public LPRServiceBody()
        {
            m_AppData = new APPLICATION_DATA();
     
           
            m_AppData.HealthStatistics.AddStat("LPRStats");
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

        public delegate void OnSelfDestructEvent();
        /// <summary>
        /// Add the top level Main loop close event handler to this event to do the final close of the main loop 
        /// </summary>
        public OnSelfDestructEvent OnSelfDestruct;
        void HandleSelfDestructEvent()
        { // one of our threads called SelfDestruct in AppData, AppData called this, now send the event to the Main Loop to stop the service.
            OnSelfDestruct();
        }

    

        public void Start()
        {


            //////////////////////////////////////
            //
            // start error reporting lib  ( decides if local and/or remote reporting)

            m_AppData.Logger = new ErrorLog(m_AppData);

            m_Log = (ErrorLog)  m_AppData.Logger;

            //////////////////////////////////////
            //
            // start email lib (used by error reporting lib for remote error notifications and by the watch list processor)

            m_AppData.EmailServices = new EmailServices(m_AppData);
            m_Email = (EmailServices)m_AppData.EmailServices;
           
           
          
            //////////////////////////////////////
            //
            // load the Frame Generator

            m_AppData.FrameGenerator = (object)new FrameGenerator(m_AppData);


            //////////////////////////////////////
            //
            // load the DVR 

            m_DVR = new DVR(m_AppData);
            m_AppData.DVR = (object)m_DVR;


            //////////////////////////////////////
            //
            // start the TCP Server
            m_RCServer = new RemoteConnectionServer.RemoteConnectionServer(m_AppData);

            //m_TCPServerThread = new Thread(TCPServer);
            //m_TCPServerThread.Start();
         

            //////////////////////////////////////
            //
            // load the LPR Engine

            m_LPREngine = new LPREngine (m_AppData);
            m_AppData.LPREngine = m_LPREngine;

            //////////////////////////////////////
            //
            // load the Watch List Processor

            m_WatchList = new WatchLists(m_AppData);
           

            //  now that all modules are loaded, let them register with each other for event communications
            m_RCServer.StartRegistration();
            m_DVR.StartRegistration();
            m_LPREngine.StartRegistration();
            m_WatchList.StartRegistration();
       

          
            // now let all modules start their threads
            m_RCServer.StartThreads();
            m_DVR.StartThreads();
            m_LPREngine.StartThreads();
            m_WatchList.StartThreads();
            m_Email.StartThreads();

            // is everyone happy?
            if (!m_DVR.GetDVRReady || !((FrameGenerator) m_AppData.FrameGenerator).GetReadyStatus)
                m_AppData.SelfDestruct();
           
        }


        void OnGPSDeviceFailure()
        {
        }

        public void Stop()
        {
            m_Log.Log("LPRService received close notification, closing program", ErrorLog.LOG_TYPE.FATAL);

            m_StopProgram = new Thread(StopProgram);
            m_StopProgram.Start();

        }

        void StopProgram()
        {

            m_Log.Log("LPR service stopping", ErrorLog.LOG_TYPE.FATAL);
            m_AppData.CloseApplication();
           
        }

     

       
    }
}
