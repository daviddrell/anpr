using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Control_Center
{
    // allow the user to choose login as Admin or Viewer

    public partial class NotLoggedInUC : UserControl
    {
        public NotLoggedInUC(OnChooseLoginAsDel onChoose)
        {
            InitializeComponent();
            OnChooseLoginAs = onChoose;
        }

        public enum LOGIN_AS {ADMIN, VIEWER }
        public delegate void OnChooseLoginAsDel ( LOGIN_AS loginAs);
        OnChooseLoginAsDel OnChooseLoginAs; 

        private void NotLoggedInUC_Load(object sender, EventArgs e)
        {
            this.SuspendLayout();
            PaintNotLoggedIn();
            this.ResumeLayout(false);
        }

        Label NotLoggedIn;
        Button LogInAsAdmin;
        Button LogInAsViewer;

        void PaintNotLoggedIn()
        {
            NotLoggedIn = new Label();
            NotLoggedIn.AutoSize = true;
            NotLoggedIn.Location = new Point(125, 25);
            NotLoggedIn.Size = new Size(150, 13);
            NotLoggedIn.ForeColor = Color.White;
            NotLoggedIn.Text = "You are not currently logged in";
            this.Controls.Add(NotLoggedIn);


            LogInAsAdmin = new Button();
            LogInAsAdmin.BackColor = Color.FromArgb(70, 70, 90);
            LogInAsAdmin.ForeColor = Color.White;
            LogInAsAdmin.Size = new Size(150, 23);
            LogInAsAdmin.Text = "Log in as Administrator";
            LogInAsAdmin.FlatStyle = FlatStyle.Flat;
            LogInAsAdmin.UseVisualStyleBackColor = true;
            LogInAsAdmin.Click += new EventHandler(LogInAsAdmin_Click);
            LogInAsAdmin.Location = new Point(50, 75);
            this.Controls.Add(LogInAsAdmin);

            LogInAsViewer = new Button();
            LogInAsViewer.BackColor = Color.FromArgb(70, 70, 90);
            LogInAsViewer.ForeColor = Color.White;
            LogInAsViewer.Size = new Size(150, 23);
            LogInAsViewer.Text = "Log in as Viewer";
            LogInAsViewer.FlatStyle = FlatStyle.Flat;
            LogInAsViewer.UseVisualStyleBackColor = true;
            LogInAsViewer.Click += new EventHandler(LogInAsViewer_Click);
            LogInAsViewer.Location = new Point(225, 75);
            this.Controls.Add(LogInAsViewer);


        }

        void LogInAsViewer_Click(object sender, EventArgs e)
        {
            OnChooseLoginAs(LOGIN_AS.VIEWER);
        }

        void LogInAsAdmin_Click(object sender, EventArgs e)
        {
            OnChooseLoginAs(LOGIN_AS.ADMIN);
        }
    }
}
