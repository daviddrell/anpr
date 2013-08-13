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
using System.Threading;
using UserSettingsLib;
using System.IO;
using Utilities;
using DVRLib;

namespace AnalystsWorkstation
{
    public partial class ImportImageDrive : UserControl
    {
        public ImportImageDrive(APPLICATION_DATA appData)
        {
            InitializeComponent();
            m_AppData = appData;
            m_AppData.AddOnClosing(Stop, APPLICATION_DATA.CLOSE_ORDER.MIDDLE);
            m_Log = (ErrorLog)m_AppData.Logger;

       
            m_PathManager = (PATHS)m_AppData.PathManager;
            m_DVR = (DVR)m_AppData.DVR;

            m_CancelMove = new CancelObject();
            m_CancelMove.cancel = false;

            
        }

        CancelObject m_CancelMove;
        PATHS m_PathManager;
        DVR m_DVR;
        APPLICATION_DATA m_AppData;
        ErrorLog m_Log;
       
        Thread m_MoveFilesThread;
        string m_CentralRepository;
        string m_ImportDrive;

        bool m_Stop;
        void Stop()
        {
            m_Stop = true;
        }

        public void PushCentralRepository(string central)
        {
            try
            {
                m_CentralRepository = central;

                string displayText = null;
                if (m_CentralRepository != null)
                    displayText = m_CentralRepository;
                else
                    displayText = "";

                this.BeginInvoke((MethodInvoker)delegate { textBoxCentralRepository.Text = displayText; });

               

                if (Directory.Exists(m_ImportDrive) && Directory.Exists(m_CentralRepository) && !m_StartImportClicked)
                {
                    buttonStartImport.Enabled = true;
                }
                else
                {
                    buttonStartImport.Enabled = false;   
                }

                if (! Directory.Exists(m_ImportDrive) || ! Directory.Exists(m_CentralRepository)  )
                {
           
                    if (m_StartImportClicked) m_CancelMove.cancel = true;
                }

                this.BeginInvoke((MethodInvoker)delegate { labelDriveFreeSpace.Text = "Free space: " + m_DVR.DriveFreeSpace.ToString("###,###."); });

            }
            catch { }
        }

        public void PushImportDrive(string import)
        {
            try
            {
                m_ImportDrive = import;

                string displayText = null;
                if (m_ImportDrive != null)
                    displayText = m_ImportDrive;
                else
                    displayText = "";

                this.BeginInvoke((MethodInvoker)delegate { textBoxFieldDrive.Text = displayText; });

                if (Directory.Exists(m_ImportDrive) && Directory.Exists(m_CentralRepository) && !m_StartImportClicked)
                {
                    buttonStartImport.Enabled = true;
                }
                else
                {
                    buttonStartImport.Enabled = false;
                }

                if (!Directory.Exists(m_ImportDrive) || !Directory.Exists(m_CentralRepository))
                {                  
                    if (m_StartImportClicked) m_CancelMove.cancel = true;
                }
               
            }
            catch { }
        }


        bool m_StartImportClicked = false;
        int m_NumFilesToMove = 0;
      

        void StatusCallBackHandler(int count, DateTime time)
        {
            string status = "Phase: " + m_Phase + ", count :  " + count.ToString()+ ", Time: " + time.ToString();

            if (this.IsHandleCreated)
            {
                this.BeginInvoke((MethodInvoker)delegate { listBoxStatus.Items.Add(status); });
                this.BeginInvoke((MethodInvoker)delegate { SelectLastItemInListBox(); });
            }

        }

        void AddToListBox(string status)
        {
            if (this.IsHandleCreated)
            {
                this.BeginInvoke((MethodInvoker)delegate { _AddToListBox(status); });
             
            }
        }

        void _AddToListBox(string status)
        {
            listBoxStatus.Items.Add(status);
          //  if (listBoxStatus.Items.Count > 200) listBoxErrors.Items.RemoveAt(0);
            SelectLastItemInListBox();
        }

        void SetStatusLabel(string status)
        {
            if (this.IsHandleCreated)
            {
                this.BeginInvoke((MethodInvoker)delegate { labelStatus.Text = status; });
               
            }
        }


        void SelectLastItemInListBox()
        {
            listBoxStatus.SelectedIndex = listBoxStatus.Items.Count - 1;
        }
     
        private void buttonStartImport_Click(object sender, EventArgs e)
        {
            m_StartImportClicked = true;

            m_GetCountThread = new Thread(GetCountLoop);
            m_GetCountThread.Start();
          
        }

        Thread m_GetCountThread;
        PATHS.CancelJPegCountLoop m_CancelJPegCount;

        void GetCountLoop()
        {
            DateTime aLongLongTimeAgo = new DateTime(1901, 1, 1);
            DateTime aLongLongTimInTheFuture = new DateTime(2099, 1, 1);

            int statusIncrement = 1000;

            m_Phase = "Counting files";

            // get a count of the number of files to move
            m_CancelJPegCount = new PATHS.CancelJPegCountLoop();
            m_CancelJPegCount.cancel = false;
            m_NumFilesToMove = m_PathManager.GetCountOfJpegsInRange(m_ImportDrive, aLongLongTimeAgo, aLongLongTimInTheFuture, StatusCallBackHandler, statusIncrement, m_CancelJPegCount);


            if (m_Stop || m_CancelMove.cancel)
            {
                StopAndResetStates();
                return;
            }

            // tell user how many files are about to be moved

            AddToListBox("will move " + m_NumFilesToMove.ToString() + "  files to central repository");
            SetStatusLabel ( "will move " + m_NumFilesToMove.ToString() + "  files to central repository");

            // spin a thread to move the files, the thread needs to send status counts back to the user display
            if (m_Stop || m_CancelMove.cancel)
            {
                StopAndResetStates();
                return;
            }

            m_CancelMove.cancel = false;
            m_MoveFilesThread = new Thread(MoveFilesLoop);
            m_MoveFilesThread.Start();

        }

        void StopAndResetStates()
        {
            m_StartImportClicked = false;
            m_CancelMove.cancel = false;
        }

        string m_Phase = "starting";

        void MoveFilesLoop()
        {
            // get first day
            DateTime day = m_PathManager.GetFirstDay(m_ImportDrive);
            DateTime lastDay = m_PathManager.GetLastDay(m_ImportDrive);
            TimeSpan oneDay = new TimeSpan(1, 0, 0, 0);
            DateTime dayPlusOne = day.Add(oneDay);
            int count = 0;

            DateTime Aug_25 = new DateTime(2009, 8, 25);

            while (!m_Stop && !m_CancelMove.cancel)
            {
                // move a day

                // move all jpegs

            //    AddToListBox("collecting file names for day : "+day.ToString());

                if (day.Day == Aug_25.Day && day.Month == Aug_25.Month)
                {
                    int jj = day.Month;//breakpoint
                }
          
                string[] filesToMove = m_PathManager.GetAllJpegsInRange(m_ImportDrive, day, dayPlusOne);

                if (m_Stop || m_CancelMove.cancel)
                {
                    StopAndResetStates();
                    return;
                }

                if (filesToMove.Count() >  0 )
                    AddToListBox("starting move for day : "+day.ToString());

                m_Phase = "Moving Images";

               
                foreach (string sourcPath in filesToMove)
                {
                    PATHS.IMAGE_FILE_DATA sourceFd = m_PathManager.ParseFileCompleteJpegPath(sourcPath);
                    PATHS.IMAGE_FILE_DATA destFd = m_PathManager.GetCompleteFilePath(sourceFd, m_CentralRepository);
                    
                    m_DVR.MoveMergeFileToDVRStorage( sourceFd , destFd);

                

                    count++;

                    if (count % 1000 == 0) StatusCallBackHandler(count, sourceFd.timeStamp);

                    if (m_Stop || m_CancelMove.cancel)
                    {
                        StopAndResetStates();
                        return;
                    }
                }
              

                // move all event log files

                filesToMove = m_PathManager.GetAllLogFilesInRange(m_ImportDrive, day, dayPlusOne);

                if (m_Stop || m_CancelMove.cancel)
                {
                    StopAndResetStates();
                    return;
                }

             

                foreach (string sourcPath in filesToMove)
                {
                    PATHS.IMAGE_FILE_DATA sourceFd = m_PathManager.ParseFileCompleteLogfilePath(sourcPath);
                    PATHS.IMAGE_FILE_DATA destFd = m_PathManager. GetCompleteFilePath(sourceFd, m_CentralRepository);

                    m_DVR.MoveMergeFileToDVRStorage(sourceFd, destFd);


                    count++;

                   
                    if (m_Stop || m_CancelMove.cancel)
                    {
                        StopAndResetStates();
                        return;
                    }
                }

              
                day = day.Add(oneDay);
                dayPlusOne = dayPlusOne.Add(oneDay);

                if (day.CompareTo(lastDay.Add(oneDay)) > 0) 
                    break;                         // we are finished

              
            }

            if (m_Stop || m_CancelMove.cancel)
            {
                StopAndResetStates();
                return;
            }

            // now delete the directories that are now empty

            string oldestDay = m_DVR.Paths.GetOldestDayDir(m_ImportDrive);

            while (oldestDay != null)
            {
                m_DVR.FileAccessControl.DeleteDirectoryAndContents( oldestDay);

                oldestDay = m_DVR.Paths.GetOldestDayDir(m_ImportDrive);

                if (m_Stop || m_CancelMove.cancel)
                {
                    StopAndResetStates();
                    return;
                }
            }
      

         

            // tell the user we are finished
          

            AddToListBox("finsihed");
            SetStatusLabel("finsihed");

            m_CancelMove.cancel = false;
            m_StartImportClicked = false;
        }


        class CancelObject
        {
            public bool cancel;
        }

        private void buttonStopImport_Click(object sender, EventArgs e)
        {
            //cancel the moving
            m_CancelMove.cancel = true;
            m_CancelJPegCount.cancel = true;

            // tell the user we are cancelling moving.
            AddToListBox("Cancelling");

           
        }
    }
}
