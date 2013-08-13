using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using ApplicationDataClass;
using FrameGeneratorLib;
using ErrorLoggingLib;

namespace PathsLib
{



    // /////////////////////////////////////////////////////////
    //  ////////////////////////////////////////////////////
    //
    //     PATHS
    //
    //       drive = "f:\\" , either autodetected external drive or user specified in configfile
    //
    //       storage directory = "DVRSTORAGE" or user specified in config file
    //       storage path = "f:\\DVRSTORAGE"

    //       pre-motion buffer storage directory = "PREMOTION"
    //       pre-motion buffer storage path = "f:\\DVRSTORAGE\\PREMOTION" 
    //
    //       frame storage file path =  storage directory\\Year\\Month\\Day\\Hour\\PSS_Name\\Source_Name\\YYYY_MM_DD_HH_MM_SS_mmm.jpg
    //                                 "f:\\DVRSTORAGE\\2009\\0809\\15\\PSS_01\\Camera_01\\2009_07_09_15_55_345.jpg 
    //
    //
    //      event log file path =  STORAGE_DIR\\Year\\Month\\Day\\Hour\\PSS_Name\\EVENTLOG.TXT

    public class PATHS
    {
        private bool ParentIsDVR;
        private string drive;
        public string StorageDir;
        public string StoragePath;
        public string PreMotionDir;
        public string PreMotionPath;
        public string TempDir;
        public string SystemName;
        private double accumulatedSize;
        public bool Enabled;
        private APPLICATION_DATA m_AppData;
        ErrorLog m_Log;

        public double AccumulatedSize
        {
            set
            {
                lock (singleton)
                {
                    accumulatedSize = value;
                }
            }
            get
            {
                lock (singleton)
                {
                    return (accumulatedSize);
                }
            }
        }
        object singleton;

        public PATHS(string systemName, bool parentIsDVR, APPLICATION_DATA appData) // constructor
        {

            singleton = new object();
            StorageDir = "DVRSTORAGE";
            TempDir = "TEMPFILES";
            PreMotionDir = "PREMOTION";
            SystemName = systemName;
            accumulatedSize = 0;
            Enabled = false;
            ParentIsDVR = parentIsDVR;
            m_AppData = appData;
            m_AppData.AddOnClosing(Stop, APPLICATION_DATA.CLOSE_ORDER.FIRST);
            m_Log = (ErrorLog)m_AppData.Logger;
        }

        bool m_Stop = false;
        void Stop()
        {
            m_Stop = true;
        }



        public void StartThreads() { /* no threads to start */}
       

        public string UserSetStorageLocation
        {
            set
            {
                lock (singleton)
                {
                    //value is of the form=> c:\\fasd\\asdfsdf\\afsdf
                    if (value == null)
                    {
                        Enabled = false;
                        return;
                    }
                    string s = value;
                    string[] sp = s.Split(':');
                    drive = sp[0] +":";

                    if (!Directory.Exists(drive))
                    {
                        Enabled = false;
                        return;
                    }

                    StorageDir = sp[1];

                    StoragePath = drive + StorageDir;
                    PreMotionPath = StoragePath + "\\" + PreMotionDir;
                    accumulatedSize = 0;
                   
                    Enabled = true;
                }

            }
        }

        public string Drive
        {
            set
            {
                lock (singleton)
                {
                    drive = value;
                    if (drive == null)
                    {
                        Enabled = false;
                        StoragePath = null;
                        PreMotionPath = null;
                        accumulatedSize = 0;
                        return;
                    }
                    if (!Directory.Exists(drive))
                    {
                        Enabled = false;
                        return;
                    }
                    StoragePath = drive + StorageDir;
                    PreMotionPath = StoragePath + "\\" + PreMotionDir;
                    accumulatedSize = 0;
                   // GetDriveFileSize();
                    Enabled = true;


                }

            }
            get
            {
                lock (singleton)
                {
                    return drive;
                }
            }
        }

        public string GetFrameStoragePath(string fileName, string sourceName, out string destDir)
        {
            lock (singleton)
            {
                int year;
                int month;
                int day;
                int hour;

                // STORAGE_DIR/Year/Month/Day/Hour/PSS_Name/Source_Name/YYYY_MM_DD_HH_MM_SS_mmm.jpg

                string[] sp = fileName.Split('_');

                year = Convert.ToInt32(sp[0]);
                month = Convert.ToInt32(sp[1]);
                day = Convert.ToInt32(sp[2]);
                hour = Convert.ToInt32(sp[3]);

                destDir = Drive + StorageDir + "\\" +
                                year.ToString() + "\\" +
                                month.ToString() + "\\" +
                                day.ToString() + "\\" +
                                hour.ToString() + "\\" +
                                SystemName + "\\" +
                                sourceName ;

                string path = destDir + "\\" + fileName;
                return (path);
            }
        }

        public string GetFrameStoragePath(FRAME frame)
        {
            lock (singleton)
            {
                DateTime timeStamp = frame.TimeStamp;

                int year = timeStamp.ToUniversalTime().Year;
                int month = timeStamp.ToUniversalTime().Month;
                int day = timeStamp.ToUniversalTime().Day;
                int hour = timeStamp.ToUniversalTime().Hour;

                string path = Drive + StorageDir + "\\" +
                                year.ToString("####") + "\\" +
                                month.ToString("####") + "\\" +
                                day.ToString("####") + "\\" +
                                hour.ToString("####") + "\\" +
                                SystemName + "\\" +
                                frame.SourceName + "\\" +
                                frame.JpegFileNameOnly;

                return (path);
            }
        }

        public string GetJpegRelateivePath(FRAME frame)
        {
            lock (singleton)
            {
                DateTime timeStamp = frame.TimeStamp;

                int year = timeStamp.ToUniversalTime().Year;
                int month = timeStamp.ToUniversalTime().Month;
                int day = timeStamp.ToUniversalTime().Day;
                int hour = timeStamp.ToUniversalTime().Hour;

                string path = year.ToString("####") + "\\" +
                                month.ToString("####") + "\\" +
                                day.ToString("####") + "\\" +
                                hour.ToString("####") + "\\" +
                                SystemName + "\\" +
                                frame.SourceName + "\\" +
                                frame.JpegFileNameOnly;

                return (path);
            }
        }

        /// <summary>
        /// Returns an event log file path given a time and channel source name, assumes this machine for PSS system name
        /// and assumes the currently connected storage drive
        /// </summary>
        /// <param name="timeStamp"></param>
        /// <param name="sourceName"></param>
        /// <returns></returns>
        public string GetEventLogFilePath(DateTime timeStamp, string sourceName)
        {
            lock (singleton)
            {

                int year = timeStamp.ToUniversalTime().Year;
                int month = timeStamp.ToUniversalTime().Month;
                int day = timeStamp.ToUniversalTime().Day;
                int hour = timeStamp.ToUniversalTime().Hour;

                string path = Drive + StorageDir + "\\" +
                                year.ToString() + "\\" +
                                month.ToString() + "\\" +
                                day.ToString() + "\\" +
                                hour.ToString() + "\\" +
                                SystemName + "\\" +
                                "EVENTLOG.TXT";

                return (path);
            }
        }


        /// <summary>
        /// Returns the event log file path for a new destination drive given a source in IMAGE_FILE_DATA format
        /// </summary>
        /// <param name="fd"></param>
        /// <param name="destDrive"></param>
        /// <returns></returns>
        public string GetEventLogFilePath(IMAGE_FILE_DATA fd, string destDrive)
        {
            lock (singleton)
            {

                int year = fd.timeStamp.ToUniversalTime().Year;
                int month = fd.timeStamp.ToUniversalTime().Month;
                int day = fd.timeStamp.ToUniversalTime().Day;
                int hour = fd.timeStamp.ToUniversalTime().Hour;

                string path = destDrive + StorageDir + "\\" +
                                year.ToString("####") + "\\" +
                                month.ToString("####") + "\\" +
                                day.ToString("####") + "\\" +
                                hour.ToString("####") + "\\" +
                                fd.PSSName + "\\" +
                                "EVENTLOG.TXT";

                return (path);
            }
        }

        public string GetEventLogDirectoryPath(DateTime timeStamp, string sourceName)
        {
            lock (singleton)
            {

                int year = timeStamp.ToUniversalTime().Year;
                int month = timeStamp.ToUniversalTime().Month;
                int day = timeStamp.ToUniversalTime().Day;
                int hour = timeStamp.ToUniversalTime().Hour;

                string path = Drive + StorageDir + "\\" +
                                year.ToString("####") + "\\" +
                                month.ToString("####") + "\\" +
                                day.ToString("####") + "\\" +
                                hour.ToString("####") + "\\" +
                                SystemName + "\\";

                return (path);
            }
        }

        double RecursivelyGetCurrentFileSystemSize(string rootDir, ref double storageTally)
        {
            int errorCount = 0;

            if (! Enabled) return 0;// we can loose a drive in while looping

            if (rootDir.Contains("PREMOTION")) return 0.0;// these files are few and constantly getting deleted, ignore them

            string[] files;

            // get the list of Directories
            string[] directories=null;
            try
            {
                directories = Directory.GetDirectories(rootDir);
            }
            catch (Exception ex)
            {
                errorCount++;
                m_Log.Log("RecursivelyGetCurrentFileSystemSize ex " + ex.Message, ErrorLog.LOG_TYPE.FATAL);

                if (errorCount > 2) return 0;
            }

            if (directories == null) return 0;
            foreach (string directory in directories)
            {

                if (!Enabled) return 0;// we can loose a drive in while looping
                try
                {
                    // does this directory have sub directories ?
                    string[] directories1 = Directory.GetDirectories(directory);

                    foreach (string d1 in directories1)
                    {
                        if (!Enabled) return 0;// we can loose a drive in while looping
                        RecursivelyGetCurrentFileSystemSize(d1, ref  storageTally);
                        if (m_Stop) break;
                    }
                }
                catch (Exception ex)
                {
                    errorCount++;
                    m_Log.Log("RecursivelyGetCurrentFileSystemSize ex " + ex.Message, ErrorLog.LOG_TYPE.FATAL);
                    if (errorCount > 2) return 0;
                }

                if (m_Stop) break;

            }


            // does this directory have files ?
            files = Directory.GetFiles(rootDir);

            foreach (string file in files)
            {
                if (!Enabled) return 0;// we can loose a drive in while looping

                try
                {
                    FileInfo fi = new FileInfo(file);
                    storageTally += (int)fi.Length;
                    if (m_Stop) break;
                }
                catch (Exception ex)
                {
                    errorCount++;
                    m_Log.Log("RecursivelyGetCurrentFileSystemSize ex " + ex.Message, ErrorLog.LOG_TYPE.FATAL);
                    if (errorCount > 2) return 0;
                }

            }


            return (storageTally);
        }

        static string GetEndofString(string path)
        {
            string[] sp = path.Split('\\');
            string result = sp[sp.Length - 1];
            return (result);
        }

        /// <summary>
        /// Returns the complete paths for all log files which meet the search criteria
        /// </summary>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <returns></returns>
        public string[] GetAllLogFilesInRange(string sourceDrive, DateTime start, DateTime stop)
        {
            return (_GetAllLogFilesInRange(sourceDrive + StorageDir, start, stop));
        }
        /// <summary>
        /// Returns the complete paths for all log files which meet the search criteria
        /// </summary>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <returns></returns>
        public string[] GetAllLogFilesInRange( DateTime start, DateTime stop)
        {
            return (_GetAllLogFilesInRange(StoragePath, start, stop));
        }

        public string[] _GetAllLogFilesInRange(string sourcePath, DateTime start, DateTime stop)
        {
            List<string> allLogsInRange = new List<string>();
            try
            {

                DateTime timeAtCurrentDirectory = default(DateTime);

                DateTime startYear = new DateTime(start.Year, 1, 1, 0, 0, 0);
                DateTime startMonth = new DateTime(start.Year, start.Month, 1, 0, 0, 0);
                DateTime startDay = new DateTime(start.Year, start.Month, start.Day, 0, 0, 0);
                DateTime startHour = new DateTime(start.Year, start.Month, start.Day, start.Hour, 0, 0);
                DateTime startMin = new DateTime(start.Year, start.Month, start.Day, start.Hour, start.Minute, 0);
                DateTime startSec = new DateTime(start.Year, start.Month, start.Day, start.Hour, start.Minute, start.Second);

                DateTime stopYear = new DateTime(stop.Year, 1, 1, 0, 0, 0);
                DateTime stopMonth = new DateTime(stop.Year, stop.Month, 1, 0, 0, 0);
                DateTime stopDay = new DateTime(stop.Year, stop.Month, stop.Day, 0, 0, 0);
                DateTime stopHour = new DateTime(stop.Year, stop.Month, stop.Day, stop.Hour, 0, 0);
                DateTime stopMin = new DateTime(stop.Year, stop.Month, stop.Day, stop.Hour, stop.Minute, 0);
                DateTime stopSec = new DateTime(stop.Year, stop.Month, stop.Day, stop.Hour, stop.Minute, stop.Second);

                string[] years = Directory.GetDirectories(sourcePath);
                if (years.Length == 0) return (null);
                years = RemoveNonYearDirs(years);
                years = SortByNumeric(years);

                foreach (string year in years)
                {

                    int yearInt = 0;
                    try
                    {
                        yearInt = Convert.ToInt16(GetEndofString(year));
                    }
                    catch { continue; } // expect there to be sub directories here that are not valid numbers, just skip them.

                    timeAtCurrentDirectory = new DateTime(yearInt, 1, 1, 0, 0, 0);
                    if (timeAtCurrentDirectory.CompareTo(startYear) >= 0 && timeAtCurrentDirectory.CompareTo(stopYear) <= 0)
                    {
                        string[] months = Directory.GetDirectories(year);
                        if (months.Length == 0) continue;
                        months = RemoveNonYearDirs(months);
                        months = SortByNumeric(months);

                        foreach (string month in months)
                        {

                            int monthInt = getIntValue(month);
                            timeAtCurrentDirectory = new DateTime(yearInt, monthInt, 1, 0, 0, 0);
                            if (timeAtCurrentDirectory.CompareTo(startMonth) >= 0 && timeAtCurrentDirectory.CompareTo(stopMonth) <= 0)
                            {
                                string[] days = Directory.GetDirectories(month);
                                if (days.Length == 0) continue;
                                days = RemoveNonYearDirs(days);
                                days = SortByNumeric(days);
                                foreach (string day in days)
                                {

                                    int dayInt = getIntValue(day);
                                    timeAtCurrentDirectory = new DateTime(yearInt, monthInt, dayInt, 0, 0, 0);
                                    if (timeAtCurrentDirectory.CompareTo(startDay) >= 0 && timeAtCurrentDirectory.CompareTo(stopDay) <= 0)
                                    {
                                        string[] hours = Directory.GetDirectories(day);
                                        if (hours.Length == 0) continue;
                                        hours = RemoveNonYearDirs(hours);
                                        hours = SortByNumeric(hours);
                                        foreach (string hour in hours)
                                        {

                                            int hourInt = getIntValue(hour);
                                            timeAtCurrentDirectory = new DateTime(yearInt, monthInt, dayInt, hourInt, 0, 0);
                                            if (timeAtCurrentDirectory.CompareTo(startHour) >= 0 && timeAtCurrentDirectory.CompareTo(stopHour) <= 0)
                                            {
                                                string[] PSSUnitsNode = Directory.GetDirectories(hour);
                                                if (PSSUnitsNode.Length == 0) continue;
                                            
                                                foreach (string pss in PSSUnitsNode)
                                                {
                                                    string[] sa = Directory.GetFiles(pss);
                                                    foreach (string file in sa)
                                                        allLogsInRange.Add(file);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            { m_Log.Log(" GetAllLogFilesInRange ex " + ex.Message, ErrorLog.LOG_TYPE.FATAL); return (null); }
            return (allLogsInRange.ToArray());
        }

        int getIntValue(string s)
        {
            try
            {
                int val = Convert.ToInt32(GetEndofString(s));
                return (val);
            }
            catch (Exception ex)
            {
                m_Log.Log("getIntValue ex: " + ex.Message + ", input string: " + s, ErrorLog.LOG_TYPE.FATAL);
                return (0);
            }
        }

        /// <summary>
        /// Returns a list of jpeg files which meet the search criteria. Returns complete file path. Uses the default primary drive storage path.
        /// </summary>
        /// <param name="PSSName"></param>
        /// <param name="sourceName"></param>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <returns></returns>
        public string[] GetAllJpegsInRange(DateTime start, DateTime stop, string PSS, string channelSource)
        {
            m_GetAllJpegsProcessCallback = null;
            bool stopSearch = false;
            return (_GetAllJpegsInRange(StoragePath, start, stop, PSS, channelSource, (object) stopSearch, m_AppData.MAX_SEARCH_RESULTS_TO_DISPLAY));
        }

        public class PATHS_GET_STATUS
        {
            public int totalCount;
            public int currentCount;
            public DateTime currentTime;
            public DateTime startTime;
            public DateTime endTime;
            public string errors;
        }

        /// <summary>
        /// Returns a list of jpeg files which meet the search criteria. Returns complete file path. Uses the default primary drive storage path.
        /// </summary>
        /// <param name="PSSName"></param>
        /// <param name="sourceName"></param>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <returns></returns>
        ///         
        public delegate void GetAllJpegsProgressDelegate(PATHS_GET_STATUS status);
        GetAllJpegsProgressDelegate m_GetAllJpegsProcessCallback;

        int m_ProgressUpdateFrequency = 0;

        public class GET_ALL_JPEGS_IN_RANGE_PARAMS
        {
            public DateTime start;
            public DateTime stop;
            public GetAllJpegsProgressDelegate callBack;
            public int updateFreq;
            public object stopSearch;// want bool but need reference so that it can change globally and be detected
            public int maxResultsCount;
        }

        public string[] GetAllJpegsInRange(GET_ALL_JPEGS_IN_RANGE_PARAMS parameters)
        {
            m_GetAllJpegsProcessCallback = parameters.callBack;
            m_ProgressUpdateFrequency = parameters.updateFreq;
            return ( _GetAllJpegsInRange(StoragePath, parameters.start, parameters.stop, null, null,  parameters.stopSearch, parameters.maxResultsCount));
        }

        public string[] GetAllJpegsInRange(DateTime start, DateTime stop)
        {
            m_GetAllJpegsProcessCallback = null;
            bool stopSearch = false;
            return (_GetAllJpegsInRange(StoragePath, start, stop, null, null, (object)stopSearch, m_AppData.MAX_SEARCH_RESULTS_TO_DISPLAY));
        }

        /// <summary>
        /// Returns a list of jpeg files which meet the search criteria. Returns complete file path. Specify the source drive , e.g. 'D:\\'
        /// </summary>
        /// <param name="PSSName"></param>
        /// <param name="sourceName"></param>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <returns></returns>
        /// 

        public string[] GetAllJpegsInRange(string sourceDrive, DateTime start, DateTime stop)
        {
            m_GetAllJpegsProcessCallback = null;
            bool stopSearch = false;
            return (_GetAllJpegsInRange(sourceDrive + StorageDir, start, stop, null, null, (object) stopSearch, m_AppData.MAX_SEARCH_RESULTS_TO_DISPLAY));
        }

        private string[] _GetAllJpegsInRange(string sourcePath, DateTime start, DateTime stop, string selectedPSS, string selectedChannelSource, object StopLoop, int maxCount)
        {
            List<string> allJpegsInRange = new List<string>();
            DateTime timeAtCurrentDirectory = default(DateTime);

            try
            {

               

                DateTime startYear = new DateTime(start.Year, 1, 1, 0, 0, 0);
                DateTime startMonth = new DateTime(start.Year, start.Month, 1, 0, 0, 0);
                DateTime startDay = new DateTime(start.Year, start.Month, start.Day, 0, 0, 0);
                DateTime startHour = new DateTime(start.Year, start.Month, start.Day, start.Hour, 0, 0);
                DateTime startMin = new DateTime(start.Year, start.Month, start.Day, start.Hour, start.Minute, 0);
                DateTime startSec = new DateTime(start.Year, start.Month, start.Day, start.Hour, start.Minute, start.Second);

                DateTime stopYear = new DateTime(stop.Year, 1, 1, 0, 0, 0);
                DateTime stopMonth = new DateTime(stop.Year, stop.Month, 1, 0, 0, 0);
                DateTime stopDay = new DateTime(stop.Year, stop.Month, stop.Day, 0, 0, 0);
                DateTime stopHour = new DateTime(stop.Year, stop.Month, stop.Day, stop.Hour, 0, 0);
                DateTime stopMin = new DateTime(stop.Year, stop.Month, stop.Day, stop.Hour, stop.Minute, 0);
                DateTime stopSec = new DateTime(stop.Year, stop.Month, stop.Day, stop.Hour, stop.Minute, stop.Second);

                string[] years = Directory.GetDirectories(sourcePath);
                if (years.Length == 0) return null;
                years = RemoveNonYearDirs(years);
                years = SortByNumeric(years);

                foreach (string year in years)
                {
                    int yearInt;
                    try
                    {
                        yearInt = Convert.ToInt16(GetEndofString(year));
                    }
                    catch
                    {
                        continue; // should no longer get here because I added years = RemoveNonYearDirs(years);
                    }

                    timeAtCurrentDirectory = new DateTime(yearInt, 1, 1, 0, 0, 0);
                    if (timeAtCurrentDirectory.CompareTo(startYear) >= 0 && timeAtCurrentDirectory.CompareTo(stopYear) <= 0)
                    {
                        string[] months = Directory.GetDirectories(year);
                        if (months.Length == 0) continue;
                        months = SortByNumeric(months);

                        foreach (string month in months)
                        {

                            int monthInt=0;
                            try
                            {
                                monthInt = Convert.ToInt32(GetEndofString(month));
                            }
                            catch { continue; }
                            timeAtCurrentDirectory = new DateTime(yearInt, monthInt, 1, 0, 0, 0);
                            if (timeAtCurrentDirectory.CompareTo(startMonth) >= 0 && timeAtCurrentDirectory.CompareTo(stopMonth) <= 0)
                            {
                                string[] days = Directory.GetDirectories(month);
                                if (days.Length == 0) continue;
                                days = SortByNumeric(days);

                                foreach (string day in days)
                                {
                                    int dayInt = 0;
                                    try
                                    {
                                        dayInt = Convert.ToInt32(GetEndofString(day));
                                    }
                                    catch { continue; }
                                    timeAtCurrentDirectory = new DateTime(yearInt, monthInt, dayInt, 0, 0, 0);
                                    if (timeAtCurrentDirectory.CompareTo(startDay) >= 0 && timeAtCurrentDirectory.CompareTo(stopDay) <= 0)
                                    {
                                        string[] hours = Directory.GetDirectories(day);
                                        if (hours.Length == 0) continue;
                                        hours = SortByNumeric(hours);

                                        foreach (string hour in hours)
                                        {
                                            int hourInt;
                                            try
                                            {
                                                hourInt = Convert.ToInt32(GetEndofString(hour));
                                            }
                                            catch { continue; }
                                            timeAtCurrentDirectory = new DateTime(yearInt, monthInt, dayInt, hourInt, 0, 0);
                                            if (timeAtCurrentDirectory.CompareTo(startHour) >= 0 && timeAtCurrentDirectory.CompareTo(stopHour) <= 0)
                                            {
                                                string[] PSSUnitsNode = Directory.GetDirectories(hour);
                                                if (PSSUnitsNode.Length == 0) continue;

                                                foreach (string pss in PSSUnitsNode)
                                                {
                                                    if (selectedPSS != null)
                                                    {
                                                        if (GetEndofString(pss) != selectedPSS) continue;
                                                    }
                                                    string[] sources = Directory.GetDirectories(pss);
                                                    foreach (string source in sources)
                                                    {
                                                        if (selectedChannelSource != null)
                                                        {
                                                            if (GetEndofString(source) != selectedChannelSource) continue;
                                                        }
                                                        if (m_Stop) return null;



                                                        string[] jpegs = Directory.GetFiles(source, "*.jpg");
                                                        string[] jpegsInRange = GetJpegsInMinutesRange(jpegs, start, stop);
                                                        if (jpegsInRange != null)
                                                        {
                                                            foreach (string jpeg in jpegsInRange)
                                                            {
                                                                allJpegsInRange.Add(jpeg);

                                                                if (m_GetAllJpegsProcessCallback != null)
                                                                {
                                                                    if (allJpegsInRange.Count % m_ProgressUpdateFrequency == 0)
                                                                    {
                                                                        PATHS_GET_STATUS status = new PATHS_GET_STATUS();
                                                                        status.currentTime = timeAtCurrentDirectory;
                                                                        status.currentCount = 0;
                                                                        status.totalCount = 0;
                                                                        status.errors = null;
                                                                        m_GetAllJpegsProcessCallback(status);
                                                                    }
                                                                }
                                                            }
                                                        }

                                                        if ((bool)StopLoop || allJpegsInRange.Count > maxCount) break; 
                                                    }
                                                    if ((bool)StopLoop || allJpegsInRange.Count > maxCount) break; 
                                                }
                                                if ((bool)StopLoop || allJpegsInRange.Count > maxCount) break; 
                                            }
                                            if ((bool)StopLoop || allJpegsInRange.Count > maxCount) break; 
                                        }
                                        if ((bool)StopLoop || allJpegsInRange.Count > maxCount) break; 
                                    }
                                    if ((bool)StopLoop || allJpegsInRange.Count > maxCount) break; 
                                }
                                if ((bool)StopLoop || allJpegsInRange.Count > maxCount) break; 
                            }
                            if ((bool)StopLoop || allJpegsInRange.Count > maxCount) break; 
                        }
                        if ((bool)StopLoop || allJpegsInRange.Count > maxCount) break; 
                    }
                    if ((bool)StopLoop || allJpegsInRange.Count > maxCount) break; 
                }
            }
            catch (Exception ex)
            {
                PATHS_GET_STATUS status = new PATHS_GET_STATUS();
                status.currentTime = timeAtCurrentDirectory;
                status.currentCount = 0;
                status.totalCount = 0;
                status.errors = ex.Message;
                m_GetAllJpegsProcessCallback(status);
                m_Log.Log("GetAllJpegsInRange ex " + ex.Message, ErrorLog.LOG_TYPE.FATAL);
            }

            string[] fileList = allJpegsInRange.ToArray();


            if (!(bool)StopLoop && allJpegsInRange.Count < maxCount) SortFilesByTimeStamp(fileList);

            if (m_GetAllJpegsProcessCallback != null)
            {
                PATHS_GET_STATUS status = new PATHS_GET_STATUS();
                status.currentTime = timeAtCurrentDirectory;
                status.currentCount = 0;
                status.totalCount = 0;
                status.errors = null;
                m_GetAllJpegsProcessCallback(status);
            }

            m_GetAllJpegsProcessCallback = null;

            return (fileList);
        }
        /// <summary>
        /// Returns aCount Of Jpegs In the time Range, specify the drive
        /// </summary>
        /// <param name="PSSName"></param>
        /// <param name="sourceName"></param>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <returns></returns>
        public delegate void statusIncrementCallBack(int count,DateTime time);
        public class CancelJPegCountLoop
        {
            public bool cancel;
        }

        public int GetCountOfJpegsInRange(string drive, DateTime start, DateTime stop, statusIncrementCallBack statusUpdate, int statusIncrement, CancelJPegCountLoop cancelControl)
        {
            CancelJPegCountLoop CancelControl = (CancelJPegCountLoop)cancelControl;
           
            int count = 0;
            try
            {

                DateTime timeAtCurrentDirectory = default(DateTime);

                DateTime startYear = new DateTime(start.Year, 1, 1, 0, 0, 0);
                DateTime startMonth = new DateTime(start.Year, start.Month, 1, 0, 0, 0);
                DateTime startDay = new DateTime(start.Year, start.Month, start.Day, 0, 0, 0);
                DateTime startHour = new DateTime(start.Year, start.Month, start.Day, start.Hour, 0, 0);
                DateTime startMin = new DateTime(start.Year, start.Month, start.Day, start.Hour, start.Minute, 0);
                DateTime startSec = new DateTime(start.Year, start.Month, start.Day, start.Hour, start.Minute, start.Second);

                DateTime stopYear = new DateTime(stop.Year, 1, 1, 0, 0, 0);
                DateTime stopMonth = new DateTime(stop.Year, stop.Month, 1, 0, 0, 0);
                DateTime stopDay = new DateTime(stop.Year, stop.Month, stop.Day, 0, 0, 0);
                DateTime stopHour = new DateTime(stop.Year, stop.Month, stop.Day, stop.Hour, 0, 0);
                DateTime stopMin = new DateTime(stop.Year, stop.Month, stop.Day, stop.Hour, stop.Minute, 0);
                DateTime stopSec = new DateTime(stop.Year, stop.Month, stop.Day, stop.Hour, stop.Minute, stop.Second);

                string[] years = Directory.GetDirectories(drive + StorageDir);
                if (years.Length == 0) return 0;
                Array.Sort(years);

                foreach (string year in years)
                {
                    int yearInt;
                    try
                    {
                        yearInt = Convert.ToInt16(GetEndofString(year));
                    }
                    catch
                    {
                        // the PREMOTION sub dir will catch here since its not a year
                        continue;
                    }

                    timeAtCurrentDirectory = new DateTime(yearInt, 1, 1, 0, 0, 0);
                    if (timeAtCurrentDirectory.CompareTo(startYear) >= 0 && timeAtCurrentDirectory.CompareTo(stopYear) <= 0)
                    {
                        string[] months = Directory.GetDirectories(year);
                        if (months.Length == 0) continue;
                        months = SortByNumeric(months);

                        foreach (string month in months)
                        {
                            if (CancelControl.cancel) return (count);
                            if (m_Stop) return count;

                            int monthInt;
                            try
                            {
                                monthInt = Convert.ToInt32(GetEndofString(month));
                            }
                            catch { continue; }
                            timeAtCurrentDirectory = new DateTime(yearInt, monthInt, 1, 0, 0, 0);
                            if (timeAtCurrentDirectory.CompareTo(startMonth) >= 0 && timeAtCurrentDirectory.CompareTo(stopMonth) <= 0)
                            {
                                string[] days = Directory.GetDirectories(month);
                                if (days.Length == 0) continue;
                                days = SortByNumeric(days);

                                foreach (string day in days)
                                {
                                    if (CancelControl.cancel) return (count);
                                    if (m_Stop) return count;

                                    int dayInt ;
                                    try
                                    {
                                        dayInt = Convert.ToInt32(GetEndofString(day));
                                    }
                                    catch { continue; }
                                    timeAtCurrentDirectory = new DateTime(yearInt, monthInt, dayInt, 0, 0, 0);
                                    if (timeAtCurrentDirectory.CompareTo(startDay) >= 0 && timeAtCurrentDirectory.CompareTo(stopDay) <= 0)
                                    {
                                        string[] hours = Directory.GetDirectories(day);
                                        if (hours.Length == 0) continue;
                                        hours = SortByNumeric(hours);

                                        foreach (string hour in hours)
                                        {
                                            if (CancelControl.cancel) return (count);
                                            if (m_Stop) return count;

                                            int hourInt;
                                            try
                                            {
                                                hourInt = Convert.ToInt32(GetEndofString(hour));
                                            }
                                            catch { continue; }
                                            timeAtCurrentDirectory = new DateTime(yearInt, monthInt, dayInt, hourInt, 0, 0);
                                            if (timeAtCurrentDirectory.CompareTo(startHour) >= 0 && timeAtCurrentDirectory.CompareTo(stopHour) <= 0)
                                            {
                                                string[] PSSUnitsNode = Directory.GetDirectories(hour);
                                                if (PSSUnitsNode.Length == 0) continue;

                                                foreach (string pss in PSSUnitsNode)
                                                {
                                                    if (CancelControl.cancel) return (count);
                                                    if (m_Stop) return count;

                                                    string[] sources = Directory.GetDirectories(pss);
                                                    foreach (string source in sources)
                                                    {
                                                        if (m_Stop) return count;
                                                        if (CancelControl.cancel) return (count);

                                                        string[] jpegs = Directory.GetFiles(source, "*.jpg");
                                                        string[] jpegsInRange = GetJpegsInMinutesRange(jpegs, start, stop);
                                                        if (jpegsInRange != null)
                                                        {
                                                            foreach (string jpeg in jpegsInRange)
                                                            {
                                                                count++;
                                                                if (count % statusIncrement == 0) statusUpdate(count, timeAtCurrentDirectory);
                                                            }
                                                        }

                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            { m_Log.Log("GetAllJpegsInRange ex " + ex.Message, ErrorLog.LOG_TYPE.FATAL); }

            return (count);
        }



        /// <summary>
        /// Return datetime of the first day that appears on the target drive
        /// </summary>
        /// <param name="drive"></param>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <param name="statusUpdate"></param>
        /// <param name="statusIncrement"></param>
        ///// <returns></returns>
        public DateTime GetFirstDay(string drive)
        {

            try
            {
                DateTime start = new DateTime(1901, 1, 1);
                DateTime stop = new DateTime(2209, 1, 1); // give it two hundred years of operation!!!


                DateTime timeAtCurrentDirectory = default(DateTime);

                DateTime startYear = new DateTime(start.Year, 1, 1, 0, 0, 0);
                DateTime startMonth = new DateTime(start.Year, start.Month, 1, 0, 0, 0);
                DateTime startDay = new DateTime(start.Year, start.Month, start.Day, 0, 0, 0);
                DateTime startHour = new DateTime(start.Year, start.Month, start.Day, start.Hour, 0, 0);
                DateTime startMin = new DateTime(start.Year, start.Month, start.Day, start.Hour, start.Minute, 0);
                DateTime startSec = new DateTime(start.Year, start.Month, start.Day, start.Hour, start.Minute, start.Second);

                DateTime stopYear = new DateTime(stop.Year, 1, 1, 0, 0, 0);
                DateTime stopMonth = new DateTime(stop.Year, stop.Month, 1, 0, 0, 0);
                DateTime stopDay = new DateTime(stop.Year, stop.Month, stop.Day, 0, 0, 0);
                DateTime stopHour = new DateTime(stop.Year, stop.Month, stop.Day, stop.Hour, 0, 0);
                DateTime stopMin = new DateTime(stop.Year, stop.Month, stop.Day, stop.Hour, stop.Minute, 0);
                DateTime stopSec = new DateTime(stop.Year, stop.Month, stop.Day, stop.Hour, stop.Minute, stop.Second);

                string[] years = Directory.GetDirectories(drive+StorageDir);
                years = RemoveNonYearDirs(years);

                if (years.Count() == 0) return default(DateTime);

                years = SortByNumeric(years);

                
                string year = years[0];
                {
                    int yearInt=0;
                    try
                    {
                        yearInt = Convert.ToInt16(GetEndofString(year));
                    }
                    catch
                    {
                    
                    }

                    timeAtCurrentDirectory = new DateTime(yearInt, 1, 1, 0, 0, 0);
                    if (timeAtCurrentDirectory.CompareTo(startYear) >= 0 && timeAtCurrentDirectory.CompareTo(stopYear) <= 0)
                    {
                        string[] months = Directory.GetDirectories(year);
                        if (months.Length == 0) return (timeAtCurrentDirectory);

                        months = SortByNumeric(months);

                        string month = months[0];
                        {
                            int monthInt;
                            try
                            {
                                monthInt = Convert.ToInt32(GetEndofString(month));
                            }
                            catch { return (timeAtCurrentDirectory); }

                            timeAtCurrentDirectory = new DateTime(yearInt, monthInt, 1, 0, 0, 0);
                            if (timeAtCurrentDirectory.CompareTo(startMonth) >= 0 && timeAtCurrentDirectory.CompareTo(stopMonth) <= 0)
                            {
                                string[] days = Directory.GetDirectories(month);
                                if (days.Length == 0) return (timeAtCurrentDirectory);

                                days = SortByNumeric(days);

                                string day = days[0];
                                {
                                    int dayInt;
                                    try
                                    {
                                        dayInt = Convert.ToInt32(GetEndofString(day));
                                    }
                                    catch { return (timeAtCurrentDirectory); }
                                    timeAtCurrentDirectory = new DateTime(yearInt, monthInt, dayInt, 0, 0, 0);
                                    return (timeAtCurrentDirectory);
                                }

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            { m_Log.Log("GetAllJpegsInRange ex " + ex.Message, ErrorLog.LOG_TYPE.FATAL); }

            return (default(DateTime));
        }

        /// <summary>
        /// Return datetime of the last day that appears on the target drive
        /// </summary>
        /// <param name="drive"></param>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <param name="statusUpdate"></param>
        /// <param name="statusIncrement"></param>
        ///// <returns></returns>
        public DateTime GetLastDay(string drive)
        {

           
            try
            {
                DateTime start = new DateTime(1901, 1, 1);
                DateTime stop = new DateTime(2209, 1, 1); // give it two hundred years of operation!!!


                DateTime timeAtCurrentDirectory = default(DateTime);

                DateTime startYear = new DateTime(start.Year, 1, 1, 0, 0, 0);
                DateTime startMonth = new DateTime(start.Year, start.Month, 1, 0, 0, 0);
                DateTime startDay = new DateTime(start.Year, start.Month, start.Day, 0, 0, 0);
                DateTime startHour = new DateTime(start.Year, start.Month, start.Day, start.Hour, 0, 0);
                DateTime startMin = new DateTime(start.Year, start.Month, start.Day, start.Hour, start.Minute, 0);
                DateTime startSec = new DateTime(start.Year, start.Month, start.Day, start.Hour, start.Minute, start.Second);

                DateTime stopYear = new DateTime(stop.Year, 1, 1, 0, 0, 0);
                DateTime stopMonth = new DateTime(stop.Year, stop.Month, 1, 0, 0, 0);
                DateTime stopDay = new DateTime(stop.Year, stop.Month, stop.Day, 0, 0, 0);
                DateTime stopHour = new DateTime(stop.Year, stop.Month, stop.Day, stop.Hour, 0, 0);
                DateTime stopMin = new DateTime(stop.Year, stop.Month, stop.Day, stop.Hour, stop.Minute, 0);
                DateTime stopSec = new DateTime(stop.Year, stop.Month, stop.Day, stop.Hour, stop.Minute, stop.Second);

                string[] years = Directory.GetDirectories( drive + StorageDir);
                if (years.Length == 0) return (default(DateTime));

                years = RemoveNonYearDirs(years);
                years = SortByNumeric(years);

                string year = years[years.Count() - 1];
                {
                    int yearInt=0;
                    try
                    {
                        yearInt = Convert.ToInt16(GetEndofString(year));
                    }
                    catch
                    {
                        
                    }

                    timeAtCurrentDirectory = new DateTime(yearInt, 1, 1, 0, 0, 0);
                    if (timeAtCurrentDirectory.CompareTo(startYear) >= 0 && timeAtCurrentDirectory.CompareTo(stopYear) <= 0)
                    {
                        string[] months = Directory.GetDirectories(year);
                        if (months.Length == 0) return (timeAtCurrentDirectory);

                        months = SortByNumeric(months);

                        string month = months[months.Count() - 1];
                        {
                            int monthInt;
                            try
                            {
                                monthInt = Convert.ToInt32(GetEndofString(month));
                            }
                            catch { return (timeAtCurrentDirectory); }
                            timeAtCurrentDirectory = new DateTime(yearInt, monthInt, 1, 0, 0, 0);
                            if (timeAtCurrentDirectory.CompareTo(startMonth) >= 0 && timeAtCurrentDirectory.CompareTo(stopMonth) <= 0)
                            {
                                string[] days = Directory.GetDirectories(month);
                                if (days.Length == 0) return (timeAtCurrentDirectory);

                                days = SortByNumeric(days);
                                string day = days[days.Count()-1];
                                {
                                    int dayInt;
                                    try
                                    {
                                        dayInt = Convert.ToInt32(GetEndofString(day));
                                    }
                                    catch { return (timeAtCurrentDirectory); }
                                    timeAtCurrentDirectory = new DateTime(yearInt, monthInt, dayInt, 0, 0, 0);
                                    return (timeAtCurrentDirectory);
                                }

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            { m_Log.Log("GetAllJpegsInRange ex " + ex.Message, ErrorLog.LOG_TYPE.FATAL); }

            return (default(DateTime));
        }

        string[] RemoveNonYearDirs(string [] years)
        {
            List<string> newArray = new List<string>();

            int yearInt = 0;
            foreach (string year in years)
            {
                try
                {
                    yearInt = Convert.ToInt16(GetEndofString(year));

                    newArray.Add(year); // if not valid year, hits catch not here
                }
                catch { }
            }

            return (newArray.ToArray());
        }


        /// <summary>
        /// Creats a complete destination path for a new destination drive.
        /// </summary>
        /// <param name="fd"></param>
        /// <param name="destinationDrive"></param>
        /// <returns></returns>
        public IMAGE_FILE_DATA GetCompleteFilePath(IMAGE_FILE_DATA srcFd, string destinationDrive)
        {
            lock (singleton)
            {
                int year;
                int month;
                int day;
                int hour;

                // STORAGE_DIR/Year/Month/Day/Hour/PSS_Name/Source_Name/YYYY_MM_DD_HH_MM_SS_mmm.jpg
                IMAGE_FILE_DATA newFd = new IMAGE_FILE_DATA();

                string[] sp = srcFd.fileNameOnly.Split('_');
                newFd.fileNameOnly = srcFd.fileNameOnly;
                newFd.timeStamp = srcFd.timeStamp;

                newFd.PSSName = srcFd.PSSName;
                newFd.SourceName = srcFd.SourceName;
                
                    
                year =  srcFd.timeStamp.Year;
                month = srcFd.timeStamp.Month;
                day = srcFd.timeStamp.Day;
                hour = srcFd.timeStamp.Hour;

                if (srcFd.SourceName != null) // jpeg file case
                {
                    newFd.relativePath = StorageDir + "\\" +
                                    year.ToString() + "\\" +
                                    month.ToString() + "\\" +
                                    day.ToString() + "\\" +
                                    hour.ToString() + "\\" +
                                    srcFd.PSSName + "\\" +
                                    srcFd.SourceName + "\\";
                }
                else
                {  // eventlog.txt file case
                    newFd.relativePath = StorageDir + "\\" +
                                   year.ToString() + "\\" +
                                   month.ToString() + "\\" +
                                   day.ToString() + "\\" +
                                   hour.ToString() + "\\" +
                                   srcFd.PSSName + "\\";
                }


                string destDir = null;

                if (srcFd.SourceName != null) // jpeg file case
                {
                    destDir = destinationDrive + StorageDir + "\\" +
                                   year.ToString() + "\\" +
                                   month.ToString() + "\\" +
                                   day.ToString() + "\\" +
                                   hour.ToString() + "\\" +
                                   srcFd.PSSName + "\\" +
                                   srcFd.SourceName + "\\";
                }
                else
                {           // eventlog.txt file case
                    destDir = destinationDrive + StorageDir + "\\" +
                                 year.ToString() + "\\" +
                                 month.ToString() + "\\" +
                                 day.ToString() + "\\" +
                                 hour.ToString() + "\\" +
                                 srcFd.PSSName + "\\";
                }
                
                newFd.dirOnly = destDir;

                newFd.completePath  = destDir + srcFd.fileNameOnly;
               
                return (newFd);
            }
        }

        /// <summary>
        /// supply a file relative path(does not include the driver and storage directory) and get back the complete path
        /// </summary>
        /// <param name="relativeFileName"></param>
        /// <returns></returns>
        public string GetCompleteFilePath(string relativeFileName)
        {
            return (StoragePath + "\\" + relativeFileName);
        }

        public string TempPath
        {
            get
            {
                lock (singleton)
                {
                    return (drive + StorageDir + "\\" + TempDir);
                }
            }
        }
        public string GetTempDirectory()
        {

            CreateTempDirectory();

            return (TempPath);

        }


        void CreateTempDirectory()
        {
            if (!Enabled) return;

            try
            {

                if (!Directory.Exists(TempPath))
                {
                    Directory.CreateDirectory(TempPath);
                }
            }
            catch (Exception ex)
            {
                m_Log.Log("CreateTempDirectory ex: " + ex.Message, ErrorLog.LOG_TYPE.FATAL);
            }
        }

        /// <summary>
        /// Supply a list of jpeg file names (no paths), start and stop time range,and get back a list which is a subset of the files which meet the time constraint.
        /// </summary>
        /// <param name="jpegs">jpeg file names with no paths</param>
        /// <param name="startTime"></param>
        /// <param name="stopTime"></param>
        /// <returns></returns>
        string[] GetJpegsInMinutesRange(string[] jpegs, DateTime startTime, DateTime stopTime)
        {
            List<string> imageList = new List<string>();
          
            int count = 0;

            foreach (string file in jpegs)
            {
                if (m_Stop) return null;
                try
                {
                    string jp = GetEndofString(file);
                    string timePart = jp.Split('.')[0].Replace('_', ':');
                    DateTime time = ParseDateTime(timePart);

                    if (time.CompareTo(startTime) >= 0 && time.CompareTo(stopTime) <= 0)
                    {

                        imageList.Add(file);

                    }
                    count++;

                }
                catch (Exception ex)
                {
                    m_Log.Log("GetJpegsInMinutesRange ex : " + ex.Message, ErrorLog.LOG_TYPE.INFORMATIONAL);
                }
            }
            return (imageList.ToArray());
        }


        /// <summary>
        /// Supply the jpeg file name (no path) and get back the time stamp parsed from the file name
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static DateTime GetTimeFromFile(string file)
        {
            string s = GetEndofString(file);
            string[] sp1 = s.Split('.');
            string timestring = sp1[0];
            string timePart = timestring.Split('.')[0].Replace('_', ':');
            DateTime time = ParseDateTime(timePart);
            return (time);
        }

        /// <summary>
        /// Supply a time string with the ':' delimiter and get back a DateTime
        /// </summary>
        /// <param name="timestring"></param>
        /// <returns></returns>
        public static DateTime ParseDateTime(string timestring)
        {
            // 2009:07:16:12:47:11:9220
            try
            {
                string[] sp1 = timestring.Split(':');
                int year = Convert.ToInt32(sp1[0]);
                int month = Convert.ToInt32(sp1[1]);
                int day = Convert.ToInt32(sp1[2]);
                int hour = Convert.ToInt32(sp1[3]);
                int min = Convert.ToInt32(sp1[4]);
                int sec = Convert.ToInt32(sp1[5]);

                // this double to int is because if you convert 0462 to Int and 462 to int, both are 462. But its really 0.0462 and 0.462
                double msecf = Convert.ToDouble("."+sp1[6]);
                int msec = (int)(msecf * 1000.0);
                

                DateTime result = new DateTime(year, month, day, hour, min, sec, msec);
                return (result);
            }
            catch{return default(DateTime);}
        }

        /// <summary>
        /// holds all the data that can be derived from a jpeg file's compete path
        /// </summary>
        public class IMAGE_FILE_DATA
        {
            public string completePath;
            public string dirOnly;
            public string fileNameOnly;
            public string PSSName;
            public string SourceName;
            public string relativePath;
            public DateTime timeStamp;
        }

        /// <summary>
        /// supply the complete path, and get back the parsed info from the path structure
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public IMAGE_FILE_DATA ParseFileCompleteLogfilePath(string path)
        {

            //D:\DVRSTORAGE\2009\8\22\20\David-PC17\EVENTLOG.TXT

            try
            {
                string[] pathSegments = path.Split('\\');

                int startingIndex = 0;

              
                // find the DVRSTORAGE segment
                for (int i = 0; i < pathSegments.Count(); i++)
                {
                    if (pathSegments[i].Contains(StorageDir))
                    {
                        startingIndex = i;
                        break;
                    }
                }
                IMAGE_FILE_DATA data = new IMAGE_FILE_DATA();
                data.completePath = path;
                data.PSSName = pathSegments[startingIndex + 5];
              
               
                data.fileNameOnly = pathSegments[startingIndex + 6];
                data.dirOnly = path.Replace(pathSegments[startingIndex + 6], "");


                int year = Convert.ToInt32(pathSegments[startingIndex + 1]);
                int month = Convert.ToInt32(pathSegments[startingIndex + 2]);
                int day = Convert.ToInt32(pathSegments[startingIndex + 3]);
                int hour = Convert.ToInt32(pathSegments[startingIndex + 4]);

                data.timeStamp = new DateTime(year, month, day, hour, 0, 0);

                char[] trimLeadingChar = { '\\' };

                string removeDriveColon = path.Replace(pathSegments[0], "").TrimStart(trimLeadingChar);
                data.relativePath = removeDriveColon.Replace(StorageDir + "\\", "");

                return (data);
            }
            catch (Exception ex) { m_Log.Trace(ex, ErrorLog.LOG_TYPE.INFORMATIONAL); return (null); }

        }

        /// <summary>
        /// supply the complete path, and get back the parsed info from the path structure
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public IMAGE_FILE_DATA ParseFileCompleteJpegPath(string path)
        {
            //F:\DVRSTORAGE\2009\8\19\17\k000a9df304bf\1\2009_08_19_17_03_46_4062690.jpg

            try
            {
                string[] pathSegments = path.Split('\\');

                int startingIndex = 0;

                // find the DVRSTORAGE segment
                for (int i = 0; i < pathSegments.Count(); i++)
                {
                    if (pathSegments[i].Contains(StorageDir))
                    {
                        startingIndex = i;
                        break;
                    }
                }
                IMAGE_FILE_DATA data = new IMAGE_FILE_DATA();
                data.completePath = path;
                data.PSSName = pathSegments[startingIndex + 5];
                data.SourceName = pathSegments[startingIndex + 6];
                data.fileNameOnly = pathSegments[startingIndex + 7];
                data.dirOnly = path.Replace(pathSegments[startingIndex + 7], "");
                data.timeStamp = GetTimeFromFile(pathSegments[startingIndex + 7]);

                char[] trimLeadingChar  = {'\\'};

                string removeDriveColon = path.Replace(pathSegments[0], "").TrimStart(trimLeadingChar);

                int offset = path.IndexOf(StorageDir);
                data.relativePath = path.Substring(offset+StorageDir.Length); // get the string after the "DVRSTORAGE"
                data.relativePath = data.relativePath.Remove(0, 1);// remove the leading '\' character

                return (data);
            }
            catch (Exception ex) { m_Log.Trace(ex, ErrorLog.LOG_TYPE.INFORMATIONAL); return (null); }

        }

        /// <summary>
        /// Returns a string array of PSS Names. 
        /// </summary>
        /// <param name="PSSNodePaths">Outputs the full paths for each PSS Node</param>
        /// <returns></returns>
        public string[] GetPSSNodes(out string[] PSSNodePaths)
        {
            // frame storage file path =  "f:\\DVRSTORAGE\\2009\\0809\\15\\PSS_01\\Camera_01
            // StoragePath  = "f:\\DVRSTORAGE"
            List<string> PSSUnits = new List<string>();
            List<string> PSSPaths = new List<string>();
            PSSNodePaths = null;

            if (StoragePath == null)
            {
                return null;
            }

            // open the years
            string[] years = Directory.GetDirectories(StoragePath);
            if (years.Length == 0) return null;

            foreach (string year in years)
            {
                string[] months = Directory.GetDirectories(year);
                if (months.Length == 0) return null;

                foreach (string month in months)
                {
                    string[] days = Directory.GetDirectories(month);
                    if (days.Length == 0) return null;

                    foreach (string day in days)
                    {
                        string[] hours = Directory.GetDirectories(day);
                        if (hours.Length == 0) return null;

                        foreach (string hour in hours)
                        {
                            string[] PSSUnitsNode = Directory.GetDirectories(hour);
                            if (PSSUnitsNode.Length == 0) return null;

                            foreach (string pss in PSSUnitsNode)
                            {
                                string[] pssPathSegments = pss.Split('\\');
                                string name = pssPathSegments[pssPathSegments.Length - 1];
                                if (!PSSUnits.Contains(name))
                                {
                                    PSSPaths.Add(pss);
                                    PSSUnits.Add(name);
                                }
                            }
                        }
                    }
                }
            }
            PSSNodePaths = PSSPaths.ToArray();
            return (PSSUnits.ToArray());
        }

        /// <summary>
        /// Supply a path to a PSS Directory, and get back all channel sources
        /// </summary>
        /// <param name="PSSdir"></param>
        /// <returns></returns>
        public string[] GetCameraSourceNames(string PSSdir)
        {
            string[] sourcePaths = Directory.GetDirectories(PSSdir);
            List<string> Sources = new List<string>();

            foreach (string sourcePath in sourcePaths)
            {
                string[] namePathSegments = sourcePath.Split('\\');
                string name = namePathSegments[namePathSegments.Length - 1];
                if (!Sources.Contains(name))
                {
                    Sources.Add(name);
                }
            }
            return (Sources.ToArray());
        }

        /// <summary>
        /// Returns the a list of all drive mounts which contain first evidence image stores
        /// </summary>
        /// <returns></returns>
        public string[] GetAllAttachedFrameStoreDrives()
        {
            List<string> stores = new List<string>();

            // get the list of existing drives

            string[] sa = Environment.GetLogicalDrives();

            foreach (string drive in sa)
            {

                if (Directory.Exists(drive + "\\" + StorageDir))
                {
                    stores.Add(drive);
                }

            }
            return (stores.ToArray());
        }

        public string GetOldestDayDir(string drive)
        {
           return( _GetOldestDayDir(drive + StorageDir));
        }

        public string GetOldestDayDir()
        {
            return (_GetOldestDayDir(StoragePath));
        }

        public string _GetOldestDayDir(string storagePath)
        {
            // first get oldest year

            string oldestYear = GetOldestSub(storagePath);
            if (oldestYear == null) return (null);
            // get oldest month

            string oldestMonth = GetOldestSub(oldestYear);
            if (oldestMonth == null) return (oldestYear);

            // get oldest day

            string oldestDay = GetOldestSub(oldestMonth);
            if (oldestDay == null) return (oldestMonth);

            return (oldestDay);
        }

        string GetOldestSub(string start)
        {
            if (start == null) return null;

            if (!Enabled) return null;

            string[] subDirectories = null;
            try
            {
                subDirectories = Directory.GetDirectories(start);
            }
            catch { return null; }


            List<string> namesThatAreValidInts = new List<string>();

            int val;

            foreach (string sub in subDirectories)
            {
                bool validInt = true;
                try
                {
                    val = Convert.ToInt16(GetEndofString(sub));
                }
                catch { validInt = false; }

                if (validInt)
                    namesThatAreValidInts.Add(sub);
            }

            subDirectories = namesThatAreValidInts.ToArray();
            if (subDirectories.Length == 0) return (null);
            subDirectories = SortByNumeric(subDirectories);
            if (subDirectories == null) return (null);

            string oldestSub = subDirectories[0];

            return (oldestSub);
        }

        /// <summary>
        /// 
        /// input an array of file names where the file names are time_stamps such as: path/2009_09_14_19_05_37_68101156.jpg
        /// </summary>
        /// <param name="inArray"></param>
        /// <returns></returns>
        string[] SortFilesByTimeStamp(string[] inArray)
        {
            

            CompareFileNameTimeStamp comparer = new CompareFileNameTimeStamp();
            Array.Sort(inArray, (IComparer)comparer);

            return (inArray);
        }

        class CompareFileNameTimeStamp : IComparer
        {
            int IComparer.Compare(object a, object b)
            {
                string aString = (string)a;
                string bString = (string)b;

                DateTime aTime = GetTimeFromFile(aString);
                DateTime bTime = GetTimeFromFile(bString);

                if (aTime.CompareTo(bTime) > 0 )
                    return 1;

                else if (aTime.CompareTo(bTime) < 0)
                    return -1;

                else
                    return 0;
            }
        }

        string[] SortByNumeric(string[] inArray)
        {
            CompareStringNumeric comparer = new CompareStringNumeric();
            Array.Sort(inArray, (IComparer)comparer);

            return (inArray);
        }

        /// <summary>
        /// used to compare path strings which end in years/months/days/hours, converting end of the string to an int for numeric comparison
        /// </summary>
        class CompareStringNumeric:IComparer 
        {
            int IComparer.Compare(object a, object b)
            {
                int ai;
                int bi;
                try
                {
                    ai = Convert.ToInt32(GetEndofString((string)a));
                    bi = Convert.ToInt32(GetEndofString((string)b));
                }
                catch { return(0);}

                if ( ai > bi)
                    return 1;

                if (ai < bi)
                    return -1;

                else
                    return 0;
            }


        }
    }


}
