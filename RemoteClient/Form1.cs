using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RCS_Protocol;
using RCSClientLib;
using System.IO;
using System.Threading;
using ApplicationDataClass;
using ScreenLoggerLib;

namespace RemoteClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            m_AppData = new APPLICATION_DATA();
            m_AppData.LogToScreen = (object)new ScreenLogger();
            m_ViewImagesThread = new Thread(GetImages);
            m_TCPConnection = new RCSClientLib.RCSClient( m_AppData);
            m_AppData.tcpconnection = (object)m_TCPConnection;

            this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);
        }

        RCSClientLib.RCSClient m_TCPConnection;

        Thread m_CloseDownThread;
        bool m_ClosingApp = false;
        void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // notify all the theads to stop running, then wait one second and then really close this app. - prevents sub-apps from feeding to form objects which have been disposed.
            if (!m_ClosingApp)
            {
                e.Cancel = true;
                m_ClosingApp = true;
                m_CloseDownThread = new Thread(StopApplication);
                m_CloseDownThread.Start();
            }
        }

        void StopApplication()
        {
            m_AppData.CloseApplication();
            Thread.Sleep(1000);

            this.BeginInvoke((MethodInvoker)delegate { this.Close(); });
        }

        Thread m_ViewImagesThread;
      
        void GetImages()
        {
            while (! m_ClosingApp)
            {
               // if (m_NeedNewImage)
                {
                    m_TCPConnection.SendLiveViewRequest("channel 0", " ");
                   
                }
                Thread.Sleep(100);// 10 fps
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            m_AppData.tcpconnection = new RCSClientLib.RCSClient( m_AppData);

            m_TCPConnection.Connect("192.168.2.2", true, null);

            m_TCPConnection.SendLiveViewRequest("channel 0", " ");
            m_ViewImagesThread.Start();
            
        }

        public delegate void PrettyMuchUseless_handleRxJpeg_CBDelegate( byte[] jpeg);

        void handleRxJpeg(byte[] jpeg)
        {
            if (jpeg.Length < 1000)
            {
             
                return;// the first unitilized image is not realy a valid image
            }

            if (pictureBox1.InvokeRequired)
            {
                this.Invoke(new PrettyMuchUseless_handleRxJpeg_CBDelegate(handleRxJpeg), new object[] { jpeg });
            }
            else
            {
                Image img = Bitmap.FromStream(new MemoryStream(jpeg));
                jpeg = null;
                GC.Collect();

                pictureBox1.Image = img;
               
            }
        }

        public delegate void _LogToScreen(string text);
        void LogToScreen(string text)
        {
            //if (m_ClosingApp) return;

            //if (listBox1.InvokeRequired)
            //{
            //    this.Invoke(new _LogToScreen(LogToScreen), new object[] { text });
            //}
            //else
            //{
            //    listBox1.Items.Add(text);

            //    if (listBox1.Items.Count > 100) listBox1.Items.RemoveAt(0);

            //    listBox1.SelectedIndex = listBox1.Items.Count - 1;
            //}
        }
       

        APPLICATION_DATA m_AppData;
    }
}
