using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Utilities;
using ApplicationDataClass;
using System.Globalization;

namespace Control_Center
{
    public partial class StatsViewer : UserControl
    {
        public StatsViewer( APPLICATION_DATA appData)
        {
            InitializeComponent();
            m_AppData = appData;

            m_StatGroups = m_AppData.HealthStatistics.GetGroupList();
            m_AllStats = m_AppData.HealthStatistics.GetStatList();
            m_GroupManager = new STAT_GROUP_MANAGER();
            m_GroupManager.LoadGroups(m_StatGroups);
           

          

            singleton = new object();

            ButtonSize = new Size(this.Size.Width/4, 25 );
            Locations = new Point[NUM_BUTTONS];
            m_PushStatusInfoQ = new ThreadSafeQueue<string>(2);
            m_ButtonClickHandlers = new ButtonClick[NUM_BUTTONS];

            // row one
            Locations[0] = new Point(0, 0);
            Locations[1] = new Point(ButtonSize.Width, 0);
            Locations[2] = new Point(ButtonSize.Width*2, 0);
            Locations[3] = new Point(ButtonSize.Width*3, 0);

            // row two
            Locations[4] = new Point(0, ButtonSize.Height);
            Locations[5] = new Point(ButtonSize.Width, ButtonSize.Height);
            Locations[6] = new Point(ButtonSize.Width * 2, ButtonSize.Height);
            Locations[7] = new Point(ButtonSize.Width * 3, ButtonSize.Height);

            ButtonUsed = new bool[NUM_BUTTONS];

            Buttons = new Button[NUM_BUTTONS];
            for (int b = 0; b < NUM_BUTTONS; b++)
            {
                Buttons[b] = new Button();
                ButtonUsed[b] = false;
            }

            int groupCnt = 0;
            foreach (string group in m_StatGroups)
            {
                if (groupCnt == NUM_BUTTONS) break;
                int b = GetNextAvailableButton();
                DefineButton(Buttons[b], ButtonSize, group, Locations[b], b);
                this.Controls.Add(Buttons[b]);
            }

           

            foreach (string group in m_StatGroups)
            {
                GridTableManager grid = new GridTableManager();
                grid.CreateGrid(new Size(this.Size.Width, this.Size.Height - 50), new Point(0, 50));
                m_GroupManager[group].AddGrid(grid);
            }

        }


        APPLICATION_DATA m_AppData;
        const int NUM_BUTTONS = 8;
        Size ButtonSize;
        Point[] Locations;
        Button[] Buttons;
        bool[] ButtonUsed;
        object singleton;
        string[] m_StatGroups;
        string[] m_AllStats;
        STAT_GROUP_MANAGER m_GroupManager;

        delegate void ButtonClick(object sender, EventArgs e);
        ButtonClick[] m_ButtonClickHandlers;
      

        void HandleGroupButtonClick(int buttonIndex)
        {

        }

        ThreadSafeQueue<string> m_PushStatusInfoQ;

        /// <summary>
        /// Push a new status string (received from server) to be displayed
        /// </summary>
        /// <param name="statusInfo"></param>
        public void PushStatusInfo (string statusInfo )
        {
            m_PushStatusInfoQ.Enqueue(statusInfo);
            this.BeginInvoke((MethodInvoker)delegate { this._PushStatusInfo( ); });
        }

        void _PushStatusInfo( )
        {
            string statusInfo  = m_PushStatusInfoQ.Dequeue();
            if (statusInfo == null) return;

            ParseAndDisplay(statusInfo);
        }

        void ParseAndDisplay(string statusInfo)
        {
            string[] sp1 = statusInfo.Split(',');

            // do groupings
            foreach (string group in m_StatGroups)
            {
                m_GroupManager[group].ClearGroup();// clear out the values from the last update

                foreach (string s in sp1)
                {
                    if (s.Contains(group + "_"))
                    {
                        m_GroupManager[group].AddStats(s);
                    }
                }
            }

            // now load the display table for this group.

        }

        public bool AddGroup(string GroupName)
        {
            lock (singleton)
            {
                int b = GetNextAvailableButton();
                if (b == -1) return (false);

                DefineButton(Buttons[b], ButtonSize, GroupName, Locations[b], b);
            }
            return (true);
        }

        int GetNextAvailableButton()
        {
            lock (singleton)
            {
                for (int b = 0; b < NUM_BUTTONS; b++)
                {
                    if (ButtonUsed[b] == false)
                    {
                        ButtonUsed[b] = true;
                        return (b);
                    }
                }
            }
            return (-1);
        }

        void DefineButton(Button btn, Size size, string label, Point location, int tabIndex)
        {
            // 
            btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            btn.BackColor = Color.FromArgb(60, 60, 90);
            btn.ForeColor = System.Drawing.Color.White;
            btn.FlatAppearance.BorderSize = 1;
            btn.FlatAppearance.BorderColor = Color.FromArgb(100, 100, 150);
            btn.FlatAppearance.CheckedBackColor = Color.FromArgb(100, 100, 130);
            btn.Location = location;
            btn.Name = label;
            btn.Size = size;
            btn.TabIndex = tabIndex;
            btn.Text = label;
            btn.UseVisualStyleBackColor = true;
            btn.Click += new EventHandler(btn_Click);
        }


        void btn_Click(object sender, EventArgs e)
        {
            for (int b = 0; b < NUM_BUTTONS; b++)
            {
                if (sender.Equals(Buttons[b]))
                {
                    ShowGroupGrid(Buttons[b].Name);
                }
            }

        }

        DataGridView m_CurrentGrid;

        void ShowGroupGrid(string groupName)
        {
            if ( m_CurrentGrid != null )
                this.Controls.Remove(m_CurrentGrid);

            m_CurrentGrid = m_GroupManager[groupName].GetDataGrid();

            this.Controls.Add(m_CurrentGrid);
        }



        class STAT_GROUP_MANAGER
        {

            public STAT_GROUP_MANAGER()
            {
                groupNamesTable = new Hashtable();
                singleton = new object();
            }
            Hashtable groupNamesTable;
            object singleton;


            /// <summary>
            /// Provide a list of group names to create the group headings
            /// </summary>
            /// <param name="groupNames"></param>
            public void LoadGroups(string[] groupNames)
            {
                lock (singleton)
                {
                    foreach (string gn in groupNames)
                    {
                        GROUP g = new GROUP();
                        groupNamesTable.Add(gn, g);
                    }
                }
            }

            /// <summary>
            /// use an array indexer to access a group using the group name as the indexer
            /// </summary>
            /// <param name="groupName"></param>
            /// <returns></returns>
            public GROUP this[string groupName]
            {
                get
                {
                    GROUP g = null;

                    lock (singleton)
                    {
                        if (groupNamesTable.ContainsKey(groupName))
                            return ( (GROUP)groupNamesTable[groupName]);

                        return (g);
                    }
                }
            }

            public class GROUP
            {
               
                
                List<string> StatNames;
                List<string> StatValues;
                GridTableManager Grid;
          
                object _singleton;
              

                public GROUP()
                {
                    _singleton = new object();
                    ClearGroup();

                }

                public void AddGrid(GridTableManager dg)
                {
                    lock (_singleton)
                    {
                        Grid = dg;
                    }
                }

                public DataGridView GetDataGrid()
                {
                    lock (_singleton)
                    {
                        return (Grid.GetGrid());
                    }
                }

                public void GetStat(int index, out string name, out string value)
                {
                    lock (_singleton)
                    {
                        name = null;
                        value = null;

                        if (index < 0) return;
                        if (index >= StatNames.Count) return;

                        name = StatNames[index];
                        value = StatNames[index];
                    }
                }

                public void GetAllStats(out string[] names, out string[] values)
                {
                    lock (_singleton)
                    {
                        names = StatNames.ToArray();
                        values = StatNames.ToArray();
                    }
                }

                public void ClearGroup()
                {
                    // reset the lists
                    lock (_singleton)
                    {
                        StatNames = new List<string>();
                        StatValues = new List<string>();
                        if (Grid != null) Grid.ClearGrid();
                    }
                }

                public  int Count ()
                {
                    lock (_singleton)
                    {
                        return (StatNames.Count);
                    }
                }

                public void AddStats(string stat)
                {
                    // sample string:   "LPR_FrameCount:14"
                    lock (_singleton)
                    {
                        string[] sp1 = stat.Split(new Char[] { '_' }, 2);

                        if (sp1.Length < 2) return;
                        string[] sp2 = sp1[1].Split(':');

                        string name = sp2[0];
                        if (sp2.Length < 2) return;
                        string value = sp2[1].Replace("^^", ":");

                        StatNames.Add(name);

                         // DVR_FreeSpace,
                        //DVR_UsedSpace,
                        if (name.Contains("FreeSpace") || name.Contains("UsedSpace"))
                        {
                            double val = (double)Convert.ToDouble(value);
                            val /= 1000000000;
                            value = val.ToString("###,###.######", CultureInfo.InvariantCulture) + " GB";
                           
                        }

                        // if DVR group, get rid of the type specifier on the end: FreeSpace.StringStat  gets rid of the "StringStat" and leaves FreeSpace
                        if (sp1[0].Contains("DVR"))
                        {
                            string[] sp3 = name.Split('.');
                            name = sp3[0];
                        }

                     

                        StatValues.Add(value);

                        Grid.AddRow(name, value);
                    }
                  
                }
            }
        }


        /// ////////////////////////////////////////////////////////////////

        //
        //    GridTableManager

        class GridTableManager
        {


            public GridTableManager()
            {
                singleton = new object();
            }

            object singleton;
            DataGridView dg;


            public DataGridView GetGrid()
            {
                lock (singleton)
                {
                    return (dg);
                }
            }

            public void ClearGrid()
            {
                lock (singleton)
                {
                    dg.Rows.Clear();
                }
            }

            public void CreateGrid( System.Drawing.Size size, System.Drawing.Point location)
            {
                lock (singleton)
                {
                    dg = new DataGridView();

                    InitMainContainerGrid(dg, size, location);
                }

            }

            public void AddRow(string statLable, string statValue)
            {
                lock (singleton)
                {
                    dg.Rows.Add(statLable, statValue);
                }
            }

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

                DataGridViewTextBoxColumn statLable = new DataGridViewTextBoxColumn();
                statLable.HeaderText = " ";
                statLable.ReadOnly = true;
                statLable.Name = "statLable";
                statLable.Width = grid.Width/ 2;

                DataGridViewTextBoxColumn statValue = new DataGridViewTextBoxColumn();
                statValue.HeaderText = " ";
                statValue.ReadOnly = true;
                statValue.Name = "statValue";
                statValue.Width = grid.Width / 2;

                grid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { statLable, statValue });

                grid.AllowUserToAddRows = false;
               

            }
       
        
        }
    }
}
