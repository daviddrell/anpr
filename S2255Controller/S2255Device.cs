using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices; //For DLL support
using ApplicationDataClass;
using System.Threading;
using UserSettingsLib;
using ErrorLoggingLib;
using LPROCR_Wrapper;
using Utilities;

namespace S2255Controller
{
    
    /// <summary>
    /// Create one instance of this class for each physical hardware unit known as the S2255 Frame Grabber
    /// </summary>
    public class S2255Device
    {
        
        public S2255Device(int index,int handle, APPLICATION_DATA appData, int videoStandard, S2255Controller.S2255DevicePortChannelMappings portMappings)
        {
            m_DeviceHandle = handle;
            m_DeviceIndex = index;
            m_AppData = appData;
            m_VideoStandard = videoStandard;
            m_PortMappings = portMappings;

            singleton = new object();

         //   m_AppData.AddOnClosing(Stop, APPLICATION_DATA.CLOSE_ORDER.FIRST);
 
            OnNewFrame = new OnNewFrameEvent[4];
         
            unsafe
            {
                _Obj_of_Type_HANDLE_NEW_BUFFER_FROM_S2255 = OnNewFrameFromDevice;

                unsafeMethodCallbackPtr = Marshal.GetFunctionPointerForDelegate(_Obj_of_Type_HANDLE_NEW_BUFFER_FROM_S2255);
            }

            m_Log = (ErrorLog)m_AppData.Logger;

            TimeAtLastFrame = DateTime.Now;

            m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.PHYSICAL_CHANNELS.PhysicalChannels_c0].Peak.RegisterForUse(true);
            m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.PHYSICAL_CHANNELS.PhysicalChannels_c1].Peak.RegisterForUse(true);
            m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.PHYSICAL_CHANNELS.PhysicalChannels_c2].Peak.RegisterForUse(true);
            m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.PHYSICAL_CHANNELS.PhysicalChannels_c3].Peak.RegisterForUse(true);
           
            m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.PHYSICAL_CHANNELS.PhysicalChannels_c0].PerSecond.RegisterForUse(true);
            m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.PHYSICAL_CHANNELS.PhysicalChannels_c1].PerSecond.RegisterForUse(true);
            m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.PHYSICAL_CHANNELS.PhysicalChannels_c2].PerSecond.RegisterForUse(true);
            m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.PHYSICAL_CHANNELS.PhysicalChannels_c3].PerSecond.RegisterForUse(true);
           
            m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.PHYSICAL_CHANNELS.PhysicalChannels_c0].RunningAverage.RegisterForUse(true);
            m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.PHYSICAL_CHANNELS.PhysicalChannels_c1].RunningAverage.RegisterForUse(true);
            m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.PHYSICAL_CHANNELS.PhysicalChannels_c2].RunningAverage.RegisterForUse(true);
            m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.PHYSICAL_CHANNELS.PhysicalChannels_c3].RunningAverage.RegisterForUse(true);
           
        }

        S2255Controller.S2255DevicePortChannelMappings m_PortMappings;
        object singleton;

        int m_DeviceHandle;
        public int DeviceHandle
        { get { return m_DeviceHandle; } set { m_DeviceHandle = value; } }

        string[] chanSourceNames;
        int m_DeviceIndex;
        public int DeviceIndex
        {
            get { return (m_DeviceIndex); }
        }
        int m_VideoStandard;
        public delegate void OnNewFrameEvent(FRAME frame);
        public OnNewFrameEvent[] OnNewFrame;// allows consumers to register to receive new frames

        ErrorLog m_Log;
        APPLICATION_DATA m_AppData;
        public DateTime TimeAtLastFrame;
   

        public bool UserConfigured;
        public bool DetectedFrames = false;
        public int frameCountPerInterval;

    
        public enum DEVICE_STATE { OPEN, CLOSED }
        public enum DEVICE_MODEL { S2255 }

        DEVICE_MODEL m_DeviceModel;
        public DEVICE_MODEL Model { set { m_DeviceModel = value; } get { return m_DeviceModel; } }

        DEVICE_STATE m_DeviceState;
        public DEVICE_STATE State { set { m_DeviceState = value; } get { return m_DeviceState; } }

        unsafe delegate void HANDLE_NEW_BUFFER_FROM_S2255(int jpegLength, int* binfo, byte* image, int cindex, int frameIndex);
        HANDLE_NEW_BUFFER_FROM_S2255 _Obj_of_Type_HANDLE_NEW_BUFFER_FROM_S2255;
        unsafe  IntPtr unsafeMethodCallbackPtr;

        Thread m_StopperThread;
      
        bool m_Stop = false;


        /// <summary>
        /// used to test if a 2255 devices exists at the index number. This is the only way to discover which
        /// logical port/index the device was assigned by the 2255 USB driver
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static bool TestOpenDevice(int handle)
        {
            int result;


            result = Open2255Device(handle, 0);

            if (result == 1)
            {
                Close2255Device(handle);
            }

            if (result == 1) return true;

            return false;


        }


        public bool OpenDevice()
        {
            lock (singleton)
            {
                int v = Open2255Device(m_DeviceHandle, m_VideoStandard);
                if (v == 1)
                {
                   
                    State = S2255Device.DEVICE_STATE.OPEN;
                    StartChannels();
                    return (true);
                }
                else
                {
                    State = S2255Device.DEVICE_STATE.CLOSED;
                    return (false);
                }
            }
        }

        public bool ChannelsStarted = false;

        public void StartChannels ( )
        {
            if (State != DEVICE_STATE.OPEN) return;

            for (int c = 0; c < 4; c++)
            {
                if ( m_PortMappings.enabled[c] )
                    StartChannel(c);
            }
       
            TimeAtLastFrame = DateTime.Now;
            ChannelsStarted = true;

        }

        public void CloseDevice()
        {
            lock (singleton)
            {
                try
                {
                    StopChannels();

                    int count = 10;
                    while (!m_AllChannelsStopped && count-- > 0)
                        Thread.Sleep(1000);

                    Close2255Device(m_DeviceHandle);
                    State = S2255Device.DEVICE_STATE.CLOSED;
                }
                catch (Exception ex) { m_Log.Trace(ex, ErrorLog.LOG_TYPE.FATAL); }
            }
        }

        /// <summary>
        /// Give the device the entire list of channel names, allows this device to calculate its own channel number and to select the channel name from the list
        /// </summary>
        /// <param name="names"></param>
        public void SetSourceNames(string[] names)
        {
            chanSourceNames = names;
        }

   

        void Stop()
        {
            m_Stop = true;
            StopChannels();
        }

       

        /// <summary>
        /// Start one of the channels on this device, chan is 0 based
        /// </summary>
        /// <param name="chan">valid range is 0 through 3</param>
      

        void StartChannel(int chan)
        {
            m_AllChannelsStopped = false;

            S2255Controller.COMPRESSION_MODE compression;
            unsafe
            {
                compression = m_PortMappings.compressionMode[chan];

                SetChannelMode(m_DeviceHandle, chan + 1 /* the 2255 driver from sensoray has chan index start at 1*/, (int)m_VideoStandard, (int)compression);
                StartAcquisitionThread(m_DeviceHandle, chan + 1, unsafeMethodCallbackPtr);
             
            }
        }

        /// <summary>
        /// Stop one of the channels on this device, chan is 0 based
        /// </summary>
        /// <param name="chan">valid range is 0 through 3</param>
        public void StopChannel(int chan)
        {
           
            unsafe
            {
                StopAcquisitionThread(m_DeviceHandle, chan + 1);
               
            }
        }

        /// <summary>
        /// Register to receive new frames as they arrive on channel 0 to 3 of this S2255 device
        /// </summary>
        /// <param name="chan"></param>
        public void RegisterToReceiveFrames(int chan, OnNewFrameEvent eventHandler)
        {
            if (chan < 0 || chan > 3) return;

            OnNewFrame[chan] += eventHandler;
        }

        /// <summary>
        /// De-Register to receive new frames as they arrive on channel 0 to 3 of this S2255 device
        /// </summary>
        /// <param name="chan"></param>
        public void DeRegisterToReceiveFrames(int chan, OnNewFrameEvent eventHandler)
        {
            if (chan < 0 || chan > 3) return;

            OnNewFrame[chan] -= eventHandler;
        }


        public bool GetChannelConnectionStatus(int chan)
        {

            if (State == DEVICE_STATE.OPEN) return true;
            else return false;

            // calling GetVideoConnectionStatus causes a pause in the video stream, so its out for now

            //int s1 = GetVideoConnectionStatus(m_DeviceIndex, chan + 1);// channel starts at 1       
            //if (s1 == 0) return (false);
            //else return (true);

        }

        public void StopChannels()
        {

            m_Stop = true;

            // because the StopAcquisitionThread call blocks until the 2255Interface thread stops, we can get thread deadlock since that thread may be blocking on giving this entity a new frame callback
            // so spin off a stopper thread
            m_StopperThread = new Thread(ReallyStopChannels);
            m_StopperThread.Start();

        }

        void ReallyStopChannels()
        {

            if (m_DeviceState == DEVICE_STATE.CLOSED) return;

            try
            {
                unsafe
                {

                    for (int c = 0; c < 4; c++)
                    {
                        if (m_PortMappings.enabled[c])
                        {
                            StopAcquisitionThread(m_DeviceHandle, c + 1);
                            SetDontHaveVideo(c);
                        }
                    }


                    m_AllChannelsStopped = true;
                    ChannelsStarted = false;
                }
            }
            catch (Exception ex) { m_Log.Trace(ex, ErrorLog.LOG_TYPE.FATAL); }


        }

        bool m_AllChannelsStopped = false;

        TimeSpan OneSecond = new TimeSpan(0, 0, 1);

        unsafe void OnNewFrameFromDevice(int jpegLength, int* binfo, byte* image, int cindex/* 0 base */, int frameIndex)
        {
            int channel = 0;

            TimeAtLastFrame = DateTime.Now;

            if (jpegLength > 40000) return;// its garbage - does not happen often but have seen this

            DetectedFrames = true;
            frameCountPerInterval++;


            // current channel mapping:
            //   each device has four ports. a camera will be connected in parrallel to two ports.
            //   one port of the port pair will be setup as jpeg ecoded and the other is raw bitmap

            //  on device 0
            //   port 0 and 1 will be camera channel 0
            //   port 2 and 3 will be camera channel 1
            //  on device 1
            //   port 0 and 1 will be camera channel 2
            //   port 2 and 3 will be camera channel 3

            channel = m_PortMappings.globalChannIndex[cindex];

            //if (m_DeviceIndex == 0)
            //{
            //    if (cindex == 0 || cindex == 1) channel = 0;
            //    if (cindex == 2 || cindex == 3) channel = 1;
            //}

            //if (m_DeviceIndex == 1)
            //{
            //    if (cindex == 0 || cindex == 1) channel = 2;
            //    if (cindex == 2 || cindex == 3) channel = 3;
            //}
           

            if (jpegLength == 0)
            {
                // its a bitmap 
                HitStats(channel, 1);
                handleBitmap(binfo, image, channel, frameIndex);
            }
            else
            {

                handleJpeg(jpegLength, image, channel);
            }
        }



        unsafe void handleJpeg(int len, byte* imageBytes, int cindex)
        {
            if (imageBytes == null) return;

          

            try
            {
                if (len == 0) return;

                byte[] image = new byte[len];


                Marshal.Copy(new IntPtr(imageBytes), image, 0, len);


                SendImageToConsumer(image, S2255Controller.COMPRESSION_MODE.JPEG, cindex);

            }
            catch (Exception ex)
            {
                m_Log.Log("handleJpeg ex: " + ex.Message, ErrorLog.LOG_TYPE.FATAL);
            }
        }

        unsafe void handleBitmap(int* binfo, byte* image, int cindex, int frameIndex)
        {

            try
            {

                if (m_Stop) return;

                // compose a managed bitmap from the bitmap parts being delivered

                System.Drawing.Imaging.BitmapData srcBmpHeader = new System.Drawing.Imaging.BitmapData();
                Marshal.PtrToStructure((IntPtr)binfo, srcBmpHeader);

                //pDoc->m_buf[idx].lpbmi[frm_idx]->bmiHeader.biSizeImage

                // for some reason stride and width a swapped in position between the 2255 definition and the windows bitmpa definition
                int width = srcBmpHeader.Height;
                int height = srcBmpHeader.Stride;

                Rectangle rect = new Rectangle(0, 0, width, height);

                //     Bitmap nBmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                Bitmap nBmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);



                System.Drawing.Imaging.BitmapData bmpData = nBmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, nBmp.PixelFormat);

                // Get the address of the first line.
                //  int byteCount = width * height * 3;
                int byteCount = width * height;

                //byte[] byteArray = new byte[byteCount];
                //Marshal.Copy(new IntPtr(image), byteArray, 0, byteCount);
                //Marshal.Copy(byteArray, 0, bmpData.Scan0, byteCount);

                bool detectedNoVideoPresent = false;
                LPROCR_Lib.MemCopyByte((int*)image, bmpData.Scan0, byteCount, ref detectedNoVideoPresent);
                //  LPROCR_Lib.MemCopyInt((int*)image, bmpData.Scan0, byteCount );

                nBmp.UnlockBits(bmpData);


                //  nBmp.RotateFlip(RotateFlipType.RotateNoneFlipY);


                // if there is no video connected, the 2255 device sends frames at 30fps, but they are solid blue-screen.
                if (!detectedNoVideoPresent)
                {
                    SetHaveVideo(cindex);
                    SendImageToConsumer(nBmp, S2255Controller.COMPRESSION_MODE.BITMAP, cindex);
                }
                else
                {
                    SetDontHaveVideo(cindex);
                }
            }
            catch (Exception ex)
            {
                m_Log.Log("handleBitmap ex: " + ex.Message, ErrorLog.LOG_TYPE.FATAL);
            }
        }


        void SetHaveVideo(int cindex)
        {
            switch (cindex)
            {
                case 0:
                    m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.System.System_videoChannel1].StatString.SetValue = "Video Connected";
                    break;

                case 1:
                    m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.System.System_videoChannel2].StatString.SetValue = "Video Connected";
                    break;

                case 2:
                    m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.System.System_videoChannel3].StatString.SetValue = "Video Connected";
                    break;

                case 3:
                    m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.System.System_videoChannel4].StatString.SetValue = "Video Connected";
                    break;

            }
        }

        void SetDontHaveVideo(int cindex)
        {
            switch (cindex)
            {
                case 0:
                    m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.System.System_videoChannel1].StatString.SetValue = "No Video Connected";
                    break;

                case 1:
                    m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.System.System_videoChannel2].StatString.SetValue = "No Video Connected";
                    break;

                case 2:
                    m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.System.System_videoChannel3].StatString.SetValue = "No Video Connected";
                    break;

                case 3:
                    m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.System.System_videoChannel4].StatString.SetValue = "No Video Connected";
                    break;

            }

        }

        void SendImageToConsumer(object image, S2255Controller.COMPRESSION_MODE frmType, int cindex)
        {
            FRAME frame = new FRAME(m_AppData);

            if (frmType == S2255Controller.COMPRESSION_MODE.BITMAP)
            {
                frame.Bmp = (Bitmap)image;
                frame.Jpeg = null;
            }
            else
            {
                frame.Bmp = null;
                frame.Jpeg = (byte[])image;
            }

            frame.SourceName = chanSourceNames[cindex];
            frame.SourceChannel = cindex;

            if (OnNewFrame[frame.SourceChannel] != null) OnNewFrame[frame.SourceChannel](frame);
       
        }


        void HitStats(int chan, int value)
        {
            switch (chan)
            {
                case 0:
                    m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.PHYSICAL_CHANNELS.PhysicalChannels_c0].HitMe = value;
                    break;
                case 1:
                    m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.PHYSICAL_CHANNELS.PhysicalChannels_c1].HitMe = value;
                    break;
                case 2:
                    m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.PHYSICAL_CHANNELS.PhysicalChannels_c2].HitMe = value;
                    break;
                case 3:
                    m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.PHYSICAL_CHANNELS.PhysicalChannels_c3].HitMe = value;
                    break;
            }
        }

        [DllImport("S2255Interface.dll", CallingConvention = CallingConvention.StdCall)]
        unsafe static extern int SetChannelMode(int deviceHandle, int channel, int standard, int mode);

        [DllImport("S2255Interface.dll", CallingConvention = CallingConvention.StdCall)]
        unsafe static extern int GetVideoConnectionStatus(int deviceHandle, int channel);

        [DllImport("S2255Interface.dll", CallingConvention = CallingConvention.StdCall)]
        unsafe static extern void StartAcquisitionThread(int deviceHandle, int channel, IntPtr buffFilledCB);


        [DllImport("S2255Interface.dll", CallingConvention = CallingConvention.StdCall)]
        unsafe static extern void StopAcquisitionThread(int deviceHandle, int channel);

        [DllImport("S2255Interface.dll", CallingConvention = CallingConvention.StdCall)]
        unsafe static extern int Open2255Device(int deviceHandle, int standard);

        [DllImport("S2255Interface.dll", CallingConvention = CallingConvention.StdCall)]
        unsafe static extern int Close2255Device(int deviceHandle);

    }


}
