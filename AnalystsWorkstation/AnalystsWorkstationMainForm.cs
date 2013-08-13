using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SmartSearchLib;
using ApplicationDataClass;
using PathsLib;
using ErrorLoggingLib;
using ScreenLoggerLib;
using FrameGeneratorLib;
using LPRServiceCore;
using System.Threading;
using UserSettingsLib;
using LPREngineLib;
using System.IO;
using Utilities;
using LPRInteractiveEditUC;
using DVRLib;
using EmailServicesLib;// just for testing
using WatchlistLib;// just for testing

namespace AnalystsWorkstation
{
    public partial class AnalystsWorkstationMainForm : Form
    {
        public AnalystsWorkstationMainForm()
        {
            InitializeComponent();

            m_LPRCore = new LPRServiceEntryPoint();
            m_AppData = m_LPRCore.GetAppData();

            this.Text = "First Evidence Plate Analysts Workstation, version " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();


            /// 
            ///    enable LPR Diagnostics
            ///    

            if (Application.ExecutablePath.Contains("Visual Studio")) // assumes I am debugging on Vista
            {
                m_AppData.LPRDiagEnabled = true;
            }
            else
            {
                m_AppData.LPRDiagEnabled = false;
            }

            m_AppData.LPRDiagEnabled = false;

            //  Remove Diagnostic tabs if not in diag mode

            if (!m_AppData.LPRDiagEnabled)
            {
                tabControlMain.TabPages.Remove(tabPageLPRDiagnostics);
                tabControlMain.TabPages.Remove(tabPageOCRLib);
            }
            else
            {
                OCRSourceFolder = UserSettings.Get(UserSettingTags.AW_OCRLibSourceDirectory);
                labelOCRSourceFolder.Text = OCRSourceFolder;

                OCRDestinationFolder = UserSettings.Get(UserSettingTags.AW_OCRLibDestinationDirectory);
                labelOCRDestinationFolder.Text = OCRDestinationFolder;
            }


            //// need some channel names assigned as place holders, off-line processing simulates hardware channels to the rest of the chain

            //for (int i = 0; i < m_AppData.MAX_VIRTUAL_CHANNELS; i++)
            //    UserSettings.Set(UserSettingTags.ChannelNames.Name(i), i.ToString());

            // allow parallel processing based on the number of cores. The ProcessorCount returns the number of cores

            m_AppData.MAX_VIRTUAL_CHANNELS = Math.Min(Environment.ProcessorCount,m_AppData.MAX_PHYSICAL_CHANNELS);


            m_DataGridRowIndex = new ThreadSafeHashTable(m_AppData.MAX_MOVIE_FILES_TO_LOAD);

            m_AppData.MoviePlayerParentForm = (object)this;

            m_AppData.RunninAsService = false;

            m_LPRCore.Start(m_AppData.RunninAsService);

            m_LPREngine = (LPREngine)m_AppData.LPREngine;
            m_LPREngine.OnNewFilteredPlateGroupEvent += new LPREngine.NewPlateEvent(m_LPREngine_OnNewPlateEvent);

            m_Log = (ErrorLog)m_AppData.Logger;

            m_FrameGenerator = (FrameGenerator)m_AppData.FrameGenerator;

            this.FormClosing += new FormClosingEventHandler(AnalystsWorkstationMainForm_FormClosing);

            m_PathManager = (PATHS)m_AppData.PathManager;
            m_DVR = (DVR)m_AppData.DVR;

            m_SmartSearchUC = new SmartSearchLibUC(m_AppData, OnSearchFatalError);

            m_SmartSearchUC.Location = new Point(0, 0);
            m_SmartSearchUC.Dock = DockStyle.Fill;


            CreateBatchModeVideoDisplayPanels();
            m_FrameGenerator.MovieFileController.DisplayPanels = m_VideoDisplayPanels;

            m_FrameGenerator.MovieFileController.OnStatusUpdateFromPlayerEvent += new MovieFiles.OnStatusUpdateFromPlayer(MovieFileController_OnStatusUpdateFromPlayerEvent);
            tabPageSearch.Controls.Add(m_SmartSearchUC);


            InitMainContainerGrid(dataGridViewFilesInProcess, new System.Drawing.Size(761, 200), new System.Drawing.Point(273, 483));

            m_EditModePictureSelectionViewer = new ListView();
            m_EditModePictureSelectionViewer.Location = new Point(219, 200);
            m_EditModePictureSelectionViewer.View = View.LargeIcon;
            m_EditModePictureSelectionViewer.Size = new Size(this.Size.Width - 275, this.Size.Height - 300);
            m_EditModePictureSelectionViewer.ItemSelectionChanged += new ListViewItemSelectionChangedEventHandler(m_EditModePictureSelectionViewer_ItemSelectionChanged);
            buttonListViewVisible.Text = m_ShowThumbNails;

            m_EditModePictureSelectionViewer.Enabled = false;
            m_EditModePictureSelectionViewer.Visible = false;
            m_EditModePictureSelectionViewer.SendToBack();

            tabPageEditMode.Controls.Add(m_EditModePictureSelectionViewer);

            tabControlLPRResults.TabPages.Clear();



            // get DVR storage mode for the batch and edit mode processing

            string sm = UserSettings.Get(UserSettingTags.DVR_StorageMode);
            if (sm != null)
            {
                if (sm.Contains(UserSettingTags.DVR_StorageModeValueStoreOnMotion))
                {
                    m_AppData.DVRMode = APPLICATION_DATA.DVR_MODE.STORE_ON_MOTION;

                    radioButtonStoreOnMotion.Checked = true;
                    radioButtonStoreOnPlate.Checked = false;
                }
                else
                {
                    radioButtonStoreOnMotion.Checked = false;
                    radioButtonStoreOnPlate.Checked = true;
                    m_AppData.DVRMode = APPLICATION_DATA.DVR_MODE.STORE_ON_PLATE_FOUND;
                }
            }
            else
            {
                radioButtonStoreOnMotion.Checked = false;
                radioButtonStoreOnPlate.Checked = true;
                m_AppData.DVRMode = APPLICATION_DATA.DVR_MODE.STORE_ON_PLATE_FOUND;
            }


            // by default, store batch outputs to repository

            radioToRepository.Checked = true;
            radioButtonToUserSpecifiedStorage.Checked = false;
            m_AppData.DVR_StoreToUserSpecifiedFolder = false;

            // now see if the user has previously chosen to store to specific folder -- get the user specified (if present) storage path for batch and edit mode processing

            string p = UserSettings.Get(UserSettingTags.DVR_UserSpecifiedStoragePath);
            if (p != null)
            {
                if (p.Contains(UserSettingTags.BOOL_TRUE))
                {
                    // the user want the batch processing results to go to a special folder and not the repository

                    radioToRepository.Checked = false;
                    string s = UserSettings.Get(UserSettingTags.DVR_UserSpecifiedStoragePath);
                    if (s == null)
                    {
                        // error condition, the config setting says use userspecfifiedstorage, but the path is not there

                        UserSettings.Set(UserSettingTags.DVR_StoreToUserSpecifiedFolder, UserSettingTags.BOOL_FALSE);
                        radioButtonToUserSpecifiedStorage.Checked = false;
                        string drive = null;

                        if (m_PathManager.Drive == null)
                            drive = "No repository found";
                        else
                            drive = m_PathManager.Drive;

                        textBoxUserSpecifiedStorageLocation.Text = drive;
                    }
                    else
                    {
                        // the good condition, all as it should be
                        m_AppData.DVR_UserSpecifiedStoragePath = s;

                        textBoxUserSpecifiedStorageLocation.Text = s;
                    }
                }
                else
                {
                    // the auto condition, use the DVRSTORAGE default storage area to put images and results into

                    radioButtonToUserSpecifiedStorage.Checked = false;
                    string drive = null;

                    if (m_PathManager.Drive == null)
                    {
                        // if the user has configured a standard repository, then create one in the default user data folder
                        string dirPath = Application.UserAppDataPath + "\\DVRSTORAGE";
                        if (!Directory.Exists(dirPath))
                        {
                            Directory.CreateDirectory(Application.UserAppDataPath + "\\DVRSTORAGE");
                        }


                        drive = m_PathManager.Drive;
                    }
                    else
                    {
                        drive = m_PathManager.Drive;
                    }

                    textBoxUserSpecifiedStorageLocation.Text = drive;
                }
            }


            // display to the user the time stamps from the processed images so that user has a clue where to find
            //  this images when searching in the search tool
            {
                int consumerID = ((FrameGenerator)m_AppData.FrameGenerator).GetNewConsumerID();
                int maxChannels = ((FrameGenerator)m_AppData.FrameGenerator).GetNumberOfPhysicalChannels();

                for (int c = 0; c < maxChannels; c++)
                    ((FrameGenerator)m_AppData.FrameGenerator).RegisterToConsumeChannel(consumerID, c, OnReceiveNewFrameToExtractTimeStamp); // get callback on new frames

                m_ToDisplayProcessedTimeStampsQ = new ThreadSafeQueue<FRAME>(240);     // store the frames into a Q when calledback.
                m_DisplayBatchTimeStampsThread = new Thread(DisplayBatchTimeStampsLoop); // empty the Q with this thread, dump to the listbox for user display
                m_DisplayBatchTimeStampsThread.Start();
            }




            // install the  user control to allow the user to import images from a field drive
            m_ImportImageDrive = new ImportImageDrive(m_AppData);
            tabPageImportImages.Controls.Add(m_ImportImageDrive);

            // show the results of LPR strings to the user
            m_LPRResultsToPostQ = new ThreadSafeQueue<LPR_RESULT_TO_POST>(100);
            m_LPRPostResultsThread = new Thread(LPRPostResultsLoop);
            m_LPRPostResultsThread.Start();

            // keep tabs on the repository - find one and keep checking for lost drives
            m_CheckRepositoryStatusThread = new Thread(CheckRepositoryLoop);
            m_CheckRepositoryStatusThread.Start();

            //    m_LPRCore.OnSelfDestruct += CloseThis;


        }


        Thread m_DisplayBatchTimeStampsThread;
        ThreadSafeQueue<FRAME> m_ToDisplayProcessedTimeStampsQ;

        ImportImageDrive m_ImportImageDrive;

        DVR m_DVR;
        Thread m_CheckRepositoryStatusThread;
        ListView m_EditModePictureSelectionViewer;
      
        LPRServiceEntryPoint m_LPRCore;
        LPREngine m_LPREngine;
        FrameGenerator m_FrameGenerator;
        ErrorLog m_Log;
        PathsLib.PATHS m_PathManager;
        APPLICATION_DATA m_AppData;
        SmartSearchLib.SmartSearchLibUC m_SmartSearchUC;
        bool m_Stop = false;
        ThreadSafeHashTable m_DataGridRowIndex;


        // set up picture box
        Panel[] m_VideoDisplayPanels;
        Label[] m_LPRResultDisplays;

        void OnReceiveNewFrameToExtractTimeStamp(FRAME frame)
        {
            m_ToDisplayProcessedTimeStampsQ.Enqueue(frame);
        }

        void DisplayBatchTimeStampsLoop()
        {
            while (!m_Stop)
            {
                // get time stamps from processed images to display to the user, so the user has a clue
                //  as to where these get stored, to enable searching

                FRAME frame = m_ToDisplayProcessedTimeStampsQ.Dequeue();

                if (frame != null)
                {
                   string timeStamp = frame.TimeStamp.ToString(m_AppData.TimeFormatStringForFileNames);
                   this.BeginInvoke((MethodInvoker)delegate { PostTimeStampToUser(timeStamp); });
                }

                Thread.Sleep(1);
            }
        }

        void PostTimeStampToUser(string timeStamp)
        {
            listBoxDisplayTimeStamps.Items.Add(timeStamp);

            // limit the lenght of the list box
            if (listBoxDisplayTimeStamps.Items.Count > 1000)
                listBoxDisplayTimeStamps.Items.RemoveAt(0);
        }

        void CheckRepositoryLoop()
        {
            while (!m_Stop)
            {
                Thread.Sleep(1000);

                // we are either in the back office and have an actual central repository,
                //  or we are running on a field unit and the field drive acts as the cetral repository  (for searching only).

                // the search tool uses m_DVR.Paths.Drive as a search location, and this field is always set so long as there is at least
                // one drive attached (of either type). If there is a central repository attached, it is the primay m_DVR.Paths.Drive

                // in the following logic, the ImportImage module and the batch mode and edit mode modules need a real 
                // central repository to push images to. We only have a real central if we have both
                //  m_DVR.Paths.Drive non-null and _DVR.GetFieldDrive() are non-null.
                // 
                // so, do we have two drives or one?

                string fieldDrive = m_DVR.GetFieldDrive();
                string centralRepositoryDrive = m_DVR.Paths.Drive;
            
                
                bool centralEnabled = false;

                if (centralRepositoryDrive != null)
                {
                    centralEnabled = true;
                }

                if ( this.IsHandleCreated)
                    SetRepositoryStatus(centralEnabled);

                if (m_SmartSearchUC.IsHandleCreated)
                    m_SmartSearchUC.SetCentralRepository(centralEnabled, m_PathManager.Drive);

                if (m_ImportImageDrive.IsHandleCreated)
                {
                    m_ImportImageDrive.PushCentralRepository(centralRepositoryDrive);// pusing a null value indicates no drive connected

                    m_ImportImageDrive.PushImportDrive(fieldDrive);// pusing a null value indicates no drive connected

                }

            }
        }

        string RepositoryEnabled = "Repository Enabled";
        string RepositoryDisabled = "Repository Not Connected";
        bool m_HaveNewDataToStore = false;

        void SetRepositoryStatus(bool enabled)
        {
            if (enabled)
            {
                this.BeginInvoke((MethodInvoker)delegate { labelRepositoryStatus.Text = RepositoryEnabled; });
                if ( m_HaveNewDataToStore )
                    this.BeginInvoke((MethodInvoker)delegate { buttonStoreResults.Enabled = true; });
            }
            else
            {
                this.BeginInvoke((MethodInvoker)delegate { labelRepositoryStatus.Text = RepositoryDisabled; });
                this.BeginInvoke((MethodInvoker)delegate { buttonStoreResults.Enabled = false; });
            }

        }

        void MovieFileController_OnStatusUpdateFromPlayerEvent(string file, MovieFiles.VIRTUAL_CHANNEL.PLAY_STATUS_UPDATE status, MovieFiles.VIRTUAL_CHANNEL.PlayNextFileDelegate playnext)
        {
            if (m_DataGridRowIndex.Contains(file))
            {
                int rowIndex = (int)m_DataGridRowIndex[file];
                dataGridViewFilesInProcess.Rows[rowIndex].Cells[1].Value = status.ToString();
            }

            if (playnext != null)
            {
                // the playnext delegate will be called from a UI thread, so that the Directshow movie player will run on the UI thread to eable painting on the UI owned panel
                this.BeginInvoke((MethodInvoker)delegate { playnext(); });
            }

        }

    
        void CreateBatchModeVideoDisplayPanels()
        {
            // 
            // panelVideoDisplay
            // 
            m_VideoDisplayPanels = new Panel[m_AppData.MAX_VIRTUAL_CHANNELS];
            m_LPRResultDisplays = new Label[m_AppData.MAX_VIRTUAL_CHANNELS];

            int startX = 273;
            int startY = 26;
            int width = 244;
            int height = 177;
            int xOffset = 260;
            int yOffset = 220;

            int labelYOffset = yOffset - 30;

         
            for (int c = 0; c < m_AppData.MAX_VIRTUAL_CHANNELS; c++)
            {
                int newYRow = 0;
                for (int j = 0; j < 4 && c < m_AppData.MAX_VIRTUAL_CHANNELS; j++)
                {
                  

                    m_VideoDisplayPanels[c] = new Panel();
                    m_VideoDisplayPanels[c].BackColor = Color.Black;
                    m_VideoDisplayPanels[c].Location = new System.Drawing.Point(startX + (j * xOffset), startY + (newYRow * yOffset));
                    m_VideoDisplayPanels[c].Name = "panelVideoDisplay" + c.ToString();
                    m_VideoDisplayPanels[c].Size = new System.Drawing.Size(width, height);
                    m_VideoDisplayPanels[c].TabIndex = c + 1;
                    tabPageBatchProcess.Controls.Add(m_VideoDisplayPanels[c]);
                    m_AppData.MoviePlayerParentPanel[c] = (object)m_VideoDisplayPanels[c];

                    m_LPRResultDisplays[c] = new Label();
                    m_LPRResultDisplays[c].Location = new System.Drawing.Point(startX + (j * xOffset), labelYOffset + startY + (newYRow * yOffset));

                    m_LPRResultDisplays[c].AutoSize = true;
                    m_LPRResultDisplays[c].Font = new System.Drawing.Font("Microsoft Sans Serif", 12.0F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    m_LPRResultDisplays[c].Name = "label" + c.ToString();
                    m_LPRResultDisplays[c].Size = new System.Drawing.Size(60, 20);
                    m_LPRResultDisplays[c].TabIndex = 1+c;
                    m_LPRResultDisplays[c].Text = c.ToString() ;

                    tabPageBatchProcess.Controls.Add(m_LPRResultDisplays[c]);
                    c++;

                }

                newYRow = 1;
                for (int j = 0; j < 4 && c < m_AppData.MAX_VIRTUAL_CHANNELS; j++)
                {
                    

                    m_VideoDisplayPanels[c] = new Panel();
                    m_VideoDisplayPanels[c].BackColor = Color.Black;
                    m_VideoDisplayPanels[c].Location = new System.Drawing.Point(startX + (j * xOffset), startY + (newYRow * yOffset));
                    m_VideoDisplayPanels[c].Name = "panelVideoDisplay" + c.ToString();
                    m_VideoDisplayPanels[c].Size = new System.Drawing.Size(width, height);
                    m_VideoDisplayPanels[c].TabIndex = c + 1;
                    tabPageBatchProcess.Controls.Add(m_VideoDisplayPanels[c]);
                    m_AppData.MoviePlayerParentPanel[c] = (object)m_VideoDisplayPanels[c];

                    m_LPRResultDisplays[c] = new Label();
                    m_LPRResultDisplays[c].Location = new System.Drawing.Point(startX + (j * xOffset), labelYOffset + startY + (newYRow * yOffset));

                    m_LPRResultDisplays[c].AutoSize = true;
                    m_LPRResultDisplays[c].Font = new System.Drawing.Font("Microsoft Sans Serif", 12.0F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    m_LPRResultDisplays[c].Name = "label" + c.ToString();
                    m_LPRResultDisplays[c].Size = new System.Drawing.Size(60, 20);
                    m_LPRResultDisplays[c].TabIndex = 1 + c;
                    m_LPRResultDisplays[c].Text = c.ToString();

                    tabPageBatchProcess.Controls.Add(m_LPRResultDisplays[c]);

                    c++;
                }

               
            }
        }



        void ClearDisplayPanels()
        {
            for (int c = 0; c < m_AppData.MAX_VIRTUAL_CHANNELS; c++)
            {
                m_VideoDisplayPanels[c].BackColor = Color.Black;
                m_VideoDisplayPanels[c].Invalidate();

                m_LPRResultDisplays[c].Text = "_";
            }



        }


        //  ////////////////////////////
        //
        //
        //    Receive New LPR results and display

        void m_LPREngine_OnNewPlateEvent(FRAME frame)
        {
            int chan = frame.SourceChannel;
            StringBuilder sb = new StringBuilder();
            foreach (string s in frame.PlateNumberLatin)
            {
                sb.Append(s);
                sb.Append(", ");
            }
            //string result = sb.ToString();
          //  this.BeginInvoke((MethodInvoker)delegate { this.PostLPRString(chan,result); });
            LPR_RESULT_TO_POST result = new LPR_RESULT_TO_POST();

            result.result = sb.ToString();
            result.chan = chan;
            m_LPRResultsToPostQ.Enqueue(result);
        }
        class LPR_RESULT_TO_POST
        {
            public string result;
            public int chan;
        }
        ThreadSafeQueue<LPR_RESULT_TO_POST> m_LPRResultsToPostQ;
        Thread m_LPRPostResultsThread;
        void LPRPostResultsLoop()
        {
            while (!m_Stop)
            {
                Thread.Sleep(100);

                LPR_RESULT_TO_POST r = m_LPRResultsToPostQ.Dequeue();
                if (r != null)
                {
                    this.BeginInvoke((MethodInvoker)delegate { this.PostLPRString(r.chan, r.result); });
                }
            }
        }

        void PostLPRString(int chan, string s)
        {
            m_LPRResultDisplays[chan].Text = s;
        }

        // close
        //    if generated internally, then AppData has started shutdown and told this form to close
        //    if generated externally, then this form has been told to close by the OS and now needs to tell AppData to close the rest of the threads
        //       if generated internally, then CloseThis is called before AnalystsWorkstationMainForm_FormClosing

        void CloseThis()
        {
            InternallyGeneratedClose = true;
            this.Close();
        }

        bool InternallyGeneratedClose = false;
        void AnalystsWorkstationMainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_Stop = true;
            if (m_AppLoaded && !InternallyGeneratedClose)
            {
                m_LPRCore.OnSelfDestruct -= CloseThis;
                m_AppData.CloseApplication();// this close was generated externall, tell AppData to shutdown
            }

            while (!m_SmartSearchUC.ThreadsStopped)
                Thread.Sleep(1);
        }

        void OnSearchFatalError()
        {
            // search tool wants a shutdown
            m_Stop = true;

        }

        bool m_AppLoaded = false;

        private void AnalystsWorkstationMainForm_Load(object sender, EventArgs e)
        {
            m_AppLoaded = true;
            if (m_Stop)
            {
                this.Close();// did the search tool ask for a shutdown?
            }



        }

        private void buttonSelectFile_Click(object sender, EventArgs e)
        {
            try
            {
                dataGridViewFilesInProcess.Rows.Clear();
                m_DataGridRowIndex.Clear();

                openFileDialog1.FileName = " "; // clear out the default file name 'openFileDialog1'

                openFileDialog1.Filter = "Media Files|*.asf;*.avi;*.wmv;*.mov|All Files|*.*";

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string[] files = new string[1];// because MovieFileController.LoadFiles(files) wants a file array
                    files[0] = openFileDialog1.FileName;
                    LoadDefaultCameraName(files[0]);
                    m_FrameGenerator.MovieFileController.LoadFiles(files);

                    dataGridViewFilesInProcess.Rows.Add(files[0], "not started");
                    m_DataGridRowIndex.Add(files[0], dataGridViewFilesInProcess.Rows.Count - 1);

                }
            }
            catch (Exception ex) { m_Log.Trace(ex, ErrorLog.LOG_TYPE.INFORMATIONAL); }
        }

      //    dg.Rows.Add(statLable, statValue);


        void InitMainContainerGrid(DataGridView grid, System.Drawing.Size size, System.Drawing.Point location)
        {
            grid.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            DataGridViewRow row = grid.RowTemplate;
            row.DefaultCellStyle.BackColor = Color.Bisque;
            row.Height = 20;
            row.MinimumHeight = 20;


            grid.Location = location;
            grid.Size = size;
            grid.TabIndex = 0;

            grid.RowHeadersVisible = false;
            grid.ColumnHeadersVisible = false;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.None;

            DataGridViewTextBoxColumn fileName = new DataGridViewTextBoxColumn();
            fileName.HeaderText = "File Name";
            fileName.ReadOnly = true;
            fileName.Name = "fileName";
            fileName.Width = ( 3* grid.Width )/ 4;

            DataGridViewTextBoxColumn processStatus = new DataGridViewTextBoxColumn();
            processStatus.HeaderText = "Process Status";
            processStatus.ReadOnly = true;
            processStatus.Name = "processStatus";
            processStatus.Width = grid.Width / 4;

            grid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { fileName, processStatus });

            grid.AllowUserToAddRows = false;

        }

        private void dataGridViewFilesInProcess_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void buttonAddDirectory_Click(object sender, EventArgs e)
        {
            try
            {
                dataGridViewFilesInProcess.Rows.Clear();
                m_DataGridRowIndex.Clear();

                openFileDialog1.FileName = " "; // clear out the default file name 'openFileDialog1'

                openFileDialog1.Filter = "Media Files|*.asf;*.avi;*.wmv;*.mov|All Files|*.*";

                if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                {
                    string dirPath = folderBrowserDialog1.SelectedPath;

                    string[] allfiles = Directory.GetFiles(dirPath);

                    if ( allfiles.Count() > 0 ) LoadDefaultCameraName(allfiles[0]);

                    List<string> filesToProcess = new List<string>();
                    foreach (string file in allfiles)
                    {
                        if (file.Contains(".asf") || file.Contains(".avi") || file.Contains(".wmv") || file.Contains(".mov"))
                        {
                            dataGridViewFilesInProcess.Rows.Add(file, "not started");
                            m_DataGridRowIndex.Add(file, dataGridViewFilesInProcess.Rows.Count-1);
                            filesToProcess.Add(file);
                        }
                    }

                    string [] files = (string[])filesToProcess.ToArray();
                    m_FrameGenerator.MovieFileController.LoadFiles(files);

             
                }

            }
            catch (Exception ex) { m_Log.Trace(ex, ErrorLog.LOG_TYPE.FATAL); }
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {

            if (dataGridViewFilesInProcess.Rows.Count < 1)
            {
                MessageBox.Show("No video files or jpegs have been selected to process. Please select source.");
                return;
            }

            this.BeginInvoke((MethodInvoker)delegate { listBoxDisplayTimeStamps.Items.Clear(); });// clear out previous time-stamp display

            ClearDisplayPanels();

            m_FrameGenerator.MovieFileController.Play();
        }

        private void buttonStop_Click(object sender, EventArgs e)
        {

            if (!AreWeFinishedProcessingAllFiles())
            {
                DialogResult r =  MessageBox.Show ("Are you sure you want to stop? Stopping will require a re-start from the begining. Click No to cancel Stop.","Cofirm Stop", MessageBoxButtons.YesNo);
                {
                    if ( r !=  DialogResult.Yes )
                    {
                        return;
                    }
                }
            }

            // need to stop on a seperate thread to avoid directshow deadlock, DS runs on the UI thread
            m_StopVideosThread = new Thread(StopVideos);
            m_StopVideosThread.Start();

            dataGridViewFilesInProcess.Rows.Clear();
            m_DataGridRowIndex.Clear();
        }

        Thread m_StopVideosThread;

        void StopVideos()
        {
            m_FrameGenerator.MovieFileController.Stop();
            this.BeginInvoke((MethodInvoker)delegate { this.ClearDisplayPanels(); });
            
        }

        private void tabPageEditMode_Click(object sender, EventArgs e)
        {

        }

        private void radioButtonStoreOnMotion_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonStoreOnMotion.Checked)
            {
                m_AppData.DVRMode = APPLICATION_DATA.DVR_MODE.STORE_ON_MOTION;
                UserSettings.Set(UserSettingTags.DVR_StorageMode, UserSettingTags.DVR_StorageModeValueStoreOnMotion);
            }
            else
            {
                m_AppData.DVRMode = APPLICATION_DATA.DVR_MODE.STORE_ON_PLATE_FOUND;
                UserSettings.Set(UserSettingTags.DVR_StorageMode, UserSettingTags.DVR_StorageModeValueStoreOnPlate);
            }
        }

        private void radioButtonStoreOnPlate_CheckedChanged(object sender, EventArgs e)
        {

        }
  

        private void buttonSelectJpegDirectory_Click(object sender, EventArgs e)
        {


            try
            {
                dataGridViewFilesInProcess.Rows.Clear();
                m_DataGridRowIndex.Clear();

              

                if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                {

                    string[] jpegDirectory = new string[1];
                    jpegDirectory[0] = folderBrowserDialog1.SelectedPath;

                    if (jpegDirectory.Count() > 0) LoadDefaultCameraName(jpegDirectory[0]);

                    m_FrameGenerator.MovieFileController.LoadFiles(jpegDirectory);

                    dataGridViewFilesInProcess.Rows.Add(jpegDirectory[0], "not started");
                    m_DataGridRowIndex.Add(jpegDirectory[0], dataGridViewFilesInProcess.Rows.Count - 1);

                }
            }
            catch (Exception ex) { m_Log.Trace(ex, ErrorLog.LOG_TYPE.INFORMATIONAL); }
        }


        string [] BatchLoadJpegsFromDirectory(string path)
        {
            string[] jpegsToProcess = null;

            try
            {
              
                string[] sa1 = Directory.GetFiles(path, "*.jpg");
                string[] sa2 = Directory.GetFiles(path, "*.jpeg");
                string[] sa3 = Directory.GetFiles(path, "*.bmp");

                jpegsToProcess = new string[sa1.Length + sa2.Length + sa3.Length];

                sa1.CopyTo(jpegsToProcess, 0);
                sa2.CopyTo(jpegsToProcess, sa1.Length);
                sa3.CopyTo(jpegsToProcess, sa2.Length);
               
               

                if (jpegsToProcess == null)
                {
                    MessageBox.Show("No images found");
                    labelNumberOfImagesLoaded.Text = "No images loaded";
                    return null;
                }

                if (jpegsToProcess.Count() == 0)
                {
                    MessageBox.Show("No images found");
                    labelNumberOfImagesLoaded.Text = "No images loaded";
                    return null;
                }

                if (jpegsToProcess.Count() > m_AppData.MAX_IMAGES_TO_EDIT)
                {
                    MessageBox.Show("Too many images to load, max is " + m_AppData.MAX_IMAGES_TO_EDIT.ToString());
                    labelNumberOfImagesLoaded.Text = "No images loaded";
                    return null;
                }

            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }

            return (jpegsToProcess);
        }




        bool pictureBoxCurrentImageFullSize = false;
        private void pictureBoxCurrentImage_Click(object sender, EventArgs e)
        {
            if (pictureBoxCurrentImageFullSize)
            {
                pictureBoxCurrentImageFullSize = false;
                pictureBoxCurrentImage.Location = new Point(222, 42);
                pictureBoxCurrentImage.Size = new Size(286, 154);
                pictureBoxCurrentImage.Invalidate();
            }
            else
            {
                pictureBoxCurrentImageFullSize = true;
                pictureBoxCurrentImage.Location = new Point(0, 0);
                pictureBoxCurrentImage.Size = new Size(tabPageEditMode.Size.Width, tabPageEditMode.Size.Height);
                pictureBoxCurrentImage.BringToFront();
                pictureBoxCurrentImage.Invalidate();
            }
        }

        string OCRSourceFolder;
        private void buttonSelectOCRSourceFolder_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                OCRSourceFolder = folderBrowserDialog1.SelectedPath;
                labelOCRSourceFolder.Text = OCRSourceFolder;
                UserSettings.Set(UserSettingTags.AW_OCRLibSourceDirectory, OCRSourceFolder);
            }
        }

        string OCRDestinationFolder;
        private void buttonSelectOCRDestinationFolder_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                OCRDestinationFolder = folderBrowserDialog1.SelectedPath;
                labelOCRDestinationFolder.Text = OCRDestinationFolder;
                UserSettings.Set(UserSettingTags.AW_OCRLibDestinationDirectory, OCRDestinationFolder);
            }
        }

        private void buttonOCRGenLib_Click(object sender, EventArgs e)
        {
            if (OCRSourceFolder == null || OCRDestinationFolder == null) return;

            GenerateOCRLib genLib = new GenerateOCRLib(m_AppData, OCRSourceFolder, OCRDestinationFolder);

            genLib.GenerateLibrary();

            MessageBox.Show("Done");
        }

        private void textBoxUserSpecifiedCameraName_TextChanged(object sender, EventArgs e)
        {
            m_AppData.UserSpecifiedCameraName = textBoxUserSpecifiedCameraName.Text;
        }

        private void radioToRepository_CheckedChanged(object sender, EventArgs e)
        {
            if (radioToRepository.Checked)
            {
                m_AppData.DVR_StoreToUserSpecifiedFolder = false;
                textBoxUserSpecifiedStorageLocation.Text = m_PathManager.Drive;
            }
            else
            {
                ConfigureUserSpecifiedStorage();
            }
        }

        private void radioButtonToUserSpecifiedStorage_CheckedChanged(object sender, EventArgs e)
        {

        }



        private void ConfigureUserSpecifiedStorage()
        {
            if (radioButtonToUserSpecifiedStorage.Checked)
            {
                string p = UserSettings.Get(UserSettingTags.DVR_UserSpecifiedStoragePath);
                if (p != null) folderBrowserDialog1.SelectedPath = p;

                if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                {
                    m_AppData.DVR_UserSpecifiedStoragePath = folderBrowserDialog1.SelectedPath;
                    m_AppData.DVR_StoreToUserSpecifiedFolder = true;

                    textBoxUserSpecifiedStorageLocation.Text = m_AppData.DVR_UserSpecifiedStoragePath;

                    UserSettings.Set(UserSettingTags.DVR_UserSpecifiedStoragePath, m_AppData.DVR_UserSpecifiedStoragePath);
                    UserSettings.Set(UserSettingTags.DVR_StoreToUserSpecifiedFolder, UserSettingTags.BOOL_TRUE);
                }
                else // user canceled folder selection
                {
                    m_AppData.DVR_UserSpecifiedStoragePath = null;
                    m_AppData.DVR_StoreToUserSpecifiedFolder = false;
                    textBoxUserSpecifiedStorageLocation.Text = m_PathManager.Drive;

                    UserSettings.Remove(UserSettingTags.DVR_UserSpecifiedStoragePath);
                    UserSettings.Set(UserSettingTags.DVR_StoreToUserSpecifiedFolder, UserSettingTags.BOOL_FALSE);

                    radioToRepository.Checked = true;
                }

            }
            else
            {
                m_AppData.DVR_UserSpecifiedStoragePath = null;
                m_AppData.DVR_StoreToUserSpecifiedFolder = false;
                textBoxUserSpecifiedStorageLocation.Text = m_PathManager.Drive;

                UserSettings.Remove(UserSettingTags.DVR_UserSpecifiedStoragePath);
                UserSettings.Set(UserSettingTags.DVR_StoreToUserSpecifiedFolder, UserSettingTags.BOOL_FALSE);
            }
        }



        void LoadDefaultCameraName(string fromFirstFile)
        {
            textBoxUserSpecifiedCameraName.Text = Path.GetFileNameWithoutExtension(fromFirstFile);
        }


        bool AreWeFinishedProcessingAllFiles()
        {
            //MovieFiles.VIRTUAL_CHANNEL.PLAY_STATUS_UPDATE

            for(int i  =0 ; i < dataGridViewFilesInProcess.Rows.Count; i++)
            {
                // is the status == completed for all rows?
                if ( ! dataGridViewFilesInProcess.Rows[i].Cells[1].Value.ToString().Contains(MovieFiles.VIRTUAL_CHANNEL.PLAY_STATUS_UPDATE.COMPLETED.ToString()))
                {
                    return (false);
                }

            }
            return true;
        }
 
    }
}
