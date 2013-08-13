using System;
using System.Collections.Generic;
using System.Collections;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using System.Drawing;
using ApplicationDataClass;
using ErrorLoggingLib;

namespace Control_Center
{
    class MultiLevelDataGrid
    {

        public MultiLevelDataGrid(APPLICATION_DATA appData, System.Drawing.Size size, System.Drawing.Point location, string name, UserControl parent)
        {
            m_AppData = appData;
            m_Log = (ErrorLog) m_AppData.Logger;

            m_MainGrid = new DataGridView();
            m_SubGrids = new List<DataGridView>();
            m_GroupHashTable = new Hashtable();
            m_SubGroupHashTable = new Hashtable();


            plusSign = (Bitmap)Bitmap.FromFile(Application.StartupPath + "//plus.bmp");
          
            minusSign = (Bitmap)Bitmap.FromFile(Application.StartupPath + "//minus.bmp");
          

            InitMainContainerGrid(m_MainGrid, size, location, name);
       //     AddExpandIconCol(m_MainGrid);

            parent.Controls.Add(m_MainGrid);

        }

        APPLICATION_DATA m_AppData;
        ErrorLog m_Log;
        DataGridView m_MainGrid;
        List<DataGridView> m_SubGrids;
        Hashtable m_GroupHashTable;
        Hashtable m_SubGroupHashTable;
       
        Bitmap plusSign;
       
        Bitmap minusSign;
 

        /// <summary>
        /// Add a group. Returns the new group index.
        /// </summary>
        public int AddGroup ( string Group )
        {
           
            m_MainGrid.Rows.Add(plusSign, Group);
           
            int index = m_MainGrid.Rows.Count - 1;
            m_MainGrid.Rows[index].Height = 14;

            m_GroupHashTable.Add(Group, index);


            return (index);
        }

        void CreateSubTable()
        {
            DataGridView table = new DataGridView();

        }
        /// <summary>
        /// Removes the group by index
        /// </summary>
        public void RemoveGroup(int index)
        {
           

            if (index < m_MainGrid.Rows.Count)
            {
                string group =(string) m_MainGrid.Rows[index].Cells[1].Value;
                m_MainGrid.Rows.RemoveAt(index);

                if (m_GroupHashTable.Contains(group))
                {
                    m_GroupHashTable.Remove(group);
                }
            }

        }

        /// <summary>
        /// Removes the group by name
        /// </summary>
        public void RemoveGroup(string Group)
        {
            int index = 0;

            if (m_GroupHashTable.Contains(Group))
            {
                index =(int) m_GroupHashTable[Group];
                m_MainGrid.Rows.RemoveAt(index);
                m_GroupHashTable.Remove(Group);
            }
        }

        void InitMainContainerGrid(DataGridView grid,  System.Drawing.Size size, System.Drawing.Point location, string name)
        {
            grid.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
           // grid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            //grid.colRemoteName,
            //grid.colIPAddress,
            //grid.colDescription,
            //grid.colStatus});
            grid.Location = location;
            grid.Name = name;
            grid.Size = size;
            grid.TabIndex = 6;
      //      grid.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewRemoteHostList_CellContentClick);

            grid.RowHeadersVisible = false;
            grid.ColumnHeadersVisible = false;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.None;

            DataGridViewTextBoxColumn headerCol = new DataGridViewTextBoxColumn();
            headerCol.HeaderText = " ";
            headerCol.ReadOnly = true;
            headerCol.Name = "colGroup";
            headerCol.Width = grid.Width - 18 ;
            grid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] { headerCol });

            grid.AllowUserToAddRows = false;
            //grid.colRemoteName,
            //grid.colIPAddress,
            //grid.colDescription,
            //grid.colStatus});

        }

        void AddExpandIconCol(DataGridView grid)
        {
            DataGridViewImageColumn imageColumn = new DataGridViewImageColumn();
            imageColumn.Width = 16;
            imageColumn.Image = plusSign;
            imageColumn.HeaderText = " ";
            grid.Columns.Insert(0, imageColumn);
            grid.CellClick += new DataGridViewCellEventHandler(grid_CellClick);
        }

        void grid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {
                if (m_MainGrid.Rows[e.RowIndex].Cells[0].Value == (Bitmap)plusSign)
                {
                    m_MainGrid.Rows[e.RowIndex].Cells[0].Value = (Bitmap)minusSign;

                }
                else
                {
                    m_MainGrid.Rows[e.RowIndex].Cells[0].Value = (Bitmap)plusSign;
                }
            }
        }


    }
}
