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
                if (success)
                {
                    labelStatus.Text = "Sucessfully logged in";

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

        private void buttonLogin_Click(object sender, EventArgs e)
        {
           // C:\Users\David\Documents

         //   string pwenc= Encryption.EncryptText(textBox1.Text);
         //   File.AppendAllText ("C:\\Users\\David\\Documents\\pw.txt",pwenc);

            if (OnPasswordEntered != null) OnPasswordEntered(textBox1.Text);
        }

       
    }
}
