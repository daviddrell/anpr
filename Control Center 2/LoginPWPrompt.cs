using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EncryptionLib;
using System.IO;


namespace Control_Center
{
    public partial class LoginPWPrompt : Form
    {
        public LoginPWPrompt(string title)
        {
            InitializeComponent();

            this.Text = title;

            textBox1.PasswordChar = '*';

            labelStatus.Text = "Not logged in";

            this.FormClosing += new FormClosingEventHandler(LoginPWPrompt_FormClosing);
        }

        void LoginPWPrompt_FormClosing(object sender, FormClosingEventArgs e)
        {
           //breakpoint
            CloseReason r = e.CloseReason;
        }

        public delegate void OnPasswordEnteredEvent(string pw);
        public OnPasswordEnteredEvent OnPasswordEntered; 

        public void PasswordValidationResult (bool success)
        {
            if (labelStatus.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate { this.PasswordValidationResult(success); });
            }
            else
            {
                PasswordSubmitted = false;

                if (success)
                {
                    labelStatus.Text = "Sucessfully logged in";
                    buttonLogin.Enabled = false;
                    buttonLogin.Visible = false;

                }
                else
                {
                    labelStatus.Text = "password not correct";
                    textBox1.Text = "";
                }
            }
        }

        private void LoginPWPrompt_Load(object sender, EventArgs e)
        {

        }

        bool PasswordSubmitted = false;
        private void buttonLogin_Click(object sender, EventArgs e)
        {
            if (PasswordSubmitted) return; // do not allow more than one click until the result gets back.
            PasswordSubmitted = true;
          

            if (OnPasswordEntered != null)
            {

                this.BeginInvoke((MethodInvoker)delegate { this.OnPasswordEntered((string)textBox1.Text.Clone()); });
            }
        }

       
    }
}
