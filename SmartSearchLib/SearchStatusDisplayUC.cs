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

namespace SmartSearchLib
{
    public partial class SearchStatusDisplayUC : UserControl
    {
        // design
       
        /*
         *  show user search status
         *      show search phase - text
         *          scanning files - show last date scanned
         *          comparing search strings - show percent complete
         *          loading search results into search table - show percent complete
         *      show search count of total count - text & progress bar
         *      show search complete - text and flashing bar
         *      
         *  allow user to cancel search - button
         * 
         *  
         * 
         * */


        /// <summary>
        /// Non-default Constructor - needed to pass in AppData reference, used for thread stopping.
        /// </summary>
        /// <param name="appData"></param>
        public SearchStatusDisplayUC(APPLICATION_DATA appData, UserCanceledSearchCB cancelCB)
        {
            InitializeComponent();
            m_AppData = appData;
            m_AppData.AddOnClosing(Stop, APPLICATION_DATA.CLOSE_ORDER.MIDDLE);
            m_UserCanceledSearchEvent = cancelCB;

        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public SearchStatusDisplayUC()
        {
            InitializeComponent();
            m_AppData.AddOnClosing(Stop, APPLICATION_DATA.CLOSE_ORDER.MIDDLE);

        }

        public delegate void UserCanceledSearchCB();
        UserCanceledSearchCB m_UserCanceledSearchEvent;

        /// <summary>
        /// Use the SEARCH_STATUS class to pass in status Data. If totalCount is zero, then progress will be indicated
        /// by currentTime as referenced to startTime/endTime. If totalCount is non-zero, then progress will be indicated
        /// by currentCount as a percentage of totalCount.
        /// In either case, currentTime will be indicated to the user as the current search point.
        /// </summary>
        /// <param name="status"></param>
        public void SetStatus(SearchLib.SEARCH_STATUS status)
        {
            if ( status.totalCount == 0 )
                this.BeginInvoke((MethodInvoker)delegate { _SetStatus(status, status.currentTime, 0, 0, status.startTime, status.endTime, status.errorString); });
            else
                this.BeginInvoke((MethodInvoker)delegate { _SetStatus(status, status.currentTime, status.currentCount, status.totalCount, default(DateTime), default(DateTime), status.errorString); });

        }

        /// <summary>
        /// Clear out last results, including error indicators
        /// </summary>

        public void ClearResults()
        {
            this.BeginInvoke((MethodInvoker)delegate { _ClearResults(); });
        }

        public void _ClearResults()
        {
            labelSearchPhase.Text = " ";
            labelCurrentlyAt.Text = " ";
            textBoxErrorText.Text = " ";
            m_lastError = "None";
            SetProgressIndicator(0,0);
        }

        /// <summary>
        /// Set Current Status
        /// </summary>
        /// 
        void _SetStatus(SearchLib.SEARCH_STATUS status, DateTime currentSearchTime, int count, int totalCount, DateTime startTime, DateTime endTime, string errors)
        {
            switch (status.phase)
            {
                case SearchLib.SEARCH_PHASE.COMPARING_STRINGS:
                    labelSearchPhase.Text = "Searching Plate Strings";
                    labelCurrentlyAt.Text = currentSearchTime.ToString(m_AppData.TimeFormatStringForDisplay);
                    textBoxErrorText.Text = (errors == null) ? m_lastError : errors;
                    SetProgressIndicator(count, totalCount);
                    break;


                case SearchLib.SEARCH_PHASE.FINDING_ITEMS_IN_TIME_RANGE:
                    labelSearchPhase.Text = "Finding Items In Time Range";
                    labelCurrentlyAt.Text = currentSearchTime.ToString(m_AppData.TimeFormatStringForDisplay);
                    textBoxErrorText.Text = (errors == null) ? m_lastError : errors;

                    SetProgressIndicator(currentSearchTime, startTime, endTime);

                    break;

                case SearchLib.SEARCH_PHASE.COMPLETE:
                    labelSearchPhase.Text = "Search Complete";
                    labelCurrentlyAt.Text = currentSearchTime.ToString(m_AppData.TimeFormatStringForDisplay);
                    textBoxErrorText.Text = (errors == null) ? m_lastError : errors;

                    if ( totalCount == 0 )
                        SetProgressIndicator(currentSearchTime, startTime, endTime);
                    else
                        SetProgressIndicator(count, totalCount);

                    ShowSearchCompleteIndicator();
                    break;

                case SearchLib.SEARCH_PHASE.LOADING_RESULTS:
                    labelSearchPhase.Text = "Loading Results";
                    labelCurrentlyAt.Text = currentSearchTime.ToString(m_AppData.TimeFormatStringForDisplay);
                    textBoxErrorText.Text = (errors == null) ? m_lastError : errors;
                    SetProgressIndicator(count, totalCount);
                    break;
            }
            if (errors != null) m_lastError = errors;

        }

        string m_lastError = "None";


        APPLICATION_DATA m_AppData;
        Thread m_SearchCompleteIndicator;

        bool m_Stop = false;
        void Stop()
        {
            m_Stop = true;

            int count = 10;
            while (count-- > 0 && !m_BlinkIndicatorLoopStopped) Thread.Sleep(250);
        }


        void SetProgressIndicator(DateTime currentTime, DateTime startTime, DateTime endTime)
        {
            int value;

            double currentSeconds = (double)currentTime.Ticks * 10000000.0;
            double startTimeSeconds = (double)startTime.Ticks * 10000000.0;
            double endTimeSeconds = (double)endTime.Ticks * 10000000.0;
            double totalSeconds = (double)endTimeSeconds - startTimeSeconds;
            double currentProgress = (double)currentSeconds - startTimeSeconds;

            if (currentProgress < 0) currentProgress = 0;
            if (currentProgress >= endTimeSeconds) currentProgress = endTimeSeconds;

            if (totalSeconds <= 0)
            {
                value = 0;
            }
            else
            {
                value = (int) (100.0 * (currentProgress) / (totalSeconds));
                if (value > 100) value = 100;
            }
            progressBar1.Value = value;
        }


        void SetProgressIndicator(int currentCount, int totalFileCount)
        {
            int value;

            if (totalFileCount <= 0)
            {
                value = 0;
            }
            else
            {
                value = (100 * currentCount) / totalFileCount;
                if (value > 100) value = 100;
            }
            progressBar1.Value = value;
        }

        void ShowSearchCompleteIndicator()
        {
            m_BlinkIndicatorLoopStopped = false;

            m_SearchCompleteIndicator = new Thread(BlinkIndicator);
            m_SearchCompleteIndicator.Start();
        }

        bool m_BlinkIndicatorLoopStopped = true;

        void BlinkIndicator()
        {
            int i = 0;
            Color c;

            while (i < 9 && ! m_Stop)
            {

                c = Color.Blue;

                SetButton(c, "Search Complete", false);
                Thread.Sleep(300);

                if (m_Stop) break;

                c = Color.White;
                SetButton(c, "Search Complete", false);
                Thread.Sleep(300);

                i++;

            }

            c = Color.Black;
            SetButton(c, " ", true);

            m_BlinkIndicatorLoopStopped = true;

        }



        private delegate void PrettyMuchUselessSetButtonCBDelegate(Color c, string text, bool turnOffProgressBar);
        void SetButton(Color c, string text, bool turnOffProgressBar)
        {
            if (m_Stop) return;

            if (this.InvokeRequired)
            {
                this.Invoke(new PrettyMuchUselessSetButtonCBDelegate(SetButton), new object[] { c, text, turnOffProgressBar });
            }
            else
            {
                buttonSearchCompleteIndicator.Text = text;
                buttonSearchCompleteIndicator.BackColor = c;

                if (turnOffProgressBar)
                {
                    progressBar1.Value = 0;
                    progressBar1.Enabled = false;
                }
            }

        }

        private void labelCount_Click(object sender, EventArgs e)
        {

        }

        private void buttonSearchCompleteIndicator_Click(object sender, EventArgs e)
        {

        }

        private void buttonCancelSearch_Click(object sender, EventArgs e)
        {
            m_UserCanceledSearchEvent();
        }

    }
}
