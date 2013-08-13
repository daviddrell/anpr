using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UserSettingsLib;


namespace ConfigureGPS_UC
{
    public partial class EnterFractional : UserControl
    {
        public class FRAC_DATA
        {
            public double degrees;
            public double fractional;
            public EWNS direction;
        }

        public enum EWNS  { EAST, WEST, NORTH, SOUTH };

        LatitudeLongitudeType m_type;
   

        public EnterFractional(LatitudeLongitudeType type)
        {
            InitializeComponent();

            m_type = type;

            radioButton1.Click += new EventHandler(radioButton1_Click);
            radioButton2.Click += new EventHandler(radioButton2_Click);


            textBoxDegrees.Leave += new EventHandler(textBoxDegrees_MouseLeave);
            textBoxMinutes.Leave += new EventHandler(textBoxMinutes_MouseLeave);
            
            Refresh();
        }

        public override void Refresh()
        {

            base.Refresh();

            if (m_type == LatitudeLongitudeType.LONGITUDE)
            {
                radioButton1.Text = "EAST";
                radioButton2.Text = "WEST";

                string d = UserSettings.Get(UserSettingTags.GPSLocationFixedLonDegrees);
                if (d != null)
                    textBoxDegrees.Text = d;
                else
                    textBoxDegrees.Text = null;

                string m = UserSettings.Get(UserSettingTags.GPSLocationFixedLonMinutes);
                if (m != null)
                    textBoxMinutes.Text = m;
                else
                    textBoxMinutes.Text = null;

                d = UserSettings.Get(UserSettingTags.GPSLocationFixedLonDirection);
                radioButton2.Checked = true;//default setting
                if (d != null)
                {
                    if (d.Contains("EAST"))
                        radioButton1.Checked = true;
                    else
                        radioButton2.Checked = true;
                }

            }

            if (m_type == LatitudeLongitudeType.LATITUDE)
            {
                radioButton1.Text = "NORTH";
                radioButton2.Text = "SOUTH";

                string d = UserSettings.Get(UserSettingTags.GPSLocationFixedLatDegrees);
                if (d != null)
                    textBoxDegrees.Text = d;
                else
                    textBoxDegrees.Text = null;

                string m = UserSettings.Get(UserSettingTags.GPSLocationFixedLatMinutes);
                if (m != null)
                    textBoxMinutes.Text = m;
                else
                    textBoxMinutes.Text = null;

                d = UserSettings.Get(UserSettingTags.GPSLocationFixedLatDirection);
                radioButton1.Checked = true; // default setting
                if (d != null)
                {

                    if (d.Contains("NORTH"))
                        radioButton1.Checked = true;
                    else
                        radioButton2.Checked = true;
                }
            }

           
        }

        void radioButton2_Click(object sender, EventArgs e)
        {
            if (m_type == LatitudeLongitudeType.LONGITUDE)
                UserSettings.Set(UserSettingTags.GPSLocationFixedLonDirection, radioButton2.Text);

            if (m_type == LatitudeLongitudeType.LATITUDE)
                UserSettings.Set(UserSettingTags.GPSLocationFixedLatDirection, radioButton2.Text);
                      
        }

        void radioButton1_Click(object sender, EventArgs e)
        {
            if (m_type == LatitudeLongitudeType.LONGITUDE)
                UserSettings.Set(UserSettingTags.GPSLocationFixedLonDirection, radioButton1.Text);

            if (m_type == LatitudeLongitudeType.LATITUDE)
                UserSettings.Set(UserSettingTags.GPSLocationFixedLatDirection, radioButton1.Text);
        }

        public void GetData ( FRAC_DATA data)
        {
            data.degrees = Convert.ToDouble(textBoxDegrees.Text);
            data.fractional = Convert.ToDouble(textBoxMinutes.Text);

            if (m_type == LatitudeLongitudeType.LONGITUDE)
            {
                if (radioButton1.Checked)
                    data.direction = EWNS.EAST;
                else
                    data.direction = EWNS.WEST;
            }
            else
            {
                if (radioButton1.Checked)
                    data.direction = EWNS.NORTH;
                else
                    data.direction = EWNS.SOUTH;
            }

        }

        void SaveMinutes()
        {

            if (textBoxMinutes.Text.Length == 0) return;

            try
            {
                double minutes = Convert.ToDouble(textBoxMinutes.Text);

                if (minutes < 0 || minutes > 60.0)
                {
                    MessageBox.Show("Error: minutes must be greater than or equal to zero and less than 60.0");
                    textBoxMinutes.Text = "";
                    return;
                }

            }
            catch
            {
                MessageBox.Show("Error: minutes must be greater than or equal to zero and less than 60.0");
                textBoxMinutes.Text = "";
                return;
            }

            // if here we have valid data
            if (m_type == LatitudeLongitudeType.LONGITUDE)
            {
                UserSettings.Set(UserSettingTags.GPSLocationFixedLonMinutes, textBoxMinutes.Text);
            }
            else
            {
                UserSettings.Set(UserSettingTags.GPSLocationFixedLatMinutes, textBoxMinutes.Text);
            }
        }
        void textBoxMinutes_MouseLeave(object sender, EventArgs e)
        {

         
        }

        void SaveDegrees()
        {
             double Degrees;

            if (textBoxDegrees.Text.Length == 0) return;

            try
            {
                Degrees = Convert.ToDouble(textBoxDegrees.Text);

                if (m_type == LatitudeLongitudeType.LATITUDE)
                {
                    if (Degrees < 0 || Degrees > 90.0)
                    {
                        MessageBox.Show("Error: degress must be greater than or equal to zero and less than or equal to 90.0");
                        textBoxDegrees.Text = "";
                        return;
                    }
                }

                if (m_type == LatitudeLongitudeType.LONGITUDE)
                {
                    if (Degrees < 0 || Degrees > 180.0)
                    {
                        MessageBox.Show("Error: degress must be greater than or equal to zero and less than or equal to 180.0");
                        textBoxDegrees.Text = "";
                        return;
                    }
                }
            }
            catch
            {
                if (m_type == LatitudeLongitudeType.LATITUDE)
                {

                    MessageBox.Show("Error: degress must be greater than or equal to zero and less than or equal to 90.0");
                    textBoxDegrees.Text = "";
                    return;
                }

                if (m_type == LatitudeLongitudeType.LONGITUDE)
                {

                    MessageBox.Show("Error: degress must be greater than or equal to zero and less than or equal to 180.0");
                    textBoxDegrees.Text = "";
                    return;
                }
            }

            // if here we have valid data
            if (m_type == LatitudeLongitudeType.LONGITUDE)
            {
                UserSettings.Set(UserSettingTags.GPSLocationFixedLonDegrees, textBoxDegrees.Text);
            }
            else
            {
                UserSettings.Set(UserSettingTags.GPSLocationFixedLatDegrees, textBoxDegrees.Text);
            }
        }

        
        void textBoxDegrees_MouseLeave(object sender, EventArgs e)
        {
          
           
        }

        public void SaveChanges()
        {
            SaveMinutes();
            SaveDegrees();
        }

        private void panelEnterLatitudeMinutes_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
