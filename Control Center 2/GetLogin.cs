using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EncryptionLib;

namespace Control_Center
{
    public partial class GetLogin : UserControl
    {
        public GetLogin(OnPasswordEntered passwordEntered, string mainText)
        {

            InitializeComponent();

            m_OnPasswordEntered = passwordEntered;
            labelMainControlText.Text = mainText;
            labelMainControlText.ForeColor = Color.White;

            textBoxFirstPassword.PasswordChar = '*';
           

        }

        public delegate void OnPasswordEntered(string password);
        OnPasswordEntered m_OnPasswordEntered;
       
        private void FirstTimeLogin_Load(object sender, EventArgs e)
        {
            textBoxStatus.Text = "Password not set";
            textBoxStatus.ForeColor = Color.Red;
        }

        private void textBoxFirstPassword_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBoxSecondPassword_TextChanged(object sender, EventArgs e)
        {

        }

        private void buttonCommittPassword_Click(object sender, EventArgs e)
        {
            string pw1 = textBoxFirstPassword.Text;

            textBoxStatus.Text = "Sucessfully entered password";
            textBoxStatus.ForeColor = Color.Green;

            textBoxFirstPassword.Text = "";

            m_OnPasswordEntered(pw1);
            
        }

        private void textBoxStatus_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
