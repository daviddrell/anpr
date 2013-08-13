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


namespace WatchlistLib
{

    public partial class WatchLists
    {

      

        /// <summary>
        /// A watch list match first goes into a grouping of similar alert items(multiple sightings of the same plate.  Then after a time-out, all similar items
        /// are consolidated into one watch alert. 
        /// </summary>
        class IntermediateWLEntry 
        {
            public IntermediateWLEntry(WatchListControl parent)
            {
                WLIsingleton = new object();
                score = 0;
                timeInQue = DateTime.UtcNow;
                parentWatchList = parent;
            }

            FRAME matchingFrame;
           
            public void AddFrame(FRAME frame, string matchingPlateString,int newScore, string alertString)
            {
                if (newScore > score)
                {
                    score = newScore;
                    bestMatchPlateString = matchingPlateString;
                    matchingFrame = frame;
                    matchingFrame.BestMatchingString = matchingPlateString;
                    matchingFrame.MatchScore = newScore;
                    matchingFrame.ParentWatchList = (object)parentWatchList;
                    matchingFrame.WatchListMatchingNumber = alertString;
                }
            }

            public FRAME GetFrame()
            {
                return (matchingFrame);
            }

            string watchListEntry;
            public string WatchListEntry
            { set { lock (WLIsingleton) { watchListEntry = value; } } get { lock (WLIsingleton) { return watchListEntry; } } }

            string bestMatchPlateString;
            public string BestMatchPlateString
            { set { lock (WLIsingleton) { bestMatchPlateString = value; } } get { lock (WLIsingleton) { return bestMatchPlateString; } } }

            WatchListControl parentWatchList;
            public WatchListControl ParentWatchList
            { set { lock (WLIsingleton) { parentWatchList = value; } } get { lock (WLIsingleton) { return parentWatchList; } } }

            int score;
            public int Score
            { set { lock (WLIsingleton) { score = value; } } get { lock (WLIsingleton) { return score; } } }

            DateTime timeInQue;
            public DateTime TimeInQue
            { set { lock (WLIsingleton) { timeInQue = value; } } get { lock (WLIsingleton) { return timeInQue; } } }

            bool IsExpired(DateTime now, TimeSpan timeout)
            {
                if (now.Subtract(timeInQue).CompareTo(timeout) >= 0) return (true); else return (false);
            }

            object WLIsingleton;
        }


        public class WatchedNumber
        {
            public string Number;
            public string UserComment;
        }

        /// <summary>
        /// Control structure for a watch list
        /// </summary>
        public class WatchListControl
        {
            public WatchListControl( )
            {
                singleton = new object();
            }

            public WatchListControl(int idx)
            {
                singleton = new object();
                index = idx;
            }

            public WatchListControl Clone()
            {
                WatchListControl clone = new WatchListControl();
                clone.WatchEntrys = new WatchedNumber[this.WatchEntrys.Length];
                clone.index = this.index;
                if (this.userAssignedGroupName != null) clone.userAssignedGroupName = (string)this.userAssignedGroupName.Clone();
                if (this.dataFileCompletePath != null) clone.dataFileCompletePath = (string)this.dataFileCompletePath.Clone();
                if (this.emailFileCompletePath != null) clone.emailFileCompletePath = (string)this.emailFileCompletePath.Clone();

                for (int i = 0; i < this.WatchEntrys.Length; i++ )
                {
                    clone.watchEntrys[i] = new WatchedNumber();
                    clone.watchEntrys[i].Number =(string) this.watchEntrys[i].Number.Clone();
                    clone.watchEntrys[i].UserComment = (string)this.watchEntrys[i].UserComment.Clone();
                }

                clone.watchListLoaded = this.watchListLoaded;

                if ( this.watchEmailAdresses != null)  clone.watchEmailAdresses = (string[]) this.watchEmailAdresses.Clone();

                clone.watchEmailLoaded = this.watchEmailLoaded;
                clone.alertThreshold = this.alertThreshold;
                clone.emailsEnabled = this.emailsEnabled;
                if (this.Watcher != null) clone.Watcher = this.Watcher;

                return (clone);
            }

            int index;
            public int Index
            { set { lock (singleton) { index = value; } } get { lock (singleton) { return index; } } }

            string userAssignedGroupName;
            public string UserAssignedGroupName
            { set { lock (singleton) { userAssignedGroupName = value; } } get { lock (singleton) { return userAssignedGroupName; } } }

            // file data
            string dataFileCompletePath;
            public string DataFileCompletePath
            { set { lock (singleton) { dataFileCompletePath = value; } } get { lock (singleton) { return dataFileCompletePath; } } }

            string emailFileCompletePath;
            public string EmailFileCompletePath
            { set { lock (singleton) { emailFileCompletePath = value; } } get { lock (singleton) { return emailFileCompletePath; } } }

            // watch data
            WatchedNumber[] watchEntrys;
            public WatchedNumber [] WatchEntrys
            { set { lock (singleton) { watchEntrys = value; } } get { lock (singleton) { return watchEntrys; } } }

            bool watchListLoaded;
            public bool WatchListLoaded
            { set { lock (singleton) { watchListLoaded = value; } } get { lock (singleton) { return watchListLoaded; } } }

            string[] watchEmailAdresses;
            public string[] WatchEmailAddresses
            { set { lock (singleton) { watchEmailAdresses = value; } } get { lock (singleton) { return watchEmailAdresses; } } }


            bool watchEmailLoaded;
            public bool WatchEmailLoaded
            { set { lock (singleton) { watchEmailLoaded = value; } } get { lock (singleton) { return watchEmailLoaded; } } }

            int alertThreshold;
            public int AlertThreshold
            { set { lock (singleton) { alertThreshold = value; } } get { lock (singleton) { return alertThreshold; } } }

            bool emailsEnabled;
            public bool EmailsEnabled
            { set { lock (singleton) { emailsEnabled = value; } } get { lock (singleton) { return emailsEnabled; } } }


            // get notified if the watch list changes on disk
            public FileSystemWatcher Watcher;

            object singleton;
        }
    }
}
