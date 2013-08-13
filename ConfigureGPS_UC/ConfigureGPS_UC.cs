using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ApplicationDataClass;

namespace ConfigureGPS_UC
{
    public enum LatitudeLongitudeType { LATITUDE, LONGITUDE }


    public partial class ConfigureGPS_UC : UserControl
    {
        public ConfigureGPS_UC()
        {
            InitializeComponent();
        }

        /// <summary>
        /// set the AppData reference for this class to use
        /// </summary>
 
        public APPLICATION_DATA AppData
        {
            set { m_AppData = value; }
        }

        private void ConfigureGPS_UC_Load(object sender, EventArgs e)
        {
            m_enterLocationFractionalPage = new enterLocationFractionalPage();
            m_enterLocationFractionalPage.Location = new Point(10, 10);

            m_enterLocationMinSecPage = new enterLocationMinSecPage();
            m_enterLocationMinSecPage.Location = new Point(10, 10);

            groupBoxEnterFractional.Controls.Add(m_enterLocationFractionalPage);

            groupBoxEnterInMinutesSeconds.Controls.Add(m_enterLocationMinSecPage);

        }

     APPLICATION_DATA m_AppData;

        enterLocationFractionalPage m_enterLocationFractionalPage;
        enterLocationMinSecPage m_enterLocationMinSecPage;

        private void buttonSaveMinSec_Click(object sender, EventArgs e)
        {
            m_AppData.LPRServiceControl.ConfigChangedServiceNeedsRestarting = true;

            m_enterLocationMinSecPage.SaveChanges();
            m_enterLocationFractionalPage.Refresh();
        }

        private void buttonSaveFrac_Click(object sender, EventArgs e)
        {
            m_AppData.LPRServiceControl.ConfigChangedServiceNeedsRestarting = true;

            m_enterLocationFractionalPage.SaveChanges();
            m_enterLocationMinSecPage.Refresh();
        }
      
    }
}
