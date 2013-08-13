using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ApplicationDataClass;
using S2255Controller;
using GPSLib;
using System.Threading;
using ErrorLoggingLib;
using System.Drawing;
using Utilities;
using LPROCR_Wrapper;
using UserSettingsLib;
using System.Collections;
using System.IO;

namespace FrameGeneratorLib
{
    public class FrameGenerator
    {

        // inputs
        //
        //   jpegs from hardware devices
        //   bitmaps from hardware devices
        //   GPS from stored data or hardware devices
        //   Camera name source info from stored configuration
        //   video files on disk
        //   still images on disk


        // outputs 
        //
        //    bitmaps to LPR, with source, time and GPS
        //    jpegs to DVR, with source, time and GPS
        //    
        //
        //
        //

        public FrameGenerator( APPLICATION_DATA appData,bool AsService)
        {
            try
            {

                m_AppData = appData;
                m_AppData.AddOnClosing(OnClose, APPLICATION_DATA.CLOSE_ORDER.MIDDLE);
                m_Log = (ErrorLog)m_AppData.Logger;

                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.FRAME_GENERATOR.FrameGen_FrameCnt].Peak.RegisterForUse(true);
                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.FRAME_GENERATOR.FrameGen_FrameCnt].PerSecond.RegisterForUse(true);
                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.FRAME_GENERATOR.FrameGen_FrameCnt].RunningAverage.RegisterForUse(true);
                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.FRAME_GENERATOR.FrameGen_FrameCnt].Snapshot.RegisterForUse(true);

                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.FRAME_GENERATOR.FrameGen_MotionDetectionPendingQ].Peak.RegisterForUse(true);
                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.FRAME_GENERATOR.FrameGen_MotionDetectionPendingQ].PerSecond.RegisterForUse(true);
                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.FRAME_GENERATOR.FrameGen_MotionDetectionPendingQ].RunningAverage.RegisterForUse(true);
                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.FRAME_GENERATOR.FrameGen_MotionDetectionPendingQ].Snapshot.RegisterForUse(true);

                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.FRAME_GENERATOR.FrameGen_NonMotionFramePushQ].Peak.RegisterForUse(true);
                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.FRAME_GENERATOR.FrameGen_NonMotionFramePushQ].PerSecond.RegisterForUse(true);
                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.FRAME_GENERATOR.FrameGen_NonMotionFramePushQ].RunningAverage.RegisterForUse(true);
                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.FRAME_GENERATOR.FrameGen_NonMotionFramePushQ].Snapshot.RegisterForUse(true);

                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.FRAME_GENERATOR.FrameGen_GPSLocation].StatString.RegisterForUse(true);

                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.MOTION_DETECTION.MotionDetector_DroppedFrames].Accumulator.RegisterForUse(true);
                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.MOTION_DETECTION.MotionDetector_FrameCnt].Accumulator.RegisterForUse(true);
                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.MOTION_DETECTION.MotionDetector_FramesDetected].Accumulator.RegisterForUse(true);
                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.MOTION_DETECTION.MotionDetector_ProcessQCnt].Snapshot.RegisterForUse(true);
                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.MOTION_DETECTION.MotionDetector_ProcessQCnt].Peak.RegisterForUse(true);

                m_EabledChannelArray = new bool[m_AppData.MAX_PHYSICAL_CHANNELS];// used tell the 2255 device which of its channels are enabled

                m_LastJPEGReceived = new LAST_JPEG_RECEIVED[m_AppData.MAX_PHYSICAL_CHANNELS];
                m_Channels = new CHANNEL[m_AppData.MAX_PHYSICAL_CHANNELS];
                for (int c = 0; c < m_AppData.MAX_PHYSICAL_CHANNELS; c++)
                {
                    m_LastJPEGReceived[c] = new LAST_JPEG_RECEIVED();
                    m_Channels[c] = new CHANNEL(c);
                }

                LoadChannelNames();



                m_ConsumerIDs = new CONSUMER_ID();

                m_CurrentGPSPosition = m_NoPositionAvailable;


                m_MotionDetectionQ = new ThreadSafeQueue<FRAME>(m_MotionDetectionQueLevel, "QueueOverruns_FG_MotionDetectionQ", m_AppData);
                m_AppData.MotionDetectionGettingBehind = false; 


                //////////////////////////////////////
                //
                // start the thread that pushes new frames to the registered consumers

                m_AllFramesConsumerPushQ = new ThreadSafeQueue<CONSUMER_PUSH>(240, "QueueOverruns_FG_AllFramesConsumerPushQ", m_AppData);

                m_MotionDetectedConsumerPushQ = new ThreadSafeQueue<CONSUMER_PUSH>(240, "QueueOverruns_FG_MotionDetectedConsumerPushQ", m_AppData); //120

                PushThread = new Thread(PushLoop);
                PushThread.Priority = ThreadPriority.AboveNormal;
                PushThread.Start();

                m_MotionDetectionThread = new Thread(MotionDetectionLoop);
                m_MotionDetectionThread.Start();




                //////////////////////////////////////
                //
                // start the S2255 controller

                // the 2255 controller has a polling loop that looks for 2255 devices to be added/deleted by the user plugging/unplugging the cables
                //  as new devices are detected, the stored config data is read and the channels are assigned and start running as appropriate
                //   the image data flows into this class via callbacks. this class then pushes the data up a layer after adding GPS and time stamps.

                if (AsService)
                {
                    try
                    {


                        
                        S2255Controller.S2255Controller.PAL_NTSC_MODE videoStandard = S2255Controller.S2255Controller.PAL_NTSC_MODE.NTSC;

                        string NTSC_PAL = UserSettings.Get(UserSettingTags.VideoSetup_PAL_NTSC);
                        if (NTSC_PAL != null)
                        {
                            if (NTSC_PAL.Equals(UserSettingTags.VideoSetup_PAL))
                                videoStandard = S2255Controller.S2255Controller.PAL_NTSC_MODE.PAL;
                        }

                        unsafe
                        {
                            m_S2255Controller = new S2255Controller.S2255Controller(videoStandard, m_AppData, m_EabledChannelArray);
                            m_S2255Controller.OnNewFrame += new S2255Controller.S2255Controller.OnNewFrameFromDeviceEvent(OnReceiveNewImageFromS2255Device);
                            m_S2255Controller.StartThreads();
                        }
                    }
                    catch (Exception ex)
                    {
                        m_Log.Trace(ex, ErrorLog.LOG_TYPE.FATAL);
                    }


                    //////////////////////////////////////
                    //
                    // start the GPS Controller


                    string GPSPort = FindDevicePort.GetGPSCommPort();
                    m_GPSController = new GPSController(PutNewGPSData, m_AppData);

                    if (m_S2255Controller.GetReadyStatus)
                        m_FrameGenReadyStatus = true;
                    else
                        m_FrameGenReadyStatus = false;
                }

                if (!AsService)
                {
                    MovieFileController = new MovieFiles(m_AppData);
                    MovieFileController.OnNewImage += new MovieFiles.OnNewImageEvent(MovieFiles_OnNewImage);
                    MovieFileController.Start();
                }

            }
            catch (Exception ex) { m_Log.Trace(ex, ErrorLog.LOG_TYPE.FATAL);}
             

        }


        int m_MotionDetectionQueLevel = 240;

        public MovieFiles MovieFileController;

        bool m_FrameGenReadyStatus = false;
        public bool GetReadyStatus
        { get{return m_FrameGenReadyStatus ;} }

        ErrorLog m_Log;
        GPSController m_GPSController;

        APPLICATION_DATA m_AppData;

        S2255Controller.S2255Controller m_S2255Controller;

        CHANNEL[] m_Channels;

        public delegate void NotificationOfNewFrameReady(FRAME frame);

        bool[] m_EabledChannelArray;// used to pass in to the S2255 device object

        void LoadChannelNames()
        {
            if (m_AppData.RunninAsService)
            {
                for (int c = 0; c < m_AppData.MAX_PHYSICAL_CHANNELS; c++)
                {
                    m_Channels[c].Enabled = false;
                    m_EabledChannelArray[c] = false;

                    m_Channels[c].Name = UserSettings.Get(UserSettingTags.ChannelNames.Name(c));
                    if (m_Channels[c].Name == null) continue;
                    if (!m_Channels[c].Name.Equals(UserSettingTags.ChannelNotUsed))
                    {
                        m_EabledChannelArray[c] = true;
                        m_Channels[c].Enabled = true;
                    }

                }
            }
            else
            {      /// else running from Analysts Workstation
                for (int c = 0; c < m_AppData.MAX_VIRTUAL_CHANNELS; c++)
                {
                    m_Channels[c].Enabled = true;
                    m_EabledChannelArray[c] = true;
                }
            }

        }

        /// <summary>
        /// Returns an array of strings with the names of the source channels, as configured by the user
        /// </summary>
        /// <returns></returns>
        public string[] GetChannelList()
        {
            List<string> names = new List<string>();

            for (int c = 0; c < m_AppData.MAX_PHYSICAL_CHANNELS; c++) 
            {
                names.Add(m_Channels[c].Name);
            }

            string[] list = names.ToArray();

            return (list);
        }

        /// <summary>
        /// Get the system channel index from the channel name - used by remote monitors to request image streams by name
        /// </summary>
        /// <param name="channelName"></param>
        /// <returns></returns>
        public int GetChannelIndex(string channelName)
        {
            for (int c = 0; c < m_AppData.MAX_PHYSICAL_CHANNELS; c++)
            {
                if ( m_Channels[c].Name.Equals(channelName))
                    return (c);
            }
            return (0);
        }

        
        /// <summary>
        /// Returns the number of physical channels available to be configured
        /// </summary>
        /// <returns></returns>
        public int GetNumberOfPhysicalChannels()
        {
            return (m_AppData.MAX_PHYSICAL_CHANNELS); // TBD, implement hardware polling
        }


        /////////////////////////////////////////////////////////////
        ///
        ///    Consumers - a consumer receives FRAMES, such as LPR, DVR, Remote Connection Server
        ///    
        ///     each consumer must register to receive FRAMES
        ///     
        ///     Registration required requesting a consumer ID from the FrameGenerator
        ///     
        ///     The consumer ID is then used in source requests
        ///     
        class CONSUMER_ID
        {
            
            public bool[] inUse;
            object singleton;
            const int MAX_CONSUMERS=32;
            int consumerCount = 0;

            public CONSUMER_ID()
            {
                singleton = new object();

                inUse = new bool[MAX_CONSUMERS];

                for (int i = 0; i < MAX_CONSUMERS; i++)
                {
                    inUse[i] = false;
                }
            }

            public int GetNewID()
            {
                lock (singleton)
                {
                    for (int i = 0; i < MAX_CONSUMERS; i++)
                    {
                        if (!inUse[i])
                        {
                            inUse[i] = true;
                            consumerCount = i + 1;
                            return (i);
                        }
                    }
                    return (-1);
                }
            }

            public void ReleaseID(int id)
            {
                lock (singleton)
                {
                    if (id >= 0 && id < MAX_CONSUMERS)
                    {
                        consumerCount--;
                        inUse[id] = false;
                    }
                }
            }

           public int GetConsumerCount 
           {
               get { lock (singleton) { return consumerCount; } }
           }

        }
        CONSUMER_ID m_ConsumerIDs;
        /// <summary>
        /// This routine assigned a consumer ID. A consumer ID is required before registering to consume frames.
        /// </summary>
        /// <returns></returns>
        public int GetNewConsumerID ( )
        {
            return (m_ConsumerIDs.GetNewID());
        }

        /// <summary>
        /// Release a consumer ID after unregistering to consume frames.
        /// </summary>
        /// <param name="id"></param>
        public void ReleaseConsumerID(int id)
        {
            m_ConsumerIDs.ReleaseID(id);
        }

        /////////////////////////////////////////////////////////////
        ///
        ///    Channel - a channel is a video source (such as a S2255 port-pair or single port, or an IP camera videos stream)
        ///    
        ///     channels produce FRAMES - a FRAME contains all the supported image formats and meta data associatd with the source
        ///     
        ///     consumers register to receive FRAMES from channels
        ///     
        ///     the Channel registers to receive images from the video source controller such as the S2255Controller
        ///     
        public enum CHANNEL_TYPES { S2255PortPairs, S2255SinglePorts, AXIS_CAMERA}
        class CHANNEL
        {
            object singleton;

            bool enabled;
            public bool Enabled
            {
                get { lock (singleton) { return enabled; } }
                set { lock (singleton) { enabled = value; } }
            }

            string name;
            public string Name { set { lock (singleton) { name = value;} } get { lock (singleton) {return(name); } } }

            public ThreadSafeList<ConsumerNotificationInfo> m_NewImageCallBackList; // the consumers of this channel
            public ThreadSafeList<ConsumerNotificationInfo> m_MotionDetectedCallBackList; // the consumers of this channel
            int index;

            public CHANNEL(int idx)
            {
                index = idx;
                
                name = idx.ToString(); // default channel name

                singleton = new object();
                m_NewImageCallBackList = new ThreadSafeList<ConsumerNotificationInfo>(30);
                m_MotionDetectedCallBackList = new ThreadSafeList<ConsumerNotificationInfo>(30);
            }
        }
       

        /// <summary>
        /// Register to receive new frames as they arrive. Copy the frame off to a new instance, then release the frame. Do not process on this thread.
        /// </summary>
        /// <param name="consumerID">Get a consumer ID from GetNewConsumerID() before this call.</param>
        /// <param name="chan">A portpair channel index starting at 0</param>
        /// <param name="callback">Callback to get new frame</param>
        public void RegisterToConsumeChannel (int consumerID, int chan, NotificationOfNewFrameReady callback )
        {
            if (chan < 0 || chan > m_AppData.MAX_PHYSICAL_CHANNELS) return;

            lock (m_Channels)
            {
                ConsumerNotificationInfo consumer = new ConsumerNotificationInfo(callback, chan, consumerID);
                m_Channels[chan].m_NewImageCallBackList.Add(consumer);
            }
        }

        /// <summary>
        /// Stop receiving new frames.
        /// </summary>
        /// <param name="consumerID">from GetNewConsumerID()</param>
        /// <param name="chan">A portpair channel index starting at 0</param>
        public void DeRegisterToConsumeChannel(int consumerID, int chan)
        {
            if (chan < 0 || chan > m_AppData.MAX_PHYSICAL_CHANNELS) return;

            lock (m_Channels)
            {
                foreach (ConsumerNotificationInfo consumer in m_Channels[chan].m_NewImageCallBackList)
                {
                    if (consumer.consumerID == consumerID)
                    {
                        m_Channels[chan].m_NewImageCallBackList.Remove(consumer);
                        m_ConsumerIDs.ReleaseID(consumerID);
                        return;
                    }
                }
            }
        }


        /// <summary>
        /// Register to receive new frames if motion is detected. Do not process on this thread.
        /// </summary>
        /// <param name="consumerID"></param>
        /// <param name="chan"></param>
        /// <param name="callback"></param>
        public void RegisterToConsumeMotionDetectedFrames(int consumerID, int chan, NotificationOfNewFrameReady callback)
        {
            if (chan < 0 || chan > m_AppData.MAX_PHYSICAL_CHANNELS) return;

            lock (m_Channels)
            {
                ConsumerNotificationInfo consumer = new ConsumerNotificationInfo(callback, chan, consumerID);
                m_Channels[chan].m_MotionDetectedCallBackList.Add(consumer);
            }
        }
        /// <summary>
        /// Stop receiving motion detected frames.
        /// </summary>
        /// <param name="consumerID">from GetNewConsumerID()</param>
        /// <param name="chan">A portpair channel index starting at 0</param>
        public void DeRegisterToConsumeMotionDetectedFrames(int consumerID, int chan)
        {
            if (chan < 0 || chan > m_AppData.MAX_PHYSICAL_CHANNELS) return;

            lock (m_Channels)
            {
                foreach (ConsumerNotificationInfo consumer in m_Channels[chan].m_NewImageCallBackList)
                {
                    if (consumer.consumerID == consumerID)
                    {
                        m_Channels[chan].m_MotionDetectedCallBackList.Remove(consumer);
                        m_ConsumerIDs.ReleaseID(consumerID);
                        return;
                    }
                }
            }
        }
       

        string m_CurrentGPSPosition;
        string m_NoPositionAvailable = "No Position Available";

        void PutNewGPSData(string position, bool deviceFound, bool satellitesFound)
        {
            lock (m_CurrentGPSPosition)
            {
                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.FRAME_GENERATOR.FrameGen_GPSLocation].StatString.SetValue = position.Replace(',','^');

                if ( ! satellitesFound)
                {
                    m_CurrentGPSPosition = m_NoPositionAvailable;
                }
                else
                    m_CurrentGPSPosition = position;
            }

            if (!deviceFound)
            {
                if (position.Contains("config error: fixed position not set"))
                {
                    m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.System.System_GPS].StatString.SetValue = "No GPS Device; No fixed position set";
                }
                else
                {
                    m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.System.System_GPS].StatString.SetValue = "No GPS Device; using fixed position";
                }
            }
            else
            {
                if (!satellitesFound)
                {
                    m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.System.System_GPS].StatString.SetValue = "Have GPS device; No Satellite Acquisition";
                }
                else
                {
                    m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.System.System_GPS].StatString.SetValue = "Have Satelltie: " + m_CurrentGPSPosition;
                }

            }
         

           // m_Log.Log("GPS position: " + position + ", device found=" + deviceFound.ToString() + " sat found = " + satellitesFound.ToString(), ErrorLog.LOG_TYPE.INFORMATIONAL);
        }

    
        bool m_Stop = false;

        void OnClose()
        {
            m_Stop = true;
            if (MovieFileController != null)
            {
                for (int c = 0; c < m_AppData.MAX_VIRTUAL_CHANNELS; c++)
                {
                    MovieFileController.Stop();
                }
            }
        }




        /////////////////////////////////////////////////////////////
        ///
        ///    FRAME - put together the FRAME when receiving images from channels, and then push the frames up to the consumers.
        ///    

  
        int m_FrameCount = 0;

        class LAST_JPEG_RECEIVED
        {
            Queue<byte[]> jpegObj;

            byte[] lastJpeg;

            public void SetJpeg(byte[] jpg)
            {
           
                lock (Singleton) { if (jpegObj.Count > 2) { jpegObj.Dequeue(); } jpegObj.Enqueue(jpg); lastJpeg = jpg; }
              
            }

            public byte[] GetJpeg()
            {
               
                lock (Singleton) { if (jpegObj.Count > 0) return (jpegObj.Dequeue()); else return (lastJpeg); }
            }

            object Singleton;
           
            public LAST_JPEG_RECEIVED()
            {
                
                Singleton = new object();
                jpegObj = new Queue<byte[]>();
                jpegObj.Enqueue(new byte[10]);// a empty jpeg
                lastJpeg = new byte[10];
            }
        }


        public FRAME CompleteFrameDataToByPassLPR(FRAME frame)
        {
            // used by the image hand-editor in the Analysts Workstation application, to by-pass the LPREngine and send results to storage

            frame.TimeStamp = DateTime.UtcNow;
            frame.SerialNumber = m_FrameCount;
            frame.GPSPosition = m_CurrentGPSPosition;
            frame.PSSName = "AnalystsWorkstation_" + m_AppData.ThisComputerName;
            frame.SetFileName();

            return (frame);
        }

        //   /////////////////////////////////////////////////////////
        //   /////////////////////////////////////////////////////////
        //   /////////////////////////////////////////////////////////


        //     Receiving new frames from the lower layers

      

        // receive a new from from a movie file being played

        void MovieFiles_OnNewImage(FRAME frame)
        {
            // this is a bitmap - send it and the last jpeg received up the chain

            try
            {
                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.FRAME_GENERATOR.FrameGen_FrameCnt].HitMe++;


                // manufacture a jpeg from the bitmap

                Image image = frame.Bmp;

                MemoryStream stream = new MemoryStream();

                image.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);

                byte[] jpeg = stream.ToArray();

                frame.Jpeg = jpeg;
                
                frame.SerialNumber = m_FrameCount;
                frame.GPSPosition = m_CurrentGPSPosition;
                frame.PSSName = m_AppData.ThisComputerName;
                frame.SetFileName();

                m_FrameCount++;

                // send to motion detection
                if (!frame.NotVideoEachFrameIsUniqueSize)
                {
                    m_MotionDetectionQ.Enqueue(frame);

                    if (m_MotionDetectionQ.Count > m_MotionDetectionQueLevel / 2)
                        m_AppData.MotionDetectionGettingBehind = true;
                    else
                        m_AppData.MotionDetectionGettingBehind = false;

                }
                else
                {
                    // skip motion detection because the source is a directory of independent jpegs, each potentially of a different size
                    // pretend we detected motion

                    CONSUMER_PUSH p = new CONSUMER_PUSH();
                    p.FrameToPush = frame;
                    p.ConsumersToPush = m_Channels[frame.SourceChannel].m_MotionDetectedCallBackList;


                    m_MotionDetectedConsumerPushQ.Enqueue(p);
                    
                }

                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.FRAME_GENERATOR.FrameGen_MotionDetectionPendingQ].HitMe = m_MotionDetectionQ.Count;

                // send to non-motion-detection consumers

                CONSUMER_PUSH push = new CONSUMER_PUSH();
                push.FrameToPush = frame;
                push.ConsumersToPush = m_Channels[frame.SourceChannel].m_NewImageCallBackList;

                m_AllFramesConsumerPushQ.Enqueue(push);
                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.FRAME_GENERATOR.FrameGen_NonMotionFramePushQ].HitMe = m_AllFramesConsumerPushQ.Count;

            }
            catch (Exception ex) { m_Log.Trace(ex, ErrorLog.LOG_TYPE.FATAL); }

        }

        LAST_JPEG_RECEIVED[] m_LastJPEGReceived;

        void OnReceiveNewImageFromS2255Device(FRAME partialFrame)
        {


            if (partialFrame.Jpeg != null)
            {
                // this is a jpeg
                m_LastJPEGReceived[partialFrame.SourceChannel].SetJpeg(partialFrame.Jpeg);
            }
            else
            {
               
                // this is a bitmap - send it and the last jpeg received up the chain

                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.FRAME_GENERATOR.FrameGen_FrameCnt].HitMe++;

                int chan = partialFrame.SourceChannel;

                FRAME frame = new FRAME(m_AppData);


                frame.SetNew(partialFrame.Bmp, m_LastJPEGReceived[partialFrame.SourceChannel].GetJpeg(),
                         partialFrame.SourceName, DateTime.UtcNow, m_FrameCount, m_CurrentGPSPosition, m_ConsumerIDs.GetConsumerCount, partialFrame.SourceChannel);


                frame.PSSName = m_AppData.ThisComputerName;


                m_FrameCount++;

                // convert the bitmap format to a luminace 2-D array for image processing
                int[,] luminance = new int[frame.Bmp.Width, frame.Bmp.Height];

                getPixelsFromImageInY(frame.Bmp, ref luminance);

                frame.Luminance = luminance;

                // send to motion detection
                m_MotionDetectionQ.Enqueue(frame);

    
                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.FRAME_GENERATOR.FrameGen_MotionDetectionPendingQ].HitMe = m_MotionDetectionQ.Count;

                // send to non-motion-detection consumers

                CONSUMER_PUSH push = new CONSUMER_PUSH();
                push.FrameToPush = frame;
                push.ConsumersToPush = m_Channels[chan].m_NewImageCallBackList;

                m_AllFramesConsumerPushQ.Enqueue(push);
                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.FRAME_GENERATOR.FrameGen_NonMotionFramePushQ].HitMe = m_AllFramesConsumerPushQ.Count;


                

            }
            
        }

        Thread m_MotionDetectionThread;
        ThreadSafeQueue<FRAME> m_MotionDetectionQ;
        void MotionDetectionLoop()
        {
            FRAME frm = null;
            int count = 0;

            while (!m_Stop)
            {
                try
                {
                    count = 4;
                    while (count > 0)
                    {
                        frm = m_MotionDetectionQ.Dequeue();

                        if (frm != null) DetectMotion(frm);

                        count--;

                        if (!m_Stop) break;
                    }

                    Thread.Sleep(1);
                }
                catch (Exception ex) { m_Log.Trace(ex, ErrorLog.LOG_TYPE.FATAL); }

            }
        }

        
        void DetectMotion(FRAME frame)
        {
            m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.MOTION_DETECTION.MotionDetector_FrameCnt].HitMe++;

            int[,] luminance = frame.Luminance;

            bool motionDetected = false;

            int error = 0;
            try
            {
                motionDetected = LPROCR_Lib.DetectMotion(frame.SourceChannel, luminance, luminance.GetLength(0), luminance.GetLength(1), ref error);
            }
            catch (Exception ex)
            {
                m_Log.Trace(ex, ErrorLog.LOG_TYPE.INFORMATIONAL);
            }

            if (error != 0)
            {
                m_Log.Log("DetectMotion error = " + error.ToString(), ErrorLog.LOG_TYPE.FATAL);
            }

            if (motionDetected)
            {
                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.MOTION_DETECTION.MotionDetector_FramesDetected].HitMe++;


                CONSUMER_PUSH push = new CONSUMER_PUSH();
                push.FrameToPush = frame;
                push.ConsumersToPush = m_Channels[frame.SourceChannel].m_MotionDetectedCallBackList;

              
                m_MotionDetectedConsumerPushQ.Enqueue(push);

            }
        }



        class CONSUMER_PUSH
        {
            public FRAME FrameToPush;
            public ThreadSafeList<ConsumerNotificationInfo> ConsumersToPush;
        }

        ThreadSafeQueue<CONSUMER_PUSH> m_AllFramesConsumerPushQ;
        ThreadSafeQueue<CONSUMER_PUSH> m_MotionDetectedConsumerPushQ;

      
        Thread PushThread;
        public void PushLoop()
        {

            while (!m_Stop)
            {
                try
                {
                    CONSUMER_PUSH consumerPush = null;

                    while (m_AllFramesConsumerPushQ.Count > 0)
                    {

                        consumerPush = m_AllFramesConsumerPushQ.Dequeue();

                        if (consumerPush != null)
                        {
                            foreach (ConsumerNotificationInfo consumer in consumerPush.ConsumersToPush)
                            {
                                consumer.callback(consumerPush.FrameToPush);
                            }
                        }
                    }



                    consumerPush = null;

                    while (m_MotionDetectedConsumerPushQ.Count > 0)
                    {

                        consumerPush = m_MotionDetectedConsumerPushQ.Dequeue();

                        if (consumerPush != null)
                        {
                            foreach (ConsumerNotificationInfo consumer in consumerPush.ConsumersToPush)
                            {
                                consumer.callback(consumerPush.FrameToPush);
                            }
                        }
                    }

                }
                catch (Exception ex) { m_Log.Trace(ex, ErrorLog.LOG_TYPE.FATAL); }

                Thread.Sleep(1);
            }
        }
      
        class ConsumerNotificationInfo
        {
            public  NotificationOfNewFrameReady callback;
            public int portPairIndex;
            public int consumerID;
            public ConsumerNotificationInfo(NotificationOfNewFrameReady cb, int channel, int consumer)
            {
                callback = cb;
                portPairIndex = channel;
                consumerID = consumer;
            }
        }

        unsafe public void getPixelsFromImageInY(Bitmap bmp, ref int[,] Y)
        {

            // Lock the bitmap's bits.  
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            System.Drawing.Imaging.BitmapData bmpData =
                bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                bmp.PixelFormat);

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap.
            int bytes = bmpData.Stride * bmp.Height;

            int pixelOffset = bmpData.Stride / bmp.Width;

            fixed (int* arrayPtr = Y)
            {
                int* srcPtr = (int*)ptr;
                IntPtr dest = new IntPtr((void*)arrayPtr);


                // Copy the RGB values into the array.
                //                LPROCR_Lib.MemCopyInt(srcPtr, dest, bytes / 4);
                LPROCR_Lib.MemCopyByteArrayToIntArray(bmpData.Scan0, dest, bytes, bmp.Width, bmp.Height);
            }

            // Unlock the bits.
            bmp.UnlockBits(bmpData);

        }

        //public static void getPixelsFromImageInY(Bitmap bmp, ref int[,] Y)
        //{

        //    // Lock the bitmap's bits.  
        //    Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
        //    System.Drawing.Imaging.BitmapData bmpData =
        //        bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
        //        bmp.PixelFormat);

        //    // Get the address of the first line.
        //    IntPtr ptr = bmpData.Scan0;

        //    // Declare an array to hold the bytes of the bitmap.
        //    int bytes = bmpData.Stride * bmp.Height;
        //    byte[] rgbValues = new byte[bytes];

        //    int pixelOffset = bmpData.Stride / bmp.Width;

        //    // Copy the RGB values into the array.
        //    System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

        //    int x = 0;
        //    int y = 0;
        //    int b = 0;
        //    int bv = 0;
        //    int rv = 0;
        //    int gv = 0;

        //    for (y = 0; y < Y.GetLength(1); y++)
        //    {
        //        for (x = 0; x < Y.GetLength(0); x++)
        //        {
        //            // some guy on code project says the values are in B G R order
        //            bv = rgbValues[b] * 114;
        //            gv = rgbValues[b + 1] * 587;
        //            rv = rgbValues[b + 2] * 299;

        //            Y[x, y] = ((((bv + rv + gv) / 1000)));

        //            b += pixelOffset;
        //        }
        //        while (b % 4 != 0) b++;
        //    }


        //    // Unlock the bits.
        //    bmp.UnlockBits(bmpData);

        //}

    }
}
