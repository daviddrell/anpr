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
using DirectShowLib;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.IO;


namespace FrameGeneratorLib
{
    //class DSFileDecoder
    //{
    //}
    //   public class MyResource: IDisposable
    public class DSFileDecoder : ISampleGrabberCB, IDisposable, IFilePlayerInterface
    {

        public DSFileDecoder(APPLICATION_DATA appData, Panel parentPanel, Form parentForm, int chan)
        {

            m_AppData = appData;
            m_AppData.AddOnClosing(OnStop, APPLICATION_DATA.CLOSE_ORDER.FIRST);
            m_Log = (ErrorLog)m_AppData.Logger;

            m_Channel = chan;
            m_ParentForm = parentForm;
            m_ParentPanel = parentPanel;

            lockSampleGrabberState = new object();
        }

        int m_Channel;
        ErrorLog m_Log;
        APPLICATION_DATA m_AppData;

      //  public delegate void OnEndOfFile();
        public event FrameGeneratorLib.OnEndOfFile OnEndOfFileEvent;

       // public delegate void OnNewFrameEvent(FRAME frame);
        public event FrameGeneratorLib.OnNewFrameEvent OnNewFrame;

        Panel m_ParentPanel;
        Form m_ParentForm;



        // DirectShow stuff

        private IFilterGraph2 graphBuilder = null;
        private IMediaControl mediaControl = null;
        private IMediaPosition mediaPosition = null;
        private IMediaEvent m_MediaEvent = null;
        private IBaseFilter vmr9 = null;
        private IVMRWindowlessControl9 windowlessCtrl = null;
        private bool handlersAdded = false;

        int m_videoWidth = 0;
        int m_videoHeight = 0;
        int m_stride = 0;
        private double currentPlaybackRate = 1.0;


        // Implement IDisposable.
        // Do not make this method virtual.
        // A derived class should not be able to override this method.
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue 
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }


        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                //if (components != null)
                //{
                //   components.Dispose();
                //}
            }

            // Clean-up DirectShow interfaces
            CloseInterfaces();
        }

        private void OnStop()
        {
            StopGraph();
         
            this.Dispose();
        }

        private void CloseInterfaces()
        {

            while (!m_WaitUntilDoneIsDone)
                Thread.Sleep(1);

            if (!m_WaitUntilDoneIsDone)
            {
                MessageBox.Show("wait not done");

            }

            try
            {
                if (mediaControl != null)
                {
                    mediaControl.Stop();
                    Marshal.ReleaseComObject(mediaControl);
                    mediaControl = null;
                }

                if (mediaPosition != null)
                {
                    Marshal.ReleaseComObject(mediaPosition);
                    mediaPosition = null;
                }
                if (windowlessCtrl != null)
                {
                    Marshal.ReleaseComObject(windowlessCtrl);
                    windowlessCtrl = null;
                }

                if (handlersAdded)
                    RemoveHandlers();

                if (m_MediaEvent != null)
                {
                    Marshal.ReleaseComObject(m_MediaEvent);
                    m_MediaEvent = null;
                }

                if (vmr9 != null)
                {
                    Marshal.ReleaseComObject(vmr9);
                    vmr9 = null;
                    windowlessCtrl = null;
                }

                if (graphBuilder != null)
                {
                    Marshal.ReleaseComObject(graphBuilder);
                    graphBuilder = null;
                    mediaControl = null;
                }
            }
            catch { }

        }

        private void BuildGraph(string fileName)
        {
            int hr = 0;

            try
            {
                graphBuilder = (IFilterGraph2)new FilterGraph();
                mediaControl = (IMediaControl)graphBuilder;
                ISampleGrabber sampGrabber = null;
                IBaseFilter baseGrabFlt = null;

                // Get the SampleGrabber interface
                sampGrabber = (ISampleGrabber)new SampleGrabber();

                vmr9 = (IBaseFilter)new VideoMixingRenderer9();
           

                ConfigureVMR9InWindowlessMode();

                hr = graphBuilder.AddFilter(vmr9, "Video Mixing Renderer 9");
                DsError.ThrowExceptionForHR(hr);

                baseGrabFlt = (IBaseFilter)sampGrabber;
                ConfigureSampleGrabber(sampGrabber);

                // Add the frame grabber to the graph
                hr = graphBuilder.AddFilter(baseGrabFlt, "Ds.NET Grabber");
                DsError.ThrowExceptionForHR(hr);


                mediaPosition = (IMediaPosition)graphBuilder;
                m_MediaEvent = graphBuilder as IMediaEvent;

                hr = graphBuilder.RenderFile(fileName, null);
                DsError.ThrowExceptionForHR(hr);


                SaveSizeInfo(sampGrabber);

            }
            catch (Exception e)
            {
                CloseInterfaces();
                MessageBox.Show("An error occured during the graph building : \r\n\r\n" + e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary> Set the options on the sample grabber </summary>
        private void ConfigureSampleGrabber(ISampleGrabber sampGrabber)
        {
            int hr;
            AMMediaType media = new AMMediaType();



            // Set the media type to Video/RBG24
            media.majorType = MediaType.Video;
            media.subType = MediaSubType.RGB24;
            media.formatType = FormatType.VideoInfo;
            hr = sampGrabber.SetMediaType(media);
            DsError.ThrowExceptionForHR(hr);

            DsUtils.FreeAMMediaType(media);
            media = null;

            // Configure the samplegrabber callback
            hr = sampGrabber.SetCallback(this, 1);
            DsError.ThrowExceptionForHR(hr);
        }


        private void ConfigureVMR9InWindowlessMode()
        {
            int hr = 0;

            IVMRFilterConfig9 filterConfig = (IVMRFilterConfig9)vmr9;

            // Not really needed for VMR9 but don't forget calling it with VMR7
            hr = filterConfig.SetNumberOfStreams(1);
            DsError.ThrowExceptionForHR(hr);

            // Change VMR9 mode to Windowless
            hr = filterConfig.SetRenderingMode(VMR9Mode.Windowless);
            DsError.ThrowExceptionForHR(hr);

            windowlessCtrl = (IVMRWindowlessControl9)vmr9;

            // Set "Parent" window
            hr = windowlessCtrl.SetVideoClippingWindow(this.m_ParentPanel.Handle);
            DsError.ThrowExceptionForHR(hr);

            // Set Aspect-Ratio
            hr = windowlessCtrl.SetAspectRatioMode(VMR9AspectRatioMode.LetterBox);
            DsError.ThrowExceptionForHR(hr);

            // Add delegates for Windowless operations
            AddHandlers();



            // Call the resize handler to configure the output size
            MainForm_ResizeMove(null, null);
        }


        private void AddHandlers()
        {
            // Add handlers for VMR purpose
            //  SetStyle(ControlStyles.UserPaint, true);

            m_ParentForm.Paint += new PaintEventHandler(MainForm_Paint); // for WM_PAINT
            m_ParentForm.Resize += new EventHandler(MainForm_ResizeMove); // for WM_SIZE
            m_ParentForm.Move += new EventHandler(MainForm_ResizeMove); // for WM_MOVE
            Microsoft.Win32.SystemEvents.DisplaySettingsChanged += new EventHandler(SystemEvents_DisplaySettingsChanged); // for WM_DISPLAYCHANGE
            handlersAdded = true;
        }

        private void RemoveHandlers()
        {
            // remove handlers when they are no more needed
            handlersAdded = false;
            m_ParentForm.Paint -= new PaintEventHandler(MainForm_Paint);
            m_ParentForm.Resize -= new EventHandler(MainForm_ResizeMove);
            m_ParentForm.Move -= new EventHandler(MainForm_ResizeMove);
            Microsoft.Win32.SystemEvents.DisplaySettingsChanged -= new EventHandler(SystemEvents_DisplaySettingsChanged);
        }

        public void RunGraph()
        {
            if (mediaControl != null)
            {
                m_Playing = true;
                SetRate(.1);

                int hr = mediaControl.Run();
                DsError.ThrowExceptionForHR(hr);


                Thread m_WaitForCompletionThread = new Thread(WaitUntilDone);
                m_WaitForCompletionThread.Start();
            }
        }

        bool m_Playing = false;
        bool m_Stop = false;
        public void StopGraph()
        {
            if (! m_Playing) return;

            if (mediaControl != null)
            {
                m_Stop = true;

                bool sampleGrabberState;
                lock (lockSampleGrabberState)
                {
                    sampleGrabberState = m_SampleGrabberCallBackIsDone;
                }
                int count = 20000;
                while (count-- > 0 &&  !sampleGrabberState)// wait for the capture buff callbacks to see the m_Stop flag
                {
                    System.Windows.Forms.Application.DoEvents();
                    
                    if (m_Stop) break;

                    Thread.Sleep(1);
                    lock (lockSampleGrabberState)
                    {
                        sampleGrabberState = m_SampleGrabberCallBackIsDone;
                    }
                }
                

                m_Playing = false;

                int hr = mediaControl.Stop();
                DsError.ThrowExceptionForHR(hr);


                FilterState fs;// wait for stop to complete
                mediaControl.GetState(30000, out fs);

                if (handlersAdded) RemoveHandlers();
            }
        }

        /// <summary> Read and store the properties </summary>
        private void SaveSizeInfo(ISampleGrabber sampGrabber)
        {
            int hr;

            // Get the media type from the SampleGrabber
            AMMediaType media = new AMMediaType();
            hr = sampGrabber.GetConnectedMediaType(media);
            DsError.ThrowExceptionForHR(hr);

            if ((media.formatType != FormatType.VideoInfo) || (media.formatPtr == IntPtr.Zero))
            {
                throw new NotSupportedException("Unknown Grabber Media Format");
            }

            // Grab the size info
            VideoInfoHeader videoInfoHeader = (VideoInfoHeader)Marshal.PtrToStructure(media.formatPtr, typeof(VideoInfoHeader));
            m_videoWidth = videoInfoHeader.BmiHeader.Width;
            m_videoHeight = videoInfoHeader.BmiHeader.Height;
            m_stride = m_videoWidth * (videoInfoHeader.BmiHeader.BitCount / 8);

            DsUtils.FreeAMMediaType(media);
            media = null;
        }

        public void getVideoSize(ref int width, ref int height)
        {
            width = m_videoWidth;
            height = m_videoHeight;
        }


      

        public void Start(string[] fileNames)
        {
            // not supported
        }

        DateTime m_FileTimeOfCurrentFile;

        public void Start(string fileName)
        {

            m_FileTimeOfCurrentFile = new FileInfo( fileName).LastWriteTimeUtc;
          
            CloseInterfaces();
            BuildGraph(fileName);
            RunGraph();
          
        }

       

      
        private void MainForm_Paint(object sender, PaintEventArgs e)
        {

            try
            {
                if (windowlessCtrl != null)
                {

                    FilterState fs;// wait for pause to complete
                    mediaControl.GetState(2000, out fs);

                    //   if (fs == FilterState.Running)
                    {
                        IntPtr hdc = e.Graphics.GetHdc();
                        int hr = windowlessCtrl.RepaintVideo(this.m_ParentPanel.Handle, hdc);
                        e.Graphics.ReleaseHdc(hdc);
                    }
                }
            }
            catch (Exception ex)
            {
                m_Log.Log("sourceVideoFile MainForm_Paint: ex: " + ex.Message,ErrorLog.LOG_TYPE.FATAL);
            }
        }


        private void MainForm_ResizeMove(object sender, EventArgs e)
        {
            try
            {
                if (windowlessCtrl != null)
                {
                    FilterState fs;// wait for pause to complete
                    mediaControl.GetState(2000, out fs);
                    //      if (fs == FilterState.Running)
                    {
                        int hr = windowlessCtrl.SetVideoPosition(null, DsRect.FromRectangle(this.m_ParentPanel.ClientRectangle));
                    }
                }
            }
            catch (Exception ex)
            {
                m_Log.Log("sourceVideoFile MainForm_ResizeMove: ex: " + ex.Message,ErrorLog.LOG_TYPE.FATAL);
            }
        }


        private void SystemEvents_DisplaySettingsChanged(object sender, EventArgs e)
        {
            if (windowlessCtrl != null)
            {
                int hr = windowlessCtrl.DisplayModeChanged();
            }
        }


        public void Pause()
        {
            if (mediaControl != null)
            {
                int hr = mediaControl.Pause();
                DsError.ThrowExceptionForHR(hr);

                FilterState fs;// wait for pause to complete
                mediaControl.GetState(2000, out fs);
            }
          
        }


        private int ModifyRate(double dRateAdjust)
        {
            int hr = 0;
            double dRate;

            // If the IMediaPosition interface exists, use it to set rate
            if ((this.mediaPosition != null) && (dRateAdjust != 0.0))
            {
                hr = this.mediaPosition.get_Rate(out dRate);
                if (hr == 0)
                {
                    // Add current rate to adjustment value
                    double dNewRate = dRate + dRateAdjust;
                    hr = this.mediaPosition.put_Rate(dNewRate);

                    // Save global rate
                    if (hr == 0)
                    {
                        this.currentPlaybackRate = dNewRate;
                       // UpdateMainTitle();
                    }
                }
            }

            return hr;
        }

        private int SetRate(double rate)
        {
            int hr = 0;

            // If the IMediaPosition interface exists, use it to set rate
            if (this.mediaPosition != null)
            {
                hr = this.mediaPosition.put_Rate(rate);
                if (hr >= 0)
                {
                    this.currentPlaybackRate = rate;
                 //   UpdateMainTitle();
                }
            }

            return hr;
        }



        /// <summary> sample callback, NOT USED. </summary>
        int ISampleGrabberCB.SampleCB(double SampleTime, IMediaSample pSample)
        {
            Marshal.ReleaseComObject(pSample);
            return 0;
        }

        object lockSampleGrabberState;
        bool m_SampleGrabberCallBackIsDone = true;

        // get the individual frames and send them to the DVR/LPR processing chains

        /// <summary> buffer callback, COULD BE FROM FOREIGN THREAD. </summary>
        int ISampleGrabberCB.BufferCB(double SampleTime, IntPtr pBuffer, int BufferLen)
        {
            lock (lockSampleGrabberState)
            {
                if (m_Stop) return 0;

                m_SampleGrabberCallBackIsDone = false;

                IntPtr ipSource = pBuffer;

                int[,] ipDest = new int[m_videoWidth, m_videoHeight];


                bool invert = true;

                if (m_videoWidth != ipDest.GetLength(0) || m_videoHeight != ipDest.GetLength(1))
                {
                    return 0;
                }

                LPROCR_Wrapper.LPROCR_Lib.extractFromBmpDataToLumArray(ipSource, ipDest, m_stride, m_videoWidth, m_videoHeight, invert);


                // compose a new bitmap

                Bitmap bmp = new Bitmap(m_videoWidth, m_videoHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                // Lock the bitmap's bits.  
                Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                System.Drawing.Imaging.BitmapData bmpData =
                    bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                    bmp.PixelFormat);

                // Get the address of the first line.
                IntPtr ptr = bmpData.Scan0;

                // copy out to the new bitamp
                bool dontcare = false;

                unsafe
                {
                    LPROCR_Lib.MemCopyByte((int*)ipSource, ptr, BufferLen, ref dontcare);
                }


                bmp.UnlockBits(bmpData);

                bmp.RotateFlip(RotateFlipType.Rotate180FlipX);// what it takes to make it look right, if I had time I would do this in one step in LPROCR_Lib.MemCopyByte

                FRAME frame = new FRAME(m_AppData);
                frame.Luminance = ipDest;
                frame.TimeStamp = m_FileTimeOfCurrentFile.AddSeconds(SampleTime);
                frame.Bmp = bmp;
                frame.SourceChannel = m_Channel;
                frame.SourceName = m_AppData.UserSpecifiedCameraName == null ? "storedjpeg" : m_AppData.UserSpecifiedCameraName;
                frame.SetFileName();
                OnNewFrame(frame);

                m_SampleGrabberCallBackIsDone = true;

                return 0;
            }
        }


        bool m_WaitUntilDoneIsDone=true;
        
        public void WaitUntilDone()
        {
            try
            {
                int hr;
                EventCode evCode;
                const int E_Abort = unchecked((int)0x80004004);
               
                do
                {
                    m_WaitUntilDoneIsDone = false;
                    System.Windows.Forms.Application.DoEvents();
                    hr = this.m_MediaEvent.WaitForCompletion(100, out evCode);
                } while (hr == E_Abort && !m_Stop);


                if (OnEndOfFileEvent != null) OnEndOfFileEvent();

                m_WaitUntilDoneIsDone = true;
            }
            catch { m_WaitUntilDoneIsDone = true; }
        }

    }



}
