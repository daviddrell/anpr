using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ApplicationDataClass;
using System.Threading;

namespace VideoPlayerForm
{
    public partial class VideoPlayerMainForm : Form
    {
        public VideoPlayerMainForm()
        {
            InitializeComponent();
            this.FormClosing += new FormClosingEventHandler(VideoPlayerMainForm_FormClosing);
        }

        void Stop() // if the appData or parent control says stop, then close this form
        {
            this.Close();
        }

        void VideoPlayerMainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!this.IsHandleCreated) return;

            m_UserClosedThePlayer();

            // if this form is closing, stop the video player first.

            videoPlayer1.Stop();

            videoPlayer1.BlockOnThreadStop(2000);
        }

        public delegate void UserClosedThePlayerDelegate();// used to signal back up to caller that the user clicked the X, make it a reference value
        UserClosedThePlayerDelegate m_UserClosedThePlayer;

        public VideoPlayerMainForm(APPLICATION_DATA appData, UserClosedThePlayerDelegate closedCallBack)
        {
            InitializeComponent();
            m_AppData = appData;
            videoPlayer1.AppData = m_AppData;

            m_UserClosedThePlayer = closedCallBack;
            
            this.FormClosing += new FormClosingEventHandler(VideoPlayerMainForm_FormClosing);
            m_AppData.AddOnClosing(Stop,APPLICATION_DATA.CLOSE_ORDER.LAST); 
        }

        APPLICATION_DATA m_AppData;

        public void LoadVideo(string PSS, string source, DateTime start, DateTime stop)
        {
            videoPlayer1.LoadVideClip(PSS, source, start, stop);
        }

        private void videoPlayer1_Load(object sender, EventArgs e)
        {

        }
    }
}
