using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices; //For DLL support
using System.Threading;
using System.Drawing;
using System.IO;
using ApplicationDataClass;
using ErrorLoggingLib;
using LPROCR_Wrapper;
using Utilities;

namespace S2255Controller
{
    //public class PortPairController
    //{

    //    public PortPairController(PortPairController.S2255_MODE mode, APPLICATION_DATA appData)
    //    {
    //        m_Mode = mode;
    //        m_AppData = appData;

    //        singleton = new object();

    //        m_Log = (ErrorLog) m_AppData.Logger;
            
    //        m_CurrentFramePairLock = new object();

       

    //        lock (singleton)
    //        {
    //            unsafe
    //            {
    //                _Obj_of_Type_HANDLE_NEW_BUFFER_FROM_S2255 = handleNewBuffer;

    //                unsafeMethodCallbackPtr = Marshal.GetFunctionPointerForDelegate(_Obj_of_Type_HANDLE_NEW_BUFFER_FROM_S2255);

    //                m_CurrentFramePair = new FRAME_PAIR[2];
    //                m_CurrentFramePair[0] = new FRAME_PAIR();
    //                m_CurrentFramePair[1] = new FRAME_PAIR();
    //            }
    //        }
    //    }
       
    //    object singleton;

    //    ErrorLog m_Log;

    //    unsafe delegate void HANDLE_NEW_BUFFER_FROM_S2255(int jpegLength, int* binfo, byte* image, int cindex, int frameIndex);
    //    HANDLE_NEW_BUFFER_FROM_S2255 _Obj_of_Type_HANDLE_NEW_BUFFER_FROM_S2255;
    //    unsafe static IntPtr unsafeMethodCallbackPtr;

    //    APPLICATION_DATA m_AppData;

    //    S2255_MODE m_Mode;

    //    public enum S2255_MODE { PORT_PAIR, ALL_BMPS }

    //    int m_deviceIndex; 
    //    public int DeviceIndex { set { m_deviceIndex = value; } get { return m_deviceIndex; } }

    //    int m_portPairIndex; // either 0 or 1, two pairs per 2255 device
    //    public int PortPairIndex { set { m_portPairIndex = value; } get { return m_portPairIndex; } }

    //    int m_SystemChannel;// starts at 0, runs to max channels per computer (like 8 or 16), a port-pair can be a channel
    //    public int SystemChannel { set { m_SystemChannel = value; } get { return m_SystemChannel; } }


    //    S2255Controller.PAL_NTSC_MODE m_VideoStandard;
    //    public S2255Controller.PAL_NTSC_MODE VideoStandard { set { m_VideoStandard = value; } get { return m_VideoStandard; } }

    //    public delegate void HANDLE_NEW_FRAME_PAIRS(FRAME_PAIR fp, int deviceIndex);
    //    HANDLE_NEW_FRAME_PAIRS m_NewFrameHandler;
    //    public HANDLE_NEW_FRAME_PAIRS NewFrameHandler { set { m_NewFrameHandler = value; } get { return m_NewFrameHandler; } }

       
    //    public void StartPortPair()
    //    {
    //        if (m_NewFrameHandler == null) return;

    //        lock (singleton)
    //        {
    //            if (m_StopThreads) return;

    //            unsafe
    //            {
    //                // deviceChannel - each S2255 has four physical ports and index in the driver from 1 to 4
    //                //   this port pair controls two of those channesl => deviceChannel0 and deviceChannel1

    //                int deviceChannel0 = (m_portPairIndex * 2) + 1;// channel starts at 1
    //                int deviceChannel1 = (m_portPairIndex * 2) + 1 + 1;


    //                SetChannelMode(m_deviceIndex, deviceChannel0, (int)m_VideoStandard, (int)COMPRESSION_MODE.BITMAP);
    //                SetChannelMode(m_deviceIndex, deviceChannel1, (int)m_VideoStandard, (int)COMPRESSION_MODE.JPEG);

    //                StartAcquisitionThread(m_deviceIndex, deviceChannel0, unsafeMethodCallbackPtr);
    //                StartAcquisitionThread(m_deviceIndex, deviceChannel1, unsafeMethodCallbackPtr);

    //            }
    //        }

    //        m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.PHYSICAL_CHANNELS.PhysicalChannels_c0].Peak.RegisterForUse(true);
    //        m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.PHYSICAL_CHANNELS.PhysicalChannels_c1].Peak.RegisterForUse(true);
    //        m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.PHYSICAL_CHANNELS.PhysicalChannels_c2].Peak.RegisterForUse(true);
    //        m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.PHYSICAL_CHANNELS.PhysicalChannels_c3].Peak.RegisterForUse(true);
    //        m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.PHYSICAL_CHANNELS.PhysicalChannels_c4].Peak.RegisterForUse(true);
    //        m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.PHYSICAL_CHANNELS.PhysicalChannels_c5].Peak.RegisterForUse(true);
    //        m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.PHYSICAL_CHANNELS.PhysicalChannels_c6].Peak.RegisterForUse(true);
    //        m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.PHYSICAL_CHANNELS.PhysicalChannels_c7].Peak.RegisterForUse(true);

    //        m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.PHYSICAL_CHANNELS.PhysicalChannels_c0].PerSecond.RegisterForUse(true);
    //        m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.PHYSICAL_CHANNELS.PhysicalChannels_c1].PerSecond.RegisterForUse(true);
    //        m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.PHYSICAL_CHANNELS.PhysicalChannels_c2].PerSecond.RegisterForUse(true);
    //        m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.PHYSICAL_CHANNELS.PhysicalChannels_c3].PerSecond.RegisterForUse(true);
    //        m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.PHYSICAL_CHANNELS.PhysicalChannels_c4].PerSecond.RegisterForUse(true);
    //        m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.PHYSICAL_CHANNELS.PhysicalChannels_c5].PerSecond.RegisterForUse(true);
    //        m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.PHYSICAL_CHANNELS.PhysicalChannels_c6].PerSecond.RegisterForUse(true);
    //        m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.PHYSICAL_CHANNELS.PhysicalChannels_c7].PerSecond.RegisterForUse(true);

    //        m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.PHYSICAL_CHANNELS.PhysicalChannels_c0].RunningAverage.RegisterForUse(true);
    //        m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.PHYSICAL_CHANNELS.PhysicalChannels_c1].RunningAverage.RegisterForUse(true);
    //        m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.PHYSICAL_CHANNELS.PhysicalChannels_c2].RunningAverage.RegisterForUse(true);
    //        m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.PHYSICAL_CHANNELS.PhysicalChannels_c3].RunningAverage.RegisterForUse(true);
    //        m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.PHYSICAL_CHANNELS.PhysicalChannels_c4].RunningAverage.RegisterForUse(true);
    //        m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.PHYSICAL_CHANNELS.PhysicalChannels_c5].RunningAverage.RegisterForUse(true);
    //        m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.PHYSICAL_CHANNELS.PhysicalChannels_c6].RunningAverage.RegisterForUse(true);
    //        m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.PHYSICAL_CHANNELS.PhysicalChannels_c7].RunningAverage.RegisterForUse(true);

    //    }

    //    void HitStats(int chan, int value)
    //    {
    //        switch (chan)
    //        {
    //            case 0:
    //                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.PHYSICAL_CHANNELS.PhysicalChannels_c0].HitMe = value;
    //                break;
    //            case 1:
    //                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.PHYSICAL_CHANNELS.PhysicalChannels_c1].HitMe = value;
    //                break;
    //            case 2:
    //                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.PHYSICAL_CHANNELS.PhysicalChannels_c2].HitMe = value;
    //                break;
    //            case 3:
    //                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.PHYSICAL_CHANNELS.PhysicalChannels_c3].HitMe = value;
    //                break;
    //            case 4:
    //                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.PHYSICAL_CHANNELS.PhysicalChannels_c4].HitMe = value;
    //                break;
    //            case 5:
    //                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.PHYSICAL_CHANNELS.PhysicalChannels_c5].HitMe = value;
    //                break;
    //            case 6:
    //                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.PHYSICAL_CHANNELS.PhysicalChannels_c6].HitMe = value;
    //                break;
    //            case 7:
    //                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.PHYSICAL_CHANNELS.PhysicalChannels_c7].HitMe = value;
    //                break;
             

    //        }

    //    }

    //    object m_CurrentFramePairLock;
    //    FRAME_PAIR[] m_CurrentFramePair;
    //    int m_CurrentFramePairIndex = 0;

    //    public bool GetChannelConnectionStatus()
    //    {
    //        lock (singleton)
    //        {
    //            int deviceChannel0 = (m_portPairIndex * 2) + 1;// channel starts at 1
    //            int deviceChannel1 = (m_portPairIndex * 2) + 1 + 1;

    //            int s1 = GetVideoConnectionStatus(m_deviceIndex, deviceChannel0);
    //            int s2 = GetVideoConnectionStatus(m_deviceIndex, deviceChannel1);

    //            if (s1 == 0 || s2 == 0) return (false);
    //            else return (true);
    //        }
    //    }



    //    [DllImport("S2255Interface.dll", CallingConvention = CallingConvention.StdCall)]
    //    unsafe static extern void StopAcquisitionThread(int deviceIndex, int channel);

    //    Thread m_StopperThread;
    //    bool m_StopThreads = false;

    //    public void StopPortPair()
    //    {
    //        lock (singleton)
    //        {
    //            m_StopThreads = true;

    //            // because the StopAcquisitionThread call blocks until the 2255Interface thread stops, we can get thread deadlock since that thread may be blocking on giving this entity a new frame callback
    //            // so spin off a stopper thread
    //            m_StopperThread = new Thread(ReallyStopPortPair);
    //            m_StopperThread.Start();
    //        }

    //    }

    //    void ReallyStopPortPair()
    //    {
    //        int channel0 = (m_portPairIndex * 2) + 1;
    //        int channel1 = (m_portPairIndex * 2) + 1 + 1;

    //        int[] dummy = new int[2];

    //        lock (singleton)
    //        {
    //            unsafe
    //            {
    //                fixed (int* dummyPtr = dummy)
    //                {
    //                    StopAcquisitionThread(m_deviceIndex, channel0);
    //                    StopAcquisitionThread(m_deviceIndex, channel1);
    //                }
    //            }
    //        }

    //    }


    //    TimeSpan OneSecond = new TimeSpan(0, 0, 1);

    //    unsafe void handleNewBuffer(int jpegLength, int* binfo, byte* image, int cindex, int frameIndex)
    //    {          

    //        if (jpegLength == 0)
    //            handleBitmap(binfo, image, cindex, frameIndex);
    //        else
    //        {
    //            handleJpeg(jpegLength, image, cindex);
    //        }
    //    }

       

    //    unsafe void handleJpeg(int len, byte* imageBytes, int cindex)
    //    {
    //        if (imageBytes == null) return;

    //        try
    //        {
    //            if (len == 0) return;

    //            byte[] image = new byte[len];
         

    //            Marshal.Copy(new IntPtr(imageBytes), image, 0, len);

               

    //            AddFrameToPair(image, COMPRESSION_MODE.JPEG, cindex);

    //        }
    //        catch (Exception ex)
    //        {
    //            m_Log.Log("handleJpeg ex: " + ex.Message, ErrorLog.LOG_TYPE.FATAL);
    //        }
    //    }

    //    unsafe void handleBitmap(int* binfo, byte* image, int cindex, int frameIndex)
    //    {
    //        // this is called from the unmanaged S2255Interface thread
    //        if (m_SystemChannel == 2 || m_SystemChannel == 3)
    //        {
    //            int jj = m_SystemChannel;//breakpoint
    //        }
    //        HitStats(m_SystemChannel, 1);
            
    //        try
    //        {

    //            if (m_StopThreads) return;

    //            // compose a managed bitmap from the bitmap parts being delivered

    //            System.Drawing.Imaging.BitmapData srcBmpHeader = new System.Drawing.Imaging.BitmapData();
    //            Marshal.PtrToStructure((IntPtr)binfo, srcBmpHeader);

    //            //pDoc->m_buf[idx].lpbmi[frm_idx]->bmiHeader.biSizeImage

    //            // for some reason stride and width a swapped in position between the 2255 definition and the windows bitmpa definition
    //            int width = srcBmpHeader.Height;
    //            int height = srcBmpHeader.Stride;

    //            Rectangle rect = new Rectangle(0, 0, width, height);

    //       //     Bitmap nBmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
    //            Bitmap nBmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
           


    //            System.Drawing.Imaging.BitmapData bmpData = nBmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, nBmp.PixelFormat);

    //            // Get the address of the first line.
    //          //  int byteCount = width * height * 3;
    //            int byteCount = width * height ;

    //            //byte[] byteArray = new byte[byteCount];
    //            //Marshal.Copy(new IntPtr(image), byteArray, 0, byteCount);
    //            //Marshal.Copy(byteArray, 0, bmpData.Scan0, byteCount);

    //            LPROCR_Lib.MemCopyByte((int*)image, bmpData.Scan0, byteCount);
    //          //  LPROCR_Lib.MemCopyInt((int*)image, bmpData.Scan0, byteCount );
                
    //            nBmp.UnlockBits(bmpData);

           
    //          //  nBmp.RotateFlip(RotateFlipType.RotateNoneFlipY);

    //            AddFrameToPair(nBmp, COMPRESSION_MODE.BITMAP, cindex);



    //        }
    //        catch (Exception ex)
    //        {
    //            m_Log.Log("handleBitmap ex: " + ex.Message, ErrorLog.LOG_TYPE.FATAL);
    //        }
    //    }


    //    void AddFrameToPair(object frame, COMPRESSION_MODE frmType, int cindex )
    //    {
    //        lock (m_CurrentFramePairLock)
    //        {

    //            if (frmType == COMPRESSION_MODE.BITMAP)
    //                m_CurrentFramePair[m_CurrentFramePairIndex].Add((Bitmap)frame, m_portPairIndex, m_deviceIndex, m_AppData.GlobalFrameSerialNumber, m_SystemChannel);
    //            else
    //            {
    //                m_CurrentFramePair[m_CurrentFramePairIndex].Add((byte[])frame, m_portPairIndex, m_deviceIndex, m_AppData.GlobalFrameSerialNumber, m_SystemChannel);
    //            }


    //            if (m_CurrentFramePair[m_CurrentFramePairIndex].state == FRAME_PAIR.STATE.COMPLETE)
    //            {
    //                m_NewFrameHandler(m_CurrentFramePair[m_CurrentFramePairIndex], m_deviceIndex);
    //                m_CurrentFramePairIndex++;
    //                if (m_CurrentFramePairIndex == 2) m_CurrentFramePairIndex = 0;
    //                m_CurrentFramePair[m_CurrentFramePairIndex] = new FRAME_PAIR();// let the consumer keep the old one until its done with it
    //                m_AppData.GlobalFrameSerialNumber++;
    //            }
    //        }
    //    }


    //}
}
