using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WatchlistLib;
using ApplicationDataClass;
using System.Threading;

namespace ConfigWatchListsUC
{
    public partial class ConfigWatchListsUC : UserControl
    {
        public ConfigWatchListsUC(APPLICATION_DATA appData)
        {
            InitializeComponent();
            m_AppData = appData;
            m_AppData.AddOnClosing(Stop, APPLICATION_DATA.CLOSE_ORDER.LAST);

            textBoxAlertMatchThreshold.Leave += new EventHandler(textBoxAlertMatchThreshold_Leave);

            m_EditingExisting = false;
        }

       

        APPLICATION_DATA m_AppData;
        WatchLists WatchTools;

      

        private void ConfigWatchListsUC_Load(object sender, EventArgs e)
        {
            // load any existing watch lists specified in the user settings file

            WatchTools = new WatchLists(m_AppData);

            WatchTools.MasterList = WatchTools.LoadListsFromUserConfig();
           
            labelListStatus.Text = " ";
            LoadTable();
            DisableBoxes();
        }

        bool m_Stop;
        void Stop()
        {
            m_Stop = true;
        }

        void LoadTable()
        {
            foreach (WatchLists.WatchListControl list in WatchTools.MasterList)
            {
                listBoxCurrentlyLoadedWatchLists.Items.Add(list.UserAssignedGroupName);
            }
        }

        WatchLists.WatchListControl m_NewListInEditMode;
        private void buttonAddNew_Click(object sender, EventArgs e)
        {
            if (m_EditingExisting)
            {
                MessageBox.Show("Edit in progress, either save changes or cancel changes before adding new");
                return;
            }

            int index = WatchTools.GetNextIndex();
            m_NewListInEditMode = new WatchLists.WatchListControl(index);
            textBoxName.Text = m_NewListInEditMode.UserAssignedGroupName = "New List";
            labelListStatus.Text = "Editing New List";
            ClearBoxes();
            EnableBoxes();
        }

        private void textBoxWatchListFile_TextChanged(object sender, EventArgs e)
        {
            if (m_NewListInEditMode == null) return;

        }
        
        void textBoxAlertMatchThreshold_Leave(object sender, EventArgs e)
        {
            //  if (m_NewListInEditMode == null) return;
            try
            {
                int score = Convert.ToInt32(textBoxAlertMatchThreshold.Text);
                if (score <= 0 || score > 100)
                {
                    MessageBox.Show("Threshold must be a number between 1 and 100");
                    textBoxAlertMatchThreshold.Text = "0";
                    return;
                }
            }
            catch 
            {
                MessageBox.Show("Threshold must be a number between 1 and 100");
                textBoxAlertMatchThreshold.Text = "0";
                return;
            }
        }
     
        private void textBoxAlertMatchThreshold_TextChanged(object sender, EventArgs e)
        {
           
        }

        private void textBoxEmailListFile_TextChanged(object sender, EventArgs e)
        {
            if (m_NewListInEditMode == null) return;

        }

        private void buttonSelectWatchFile_Click(object sender, EventArgs e)
        {
            if (m_NewListInEditMode == null) return;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                m_NewListInEditMode.DataFileCompletePath = openFileDialog1.FileName;
                textBoxWatchListFile.Text = openFileDialog1.FileName;
                m_LoadListThread = new Thread(LoadListLoop);
                m_LoadListThread.Start();
            }
        }

        Thread m_LoadListThread;
        void LoadListLoop()
        {
            int count = WatchTools.loadList(m_NewListInEditMode);
            this.BeginInvoke((MethodInvoker)delegate { this.textBoxListCount.Text = count.ToString(); });
        }

        private void buttonSelectedEmailList_Click(object sender, EventArgs e)
        {
            if (m_NewListInEditMode == null) return;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                m_NewListInEditMode.EmailFileCompletePath = openFileDialog1.FileName;
                textBoxEmailListFile.Text = openFileDialog1.FileName;
                WatchTools.loadEmails(m_NewListInEditMode);
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
           
            m_NewListInEditMode.UserAssignedGroupName = textBoxName.Text;
            m_NewListInEditMode.AlertThreshold = Convert.ToInt32(textBoxAlertMatchThreshold.Text);
            
            // are there duplicates of this name?
            if (!m_EditingExisting)
            {
                if (WatchTools.NameIsADuplicate(m_NewListInEditMode))
                {
                    MessageBox.Show("ERROR the user assigned name: " + m_NewListInEditMode.UserAssignedGroupName + " is a duplicate of an existing name");
                    return; 
                }
            }

            // is the watch list loaded ?
            if (!m_NewListInEditMode.WatchListLoaded)
            {
                MessageBox.Show("Error Watchlist is not loaded");
                return;
            }

            m_SaveInProcess = true;
            listBoxCurrentlyLoadedWatchLists.Enabled = false;

            m_AppData.LPRServiceControl.ConfigChangedServiceNeedsRestarting = true;

            if (!m_EditingExisting)
            {
                // add new item
                WatchTools.AddItem(m_NewListInEditMode); // this includes a save to user config file 
            }
            else
            {
                // modify existing item

                WatchTools.RemoveItem(m_OriginalBeforeEdits.UserAssignedGroupName);
                WatchTools.AddItem(m_NewListInEditMode);// this includes a save to user config file
            }


            listBoxCurrentlyLoadedWatchLists.Items.Clear();

            LoadTable();// re-load the master list into the listbox
           
            m_NewListInEditMode = null;

            ClearBoxes();

            labelListStatus.Text = "List Saved";
            DisableBoxes();
            m_EditingExisting = false;
            listBoxCurrentlyLoadedWatchLists.Enabled = true;
            m_SaveInProcess = false;

        }

        void ClearBoxes()
        {
            textBoxAlertMatchThreshold.Text = "";
            textBoxEmailListFile.Text = "";
            textBoxListCount.Text = "";
            textBoxName.Text = "";
            textBoxWatchListFile.Text = "";
        }

        void EnableBoxes()
        {
            textBoxAlertMatchThreshold.Enabled = true;
            textBoxEmailListFile.Enabled = true;
            textBoxListCount.Enabled = true;
            textBoxName.Enabled = true;
            textBoxWatchListFile.Enabled = true;
        }

        void DisableBoxes()
        {
     
            textBoxAlertMatchThreshold.Enabled = false;
            textBoxEmailListFile.Enabled = false;
            textBoxListCount.Enabled = false;
            textBoxName.Enabled = false;
            textBoxWatchListFile.Enabled = false;
        }

       

        private void buttonDeleteSelected_Click(object sender, EventArgs e)
        {
            string name = (string)listBoxCurrentlyLoadedWatchLists.SelectedItem;

            WatchTools.RemoveItem(name);

            listBoxCurrentlyLoadedWatchLists.Items.Clear();
            LoadTable();

            ClearBoxes();
            m_NewListInEditMode = null;
            m_EditingExisting = false;
            DisableBoxes();
            labelListStatus.Text = "Deleted Selected Watch List";            
        }


        bool m_SaveInProcess;
        bool m_EditingExisting;
        WatchLists.WatchListControl m_OriginalBeforeEdits;

        private void listBoxCurrentlyLoadedWatchLists_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (m_SaveInProcess) return;

            ClearBoxes();
            m_NewListInEditMode = new WatchLists.WatchListControl();

            m_NewListInEditMode.UserAssignedGroupName = (string)listBoxCurrentlyLoadedWatchLists.SelectedItem;
            WatchTools.GetListFromUserConfig( m_NewListInEditMode.UserAssignedGroupName, m_NewListInEditMode);

            textBoxAlertMatchThreshold.Text = m_NewListInEditMode.AlertThreshold.ToString();
            textBoxEmailListFile.Text = m_NewListInEditMode.EmailFileCompletePath;
            textBoxListCount.Text = WatchTools.loadList(m_NewListInEditMode).ToString();
            textBoxName.Text = m_NewListInEditMode.UserAssignedGroupName;
            textBoxWatchListFile.Text = m_NewListInEditMode.DataFileCompletePath;

            m_OriginalBeforeEdits = m_NewListInEditMode.Clone();

            EnableBoxes();
            labelListStatus.Text = "Editing List";
            m_EditingExisting = true;
        }

        private void buttonCancelChanges_Click(object sender, EventArgs e)
        {
            ClearBoxes();
            m_NewListInEditMode = null;
            m_EditingExisting = false;
            DisableBoxes();
            labelListStatus.Text = "Canceled Edit";
        }

    }
}
