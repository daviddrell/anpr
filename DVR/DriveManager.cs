using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Management;
using ApplicationDataClass;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using ErrorLoggingLib;
using System.Globalization;
using System.Diagnostics;
using Utilities;

namespace DVRLib
{
    /// <summary>
    /// This class is only used if the user selects to use external USB drives for storage
    /// </summary>
    class DriveManager
    {
     

        //  /////////////////
        //
        //  design
        //
        //   at startup, count how many external drives are available  - there should only be one.
        //
        //   if one drive, select that drive and create the default storage directory
        //    then inform the DVR function that a drive has been found and is ready
        //

        //   setup a function to watch for changes in connected drives
        //   if a new drive is added, start the hotswap switch over, expect the previous drive to be disconnected

        //   if the currently selected drive is disconnected, notify the DVR the primary drive was lost

        //   if no drives are available, watch for new ones to be added, if one is added make it the primary.


        public DriveManager(APPLICATION_DATA appData, OnDriveChangeEvent callback, string defaultStorageDir )
        {
            try
            {
                m_AppData = appData;
                m_AppData.AddOnClosing(Stop, APPLICATION_DATA.CLOSE_ORDER.MIDDLE);
                m_Log = (ErrorLog)m_AppData.Logger;
                OnDriveChange = callback;
                m_DefaultStorageDirectory = defaultStorageDir;
                singleton = new object();

                m_HotSwapStatusString = new ThreadSafeQueue<string>(20);

                // make the list of current drives now that we are going into a active mode
                m_DriveListAtStartup = Environment.GetLogicalDrives();// to be used later if a drive change is detected


                // are we running as AnalystsWorkstation, and no repositories found? if so, then use user-config storage area instead
                bool runDriveCheckLoop = true;
                if ( ! m_AppData.RunninAsService)
                {
                    string[] drives = GetAllFirstEvidenceDrives();
                    if ( drives == null) drives = new string[0];
                    if (drives.Length < 1)
                    {
                     //   runDriveCheckLoop = false;

                        m_Log.Log("User App Path =  " + Application.UserAppDataPath, ErrorLog.LOG_TYPE.INFORMATIONAL);


                        m_CentralRepositoryDrive = Application.UserAppDataPath.Split(':')[0]+  ":\\";
                        m_SelectedDrive = m_CentralRepositoryDrive;

                        m_Log.Log("dm setting central to " + m_CentralRepositoryDrive, ErrorLog.LOG_TYPE.INFORMATIONAL);

                        RunDelayedDriveChangeNoticeThread();// run the new drive notify after this constructor completes execution
                    }
                }

                // look for drive changes..
               // SetupDriveWatcher();  // USB events do not always fire correctly when hubs are used on  certain computers
                if ( runDriveCheckLoop)
                    StartPollForDriveChangesThread();
            }
            catch (Exception ex)
            {
                m_Log.Log("DriveManager ex: " + ex.Message, ErrorLog.LOG_TYPE.FATAL);
            }

        
        }

        Thread m_DelayedDriveChangeNoticeThread;
        void RunDelayedDriveChangeNoticeThread()
        {
            
            m_DelayedDriveChangeNoticeThread = new Thread(DelayedDriveChangeNoticeLoop);
            m_DelayedDriveChangeNoticeThread.Start();
        }

        void DelayedDriveChangeNoticeLoop()
        {

            string[] sp = Application.UserAppDataPath.Split(':');
           

            Thread.Sleep(1000);

            m_Log.Log("on drive change " + Application.UserAppDataPath + "\\", ErrorLog.LOG_TYPE.INFORMATIONAL);

            OnDriveChange("C:\\", Application.UserAppDataPath+"\\", DRIVE_CHANGE_EVENT.NEWDRIVE);

        }

        public void Start()
        {
            // count how many valid First Evidence drives are available
            int FECount = 0;
            m_SelectedDrive = GetDrive(ref  FECount);
            if (FECount > 1)
            {
                OnDriveChange("", "", DRIVE_CHANGE_EVENT.TOO_MANY_DRIVES);
            }

            // make the list of current drives now that we are going into a active mode
            m_DriveListAtStartup = Environment.GetLogicalDrives();// to be used later if a drive change is detected

            if (m_SelectedDrive != null)
            {
                // does the default directory exist?
                CheckDVRDirectory(m_SelectedDrive + m_DefaultStorageDirectory);

                OnDriveChange(m_SelectedDrive, m_SelectedDrive, DRIVE_CHANGE_EVENT.NEWDRIVE);
            }
        }

        void CheckDVRDirectory(string directory)
        {
            try
            {
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
            }
            catch (Exception ex)
            {
                m_Log.Log("CheckDVRDirectory ex: " + ex.Message, ErrorLog.LOG_TYPE.FATAL);
            }

        }

        ThreadSafeQueue<string> m_HotSwapStatusString;
        ErrorLog m_Log;
        string m_DefaultStorageDirectory;
        void Stop(){m_Stop = true;}
        bool m_Stop = false;
        static object singleton = new object();
        string m_SelectedDrive;
        
        public string GetFieldDrive()
        { lock (singleton) { return m_FieldDrive; } }
        string m_FieldDrive = null; //only used by AnalystsWorkstation


        public enum DRIVE_CHANGE_EVENT { NEWDRIVE, LOST_PRIMARY_DRIVE, PAUSE, UNPAUSE, TOO_MANY_DRIVES}

        /// <summary>
        /// Connect handler to this notifier to handle detected changes in the external drive
        /// </summary>
        /// <param name="drive">string of the form "F:\\"</param>
        /// <param name="storagePath">string with the drive and the storage directory of the form: "F:\\DVRSTORAGE\\"</param>
        /// <param name="dEvent">event type enum, eiher gained  a drive or lost a drive</param>
        public delegate void OnDriveChangeEvent(string drive,string storagePath, DRIVE_CHANGE_EVENT dEvent);
        OnDriveChangeEvent OnDriveChange;

        string[] m_DriveListAtStartup;

        APPLICATION_DATA m_AppData;

        public string GetDriveHotSwapStatus()
        {
            return (m_HotSwapStatusString.Dequeue());
        }

        public void GetDriveStatus(ref bool driveReady, ref string driveName, ref double driveFreeSpace, ref double driveSize)
        {
            if (m_SelectedDrive != null)
            {
                driveName = m_SelectedDrive;

                driveReady = true;
                UInt64 Size = 0;

                driveFreeSpace = (double)GetDriveFreeSpace(m_SelectedDrive,ref Size);
                driveSize = (double)Size;
            }
            else
            {
                driveName =" ";
                driveReady = false;
                driveFreeSpace = 0.0;
                driveSize = 0.0;
            }
        }



        public double GetDriveSize(string drive)
        {
            try
            {
                SelectQuery query = new SelectQuery(
                "select name, Size from win32_logicaldisk where drivetype=3");
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);

                string[] s1 = drive.Split(':'); // get rid of the \\ at the end of the drive (F:\\)
                string letter = s1[0];
                letter += ":";

                foreach (ManagementObject mo in searcher.Get())
                {
                    string n = mo["name"].ToString();
                    if (n.Equals(letter))
                    {

                        UInt64 size = (UInt64)mo["Size"];

                        return ((double)size);
                    }
                }
            }
            catch { }

            return (0.0);
        }

        double GetDriveFreeSpace(string drive, ref UInt64 size )
        {
            try
            {
                SelectQuery query = new SelectQuery(
                "select name, Size, FreeSpace from win32_logicaldisk where drivetype=3");
                ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);

                string[] s1 = drive.Split(':'); // get rid of the \\ at the end of the drive (F:\\)
                string letter = s1[0];
                letter += ":";

                foreach (ManagementObject mo in searcher.Get())
                {
                    string n = mo["name"].ToString();
                    if (n.Equals(letter))
                    {
                        UInt64 space = (UInt64)mo["FreeSpace"];
                        size = (UInt64)mo["Size"];

                        return ((double)space);
                    }
                }
            }
            catch { }

            return (0.0);
        }

        /// <summary>
        /// Returns the current drive to use in a string of the form: "F:\\". Returns null if no external drive is found
        /// </summary>
        /// <returns></returns>
        public string GetDrive(ref int count)
        {
            lock (singleton)
            {
                return (m_CentralRepositoryDrive);
            }
           
        }

        string m_CentralRepositoryDrive;
        /// <summary>
        /// Return null if no central repository was found or the drive of the central repository
        /// </summary>
        public string CentralRepository
        { get { lock (singleton) { return(m_CentralRepositoryDrive);} } }

        void SetupDriveWatcher()
        {
            try
            {
                ManagementEventWatcher w = null;
                WqlEventQuery q;
                ManagementOperationObserver observer = new ManagementOperationObserver();
                // Bind to local machine
                ManagementScope scope = new ManagementScope("root\\CIMV2");
                scope.Options.EnablePrivileges = true; //sets required privilege

                q = new WqlEventQuery();
                q.EventClassName = "__InstanceOperationEvent";
                q.WithinInterval = new TimeSpan(0, 0, 3);
                q.Condition = @"TargetInstance ISA 'Win32_DiskDrive' ";

                //Console.WriteLine(q.QueryString);
                w = new ManagementEventWatcher(scope, q);

                w.EventArrived += new EventArrivedEventHandler(UsbEventArrived);
                w.Start();
                // Console.ReadLine(); // block main thread for test purposes
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }



        void StartPollForDriveChangesThread()
        {
            m_PollForDriveChanges = new Thread(PollForDriveChangesLoop);
            m_PollForDriveChanges.Start();
        }
        Thread m_PollForDriveChanges;

        void PollForDriveChangesLoop()
        {
            string[] drivesAfterChangeDetected;

            while (!m_Stop)
            {


                if (m_SwapDrivesThreadRunning)
                {
                    while (m_SwapDrivesThreadRunning && ! m_Stop)
                    {
                        Thread.Sleep(1000);
                    }
                }

                // Store in a string array the new list
                drivesAfterChangeDetected = Environment.GetLogicalDrives();

                int changeCount = (int)drivesAfterChangeDetected.Count() - (int)m_DriveListAtStartup.Count();

                if (m_AppData.RunninAsService)
                {
                    ProcessNewDriveConfigAsLPRService(changeCount, drivesAfterChangeDetected);
                }
                else
                {
                    // running as AnalystsWorkstation

                    ProcessNewDriveConfigAsAnalystsWorkstation();
                }

                m_DriveListAtStartup = drivesAfterChangeDetected;

                Thread.Sleep(1000);
            }

        }

        bool IsCentralRepository(string drive)
        {
            if (File.Exists(drive + "firstevidencedrive.txt"))
            {
                string[] contents = File.ReadAllLines(drive + "firstevidencedrive.txt");
                if (contents.Count() > 0)
                {
                    if (contents[0].Contains("central"))
                    {
                        lock (singleton)
                        {
                            return (true);
                        }
                    }
                }
            }
            return (false);
        }

      
        void ProcessNewDriveConfigAsAnalystsWorkstation()
        {

            // we allow two drives to be connected, a central repository and a field drive to import data from ( and merge the data
            //  into the central repository)

            string centralDriveBeforePoll = m_CentralRepositoryDrive;

            string centralDrive = null;
            string fieldDrive = null;
            DRIVE_CHANGE_EVENT e = new DRIVE_CHANGE_EVENT();

            string[] drives = GetAllFirstEvidenceDrives();

            foreach (string drive in drives)
            {
                if (IsCentralRepository(drive) && centralDrive == null) // stop at the first one found, if more than one exists we have problems
                {
                    centralDrive = drive;
                }
                else if (! IsCentralRepository(drive) && fieldDrive == null)// stop at the first one found, if more than one exists we have problems
                {
                    fieldDrive = drive;
                }
            }

            // do we have a Central repository attached?
            lock (singleton)
            {

                if (centralDrive != m_CentralRepositoryDrive)
                {
                    
                    if (m_CentralRepositoryDrive != null & centralDrive == null)
                    {
                        // we lost a central repos drive
                        e = DRIVE_CHANGE_EVENT.LOST_PRIMARY_DRIVE;
                         { m_CentralRepositoryDrive = centralDrive; }

                    }
                    else if (m_CentralRepositoryDrive == null & centralDrive != null)
                    {
                        // we gained a centrl repo drive
                        e = DRIVE_CHANGE_EVENT.NEWDRIVE;
                         { m_CentralRepositoryDrive = centralDrive; }

                    }
                    else
                    {
                        // we changed from one central to another
                        e = DRIVE_CHANGE_EVENT.NEWDRIVE;
                         { m_CentralRepositoryDrive = centralDrive; }

                    }
                }

                // do we have a field drive attached?

                if (fieldDrive != m_FieldDrive)
                {
                    if (m_FieldDrive != null & fieldDrive == null)
                    {
                        // we lost a central repos drive
                        e = DRIVE_CHANGE_EVENT.LOST_PRIMARY_DRIVE;
                         { m_FieldDrive = fieldDrive; }

                        // if we do not have a central repos, then the field drive becomes the primary drive
                        
                        {
                            if (m_CentralRepositoryDrive == null)
                            {
                                m_CentralRepositoryDrive = m_FieldDrive;
                                m_FieldDrive = null;
                            }
                        }
                    }
                    else if (m_FieldDrive == null & fieldDrive != null)
                    {
                        // we gained a centrl repo drive
                         { m_FieldDrive = fieldDrive; }

                        // if we do not have a central repos, then the field drive becomes the primary drive
                       
                        {
                            if (m_CentralRepositoryDrive == null)
                            {
                                m_CentralRepositoryDrive = m_FieldDrive;
                                m_FieldDrive = null;
                            }
                        }
                    }
                    else
                    {
                        // we changed from one central to another
                        { m_FieldDrive = fieldDrive; }

                        // if we do not have a central repos, then the field drive becomes the primary drive
                       
                        {
                            if (m_CentralRepositoryDrive == null)
                            {
                                m_CentralRepositoryDrive = m_FieldDrive;
                                m_FieldDrive = null;
                            }
                        }
                    }
                }


                // do we have No drives at all?
                if (m_CentralRepositoryDrive == null)
                {
                    // then use the default user data path for storage
                    m_CentralRepositoryDrive =  Application.UserAppDataPath + "\\";
                }


                m_SelectedDrive = m_CentralRepositoryDrive;


                if (centralDriveBeforePoll != m_CentralRepositoryDrive)
                {
                    if (m_CentralRepositoryDrive == null) e = DRIVE_CHANGE_EVENT.LOST_PRIMARY_DRIVE;
                    else e = DRIVE_CHANGE_EVENT.NEWDRIVE;

                    OnDriveChange(m_CentralRepositoryDrive, m_CentralRepositoryDrive, e);
                }
            }
          
        }





        void ProcessNewDriveConfigAsLPRService(int changeCount, string[] drivesAfterChangeDetected)
        {
            string newDrive = null;

            try
            {
                // start up case, we have not found a drive yet, but it may be already connected
                if (m_SelectedDrive == null)
                {
                    if (checkForValidFirstEvidenceDrive(drivesAfterChangeDetected, out newDrive))
                    {
                        m_Log.Log("adding a primary drive: " + newDrive, ErrorLog.LOG_TYPE.INFORMATIONAL);
                        m_HotSwapStatusString.Enqueue("adding a primary drive: " + newDrive);


                        CheckDVRDirectory(newDrive + m_DefaultStorageDirectory);// creates the directory if its not there

                        m_SelectedDrive = newDrive;

                     
                           
                      //  CheckForCentralRepository();
                     
                        OnDriveChange(m_SelectedDrive, m_SelectedDrive, DRIVE_CHANGE_EVENT.NEWDRIVE);
                        return;
                    }
                }

                // we had a drive but it got disconnected
                if (m_SelectedDrive != null)
                {
                    if (CheckForLostPrimaryDisk(drivesAfterChangeDetected))
                    {
                        m_SelectedDrive = null;
                        m_Log.Log("Lost Primary External Drive, pausing DVR", ErrorLog.LOG_TYPE.FATAL);
                        m_HotSwapStatusString.Enqueue("Lost Primary External Drive, pausing DVR");
                     //   CheckForCentralRepository();

                        OnDriveChange(null, null, DRIVE_CHANGE_EVENT.LOST_PRIMARY_DRIVE);
                        return;

                    }
                }

                //  a new drive got connected, but it could be a second drive(hotswap) or re-connect a previously connected primary
                if (checkForNewValidFirstEvidenceDrive(drivesAfterChangeDetected, out newDrive))
                {
                    // we have a new drive
                    m_Log.Log(" detected new drive: " + newDrive, ErrorLog.LOG_TYPE.INFORMATIONAL);
                    m_HotSwapStatusString.Enqueue("Detected new drive: " + newDrive);

                    // are we adding a second or adding a first drive?
                    if (m_SelectedDrive == null)
                    {
                        // this is a primary drive being re-connected

                        m_Log.Log("adding a primary drive: " + newDrive, ErrorLog.LOG_TYPE.INFORMATIONAL);
                        m_HotSwapStatusString.Enqueue("adding a primary drive: " + newDrive);


                        CheckDVRDirectory(newDrive + m_DefaultStorageDirectory);
                        m_SelectedDrive = newDrive;
                     //   CheckForCentralRepository();
                        OnDriveChange(m_SelectedDrive, m_SelectedDrive, DRIVE_CHANGE_EVENT.NEWDRIVE);
                        return;
                    }
                    else
                    {      /// this is a second drive, do a hot swap
                        m_Log.Log("starting hotswap " + newDrive, ErrorLog.LOG_TYPE.INFORMATIONAL);
                        m_HotSwapStatusString.Enqueue("starting hotswap " + newDrive);

                        m_StopSwap = false;
                        m_newDrive = newDrive;
                        SwapDrives();
                        return;
                    }
                }
            }
            catch (Exception ex) { m_Log.Trace(ex, ErrorLog.LOG_TYPE.INFORMATIONAL); }

          
        }


        // I originally got the Usb Event and checked for changes in the event handler, but some times no changes appeared in the drive
        // list immediately after the Usb event. So the event now kicks off a polling thread.

        void UsbEventArrived(object sender, EventArrivedEventArgs e)
        {
            m_Log.Log("received Usb Disk Change Event", ErrorLog.LOG_TYPE.FATAL);
            m_HotSwapStatusString.Enqueue("received Usb Disk Change Event");
            StartPollForDriveChangesThread();
        }


       

        string[] GetAllFirstEvidenceDrives()
        {
            List<string> FEDrives = new List<string>();

            foreach (string drive in m_DriveListAtStartup)
            {
              
                string upper = drive.ToUpper();

                if (File.Exists(drive+ "firstevidencedrive.txt"))
                {
                    FEDrives.Add(drive);
                }
               
            }
            return (FEDrives.ToArray());
        }


        bool checkForValidFirstEvidenceDrive(string[] drivesAfterChangeDetected, out string newDrive)
        {
            newDrive = null;

            // find the new one
            foreach (string possibleNewDrive in drivesAfterChangeDetected)
            {
                string upper = possibleNewDrive.ToUpper();

                if (File.Exists(possibleNewDrive + "firstevidencedrive.txt"))
                {
                    newDrive = possibleNewDrive;
                }
                
            }
            if (newDrive != null) return true;// found a new drive that is a First Evidence drive
            else return (false);
        }


        bool checkForNewValidFirstEvidenceDrive(string[] drivesAfterChangeDetected, out string newDrive)
        {
            newDrive = null;

            // find the new one
            foreach (string possibleNewDrive in drivesAfterChangeDetected)
            {
                bool driveIsInList = false;
                foreach (string existingdrive in m_DriveListAtStartup)
                {
                    if (existingdrive.Contains(possibleNewDrive))
                        driveIsInList = true;
                }
                if (!driveIsInList)
                {
                    {
                        if (File.Exists(possibleNewDrive + "firstevidencedrive.txt"))
                            newDrive = possibleNewDrive;
                    }
                }
            }
            if (newDrive != null) return true;// found a new drive that is a First Evidence drive
            else return (false);
        }

        bool CheckForLostPrimaryDisk(string[] drivesAfterChangeDetected)
        {
            try
            {
                bool matchFound = false;
                foreach (string drive in drivesAfterChangeDetected)
                {
                    if (m_SelectedDrive == null) continue;
                    if (drive.Contains(m_SelectedDrive))
                    {
                        matchFound = true;// we still have the primary drive
                        break;
                    }
                }
                if (!matchFound) return (true);// we lost it
                else return false; // we did not loose it
            }
            catch { return true; }
        }

    
        void SwapDrives()
        {
          

            Thread.Sleep(250);

            m_SwapDrivesThread = new Thread(SwapDrivesLoop);
            m_SwapDrivesThread.Start();
        }

        string m_newDrive;
        Thread m_SwapDrivesThread;
        bool m_StopSwap = false;
        bool m_SwapDrivesThreadRunning = false;
        void SwapDrivesLoop()
        {
            while (!m_Stop && !m_StopSwap)
            {
                m_SwapDrivesThreadRunning = true;
                try
                {
                    m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.System.System_Hotswap].StatString.SetValue = "Starting HotSwap";

                    // pause the DVR
                    OnDriveChange(null, null, DRIVE_CHANGE_EVENT.PAUSE);
                 
                    Thread.Sleep(10);

                    CheckDVRDirectory(m_newDrive);

                    // switch to the new drive
                    m_SelectedDrive = m_newDrive;
                    m_newDrive = null;
                  
                 
                    OnDriveChange(m_SelectedDrive, m_SelectedDrive, DRIVE_CHANGE_EVENT.NEWDRIVE);
                    OnDriveChange(m_SelectedDrive, m_SelectedDrive, DRIVE_CHANGE_EVENT.UNPAUSE);


                    m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.System.System_Hotswap].StatString.SetValue = "Completed HotSwap";


                    break;

                }
                catch (Exception ex)
                {
                    m_Log.Log("SwapDrivesLoop ex: " + ex.Message, ErrorLog.LOG_TYPE.FATAL);
                    m_HotSwapStatusString.Enqueue("SwapDrivesLoop ex: " + ex.Message);

                }

                Thread.Sleep(1);
            }

         

            m_SwapDrivesThreadRunning = false;

            
            
        }
        
   

        DriveInfo GetDriveInfo(string drive)
        {
           
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            foreach (DriveInfo di in allDrives)
            {
                if (di.Name.Contains(drive))
                    return (di);
            }

            return (default(DriveInfo));
        }

        string DriveInfoString(DriveInfo di)
        {
            double driveSize = (double)di.TotalSize;
            driveSize /= 1000000000;

            double freeSpace = (double)di.AvailableFreeSpace;
            freeSpace /= 1000000000;

            string s = "Drive Name: " + di.Name + "\r\n" +
                
               "VolumeLabel: " + di.VolumeLabel + "\r\n" +
               //  "TotalSize: " + di.TotalSize.ToString("#,#", CultureInfo.InvariantCulture) + "\r\n" +
                
               "TotalSize: " + driveSize.ToString("#,#", CultureInfo.InvariantCulture) + " GB\r\n" +

               // "TotalFreeSpace: " + di.TotalFreeSpace.ToString("#,#", CultureInfo.InvariantCulture) + "\r\n" +
               "TotalFreeSpace: " + freeSpace.ToString("#,#", CultureInfo.InvariantCulture) + " GB\r\n" +

               "DriveType: " + di.DriveType + "\r\n";

            return (s);
        }
    }

 

}


