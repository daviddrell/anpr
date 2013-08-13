using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ApplicationDataClass;
using FrameGeneratorLib;
using System.Threading;
using Utilities;
using System.IO;
using ErrorLoggingLib;
using System.Drawing;
using UserSettingsLib;
using System.Windows.Forms;
using PathsLib;
using LPREngineLib;

namespace DVRLib
{
    public partial class DVR
    {
        // constructor
        public DVR(APPLICATION_DATA appData)
        {
            try
            {
                m_AppData = appData;
                m_AppData.AddOnClosing(Stop, APPLICATION_DATA.CLOSE_ORDER.MIDDLE);
                m_Log = (ErrorLog)m_AppData.Logger;

                PauseFlag = new PAUSE_FLAG();

                PauseFlag.Pause = true;

                FileAccessControl = new FILE_SYSTEM_ACCESS(PauseFlag, m_AppData);

                try
                {
                    Paths = new PATHS(m_AppData.ThisComputerName, true, m_AppData);

                    m_AppData.PathManager = (object)Paths;
                }
                catch (Exception ex) { m_Log.Trace(ex, ErrorLog.LOG_TYPE.FATAL); }

                m_EventLogFile = new EventLogFiles.EventLogFiles(m_AppData);

                //   PRE MOTION BUFFER LENGHT

                m_NumberOfFramesToPrePostBuffer =    30;   // this many frames before and after moton event, if too small, files may be erased before motion is detected because of processing lag

                ////////



                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.DVR.DVR_DriveName].StatString.RegisterForUse(true);
                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.DVR.DVR_DriveReady].boolean.RegisterForUse(true);
                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.DVR.DVR_FreeSpace].SnapshotDouble.RegisterForUse(true);
                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.DVR.DVR_UsedSpace].SnapshotDouble.RegisterForUse(true);
                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.DVR.DVR_DriveHotSwap].StatString.RegisterForUse(true);

                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.System.System_Drive].StatString.SetValue = "No drive ";

                m_FrameGenerator = (FrameGenerator)m_AppData.FrameGenerator;

             
                m_DriveManager = new DriveManager(m_AppData, OnExternalDriveChange, Paths.StorageDir);

                // alway use external storage

                if ( m_AppData.RunninAsService)
                {
                    
                    int count = 0;
                    Paths.Drive = m_DriveManager.GetDrive(ref count);
                    PauseFlag.DriveExists = true;
                    if (count > 1)
                    {
                        PauseFlag.Pause = true;
                        m_Log.Log("Too many external drives on start", ErrorLog.LOG_TYPE.FATAL);
                        //m_MessageBoxMessage = "Too many external drives on start. Please connect only one external drive and re-start the LPR Service.";
                        //RunMessageBoxDialog();

                    }
                    else if (Paths.Drive == null)
                    {
                        PauseFlag.Pause = true;

                        m_Log.Log("External Drive not found", ErrorLog.LOG_TYPE.FATAL);
                      //  m_MessageBoxMessage = "External drive not found. Please connect one external drive";
                        PauseFlag.DriveExists = false;
                       // RunMessageBoxDialog();
                    }

                }
               


                m_ConsumerID = m_FrameGenerator.GetNewConsumerID();

                m_NewFrameQ = new ThreadSafeQueue<FRAME>(240, "QueueOverruns_DVR_NewFrameQ", m_AppData);
                m_MotionDetectedQ = new ThreadSafeQueue<FRAME>(240, "QueueOverruns_DVR_MotionDetectedQ", m_AppData);
                m_DirectyToStorageQ = new ThreadSafeQueue<FRAME>(240, "QueueOverruns_DVR_DirectyToStorageQ", m_AppData);
                m_NewLPRRecordQ = new ThreadSafeQueue<FRAME>(m_LPRRecordQueLen, "QueueOverruns_DVR_NewLPRRecordQ", m_AppData);
                m_TempFileList = new ThreadSafeList<TEMPFILES>(120);


                m_NumSourceChannels = (m_AppData.RunninAsService) ? m_AppData.MAX_PHYSICAL_CHANNELS : m_AppData.MAX_VIRTUAL_CHANNELS;

                m_PreMotionRecords = new PRE_MOTION_RECORDS[m_NumSourceChannels];

                m_DVRLoopThread = new Thread(DVRLoop);
           
                m_ReportDVRStats = new Thread(ReportDVRStatusLoop);

                m_TempFileCleanUpThread = new Thread(TempFilesCleanUpLoop);

                m_MaintainFileSystemSizeLimit = new Thread(MaintainFileSystemSizeLimitLoop);

          
               
            }
            catch (Exception ex) { m_Log.Trace(ex, ErrorLog.LOG_TYPE.FATAL); }
        }

        LPREngine m_LPREngine;
        public PATHS Paths;
        EventLogFiles.EventLogFiles m_EventLogFile;
        int m_NumberOfFramesToPrePostBuffer;
        Thread m_ReportDVRStats;
        Thread m_MaintainFileSystemSizeLimit;
        bool m_DVRReadyStatus = false;
        DriveManager m_DriveManager;
        APPLICATION_DATA m_AppData;
        FrameGenerator m_FrameGenerator;
        int m_ConsumerID;
        int m_NumSourceChannels;
        int m_LPRRecordQueLen = 480;
        ErrorLog m_Log;

        Thread m_DVRLoopThread;
      


        public FILE_SYSTEM_ACCESS FileAccessControl;
        PAUSE_FLAG PauseFlag;


        public void StartRegistration()
        {

            try
            {


                for (int c = 0; c < m_NumSourceChannels; c++)
                {
                    m_PreMotionRecords[c] = new PRE_MOTION_RECORDS();
                    m_PreMotionRecords[c].MotionDetectedMovingFilesInProcess = false;
                    m_PreMotionRecords[c].PreMotionChannelSubDirCreated = false;
                    m_PreMotionRecords[c].PendingMotionDetectionQ = new ThreadSafeQueue<string>(m_NumberOfFramesToPrePostBuffer + 10);

                    m_FrameGenerator.RegisterToConsumeMotionDetectedFrames(m_ConsumerID, c, OnMotionDetection);
                    m_FrameGenerator.RegisterToConsumeChannel(m_ConsumerID, c, OnNewFrame);
                }

                m_LPREngine = (LPREngine)m_AppData.LPREngine;
                m_LPREngine.OnNewFilteredPlateGroupEvent += OnLPRNewRecord;
            }
            catch (Exception ex) 
            { 
                m_Log.Trace(ex, ErrorLog.LOG_TYPE.FATAL); 
            }

        }

        public void StartThreads()
        {
            Paths.StartThreads();

            m_DVRLoopThread.Start();
  
            m_ReportDVRStats.Start();
            m_TempFileCleanUpThread.Start();
            m_DriveManager.Start();
            m_MaintainFileSystemSizeLimit.Start();
            m_DVRReadyStatus = true;
        }

        int LoopPeriod = 30000;
        void MaintainFileSystemSizeLimitLoop()
        {
            while (!m_Stop)
            {
                int count = LoopPeriod;
                while ( count-- > 0 && ! m_Stop)
                    Thread.Sleep(1);

                if ( ! Paths.Enabled) continue;

                double driveSize = m_DriveManager.GetDriveSize(Paths.Drive);

                if (driveSize == 0) continue; // the drive was disconnected

                double limit = driveSize * 0.90;
      

                // are we over the limit?

                if (Paths.AccumulatedSize >= limit)
                {
                    // need to delete some files
                    DeleteOldestFiles(Paths.AccumulatedSize - limit);
                }

               
            }

        }

        void DeleteOldestFiles(double totalSizeToDelete)
        {
            double deleted = 0.0;

            while (deleted < totalSizeToDelete)
            {
                string oldestDay = Paths.GetOldestDayDir();
                if (oldestDay == null)
                {
                    m_Log.Log("DeleteOldestFiles oldestDay was null", ErrorLog.LOG_TYPE.FATAL);
                    return;
                }
                deleted += FileAccessControl.DeleteDirectoryAndContents(oldestDay);
                m_Log.Log("DeleteOldestFiles deleted day: "+ oldestDay , ErrorLog.LOG_TYPE.INFORMATIONAL);
            }
            m_Log.Log("DeleteOldestFiles deleted total size: " + deleted.ToString(), ErrorLog.LOG_TYPE.INFORMATIONAL);
        }

        void OnLPRNewRecord(FRAME frame)
        {
            m_NewLPRRecordQ.Enqueue(frame);

            if (m_NewLPRRecordQ.Count > m_LPRRecordQueLen / 2)
                m_AppData.DVRStoringLPRRecordsGettingBehind = true;
            else
                m_AppData.DVRStoringLPRRecordsGettingBehind = false;
        }

        public bool GetDVRReady
        { get { return m_DVRReadyStatus; } }
    

        public double DriveFreeSpace { get { return m_CurrentDriveFreeSpace; } }
        double m_CurrentDriveFreeSpace;

        void ReportDVRStatusLoop()
        {
            TimeSpan updatePeriod = new TimeSpan(0, 0, 15);
            DateTime lastTime = DateTime.Now;

            while (!m_Stop)
            {
                try
                {
                    if (DateTime.Now.Subtract(lastTime).CompareTo(updatePeriod) > 0)
                    {
                        lastTime = DateTime.Now;

                        string driveName = null;
                        bool driveReady = false;
                        double driveFreeSpace = 0.0;
                        double size = 0.0;

                        if (m_DriveManager != null)
                        {
                            m_DriveManager.GetDriveStatus(ref driveReady, ref driveName, ref driveFreeSpace, ref size);
                            m_CurrentDriveFreeSpace = driveFreeSpace;

                            m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.DVR.DVR_DriveName].StatString.SetValue = (Paths.StoragePath == null ? " " : Paths.StoragePath);
                            m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.DVR.DVR_DriveReady].boolean.SetValue = Paths.Enabled;
                            m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.DVR.DVR_FreeSpace].SnapshotDouble.SetValue = driveFreeSpace;
                            m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.DVR.DVR_UsedSpace].SnapshotDouble.SetValue = Paths.AccumulatedSize;
                        }
                        else
                        {
                            m_CurrentDriveFreeSpace = 0;
                            m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.DVR.DVR_DriveName].StatString.SetValue = "No device storage connected";
                            m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.DVR.DVR_DriveReady].boolean.SetValue = false;
                            m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.DVR.DVR_FreeSpace].SnapshotDouble.SetValue = 0.0;
                            m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.DVR.DVR_UsedSpace].SnapshotDouble.SetValue = 0.0;
                        }
                    }
                    m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.DVR.DVR_DriveHotSwap].StatString.SetValue = m_DriveManager.GetDriveHotSwapStatus();
                    Thread.Sleep(250);
                }
                catch (Exception ex) { m_Log.Trace(ex, ErrorLog.LOG_TYPE.FATAL); }

            }
        }


        public void UserSpecifiedDriveHasBeenSet()
        {
            OnExternalDriveChange(Paths.Drive, Paths.StoragePath, DriveManager.DRIVE_CHANGE_EVENT.NEWDRIVE);
        }

        void OnExternalDriveChange(string drive, string storagePath, DriveManager.DRIVE_CHANGE_EVENT dEvent)
        {
            PauseFlag.Pause = true;

            try
            {
                switch (dEvent)
                {
                    case DriveManager.DRIVE_CHANGE_EVENT.PAUSE:
                        PauseFlag.Pause = true;
                        Paths.Enabled = false;
                        break;

                    case DriveManager.DRIVE_CHANGE_EVENT.UNPAUSE:
                        PauseFlag.Pause = false;
                        Paths.Enabled = true;
                        break;

                    case DriveManager.DRIVE_CHANGE_EVENT.NEWDRIVE:
                        
                        Paths.Drive = storagePath;

                        m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.System.System_Drive].StatString.SetValue = "Have drive " + Paths.Drive;

                        // start the accumulated file size at the intial value of the disk
                        bool driveReady=false;
                        string name=null;
                        double freeSpace=0.0;
                        double size=0.0;

                        m_DriveManager.GetDriveStatus (ref driveReady, ref name, ref freeSpace, ref size);
                        double initialUsedSpace = size - freeSpace;
                        Paths.AccumulatedSize = initialUsedSpace;

                        PauseFlag.DriveExists = true;
                        PauseFlag.OverridePause = true;
                        CreatePreMotionBufferDirectory();
                        PauseFlag.OverridePause = false;
                        PauseFlag.Pause = false;

                        m_Log.Log("Found new drive, unpausing DVR", ErrorLog.LOG_TYPE.INFORMATIONAL);
                        break;

                    case DriveManager.DRIVE_CHANGE_EVENT.LOST_PRIMARY_DRIVE:
                        PauseFlag.DriveExists = false;
                        PauseFlag.Pause = true;
                        Paths.Drive = null;

                        m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.System.System_Drive].StatString.SetValue = "No drive ";

                        m_Log.Log("DVR Lost primary drive", ErrorLog.LOG_TYPE.FATAL);
                        break;

                    case DriveManager.DRIVE_CHANGE_EVENT.TOO_MANY_DRIVES:
                        m_Log.Log("Too many external drives on start", ErrorLog.LOG_TYPE.FATAL);
                     //   m_MessageBoxMessage = "Too many external drives on start. Please connect only one external drive and re-start the LPR Service.";
                     //   RunMessageBoxDialog();
                        m_DVRReadyStatus = false;
                        break;

                }
            }
            catch (Exception ex) { m_Log.Trace(ex, ErrorLog.LOG_TYPE.FATAL); }

        }

        // /////////////////////////////////////////////////
        // /////////////////////////////////////////////////
        //
        //
        //  design notes:
        //
        //    m_NumberOfFramesToPrePostBuffer pre-motion images are kept around for each channel in a directory structure F:\DVRSTORAGE\PREMOTION\sourceName\2009_07_13_03_47_30_5363.jpg
        //
        //    when a motion detected frame comes, inpsect the channel number of that frame and then copy out the the pre-motion frames associated with that source to their final resting place
        //
        //    

        ThreadSafeQueue<FRAME> m_NewFrameQ;// all new frames go here to wait for the DVR thread to put them on disk in the pre-motion buffer directory

        ThreadSafeQueue<FRAME> m_MotionDetectedQ; // in-comming frames from the motion detector, causes the DVR thread to trigger a copy of the pre-motion buffer to the storage area
        ThreadSafeQueue<FRAME> m_DirectyToStorageQ; // DVR moves from this Q to storage after a motion detection event comming from the m_MotionDetectionQ
        ThreadSafeQueue<FRAME> m_NewLPRRecordQ; // holds new LPR records until the DVR thread can write them to disk

        bool m_Stop = false;
        void Stop()
        {
            PauseFlag.Pause = true;
            PauseFlag.Stop = true;

            Thread.Sleep(100);

            m_Stop = true;
        }

        int m_MotionFrameCount = -1;

        void OnNewFrame(FRAME frame)
        {
            if (m_AppData.DVRMode != APPLICATION_DATA.DVR_MODE.STORE_ON_MOTION) return; // if not store on motion, we only get frames from LPR to store


            // dont process stuff on this thread

            if (m_MotionFrameCount-- > 0)
            {
                // record it, we had motion
                m_DirectyToStorageQ.Enqueue(frame);
            }
            else
            {
                m_NewFrameQ.Enqueue(frame);// goes to the pre-motion buffer
            }
        }


        // this event is calld from the FrameGenerator, which does the motion detection

        void OnMotionDetection(FRAME frame)
        {
            if (m_AppData.DVRMode != APPLICATION_DATA.DVR_MODE.STORE_ON_MOTION) return;// if not store on motion, we only get frames from LPR to store

            // dont process stuff on this thread

            frame.JpegFileRelativePath = Paths.GetJpegRelateivePath(frame);

            // if we are still in within the post-motion copy-out time from the previous motion event, do not trigger a new copy-out of the pre-motion buffer
            if (m_MotionFrameCount < 0)
            {
                m_MotionDetectedQ.Enqueue(frame);  // send this frame to the DVR thread so it can use this frame's time/channel reference to trigger a copy out of the pre-motion buffer
            }

            // reset the post-motion recording frame counter
            m_MotionFrameCount = m_NumberOfFramesToPrePostBuffer;
        }

        void CreateLogDirectory(string dir)
        {
            if (!FileAccessControl.DirectoryExists((string)dir))
            {
                char[] seperator = { '\\', '\\' };

                string[] branches = dir.Split(seperator);
                string path = null;
                for (int i = 0; i < branches.Count(); i++)
                {
                    if (branches[i].Length < 1) continue;
                    path += (branches[i] + "\\");

                    if (!FileAccessControl.DirectoryExists(path))
                        FileAccessControl.CreateDirectory(path);

                }
            }
        }

        void CreatePreMotionBufferDirectory()
        {
            string preMotionDirectory = null;

            string[] sources = m_FrameGenerator.GetChannelList();

            m_Log.Log("sources = " + sources[0].Length.ToString(), ErrorLog.LOG_TYPE.INFORMATIONAL);

            if (sources == null) return;

            foreach (string source in sources)
            {


                preMotionDirectory = Paths.PreMotionPath + "\\" + source;

                m_Log.Log("preMotionDirectory = " + preMotionDirectory, ErrorLog.LOG_TYPE.INFORMATIONAL);

                if (FileAccessControl.DirectoryExists(preMotionDirectory))
                {
                    // remove its contents and delete it  - its left over from a previous run
                    FileAccessControl.DeleteDirectoryAndContents(preMotionDirectory);
                }

                // create it fresh and empty
                FileAccessControl.CreateDirectory(preMotionDirectory);
            }

        }


        void DVRLoop()
        {
            FRAME frame = null;
            int count = 10;

            while (!m_Stop)
            {
                

                // if motion detected, send to the copy-out thread which copies the pre-motion buffer to storage


                frame = m_MotionDetectedQ.Dequeue();
                while (frame != null && count-- > 0)
                {

                    WriteMotionEventAndPushPreMotionBuffer(frame);// store it on disk

                    frame = m_MotionDetectedQ.Dequeue(); // get the next one from the queue
                }
                


                // if we are in the post-motion copy-out time, copy this frame to storage
                count = 10;
                frame = m_DirectyToStorageQ.Dequeue();
                while (frame != null && count-- > 0)
                {
                    WriteToStorage(frame);

                    frame = m_DirectyToStorageQ.Dequeue();
                }

                try
                {


                    // write out LPR events to the event log
                    count = 10;
                    frame = m_NewLPRRecordQ.Dequeue();
                    while (frame != null && count-- > 0)
                    {
                        frame.JpegFileRelativePath = Paths.GetJpegRelateivePath(frame);

                        // if ( ! m_AppData.DVR_StoreToUserSpecifiedFolder) ===>> if user is in Workstation and pushing output images to a special folder, do not 
                        //  write the LPR string results to the event log

                        if ( ! m_AppData.DVR_StoreToUserSpecifiedFolder)
                        {

                            //
                            // write LPR results to the Event Log
                            //

                            if (!PauseFlag.Pause)
                            {
                                EventLogFiles.EventLogFiles.EVENT_TO_WRITE data = m_EventLogFile.WriteLPREvent(frame);
                                CreateLogDirectory(data.directory);
                                FileAccessControl.AppendAllText(data.file, data.line);
                            }
                        }

                        if (m_AppData.DVRMode == APPLICATION_DATA.DVR_MODE.STORE_ON_PLATE_FOUND)
                        {
                            WriteToStorage(frame);
                        }

                        frame = m_NewLPRRecordQ.Dequeue();
                    }

                }
                catch (Exception ex) { m_Log.Trace(ex, ErrorLog.LOG_TYPE.FATAL); }


                try
                {
                    // all new frames go into the pre-motion buffer
                    count = 10;
                    frame = m_NewFrameQ.Dequeue();
                    while (frame != null && count-- > 0)
                    {
                        PendingMotionDetection(frame);
                        frame = m_NewFrameQ.Dequeue();
                    }
                }
                catch (Exception ex) { m_Log.Trace(ex, ErrorLog.LOG_TYPE.FATAL); }



                Thread.Sleep(1);
            }


        }




        /// <summary>
        /// Used by the hand-editor in Analysts workstation to by-pass the automatic processing chain of motion-detection based storage
        /// </summary>
        /// <param name="frame"></param>
        public void SendFrameDirectlyToStorage(FRAME frame)
        {
            WriteToStorage(frame);
        }


        void WriteMotionEventAndPushPreMotionBuffer(FRAME frame)
        {
            //
            // write the motion event to the Event Log
            //
            EventLogFiles.EventLogFiles.EVENT_TO_WRITE data = m_EventLogFile.WriteMotoinEvent(frame.PSSName, frame.TimeStamp, frame.SourceName, frame.GPSPosition, frame.JpegFileRelativePath);

            CreateLogDirectory(data.directory);
            FileAccessControl.AppendAllText(data.file, data.line);

          

            string sourceDir = Paths.PreMotionPath + "\\" + frame.SourceName;
            if (sourceDir == null) return;

            //
            //   are the frames in the PRE-MOTION dir to copy out to storage ?
            //

            // based on time stamp and channel, copy out the pre-motion files to the storage location

            string[] sourceFiles = FileAccessControl.GetFiles(sourceDir);

            if (sourceFiles == null)return;

            m_PreMotionRecords[frame.SourceChannel].MotionDetectedMovingFilesInProcess = true;

            foreach (string file in sourceFiles)
            {
                MoveFileToDVRStorage(file,  frame.SourceName);
            }

            m_PreMotionRecords[frame.SourceChannel].PendingMotionDetectionQ.Clear();

            m_PreMotionRecords[frame.SourceChannel].MotionDetectedMovingFilesInProcess = false ;

        }

        void WriteToStorage(FRAME frame)
        {
            if (!m_AppData.DVR_StoreToUserSpecifiedFolder)
            {
                if (PauseFlag.Pause) return;
            }
            string destDir = null;
            string destPath=null;

            try
            {
                if (!m_AppData.DVR_StoreToUserSpecifiedFolder)
                {

                    destPath = Paths.GetFrameStoragePath(frame.JpegFileNameOnly, frame.SourceName, out destDir);

                    if (!FileAccessControl.DirectoryExists(destDir))
                    {
                        char[] seperator = { '\\', '\\' };

                        string[] branches = destDir.Split(seperator);
                        string path = null;
                        for (int i = 0; i < branches.Count(); i++)
                        {
                            if (branches[i].Length < 1) continue;
                            path += (branches[i] + "\\");

                            if (!FileAccessControl.DirectoryExists(path))
                                FileAccessControl.CreateDirectory(path);

                        }
                    }
                }
                else
                {
                    // else this is the Analysts workstation app and the user has selected an alternative storage path
                    PauseFlag.Pause = false;
                    PauseFlag.DriveExists = true;

                    destDir = m_AppData.DVR_UserSpecifiedStoragePath + "\\" + frame.SourceName;
                   // destDir = m_AppData.DVR_UserSpecifiedStoragePath;

                    if ( ! FileAccessControl.DirectoryExists(destDir))
                        FileAccessControl.CreateDirectory(destDir);

                    destPath = destDir +"\\" + frame.JpegFileNameOnly;

                    
                }

                FileAccessControl.WriteStream(destPath, frame.Jpeg);

                Paths.AccumulatedSize += frame.Jpeg.Length;
            }
            catch (Exception ex)
            {
                m_Log.Log("WriteToStorage ex :" + ex.Message, ErrorLog.LOG_TYPE.FATAL);
            }

        }

        /// <summary>
        /// Given an image source file path, merge it into a destination file system
        ///  used to import files from a field drive and merge them into a central repository
        /// </summary>
        /// <param name="sourcePath"></param>
        /// <param name="destPath"></param>
        /// <param name="fd"></param>
        public void MoveMergeFileToDVRStorage(PATHS.IMAGE_FILE_DATA srcFd, PATHS.IMAGE_FILE_DATA destFd)
        {
            if (PauseFlag.Pause) return;

            try
            {
                FileInfo fi = new FileInfo(srcFd.completePath);

                if (!FileAccessControl.DirectoryExists( destFd.dirOnly ))
                {
                    char[] seperator = { '\\', '\\' };

                    string[] branches = destFd.dirOnly.Split(seperator);
                    string path = null;
                    for (int i = 0; i < branches.Count()-1; i++)  // the last string in the array is the file name
                    {
                        if (branches[i].Length < 1) continue;
                        path += (branches[i] + "\\");

                        if ( ! FileAccessControl.DirectoryExists(path))
                        {
                            FileAccessControl.CreateDirectory(path);
                        }
                    }
                }

                Paths.AccumulatedSize += fi.Length;

                FileAccessControl.FileMove( srcFd.completePath ,  destFd.completePath);

            }

            catch (Exception ex)
            {
                m_Log.Log("CopyFileToDVRStorage ex :" + ex.Message, ErrorLog.LOG_TYPE.FATAL);
            }
        }

        public void DebugWriteBmpToFile(string name, Bitmap bmp)
        {
            try
            {
                if (!Directory.Exists(Paths.Drive + "Debug"))
                {
                    Directory.CreateDirectory(Paths.Drive + "Debug");
                }
                string path = Paths.Drive + "Debug\\" + name + ".bmp";

                bmp.Save(path);
            }
            catch { }
        }

        // used by the DVR loop to move files from the pre-motion buffer directory to their final resting place, based on a motion event
        void MoveFileToDVRStorage(string file, string sourceName)
        {
            if (PauseFlag.Pause) return;

            try
            {
                FileInfo fi = new FileInfo(file);
                string destDir = null;
                string destPath = Paths.GetFrameStoragePath(fi.Name, sourceName, out destDir);


                if (!FileAccessControl.DirectoryExists((string)destDir))
                {
                    char[] seperator = { '\\', '\\' };

                    string[] branches = destDir.Split(seperator);
                    string path = null;
                    for (int i = 0; i < branches.Count(); i++)
                    {
                        if (branches[i].Length < 1) continue;
                        path += (branches[i] + "\\");

                        if ( ! FileAccessControl.DirectoryExists(path))
                        {
                            FileAccessControl.CreateDirectory(path);
                        }
                    }
                }

                Paths.AccumulatedSize += fi.Length;
                FileAccessControl.FileMove(file, destPath);
             
            }
            catch (Exception ex)
            {
                m_Log.Log("CopyFileToDVRStorage ex :" + ex.Message, ErrorLog.LOG_TYPE.FATAL);
            }
        }

        class PRE_MOTION_RECORDS
        {

            public bool PreMotionChannelSubDirCreated;
            public bool MotionDetectedMovingFilesInProcess;
            public ThreadSafeQueue<string> PendingMotionDetectionQ;

        }

        PRE_MOTION_RECORDS[] m_PreMotionRecords;

        string GetPreMotionFileNameCompletePath(FRAME frame)
        {
            return (Paths.PreMotionPath + "\\" + frame.SourceName + "\\" + frame.JpegFileNameOnly);
        }

        /// <summary>
        /// put images into the pre-motion temp buffer area on disk
        /// </summary>
        /// <param name="frame"></param>
        void PendingMotionDetection(FRAME frame)
        {
            if (PauseFlag.Pause) return;

            if (m_PreMotionRecords[frame.SourceChannel].MotionDetectedMovingFilesInProcess) return;

            // generate a sub-directory prequalified file name for this image to go into
            string PreMotionFileNameCompletePath = GetPreMotionFileNameCompletePath(frame);

            FileAccessControl.WriteStream(PreMotionFileNameCompletePath, frame.Jpeg);


            // use this queue to keep track of the last X frames to have come in before a motion event
            m_PreMotionRecords[frame.SourceChannel].PendingMotionDetectionQ.Enqueue(PreMotionFileNameCompletePath);

            // keep the number of pre-motion frames at m_MotionFrameCount
            if (m_PreMotionRecords[frame.SourceChannel].PendingMotionDetectionQ.Count >= m_NumberOfFramesToPrePostBuffer)
            {
                PreMotionFileNameCompletePath = m_PreMotionRecords[frame.SourceChannel].PendingMotionDetectionQ.Dequeue();
         
                FileAccessControl.FileDelete(PreMotionFileNameCompletePath);
            }


        }

        //  ////////////////////    Temporary Files

        Thread m_TempFileCleanUpThread;
        void TempFilesCleanUpLoop()
        {
            TimeSpan TimeInTempBuffer = new TimeSpan(0, 1, 0);
            while (!m_Stop)
            {
                try
                {
                    foreach (TEMPFILES file in m_TempFileList)
                    {
                        while (PauseFlag.Pause && !m_Stop) // pause if we lost a drive
                            Thread.Sleep(100);

                        if (DateTime.Now.Subtract(file.timeCreated).CompareTo(TimeInTempBuffer) >= 0)
                        {
                            // File.Delete(file.path);
                            FileAccessControl.FileDelete(file.path);
                            m_TempFileList.Remove(file);
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    m_Log.Log("TempFilesCleanUpLoop ex " + ex.Message, ErrorLog.LOG_TYPE.FATAL);
                }
                Thread.Sleep(100);
            }
        }

        class TEMPFILES
        {
            public string path;
            public DateTime timeCreated;
        }
        ThreadSafeList<TEMPFILES> m_TempFileList;
        int m_UniqueSerialNumber = 0;
        /// <summary>
        /// Creates a copy of the file in a temp directory and give it a unique file name. The file is cleaned up ater 10 minutes.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public string GetUniqueCopy(string file)
        {
            /// the email server has a problem that if you attach a file to a new mail, and then
            /// send multiple emails with the same attachment, you get an exception of file open by another process.
            ///  so create a new file copy for each mail that is sent. Don't keep this file around forever.
            ///  

            string temppath = Paths.GetTempDirectory();

            FileInfo fi = new FileInfo(file);

            // remove the exention
            string[] sp1 = fi.Name.Split('.');
            string nameOnly = sp1[0];


            string newname = nameOnly + m_UniqueSerialNumber.ToString() + "." + sp1[1];

            m_UniqueSerialNumber++;

            newname = temppath + "\\" + newname;
            try
            {
                //  File.Copy(file, newname);
                FileAccessControl.FileCopy(file, newname);

                TEMPFILES tempFileInfo = new TEMPFILES();
                tempFileInfo.path = newname;
                tempFileInfo.timeCreated = DateTime.Now;

                m_TempFileList.Add(tempFileInfo);
            }
            catch { }

            return (newname);

        }

        public class PAUSE_FLAG
        {
            public PAUSE_FLAG() { Pause = false; Stop = false; }
            public bool Pause;
            public bool Stop;
            public bool DriveExists;
            public bool Delay
            {
                get
                {
                    if (!DriveExists) return (false);  // dont pause, but do skip
                    if (OverridePause) return (Stop); // if not stopping, don't loop
                    else return (Pause && !Stop);    // loop if pauseing and not stopping
                }
            }
            public bool OverridePause;
        }

        public string GetFieldDrive()
        {
            return (m_DriveManager.GetFieldDrive());
        }

        /// <summary>
        /// Only allow one thread to touch the file system at a time. Also, last chance to catch a DVR Pause and stop the file system access.
        /// All methods block on DVR Pause, but unblock on a global Stop.
        /// </summary>
        public class FILE_SYSTEM_ACCESS
        {
            public FILE_SYSTEM_ACCESS(PAUSE_FLAG pfg, APPLICATION_DATA appData)
            {
                singleton = new object();
                pf = pfg;
                m_AppData = appData;
                m_Log = (ErrorLog) m_AppData.Logger;
            }
            object singleton;
            PAUSE_FLAG pf;
            APPLICATION_DATA m_AppData;
            ErrorLog m_Log;

            public void FileCopy(string sourceFileName, string destFileName)
            {
                lock (singleton)
                {
                    while (pf.Delay) { if (pf.Stop) break; Thread.Sleep(10); }
                    if (!pf.DriveExists) return;
                    try
                    {
                        File.Copy(sourceFileName, destFileName);
                    }
                    catch { }
                }
            }

            public void FileDelete(string file)
            {
                lock (singleton)
                {
                    while (pf.Delay) { if (pf.Stop) break; Thread.Sleep(10); }
                    if (!pf.DriveExists) return;
                    try
                    {
                        File.Delete(file);
                    }
                    catch { }
                }
            }

            public void WriteStream(string path, byte[] bytes)
            {
                lock (singleton)
                {
                    while (pf.Delay) { if (pf.Stop) break; Thread.Sleep(10); }
                    if (!pf.DriveExists) return;
                    try
                    {
                        FileStream fs = new FileStream(path, FileMode.Create);
                        fs.Write(bytes, 0, bytes.Length);
                        fs.Close();
                    }
                    catch (Exception ex) { m_Log.Trace(ex, ErrorLog.LOG_TYPE.INFORMATIONAL); }
                }
            }

            public void FileMove(string sourceFileName, string destFileName)
            {
                lock (singleton)
                {
                    while (pf.Delay) { if (pf.Stop) break; Thread.Sleep(10); }
                    if (!pf.DriveExists) return;
                    try
                    {
                       
                        if (File.Exists(destFileName))
                            File.Delete(destFileName);
                        //File.Move(sourceFileName, destFileName);
                        File.Copy(sourceFileName, destFileName, true);
                        if (File.Exists(destFileName))
                            File.Delete(sourceFileName);
                        else
                        {
                            m_Log.Log("Erroring moving file to destination : "+destFileName, ErrorLog.LOG_TYPE.FATAL );
                        }
                       
                    }
                    catch (Exception ex) { m_Log.Trace(ex, ErrorLog.LOG_TYPE.INFORMATIONAL); }
                }
            }

            public void AppendAllText(string file, string line)
            {
                lock (singleton)
                {
                    while (pf.Delay) { if (pf.Stop) break; Thread.Sleep(10); }
                    if (!pf.DriveExists) return;
                    try
                    {
                        File.AppendAllText(file, line);
                    }
                    catch (Exception ex) { m_Log.Trace(ex, ErrorLog.LOG_TYPE.INFORMATIONAL); }
                }
            }


            public void CreateDirectory(string path)
            {
                lock (singleton)
                {
                    while (pf.Delay) { if (pf.Stop) break; Thread.Sleep(10); }
                    if (!pf.DriveExists) return;
                    try
                    {
                        Directory.CreateDirectory(path);
                    }
                    catch (Exception ex) { m_Log.Trace(ex, ErrorLog.LOG_TYPE.INFORMATIONAL); }
                }
            }

            public bool DirectoryExists(string path)
            {
                lock (singleton)
                {
                    while (pf.Delay) { if (pf.Stop) break; Thread.Sleep(10); }
                    if (!pf.DriveExists) return false;
                    try
                    {
                        return (Directory.Exists(path));
                    }
                    catch (Exception ex) { m_Log.Trace(ex, ErrorLog.LOG_TYPE.INFORMATIONAL); }
                }
                return (false);
            }

            public void DirectoryDelete(string path)
            {
                lock (singleton)
                {
                    while (pf.Delay) { if (pf.Stop) break; Thread.Sleep(10); }
                    if (!pf.DriveExists) return;
                    try
                    {
                        Directory.Delete(path);
                    }
                    catch (Exception ex) { m_Log.Trace(ex, ErrorLog.LOG_TYPE.INFORMATIONAL); }
                }
            }


            public string[] GetDirectories(string path)
            {
                lock (singleton)
                {
                    while (pf.Delay) { if (pf.Stop) break; Thread.Sleep(10); }
                    if (!pf.DriveExists) return null;
                    try
                    {
                        return Directory.GetDirectories(path);
                    }
                    catch (Exception ex) { m_Log.Trace(ex, ErrorLog.LOG_TYPE.INFORMATIONAL); }
                    return (null);
                }
            }

            public string[] GetFiles(string path)
            {
                lock (singleton)
                {
                    while (pf.Delay) { if (pf.Stop) break; Thread.Sleep(10); }
                    if (!pf.DriveExists) return null;
                    try
                    {
                        return Directory.GetFiles(path);
                    }
                    catch (Exception ex) { m_Log.Trace(ex, ErrorLog.LOG_TYPE.INFORMATIONAL); }
                    return (null);
                }
            }

            public double DeleteDirectoryAndContents(string dir)
            {
                double size = RecursivelyEmptyDirectory(dir, 0.0);
                DirectoryDelete(dir);

                return (size);
            }

            double RecursivelyEmptyDirectory(string rootDir, double size)
            {
               
                if (!pf.DriveExists) return size;// we can loose a drive  while looping


                string[] files;

                // get the list of Directories
                string[] directories = null;

                directories = GetDirectories(rootDir);
                if (directories == null) return size;

                foreach (string directory in directories)
                {
                    
                    if (!pf.DriveExists) return size;// we can loose a drive in while looping

                    size += RecursivelyEmptyDirectory(directory, size);

                    // this dir should now be empty, delete it
                    DirectoryDelete(directory);

                }


                // does this directory have files ?
                files = GetFiles(rootDir);

                foreach (string file in files)
                {
                    if (!pf.DriveExists) return size ;// we can loose a drive in while looping

                    try
                    {
                        FileInfo fi = new FileInfo(file);

                        size += fi.Length;
                    }
                    catch { }

                    FileDelete(file);
                }

                return (size);
            }


        }

    }

}
