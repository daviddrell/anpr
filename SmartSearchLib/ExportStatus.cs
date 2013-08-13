using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SmartSearchLib
{
    public partial class ExportStatus : Form
    {
        public ExportStatus()
        {
            InitializeComponent();
        }

        private void ExportStatus_Load(object sender, EventArgs e)
        {
            labelStatusCount.Enabled = true;
            labelStatusCount.Text = "Currently at 0";
        }




        private delegate void PrettyMuchUseless_SetProgress_Delegate(int currentCount, int totalCount);
        public void SetProgress(int currentCount, int totalCount)
        {

            if (this.InvokeRequired)
            {
                this.Invoke(new PrettyMuchUseless_SetProgress_Delegate(SetProgress), new object[] { currentCount,  totalCount});
            }
            else
            {
                // do work here
              labelStatusCount.Text = "Currently at " + currentCount.ToString() + "  of total: " + totalCount.ToString();
              progressBar1.Value = (int) ( 100 * (float)currentCount/totalCount);

              if (currentCount == totalCount)
                  this.Close();
            }

        }
    }
}
