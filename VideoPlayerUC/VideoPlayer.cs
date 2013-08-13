using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ApplicationDataClass;
using PathsLib;
using System.Threading;
using ErrorLoggingLib;
using Utilities;
using System.IO;

namespace VideoPlayerUC
{
    public partial class VideoPlayer : UserControl
    {
        public VideoPlayer( )
        {
            InitializeComponent();
           

        }
        

        public APPLICATION_DATA AppData
        {
            set
            {
                m_AppData = value;
                m_AppData.AddOnClosing(Stop, APPLICATION_DATA.CLOSE_ORDER.FIRST);
                


                m_Paths = (PATHS)m_AppData.PathManager;
                m_Log = (ErrorLog) m_AppData.Logger;
                InitOtherControls();
            }
        }
        
        void InitOtherControls()
        {
           
            jpegArrayLock = new object();

            m_ThreadTable = new ThreadControlTable();

            dateTimeStartTime.Format = DateTimePickerFormat.Custom;
            dateTimeStartTime.CustomFormat = m_timeFormat;

            dateTimeEndTime.Format = DateTimePickerFormat.Custom;
            dateTimeEndTime.CustomFormat = m_timeFormat;

            trackBar1.SetRange(0, 100);

            m_PlayControl = new PlayControl();

            m_JpegPlayThread = new Thread(PlayLoop);
            m_JpegPlayThread.Start();

            m_WallClock = new Thread(WallClockLoop);
            m_WallClock.Start();


        }

        ErrorLog m_Log;

        string m_SelectedSource;
        string m_SelectedPSS;
       
        String m_timeFormat = "dd.MMM.yyyy : HH.mm.ss";
        APPLICATION_DATA m_AppData;
        PATHS m_Paths;


        // //////////// 

        //    Thread controls

        Thread m_JpegPlayThread;
       
        Thread m_WallClock;
     
        Thread m_LoadJpegsThread;
      
       ThreadControlTable m_ThreadTable;


        public void BlockOnThreadStop(int timeout)
        {
            while (timeout-- > 0)
            {
                bool allStopped = true;
                for (int  i = 0; i < m_ThreadTable.Count; i++)
                {
                    if (! m_ThreadTable[i]) allStopped = false;
                }

                if (allStopped) break;

                Thread.Sleep(1);
            }
        }

        private void Player_Load(object sender, EventArgs e)
        {

          
        }

        void WallClockLoop()
        {
            bool threadStopped = false;
            m_ThreadTable.Add("WallClockLoop", threadStopped);


            Thread.Sleep(2000);
            while (!m_Stop)
            {
                this.BeginInvoke((MethodInvoker)delegate { this.PostClockTime(DateTime.Now.ToUniversalTime().ToString(m_timeFormat)); });
                Thread.Sleep(1000);
            }

            m_ThreadTable["WallClockLoop"] = true;
        }
        void PostClockTime(string time)
        {
            labelCurrentTime.Text = time;
        }


       

        void StartPlay()
        {
            buttonPlay.Text = "Pause";
            m_Play = true;
        }
        void StopPlay()
        {
            buttonPlay.Text = "Play";
            m_Play = false;
        }


        /// <summary>
        /// tell the videoplayer where and when to find the jpegs that make up the sequence to start playing.
        /// </summary>
        /// <param name="PSS"></param>
        /// <param name="source"></param>
        /// <param name="start"></param>
        /// <param name="stop"></param>

        public void LoadVideClip(string PSS, string source, DateTime start, DateTime stop)
        {
            StopPlay();
            m_SelectedPSS = PSS;
            m_SelectedSource = source;
            dateTimeStartTime.Value = start;
            dateTimeEndTime.Value = stop;
           
            LoadJpegs();

        }

      

        object jpegArrayLock;
        void LoadJpegs()
        {
            m_LoadJpegsThread = new Thread(LoadJpegsLoop);
            m_LoadJpegsThread.Start();
        }

        void LoadJpegsLoop()
        {
            bool threadStopped = false;
            m_ThreadTable.Add("LoadJpegsLoop", threadStopped);

            // delay unitl window handle has been created for this control
            int delayCount = 10000;
            while (!m_Stop && ! this.IsHandleCreated)
            {
                Thread.Sleep(1);
                if  ( delayCount--  < 0 )
                {
                    m_Log.Log("time out waiting for videoPlayer handle", ErrorLog.LOG_TYPE.FATAL);
                    m_ThreadTable["LoadJpegsLoop"] = true;
                    return;
                }
            }

            StopPlay();

            Thread.Sleep(50);// wait for the play loop to break on the StopPlay()

            m_PlayControl.Reset();

            DateTime start = dateTimeStartTime.Value;
            DateTime stop = dateTimeEndTime.Value;

            lock (jpegArrayLock)
            {
               // jpegsToPlay = m_Paths.GetAllJpegsInRange(m_SelectedPSS, m_SelectedSource, start, stop);
                jpegsToPlay = m_Paths.GetAllJpegsInRange( start, stop, m_SelectedPSS, m_SelectedSource);
            }

   
            if ( this.labelNumberOfImagesFound.InvokeRequired)
                this.BeginInvoke((MethodInvoker)delegate { this.labelNumberOfImagesFound.Text = jpegsToPlay.Count().ToString(); });
    
            m_JpegPlayIndex = 0;
     
            if (this.trackBar1.InvokeRequired)
                this.BeginInvoke((MethodInvoker)delegate { this.trackBar1.Value = 0; });

            m_ThreadTable["LoadJpegsLoop"] = true;

            PlayOneFrame(true);
        }

        string[] jpegsToPlay;
        class PlayControl
        {
            public DateTime previousFrameTime;
            public TimeSpan interval;
            public int intervalCounter;

            public void Reset()
            {
                previousFrameTime = default(DateTime);
                interval = new TimeSpan(0, 0, 0, 0, 33);
                intervalCounter = 9999;
            }
        }
        PlayControl m_PlayControl;

        bool m_Play;
        bool m_Stop;

        public void Stop()
        {
            m_Stop = true;
            StopPlay();
        }

        void PlayOneFrame(bool playForward)
        {
            if (jpegsToPlay == null) return;

            if (playForward) m_JpegPlayIndex++;
            else m_JpegPlayIndex--;

            if (jpegsToPlay.Length == 0) return;
            if (m_JpegPlayIndex >= jpegsToPlay.Length) m_JpegPlayIndex = 0;
            if (m_JpegPlayIndex < 0) m_JpegPlayIndex = jpegsToPlay.Length - 1;

            lock (jpegArrayLock)
            {
                m_PlayControl.previousFrameTime = PATHS.GetTimeFromFile(jpegsToPlay[m_JpegPlayIndex]);
            }

            this.BeginInvoke((MethodInvoker)delegate { this.PushNextJpeg(jpegsToPlay[m_JpegPlayIndex]); });

            int nextIndex;

            if (playForward)
                nextIndex = m_JpegPlayIndex + 1;
            else
                nextIndex = m_JpegPlayIndex - 1;

            if (nextIndex >= jpegsToPlay.Length) nextIndex = 0;

            if (nextIndex < 0) nextIndex = jpegsToPlay.Length - 1;

            int nextNextIndex = nextIndex + 1;
            if (nextNextIndex >= jpegsToPlay.Length) nextNextIndex = nextIndex;

            DateTime nexttimestamp = PATHS.GetTimeFromFile(jpegsToPlay[nextNextIndex]);
            m_PlayControl.interval = nexttimestamp.Subtract(m_PlayControl.previousFrameTime);

            m_PlayControl.intervalCounter = 0;           
        
            TimeSpan playtime = m_PlayControl.interval.Add(new TimeSpan(0, 0, 0, 0, m_PlayControl.intervalCounter));
            this.BeginInvoke((MethodInvoker)delegate { this.PushPlayStats(m_PlayControl.previousFrameTime.Add(playtime).ToString(m_AppData.TimeFormatStringForFileNames), m_PlayControl.previousFrameTime.ToString(m_AppData.TimeFormatStringForFileNames), m_JpegPlayIndex); });
            m_PlayControl.intervalCounter++;
           
        }

        void PlayLoop()
        {
            TimeSpan interval;
            bool threadStopped=false;
            m_ThreadTable.Add("PlayLoop", threadStopped);

            DateTime actualTimeAtLastFramePlay = DateTime.Now;
            int millisecondCounter = 0;
         
            while (!m_Stop)
            {
                while (m_Play)
                {
                    if (jpegsToPlay == null) continue;
                    if ( jpegsToPlay.Length == 0) continue;
                    if (m_JpegPlayIndex >= jpegsToPlay.Length) m_JpegPlayIndex = 0;

                    if (m_JpegPlayIndex == 0) millisecondCounter = 0;

                    interval = GetNextFrameInterval();

                    if (HasFrameIntervalTimeElapsed(actualTimeAtLastFramePlay, interval))
                    {
                       
                        // reset current frame play time
                        actualTimeAtLastFramePlay = DateTime.Now;
                         

                        // play the next frame

                        this.BeginInvoke((MethodInvoker)delegate { this.PushNextJpeg(jpegsToPlay[m_JpegPlayIndex]); });

                        // increment the play index
                        if (IncrementPlayIndex() == false)
                        {
                            m_Play = false;
                            this.BeginInvoke((MethodInvoker)delegate { this.StopPlay(); });
                            continue;
                        }

                    }
                  
                    if (m_Stop) break;

                    DateTime currentPlayTime = getCurrentPlayTime(millisecondCounter);
                    DateTime currentFrameTime = getCurrentFrameTime();

                    this.BeginInvoke((MethodInvoker)delegate { this.PushPlayStats(currentPlayTime.ToString(m_AppData.TimeFormatStringForFileNames), currentFrameTime.ToString(m_AppData.TimeFormatStringForFileNames), m_JpegPlayIndex); });

                    millisecondCounter++;
                    Thread.Sleep(1);

                }

                Thread.Sleep(1);
            }
            m_ThreadTable["PlayLoop"] = true;
        }

        DateTime getCurrentPlayTime(int millisecondCounter)
        {

            int seconds = 0;
            int minutes = 0;
            int hours = 0;

            while (millisecondCounter > 999)
            {
                seconds = millisecondCounter / 1000;
                millisecondCounter = (millisecondCounter % 1000);
                while (seconds > 59)
                {
                    minutes = seconds / 60;
                    seconds = seconds % 60;

                    while (minutes > 59)
                    {
                        hours = minutes / 60;
                        minutes = minutes % 60;
                    }
                }

              
            }
            DateTime firstFrameTime = PATHS.GetTimeFromFile(jpegsToPlay[0]);

            TimeSpan interval = new TimeSpan(0, hours, minutes, seconds, millisecondCounter);

            return (firstFrameTime.Add(interval));
        }

        bool IncrementPlayIndex()
        {
            int index = m_JpegPlayIndex;
            index++;

            if (index >= jpegsToPlay.Length) return (false); //we are at the end

            m_JpegPlayIndex = index;
            
            return true;
        }

        DateTime getCurrentFrameTime()
        {
            return PATHS.GetTimeFromFile(jpegsToPlay[m_JpegPlayIndex]);
        }

        TimeSpan  GetNextFrameInterval()
        {
            int index = m_JpegPlayIndex;
            index ++;
            
            if ( index >= jpegsToPlay.Length ) return( new TimeSpan (0,0,0,0,30));// return an arbitrarily short time, since we are at the end

            DateTime frameTimeAtLastFrame = PATHS.GetTimeFromFile(jpegsToPlay[m_JpegPlayIndex]);
            DateTime frameTimeAtNextFrame = PATHS.GetTimeFromFile(jpegsToPlay[index]);

            TimeSpan interval = frameTimeAtNextFrame.Subtract(frameTimeAtLastFrame);

            return (interval);

        }


        bool HasFrameIntervalTimeElapsed(DateTime timeAtLastFramePlay, TimeSpan interval)
        {
            if (DateTime.Now.Subtract(timeAtLastFramePlay).CompareTo(interval) > 0) return (true);
            else return false;
        }

        void PushPlayStats(string playTime, string currentFrame, int jpegIndex)
        {
            labelPlayTime.Text = playTime;
            labelCurrentFrame.Text = currentFrame;
            SetTrackBarVal(jpegIndex);
        }

        int m_JpegPlayIndex = 0;
        void PushNextJpeg(string file)
        {

            try
            {
                pictureBoxMainPlayer.Image = Image.FromFile(file);
            }
            catch { }
           
        }

        private void dateTimeStartTime_ValueChanged(object sender, EventArgs e)
        {

        }

        private void dateTimeEndTime_ValueChanged(object sender, EventArgs e)
        {

        }

        private void treeViewPSSPicker_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void buttonLoadImages_Click(object sender, EventArgs e)
        {
            LoadJpegs();
        }

      

        private void buttonPlay_Click_1(object sender, EventArgs e)
        {
          
            if (buttonPlay.Text == "Play")
            {
                StartPlay();
            }
            else
            {
                StopPlay();
            }
        }

        bool ScrollDirectionWasForward;
        int PreviousScrollValue;

        private void trackBar1_Scroll_1(object sender, EventArgs e)
        {
            bool playMode = m_Play;

            if (trackBar1.Value - PreviousScrollValue > 0) 
                ScrollDirectionWasForward = true;
            else
                ScrollDirectionWasForward = false;
            
            StopPlay();
            
            lock (jpegArrayLock)
            {
                m_JpegPlayIndex = ((trackBar1.Value * jpegsToPlay.Length) / 100);

                if (m_JpegPlayIndex < 0) m_JpegPlayIndex = 0;
                if (m_JpegPlayIndex >= jpegsToPlay.Length) m_JpegPlayIndex = jpegsToPlay.Length - 1;

                m_PlayControl.Reset();
            }

            if (playMode == false)
            {
                // play one frame
                PlayOneFrame(ScrollDirectionWasForward);
            }
    
            if ( playMode == true)
                StartPlay();

            PreviousScrollValue = trackBar1.Value;
        }

        void SetTrackBarVal(int jpegIndex)
        {
            trackBar1.Value = (int) (jpegIndex * 100) / jpegsToPlay.Length;
        }

        private void VideoPlayer_Load(object sender, EventArgs e)
        {

        }

        private void buttonBackOneFrame_Click(object sender, EventArgs e)
        {
            PlayOneFrame(false);
        }

        private void buttonForwardOneFrame_Click(object sender, EventArgs e)
        {
            PlayOneFrame(true);
        }

        private void pictureBoxMainPlayer_Click(object sender, EventArgs e)
        {
            if (pictureBoxMainPlayer.Image == null) return;

            // save the current frame

            try
            {
                saveFileDialog1.Filter = " jpeg files (*.jpg)|*.jpg| Bitmap files (*.bmp)|*.bmp";

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {

                    string path = saveFileDialog1.FileName;

                    pictureBoxMainPlayer.Image.Save(path);
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }

        }

        private void buttonExportRange_Click(object sender, EventArgs e)
        {
       
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                foreach (string file in jpegsToPlay)
                {
                    try
                    {
                        FileInfo fi = new FileInfo(file);
                        File.Copy(file, folderBrowserDialog1.SelectedPath + "\\" + fi.Name);
                    }
                    catch (Exception ex) { MessageBox.Show(ex.Message+", "+file); }
                }
            }
        }
    }

}
