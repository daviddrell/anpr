using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ApplicationDataClass;
using System.Threading;
using ErrorLoggingLib;
using System.IO;
using Utilities;

namespace Control_Center
{
    public partial class LiveView : UserControl
    {
        public LiveView( APPLICATION_DATA appData)
        {
            InitializeComponent();

            m_AppData = appData;
            
            m_AppData.AddOnClosing(Stop, APPLICATION_DATA.CLOSE_ORDER.LAST);

            m_Log = (ErrorLog)m_AppData.Logger;

            LoadServerEventHandlers();

            m_GetImagesThread = new Thread(GetImagesLoop);
            m_GetImagesThread.Start();
        }

        APPLICATION_DATA m_AppData;
        SelectRemoteSystem m_SelectRemoteSystem;
        ErrorLog m_Log;

        bool m_Stop = false;
        public void Stop()
        {
            m_Stop = true;   
        }

        void LoadServerEventHandlers( )
        {
            m_SelectRemoteSystem = (SelectRemoteSystem)m_AppData.SelectServer;

            m_SelectRemoteSystem.MessageGenerators.OnCurrentServerLoggedOut += OnCurrentServerLoggedOut;
            m_SelectRemoteSystem.MessageGenerators.OnNewServerSelected += OnNewServerSelected;
        }

        void OnNewServerSelected(RemoteHosts host)
        {
            this.BeginInvoke((MethodInvoker)delegate { this.LoadSourceChannels(); });
        }

        void OnCurrentServerLoggedOut(RemoteHosts host)
        {
            this.BeginInvoke((MethodInvoker)delegate { this.UnLoadHostFromLiveView(); });
        }

       

        public void UnLoadHostFromLiveView()
        {
            StopPlay();

            textBoxCurrentRemoteServer.Text = "no server selected";

            ClearListBox();

            jpegQ.Clear();
        }


        void LoadSourceChannels()
        {
            // register with this server to receive images

            ((RemoteHosts)m_AppData.CurrentlyLoggedInRemoteServer).MessageEventGenerators.OnRxJpeg += OnRxJpeg;

            ClearListBox();

            textBoxCurrentRemoteServer.Text = ((RemoteHosts)m_AppData.CurrentlyLoggedInRemoteServer).HostNameStatus;

            GetChannelsThread = new Thread(BlockOnGetChannels);
            GetChannelsThread.Start();

        }

        Thread GetChannelsThread;

        void BlockOnGetChannels()
        {

            string[] channelList = ((RemoteHosts)m_AppData.CurrentlyLoggedInRemoteServer).GetChannelList();

            if (channelList == null)
            {
                this.BeginInvoke((MethodInvoker)delegate { this.AddItemToChannelListBox("error reading channel list"); });
                return;
            }

            int cnt = channelList.Count();

    
            for (int i = 0; i < cnt; i++)
            {
                string s = channelList[i];
                
                if (m_Stop) break;

                this.BeginInvoke((MethodInvoker)delegate { this.AddItemToChannelListBox(s); });
            }
        }

        void ClearListBox()
        {
            comboBoxSourceChannelList.Items.Remove(comboBoxSourceChannelList.SelectedItem);

            for (int i = comboBoxSourceChannelList.Items.Count - 1; i >= 0; i--)
            {
                comboBoxSourceChannelList.Items.RemoveAt(i);
            }
            comboBoxSourceChannelList.SelectedItem = null;
        }

        void AddItemToChannelListBox(string channel)
        {
            if (channel.Length < 1) return;
            comboBoxSourceChannelList.Items.Add(channel);
        }



        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        // //////////////////////////
        //
        //    Image Display Thread
        
        // start playing images when the user has selected a channel and clicked play

        Thread m_GetImagesThread;
        
        DateTime m_WaitingOnNextImageTimer;
        int m_FramesPerSecond = 10;// viewing rate
        int m_DelayBetweenFrames;
    

        void GetImagesLoop()
        {
            jpegQ = new ThreadSafeQueue<JPEG>(3);

           m_DelayBetweenFrames = (1000 / m_FramesPerSecond);
           
            TimeSpan waitOnNextImageTimeSpan = new TimeSpan(0,0,2);

            // register for frames with the RCS
         
            while (!m_Stop)
            {
                while (m_Playing && !m_Stop)
                {
                    foreach (string chan in comboBoxSourceChannelList.Items)
                    {
                        ((RemoteHosts)m_AppData.CurrentlyLoggedInRemoteServer).SendLiveViewRequest(chan);

                    
                        m_WaitingOnNextImageTimer = DateTime.Now;
                    }

                    if (m_Stop) break;
                    Thread.Sleep(m_DelayBetweenFrames);
                }
                Thread.Sleep(10);
            }
        }

        ThreadSafeQueue<JPEG> jpegQ;
        class JPEG
        {
            public byte[] jpeg;
            public string timeStamp;
            public string plateReading;
            public string channel;
        }

        void OnRxJpeg(byte[] jpegData, string channel, string timeStamp, string plateReading)
        {
        

            if (! m_Playing) return;

            JPEG jpeg = new JPEG();
            jpeg.timeStamp = timeStamp;
            jpeg.jpeg = jpegData;
            jpeg.plateReading = plateReading;
            jpeg.channel = channel;
          
            
            jpegQ.Enqueue(jpeg);
        
            this.BeginInvoke((MethodInvoker)delegate { this.PostJpeg(); });
           
        }

        void PostJpeg( )
        {
            try
            {
                JPEG jpeg = null;

                if (jpegQ.Count > 0)
                    jpeg = jpegQ.Dequeue();
                else
                    return;

                if (jpeg == null) return;

                Image image = Bitmap.FromStream(new MemoryStream(jpeg.jpeg));

                if (image == null) return;

                int index = 0;
                try
                {
                    index = comboBoxSourceChannelList.Items.IndexOf(jpeg.channel);
                }
                catch { }

                switch (index)
                {
                    case 0:
                        pictureBoxVideoDisplay0.Image = image;
                        pictureBoxVideoDisplay0.Invalidate();
                        labelDisplayFrameTime0.Text = (string)jpeg.timeStamp.Clone();
                        if (jpeg.plateReading.Length > 2)
                            labelPlateReadings0.Text = jpeg.plateReading;

                        break;

                    case 1:
                        pictureBoxVideoDisplay1.Image = image;
                        pictureBoxVideoDisplay1.Invalidate();
                        labelDisplayFrameTime1.Text = (string)jpeg.timeStamp.Clone();
                        if (jpeg.plateReading.Length > 2)
                            labelPlateReadings1.Text = jpeg.plateReading;


                        break;

                    case 2:
                        pictureBoxVideoDisplay2.Image = image;
                        pictureBoxVideoDisplay2.Invalidate();
                        labelDisplayFrameTime2.Text = (string)jpeg.timeStamp.Clone();
                        if (jpeg.plateReading.Length > 2)
                            labelPlateReadings2.Text = jpeg.plateReading;


                        break;

                    case 3:
                        pictureBoxVideoDisplay3.Image = image;
                        pictureBoxVideoDisplay3.Invalidate();
                        labelDisplayFrameTime3.Text = (string)jpeg.timeStamp.Clone();
                        if (jpeg.plateReading.Length > 2)
                            labelPlateReadings3.Text = jpeg.plateReading;


                        break;
                }


                jpeg = null;// free it
            }
            catch { }
        }

        string m_SelectedChannel = null;
        bool m_Playing = false;
        private void buttonPlayPause_Click(object sender, EventArgs e)
        {

            if (m_Playing)
            {
                StopPlay();
            }
            else //if (m_SelectedChannel != -1) 
            {
                StartPlay();
            }
        }

        void StopPlay()
        {
            m_Playing = false;
            buttonPlayPause.Text = "Play";
            pictureBoxVideoDisplay0.Image = null;
            pictureBoxVideoDisplay0.Invalidate();
        }

        void StartPlay()
        {
            m_Playing = true;
            buttonPlayPause.Text = "Pause";
        }

        private void comboBoxSourceChannelList_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                m_SelectedChannel = comboBoxSourceChannelList.SelectedItem.ToString();
            }
            catch (Exception ex)
            {
                m_Log.Log("comboBoxSourceChannelList_SelectedIndexChanged ex : " + ex.Message, ErrorLog.LOG_TYPE.INFORMATIONAL);
            }
        }

        private void label10_Click(object sender, EventArgs e)
        {

        }
    }
}
