using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PathsLib;
using ApplicationDataClass;
using FrameGeneratorLib;
using System.IO;

namespace EventLogFiles
{

    public class EventLogFiles
    {
        public EventLogFiles(APPLICATION_DATA appData)
        {
            m_AppData = appData;
            m_PathManager =(PATHS) m_AppData.PathManager;
            lineFields = new string[6];

            m_FileLock = new object();
        }

        APPLICATION_DATA m_AppData;
       
        PATHS m_PathManager;
     
        string[] lineFields;
        enum FIELDS_ON_BUILDING_STRING { EVENT = 0, PSS_NAME, TIME_STAMP, SOURCE_CAMERA_NAME, GPS_LOCATION, JPEGFILE_RELATIVE_PATH }
        enum FIELDS_ON_PARSING_STRING { EVENT = 0, PLATE_DATA, PSS_NAME, TIME_STAMP, SOURCE_CAMERA_NAME, GPS_LAT, GPS_LON, JPEGFILE_RELATIVE_PATH }

        object m_FileLock;

        public class EVENT_TO_WRITE
        {
            public string file;
            public string line;
            public string directory;
        }

      //  MOTION,,k000a9df304bf,2009_08_19_17_04_05_5937,1,N30.5091966666667,W97.7381316666667,2009\8\19\17\k000a9df304bf\1\2009_08_19_17_04_05_59371992.jpg,
        //  MOTION,,k000a9df304bf,2009_08_18_19_29_50_3593,1,No Position Available,2009\8\18\19\k000a9df304bf\1\2009_08_18_19_29_50_35933523.jpg,
        //PLATE,LATIN^3ACNT:34CN71^3ACNT:34CN71,David-PC17,2009_07_24_15_15_33_6011,channel 1,No Position Available,2009\7\24\15\David-PC17\channel 1\2009_07_24_15_15_33_6011.jpg,
        //PLATE,LATIN^33CV^33CV,David-PC17,2009_07_24_14_58_08_7671,channel 1,No Position Available,2009\7\24\14\David-PC17\channel 1\2009_07_24_14_58_08_7671.jpg,

        public enum EVENT_TYPE {PLATE, MOTION, NON_EVENT }
        public class PlateEventData
        {
            public EVENT_TYPE eventType;
            public string PlateLanguage;
            public string[] plateNumbersNativeLanguage;
            public string[] plateNumbersLatinEquivalent;
            public string PSSName;
            public DateTime timeStamp;
            public string sourceChannelName;
            public string GPSLatitude;
            public string GPSLongitude;
            public string jpegRelativeFilePath;
        }

        string[] CheckForBadlyFormedGPSStrings(string[] instrings)
        {
            // I had an error that if no GPS input then the string contained "No Position Available", when it should have contained
            //   "No Position Available, No Position Available"  - one for Lon and one for Lat. This messes up the parsing.
            //  But I need to be able to parse all the old strings which were badly formed, so fix them here.

            int count = 0;
            foreach (string s in instrings)
            {
                string test = s.ToLower();
                string compareto = "no position available";
                if (test.Contains(compareto))
                    count++;
            }

            if (count == 1) // should be either 0 or 2
            {
                List<string> newlist = new List<string>();

                // fix it
                foreach (string s in instrings)
                {
                    string test = s.ToLower();
                    string compareto = "no position available";
                    if (test.Contains(compareto))
                    {
                        newlist.Add("No Position Available");
                        newlist.Add("No Position Available");
                    }
                    else
                        newlist.Add(s);
                }

                return (newlist.ToArray());
            }
            return (instrings);
        }

        public PlateEventData ParseEventLogLine(string line)
        {
            PlateEventData pd = new PlateEventData();

            string[] lineFields = line.Split(',');

            lineFields = CheckForBadlyFormedGPSStrings(lineFields);

            pd.eventType = GetEventType(lineFields[(int)FIELDS_ON_PARSING_STRING.EVENT]);

            if (pd.eventType == EVENT_TYPE.PLATE)
            {
                ParsePlateFields(lineFields[(int)FIELDS_ON_PARSING_STRING.PLATE_DATA], pd);

                pd.PSSName = lineFields[(int)FIELDS_ON_PARSING_STRING.PSS_NAME];

                pd.sourceChannelName = lineFields[(int)FIELDS_ON_PARSING_STRING.SOURCE_CAMERA_NAME];

                pd.GPSLatitude = lineFields[(int)FIELDS_ON_PARSING_STRING.GPS_LAT];
                pd.GPSLongitude = lineFields[(int)FIELDS_ON_PARSING_STRING.GPS_LON];
                pd.jpegRelativeFilePath = lineFields[(int)FIELDS_ON_PARSING_STRING.JPEGFILE_RELATIVE_PATH];
                pd.timeStamp = ParseDateTime(lineFields[(int)FIELDS_ON_PARSING_STRING.TIME_STAMP]);
            }
            else if (pd.eventType == EVENT_TYPE.MOTION)
            {

              //  MOTION,,k000a9df304bf,2009_08_18_19_29_50_3593,1,No Position Available,2009\8\18\19\k000a9df304bf\1\2009_08_18_19_29_50_35933523.jpg,

                pd.PSSName = lineFields[(int)FIELDS_ON_PARSING_STRING.PSS_NAME];

                pd.sourceChannelName = lineFields[(int)FIELDS_ON_PARSING_STRING.SOURCE_CAMERA_NAME];

                pd.GPSLatitude = lineFields[(int)FIELDS_ON_PARSING_STRING.GPS_LAT];
                pd.GPSLongitude = lineFields[(int)FIELDS_ON_PARSING_STRING.GPS_LON];
                pd.jpegRelativeFilePath = lineFields[(int)FIELDS_ON_PARSING_STRING.JPEGFILE_RELATIVE_PATH];
                pd.timeStamp = ParseDateTime(lineFields[(int)FIELDS_ON_PARSING_STRING.TIME_STAMP]);
            }
            
            return (pd);
        }

        DateTime ParseDateTime(string timestring)
        {
            // 2009:07:16:12:47:11:9220
            string[] sp1 = timestring.Split('_');
            int year = Convert.ToInt32(sp1[0]);
            int month = Convert.ToInt32(sp1[1]);
            int day = Convert.ToInt32(sp1[2]);
            int hour = Convert.ToInt32(sp1[3]);
            int min = Convert.ToInt32(sp1[4]);
            int sec = Convert.ToInt32(sp1[5]);
            int msec = Convert.ToInt32(sp1[6]) / 10;

            DateTime result = new DateTime(year, month, day, hour, min, sec, msec);
            return (result);
        }

        void ParsePlateFields(string s, PlateEventData pd)
        {
            string[] fields = s.Split('^');
            pd.PlateLanguage = fields[0];
            pd.plateNumbersNativeLanguage = fields[1].Split(':');
            pd.plateNumbersLatinEquivalent = fields[2].Split(':');
        }

        EVENT_TYPE GetEventType(string s)
        {
            if (s.Equals("MOTION")) return (EVENT_TYPE.MOTION);
            else if (s.Equals("PLATE")) return (EVENT_TYPE.PLATE);

            return(0);
        }

        public EVENT_TO_WRITE WriteMotoinEvent(string PSSName, DateTime timeStamp, string sourceName, string GPSLocation, string jpegRelativePath)
        {
            EVENT_TO_WRITE eventData = new EVENT_TO_WRITE();


            lineFields[(int)FIELDS_ON_BUILDING_STRING.EVENT] = "MOTION,"; /// the extra comma is for the blank field where the plate number would go
            lineFields[(int)FIELDS_ON_BUILDING_STRING.PSS_NAME] = PSSName;
            lineFields[(int)FIELDS_ON_BUILDING_STRING.TIME_STAMP] = timeStamp.ToString(m_AppData.TimeFormatStringForFileNames);
            lineFields[(int)FIELDS_ON_BUILDING_STRING.SOURCE_CAMERA_NAME] = sourceName;
            if (GPSLocation.Contains("No Position"))
            {
                GPSLocation = "No Position Available, No position Available"; // once for lat and once for lon
            }
            lineFields[(int)FIELDS_ON_BUILDING_STRING.GPS_LOCATION] = GPSLocation;
            lineFields[(int)FIELDS_ON_BUILDING_STRING.JPEGFILE_RELATIVE_PATH] = jpegRelativePath;

            string line = BuildLine(lineFields) + "\r\n";
            string file = m_PathManager.GetEventLogFilePath(timeStamp, sourceName);


            eventData.file = file;
            eventData.line = line;
            eventData.directory = m_PathManager.GetEventLogDirectoryPath(timeStamp, sourceName);

            return (eventData);

           
        }

        public EVENT_TO_WRITE WriteLPREvent(FRAME frame)
        {
            EVENT_TO_WRITE eventData = new EVENT_TO_WRITE();

            lineFields[(int)FIELDS_ON_BUILDING_STRING.EVENT] = BuildPlateField(frame);
            lineFields[(int)FIELDS_ON_BUILDING_STRING.PSS_NAME] = frame.PSSName;
            lineFields[(int)FIELDS_ON_BUILDING_STRING.TIME_STAMP] = frame.TimeStamp.ToString(m_AppData.TimeFormatStringForFileNames);
            lineFields[(int)FIELDS_ON_BUILDING_STRING.SOURCE_CAMERA_NAME] = frame.SourceName;
            if ( frame.GPSPosition.Contains("No Position"))
            {
                frame.GPSPosition = "No Position Available, No position Available"; // once for lat and once for lon
            }
            lineFields[(int)FIELDS_ON_BUILDING_STRING.GPS_LOCATION] = frame.GPSPosition;
            lineFields[(int)FIELDS_ON_BUILDING_STRING.JPEGFILE_RELATIVE_PATH] = frame.JpegFileRelativePath;

            string line = BuildLine(lineFields) + "\r\n";
            string file = m_PathManager.GetEventLogFilePath(frame.TimeStamp, frame.SourceName);

            eventData.file = file;
            eventData.line = line;
            eventData.directory = m_PathManager.GetEventLogDirectoryPath(frame.TimeStamp, frame.SourceName);

            return (eventData);
            
            //lock (m_FileLock)
            //{
            //    CreateLogDirectory(m_PathManager.GetEventLogDirectoryPath(frame.TimeStamp, frame.SourceName));

            //    File.AppendAllText(file, line);
            //}
        }

        //void CreateLogDirectory(string dir)
        //{

        //    if (!Directory.Exists((string)dir))
        //    {
        //        char[] seperator = { '\\', '\\' };

        //        string[] branches = dir.Split(seperator);
        //        string path = null;
        //        for (int i = 0; i < branches.Count(); i++)
        //        {
        //            if (branches[i].Length < 1) continue;
        //            path += (branches[i] + "\\");
        //            if (!Directory.Exists(path))
        //                Directory.CreateDirectory(path);
        //        }
        //    }


        //}

        string BuildPlateField(FRAME frame)
        {
            //            PLATE^LATIN^C3C456D2AB:G3C456D2AB:G3G456D2A8
            string plateField = "PLATE," + frame.PlateNativeLanguage + "^" + BuildAltReadings(frame.PlateNumberNativeLanguage) + "^" + BuildAltReadings(frame.PlateNumberLatin);

            return (plateField);
        }

        string BuildAltReadings(string[] readings)
        {
            string ars = null;

            bool lastStringIsZeroLen = false;
            if (readings[readings.Length - 1].Length == 0) lastStringIsZeroLen = true;

            for (int i = 0; i < readings.Length; i++)
            {
                if (readings[i].Length < 1) continue;

                if (i != (readings.Length - 1) && !(i == readings.Length - 2 && lastStringIsZeroLen))
                    ars += readings[i] + ":";
                else
                    ars += readings[i];
            }
            return (ars);
        }

        string BuildLine(string[] peices)
        {
            string l = null;
            foreach (string s in peices)
            {
                l += (s + ",");
            }
            return (l);
        }
    }


}
