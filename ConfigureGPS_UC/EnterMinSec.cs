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
    public partial class EnterMinSec : UserControl
    {
        public class FRAC_DATA
        {
            public double degrees;
            public double fractional;
            public EWNS direction;
        }

        public enum EWNS  { EAST, WEST, NORTH, SOUTH };

        LatitudeLongitudeType m_type;
      
        public EnterMinSec (LatitudeLongitudeType type)
        {
            InitializeComponent();

            m_type = type;

            radioButton1.Click += new EventHandler(radioButton1_Click);
            radioButton2.Click += new EventHandler(radioButton2_Click);

          

            textBoxDegrees.Leave += new EventHandler(textBoxDegrees_MouseLeave);
            textBoxMinutes.Leave += new EventHandler(textBoxMinutes_MouseLeave);
            textBoxSeconds.Leave += new EventHandler(textBoxSeconds_Leave);

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
                {
                    double minSecs = Convert.ToDouble(m);
                    double frac = minSecs - Math.Floor(minSecs);
                    double secs = frac * 60;
                    textBoxMinutes.Text = Math.Floor(minSecs).ToString();
                    textBoxSeconds.Text = secs.ToString();
                }
                else
                    textBoxMinutes.Text = null;

                d = UserSettings.Get(UserSettingTags.GPSLocationFixedLonDirection);
                radioButton2.Checked = true;// default setting
                if (d != null)
                {
                    if (d.Contains("EAST"))
                    {
                        radioButton2.Checked = false;
                        radioButton1.Checked = true;
                        radioButton1.Invalidate();
                        radioButton2.Invalidate();
                    }
                    else
                    {
                        radioButton2.Checked = true;
                        radioButton1.Checked = false;
                        radioButton1.Invalidate();
                        radioButton2.Invalidate();

                    }
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
                {
                    double minSecs = Convert.ToDouble(m);
                    double frac = minSecs - Math.Floor(minSecs);
                    double secs = frac * 60;
                    textBoxMinutes.Text = Math.Floor(minSecs).ToString();
                    textBoxSeconds.Text = secs.ToString();
                }
                else
                    textBoxMinutes.Text = null;

                d = UserSettings.Get(UserSettingTags.GPSLocationFixedLatDirection);
                radioButton1.Checked = true;// default setting

                if (d != null)
                {
                    if (d.Contains("NORTH"))
                    {
                        radioButton2.Checked = false;
                        radioButton1.Select();
                        radioButton1.Checked = true;
                        radioButton1.Invalidate();
                        radioButton2.Invalidate();
                    }
                    else
                    {
                        radioButton1.Checked = false;

                        radioButton2.Select();
                        radioButton2.Checked = true;
                        radioButton1.Invalidate();
                        radioButton2.Invalidate();
                    }
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


        void textBoxSeconds_Leave(object sender, EventArgs e)
        {
            double Seconds;

            if (textBoxSeconds.Text.Length == 0) return;

            try
            {
                Seconds = Convert.ToDouble(textBoxSeconds.Text);

                if (Seconds < 0 || Seconds > 60.0)
                {
                    MessageBox.Show("Error: Seconds must be greater than or equal to zero and less than 60.0");
                    textBoxSeconds.Text = "";
                    return;
                }

            }
            catch
            {
                MessageBox.Show("Error: Seconds must be greater than or equal to zero and less than 60.0");
                textBoxSeconds.Text = "";
                return;
            }

            


            // if here we have valid data
            if (m_type == LatitudeLongitudeType.LONGITUDE)
            {
                UserSettings.Set(UserSettingTags.GPSLocationFixedLonMinutes, GetSecondsMinutes());
            }
            else
            {
                UserSettings.Set(UserSettingTags.GPSLocationFixedLatMinutes, GetSecondsMinutes());
            }
        }


        string GetSecondsMinutes()
        {
            double secs = GetSeconds();
            double mins = GetMinutes();

            double fractional = mins + (secs / 60.0 );
            return ( fractional.ToString());
        }

        double GetSeconds()
        {
            double secs;
            try
            {
                secs = Convert.ToDouble(textBoxSeconds.Text);
            }
            catch
            {
                secs = 0.0;
            }
            return (secs);
        }

        double GetMinutes()
        {
            double mins;
            try
            {
                mins = Convert.ToDouble(textBoxMinutes.Text);
            }
            catch
            {
                mins = 0.0;
            }
            return (mins);
        }

        void SaveMinutes()
        {
            double minutes;

            if (textBoxMinutes.Text.Length == 0) return;

            try
            {
                minutes = Convert.ToDouble(textBoxMinutes.Text);

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

            // no fractional part allowed - that goes in the seconds box
            if (minutes - Math.Ceiling(minutes) > 0)
            {
                MessageBox.Show("fractional component goes into seconds field");
                textBoxMinutes.Text = "";
                return;
            }


            // if here we have valid data
            if (m_type == LatitudeLongitudeType.LONGITUDE)
            {
                UserSettings.Set(UserSettingTags.GPSLocationFixedLonMinutes, GetSecondsMinutes());
            }
            else
            {
                UserSettings.Set(UserSettingTags.GPSLocationFixedLatMinutes, GetSecondsMinutes());
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

        void SaveDirections()
        {
            if (m_type == LatitudeLongitudeType.LONGITUDE)
            {
                if ( radioButton1.Checked)
                    UserSettings.Set(UserSettingTags.GPSLocationFixedLonDirection, radioButton1.Text);
                else
                    UserSettings.Set(UserSettingTags.GPSLocationFixedLonDirection, radioButton2.Text);
            }


            if (m_type == LatitudeLongitudeType.LATITUDE)
            {
                if (radioButton1.Checked)
                    UserSettings.Set(UserSettingTags.GPSLocationFixedLatDirection, radioButton1.Text);
                else
                    UserSettings.Set(UserSettingTags.GPSLocationFixedLatDirection, radioButton2.Text);       
            }
        }

        void textBoxDegrees_MouseLeave(object sender, EventArgs e)
        {
      
        }

        public void SaveChanges()
        {
            SaveDegrees();
            SaveMinutes();
            SaveDirections();
        }

        private void panelEnterLatitudeMinutes_Paint(object sender, PaintEventArgs e)
        {

        }

        private void textBoxSeconds_TextChanged(object sender, EventArgs e)
        {

        }

        
    }
}
