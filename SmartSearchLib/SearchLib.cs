using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using LPROCR_Wrapper;
using System.Threading;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using ApplicationDataClass;
using PathsLib;
using EventLogFiles;
using ErrorLoggingLib;

namespace SmartSearchLib
{
    public class SearchLib
    {

        public enum SEARCH_ERROR_CODES : int
        {
            TOO_MANY_ENTRIES,
            LOG_FILE_PARSE_ERROR,
            FILE_OPEN_EXCEPTION,
            NO_FILES_FOUND,
            NO_ERROR
            
        }

        public class SEARCH_STATUS
        {
            public SEARCH_STATUS()
            {
                errorCode = SEARCH_ERROR_CODES.NO_ERROR;
            }
            public SearchLib.SEARCH_PHASE phase;
            public int totalCount;
            public int currentCount;
            public DateTime currentTime;
            public DateTime startTime;
            public DateTime endTime;
            public string errorString;
            public SEARCH_ERROR_CODES errorCode;
        }

        public SearchLib(APPLICATION_DATA appData)
        {
            m_SearchStopped = true;
            m_AppData = appData;
            m_PathManager =(PATHS) m_AppData.PathManager;

            m_EventLogs = new EventLogFiles.EventLogFiles(m_AppData);
        }


        public enum SEARCH_ERRORS
        {
            NO_ERROR,
            ERROR_OPENING_FILE,
            PATH_ERROR,
            EXCEPTION
        }

        public enum SEARCH_PHASE : int
        {
            FINDING_ITEMS_IN_TIME_RANGE,
            COMPARING_STRINGS,
            LOADING_RESULTS,
            COMPLETE
        }

        public class SEARCH_RESULT
        {
            public string searchString;
            public string matchedString;
            public int percentMatch;
            public EventLogFiles.EventLogFiles.PlateEventData plateData;
        }

        APPLICATION_DATA m_AppData;
        PATHS m_PathManager;

        public delegate void SEARCH_PROGRESS_DEL(SEARCH_STATUS status);
        public delegate void SEARCH_COMPLETE_DEL(List<SEARCH_RESULT> results);

        SEARCH_PROGRESS_DEL m_SearchProgressCB;
        SEARCH_COMPLETE_DEL m_SearchCompleteCB;

        EventLogFiles.EventLogFiles m_EventLogs;

        bool m_Stop = false;

        Thread m_SearchThread;

        string m_SearchString;
        string m_CameraNameFilter;
        DateTime m_SearchStartTime;
        DateTime m_SearchEndTime;
        int m_MinMatchScore;
        bool m_SearchStopped = true;
        SEARCH_TYPE m_SearchType;

   

        public void StopSearch()
        {
            m_Stop = true;
        }

        public enum SEARCH_TYPE { PLATE, MOTION, ALL_IMAGES }

        public void SearchForPlate( SEARCH_TYPE type, string searchNumber, string cameraNameFilter, DateTime start, DateTime end, int minMatchScore, SEARCH_PROGRESS_DEL progressCallBack, SEARCH_COMPLETE_DEL completeCB)
        {
            m_SearchStopped = false;
            m_MinMatchScore = minMatchScore;
            m_SearchProgressCB = progressCallBack;
            m_SearchCompleteCB = completeCB;

            m_CameraNameFilter = cameraNameFilter;

            m_SearchString = searchNumber;
            m_SearchStartTime = start;
            m_SearchEndTime = end;
            m_SearchType = type;

            if (m_SearchType == SEARCH_TYPE.MOTION || m_SearchType == SEARCH_TYPE.PLATE)
            {
                m_SearchThread = new Thread(SearchFilesLoop);
            }
            else if (m_SearchType == SEARCH_TYPE.ALL_IMAGES)
            {
                m_SearchThread = new Thread(DumpImagesInRangeLoop);
            }

            m_Stop = false;

            m_SearchThread.Start();
        }

        public bool SearchIsStopped()
        {
            return (m_SearchStopped);
        }

        /// <summary>
        ///  handles status updates from PATHS.GetAllJpegsInRange()
        /// </summary>
        /// <param name="getStatus"></param>
        void HandleDumpAllJpegsProgressUpdate(PATHS.PATHS_GET_STATUS getStatus)
        {
            SEARCH_STATUS status = new SEARCH_STATUS();
            status.phase = SEARCH_PHASE.FINDING_ITEMS_IN_TIME_RANGE;
            status.currentCount = 0;
            status.totalCount = 0;
            status.currentTime = getStatus.currentTime;
            status.endTime = m_SearchEndTime;
            status.startTime = m_SearchStartTime;
            status.errorString = getStatus.errors;
            if (getStatus.errors != null)
            {
                status.errorCode = SEARCH_ERROR_CODES.LOG_FILE_PARSE_ERROR;
            }

            m_SearchProgressCB(status);
        }

        void DumpImagesInRangeLoop()
        {
            int ReportStatusFileCountInterval = 200;
          
            List<SEARCH_RESULT> results = new List<SEARCH_RESULT>();

            SEARCH_STATUS status = new SEARCH_STATUS();
            status.phase = SEARCH_PHASE.FINDING_ITEMS_IN_TIME_RANGE;
            status.totalCount = 0;
            status.currentCount = 0;
            status.currentTime = m_SearchStartTime;
            status.endTime = m_SearchEndTime;
            status.startTime = m_SearchStartTime;
            status.errorString = null;
            m_SearchProgressCB(status);


            // get the list of log files in the search range
            PATHS.GET_ALL_JPEGS_IN_RANGE_PARAMS jpegGetParams = new PATHS.GET_ALL_JPEGS_IN_RANGE_PARAMS();
            jpegGetParams.start = m_SearchStartTime;
            jpegGetParams.stop = m_SearchEndTime;
            jpegGetParams.callBack = HandleDumpAllJpegsProgressUpdate;
            jpegGetParams.updateFreq = ReportStatusFileCountInterval;
            jpegGetParams.stopSearch = (object)m_Stop;
            jpegGetParams.maxResultsCount = m_AppData.MAX_SEARCH_RESULTS_TO_DISPLAY;
            string[] allFilesInRange = m_PathManager.GetAllJpegsInRange(jpegGetParams); 
            if (allFilesInRange == null)
            {
                status = new SEARCH_STATUS();
                status.totalCount = 0;
                status.currentCount = 0;
                status.currentTime = m_SearchEndTime;
                status.endTime = m_SearchEndTime;
                status.startTime = m_SearchStartTime;
                status.errorString = "No files found in time range";
                status.errorCode = SEARCH_ERROR_CODES.NO_FILES_FOUND;
                m_SearchProgressCB(status);

                m_SearchStopped = true;
                m_SearchCompleteCB(null);
                return;
            }

            if (allFilesInRange.Length > m_AppData.MAX_SEARCH_RESULTS_TO_DISPLAY)
            {
                status = new SEARCH_STATUS();
                status.totalCount = 0;
                status.currentCount = 0;
                status.currentTime = m_SearchEndTime;
                status.endTime = m_SearchEndTime;
                status.startTime = m_SearchStartTime;
                status.errorCode = SEARCH_ERROR_CODES.TOO_MANY_ENTRIES;
                status.errorString = "Exceeded maximum search results("+m_AppData.MAX_SEARCH_RESULTS_TO_DISPLAY.ToString()+"), reduce search range";
                m_SearchProgressCB(status);

                m_SearchStopped = true;
                m_SearchCompleteCB(null);
                return;
            }

            // notify user going to next phase

           
            status.phase = SEARCH_PHASE.COMPARING_STRINGS;
            status.totalCount = allFilesInRange.Count();
            status.currentCount = 0;
            status.currentTime = m_SearchStartTime;
            status.endTime = m_SearchEndTime;
            status.startTime = m_SearchStartTime;
            status.errorString = null;
            m_SearchProgressCB(status);

            // now do filter comparisons on all data
            int fileCount = 0;
            foreach (string file in allFilesInRange)
            {
                if (m_Stop) break;

                PATHS.IMAGE_FILE_DATA fileData = m_PathManager.ParseFileCompleteJpegPath (file);

               
                if (fileData == null) continue;

                EventLogFiles.EventLogFiles.PlateEventData pd = new EventLogFiles.EventLogFiles.PlateEventData();
                pd.eventType = EventLogFiles.EventLogFiles.EVENT_TYPE.NON_EVENT;
                pd.GPSLatitude = "No Position Available";
                pd.GPSLongitude = "No Position Available";
                pd.jpegRelativeFilePath = "";
                pd.PSSName = fileData.PSSName;
                pd.sourceChannelName = fileData.SourceName;
                pd.timeStamp = fileData.timeStamp;
                pd.jpegRelativeFilePath = fileData.relativePath;

                // if a camera name filter has been given, only include if there is a match, else include all entries

                if (m_CameraNameFilter.Length > 0)
                {
                    if (fileData.SourceName.ToUpper().Contains(m_CameraNameFilter.ToUpper()))
                    {
                        SEARCH_RESULT r = MakeSearchResult(pd, 0, "", "");
                        results.Add(r);
                    }
                }
                else
                {
                        SEARCH_RESULT r = MakeSearchResult(pd, 0, "", "");
                        results.Add(r);
                }

                if (fileCount++ % ReportStatusFileCountInterval == 0)
                {
                    
                    status.currentCount = fileCount;
                    status.currentTime = pd.timeStamp;
                   
                    status.errorString = null;
                    m_SearchProgressCB(status);
                }
            }

        

            m_SearchStopped = true;
            m_SearchCompleteCB(results);
        }



        /// <summary>
        /// search event log files for plates or motion events
        /// </summary>

        void SearchFilesLoop()
        {
            
            int score;
            int counter = 0;
            DateTime lastTimeSearched = default(DateTime);

            List<SEARCH_RESULT> results = new List<SEARCH_RESULT>();

            SEARCH_STATUS status = new SEARCH_STATUS();
            status.phase = SEARCH_PHASE.FINDING_ITEMS_IN_TIME_RANGE;
            status.totalCount = 0;
            status.currentCount = 0;
            status.currentTime = m_SearchStartTime;
            status.endTime = m_SearchEndTime;
            status.startTime = m_SearchStartTime;
            status.errorString = null;
            m_SearchProgressCB(status);


            // get the list of log files in the search range
            string[] allLogFilesInRange = m_PathManager.GetAllLogFilesInRange(m_SearchStartTime, m_SearchEndTime);
            if (allLogFilesInRange == null)
            {
                status.errorCode = SEARCH_ERROR_CODES.NO_FILES_FOUND;
                status.errorString = "No event logs found in time range";
                m_SearchProgressCB(status);
                m_SearchStopped = true;
                m_SearchCompleteCB(null);
                return;
            }

            // notify user going to next phase

          
            status.phase = SEARCH_PHASE.COMPARING_STRINGS;
            status.totalCount = allLogFilesInRange.Count();
            status.currentCount = 0;
            status.currentTime = m_SearchStartTime;
            status.errorString = null;
            m_SearchProgressCB(status);


            // go through the logs one by one, extracting all plate numbers
            foreach (string logFile in allLogFilesInRange)
            {
                try
                {
                    string[] lines = File.ReadAllLines(logFile);
                    foreach (string line in lines)
                    {

                        // search all the extracted plate numbers and compare to the search key

                        EventLogFiles.EventLogFiles.PlateEventData pd=null;
                        try
                        {
                            pd = m_EventLogs.ParseEventLogLine(line);
                        }
                        catch 
                        {
                            status.currentCount = counter;               
                            status.errorString = "LogFile Parse error, file = " + logFile + ", line = " + line;
                            m_SearchProgressCB(status);

                            continue;
                        }

                        if (pd.eventType == EventLogFiles.EventLogFiles.EVENT_TYPE.MOTION && m_SearchType == SEARCH_TYPE.MOTION)
                        {
                            // if a camera name filter has been given, only search if there is a match, else search all entries

                            if (m_CameraNameFilter.Length > 0)
                            {
                                lastTimeSearched = pd.timeStamp;
                                if (pd.sourceChannelName.ToUpper().Contains(m_CameraNameFilter.ToUpper()))
                                {
                                    SEARCH_RESULT r = MakeSearchResult(pd, 0, "", "");
                                    results.Add(r);
                                    if (results.Count > m_AppData.MAX_SEARCH_RESULTS_TO_DISPLAY)
                                    {
                                        break;
                                    }

                                }
                            }
                            else
                            {
                                SEARCH_RESULT r = MakeSearchResult(pd, 0, "", "");
                                results.Add(r);
                                if (results.Count > m_AppData.MAX_SEARCH_RESULTS_TO_DISPLAY)
                                {
                                    break;
                                }
                            }
                           
                        }
                        else if (pd.eventType == EventLogFiles.EventLogFiles.EVENT_TYPE.PLATE && m_SearchType == SEARCH_TYPE.PLATE)
                        {
                            // if a camera name filter has been given, only search if there is a match, else search all entries
                            if (m_CameraNameFilter.Length > 0)
                            {
                                if (pd.sourceChannelName.ToUpper().Contains(m_CameraNameFilter.ToUpper()))
                                {

                                    foreach (string plateString in pd.plateNumbersLatinEquivalent)
                                    {
                                        if (pd.timeStamp.CompareTo(m_SearchStartTime) >= 0 && pd.timeStamp.CompareTo(m_SearchEndTime) <= 0)
                                        {
                                            lastTimeSearched = pd.timeStamp;
                                            score = LPROCR_Wrapper.LPROCR_Lib.scoreMatch(m_SearchString, plateString);
                                            if (score >= m_MinMatchScore)
                                            {
                                                SEARCH_RESULT r = MakeSearchResult(pd, score, plateString, m_SearchString);
                                                results.Add(r);
                                                if (results.Count > m_AppData.MAX_SEARCH_RESULTS_TO_DISPLAY)
                                                {
                                                    break;
                                                }
                                            }
                                        }
                                        if (m_Stop) break;
                                    }
                                }
                            }
                            else
                            {
                                foreach (string plateString in pd.plateNumbersLatinEquivalent)
                                {
                                    if (pd.timeStamp.CompareTo(m_SearchStartTime) >= 0 && pd.timeStamp.CompareTo(m_SearchEndTime) <= 0)
                                    {
                                        lastTimeSearched = pd.timeStamp;
                                        score = LPROCR_Wrapper.LPROCR_Lib.scoreMatch(m_SearchString, plateString);
                                        if (score >= m_MinMatchScore)
                                        {
                                            SEARCH_RESULT r = MakeSearchResult(pd, score, plateString, m_SearchString);
                                            results.Add(r);
                                            if (results.Count > m_AppData.MAX_SEARCH_RESULTS_TO_DISPLAY)
                                            {
                                                break;
                                            }
                                        }
                                    }
                                    if (m_Stop || results.Count > m_AppData.MAX_SEARCH_RESULTS_TO_DISPLAY) break;
                                }

                            }

                        }
                        if (m_Stop || results.Count > m_AppData.MAX_SEARCH_RESULTS_TO_DISPLAY) break;
                    }

                    // if a match is found over threshhold, add the meta data to the results list
                    if (m_Stop || results.Count > m_AppData.MAX_SEARCH_RESULTS_TO_DISPLAY) break;
                }
                catch (Exception ex)
                {
                    status.currentTime = lastTimeSearched;
                    status.currentCount = counter;
                    status.errorString = ex.Message;
                    m_SearchProgressCB(status);
                }

                counter++;
              
                status.currentCount = counter;
                status.currentTime = lastTimeSearched;
                status.errorString = null;
               

                m_SearchProgressCB(status);

                if (m_Stop || results.Count > m_AppData.MAX_SEARCH_RESULTS_TO_DISPLAY) break;
            }

            if (results.Count > m_AppData.MAX_SEARCH_RESULTS_TO_DISPLAY)
            {
                status.errorCode = SEARCH_ERROR_CODES.TOO_MANY_ENTRIES;
                status.errorString = "Exceeded maximum search results(" + m_AppData.MAX_SEARCH_RESULTS_TO_DISPLAY.ToString() + "), reduce search range";
                m_SearchProgressCB(status);
            }

            m_SearchStopped = true;
            m_SearchCompleteCB(results);
        }


        string MakeOneString(string[] array)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string s in array)
            {
                sb.Append(s);
                sb.Append(", ");
            }
            return (sb.ToString());
        }

        SEARCH_RESULT MakeSearchResult(EventLogFiles.EventLogFiles.PlateEventData pd, int score, string plateString, string searchString)
        {
            SEARCH_RESULT r = new SEARCH_RESULT();
            r.plateData = pd;
            r.matchedString = plateString;
            r.searchString = searchString;
            r.percentMatch = score;
            return (r);
        }
    }
}
