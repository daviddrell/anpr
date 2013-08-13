using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using UserSettingsLib;
using ApplicationDataClass;

namespace ConfigureSourceChannels
{
    public partial class ConfigSourceChannels : UserControl
    {
        public ConfigSourceChannels(APPLICATION_DATA appData)
        {
            InitializeComponent();
            m_AppData = appData;

            textBoxChan1.Leave += new EventHandler(textBoxChan1_Leave);
            textBoxChan2.Leave += new EventHandler(textBoxChan2_Leave);
            textBoxChan3.Leave += new EventHandler(textBoxChan3_Leave);
            textBoxChan4.Leave += new EventHandler(textBoxChan4_Leave);

            LoadVideoStandardSettings();

        }

        bool m_RadioButtonsBeingInitialized = false;

        void LoadVideoStandardSettings()
        {
            string pal_Ntsc = UserSettings.Get(UserSettingTags.VideoSetup_PAL_NTSC);

            m_RadioButtonsBeingInitialized = true;

            radioButtonNTSC.Checked = true; // default setting
            radioButtonPAL.Checked = false;

            if (pal_Ntsc != null)
            {
                if (pal_Ntsc.Equals(UserSettingTags.VideoSetup_PAL))
                {
                    radioButtonNTSC.Checked = false;
                    radioButtonPAL.Checked = true;
                }
            }

            m_RadioButtonsBeingInitialized = false;
        }


        APPLICATION_DATA m_AppData;

        void textBoxChan4_Leave(object sender, EventArgs e)
        {
            SaveSettings();
        }

        void textBoxChan3_Leave(object sender, EventArgs e)
        {
            SaveSettings();
        }

        void textBoxChan2_Leave(object sender, EventArgs e)
        {
            SaveSettings();
        }

        void textBoxChan1_Leave(object sender, EventArgs e)
        {
            SaveSettings();
        }

      
        private void buttonSave_Click(object sender, EventArgs e)
        {
            SaveSettings();
        }

       

        private void ConfigSourceChannels_Load(object sender, EventArgs e)
        {
            LoadSettings();
        }

        void SaveSettings()
        {   
            UserSettings.Set(UserSettingTags.ChannelNames.Name(0), textBoxChan1.Text);
            UserSettings.Set(UserSettingTags.ChannelNames.Name(1), textBoxChan2.Text);
            UserSettings.Set(UserSettingTags.ChannelNames.Name(2), textBoxChan3.Text);
            UserSettings.Set(UserSettingTags.ChannelNames.Name(3), textBoxChan4.Text);
            m_AppData.LPRServiceControl.ConfigChangedServiceNeedsRestarting = true;
        }

        bool m_SettingsBeingInitialized = false;

        void LoadSettings()
        {
            m_SettingsBeingInitialized = true;

            textBoxChan1.Text = UserSettings.Get(UserSettingTags.ChannelNames.Name(0));
            if (textBoxChan1.Text.Length == 0) textBoxChan1.Text = UserSettingTags.ChannelNotUsed;
            if (textBoxChan1.Text.Contains(UserSettingTags.ChannelNotUsed)) { checkBox1.Checked = false; textBoxChan1.Enabled = false; } else { checkBox1.Checked = true; textBoxChan1.Enabled = true; }

            textBoxChan2.Text = UserSettings.Get(UserSettingTags.ChannelNames.Name(1));
            if (textBoxChan2.Text.Length == 0) textBoxChan2.Text = UserSettingTags.ChannelNotUsed;
            if (textBoxChan2.Text.Contains(UserSettingTags.ChannelNotUsed)) { checkBox2.Checked = false; textBoxChan2.Enabled = false; } else { checkBox2.Checked = true; textBoxChan2.Enabled = true; }

            textBoxChan3.Text = UserSettings.Get(UserSettingTags.ChannelNames.Name(2));
            if (textBoxChan3.Text.Length == 0) textBoxChan3.Text = UserSettingTags.ChannelNotUsed;
            if (textBoxChan3.Text.Contains(UserSettingTags.ChannelNotUsed)) { checkBox3.Checked = false; textBoxChan3.Enabled = false; } else { checkBox3.Checked = true; textBoxChan3.Enabled = true; }

            textBoxChan4.Text = UserSettings.Get(UserSettingTags.ChannelNames.Name(3));
            if (textBoxChan4.Text.Length == 0) textBoxChan4.Text = UserSettingTags.ChannelNotUsed;
            if (textBoxChan4.Text.Contains(UserSettingTags.ChannelNotUsed)) { checkBox4.Checked = false; textBoxChan4.Enabled = false; } else { checkBox4.Checked = true; textBoxChan4.Enabled = true; }

            m_SettingsBeingInitialized = false;

        }

        void ClearSettings()
        {
            m_SettingsBeingInitialized = true;

            textBoxChan1.Text = UserSettingTags.ChannelNotUsed;
            textBoxChan1.Enabled = false;

            textBoxChan2.Text = UserSettingTags.ChannelNotUsed;
            textBoxChan2.Enabled = false;

            textBoxChan3.Text = UserSettingTags.ChannelNotUsed;
            textBoxChan3.Enabled = false;

            textBoxChan4.Text = UserSettingTags.ChannelNotUsed;
            textBoxChan4.Enabled = false;

            checkBox1.Checked = false;
            checkBox2.Checked = false;
            checkBox3.Checked = false;
            checkBox4.Checked = false;

            m_SettingsBeingInitialized = false;

        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            ClearSettings();
            SaveSettings();
        }

      
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (m_SettingsBeingInitialized) return;

            if (checkBox1.Checked)
            {
                textBoxChan1.Text = "1";
                textBoxChan1.Enabled = true;
            }
            else
            {
                textBoxChan1.Text = UserSettingTags.ChannelNotUsed;
                textBoxChan1.Enabled = false;
            }

            SaveSettings();

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (m_SettingsBeingInitialized) return;

            if (checkBox2.Checked)
            {
                textBoxChan2.Text = "2";
                textBoxChan2.Enabled = true;
            }
            else
            {
                textBoxChan2.Text = UserSettingTags.ChannelNotUsed;
                textBoxChan2.Enabled = false;
            }
            SaveSettings();

        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (m_SettingsBeingInitialized) return;

            if (checkBox3.Checked)
            {
                textBoxChan3.Text = "3";
                textBoxChan3.Enabled = true;
            }
            else
            {
                textBoxChan3.Text = UserSettingTags.ChannelNotUsed;
                textBoxChan3.Enabled = false;
            }
            SaveSettings();

        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (m_SettingsBeingInitialized) return;

            if (checkBox4.Checked)
            {
                textBoxChan4.Text = "4";
                textBoxChan4.Enabled = true;
            }
            else
            {
                textBoxChan4.Text = UserSettingTags.ChannelNotUsed;
                textBoxChan4.Enabled = false;
            }
            SaveSettings();

        }

      
        private void radioButtonNTSC_CheckedChanged(object sender, EventArgs e)
        {
            if (m_RadioButtonsBeingInitialized) return;

            if (radioButtonNTSC.Checked)
            {
                UserSettings.Set(UserSettingTags.VideoSetup_PAL_NTSC, UserSettingTags.VideoSetup_NTSC);
            }
            else
            {
                UserSettings.Set(UserSettingTags.VideoSetup_PAL_NTSC, UserSettingTags.VideoSetup_PAL);
            }
            m_AppData.LPRServiceControl.ConfigChangedServiceNeedsRestarting = true;

        }

        private void radioButtonPAL_CheckedChanged(object sender, EventArgs e)
        {
            if (m_RadioButtonsBeingInitialized) return;

            if (radioButtonPAL.Checked)
            {
                UserSettings.Set(UserSettingTags.VideoSetup_PAL_NTSC, UserSettingTags.VideoSetup_PAL);
            }
            else
            {
                UserSettings.Set(UserSettingTags.VideoSetup_PAL_NTSC, UserSettingTags.VideoSetup_NTSC);
            }
            m_AppData.LPRServiceControl.ConfigChangedServiceNeedsRestarting = true;
        }
    }
}
