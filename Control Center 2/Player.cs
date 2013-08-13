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
using UserSettingsLib;
using System.Threading;

namespace Control_Center
{
    public partial class Player : UserControl
    {
        public Player(APPLICATION_DATA appData)
        {
            InitializeComponent();
            m_AppData = appData;
            m_AppData.AddOnClosing(Stop,APPLICATION_DATA.CLOSE_ORDER.FIRST);

            m_Paths = (PATHS)m_AppData.PathManager;

            jpegArrayLock = new object();
     
            dateTimeStartTime.Format = DateTimePickerFormat.Custom;
            dateTimeStartTime.CustomFormat = m_timeFormat;

            dateTimeEndTime.Format = DateTimePickerFormat.Custom;
            dateTimeEndTime.CustomFormat = m_timeFormat;


            listBoxSelectImageStore.SelectedIndexChanged += new EventHandler(listBoxSelectImageStore_SelectedIndexChanged);
            listBoxSelectImageStore.Click += new EventHandler(listBoxSelectImageStore_Click);
            listBoxSelectPSS.SelectedIndexChanged += new EventHandler(listBoxSelectPSS_SelectedIndexChanged);
            listBoxSelectSource.SelectedIndexChanged += new EventHandler(listBoxSelectSource_SelectedIndexChanged);

            trackBar1.SetRange(0, 100);
         
        }

       
        string m_SelectedSource;
        string m_SelectedPSS;
        Hashtable m_PSSPaths;
        String m_timeFormat = "dd.MMM.yyyy : HH.mm.ss";
        APPLICATION_DATA m_AppData;
        PATHS m_Paths;
        Thread m_JpegPlayThread;
        Thread m_WallClock;

        private void Player_Load(object sender, EventArgs e)
        {

            AddAllDrivesToListBox(m_Paths.GetAllAttachedFrameStoreDrives());

            m_JpegPlayThread = new Thread(PlayLoop);
            m_JpegPlayThread.Start();

            m_WallClock = new Thread(WallClockLoop);
            m_WallClock.Start();

        }

        void WallClockLoop()
        {
            while (!m_Stop)
            {
                this.BeginInvoke((MethodInvoker)delegate { this.PostClockTime(DateTime.Now.ToUniversalTime().ToString(m_timeFormat)); });
                Thread.Sleep(1000);
            }
        }
        void PostClockTime(string time)
        {
            labelCurrentTime.Text = time;
        }

        void listBoxSelectImageStore_Click(object sender, EventArgs e)
        {
            listBoxSelectImageStore_SelectedIndexChanged(sender, e);
        }

        void listBoxSelectSource_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_SelectedSource = (string)listBoxSelectSource.SelectedItem.ToString();
      
        }
       

        void listBoxSelectImageStore_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBoxSelectPSS.Items.Clear();
            listBoxSelectSource.Items.Clear();
            labelNumberOfImagesFound.Text = "0";

            m_Paths.Drive = (string) listBoxSelectImageStore.SelectedItem.ToString();
            string [] pssNodePaths;
            string[] pssNodes = m_Paths.GetPSSNodes(out pssNodePaths);
            if (pssNodes == null) return;

            m_PSSPaths = new Hashtable();

            for (int i = 0; i < pssNodes.Count(); i++ )
            {
                listBoxSelectPSS.Items.Add(pssNodes[i]);
                m_PSSPaths.Add(pssNodes[i], pssNodePaths[i]);
            }
        }

        void AddAllDrivesToListBox(string[] drives)
        {
            foreach (string drive in drives)
                listBoxSelectImageStore.Items.Add (drive);
        }

        void listBoxSelectPSS_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBoxSelectSource.Items.Clear();
            labelNumberOfImagesFound.Text = "0";

            m_SelectedPSS = (string)m_PSSPaths[(string)listBoxSelectPSS.SelectedItem.ToString()];

            string[] sources = m_Paths.GetCameraSourceNames(m_SelectedPSS);
           
            foreach (string source in sources)
            {
                listBoxSelectSource.Items.Add(source);
            }
        }
     
        private void buttonPlay_Click(object sender, EventArgs e)
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

        object jpegArrayLock;
        void LoadJpegs()
        {
            StopPlay();
            DateTime start = dateTimeStartTime.Value;
            DateTime stop = dateTimeEndTime.Value;

            lock (jpegArrayLock)
            {
               // jpegsToPlay = m_Paths.GetAllJpegsInRange(m_SelectedPSS, m_SelectedSource, start, stop);
                jpegsToPlay = m_Paths.GetAllJpegsInRange(start, stop);
            }

            labelNumberOfImagesFound.Text = jpegsToPlay.Count().ToString();
            m_JpegPlayIndex = 0;

            m_PlayControl.Reset();

            trackBar1.Value = 0;
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

        void Stop()
        {
            m_Stop = true;
        }

        void PlayLoop()
        {
           
            m_PlayControl = new PlayControl();
            m_PlayControl.Reset();

            while (!m_Stop)
            {
                while (m_Play)
                {
                    if (m_PlayControl.intervalCounter >= (int)m_PlayControl.interval.TotalMilliseconds)
                    {
                        lock (jpegArrayLock)
                        {
                            m_PlayControl.previousFrameTime = PATHS.GetTimeFromFile(jpegsToPlay[m_JpegPlayIndex]);
                        }

                        this.BeginInvoke((MethodInvoker)delegate { this.PushNextJpeg(); });

                        if (m_JpegPlayIndex == jpegsToPlay.Length - 1)
                        {
                            m_Play = false;
                            this.BeginInvoke((MethodInvoker)delegate { this.StopPlay(); });
                            continue;
                        }
                        DateTime nexttimestamp = PATHS.GetTimeFromFile(jpegsToPlay[m_JpegPlayIndex + 1]);
                        m_PlayControl.interval = nexttimestamp.Subtract(m_PlayControl.previousFrameTime);

                        m_JpegPlayIndex++;
                        m_PlayControl.intervalCounter = 0;
                      
                    }
                    if (m_Stop) break;
                    TimeSpan playtime = m_PlayControl.interval.Add(new TimeSpan(0, 0, 0, 0, m_PlayControl.intervalCounter));
                    this.BeginInvoke((MethodInvoker)delegate { this.PushPlayStats(m_PlayControl.previousFrameTime.Add(playtime).ToString(m_AppData.TimeFormatStringForFileNames), m_PlayControl.previousFrameTime.ToString(m_AppData.TimeFormatStringForFileNames)); });
                    m_PlayControl.intervalCounter++;
                    Thread.Sleep(1);
                    
                }

                Thread.Sleep(1);
            }
        }

        void PushPlayStats(string playTime, string currentFrame)
        {
            labelPlayTime.Text = playTime;
            labelCurrentFrame.Text = currentFrame;
        }

        int m_JpegPlayIndex = 0;
        void PushNextJpeg()
        {
            lock (jpegArrayLock)
            {
                pictureBoxMainPlayer.Image = Image.FromFile(jpegsToPlay[m_JpegPlayIndex]);
            }
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

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            StopPlay();
            lock (jpegArrayLock)
            {
                m_JpegPlayIndex = ((trackBar1.Value * jpegsToPlay.Length) / 100) ;

                if (m_JpegPlayIndex < 0) m_JpegPlayIndex = 0;
                if (m_JpegPlayIndex >= jpegsToPlay.Length) m_JpegPlayIndex = jpegsToPlay.Length - 1;

                m_PlayControl.Reset();
            }
            StartPlay();
        }
    }


    class MyScrollBar : ScrollBar
    {


    }
}
