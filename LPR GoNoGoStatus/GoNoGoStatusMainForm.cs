using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RCSClientLib;
using ApplicationDataClass;
using ErrorLoggingLib;
using System.Threading;
using Utilities;
using System.IO;
using LPROCR_Wrapper;
using System.Reflection;

namespace LPR_GoNoGoStatus
{
    public partial class GoNoGoStatusMainForm : Form
    {
        public GoNoGoStatusMainForm()
        {
            InitializeComponent();
                        
            this.Text = "First Evidence GoNoGo System Monitor, version " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();

            m_AppData = new APPLICATION_DATA();
            m_Log = new ErrorLog(m_AppData);
            m_AppData.Logger =(object) m_Log;

            LPROCR_Lib LPRlib = new LPROCR_Lib();

            // get plate min max to draw the min/max target boxes for the user to see
            LPRlib.GetMinMaxPlateSize(ref minW, ref maxW, ref minH, ref maxH);

            // fudge the min box size so the user will make the plate bigger
            minH += 20;
            minW += 40;


            m_ReceiveDataSingleton = new object();

            jpegQ = new ThreadSafeQueue<JPEG>(30);

            m_PreviousPlateNumber = new string[4];
            m_UneditedImages = new UNEDITEDIMAGES[4];
            m_UneditedImages[0] = new UNEDITEDIMAGES();
            m_UneditedImages[1] = new UNEDITEDIMAGES();
            m_UneditedImages[2] = new UNEDITEDIMAGES();
            m_UneditedImages[3] = new UNEDITEDIMAGES();

            m_SystemStatusLock = new object(); 
            m_SystemStatus = new SYSTEM_STATUS_STRINGS();


            m_FullScreenPB = new bool[4];
            m_FullScreenPB[0] = false;
            m_FullScreenPB[1] = false;
            m_FullScreenPB[2] = false;
            m_FullScreenPB[3] = false;

            this.FormClosing += new FormClosingEventHandler(GoNoGoStatusMainForm_FormClosing);

            this.Resize +=new EventHandler(GoNoGoStatusMainForm_Resize);

            m_ServerConnection = new RCSClient( m_AppData);

            m_PollingThread = new Thread(PollingLoop);


            m_ServerConnection.MessageEventGenerators.OnRxChannelList += OnReceiveChannels;
            m_ServerConnection.MessageEventGenerators.OnRxJpeg += OnNewJpeg;
            m_ServerConnection.MessageEventGenerators.OnRxHealthStatus += OnRxStats;

            pb_ClickGenericHandler = new pb_ClickDelegate[4];
            pb_ClickGenericHandler[0] = pb_Click0;
            pb_ClickGenericHandler[1] = pb_Click1;
            pb_ClickGenericHandler[2] = pb_Click2;
            pb_ClickGenericHandler[3] = pb_Click3;

            m_JpegPlayControl = new JPEG_PLAY_CONTROL[4];
            m_JpegPlayControl[0] = new JPEG_PLAY_CONTROL();
            m_JpegPlayControl[1] = new JPEG_PLAY_CONTROL();
            m_JpegPlayControl[2] = new JPEG_PLAY_CONTROL();
            m_JpegPlayControl[3] = new JPEG_PLAY_CONTROL();


            progressBarPlateProcessQueueLevel = new MyProgressBar();
            progressBarPlateProcessQueueLevel.Location = new Point(10, 40);
            progressBarPlateProcessQueueLevel.Size = new Size(groupBoxPlateProcessQueLevel.Size.Width-20, 30);

            groupBoxPlateProcessQueLevel.Controls.Add(progressBarPlateProcessQueueLevel);

            buttonSaveCurrentImage.Visible = false;
            buttonSaveCurrentImage.Enabled = false;

            m_PollingThread.Start();
        }

        void GoNoGoStatusMainForm_Resize(object sender, EventArgs e)
        {
            ResizeVidPanel();
        }

        int maxW, minW, maxH, minH;
        JPEG_PLAY_CONTROL[] m_JpegPlayControl;
        object m_SystemStatusLock;
        SYSTEM_STATUS_STRINGS m_SystemStatus;

        RCSClient m_ServerConnection;
        APPLICATION_DATA m_AppData;
        ErrorLog m_Log;
        PictureBox[] m_VideDisplayPBs;
        Thread m_PollingThread;
        
        MyProgressBar progressBarPlateProcessQueueLevel;

        object m_ReceiveDataSingleton;
        string[] m_ChanneList;
        bool m_ServiceRunning = false;


        private void GoNoGoStatusMainForm_Load(object sender, EventArgs e)
        {
            // position 307, 13

            panelVidDisplayPanel.Location = new Point(300, 5);

            m_VideDisplayPBs = new PictureBox[4];

            for (int i = 0; i < 4; i++)
            {
                m_VideDisplayPBs[i] = CreateVideoPB(i);
                panelVidDisplayPanel.Controls.Add(m_VideDisplayPBs[i]);
            }

            m_ServerConnection.Connect("127.0.0.1", true, OnConnectStatusChangedEvent);
        }

        string dontCarePassword = " ";// NCIS wants to remove use of passwords.

        void PollingLoop()
        {
            int count = 33;

            m_ServerConnection.SendGetChannelsRequest(dontCarePassword);

            //debug
           

            while (!m_Stop)
            {
                Application.DoEvents();

                Thread.Sleep(1);

                if (!m_ServiceRunning )
                {
                    m_SystemStatus.Reset();
             
                    Thread.Sleep(250);

                    this.BeginInvoke((MethodInvoker)delegate { PostStats(m_SystemStatus); });

                    continue;
                }

             

              

                lock (m_ReceiveDataSingleton)
                {
                    if (m_ChanneList != null)
                    {
                        foreach (string channel in m_ChanneList)
                        {
                            int chan = (int)m_ChannelIndexTable[channel];

                            if (m_JpegPlayControl[chan].TimeToSendANewRequest())
                            {
                                m_ServerConnection.SendLiveViewRequest(channel, dontCarePassword);

                                m_JpegPlayControl[chan].SendingRequestNow();

                            }

                            Application.DoEvents();
                        }
                    }
                }

                if (count < 0)
                {
                    m_ServerConnection.SendGetHealthStatsRequest(dontCarePassword);
                    count = 33;
                }
                count--;
                
            }
        }

       
        class JPEG_PLAY_CONTROL
        {
            TimeSpan timeOut = new TimeSpan(0, 0, 0,0, 500);
            TimeSpan frameRate = new TimeSpan(0, 0, 0, 0, 66); //66 = 15 fps
            TimeSpan aLongTimeAgo = new TimeSpan(1, 0, 0);
            DateTime timeAtLastRequest;
            DateTime timeAtLastReceived;
            bool waitingOnNextJpeg;
            object singleton;
            public JPEG_PLAY_CONTROL()
            {
                singleton = new object();
                lock(singleton)
                {
                    waitingOnNextJpeg = false;
                    timeAtLastRequest = DateTime.Now.Subtract(aLongTimeAgo);
                    timeAtLastReceived = DateTime.Now;
                }
            }

            public void SendingRequestNow()
            { lock (singleton) { timeAtLastRequest = DateTime.Now; waitingOnNextJpeg = true; } }
            
            public void ReceivedImageNow()
            { lock (singleton) { timeAtLastReceived = DateTime.Now; waitingOnNextJpeg = false; } }
            
            public bool TimeToSendANewRequest() 
            {
                // if we have received a jpeg after we last sent a request, and 30 millisecond has elapsed, send another request
                // if we have not received a jpeg after we last sent a request, and 5 seconds has elapsed, send another request
                if (!waitingOnNextJpeg)
                {
                    if (DateTime.Now.Subtract(timeAtLastRequest).CompareTo(frameRate) > 0)
                        return (true);
                }
                if ( waitingOnNextJpeg)
                {
                    if (DateTime.Now.Subtract(timeAtLastRequest).CompareTo(timeOut) > 0)
                        return (true);
                }
                return (false);
            }
        }

        ThreadSafeHashTable m_ChannelIndexTable;

        void OnReceiveChannels(string[] list)
        {
           

            lock (m_ReceiveDataSingleton)
            {
                m_ChannelIndexTable = new ThreadSafeHashTable(4);
                m_ChanneList = list;
                int i  = 0;
                foreach (string chan in list)
                {
                    m_ChannelIndexTable.Add(chan,i);
                    i++;
                }
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

        void OnNewJpeg(byte[] jpegData, string channel, string timeStamp, string plateReading)
        {
       

            JPEG jpeg = new JPEG();
            jpeg.timeStamp = timeStamp;
            jpeg.jpeg = jpegData;
            jpeg.plateReading = plateReading;
            jpeg.channel = channel;

           

            jpegQ.Enqueue(jpeg);

            this.BeginInvoke((MethodInvoker)delegate { this.PostJpeg(); });

        }

        void PostJpeg()
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
                    index =(int) m_ChannelIndexTable[jpeg.channel];
                }
                catch { }

                m_JpegPlayControl[index].ReceivedImageNow();

                m_UneditedImages[index].Bmp =(Bitmap) image;

                m_VideDisplayPBs[index].Image = image;
                PutChannelText(m_VideDisplayPBs[index], jpeg.channel);
                PutPlateNumber(m_VideDisplayPBs[index], jpeg.plateReading, index);
                PutTimeStamp(m_VideDisplayPBs[index], jpeg.timeStamp);
                DrawMinMaxBoxes(m_VideDisplayPBs[index]);

                m_VideDisplayPBs[index].Invalidate();

              
                jpeg = null;// free it
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        bool m_Stop = false;
        void GoNoGoStatusMainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_Stop = true;
            m_AppData.CloseApplication();
        }


        private void labelGPSStatus_Click(object sender, EventArgs e)
        {

        }

   
        void OnConnectStatusChangedEvent(RCSClient.STATE state, string details)
        {
            if (state == RCSClient.STATE.CONNECTED)
            {
                m_ServiceRunning = true;

            }
            else
            {
                m_ServiceRunning = false;
            }
        }

        protected override void OnResizeEnd(EventArgs e)
        {
            ResizeVidPanel();

            base.OnResizeEnd(e);
        }

        //protected override void OnPaint(PaintEventArgs e)
        //{
        //    ResizeVidPanel();
        //    base.OnPaint(e);
        //}

        void ResizeVidPanel()
        {
            // re-size the video display panel to take up all of the form except the left side status area
            int leftXSpace = 300;

            panelVidDisplayPanel.Size = new Size(this.Size.Width - leftXSpace - 25, this.Size.Height - 50);
            panelVidDisplayPanel.Invalidate();

            for (int i = 0; i < 4; i++)
            {
                ResizeVidDisplayPB(m_VideDisplayPBs[i], i);
                m_VideDisplayPBs[i].Invalidate();
            }

        }


        void ResizeVidDisplayPB(PictureBox pb, int chanIndex)
        {
           
            if (m_FullScreenPB[chanIndex])
            {
                // push all to the back
                for (int i = 0; i < 4; i++)
                {
                    if (m_VideDisplayPBs[i] == null) continue;
                    m_VideDisplayPBs[i].SendToBack();
                }

                // bring the selected to the front and full size

                pb.Size = new Size(panelVidDisplayPanel.Size.Width  - 10, panelVidDisplayPanel.Size.Height - 10);

                int locationX =  5;
                int locationY =  5;

                pb.Location = new Point(locationX, locationY);
                pb.BringToFront();
            }
            else
            {
                pb.Size = new Size(panelVidDisplayPanel.Size.Width / 2 - 10, panelVidDisplayPanel.Size.Height / 2 - 10);

                int locationX = (chanIndex % 2) * panelVidDisplayPanel.Size.Width / 2 + 5;
                int locationY = (chanIndex > 1) ? (panelVidDisplayPanel.Size.Height / 2 + 5) : (5);

                pb.Location = new Point(locationX, locationY);

                
            }
        }


        PictureBox CreateVideoPB(int chanIndex)
        {
            PictureBox pb = new PictureBox();

            pb.SizeMode = PictureBoxSizeMode.StretchImage;
            pb.BackColor = Color.Black;

            pb.Click += new EventHandler(pb_ClickGenericHandler[chanIndex]);

            ResizeVidDisplayPB(pb, chanIndex);

            pb.Image = new Bitmap(pb.Width, pb.Height);

            PutChannelText(pb, "Channel: "+(chanIndex+1).ToString());

            DrawMinMaxBoxes(pb);

            return (pb);
        }


        bool[] m_FullScreenPB;

        delegate void pb_ClickDelegate(object sender, EventArgs e);
        pb_ClickDelegate[] pb_ClickGenericHandler;

        void pb_Click0(object sender, EventArgs e) { HandlePictureBoxClick(0); }
        void pb_Click1(object sender, EventArgs e) { HandlePictureBoxClick(1); }
        void pb_Click2(object sender, EventArgs e) { HandlePictureBoxClick(2); }
        void pb_Click3(object sender, EventArgs e) { HandlePictureBoxClick(3); }

        int m_CurrentlySelectedPB = 0;

        void HandlePictureBoxClick(int index)
        {
            if ( m_FullScreenPB[index] )
            {
                // go back to four up
                m_FullScreenPB[index] = false;
                ResizeVidDisplayPB(m_VideDisplayPBs[index] , index);
                buttonSaveCurrentImage.Enabled = false;
                buttonSaveCurrentImage.Visible = false;
            }
            else
            {
                // go to full screen on this one
                m_FullScreenPB[index] = true;
                ResizeVidDisplayPB(m_VideDisplayPBs[index], index);
                m_CurrentlySelectedPB = index;
                buttonSaveCurrentImage.Enabled = true;
                buttonSaveCurrentImage.Visible = true;
            }
        }

        void DrawMinMaxBoxes(PictureBox pb)
        {

            if (pb.Image == null) return;

            Graphics g = Graphics.FromImage(pb.Image);

            Pen p = new Pen(Color.Red);

            int centerX = pb.Image.Width / 2;
            int centerY = pb.Image.Height / 2;

            int leftX = centerX - (minW/2);
            if (leftX < 0) leftX = 0;

            int rightX = centerX + (minW/2);
            if (rightX > pb.Image.Width - 1) rightX = pb.Image.Width - 1;

            int topY = centerY - (minH/2);
            if (topY < 0) topY = 0;

            int bottomY = centerY + (minH/2);
            if (bottomY > pb.Image.Height - 1) bottomY = pb.Image.Height - 1;

            g.DrawLine(p, new Point(leftX,topY), new Point(rightX, topY));
            g.DrawLine(p, new Point(rightX, topY), new Point(rightX, bottomY));
            g.DrawLine(p, new Point(rightX, bottomY), new Point(leftX, bottomY));
            g.DrawLine(p, new Point(leftX, bottomY), new Point(leftX, topY));

            p = new Pen(Color.Red);


            leftX = centerX - (maxW / 2);
            if (leftX < 0) leftX = 0;

            rightX = centerX + (maxW / 2);
            if (rightX > pb.Image.Width - 1) rightX = pb.Image.Width - 1;

            topY = centerY - (maxH / 2);
            if (topY < 0) topY = 0;

            bottomY = centerY + (maxH / 2);
            if (bottomY > pb.Image.Height - 1) bottomY = pb.Image.Height - 1;

            g.DrawLine(p, new Point(leftX, topY), new Point(rightX, topY));
            g.DrawLine(p, new Point(rightX, topY), new Point(rightX, bottomY));
            g.DrawLine(p, new Point(rightX, bottomY), new Point(leftX, bottomY));
            g.DrawLine(p, new Point(leftX, bottomY), new Point(leftX, topY));


        }

        void PutChannelText(PictureBox pb, string name)
        {
            if (pb.Image == null) return;

            Graphics g = Graphics.FromImage( pb.Image);

            Point loc = new Point(5,10);
            Font font = new Font( FontFamily.GenericSansSerif, 16.0f, FontStyle.Regular);
            Brush brush = Brushes.White;

            g.DrawString(name, font, brush, loc);
        }

        string[] m_PreviousPlateNumber;

        void PutPlateNumber(PictureBox pb, string plateNumber, int index)
        {
            if (pb.Image == null) return;

            plateNumber = plateNumber.Replace('^', ',');// for John
           

            if (plateNumber.Length < 2)
                plateNumber = m_PreviousPlateNumber[index];
            else
                m_PreviousPlateNumber[index] = plateNumber;

            Graphics g = Graphics.FromImage(pb.Image);

            Point loc = new Point(5, pb.Image.Height - 45);
            Font font = new Font(FontFamily.GenericSansSerif, 16.0f, FontStyle.Regular);
            Brush brush = Brushes.White;

            g.DrawString(plateNumber, font, brush, loc);
        }

        void PutTimeStamp(PictureBox pb, string name)
        {
            if (pb.Image == null) return;

            Graphics g = Graphics.FromImage(pb.Image);

            Point loc = new Point(5, pb.Image.Height - 25);
            Font font = new Font(FontFamily.GenericSansSerif, 16.0f, FontStyle.Regular);
            Brush brush = Brushes.White;

            g.DrawString(name, font, brush, loc);
        }



        class STATUS_STRING 
        {
            public string status;
            public Color color;
        }

        class STATUS_COUNT
        {
            public int count;
            public Color color;
        }


        class SYSTEM_STATUS_STRINGS
        {
            STATUS_STRING frameGrabberJpegCompressor;
            public STATUS_STRING FrameGrabber_1
            { get { lock (singleton) { return frameGrabberJpegCompressor; } } set { lock (singleton) { frameGrabberJpegCompressor = value; } } }

            STATUS_STRING frameGrabberBitmapCapture;
            public STATUS_STRING FrameGrabber_0
            { get { lock (singleton) { return frameGrabberBitmapCapture; } } set { lock (singleton) { frameGrabberBitmapCapture=value; } } }

            STATUS_STRING gPS;
            public STATUS_STRING GPS
            { get { lock (singleton) { return gPS; } } set { lock (singleton) { gPS=value; } } }

            STATUS_STRING service;   // service running or not running
            public STATUS_STRING Service
            { get { lock (singleton) { return service; } } set { lock (singleton) { service = value; } } }

            STATUS_STRING serviceVersion;   // service running or not running
            public STATUS_STRING ServiceVersion
            { get { lock (singleton) { return serviceVersion; } } set { lock (singleton) { serviceVersion = value; } } }

            STATUS_STRING videoChannel1;
            public STATUS_STRING VideoChannel1
            { get { lock (singleton) { return videoChannel1; } } set { lock (singleton) {  videoChannel1=value; } } }

            STATUS_STRING videoChannel2;
            public STATUS_STRING VideoChannel2
            { get { lock (singleton) { return (videoChannel2); } } set { lock (singleton) {  videoChannel2=value; } } }

            STATUS_STRING videoChannel3;
            public STATUS_STRING VideoChannel3
            { get { lock (singleton) { return videoChannel3; } } set { lock (singleton) { videoChannel3 = value; } } }

            STATUS_STRING videoChannel4;
            public STATUS_STRING VideoChannel4
            { get { lock (singleton) { return videoChannel4; } } set { lock (singleton) { videoChannel4 = value; } } }

            STATUS_STRING drive;
            public STATUS_STRING Drive
            { get { lock (singleton) { return drive; } } set { lock (singleton) { drive = value; } } }

            STATUS_STRING hotswap;
            public STATUS_STRING Hotswap
            { get { lock (singleton) { return hotswap; } } set { lock (singleton) { hotswap = value; } } }


            STATUS_COUNT lprPlateProcessQueCount;
            public STATUS_COUNT LprPlateProcessQueCount
            { get { lock (singleton) { return lprPlateProcessQueCount; } } set { lock (singleton) { lprPlateProcessQueCount = value; } } }

            object singleton;

            public void Reset()
            {
                lock (singleton)
                {
                    frameGrabberJpegCompressor = new STATUS_STRING();
                    frameGrabberJpegCompressor.status = "No Device Connected";
                    frameGrabberJpegCompressor.color = Color.Red;


                    frameGrabberBitmapCapture = new STATUS_STRING();
                    frameGrabberBitmapCapture.status = "No Device Connected";
                    frameGrabberBitmapCapture.color = Color.Red;


                    GPS = new STATUS_STRING();
                    GPS.status = "No GPS Device, No fixed position set";
                    GPS.color = Color.Red;

                    Drive = new STATUS_STRING();
                    Drive.status = "No Drive Connected";
                    Drive.color = Color.Red;

                    Service = new STATUS_STRING();
                    Service.status = "Not Running";
                    Service.color = Color.Red;

                    ServiceVersion = new STATUS_STRING();
                    ServiceVersion.status = "0.0.0.0";
                    Service.color = Color.Red;

                    videoChannel1 = new STATUS_STRING();
                    videoChannel1.status = "No Video Connected";
                    videoChannel1.color = Color.Red;

                    videoChannel2 = new STATUS_STRING();
                    videoChannel2.status = "No Video Connected";
                    videoChannel2.color = Color.Red;

                    videoChannel3 = new STATUS_STRING();
                    videoChannel3.status = "No Video Connected";
                    videoChannel3.color = Color.Red;

                    videoChannel4 = new STATUS_STRING();
                    videoChannel4.status = "No Video Connected";
                    videoChannel4.color = Color.Red;

                    hotswap = new STATUS_STRING();
                    hotswap.color = Color.Red;
                    hotswap.status = "not started";

                    lprPlateProcessQueCount = new STATUS_COUNT();
                    lprPlateProcessQueCount.count = 0;
                    lprPlateProcessQueCount.color = Color.Green;
                }
            }
            public SYSTEM_STATUS_STRINGS()
            {
                singleton = new object() ;
                Reset();
            }
        }

        void PostStats(SYSTEM_STATUS_STRINGS status)
        {
          

            buttonChannel1.Text = "Channel 1 " + status.VideoChannel1.status;
            buttonChannel1.BackColor = status.VideoChannel1.color;

            buttonChannel2.Text = "Channel 2 " + status.VideoChannel2.status;
            buttonChannel2.BackColor  = status.VideoChannel2.color;

            buttonChannel3.Text = "Channel 3 " + status.VideoChannel3.status;
            buttonChannel3.BackColor  = status.VideoChannel3.color;

            buttonChannel4.Text = "Channel 4 " + status.VideoChannel4.status;
            buttonChannel4.BackColor  = status.VideoChannel4.color;


            buttonFGStatus_1.Text = status.FrameGrabber_1.status;
            buttonFGStatus_1.BackColor = status.FrameGrabber_1.color;

            buttonFGStatus_0.Text = status.FrameGrabber_0.status;
            buttonFGStatus_0.BackColor = status.FrameGrabber_0.color;

            buttonGPSStatus.Text = status.GPS.status;
            buttonGPSStatus.BackColor = status.GPS.color;


            textBoxHotSwapStatus.Text = status.Hotswap.status;
            textBoxHotSwapStatus.ForeColor = status.Hotswap.color;

            string driveString = status.Drive.status.Replace("^^", ":");
            buttonDiskDrive.Text = driveString;
            buttonDiskDrive.BackColor = status.Drive.color;



            buttonLPRServiceStatusIndicator.Text = status.Service.status;
            buttonLPRServiceStatusIndicator.BackColor= status.Service.color;

          
            progressBarPlateProcessQueueLevel.Value = (100* status.LprPlateProcessQueCount.count )/ 240;
            progressBarPlateProcessQueueLevel.Invalidate(true);

            labelServiceVersion.Text = status.ServiceVersion.status;
        }

        class MyProgressBar : System.Windows.Forms.ProgressBar
        {
            public MyProgressBar()
            {
                 this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);

                SolidBrush b;

                Rectangle rect;

                Color[] useColor = { Color.Green, Color.Yellow, Color.Red };
                int[] valueRanges = { 70, 95, 100 };
                int[] rectStarts = new int[3];
                int[] rectEnds = new int[3];

                int totalWidthToDraw = (e.ClipRectangle.Width * Value) / 100;

                bool done = false;

                rectStarts[0] = 0;
                rectStarts[1] = (e.ClipRectangle.Width * valueRanges[0]) / 100;
                rectStarts[2] = (e.ClipRectangle.Width * valueRanges[1]) / 100;

                rectEnds[0] = (e.ClipRectangle.Width * valueRanges[0]) / 100; 
                rectEnds[1] = (e.ClipRectangle.Width * valueRanges[1]) / 100;
                rectEnds[2] = (e.ClipRectangle.Width * valueRanges[2]) / 100;


                for (int i = 0; i < 3; i++)
                {
                    if (rectEnds[i] > totalWidthToDraw)
                    {
                        rectEnds[i] = totalWidthToDraw;
                        done = true;
                    }

                    int rectWidth = rectEnds[i] - rectStarts[i];

                    rect = new Rectangle(rectStarts[i], 0, rectWidth, e.ClipRectangle.Height);

                    b = new SolidBrush(useColor[i]);

                    e.Graphics.FillRegion(b, new Region(rect));

                    if (done) break;
                }
            }
        }

        void OnNewStatusUpdateReceived(string heathReport)
        {

        }

        void OnRxStats(string status)
        {
            ParseAndDisplay(status);
        }

        void ParseAndDisplay(string statusInfo)
        {
            string[] sp1 = statusInfo.Split(',');

            foreach (string s in sp1)
            {
                if (s.Contains("System_") || s.Contains("LPR_") || s.Contains("LPRServiceVersion"))
                {
                    ParseStat(s);
                }
            }
           

            // now load the display table for this group.
            this.BeginInvoke((MethodInvoker)delegate { PostStats(m_SystemStatus); });

        }

     
        void ParseStat(string statStr)
        {
            //System_frameGrabberJpegCompressor = (int)600,
            //   System_frameGrabberBitmapCapture,
            //   System_GPS,
            //   System_Service,
            //   System_videoChannel1,
            //   System_videoChannel2,
            //   System_videoChannel3,
            //   System_videoChannel4,
            //   System_Drive,
            //   System_Hotswap,
            //   LPRServiceVersionString

            //  LPR_ProcessQCnt.Snapshot

            STATUS_STRING stat = new STATUS_STRING();

            string[] sp1 = statStr.Split(':');
            
            if (sp1.Length < 1) return;

            stat.status = sp1[1];

            if (sp1[0].Contains("LPR_ProcessQCnt.Snapshot"))
            {
                STATUS_COUNT statCount = new STATUS_COUNT();

                statCount.count = 0;
                try
                {
                    statCount.count = Convert.ToInt32(sp1[1]);
                }
                catch { }

                lock (m_SystemStatusLock) { m_SystemStatus.LprPlateProcessQueCount = statCount; }
            }
            else if (sp1[0].Contains("System_frameGrabber_2"))
            {
                stat.status = sp1[1];
                if (sp1[1].Contains("No"))
                {
                    stat.color = Color.Red;
                }
                else
                {
                    stat.color = Color.Green;
                }
                lock (m_SystemStatusLock) { m_SystemStatus.FrameGrabber_1 = stat; }
            }
            else if  (sp1[0].Contains("System_frameGrabber_1"))
            {
                stat.status = sp1[1];
                if (sp1[1].Contains("No"))
                {
                    stat.color = Color.Red;
                }
                else
                {
                    stat.color = Color.Green;
                }
                lock (m_SystemStatusLock) { m_SystemStatus.FrameGrabber_0 = stat; }

            }
            else if (sp1[0].Contains("System_GPS"))
            {
                stat.status = sp1[1];
                if (sp1[1].Contains("No GPS Device; No fixed position set") || sp1[1].Contains("Have GPS device; No Satellite Acquisition"))
                {
                    stat.color = Color.Red;
                }
                else
                {
                    stat.color = Color.Green;
                }
                lock (m_SystemStatusLock) { m_SystemStatus.GPS = stat; }

            }
            else if (sp1[0].Contains("System_Service"))
            {
                if ( ! m_ServiceRunning )
                {
                    stat.status = "Service Not Running";
                    stat.color = Color.Red;
                }
                else
                {
                    stat.status = "Service Running";
                    stat.color = Color.Green;
                }
                lock (m_SystemStatusLock) { m_SystemStatus.Service = stat; }

            }
            else if (sp1[0].Contains("System_videoChannel1"))
            {
                stat.status = sp1[1];
                if (sp1[1].Contains("No Video Connected"))
                {    
                    stat.color = Color.Red;
                }
                else
                {
                    stat.color = Color.Green;
                }
                lock (m_SystemStatusLock) { m_SystemStatus.VideoChannel1 = stat; }

            }
            else if (sp1[0].Contains("System_videoChannel2"))
            {
                stat.status = sp1[1];
                if (sp1[1].Contains("No Video Connected"))
                {
                    stat.color = Color.Red;
                }
                else
                {
                    stat.color = Color.Green;
                }
                lock (m_SystemStatusLock) { m_SystemStatus.VideoChannel2 = stat; }

            }
            else if (sp1[0].Contains("System_videoChannel3"))
            {
                stat.status = sp1[1];
                if (sp1[1].Contains("No Video Connected"))
                {
                    stat.color = Color.Red;
                }
                else
                {
                    stat.color = Color.Green;
                }
                lock (m_SystemStatusLock) { m_SystemStatus.VideoChannel3 = stat; }

            }
            else if (sp1[0].Contains("System_videoChannel4"))
            {
                stat.status = sp1[1];
                if (sp1[1].Contains("No Video Connected"))
                {
                    stat.color = Color.Red;
                }
                else
                {
                    stat.color = Color.Green;
                }
                lock (m_SystemStatusLock) { m_SystemStatus.VideoChannel4 = stat; }

            }
            else if (sp1[0].Contains("System_Drive"))
            {
                stat.status = sp1[1];
                if (sp1[1].Contains("No drive"))
                {
                    stat.color = Color.Red;
                }
                else
                {
                    stat.color = Color.Green;
                }
                lock (m_SystemStatusLock) { m_SystemStatus.Drive = stat; }

            }
            else if (sp1[0].Contains("System_Hotswap"))
            {
                stat.status = sp1[1];
                if (sp1[1].Contains("Starting HotSwap"))
                {
                    stat.color = Color.Red;
                }
                else
                {
                    stat.color = Color.Green;
                }
                lock (m_SystemStatusLock) { m_SystemStatus.Hotswap = stat; }
            }
            else if (sp1[0].Contains("LPRServiceVersionString"))
            {
                stat.status = sp1[1].Replace('^','.');
                
                lock (m_SystemStatusLock) { m_SystemStatus.ServiceVersion= stat; }
            }
        }

        private void panelVidDisplayPanel_Paint(object sender, PaintEventArgs e)
        {

        }

        class UNEDITEDIMAGES
        {
            public UNEDITEDIMAGES() { singleton = new object(); }
            object singleton;
            Bitmap bmp;
            public Bitmap Bmp
            { get { lock (singleton) { return bmp; } } set { lock (singleton) { bmp = new Bitmap( value); } } }
          
        }
        UNEDITEDIMAGES[] m_UneditedImages;


        private void buttonSaveCurrentImage_Click(object sender, EventArgs e)
        {
            if( m_UneditedImages[m_CurrentlySelectedPB].Bmp == null ) return;

            Bitmap imageToSave = new Bitmap(m_UneditedImages[m_CurrentlySelectedPB].Bmp); // grab it now, its always changing

            saveFileDialog1.Filter = "Image files |*.jpg;*.jpeg;*.bmp;|All Files|*.*";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {

                    imageToSave.Save(saveFileDialog1.FileName);

                }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }
        }

    }
}
