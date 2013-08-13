using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;


namespace ApplicationDataClass
{

    public partial class APPLICATION_DATA
    {
        // ///////////////////////////////////
        ///
        ///  constants
        ///  

        public int MAX_PHYSICAL_CHANNELS = 4;
        public int MAX_VIRTUAL_CHANNELS = 4; // for off line file processing.  !!! KEEP EQUAL TO MAX_PHYSICAL_CHANNELS !!!
        public int MAX_MOVIE_FILES_TO_LOAD = 50; // max movies to load at one time
        public int MAX_IMAGES_TO_EDIT = 10000; // max number of images in Analysts Workstation to load for edit
        public int MAX_DISPLAY_CHARS = 10; // max chars to display in LPR edit mode in Analysts Workstation

        public int MIN_PLATE_WIDTH;  // these are #defines in the LPR C lib, which are pulled out and loaded into these vars at run time
        public int MAX_PLATE_WIDTH;
        public int MIN_PLATE_HEIGHT;
        public int MAX_PLATE_HEIGHT;

        public bool RunninAsService = false;// this assembly is either an LPR Service (true) or Analysts Workstation App (false)

        public int MAX_SEARCH_RESULTS_TO_DISPLAY = 10000; // used by the search tool
     
        ////////////////////////////////////
        /// 
        ///   DVR Mode
        ///   
        ///
        public enum DVR_MODE { STORE_ON_MOTION, STORE_ON_PLATE_FOUND }
        public DVR_MODE DVRMode;
        public bool DVR_StoreToUserSpecifiedFolder = false;// used in Analysts Workstation 
        public string DVR_UserSpecifiedStoragePath=null;


        ////////////////////////////////////
        /// 
        ///   enums globally shared wit the entire application
        ///   
        ///
        public enum IMAGE_FORMATS { JPEG, BMP }



        ////////////////////////////////////
        /// 
        ///   Keep Time display consistant, 24 hour clock, UTC
        ///   
        ///
        public String TimeFormatStringForFileNames = "yyyy_MM_dd_HH_mm_ss_ffff";
        public String TimeFormatStringForDisplay = "dd MMM yyyy,  HH: mm: ss: ffff (UTC)";
  
       
        ////////////////////////////////////
        /// 
        ///   Objects globally shared with the entire application
        ///   
        ///

        public object LPREngine;
        public object S2255Controller;
        public byte[] CurrentJpeg;
        public object Logger;
        public string ThisComputerName;
        public object LogToScreen;
        public object tcpconnection;
        public object FrameGenerator;
        public object TCPClient;
        public object RCSProtocol;
        public object SelectServer;
        public string ServiceAdminPW;
        public string ServiceViewPW;
        public object PathManager;
        public object EmailServices;
        public object DVR;

        object SerialNumberLock;
        int globalFrameSerialNumber;
        public int GlobalFrameSerialNumber
        { get { lock (SerialNumberLock) { return globalFrameSerialNumber; } } set { lock (SerialNumberLock) { globalFrameSerialNumber = value; } } }

        // LPR Service config and control
        public class LPR_SERVICE_CONTROL { public bool ConfigChangedServiceNeedsRestarting; }
        public LPR_SERVICE_CONTROL LPRServiceControl;
     
        // LPR Process getting behind - Analysts Workstation can slow down flow of jpeg source images
        public bool LPRGettingBehind;

        // MotionDetection getting behind - - Analysts Workstation can slow down flow of jpeg source images
        public bool MotionDetectionGettingBehind;

        // DVRStoringLPRRecords getting behind - - Analysts Workstation can slow down flow of jpeg source images
        public bool DVRStoringLPRRecordsGettingBehind;

        // In Batch Processing Mode, allow user to specify camera name to organize the output data
        public string UserSpecifiedCameraName;

        // for test
        public delegate void LogToScreenDel(string text);

        public bool LPRDiagEnabled = false;

        ////////////////////////////////////
        /// 
        ///   Ordered shutdown
        ///   
        ///

        public delegate void _AppClosingNotifierMethod();
        public enum CLOSE_ORDER { FIRST, MIDDLE, LAST };
        class APP_CLOSING_ITEM
        {
            public _AppClosingNotifierMethod closeMethod;
            public CLOSE_ORDER order;
        }

        List<APP_CLOSING_ITEM> AppClosingNotifiers;

        public HEALTH_STATISTICS HealthStatistics;
        
       
        public void AddOnClosing(_AppClosingNotifierMethod notifier, CLOSE_ORDER order)
        {
            APP_CLOSING_ITEM item = new APP_CLOSING_ITEM();
            item.closeMethod = notifier;
            item.order = order;

            AppClosingNotifiers.Add(item);
        }

        bool m_SelftDestruct = false;
        public delegate void CloseServiceLoopEvent();
        /// <summary>
        /// Add the top level Main loop close event handler to this event to do the final close of the main loop
        /// </summary>
        public CloseServiceLoopEvent OnCloseServiceLoop;

        public void SelfDestruct()
        {
            m_SelftDestruct = true;
            CloseApplication();
        }

        public void CloseApplication()
        {
            try
            {

                foreach (APP_CLOSING_ITEM notifier in AppClosingNotifiers)
                {
                    if (notifier.order == CLOSE_ORDER.FIRST)
                        notifier.closeMethod();
                }
            }
            catch { }

            Thread.Sleep(100);

            try
            {
                foreach (APP_CLOSING_ITEM notifier in AppClosingNotifiers)
                {
                    if (notifier.order == CLOSE_ORDER.MIDDLE)
                        notifier.closeMethod();
                }
            }
            catch { }

            Thread.Sleep(100);

            try
            {
                foreach (APP_CLOSING_ITEM notifier in AppClosingNotifiers)
                {
                    if (notifier.order == CLOSE_ORDER.LAST)
                        notifier.closeMethod();
                }
            }
            catch { }

            Thread.Sleep(2000);
            if ( m_SelftDestruct ) OnCloseServiceLoop();
                
        }





        ////////////////////////////////////
        /// 
        ///   Remote Server
        ///   
        ///

        public object CurrentlyLoggedInRemoteServer { set { lock (RemoteServerLock) { _RemoteServer = value; } } get { lock (RemoteServerLock) { return (_RemoteServer); } } }
        private object _RemoteServer;
        object RemoteServerLock;


        
        // /////////////////
        //
        //   User Interface for Movie Player Panels and Forms
        //
        //

        public object[]  MoviePlayerParentPanel;
        public object MoviePlayerParentForm;


        // /////////////////
        //
        //   Get Operating System
        //
        //

        public enum CURRENT_OS {VISTA, XP, WINDOWS7, UNSUPPORTED }

        public CURRENT_OS OperatingSystem
        {
            get { return GetOperatingSystem(); }
        }

        static CURRENT_OS GetOperatingSystem()
        {
            System.OperatingSystem osInfo = System.Environment.OSVersion;


                                /*
                    This example of OperatingSystem.Platform and OperatingSystem.Version
                    generates the following output.

                    Create several OperatingSystem objects and display their properties:

                    Platform: Win32NT         Version: 0.0
                    Platform: Win32S          Version: 3.5.8.13
                    Platform: Win32Windows    Version: 6.10
                    Platform: WinCE           Version: 5.25.5025
                    Platform: Win32NT         Version: 5.6.7.8 
                    Platform: Vista           Version: 6.0.6001
                    Platform: Windows 7       Version: 6.1.7600

                    */
            CURRENT_OS m_OS = CURRENT_OS.UNSUPPORTED;
            switch (osInfo.Platform)
            {

                case System.PlatformID.Win32Windows:
                    {
                        // Code to determine specific version of Windows 95, 
                        // Windows 98, Windows 98 Second Edition, or Windows Me.
                    }
                    break;
                case System.PlatformID.Win32NT:
                    {
                        if (osInfo.Version.Major == 5) m_OS = CURRENT_OS.XP; //"XP";
                        if (osInfo.Version.Major == 6)
                        {
                            if (osInfo.Version.Minor < 1)
                                m_OS = CURRENT_OS.VISTA;  //"VISTA";
                            else
                                m_OS = CURRENT_OS.WINDOWS7;
                        }
                    }
                    break;

            }
            return (m_OS);
        }


         // ///////////////////////////////////////////////////////

        //    Constructor

        /// <summary>
        /// AppData Constructor
        /// </summary>
        public APPLICATION_DATA()
        {
            try
            {
                AppClosingNotifiers = new List<APP_CLOSING_ITEM>();
                ThisComputerName = System.Net.Dns.GetHostEntry("LocalHost").HostName;
                RemoteServerLock = new object();

                HealthStatistics = new HEALTH_STATISTICS(this);

                SerialNumberLock = new object();
                globalFrameSerialNumber = 0;

                MoviePlayerParentPanel = new object[MAX_VIRTUAL_CHANNELS];

                LPRServiceControl = new LPR_SERVICE_CONTROL();
                LPRServiceControl.ConfigChangedServiceNeedsRestarting = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception on APPLICATION_DATA Startup: ex : " + ex.Message);
            }
        }

    }
}
