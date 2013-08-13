using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Threading;


namespace ApplicationDataClass
{

    public partial class APPLICATION_DATA
    {


        ////////////////////////////////////
        /// 
        ///   Health Statistics
        ///   
        ///

        //         To add a group which auto loads when this class is instantiated

        //        1. add the group tag in STAT_GROUPS
        //        2. create an enum for that group
        //        3.  add load loop for the new enum in the LoadStatObjects() method
        //        4. add a if() check for the new enum group in indexor :  "public GenericStatisticsClass this[int index]"

        //        To add a group later during run time
        //        1. Call AddStateGroup(), but the remote clients will not know about these if this is running on the server side.
        //        

        public class HEALTH_STATISTICS
        {

            //    STEP  1 - add the group tag to this list

            public enum STAT_GROUPS
            {
                LPR,
                PhysicalChannels,
                FrameGen,
                MotionDetector,
                DVR,
                WatchList,
                System,
                QueueOverruns,
                LPRServiceVersionString
            }


            //    STEP  2 - create the new enum 
            //   NOTE: group name followed by underscore followed by stat name allowes client end to parse out groups and stats

            public enum LPR : int
            {
                LPR_FramesProcessed=(int)0,
                LPR_ProcessQCnt,
                LPR_DroppedFrames,
                LPR_PlatesFound,
                LPR_LastReading,
                LPR_PlateGroupCount,
                // if more than 100 increment the startin index in PHYSICAL_CHANNELS
            }

            public enum PHYSICAL_CHANNELS : int
            {
                PhysicalChannels_c0 = (int)100,
                PhysicalChannels_c1 = (int)101,
                PhysicalChannels_c2 = (int)102,
                PhysicalChannels_c3 = (int)103,
                PhysicalChannels_c4 = (int)104,
                PhysicalChannels_c5 = (int)105,
                PhysicalChannels_c6 = (int)106,
                PhysicalChannels_c7 = (int)107,
                PhysicalChannels_c8 = (int)108,
                PhysicalChannels_c9 = (int)109,
                PhysicalChannels_c10 = (int)110,
                PhysicalChannels_c11 = (int)111,
                PhysicalChannels_c12 = (int)112,
                PhysicalChannels_c13 = (int)113,
                PhysicalChannels_c14 = (int)114,
                PhysicalChannels_c15 = (int)115,
                PhysicalChannels_c16 = (int)116,
                // if more than 100 increment the startin index in FRAME_GENERATOR
            }

            public enum FRAME_GENERATOR : int
            {
                FrameGen_FrameCnt = (int)200,
                FrameGen_MotionDetectionPendingQ,
                FrameGen_NonMotionFramePushQ,
                FrameGen_GPSLocation,
                // if more than 100 increment the startin index in MOTION_DETECTION
            }

            public enum MOTION_DETECTION : int
            {
                MotionDetector_FrameCnt = (int)300,
                MotionDetector_ProcessQCnt,
                MotionDetector_DroppedFrames,
                MotionDetector_FramesDetected,
            }
            public enum DVR : int
            {
                DVR_DriveReady = (int) 400,
                DVR_FreeSpace,
                DVR_UsedSpace,
                DVR_DriveName,
                DVR_DriveHotSwap,
            }

            public enum WatchList : int
            {
                WatchList_LastAlert = (int)500,
                WatchList_NumAlerts,
                Watchlist_NumListsLoaded,
            }

           

            public enum System : int
            {
                System_frameGrabber_2 = (int)600,
                System_frameGrabber_1,
                System_GPS,
                System_Service,
                System_videoChannel1,
                System_videoChannel2,
                System_videoChannel3,
                System_videoChannel4,
                System_Drive,
                System_Hotswap,
            }


            public enum QueueOverruns : int
            {
                QueueOverruns_FG_MotionDetectedConsumerPushQ = (int)700,
                QueueOverruns_FG_AllFramesConsumerPushQ = (int)701,
                QueueOverruns_FG_MotionDetectionQ = (int)702,
                QueueOverruns_DVR_NewFrameQ = (int)703,
                QueueOverruns_DVR_MotionDetectedQ = (int)704,
                QueueOverruns_DVR_DirectyToStorageQ = (int)705,
                QueueOverruns_DVR_NewLPRRecordQ = (int)706,
                QueueOverruns_LPR_LPRProcessQ = (int)707,
                QueueOverruns_LPR_LPRFinalPlateGroupOutputQ = (int)708,
                QueueOverruns_LPR_LPRPerFrameReadingQ = (int)709,
            }

           
            public enum LPRServiceVersionString : int
            {
                LPRServiceVersionString_version = (int)800,
            }

            //   STEP 3 - add new enum foreach loop

            void LoadStatObjects()
            {
                StatsHashTableGetGroupByName = new Hashtable();
                StatsHashTableGetIndexByName = new Hashtable();
                
                int index = 0;
                foreach (string statName in Enum.GetNames(typeof(LPR)))
                {
                    AddStat(statName, (int)(Enum.GetValues(typeof(LPR))).GetValue(index));
                    index++;
                }

                index = 0;
                foreach (string statName in Enum.GetNames(typeof(PHYSICAL_CHANNELS)))
                {
                    AddStat(statName,(int) Enum.GetValues(typeof(PHYSICAL_CHANNELS)).GetValue(index));
                    index++;
                }

                index = 0;
                foreach (string statName in Enum.GetNames(typeof(FRAME_GENERATOR)))
                {
                    AddStat(statName,(int) Enum.GetValues(typeof(FRAME_GENERATOR)).GetValue(index));
                    index++;
                }

                index = 0;
                foreach (string statName in Enum.GetNames(typeof(MOTION_DETECTION)))
                {
                    AddStat(statName,(int) Enum.GetValues(typeof(MOTION_DETECTION)).GetValue(index));
                    index++;
                }

                index = 0;
                foreach (string statName in Enum.GetNames(typeof(DVR)))
                {
                    AddStat(statName,(int) Enum.GetValues(typeof(DVR)).GetValue(index));
                    index++;
                }

                index = 0;
                foreach (string statName in Enum.GetNames(typeof(WatchList)))
                {
                    AddStat(statName,(int) Enum.GetValues(typeof(WatchList)).GetValue(index));
                    index++;
                }

                index = 0;
                foreach (string statName in Enum.GetNames(typeof(System)))
                {
                    AddStat(statName,(int) Enum.GetValues(typeof(System)).GetValue(index));
                    index++;
                }

                index = 0;
                foreach (string statName in Enum.GetNames(typeof(QueueOverruns)))
                {
                    AddStat(statName, (int)Enum.GetValues(typeof(QueueOverruns)).GetValue(index));
                    index++;
                }

                index = 0;
                foreach (string statName in Enum.GetNames(typeof(LPRServiceVersionString)))
                {
                    AddStat(statName, (int)Enum.GetValues(typeof(LPRServiceVersionString)).GetValue(index));
                    index++;
                }
            }


            //   STEP 4 - add indexer check for the new enum

            /// <summary>
            /// Allow access to the individual statistics objects via an indexer to the HealthStatistics class
            /// </summary>
            /// <param name="key"></param>
            /// <returns></returns>
            public GenericStatisticsClass this[int index]
            {

                get
                {
                    string key = null;

                    if (index < 100)
                        key = Enum.GetName(typeof(LPR), index);
                    else if (index < 200)
                        key = Enum.GetName(typeof(PHYSICAL_CHANNELS), index);
                    else if (index < 300)
                        key = Enum.GetName(typeof(FRAME_GENERATOR), index);
                    else if (index < 400)
                        key = Enum.GetName(typeof(MOTION_DETECTION), index);
                    else if (index < 500)
                        key = Enum.GetName(typeof(DVR), index);
                    else if (index < 600)
                        key = Enum.GetName(typeof(WatchList), index);
                    else if (index < 700)
                        key = Enum.GetName(typeof(System), index);
                    else if (index < 800)
                        key = Enum.GetName(typeof(QueueOverruns), index);
                    else if (index < 900)
                        key = Enum.GetName(typeof(LPRServiceVersionString), index);

                    if (StatsHashTableGetGroupByName.ContainsKey(key))
                    {
                       
                        return (GenericStatisticsClass)StatsHashTableGetGroupByName[key];
                    }
                    else { return null; }
                }
            }


            /// <summary>
            /// Returns the enum index for the enum name supplied
            /// </summary>
            /// <param name="name"></param>
            /// <returns></returns>

            public int GetStatIndexByName(string name)
            {
                if (!StatsHashTableGetIndexByName.Contains(name)) return (-1);
                return ((int)StatsHashTableGetIndexByName[name]);
            }

            /// <summary>
            /// Returns the group name headings
            /// </summary>
            /// <returns></returns>
            public string[] GetGroupList ( )
            {
                return( Enum.GetNames(typeof(STAT_GROUPS)));
            }

            /// <summary>
            /// Allow access to the individual statistics objects via an indexer to the HealthStatistics class
            /// </summary>
            /// <param name="key"></param>
            /// <returns></returns>
            public GenericStatisticsClass this[string name]
            {

                get
                {
                    if (StatsHashTableGetGroupByName.ContainsKey(name))
                    {
                        return (GenericStatisticsClass)StatsHashTableGetGroupByName[name];
                    }
                    else { return null; }
                }
            }



            /// <summary>
            /// Add a new statistics group. Access the created group using the StatGroups["groupName"] property.
            /// </summary>
            /// <param name="groupName"></param>
            void AddStat (string statName, int index)
            {
                GenericStatisticsClass grp = new GenericStatisticsClass(this,statName);
                StatsHashTableGetGroupByName.Add(statName, grp);
                StatsHashTableGetIndexByName.Add(statName, index);
            }

            /// <summary>
            /// Returns a list of statistic names that have been entered into the table
            /// </summary>
            /// <returns></returns>
            public string[] GetStatList()
            {
                string[] groupNames = new string[StatsHashTableGetGroupByName.Count];

                StatsHashTableGetGroupByName.Keys.CopyTo(groupNames, 0);

                return (groupNames);
            }

           

            private Hashtable StatsHashTableGetGroupByName;

            private Hashtable StatsHashTableGetIndexByName;
           

            // ///////////////////////////////////////////////////
            // INTERNAL OPERATIONS

            // remote clients use this contructor to build an object just to hold values by name as received over the sock connection
            /// <summary>
            /// 
            /// </summary>
            public HEALTH_STATISTICS()
            {
                singleton = new object();
                LoadStatObjects();
            }

            // servers use this constructor to actively collect and manage statistics
            /// <summary>
            /// Use this constructor on the LPR service side to instantiate the statistical counter service
            /// </summary>
            /// <param name="appData"></param>

            public HEALTH_STATISTICS(APPLICATION_DATA appData)
            {
                singleton = new object();

                LoadStatObjects();

             
                m_AppData = appData;
                m_AppData.AddOnClosing(Stop, CLOSE_ORDER.LAST);
                StartStatsUpdateThread();
            }


            //   ////////////////////
            //
            //    GenericStatistics  Class

            public class GenericStatisticsClass
            {
                public GenericStatisticsClass(HEALTH_STATISTICS parent, string objectName)
                {
                    PerSecond = new PerSecondCounterClass(parent, objectName + ".PerSecond");
                    RunningAverage = new RunningAverageClass(parent, objectName +".RunningAverage");
                    Accumulator = new AccumulatorClass(parent, objectName + ".Accumulator");
                    Snapshot = new SnapshotClass(parent, objectName + ".Snapshot");
                    Peak = new PeakClass(parent, objectName + ".Peak");
                    StatString = new StatStringClass(parent, objectName + ".StatString");
                    boolean = new booleanClass(parent, objectName + ".boolean");
                    SnapshotDouble = new SnapshotDoubleClass(parent, objectName + ".SnapshotDouble") ;
                }

                public void Reset()
                {
                    RunningAverage.Reset();
                    PerSecond.Reset();
                    Accumulator.Reset();
                    Snapshot.Reset();
                    Peak.Reset();
                    SnapshotDouble.Reset();
                }
                public int HitMe
                {
                    set
                    {

                        PerSecond.Increment(value);
                        RunningAverage.Increment(value);
                        Accumulator.Increment(value);
                        Snapshot.SetValue = value;
                        Peak.SetValue = value;
                       
                    }
                    get {  return 0;  }  // allows the += operation to add one
                }

                // the boolean

                public booleanClass boolean;
                public class booleanClass
                {
                    object singleton;
                    public booleanClass(HEALTH_STATISTICS parent, string objectName)
                    {
                        singleton = new object();
                        Name = objectName;
                        _CurrentValue = false;
                    }
                    public string Name;
                    public bool SetValue
                    {
                        set
                        {
                            lock (singleton)
                            {
                                _CurrentValue = value;
                            }
                        }
                    }
                    private bool _CurrentValue;
                    public bool CurrentValue { get { lock (singleton) { return _CurrentValue; } } }

                    /// <summary>
                    /// Returns a string composed of the "object-name:object-value" for TCP packet transmission
                    /// </summary>
                    /// <returns></returns>
                    public string GetNameValue() { lock (singleton) { if (!registeredForUse) return (null); else return (Name + ":" + _CurrentValue.ToString()); } }
                    private bool registeredForUse = false;
                    public void RegisterForUse(bool use) { lock (singleton) { registeredForUse = use; } }
                    /// <summary>
                    /// tests a object-name:object-value string of this object against the received TCP string
                    /// </summary>
                    /// <param name="test"></param>
                    /// <param name="value"></param>
                    /// <returns></returns>
                    public bool NameValueEquals(string test, out string value)
                    {
                        string[] sp1 = test.Split(':');
                        if (sp1[0].Equals(Name))
                        {
                            value = sp1[1];
                            return (true);
                        }
                        value = "";
                        return (false);
                    }
                }

                // the StatString

                public StatStringClass StatString;
                public class StatStringClass
                {
                    object singleton;
                    public StatStringClass(HEALTH_STATISTICS parent, string objectName)
                    {
                        singleton = new object();
                        Name = objectName;
                        _CurrentValue = "";
                    }
                    public string Name;
                    public string SetValue 
                    { 
                        set 
                        { 
                            lock (singleton) 
                            {
                                if (value == null) return;
                                string s = value;
                                s = s.Replace(":", "^^");// escape out the colon as it is used by the stats reporter as a delimiter
                                _CurrentValue = s; 
                            } 
                        } 
                    }
                    private string _CurrentValue;
                    public string CurrentValue { get { lock (singleton) { return _CurrentValue; } } }
                   
                    /// <summary>
                    /// Returns a string composed of the "object-name:object-value" for TCP packet transmission
                    /// </summary>
                    /// <returns></returns>
                    public string GetNameValue() 
                    {
                        lock (singleton) 
                        {
                            if (!registeredForUse)
                                return (null);
                            else
                            {
                                return (Name + ":" + _CurrentValue);
                            }
                        }
                    }
                    private bool registeredForUse = false;
                    public void RegisterForUse(bool use) { lock (singleton) { registeredForUse = use; } }
                    /// <summary>
                    /// tests a object-name:object-value string of this object against the received TCP string
                    /// </summary>
                    /// <param name="test"></param>
                    /// <param name="value"></param>
                    /// <returns></returns>
                    public bool NameValueEquals(string test, out string value)
                    {
                        string[] sp1 = test.Split(':');
                        if (sp1[0].Equals(Name))
                        {
                            value = sp1[1];
                            return (true);
                        }
                        value = "";
                        return (false);
                    }
                }

                // the Peak

                public PeakClass Peak;
                public class PeakClass
                {
                    object singleton;
                    public PeakClass(HEALTH_STATISTICS parent, string objectName)
                    {
                        singleton = new object();
                        Name = objectName;
                        parent.OnOneSecondTick += OnTimerTick;
                        _CurrentValue = 0;
                    }
                    public void Reset() { lock (singleton) { _CurrentValue = 0; } }
                    public string Name;
                    public int SetValue { set { lock (singleton) { if ( value >  _CurrentValue )  _CurrentValue = value; } } }
                    private int _CurrentValue;
                    public int CurrentValue { get { lock (singleton) { return _CurrentValue; } } }
                    private void OnTimerTick() { lock (singleton) { } }
                    /// <summary>
                    /// Returns a string composed of the "object-name:object-value" for TCP packet transmission
                    /// </summary>
                    /// <returns></returns>
                    public string GetNameValue() { lock (singleton) { if (!registeredForUse) return (null); else return (Name + ":" + _CurrentValue.ToString()); } }
                    private bool registeredForUse = false;
                    public void RegisterForUse(bool use) { lock (singleton) { registeredForUse = use; } }
                    /// <summary>
                    /// tests a object-name:object-value string of this object against the received TCP string
                    /// </summary>
                    /// <param name="test"></param>
                    /// <param name="value"></param>
                    /// <returns></returns>
                    public bool NameValueEquals(string test, out int value)
                    {
                        string[] sp1 = test.Split(':');
                        if (sp1[0].Equals(Name))
                        {
                            value = Convert.ToInt32(sp1[1]);
                            return (true);
                        }
                        value = 0;
                        return (false);
                    }
                }

                // the SnapshotDouble

                public SnapshotDoubleClass SnapshotDouble;
                public class SnapshotDoubleClass
                {
                    object singleton;
                    public SnapshotDoubleClass(HEALTH_STATISTICS parent, string objectName)
                    {
                        singleton = new object();
                        Name = objectName;
                    }
                    public string Name;
                    public void Reset() { lock (singleton) { _CurrentValue = 0; } }
                    public double SetValue { set { lock (singleton) { _CurrentValue = value; } } }
                    private double _CurrentValue;
                    public double CurrentValue { get { lock (singleton) { return _CurrentValue; } } }
                    private void OnTimerTick() { lock (singleton) { } }
                    /// <summary>
                    /// Returns a string composed of the "object-name:object-value" for TCP packet transmission
                    /// </summary>
                    /// <returns></returns>
                    public string GetNameValue() { lock (singleton) { if (!registeredForUse) return (null); else return (Name + ":" + _CurrentValue.ToString()); } }
                    private bool registeredForUse = false;
                    public void RegisterForUse(bool use) { lock (singleton) { registeredForUse = use; } }
                    /// <summary>
                    /// tests a object-name:object-value string of this object against the received TCP string
                    /// </summary>
                    /// <param name="test"></param>
                    /// <param name="value"></param>
                    /// <returns></returns>
                    public bool NameValueEquals(string test, out int value)
                    {
                        string[] sp1 = test.Split(':');
                        if (sp1[0].Equals(Name))
                        {
                            value = Convert.ToInt32(sp1[1]);
                            return (true);
                        }
                        value = 0;
                        return (false);
                    }
                }
               
                // the Snapshot

                public SnapshotClass Snapshot;
                public class SnapshotClass
                {
                    object singleton;
                    public SnapshotClass(HEALTH_STATISTICS parent, string objectName)
                    {
                        singleton = new object();
                        Name = objectName;
                        parent.OnOneSecondTick += OnTimerTick;
                    }
                    public string Name;
                    public void Reset() { lock (singleton) { _CurrentValue = 0; } }

                    public int SetValue { set { lock (singleton) { _CurrentValue = value; } } }
                    private int _CurrentValue;
                    public int CurrentValue { get { lock (singleton) { return _CurrentValue; } } }
                    private void OnTimerTick() { lock (singleton) { } }
                    /// <summary>
                    /// Returns a string composed of the "object-name:object-value" for TCP packet transmission
                    /// </summary>
                    /// <returns></returns>
                    public string GetNameValue() { lock (singleton) { if (!registeredForUse) return (null); else return (Name + ":" + _CurrentValue.ToString()); } }
                    private bool registeredForUse = false;
                    public void RegisterForUse(bool use) { lock (singleton) { registeredForUse = use; } }
                    /// <summary>
                    /// tests a object-name:object-value string of this object against the received TCP string
                    /// </summary>
                    /// <param name="test"></param>
                    /// <param name="value"></param>
                    /// <returns></returns>
                    public bool NameValueEquals(string test, out int value)
                    {
                        string[] sp1 = test.Split(':');
                        if (sp1[0].Equals(Name))
                        {
                            value = Convert.ToInt32(sp1[1]);
                            return (true);
                        }
                        value = 0;
                        return (false);
                    }
                }
               
                // the accumulator

                public AccumulatorClass Accumulator;
                public class AccumulatorClass
                {
                    object singleton;
                    public AccumulatorClass(HEALTH_STATISTICS parent, string objectName)
                    {
                        singleton = new object();
                        Name = objectName;
                        parent.OnOneSecondTick += OnTimerTick;
                    }
                    public string Name;
                    public void Reset() { lock (singleton) { _CurrentValue = 0; } }
                    public int SetValue { set { lock (singleton) { _CurrentValue = value; } } }
                    private int _CurrentValue;
                    public int CurrentValue { get { lock (singleton) { return _CurrentValue; } } }
                    public void Increment(int inc) { lock (singleton) { _CurrentValue += inc; } }
                    private void OnTimerTick() { lock (singleton) { } }
                    /// <summary>
                    /// Returns a string composed of the "object-name:object-value" for TCP packet transmission
                    /// </summary>
                    /// <returns></returns>
                    public string GetNameValue() { lock (singleton) { if (!registeredForUse) return (null); else return (Name + ":" + CurrentValue.ToString()); } }
                    private bool registeredForUse = false;
                    public void RegisterForUse(bool use) { lock (singleton) { registeredForUse = use; } }
                    /// <summary>
                    /// tests a object-name:object-value string of this object against the received TCP string
                    /// </summary>
                    /// <param name="test"></param>
                    /// <param name="value"></param>
                    /// <returns></returns>
                    public bool NameValueEquals(string test, out int value)
                    {
                        string[] sp1 = test.Split(':');
                        if (sp1[0].Equals(Name))
                        {
                            value = Convert.ToInt32(sp1[1]);
                            return (true);
                        }
                        value = 0;
                        return (false);
                    }
                }
               
                // the per second counter

                public PerSecondCounterClass PerSecond;
                public class PerSecondCounterClass
                {
                    object singleton;
                    public PerSecondCounterClass(HEALTH_STATISTICS parent, string objectName)
                    {
                        singleton = new object();
                        Name = objectName;
                        parent.OnOneSecondTick += OnTimerTick;
                    }
                    public string Name;
                    public int SetValue { set { lock (singleton) { _CurrentValue = value; } } }
                    private int _CurrentValue;
                    public int CurrentValue { get { lock (singleton) { return _CurrentValue; } } }
                    public void Increment(int inc) { lock (singleton) { currentCount += inc; } }
                    private int currentCount;
                    public void Reset() { lock (singleton) { _CurrentValue = 0; } }
                    private void OnTimerTick() { lock (singleton) { _CurrentValue = currentCount; currentCount = 0; } }
                    /// <summary>
                    /// Returns a string composed of the "object-name:object-value" for TCP packet transmission
                    /// </summary>
                    /// <returns></returns>
                    public string GetNameValue() { lock (singleton) { if (!registeredForUse) return (null); else return (Name + ":" + _CurrentValue.ToString()); } }
                    private bool registeredForUse = false;
                    public void RegisterForUse(bool use) { lock (singleton) { registeredForUse = use; } }
                    /// <summary>
                    /// tests a object-name:object-value string of this object against the received TCP string
                    /// </summary>
                    /// <param name="test"></param>
                    /// <param name="value"></param>
                    /// <returns></returns>
                    public bool NameValueEquals(string test, out int value)
                    {
                        string[] sp1 = test.Split(':');
                        if (sp1[0].Equals(Name))
                        {
                            value = Convert.ToInt32(sp1[1]);
                            return (true);
                        }
                        value = 0;
                        return (false);
                    }
                }


                // the running  ave counter

                public RunningAverageClass RunningAverage;
                public class  RunningAverageClass
                {
                    object singleton;
                    public RunningAverageClass(HEALTH_STATISTICS parent, string objectName)
                    {
                        singleton = new object();
                        Name = objectName;
                        parent.OnOneSecondTick += OnTimerTick;
                    }
                    public string Name;
                    public void Reset() { lock (singleton) { _CurrentValue = 0; currentCount = 0; totalTicks = 1; } }
                    public int SetValue { set { lock (singleton) { _CurrentValue = value; } } }
                    private int _CurrentValue;
                    public int CurrentValue { get { lock (singleton) { return _CurrentValue; } } }
                    public void Increment(int inc) 
                    { 
                        lock (singleton) 
                        { 
                            currentCount += inc; 
                        } 
                    }
                    public void ResetRunningAverage(){lock(singleton){currentCount = 0; totalTicks = 1;}}
                    private int currentCount;
                    private int totalTicks = 1;
                 
                    private void OnTimerTick() 
                    { 
                        lock (singleton) 
                        { 
                            _CurrentValue = currentCount / totalTicks; 
                            totalTicks++; 
                            if (currentCount == 0) 
                            { 
                                totalTicks = 1; 
                            } 
                        } 
                    }
                    /// <summary>
                    /// Returns a string composed of the "object-name:object-value" for TCP packet transmission
                    /// </summary>
                    /// <returns></returns>
                    public string GetNameValue() { lock (singleton) { if (!registeredForUse) return (null); else return (Name + ":" + _CurrentValue.ToString()); } }
                    private bool registeredForUse = false;
                    public void RegisterForUse(bool use) { lock (singleton) { registeredForUse = use; } }
                    /// <summary>
                    /// tests a object-name:object-value string of this object against the received TCP string
                    /// </summary>
                    /// <param name="test"></param>
                    /// <param name="value"></param>
                    /// <returns></returns>
                    public bool NameValueEquals(string test, out int value)
                    {
                        string[] sp1 = test.Split(':');
                        if (sp1[0].Equals(Name))
                        {
                            value = Convert.ToInt32(sp1[1]);
                            return (true);
                        }
                        value = 0;
                        return (false);
                    }
                } 
            }

       

            private delegate void UpdateOneSecCounters();
            UpdateOneSecCounters OnOneSecondTick;
            void UpdateOneSecondCounters()
            {
                if (OnOneSecondTick != null) OnOneSecondTick();
            }

       
            private object singleton;



            APPLICATION_DATA m_AppData;

            bool m_Stop = false;
            void Stop()
            {
                m_Stop = true;
            }

            Thread m_UpDateStatsThread;
            void StartStatsUpdateThread()
            {
                m_UpDateStatsThread = new Thread(UpdateStatsLoop);
                m_UpDateStatsThread.Start();
            }
            DateTime m_TimeAtLastUpdateOneSecond;
          //  DateTime m_TimeAtLastUpdateThirtySeconds;
            TimeSpan OneSecond;
           // TimeSpan ThirtySeconds;


            //  runs a thread which updates internal stat counters based on functionaility

            void UpdateStatsLoop()
            {
                OneSecond = new TimeSpan(0, 0, 1);
            //    ThirtySeconds = new TimeSpan(0, 0, 30);

                m_TimeAtLastUpdateOneSecond = DateTime.Now;
             //   m_TimeAtLastUpdateThirtySeconds = DateTime.Now;

                while (!m_Stop)
                {
                   
                    if (DateTime.Now.Subtract(m_TimeAtLastUpdateOneSecond).CompareTo(OneSecond) > 0)
                    {
                        UpdateOneSecondCounters();
                        m_TimeAtLastUpdateOneSecond = DateTime.Now;
                    }

                    Thread.Sleep(100);
                }
            }


        }
    }

}