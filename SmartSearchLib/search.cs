using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LPROCR_Wrapper;
using System.Drawing.Imaging;
using System.Threading;
using UserSettingsLib;
using System.Diagnostics;
using System.Deployment;
using ErrorLoggingLib;
using ApplicationDataClass;
using PathsLib;
using ScalablePictureBoxTool;
using VideoPlayerForm;

namespace SmartSearchLib
{

    public partial class SmartSearchLibUC : UserControl
    {

        SearchLib m_SearchTool;
        String m_timeFormat = "dd.MMM.yyyy : HH.mm.ss";
                                   
        int m_CurrentRowIndex = 0;
        bool m_AddingRowsToDataGrid = false;

        Bitmap m_LittleRedFlag = SmartSearchLib.Properties.Resources.littleredflag;
        Bitmap m_LittleGreenFlag = SmartSearchLib.Properties.Resources.littlegreenflag;
        Bitmap m_NullImage = SmartSearchLib.Properties.Resources.nullimage;
        ErrorLog m_Log;
        APPLICATION_DATA m_AppData;
        PATHS m_PathManager;
      
        public delegate void ABORT_CLOSE ();
        ABORT_CLOSE GiveUp;

        public SmartSearchLibUC(APPLICATION_DATA appData, ABORT_CLOSE giveup)
        {
            InitializeComponent();

            m_AppData = appData;
            m_AppData.AddOnClosing(OnClose,APPLICATION_DATA.CLOSE_ORDER.FIRST);
            m_Log = (ErrorLog)m_AppData.Logger;
            GiveUp = giveup;

            m_PathManager= (PATHS) m_AppData.PathManager ;
          

            textBoxMinMatch.Text = "0";

            m_SearchTool = new SearchLib(m_AppData);
          
            this.Text = "Smart Search";

            m_ZoomTrackLock = new object();
            trackBarZoomControl.Minimum = 0;
            trackBarZoomControl.Maximum = 1000;

            buttonUndoDelete.Enabled = false;
            dataGridView1.KeyDown += new KeyEventHandler(dataGridView1_KeyDown);

            dataGridView1.CellClick +=new DataGridViewCellEventHandler(dataGridView1_CellContentClick); // mouse click on cell
            dataGridView1.SelectionChanged += new EventHandler(dataGridView1_SelectionChanged);  // support cell selection changes by keyboard (i.e. not by mouse click)


            dataGridView1.Columns["time"].DefaultCellStyle.Format = m_AppData.TimeFormatStringForDisplay;// "dd MMM yyyy,  HH: mm: ss: ffff";


            m_SearchStatusDisplay = new SearchStatusDisplayUC(m_AppData, UserCanceledSearch);
            m_SearchStatusDisplay.Location = new Point(594, 55);
            this.Controls.Add(m_SearchStatusDisplay);
           
        }


        // support cell selection changes by keyboard (i.e. not by mouse click)

        void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (m_AddingRowsToDataGrid) return;// adding a row changes the selection, firing this event. Ignore this while loading the table
            if (dataGridView1.SelectedCells.Count ==  0 ) return;

            int row = dataGridView1.SelectedCells[0].RowIndex;
            int col = dataGridView1.SelectedCells[0].ColumnIndex;

            DataGridViewCellEventArgs newEventArg = new DataGridViewCellEventArgs( col, row);

            dataGridView1_CellContentClick(sender, newEventArg);
          
        }


        SearchStatusDisplayUC m_SearchStatusDisplay;

        bool m_RepositoryEnabled = false;

        void UserCanceledSearch()
        {
            m_SearchTool.StopSearch();
        }

        public void SetCentralRepository(bool enabled, string drive)
        {
            m_RepositoryEnabled = enabled;
            if (enabled)
            {
                this.BeginInvoke((MethodInvoker)delegate { labelRepository.Text = drive; ; });
            }
        }


        void SetGPSToANull(int rowIndex)
        {
            DataGridViewImageCell cell = (DataGridViewImageCell)dataGridView1.Rows[rowIndex].Cells[5];

            cell.Value = m_NullImage;
        }

        void SetGPSToAvailable(int rowIndex)
        {
            DataGridViewImageCell cell = (DataGridViewImageCell)dataGridView1.Rows[rowIndex].Cells[5];

            cell.Value = m_LittleGreenFlag;
        }

        void SetGPSToNotAvailable(int rowIndex)
        {
            DataGridViewImageCell cell = (DataGridViewImageCell)dataGridView1.Rows[rowIndex].Cells[5];

            cell.Value = m_LittleRedFlag;
        }

        bool m_KillThreads = false;
        bool m_BlinkStatusThreadStopped = true;
        public bool ThreadsStopped = false;

        private void OnClose( )
        {
            m_KillThreads = true;


            while (!m_BlinkStatusThreadStopped)
                Thread.Sleep(1000);

            if ( m_Plotter != null)
                m_Plotter.Close();

            if (m_SearchTool != null)
            {
                m_SearchTool.StopSearch();
            }

            ThreadsStopped = true;

        }

        void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (dataGridView1.CurrentRow == null) return;

            int delChar = (int)Keys.Delete;
            if (e.KeyValue == delChar)
            {
                DeleteGridRow(dataGridView1.CurrentRow.Index);
            }
        }

        void DeleteGridRow(int index)
        {
            m_LastDeletedRow = dataGridView1.CurrentRow;
            m_LastDeletedRowIndex = dataGridView1.CurrentRow.Index;
            dataGridView1.Rows.RemoveAt(dataGridView1.CurrentRow.Index);

           
            buttonUndoDelete.Enabled = true;

            // change the picture to the new current row
            DataGridViewCellEventArgs e = new DataGridViewCellEventArgs(0, dataGridView1.CurrentRow.Index);
            dataGridView1_CellContentClick(this, e);
        }

        DataGridViewRow m_LastDeletedRow=null;
        int m_LastDeletedRowIndex = -1;

        void UnDeleteGridRow()
        {
            if (m_LastDeletedRow != null)
            {

                dataGridView1.Rows.Insert(m_LastDeletedRowIndex, m_LastDeletedRow);
                m_LastDeletedRow = null;
                buttonUndoDelete.Enabled = false;
            }
        }

        private void buttonUndoDelete_Click(object sender, EventArgs e)
        {
            UnDeleteGridRow();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!m_RepositoryEnabled)
            {
                MessageBox.Show("No repository connected, cannot search");
                return;
            }
            
            if (e.RowIndex < 0) return;

            m_CurrentRowIndex = e.RowIndex;

            try
            {
                string relativeFileName = dataGridView1.Rows[e.RowIndex].Cells["imagePath"].Value.ToString();

                // because of an bug (or not understoon reason), not all pictures are showing up in storage.
                // the work-around is to find a picture nearest the target picture using time stamps.

                PATHS.IMAGE_FILE_DATA fd = m_PathManager.ParseFileCompleteJpegPath(m_PathManager.GetCompleteFilePath(relativeFileName));

                if (fd == null) return;// corrupted file data (relativeFileName is missing or not correctly formed)

                if (!File.Exists(fd.completePath))
                {
                    TimeSpan ts = new TimeSpan(0, 0, 0, 1, 500);
                    DateTime before = fd.timeStamp.Subtract(ts);
                    DateTime after = fd.timeStamp.Add(ts);
                    string[] jpegsAroundTarget = m_PathManager.GetAllJpegsInRange(before, after);
                    if (jpegsAroundTarget.Count() < 1)
                    {
                        MessageBox.Show("No images found near the target time");
                    }
                    else
                    {

                        string filePath = jpegsAroundTarget[jpegsAroundTarget.Count() / 2];
                        //                string filePath = m_PathManager.GetCompleteFilePath(relativeFileName);
                        {
                            trackBarZoomControl.Value = 400;
                            Image img = Image.FromFile(filePath);
                            m_ZoomPictureControl.Picture = img;

                            PushVideoRangeToVideoPlayer();
                        }
                    }
                }
                else
                {
                    trackBarZoomControl.Value = 400;
                    Image img = Image.FromFile(fd.completePath);
                    m_ZoomPictureControl.Picture = img;

                    PushVideoRangeToVideoPlayer();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("file IO exception: ex " + ex.Message);
            }

            GC.Collect(); // when scrolling down the table, can get out of memory exceptions
        }


        private void plateDisplayPB_Click(object sender, EventArgs e)
        {

        }


        private void searchStringTextBox_TextChanged(object sender, EventArgs e)
        {
            searchStringTextBox.Text = searchStringTextBox.Text.ToUpper();
            searchStringTextBox.SelectionStart = searchStringTextBox.Text.Length;
        }


        int m_MinMatchThreshold;
        bool m_SearchByNumber = false;

        void searchForPlate(string searchNumber, int minMatch, string cameraNameFilter)
        {
            
            m_SearchCount++;
         
            DateTime searchWindowStart = dateTimePicker1.Value;
            DateTime searchWindowEnd = dateTimePicker2.Value;

            m_MinMatchThreshold = minMatch;

            // clear out the previous search results

            dataGridView1.Rows.Clear();
           

            searchNumber = searchNumber.ToUpper();

            m_SearchByNumber = true;

            if (cameraNameFilter == null) cameraNameFilter = "";

            m_SearchTool.SearchForPlate(SearchLib.SEARCH_TYPE.PLATE, searchNumber, cameraNameFilter, searchWindowStart, searchWindowEnd, minMatch, SearchProgressCB, SearchCompleteCB);


        }

     
        List<SearchLib.SEARCH_RESULT> m_LastSearchResults;

        void SearchCompleteCB(List<SearchLib.SEARCH_RESULT> results)
        {
            BeginInvoke((MethodInvoker)delegate { _SearchCompleteCB( results); });
        }

       
        void _SearchCompleteCB(List<SearchLib.SEARCH_RESULT> results)
        {
            SearchLib.SEARCH_STATUS status = new SearchLib.SEARCH_STATUS();
            status.phase = SearchLib.SEARCH_PHASE.LOADING_RESULTS;
            status.totalCount = (results == null  ) ? 0 : results.Count;
            status.currentCount = 0;

            m_SearchStatusDisplay.SetStatus(status);

            if (results == null)
            {
                // there was an error or no data found
                status.phase = SearchLib.SEARCH_PHASE.COMPLETE;
                status.totalCount = 0;
                status.currentCount = 0;
                m_SearchStatusDisplay.SetStatus(status);

                m_SearchInProgress = false;
                return;
            }

            // do work here

            m_LastSearchResults = results;

            numItemsFoundLabel.Text = results.Count.ToString();

            int loopCount = 0;

            m_AddingRowsToDataGrid = true;

            foreach (SearchLib.SEARCH_RESULT r in results)
            {
                loopCount++;

                Bitmap flag = m_NullImage;
                string GPSLocation = r.plateData.GPSLatitude + "," + r.plateData.GPSLongitude;

                string gpsTest = GPSLocation.ToLower();

                if (gpsTest.Contains("no position"))
                {
                    flag = m_LittleRedFlag;
                }
                else
                {
                    flag = m_LittleGreenFlag;
                }

                dataGridView1.Rows.Add(r.percentMatch, r.matchedString, r.plateData.timeStamp, r.plateData.PSSName,
                                r.plateData.sourceChannelName, r.plateData.jpegRelativeFilePath, flag, GPSLocation);

                Application.DoEvents();// process windows thread messages in this very long loop

                if (loopCount % 100 == 0)
                {

                    status.currentCount = loopCount;
                    status.currentTime = r.plateData.timeStamp;
                    m_SearchStatusDisplay.SetStatus(status);
                }
            }

            if (m_SearchByNumber)
                dataGridView1.Sort(dataGridView1.Columns["SearchMatchLikelyhood"], ListSortDirection.Descending);
            else
                dataGridView1.Sort(new RowComparer(SortOrder.Ascending));


            status.phase = SearchLib.SEARCH_PHASE.COMPLETE;

            status.totalCount = results.Count;
            status.currentCount = results.Count;
            m_SearchStatusDisplay.SetStatus(status);

            m_SearchInProgress = false;

            m_AddingRowsToDataGrid = false;
        }


        private class RowComparer : System.Collections.IComparer
        {
            private static int sortOrderModifier = 1;

            public RowComparer(SortOrder sortOrder)
            {
                if (sortOrder == SortOrder.Descending)
                {
                    sortOrderModifier = -1;
                }
                else if (sortOrder == SortOrder.Ascending)
                {
                    sortOrderModifier = 1;
                }
            }

            public int Compare(object x, object y)
            {
                DataGridViewRow DataGridViewRow1 = (DataGridViewRow)x;
                DataGridViewRow DataGridViewRow2 = (DataGridViewRow)y;

                DateTime t1 = Convert.ToDateTime(DataGridViewRow1.Cells[2].Value);
                DateTime t2 = Convert.ToDateTime(DataGridViewRow2.Cells[2].Value);

                int CompareResult = t1.CompareTo(t2);

                return CompareResult * sortOrderModifier;
            }
        }


        

        void SearchProgressCB(SearchLib.SEARCH_STATUS status)
        {

            try
            {
                // just send this to the display
                m_SearchStatusDisplay.SetStatus(status);

                switch (status.errorCode)
                {

                    case SearchLib.SEARCH_ERROR_CODES.TOO_MANY_ENTRIES:
                        m_SearchTool.StopSearch();

                        break;
                }
            }
            catch (Exception ex)
            {
                m_Log.Trace(ex, ErrorLog.LOG_TYPE.INFORMATIONAL);
            }
              
        }

       
      
        int m_SearchCount = 0;

        private void search_Load(object sender, EventArgs e)
        {
            m_SearchStatusDisplay.ClearResults();

            timer1.Interval = 1000;
            timer1.Start();


            this.Text = "First Evidence Smart Search";


            dateTimePicker1.Format = DateTimePickerFormat.Custom;
           
            dateTimePicker1.CustomFormat = m_timeFormat;

            dateTimePicker1.CalendarFont = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dateTimePicker1.Size = new System.Drawing.Size(200, 40);

            dateTimePicker2.Format = DateTimePickerFormat.Custom;
            dateTimePicker2.CustomFormat = m_timeFormat;

            dateTimePicker2.CalendarFont = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dateTimePicker2.Size = new System.Drawing.Size(200, 40);


            DateTime timeNow = DateTime.UtcNow;

            dateTimePicker2.Value = timeNow;  // end time starts as now

            TimeSpan initialSearchWindow = new TimeSpan(0, 0, 5, 0); // start time is five minutes ago

            timeNow = timeNow.Subtract(initialSearchWindow);

            dateTimePicker1.Value = timeNow;

            dataGridView1.AllowUserToAddRows = false;

            dataGridView1.RowHeadersVisible = false;
            dataGridView1.ColumnHeadersBorderStyle = ProperColumnHeadersBorderStyle;



            dataGridView1.RowHeadersVisible = false;
            dataGridView1.ColumnHeadersBorderStyle = ProperColumnHeadersBorderStyle;
            dataGridView1.Columns["cameraName"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            dataGridView1.Columns["time"].SortMode = DataGridViewColumnSortMode.Programmatic;

            DataGridViewImageColumn imageColumn = new DataGridViewImageColumn();
            imageColumn.Width = 31;
            imageColumn.Image = m_NullImage;
            imageColumn.HeaderText = "GPS";
            dataGridView1.Columns.Insert(6, imageColumn);

            dataGridView1.Columns[5].Width -= 31;
        }

        /// <summary>
        /// Remove the column header border in the Aero theme in Vista,
        /// but keep it for other themes such as standard and classic.
        /// The border looks messy in Aero but looks good in the other
        /// themes.
        /// </summary>
        public static DataGridViewHeaderBorderStyle ProperColumnHeadersBorderStyle
        {
            get
            {
                return (SystemFonts.MessageBoxFont.Name == "Segoe UI") ?
                    DataGridViewHeaderBorderStyle.None :
                    DataGridViewHeaderBorderStyle.Raised;
            }
        }


        bool m_SearchInProgress = false;

        private void doSearchButton_Click(object sender, EventArgs e)
        {
            if (m_SearchInProgress) return;

            if (!m_RepositoryEnabled)
            {
                MessageBox.Show("No repository connected, cannot search");
                return;
            }

            m_SearchInProgress = true;
            m_SearchStatusDisplay.ClearResults();

            int minMatch = 0;

            try
            {
                minMatch = Convert.ToInt32(textBoxMinMatch.Text);
            }
            catch
            {
                MessageBox.Show("Invalid Minimum match range. Must be between 0 and 100");
                m_SearchInProgress = false;
                return;
            }

            if (minMatch < 0 || minMatch > 100)
            {
                MessageBox.Show("Invalid  Minimum match range. Must be between 0 and 100");
                m_SearchInProgress = false;
                return;
            }

            searchForPlate(searchStringTextBox.Text, minMatch, textBoxCameraNameFilter.Text);
        }

        private void numItemsFoundLabel_Click(object sender, EventArgs e)
        {

        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {

        }


      

        private void timer1_Tick(object sender, EventArgs e)
        {

            currentTimeLabel.Text = DateTime.UtcNow.ToString(m_timeFormat);
            labelTimeZoneName.Text = TimeZone.CurrentTimeZone.StandardName;
            labelLocalTime.Text = DateTime.Now.ToString(m_timeFormat);
        }



        private void currentTimeLabel_Click(object sender, EventArgs e)
        {

        }


        bool m_ExportInProgress = false;


        private void buttonExportTable_Click(object sender, EventArgs e)
        {

            if (m_ExportInProgress) return;

            m_ExportInProgress = true;

            string saveFilePathAndFileName;

            // an empty table usually has one row, the "newrow".

            if (dataGridView1.Rows.Count < 2)
            {
                if (dataGridView1.Rows.Count == 0)
                {
                    MessageBox.Show("table is empty");
                    return;
                }

                if (dataGridView1.Rows[0].IsNewRow)
                {
                    MessageBox.Show("table is empty");
                    return;
                }
            }

            saveFileDialog1.Filter = "Text files (*.txt)|*.txt";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                saveFilePathAndFileName = saveFileDialog1.FileName;

                FileInfo fi = new FileInfo(saveFilePathAndFileName);

                m_ExportDirectory = fi.DirectoryName+"\\IMAGES" ;
                try
                {

                    if (!Directory.Exists(m_ExportDirectory))
                    {
                        Directory.CreateDirectory(m_ExportDirectory);
                    }
                }
                catch (Exception ex)
                {
                    m_Log.Log("exportButton ex: " + ex.Message,ErrorLog.LOG_TYPE.FATAL);
                   
                }

                extractDataGridContents(saveFilePathAndFileName);

               
            }

        }


        string m_ExportDirectory;

  

        /// <summary>
        /// Returns an array of strings containing the full paths for all jpegs images in the current search results table
        /// </summary>
        /// <returns></returns>
        public string[] GetAllFilePathsInResultsTable()
        {

            List<string> exportList = new List<string>();

            for (int i = 0; i < dataGridView1.Rows.Count ; i++)
            {
                //  grid row:     SearchMatchLikelyhood, ScannedNumber, time, cameraName,imagePath

                string relativeFileName = dataGridView1.Rows[i].Cells["imagePath"].Value.ToString();

                string filePath = m_PathManager.GetCompleteFilePath(relativeFileName);

                exportList.Add(filePath);
            }

            return (exportList.ToArray());
        }



        struct EXPORT_DATA
        {
            public string line;
            public string filepath;
            public bool GPSOnly;
        }

        List<EXPORT_DATA> m_ExportList;
        Thread m_FileWriteThread;
        string m_exportFileName;

        void extractDataGridContents(string file)
        {
            int i;
            string searchMatch;
            string scannedNumber;
            string time;
            string cameraName;
            string imagePath;
            string line;

            m_exportFileName = file;

            try
            {
                m_ExportList = new List<EXPORT_DATA>();


                for (i = dataGridView1.Rows.Count - 1; i >= 0; i--)
                {
                    //  grid row:     SearchMatchLikelyhood, ScannedNumber, time, cameraName,imagePath

                    searchMatch = dataGridView1.Rows[i].Cells["SearchMatchLikelyhood"].Value.ToString();
                    scannedNumber = dataGridView1.Rows[i].Cells["ScannedNumber"].Value.ToString();
                    time = dataGridView1.Rows[i].Cells["time"].Value.ToString();
                    
                    if ( dataGridView1.Rows[i].Cells["cameraName"].Value == null )
                        cameraName = " ";
                    else
                        cameraName = dataGridView1.Rows[i].Cells["cameraName"].Value.ToString();
    
                 
                    imagePath = m_PathManager.GetCompleteFilePath(dataGridView1.Rows[i].Cells["imagePath"].Value.ToString());

                    line = searchMatch + ", " +
                           scannedNumber + ", " +
                           time + ", " +
                           cameraName + ", " +
                           imagePath;

                   // writer.WriteLine(line);
                    EXPORT_DATA item = new EXPORT_DATA();
                    item.line = line;
                    item.filepath = imagePath;
                    m_ExportList.Add(item);
                }

              
            }
            catch (Exception ex)
            {
                m_Log.Log("extractDataGridContents ex :" + ex.Message,ErrorLog.LOG_TYPE.FATAL);
                m_ExportInProgress = false;
            }

            m_ExportStatusForm = new ExportStatus();

            m_ExportStatusForm.Show();

            m_FileWriteThread = new Thread(listWriteLoop);
            m_FileWriteThread.Start();


        }



        void extractDataGridGPSLocation(string fileName)
        {
            int i;
       
            string line;

            m_exportFileName = fileName;

            try
            {
                m_ExportList = new List<EXPORT_DATA>();


                for (i = dataGridView1.Rows.Count - 1; i >= 0; i--)
                {
                    line = dataGridView1.Rows[i].Cells["GPSLocation"].Value.ToString();

                    if (line.Length < 5) continue; // its empty

                    // writer.WriteLine(line);
                    EXPORT_DATA item = new EXPORT_DATA();
                    item.line = line;
                    item.GPSOnly = true;
                    m_ExportList.Add(item);
                }


            }
            catch (Exception ex)
            {
                m_Log.Log("extractDataGridGPSLocation ex :" + ex.Message,ErrorLog.LOG_TYPE.FATAL);
                m_ExportInProgress = false;
            }

            m_ExportStatusForm = new ExportStatus();

            m_ExportStatusForm.Show();

            m_FileWriteThread = new Thread(listWriteLoop);
            m_FileWriteThread.Start();


        }

        ExportStatus m_ExportStatusForm;

        void listWriteLoop()
        {

           

            int count = 0;

            try
            {
                StreamWriter writer = new StreamWriter(m_exportFileName);

                // first line in file
                bool GPSData = m_ExportList[0].GPSOnly;

                if (!GPSData)
                    writer.WriteLine("match likelyhood, scanned number, time, camera name, image path");

                int totalCount = m_ExportList.Count();

                lock (m_ExportList)
                {
                    foreach (EXPORT_DATA item in m_ExportList)
                    {
                        writer.WriteLine(item.line);

                        if (!GPSData)  // copy the image file over to the export directory
                        {
                            FileInfo fi = new FileInfo(item.filepath);

                            string fileNameOnly = fi.Name;

                            string newPathName = m_ExportDirectory + "\\" + fileNameOnly;

                            File.Copy(item.filepath, newPathName, true);
                        }

                        count++;

                        if (count % 100 == 0  || count == totalCount) m_ExportStatusForm.SetProgress(count, totalCount);

                        if (m_KillThreads) break;
                    }
                    writer.Close();
                    if (m_ExportStatusForm.Visible ) m_ExportStatusForm.Close();
                    m_ExportList = null;
                }
            }
            catch (Exception ex)
            {
//                MessageBox.Show("file IO exception writing file " + m_exportFileName + "\n\r operation failed \n\r" + ex.Message);
                m_Log.Trace(ex, ErrorLog.LOG_TYPE.INFORMATIONAL);
                //m_ExportInProgress = false;
            }

            m_ExportInProgress = false;
        }
       

        private void buttonCancelSearch_Click(object sender, EventArgs e)
        {
            m_SearchInProgress = false;
            m_SearchTool.StopSearch();
        }

      
        private void buttonExportImage_Click(object sender, EventArgs e)
        {
            if (!m_RepositoryEnabled)
            {
                MessageBox.Show("No repository connected, cannot search");
                return;

            }

            if (m_ExportInProgress) return;
            m_ExportInProgress = true;

           

            saveFileDialog1.Filter = "JPEG files (*.jpg)|*.jpg";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
               
                string relativeFileName = dataGridView1.Rows[m_CurrentRowIndex].Cells["imagePath"].Value.ToString();
                string filePath = m_PathManager.GetCompleteFilePath(relativeFileName);
                Image img = Image.FromFile(filePath);
                img.Save(saveFileDialog1.FileName);
            }

            m_ExportInProgress = false;
        }

        WebPlotLocation m_Plotter;

        private void buttonPlotLocation_Click(object sender, EventArgs e)
        {

            string GPSLocation = dataGridView1.Rows[m_CurrentRowIndex].Cells["GPSLocation"].Value.ToString();

            if (GPSLocation.Length < 5)
            {
                MessageBox.Show("No location found");
                return;
            }

            string gpsTest = GPSLocation.ToLower();

            if (gpsTest.Contains("unknown"))
            {
                MessageBox.Show("No location found");
                return;
            }

            gpsTest = GPSLocation.ToLower();
            if (gpsTest.Contains("no position") )
            {
                MessageBox.Show("No location found");
                return;
            }
          
            string mapURL = UserSettings.Get(UserSettingTags.GPSMAPURL);
            if (mapURL == null)
                mapURL = "http://maps.google.com/maps?q=";

            mapURL = mapURL + GPSLocation;

            if (m_Plotter == null)
            {
                m_Plotter = new WebPlotLocation(mapURL);
                m_Plotter.Show();
            }
            else if (!m_Plotter.Visible)
            {
                m_Plotter = null;
                m_Plotter = new WebPlotLocation(mapURL);
                m_Plotter.Show();
            }
            else
            {
                m_Plotter.SetNewURL(mapURL);
            }

        }

        private void buttonExportGPSLocationList_Click(object sender, EventArgs e)
        {

            if (m_ExportInProgress) return;

            m_ExportInProgress = true;

            string saveFilePathAndFileName;

            // an empty table usually has one row, the "newrow".

            if (dataGridView1.Rows.Count < 2)
            {
                if (dataGridView1.Rows.Count == 0)
                {
                    MessageBox.Show("table is empty");
                    return;
                }

                if (dataGridView1.Rows[0].IsNewRow)
                {
                    MessageBox.Show("table is empty");
                    return;
                }
            }

            string plateName = searchStringTextBox.Text;

            saveFileDialog1.FileName = plateName + "_Locations.txt"; // suggest a file name to the user

            saveFileDialog1.Filter = "Text files (*.txt)|*.txt";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                saveFilePathAndFileName = saveFileDialog1.FileName;

                FileInfo fi = new FileInfo(saveFilePathAndFileName);

                extractDataGridGPSLocation(saveFilePathAndFileName);


            }

        }

        

        private void listBoxSelectImageStore_SelectedIndexChanged(object sender, EventArgs e)
        {
      //      m_PathManager.Drive = (string)listBoxSelectImageStore.SelectedItem.ToString();
           
        }

        private void plateDisplayPB_Click_1(object sender, EventArgs e)
        {
            if (!m_RepositoryEnabled)
            {
                MessageBox.Show("No repository connected, cannot search");
                return;

            }
                

            string relativeFileName = dataGridView1.Rows[m_CurrentRowIndex].Cells["imagePath"].Value.ToString();
            string filePath = m_PathManager.GetCompleteFilePath(relativeFileName);
           
            string PhotoGalleryPath = ProgramFilesx86()+"\\Windows Photo Gallery\\WindowsPhotoGallery.exe ";

            Process.Start(PhotoGalleryPath, filePath);
        }

        static string ProgramFilesx86()
        {
            if (8 == IntPtr.Size
                || (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432"))))
            {
                return Environment.GetEnvironmentVariable("ProgramFiles(x86)");
            }

            return Environment.GetEnvironmentVariable("ProgramFiles");
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void plateDisplayPB_Click_2(object sender, EventArgs e)
        {

        }

        object m_ZoomTrackLock;
        private void trackBarZoomControl_Scroll(object sender, EventArgs e)
        {
            if (dataGridView1.Rows.Count == 0) return;
            if (m_CurrentRowIndex + 1 > dataGridView1.Rows.Count) return;

            lock (m_ZoomTrackLock)
            {
                m_ZoomPictureControl.Zoom = (int)trackBarZoomControl.Value;

                string relativeFileName = dataGridView1.Rows[m_CurrentRowIndex].Cells["imagePath"].Value.ToString();
                string filePath = m_PathManager.GetCompleteFilePath(relativeFileName);
                {
                    Image img = Image.FromFile(filePath);
                    m_ZoomPictureControl.Picture = img;

                }
            }

       

        }

        VideoPlayerMainForm m_VideoPlayer;
       
        void UserClosedThePlayer()
        {
            m_VideoPlayer = null;
        }

        private void buttonPlayVideo_Click(object sender, EventArgs e)
        {

            if (m_VideoPlayer == null)
            {
                m_VideoPlayer = new VideoPlayerMainForm(m_AppData, UserClosedThePlayer);
            }

            if (m_VideoPlayer.IsDisposed)
            {
                m_VideoPlayer = new VideoPlayerMainForm(m_AppData, UserClosedThePlayer);
            }


            if (!m_VideoPlayer.Visible)
                m_VideoPlayer.Show();

            PushVideoRangeToVideoPlayer();
        }

        void PushVideoRangeToVideoPlayer()
        {
            if (m_VideoPlayer == null) return;
            
            Thread.Sleep(1000); // wait for the player form to get loaded by windows

            while (!m_VideoPlayer.IsHandleCreated && !m_KillThreads)
            {
                Thread.Sleep(1);
            }

            TimeSpan prepost = new TimeSpan(0, 0, 5);
            string PSS = dataGridView1.Rows[m_CurrentRowIndex].Cells["PSS"].Value.ToString();
            string cameraName = dataGridView1.Rows[m_CurrentRowIndex].Cells["cameraName"].Value.ToString();
            DateTime timeStamp = (DateTime)dataGridView1.Rows[m_CurrentRowIndex].Cells["time"].Value;

            m_VideoPlayer.LoadVideo(PSS, cameraName, timeStamp.Subtract(prepost), timeStamp.Add(prepost));
        }

        private void buttonDumpMotionEvents_Click(object sender, EventArgs e)
        {
            if (m_SearchInProgress) return;

            if (!m_RepositoryEnabled)
            {
                MessageBox.Show("No repository connected, cannot search");
                return;
            }

            m_SearchStatusDisplay.ClearResults();

            m_SearchCount++;

            DateTime searchWindowStart = dateTimePicker1.Value;
            DateTime searchWindowEnd = dateTimePicker2.Value;

            m_SearchByNumber = false;

            // clear out the previous search results

            dataGridView1.Rows.Clear();


            m_SearchTool.SearchForPlate(SearchLib.SEARCH_TYPE.MOTION, "", textBoxCameraNameFilter.Text, searchWindowStart, searchWindowEnd, 0, SearchProgressCB, SearchCompleteCB);

        }

        private void buttonAllImagesInRange_Click(object sender, EventArgs e)
        {
            if (m_SearchInProgress) return;

            if (!m_RepositoryEnabled)
            {
                MessageBox.Show("No repository connected, cannot search");
                return;
            }


            m_SearchStatusDisplay.ClearResults();


            m_SearchCount++;

            DateTime searchWindowStart = dateTimePicker1.Value;
            DateTime searchWindowEnd = dateTimePicker2.Value;


            // clear out the previous search results

            dataGridView1.Rows.Clear();

            m_SearchTool.SearchForPlate(SearchLib.SEARCH_TYPE.ALL_IMAGES, "", textBoxCameraNameFilter.Text, searchWindowStart, searchWindowEnd, 0, SearchProgressCB, SearchCompleteCB);
        }

        private void buttonDumpRange_Click_1(object sender, EventArgs e)
        {
            if (!m_RepositoryEnabled)
            {
                MessageBox.Show("No repository connected, cannot search");
                return;
            }


            if (m_SearchInProgress) return;
            m_SearchInProgress = true;

            m_SearchStatusDisplay.ClearResults();

            m_SearchByNumber = false;

            searchForPlate("", 0, textBoxCameraNameFilter.Text);

        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void labelRepository_Click(object sender, EventArgs e)
        {

        }

       
       





    }
}
