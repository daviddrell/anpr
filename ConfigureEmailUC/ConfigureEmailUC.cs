using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EmailServicesLib;
using ApplicationDataClass;

namespace ConfigureEmailUC
{
    public partial class ConfigureEmailUC : UserControl
    {
        public ConfigureEmailUC(APPLICATION_DATA appData)
        {
            InitializeComponent();
          
            m_AppData = appData;
            m_EmailServices = (EmailServices) m_AppData.EmailServices;
        }
        
        APPLICATION_DATA m_AppData;

        EmailServices m_EmailServices;
       
        private void ConfigureEmailUC_Load(object sender, EventArgs e)
        {
            textBoxPassword.PasswordChar = '*';
            textBoxRetypePassword.PasswordChar = '*';
            m_EmailServices.EmaiSettings.LoadSettings(); // read what was previously written to disk

            PushValuesToTextBoxes();
        }

      
        private void buttonSendTestEmail_Click(object sender, EventArgs e)
        {
            if (!RecoverUserInput()) return;

           

            EmailServices.SEND_MESSAGE message = new EmailServices.SEND_MESSAGE();
            m_EmailServices.EmaiSettings.LoadSettings();// re-load internal config settings that have just been modified and saved to disk

            message.to = m_EmailServices.EmaiSettings.AdminAddress;
            message.from = m_EmailServices.EmaiSettings.FromAddress;
            message.subject = "Test message, sent at " + DateTime.UtcNow.ToString();
            message.body = message.subject + " from System " + m_AppData.ThisComputerName; ;
            message.sendResultCallBack = HandleSendTestResult;

            m_EmailServices.SendMessage(message);

            textBoxTestEmailStatus.Text = "Sent Message....waiting on result";
        }

        private void HandleSendTestResult(EmailServices.SEND_RESULT result)
        {
            this.BeginInvoke((MethodInvoker)delegate { textBoxTestEmailStatus.Text = result.Commentary; });
        }

        private void buttonFinished_Click(object sender, EventArgs e)
        {
            RecoverUserInput();
        }

        void PushValuesToTextBoxes()
        {
            textBoxPassword.Text = m_EmailServices.EmaiSettings.Password;
            textBoxRetypePassword.Text = m_EmailServices.EmaiSettings.Password;
            textBoxUserName.Text = m_EmailServices.EmaiSettings.UserName;
            textBoxAdminEmailAddress.Text = m_EmailServices.EmaiSettings.AdminAddress;
            textBoxFromAddress.Text = m_EmailServices.EmaiSettings.FromAddress;
            textBoxOutBoundServer.Text = m_EmailServices.EmaiSettings.OutBoundServer;
        }

        bool RecoverUserInput()
        {
            if (textBoxPassword.Text != textBoxRetypePassword.Text)
            {
                MessageBox.Show("passwords do not match, please re-enter");
                textBoxPassword.Text = "";
                textBoxRetypePassword.Text = "";
                return false;
            }

            m_AppData.LPRServiceControl.ConfigChangedServiceNeedsRestarting = true;


            // write out the email settings to the config file via the email services class

            m_EmailServices.EmaiSettings.UserName = textBoxUserName.Text;// username must be pushed before password because its used in the password encryption
            m_EmailServices.EmaiSettings.Password = textBoxPassword.Text;
            m_EmailServices.EmaiSettings.AdminAddress = textBoxAdminEmailAddress.Text;
            m_EmailServices.EmaiSettings.FromAddress = textBoxFromAddress.Text;
            m_EmailServices.EmaiSettings.OutBoundServer = textBoxOutBoundServer.Text;

            return (true);
           

        }
    }
}
