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
using System.Management;
using System.Management.Instrumentation;

namespace S2255Controller
{
    public class S2255Controller
    {

        // auto detect insertion and de-insertion of the USB based 2255 frame grabbers
        //
        //  polling loop checks the OS for a list of connected 2255 devices.
        // 
        //  if the detected device count is greater than the connected device count, try to open a new device
        //  if the detected device count is less than the connected device count, close the missing device.

        //  problem: the OS cannot tell me which device is connected/disconnected.
        //   so if we think we lost a device, then check each open device for time stamp at last received frame, and if
        //    its been a while since a frame was received, assume that is the missing frame grabber and close it.

        //  each frame grabber is associated with a device index which is used by the 2255 USB driver to identify a device.
        //  but we have no way of knowing which device index is assigned to a device by the driver except to
        //  attempt to open the device at each index until we get success on OpenDevice(index).  So a test open
        // function is in the S2255Device class which opens and then closes if the open was sucessful.

        //  the S2255Device class is a class which is designed to control a single instance of a 2255 physical device. So if
        //  there are two frame grabbers attached there should be two instances of the S2255Device class.
        
        unsafe public S2255Controller(PAL_NTSC_MODE mode, APPLICATION_DATA appData, bool [] enableChannels)
        {
            m_PAL_NSTC_mode = mode;
            m_AppData = appData;
            m_EabledSysChannels = enableChannels;

            m_Log = (ErrorLog)m_AppData.Logger;
            m_AppData.AddOnClosing(StopThreads, APPLICATION_DATA.CLOSE_ORDER.FIRST); // how to stop this object and its subthreads

            m_VideoStandard = (mode == PAL_NTSC_MODE.NTSC) ? 0 : 1;

            // allocate 2255Device instances
            m_S2255Devices = new ThreadSafeList<S2255Device>(MAX_S2255DEVICE_COUNT); // max allowed number of connected frame grabbers

            m_DeviceHandlePool = new DeviceValuePoolManager(MAX_2255DEVICE_HANDLE_VALUE); // maximum value of device index that could be assigned by the USB driver

            m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.System.System_frameGrabber_1].StatString.SetValue = "No Device";
            m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.System.System_frameGrabber_2].StatString.SetValue = "No Device";
            m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.System.System_videoChannel1].StatString.SetValue = "No Video Connected";
            m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.System.System_videoChannel2].StatString.SetValue = "No Video Connected";
            m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.System.System_videoChannel3].StatString.SetValue = "No Video Connected";
            m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.System.System_videoChannel4].StatString.SetValue = "No Video Connected";


            m_UsedDeviceIndexes = new DeviceValuePoolManager(MAX_S2255DEVICE_COUNT);


            // current channel mapping:
            //   each S2255 device has four ports. a camera will be connected in parrallel to two ports.
            //   one port of the port pair will be setup as jpeg ecoded and the other is raw bitmap

            //  on device 0
            //   port 0 and 1 will be camera channel 0
            //   port 2 and 3 will be camera channel 1
            //  on device 1
            //   port 0 and 1 will be camera channel 2
            //   port 2 and 3 will be camera channel 3

            m_S2255DevicePortChannelMappsings = new S2255DevicePortChannelMappings[MAX_S2255DEVICE_COUNT];// set to 2
    
            int deviceCounter = 0;
            for (deviceCounter = 0; deviceCounter < m_S2255DevicePortChannelMappsings.Length; deviceCounter++)
            {
                m_S2255DevicePortChannelMappsings[deviceCounter] = new S2255DevicePortChannelMappings();
            }


            ////////////////    assumes MAX_S2255DEVICE_COUNT == 2 , doing port mappins by hand here

            deviceCounter = 0;

            m_S2255DevicePortChannelMappsings[deviceCounter].globalChannIndex[0] = 0;
            m_S2255DevicePortChannelMappsings[deviceCounter].compressionMode[0] = COMPRESSION_MODE.BITMAP;
            m_S2255DevicePortChannelMappsings[deviceCounter].enabled[0] = m_EabledSysChannels[m_S2255DevicePortChannelMappsings[deviceCounter].globalChannIndex[0]];
          
            m_S2255DevicePortChannelMappsings[deviceCounter].globalChannIndex[1] = 0;
            m_S2255DevicePortChannelMappsings[deviceCounter].compressionMode[1] = COMPRESSION_MODE.JPEG;
            m_S2255DevicePortChannelMappsings[deviceCounter].enabled[1] = m_EabledSysChannels[m_S2255DevicePortChannelMappsings[deviceCounter].globalChannIndex[1]];

            m_S2255DevicePortChannelMappsings[deviceCounter].globalChannIndex[2] = 1;
            m_S2255DevicePortChannelMappsings[deviceCounter].compressionMode[2] = COMPRESSION_MODE.BITMAP;
            m_S2255DevicePortChannelMappsings[deviceCounter].enabled[2] = m_EabledSysChannels[m_S2255DevicePortChannelMappsings[deviceCounter].globalChannIndex[2]];

            m_S2255DevicePortChannelMappsings[deviceCounter].globalChannIndex[3] = 1;
            m_S2255DevicePortChannelMappsings[deviceCounter].compressionMode[3] = COMPRESSION_MODE.JPEG;
            m_S2255DevicePortChannelMappsings[deviceCounter].enabled[3] = m_EabledSysChannels[m_S2255DevicePortChannelMappsings[deviceCounter].globalChannIndex[3]];


            deviceCounter = 1;

            m_S2255DevicePortChannelMappsings[deviceCounter].globalChannIndex[0] = 2;
            m_S2255DevicePortChannelMappsings[deviceCounter].compressionMode[0] = COMPRESSION_MODE.BITMAP;
            m_S2255DevicePortChannelMappsings[deviceCounter].enabled[0] = m_EabledSysChannels[m_S2255DevicePortChannelMappsings[deviceCounter].globalChannIndex[0]];

            m_S2255DevicePortChannelMappsings[deviceCounter].globalChannIndex[1] = 2;
            m_S2255DevicePortChannelMappsings[deviceCounter].compressionMode[1] = COMPRESSION_MODE.JPEG;
            m_S2255DevicePortChannelMappsings[deviceCounter].enabled[1] = m_EabledSysChannels[m_S2255DevicePortChannelMappsings[deviceCounter].globalChannIndex[1]];

            m_S2255DevicePortChannelMappsings[deviceCounter].globalChannIndex[2] = 3;
            m_S2255DevicePortChannelMappsings[deviceCounter].compressionMode[2] = COMPRESSION_MODE.BITMAP;
            m_S2255DevicePortChannelMappsings[deviceCounter].enabled[2] = m_EabledSysChannels[m_S2255DevicePortChannelMappsings[deviceCounter].globalChannIndex[2]];

            m_S2255DevicePortChannelMappsings[deviceCounter].globalChannIndex[3] = 3;
            m_S2255DevicePortChannelMappsings[deviceCounter].compressionMode[3] = COMPRESSION_MODE.JPEG;
            m_S2255DevicePortChannelMappsings[deviceCounter].enabled[3] = m_EabledSysChannels[m_S2255DevicePortChannelMappsings[deviceCounter].globalChannIndex[3]];

        }

        public class S2255DevicePortChannelMappings
        {
            public int[] globalChannIndex;// globally referenced throughout the system, 0 to 3 for a four channel(camera) system
            public COMPRESSION_MODE[] compressionMode;
            public bool[] enabled;
            public S2255DevicePortChannelMappings() 
            { 
                globalChannIndex = new int[4];/* there are four physical ports on a S2255 device */
                compressionMode = new COMPRESSION_MODE[4];
                enabled = new bool[4];
            }
        }


       
       

        bool[] m_EabledSysChannels;

        S2255DevicePortChannelMappings[] m_S2255DevicePortChannelMappsings;

        /// <summary>
        /// Device Indexes ==> the device index is 0 for the first device, 1 for the second, 2 for the 3rd, etc.
        /// If device 0 is lost, then the next device connected gets 0. 
        /// So the device indexes run contiguously so that it can be used as an array indexer for connected devices.
        /// </summary>
        DeviceValuePoolManager m_UsedDeviceIndexes;

        /// <summary>
        /// The device handle is used by the 2255 USB driver to identify a particular device. But we do not know what handle
        /// is assigned by the driver to a connected unit unless we attempt to open at each handle until we get a valid open result. The handle
        /// values run from 0 to 8 (see 2255.h: #define SYS_GRABBERS            8       //max frame grabbers in the system;
        /// </summary>
        DeviceValuePoolManager m_DeviceHandlePool;


        bool m_S2255ControllerReady = false;

        public bool GetReadyStatus
        { get { return m_S2255ControllerReady; } }

      
        public enum PAL_NTSC_MODE { PAL = 1, NTSC = 0 }// s2255InterfaceAPI.h
        PAL_NTSC_MODE m_PAL_NSTC_mode;
        APPLICATION_DATA m_AppData;
        int m_VideoStandard;

        //#define S2255_MODE_JPEG 1  // s2255InterfaceAPI.h
        //#define S2255_MODE_BITMAP 2
        public enum COMPRESSION_MODE { JPEG = 1, BITMAP = 2 }

        Thread m_PollForDevicesThread;

        ErrorLog m_Log;

        const int MAX_S2255DEVICE_COUNT = 2;


                                               // the sensorary supplied USB driver uses device indexes for physical device control. The valid values for device indexes are 0 to 7
        const int MAX_2255DEVICE_HANDLE_VALUE = 8;// defined in sensory supplied file s2255.h as:  #define SYS_GRABBERS            8       //max frame grabbers in the system;


        ThreadSafeList<S2255Device> m_S2255Devices = new ThreadSafeList<S2255Device>(MAX_S2255DEVICE_COUNT);

        public delegate void OnNewFrameFromDeviceEvent(FRAME frame);
        public event OnNewFrameFromDeviceEvent OnNewFrame;

   
        void OnNewFrameFromDevice(FRAME frame)
        {
            // send new frame to registered consumers

            if (OnNewFrame != null) OnNewFrame(frame);
        }



        S2255Device CreateAndStartNew2255( int index, int handle)
        {

            string[] chanNames = null;
            bool[] configedchannels = GetConfiguredChannels(out chanNames);

            S2255Device device = new S2255Device(index, handle, m_AppData, m_VideoStandard, m_S2255DevicePortChannelMappsings[index]);
            device.SetSourceNames(chanNames);
            device.State = S2255Device.DEVICE_STATE.CLOSED;
            device.Model = S2255Device.DEVICE_MODEL.S2255;
            device.UserConfigured = true;

            for (int c = 0; c < m_AppData.MAX_PHYSICAL_CHANNELS; c++)
            {
               
                    device.RegisterToReceiveFrames(c, OnNewFrameFromDevice);
                
            }

            device.OpenDevice();
  

            m_Log.Log("started 2255 FrameGrabber at handle " + handle.ToString(), ErrorLog.LOG_TYPE.INFORMATIONAL);

            m_DeviceHandlePool.MarkValueAsUsed(handle);
            m_UsedDeviceIndexes.MarkValueAsUsed(index);

            if (GetOpenDeviceCount() == 2)
            {
                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.System.System_frameGrabber_2].StatString.SetValue = "Have Device";
            }
            else if (GetOpenDeviceCount() == 1)
            {
                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.System.System_frameGrabber_1].StatString.SetValue = "Have Device";
            }


            return (device);
        }


        void CloseDevice( S2255Device dev)
        {
            m_Log.Log("stopped 2255 FrameGrabber at index " + dev.DeviceIndex.ToString(), ErrorLog.LOG_TYPE.INFORMATIONAL);

            dev.CloseDevice();

            m_UsedDeviceIndexes.ReleaseValue(dev.DeviceIndex); // free this index

            m_DeviceHandlePool.ReleaseValue(dev.DeviceHandle);// free the handle

            // this indicator does not really tell you which device is lost, only that a device is lost. the user will see one or two device present indicators,
            // but no indication of which device is there
            if (GetOpenDeviceCount() == 1)
            {
                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.System.System_frameGrabber_2].StatString.SetValue = "No Device";
            }
            else if (GetOpenDeviceCount() == 0)
            {
                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.System.System_frameGrabber_1].StatString.SetValue = "No Device";
            }
        }



      

        bool[] GetConfiguredChannels(out string[] names)
        {
            bool[] configuredChannels = new bool[m_AppData.MAX_PHYSICAL_CHANNELS];
            names = new string[m_AppData.MAX_PHYSICAL_CHANNELS];

            for(int c = 0; c < m_AppData.MAX_PHYSICAL_CHANNELS; c++)
            {
                configuredChannels[c] = false;
                names[c] = UserSettings.Get(UserSettingTags.ChannelNames.Name(c));

                if ( (names[c] != null) && (!names[c].Equals(UserSettingTags.ChannelNotUsed)))
                {
                    configuredChannels[c] = true;
                }
                
            }
            return (configuredChannels);
        }

       
        public void StartThreads()
        {
            m_PollForDevicesThread = new Thread(PollAttachedDeviceChanges);
            m_PollForDevicesThread.Start();

            m_S2255ControllerReady = true;

        }


        bool m_StopThreads = false;

        public void StopThreads()
        {
            m_StopThreads = true;
            try
            {

                foreach (S2255Device dev in m_S2255Devices)
                {
                    dev.CloseDevice();
                }

                m_S2255Devices.Clear();

            }
            catch (Exception ex)
            {
                m_Log.Log("S2255Controller StopThreads ex: " + ex.Message, ErrorLog.LOG_TYPE.FATAL);
            }
        }


       

        void PollAttachedDeviceChanges()
        {
            // check to see if the user attached a device or removed a device
            //  this is also how we find devices attached at start up

            TimeSpan frameTimeOut = new TimeSpan(0, 0, 1);
            S2255Device DeviceToRemoveFromList = null;

            while (!m_StopThreads)
            {

                int attachedCount = GetNumberAttached2255Devices();

                if (attachedCount < GetOpenDeviceCount())
                {
                    DeviceToRemoveFromList = null;

                    // we lost a device, but which one?
                    //   look for one with no video frames comming 
                    foreach (S2255Device dev in m_S2255Devices)
                    {
                        if ( DateTime.Now.Subtract(frameTimeOut).CompareTo( dev.TimeAtLastFrame) > 0 )
                        {
                            // this is the lost device, close the physical device and release the controller instance, and release the device index
                            CloseDevice(dev);
                            DeviceToRemoveFromList = dev;
                            break;
                        }
                    }
                    if (DeviceToRemoveFromList != null) 
                        m_S2255Devices.Remove(DeviceToRemoveFromList);
                }
                else if (attachedCount > GetOpenDeviceCount() && GetOpenDeviceCount() < MAX_S2255DEVICE_COUNT)
                {

                    // we gained one, try to find its assigned index by trial and error

                    for (int d = 0; d < MAX_2255DEVICE_HANDLE_VALUE; d++)
                    {
                        int handleToTry = m_DeviceHandlePool.GetNextOpenDeviceValue();
                        if (S2255Device.TestOpenDevice(handleToTry))
                        {
                            // we found a new device at this index
                            
                            int index = m_UsedDeviceIndexes.GetNextOpenDeviceValue();

                            S2255Device dev = CreateAndStartNew2255(index, handleToTry); // reserves the device index, starts video flowing into the system
                            m_S2255Devices.Add(dev);
                            break;
                        }
                    }

                }
              
                if (m_StopThreads) break;
                Thread.Sleep(2000);
             
            }
        }


        /// <summary>
        /// Gets the number of physically attached devices, which may or may not have yet been opened/started by this application
        /// </summary>
        /// <returns></returns>
        int GetNumberAttached2255Devices()
        {
            string[] deviceNames = GetAttachedDevices();
            
            if (deviceNames == null) return (0);

            int devCount = 0;

            foreach (string name in deviceNames)
            {
                if (name.Contains("Sensoray 2255 Video"))
                {
                    devCount++;
                }
            }

            return (devCount);
        }

      

        string[] GetAttachedDevices()
        {
            List<string> attachedDeviceList = new List<string>();
            

            try
            {

                ManagementObjectSearcher deviceList = new ManagementObjectSearcher("Select Name, Status from Win32_PnPEntity");
      
                // Any results? There should be!
                if (deviceList != null)
                    // Enumerate the devices
                    foreach (ManagementObject device in deviceList.Get())
                    {

                        string name = device.GetPropertyValue("Name").ToString();
                      

                        attachedDeviceList.Add(name);
                    }
            }
            catch (Exception ex)
            {

                m_Log.Log(" GetAttachedDevices ex: " + ex.Message,ErrorLog.LOG_TYPE.INFORMATIONAL);
                return (null);
            }
            return (attachedDeviceList.ToArray<string>());

        }


        int GetOpenDeviceCount()
        {
            return (m_DeviceHandlePool.UsedValueCount);
        }

        /// <summary>
        /// maintain a list of integers that can be used as indexes or handles. Issue new values from the lowest available number first.
        /// </summary>
        class DeviceValuePoolManager
        {
            public DeviceValuePoolManager(int maxHandles)
            {
                MaxHandleNumber = maxHandles;
                handleHashTable = new ThreadSafeHashTable(MaxHandleNumber);
                nextHandle = 0;
            }
            int MaxHandleNumber;
            int nextHandle;
            ThreadSafeHashTable handleHashTable;

            public int UsedValueCount
            {
                get { return handleHashTable.Count; }
                set { }
            }

            public int GetNextOpenDeviceValue()
            {
                for (int i = nextHandle; i < MaxHandleNumber; i++)
                {
                    if (!handleHashTable.Contains(i))
                    {
                        nextHandle = i+1; // remeber were we left off
                        if (nextHandle >= MaxHandleNumber) nextHandle = 0;
                        return (i);
                    }
                }
                return (-1);
            }

            /// <summary>
            /// generates exception if index is already in the table
            /// </summary>
            /// <param name="index"></param>
            public void MarkValueAsUsed(int handle)
            {
                handleHashTable.Add(handle, handle);
            }

            /// <summary>
            /// generates exception if index is not in the hashtable
            /// </summary>
            /// <param name="index"></param>
            public void ReleaseValue(int handle)
            {
                handleHashTable.Remove(handle);
            }

        }

    }

   
    

}
