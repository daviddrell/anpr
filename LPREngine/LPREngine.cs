using System.Text;
using System.Threading;
using System.IO;
using System.Collections;
using LPROCR_Wrapper;
using ApplicationDataClass;
using UserSettingsLib;
using ErrorLoggingLib;
using FrameGeneratorLib;
using Utilities;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;


namespace LPREngineLib
{

    public class LPREngine
    {
        public LPREngine(APPLICATION_DATA appData)
        {
            try
            {
                m_AppData = appData;
                m_AppData.AddOnClosing(Stop, APPLICATION_DATA.CLOSE_ORDER.MIDDLE);
                m_Log = (ErrorLog)m_AppData.Logger;
                m_FrameGen = (FrameGenerator)m_AppData.FrameGenerator;


                m_LPRProcessQ = new ThreadSafeQueue<FRAME>(m_LPRProcessQueSize, "QueueOverruns_LPR_LPRProcessQ", m_AppData); // this queue hold frames that come from the framegenerator and need to be processed by LPR
                m_AppData.LPRGettingBehind = false;

                m_LPRFinalPlateGroupOutputQ = new ThreadSafeQueue<FRAME>(60, "QueueOverruns_LPR_LPRFinalPlateGroupOutputQ", m_AppData); // filtered plate readings, grouped into similar readings, redundant readings removed

                m_LPRPerFrameReadingQ = new ThreadSafeQueue<FRAME>(60, "QueueOverruns_LPR_LPRPerFrameReadingQ", m_AppData); // instantaneous output from LPR for each fram processed 

                m_StoredFrameData = new ThreadSafeHashableQueue(30 * 60);// 60 seconds of frames at 30fps


               

                m_LPRFuntions = new LPROCR_Lib();
                unsafe
                {       // the plate group processor accumulates per-frame plate readings and consolidates them into a single plate reading where appropriate
                    m_LPRFuntions.RegisterPlateGroupCB(OnNewPlateGroupReady);
                }

                int maxW = 0, minW = 0, maxH = 0, minH = 0;

                m_LPRFuntions.GetMinMaxPlateSize(ref minW, ref maxW,ref minH, ref maxH);

                m_AppData.MAX_PLATE_HEIGHT = maxH;
                m_AppData.MIN_PLATE_HEIGHT = minH;
                m_AppData.MAX_PLATE_WIDTH = maxW;
                m_AppData.MIN_PLATE_WIDTH = minW;

                m_processOptions = new LPROCR_Lib.LPR_PROCESS_OPTIONS();

                m_processOptions.roll = 1;
                m_processOptions.rotation = 1;

                // register with the frame grabber to get new bitmaps from the channel sources as they come in

                m_ConsumerID = m_FrameGen.GetNewConsumerID();

//                m_NumSourceChannels = m_FrameGen.GetNumberOfConfiguredChannels();
                m_NumSourceChannels = (m_AppData.RunninAsService) ? m_AppData.MAX_PHYSICAL_CHANNELS : m_AppData.MAX_VIRTUAL_CHANNELS;


                m_LPREngineProcessThread = new Thread(LPREngineProcessLoop);


                PushLPRResultsThread = new Thread(PushLPRResultsLoop);
            }
            catch (Exception ex) { m_Log.Trace(ex, ErrorLog.LOG_TYPE.FATAL); }

        }

        int m_LPRProcessQueSize = 240;

        APPLICATION_DATA m_AppData;
        ErrorLog m_Log;
        FrameGenerator m_FrameGen;
        int m_ConsumerID;
        int m_NumSourceChannels;
        Thread m_LPREngineProcessThread;
        public LPROCR_Lib m_LPRFuntions;
        LPROCR_Lib.LPR_PROCESS_OPTIONS m_processOptions;
        ThreadSafeQueue<FRAME> m_LPRFinalPlateGroupOutputQ;
        ThreadSafeQueue<FRAME> m_LPRPerFrameReadingQ;
        ThreadSafeQueue<FRAME> m_LPRProcessQ;
        ThreadSafeHashableQueue m_StoredFrameData;
        Thread PushLPRResultsThread;
        public delegate void NewPlateEvent(FRAME frame);
        public event NewPlateEvent OnNewFilteredPlateGroupEvent;
        public event NewPlateEvent OnNewUnfilteredPlateEvent;

        public void StartRegistration()
        {
            try
            {
                // initialize the instrumentation
                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.LPR.LPR_ProcessQCnt].Snapshot.RegisterForUse(true);
                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.LPR.LPR_ProcessQCnt].Peak.RegisterForUse(true);
                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.LPR.LPR_DroppedFrames].Accumulator.RegisterForUse(true);
                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.LPR.LPR_FramesProcessed].Accumulator.RegisterForUse(true);
                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.LPR.LPR_PlatesFound].Accumulator.RegisterForUse(true);
                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.LPR.LPR_PlateGroupCount].Accumulator.RegisterForUse(true);

                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.LPR.LPR_LastReading].StatString.RegisterForUse(true);



                for (int c = 0; c < m_NumSourceChannels; c++)
                {
                    m_FrameGen.RegisterToConsumeMotionDetectedFrames(m_ConsumerID, c, OnRxNewMotionWasDetectedFrame);
                }
            }
            catch (Exception ex) { m_Log.Trace(ex, ErrorLog.LOG_TYPE.FATAL); }

        }

        public void StartThreads()
        {
            m_LPREngineProcessThread.Start();
            PushLPRResultsThread.Start();
        }

        bool m_Stop = false;
        public void Stop()
        {
            m_Stop = true;
        }

        void PushLPRResultsLoop()
        {
            while (!m_Stop)
            {
                Thread.Sleep(1);

                try
                {
                    FRAME frame = m_LPRFinalPlateGroupOutputQ.Dequeue();
                    if (frame != null)
                    {
                        // this delegate will send a frame to all cosumers that added their events handlers to this delegate
                        if (OnNewFilteredPlateGroupEvent != null) OnNewFilteredPlateGroupEvent(frame);
                    }

                    frame = m_LPRPerFrameReadingQ.Dequeue();
                    if (frame != null)
                    {
                        // this delegate will send a frame to all cosumers that added their events handlers to this delegate
                        if (OnNewUnfilteredPlateEvent != null) OnNewUnfilteredPlateEvent(frame);
                    }

                   
                }
                catch (Exception ex) { m_Log.Trace(ex, ErrorLog.LOG_TYPE.FATAL); }


               
            }
        }

        unsafe void OnNewPlateGroupReady(int chan, char* concatonatedstring, int serialNumber, int maxConCatStringLen)
        {
            List<string> plateStrings = new List<string>();

            string conCatString = Marshal.PtrToStringAnsi((IntPtr)concatonatedstring);

            FRAME frame = (FRAME)m_StoredFrameData[serialNumber];
            if (frame == null)
            {
                Console.Write("!!!!!!!!! could not find frame record in m_StoredFrameData, chan: \r\n", chan.ToString());
            }


            // diagnostic
            string[] sp = conCatString.Split(',');
            Console.Write("received group:{0} \r\n", chan.ToString());
            Console.Write("serial nunmber = "+ serialNumber +"\r\n");
            Console.Write("time = " + frame.TimeStamp.ToString(m_AppData.TimeFormatStringForDisplay) + "\r\n");
            foreach (string s in sp)
            {
                Console.Write(s + " \r\n");
            }


          
            m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.LPR.LPR_PlateGroupCount].HitMe++;
          

            // end debug

            // put this result into the result Q
            if (frame != null)
            {

                // debug the garbage plate number problem
                if (ContainsGarbage(sp))
                {
                    int jj = frame.SerialNumber; //breakpoint
                }
                else
                {
                    frame.PlateNumberNativeLanguage = sp;
                    frame.PlateNumberLatin = sp;
                    m_LPRFinalPlateGroupOutputQ.Enqueue(frame);
                }

              
                
            }


        }

        bool ContainsGarbage(string[] plateNumbers)
        {
            // 48 to 91 decimanal, valid ascii characters
            if (plateNumbers == null) return (false);
            if (plateNumbers.Length < 1) return (false);

            foreach (string s in plateNumbers)
            {
                char[] chars = s.ToCharArray();
                for (int c = 0; c < chars.Length; c++)
                {
                    if (chars[c] < 48 || chars[c] > 91)
                        return (true);
                }
            }
            return (false);
        }

        /// <summary>
        /// Use this routine to push hand-edited plate results into the storage system as if the plates came from the automated LPR reading chain.
        /// </summary>
        public void PushHandEditedPlate(FRAME frame)
        {

            // put this result into the result Q
            if (frame != null)
            {
                m_LPRFinalPlateGroupOutputQ.Enqueue(frame);
            }
        }


        // registered to receive frames where motion was detected from the previous frame

        void OnRxNewMotionWasDetectedFrame(FRAME frame)
        {

            bool sucess = m_LPRProcessQ.Enqueue(frame); // enqueue for the LPR thread to process
            if (!sucess)
                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.LPR.LPR_DroppedFrames].HitMe++;

            if (m_LPRProcessQ.Count > m_LPRProcessQueSize / 2) m_AppData.LPRGettingBehind = true;
            else m_AppData.LPRGettingBehind = false;

            // store the frame meta data for later retrival if the LPR finds plates
            //   no need to use memory storing bitmaps and jpegs as the jpeg has been stored on disk by the DVR and the
            //   frame meta data includes a reference to this file.

            FRAME newFrame = null;
            newFrame = frame.Clone(false, false, false);
            newFrame.PlateNativeLanguage = "LATIN";
            // keep knowledge of this frame around for a while, so that if LPR plate number group processig finds some numbers,
            //  we can look up which frame was associated with those numbers by serial number.
            //   the jpeg already went to disk by the time the LPR found numbers, so only the file url gets recorded
            //   into the eventlog.txt file 
            m_StoredFrameData.Add(newFrame.SerialNumber, newFrame);

            // for plate groupings, keep the serialnumber/frame data around for a while, only use
            //if (m_AppData.DVRMode == APPLICATION_DATA.DVR_MODE.STORE_ON_MOTION) 
            //{
            //    newFrame = frame.Clone(false, false, false);
            //    newFrame.PlateNativeLanguage = "LATIN";
            //    // keep knowledge of this frame around for a while, so that if LPR plate number group processig finds some numbers,
            //    //  we can look up which frame was associated with those numbers by serial number.
            //    //   the jpeg already went to disk by the time the LPR found numbers, so only the file url gets recorded
            //    //   into the eventlog.txt file 
            //    m_StoredFrameData.Add(newFrame.SerialNumber, newFrame); 
            //}
            //else
            //{
            //    //else m_AppData.DVRMode == APPLICATION_DATA.DVR_MODE.STORE_ON_PLATE_FOUND
            //    // keep a copy of the jpeg in this frame since this frame is what the DVR will write to disk
            //    newFrame = frame.Clone(false,true,false);
            //}

     
           
        }

        void LPREngineProcessLoop()
        {
            FRAME frm = null;
            int channel = 0;
            FrameGenerator fg = (FrameGenerator)m_AppData.FrameGenerator;
            int maxChannels = fg.GetNumberOfPhysicalChannels();
            DateTime[] timeLastTouched = new DateTime[maxChannels];
            TimeSpan timeout = new TimeSpan(0, 0, 0, 5, 0);

            int loopDelayInMilliSeconds = 1;

            while (!m_Stop)
            {
                frm = null;
                try
                {

                    frm = m_LPRProcessQ.Dequeue();

                    m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.LPR.LPR_ProcessQCnt].HitMe = m_LPRProcessQ.Count;

                    if (frm != null)
                    {
                        // hits here on every frame where motion was detected compared to the previous frame
                        timeLastTouched[frm.SourceChannel] = DateTime.Now;
                        LPRProcessImage(frm);
                    }

                    channel++;
                    if (channel == maxChannels) channel = 0;

                    // this forces the plategroup processor to release a group if the car is parked and not moving, only works
                    // if no motion was detected for timeout seconds


                    if (DateTime.Now.Subtract(timeLastTouched[channel]).CompareTo(timeout) > 0)
                    {
                        m_LPRFuntions.PlateGroups_ProcessNewImage(channel, "", 0, 0);  // if we have not had a frame to process in a while, force the plate number groupings to expire and report results from last reading
                    }

                }
                catch (Exception ex) { m_Log.Trace(ex, ErrorLog.LOG_TYPE.FATAL); }

                Thread.Sleep(loopDelayInMilliSeconds);
            }
        }

        void LPRProcessImage(FRAME frame)
        {
            try
            {
                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.LPR.LPR_FramesProcessed].HitMe++;

                int error = 0;
                int plateCount = 0;

                int diagEnabled = 0;
                if (m_AppData.LPRDiagEnabled) diagEnabled = 1;

                try
                {
                    plateCount = m_LPRFuntions.ReadThisImage(frame.Luminance, diagEnabled, ref  m_processOptions, ref error);
                    if (error != 0)
                    {
                        m_Log.Log("LPR Error = " + error.ToString(), ErrorLog.LOG_TYPE.FATAL);
                    }
                }
                catch (Exception ex)
                {
                    m_Log.Log("ReadThisImage ex: " + ex.Message, ErrorLog.LOG_TYPE.FATAL);
                }


                // did we find any plates int the image ?

                if (plateCount < 1)
                {
                    m_LPRFuntions.PlateGroups_ProcessNewImage(frame.SourceChannel, " ", 0, frame.SerialNumber);// notify next frame had no plates
                    frame = null;
                    return;
                }

                m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.LPR.LPR_PlatesFound].HitMe = plateCount;

                // extract the plate images and strings, and send to the LPRStorage class

                string[] detectedStrings = new string[plateCount];

                for (int p = 0; p < plateCount; p++)
                {
                    float score = 0;

                    // get the string from the plate

                    detectedStrings[p] = m_LPRFuntions.GetPlateString(p, ref score);

                    m_AppData.HealthStatistics[(int)APPLICATION_DATA.HEALTH_STATISTICS.LPR.LPR_LastReading].StatString.SetValue = (string) detectedStrings[p];

                    if (m_AppData.DVRMode == APPLICATION_DATA.DVR_MODE.STORE_ON_MOTION)
                    {
                        // in store-on-motion mode, consolidate multiple frames of detected plates into a single grouping of plate readings...
                        m_LPRFuntions.PlateGroups_ProcessNewImage(frame.SourceChannel, detectedStrings[p], detectedStrings[p].Length, frame.SerialNumber);
                    } // else, we are in the mode of store on LPR detecting a plate number. In this case,  we are going to store all results below...
                  

                }// end for each plate found


                // send unfiltered plate result to consumers
                FRAME justToXferPlateNumber = frame.Clone(false, false, false);
                justToXferPlateNumber.PlateNumberNativeLanguage = detectedStrings;
                justToXferPlateNumber.PlateNumberLatin = detectedStrings;
                
                m_LPRPerFrameReadingQ.Enqueue(justToXferPlateNumber);


                if (plateCount > 0 && m_AppData.DVRMode == APPLICATION_DATA.DVR_MODE.STORE_ON_PLATE_FOUND)
                {

                    // in store on plate found, store all plate readings/images on a per frame basis, no groupings.. will be redundant from image to image

                    frame.PlateNumberNativeLanguage = detectedStrings; // this contains a multiple reading from multiple plates in one image
                    frame.PlateNumberLatin = detectedStrings;

                    m_LPRFinalPlateGroupOutputQ.Enqueue(frame);
                }

            }
            catch (Exception ex)
            {
                m_Log.Log("LPRProcessImage ex : " + ex.Message, ErrorLog.LOG_TYPE.FATAL);
            }
        }

    }

  
   
}
