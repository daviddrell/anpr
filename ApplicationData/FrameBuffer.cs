using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace ApplicationDataClass
{
    

    // contains 
    //          class FRAME
    //

    /// Design:
    /// 
    /// a FRAME contains a synchronized bmp & jpeg pair (one for LPR processing and one for DVR storage)
    /// and synchronized pair serial number, time stamp, GPS location, and source name.

    public class FRAME
    {
        Bitmap bmp;
        byte[] jpeg;
        string sourceName;

        DateTime timeStamp;

        string gPSPosition;
        bool available = true;
        object singleton;

        int[,] luminance;
        int sourceChannel;

        APPLICATION_DATA m_AppData;
        int consumerCount;


        public FRAME(APPLICATION_DATA appData)
        {
            singleton = new object();
            m_AppData = appData;
        }

        public void SetNew(Bitmap Bmp, byte[] Jpeg, string SourceName, DateTime TimeStamp, int SerialNumber, string GPSPosition, int consumersCnt, int SourceChannel)
        {

            lock (singleton)
            {
                bmp = (Bitmap)Bmp;
                jpeg = (byte[])Jpeg;
                sourceName = SourceName;
                timeStamp = TimeStamp;
                serialNumber = SerialNumber;
                gPSPosition = GPSPosition;
                consumerCount = consumersCnt;
                available = false;
                sourceChannel = SourceChannel;

                uint uniqueNumber;
                uniqueNumber = (uint)serialNumber & 0xfff;

                jpegFileNameOnly = timeStamp.ToString(m_AppData.TimeFormatStringForFileNames)+ uniqueNumber.ToString() + ".jpg";
                int year = timeStamp.Year;
                int day = timeStamp.Day;
                int hour = timeStamp.Hour;

                _NotVideoEachFrameIsUniqueSize = false;


            }
        }

        public void SetFileName()
        {
            uint uniqueNumber;
            uniqueNumber = (uint)serialNumber & 0xfff;

            jpegFileNameOnly = timeStamp.ToString(m_AppData.TimeFormatStringForFileNames) + uniqueNumber.ToString() + ".jpg";
        }

        public FRAME Clone(bool copyBitmap, bool copyJpeg, bool copyLuminance)
        {
            FRAME newFrame = new FRAME(m_AppData);

            lock (singleton)
            {
                if (copyBitmap)
                    newFrame.bmp = (Bitmap)bmp.Clone();
                if (copyJpeg)
                    newFrame.jpeg = (byte[])jpeg.Clone();

                newFrame.sourceName = (string)sourceName.Clone();
                newFrame.timeStamp = timeStamp;
                newFrame.serialNumber = serialNumber;
                if ( gPSPosition != null) newFrame.gPSPosition = (string)gPSPosition.Clone();
                newFrame.consumerCount = consumerCount;
                newFrame.available = available;

                if (pSSName != null) newFrame.pSSName = (string)pSSName.Clone();

                if (jpegFileNameOnly != null) newFrame.jpegFileNameOnly = (string)jpegFileNameOnly.Clone();

                if (jpegFileRelativePath != null) newFrame.jpegFileRelativePath = (string)jpegFileRelativePath.Clone();

                newFrame.sourceChannel = sourceChannel;

                if (copyLuminance)
                    newFrame.luminance = (int[,])luminance.Clone();

                if (plateNativeLanguage != null) newFrame.plateNativeLanguage = (string)plateNativeLanguage.Clone();

                if (plateNumberLatin != null) newFrame.plateNumberLatin = (string[])plateNumberLatin.Clone();
                if (plateNumberNativeLanguage != null) newFrame.PlateNumberNativeLanguage = (string[])plateNumberNativeLanguage.Clone();

                if (bestMatchingString != null) newFrame.bestMatchingString = (string)bestMatchingString.Clone();

                newFrame.matchScore = matchScore;
                newFrame.parentWatchList = parentWatchList;

                if (watchListMatchingNumber != null) newFrame.watchListMatchingNumber = (string)watchListMatchingNumber.Clone();
                if (watchListMatchingNumberUserComment != null) newFrame.watchListMatchingNumberUserComment = (string)watchListMatchingNumberUserComment.Clone();

                newFrame.NotVideoEachFrameIsUniqueSize = NotVideoEachFrameIsUniqueSize;
                return (newFrame);
            }
        }

        bool _NotVideoEachFrameIsUniqueSize;
        public bool NotVideoEachFrameIsUniqueSize
        { set { _NotVideoEachFrameIsUniqueSize = value; } get { lock (singleton) { return _NotVideoEachFrameIsUniqueSize; } } }


        public int SourceChannel
        { set { sourceChannel = value; } get { lock (singleton) { return sourceChannel; } } }

        public int[,] Luminance
        { set { luminance = value; } get { lock (singleton) { return luminance; } } }

        public bool Available
        { set { } get { lock (singleton) { return available; } } }

        public Bitmap Bmp
        { set { lock (singleton) { bmp = value; } } get { lock (singleton) { return bmp; } } }

        public byte[] Jpeg
        { set { lock (singleton) { jpeg = value; } } get { lock (singleton) { return jpeg; } } }

        public string SourceName
        { set { lock (singleton) { sourceName = value; } } get { lock (singleton) { return sourceName; } } }

        public DateTime TimeStamp
        { set { lock (singleton) { timeStamp = value; } } get { lock (singleton) { return timeStamp; } } }


        public string GPSPosition
        { set { lock (singleton) { gPSPosition = value; } } get { lock (singleton) { return gPSPosition; } } }

        string pSSName;
        public string PSSName
        { set { lock (singleton) { pSSName = value; } } get { lock (singleton) { return pSSName; } } }

        int serialNumber;
        public int SerialNumber
        { set { lock (singleton) { serialNumber = value; } } get { lock (singleton) { return serialNumber; } } }

        string jpegFileNameOnly;
        public string JpegFileNameOnly
        { set { lock (singleton) { jpegFileNameOnly = value; } } get { lock (singleton) { return jpegFileNameOnly; } } }


        string jpegFileRelativePath;
        public string JpegFileRelativePath
        { set { lock (singleton) { jpegFileRelativePath = value; } } get { lock (singleton) { return jpegFileRelativePath; } } }

        string[] plateNumberLatin;
        public string[] PlateNumberLatin
        { set { lock (singleton) { plateNumberLatin = value; } } get { lock (singleton) { return plateNumberLatin; } } }

        string[] plateNumberNativeLanguage;
        public string[] PlateNumberNativeLanguage
        { set { lock (singleton) { plateNumberNativeLanguage = value; } } get { lock (singleton) { return plateNumberNativeLanguage; } } }

        string plateNativeLanguage;
        public string PlateNativeLanguage
        { set { lock (singleton) { plateNativeLanguage = value; } } get { lock (singleton) { return plateNativeLanguage; } } }

        string bestMatchingString; // used only after a watch list match
        public string BestMatchingString
        { set { lock (singleton) { bestMatchingString = value; } } get { lock (singleton) { return bestMatchingString; } } }

        int matchScore; // used only after a watch list match
        public int MatchScore
        { set { lock (singleton) { matchScore = value; } } get { lock (singleton) { return matchScore; } } }

        object parentWatchList; // used only after a watch list match
        public object ParentWatchList
        { set { lock (singleton) { parentWatchList = value; } } get { lock (singleton) { return parentWatchList; } } }

        string watchListMatchingNumber; // used only after a watch list match - the string this plate matched to
        public string WatchListMatchingNumber
        { set { lock (singleton) { watchListMatchingNumber = value; } } get { lock (singleton) { return watchListMatchingNumber; } } }

        string watchListMatchingNumberUserComment; // used only after a watch list match - the string this plate matched to
        public string WatchListMatchingNumberUserComment
        { set { lock (singleton) { watchListMatchingNumberUserComment = value; } } get { lock (singleton) { return watchListMatchingNumberUserComment; } } }
    }
}
