namespace ConfigurePSS
{
    partial class ConfigurePSSMainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabPageActivation = new System.Windows.Forms.TabPage();
            this.buttonActivate = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.tabPageService = new System.Windows.Forms.TabPage();
            this.buttonStartStopService = new System.Windows.Forms.Button();
            this.groupBoxServiceStatus = new System.Windows.Forms.GroupBox();
            this.textBoxServiceRunningStatus = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxServiceInstallStatus = new System.Windows.Forms.TextBox();
            this.buttonUninstallService = new System.Windows.Forms.Button();
            this.buttonInstallService = new System.Windows.Forms.Button();
            this.tabPageEnterPasswords = new System.Windows.Forms.TabPage();
            this.groupBoxViewerPassword = new System.Windows.Forms.GroupBox();
            this.buttonSaveViewerPassword = new System.Windows.Forms.Button();
            this.textBoxViewerPasswordVerify = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxViewerPassword = new System.Windows.Forms.TextBox();
            this.groupBoxAdminPassword = new System.Windows.Forms.GroupBox();
            this.buttonSaveAdmin = new System.Windows.Forms.Button();
            this.textBoxAdminPasswordVerify = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxAdminPassword = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPageChannels = new System.Windows.Forms.TabPage();
            this.tabPageConfigureGPS = new System.Windows.Forms.TabPage();
            this.tabPageConfigureEmail = new System.Windows.Forms.TabPage();
            this.tabPageConfigureWatchLists = new System.Windows.Forms.TabPage();
            this.tabControlMain = new System.Windows.Forms.TabControl();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.tabPageActivation.SuspendLayout();
            this.tabPageService.SuspendLayout();
            this.groupBoxServiceStatus.SuspendLayout();
            this.tabPageEnterPasswords.SuspendLayout();
            this.groupBoxViewerPassword.SuspendLayout();
            this.groupBoxAdminPassword.SuspendLayout();
            this.tabPageConfigureGPS.SuspendLayout();
            this.tabControlMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabPageActivation
            // 
            this.tabPageActivation.Controls.Add(this.buttonActivate);
            this.tabPageActivation.Controls.Add(this.textBox1);
            this.tabPageActivation.Location = new System.Drawing.Point(4, 22);
            this.tabPageActivation.Name = "tabPageActivation";
            this.tabPageActivation.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageActivation.Size = new System.Drawing.Size(1240, 613);
            this.tabPageActivation.TabIndex = 7;
            this.tabPageActivation.Text = "License Activation";
            this.tabPageActivation.UseVisualStyleBackColor = true;
            // 
            // buttonActivate
            // 
            this.buttonActivate.Location = new System.Drawing.Point(513, 232);
            this.buttonActivate.Name = "buttonActivate";
            this.buttonActivate.Size = new System.Drawing.Size(121, 23);
            this.buttonActivate.TabIndex = 2;
            this.buttonActivate.Text = "Activate";
            this.buttonActivate.UseVisualStyleBackColor = true;
            this.buttonActivate.Click += new System.EventHandler(this.buttonActivate_Click);
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(334, 87);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(515, 102);
            this.textBox1.TabIndex = 1;
            this.textBox1.Text = "Once you receive your license key file from your software distributor, click here" +
                " to open the key file and activate the license. Note: an Internet connection is " +
                "required for activiation.";
            // 
            // tabPageService
            // 
            this.tabPageService.Controls.Add(this.buttonStartStopService);
            this.tabPageService.Controls.Add(this.groupBoxServiceStatus);
            this.tabPageService.Controls.Add(this.buttonUninstallService);
            this.tabPageService.Controls.Add(this.buttonInstallService);
            this.tabPageService.Location = new System.Drawing.Point(4, 22);
            this.tabPageService.Name = "tabPageService";
            this.tabPageService.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageService.Size = new System.Drawing.Size(1240, 613);
            this.tabPageService.TabIndex = 5;
            this.tabPageService.Text = "LPR Service";
            this.tabPageService.UseVisualStyleBackColor = true;
            this.tabPageService.Click += new System.EventHandler(this.tabPageService_Click);
            // 
            // buttonStartStopService
            // 
            this.buttonStartStopService.Location = new System.Drawing.Point(388, 105);
            this.buttonStartStopService.Name = "buttonStartStopService";
            this.buttonStartStopService.Size = new System.Drawing.Size(151, 23);
            this.buttonStartStopService.TabIndex = 3;
            this.buttonStartStopService.Text = "Start Service";
            this.buttonStartStopService.UseVisualStyleBackColor = true;
            this.buttonStartStopService.Click += new System.EventHandler(this.buttonStartStopService_Click);
            // 
            // groupBoxServiceStatus
            // 
            this.groupBoxServiceStatus.Controls.Add(this.textBoxServiceRunningStatus);
            this.groupBoxServiceStatus.Controls.Add(this.label7);
            this.groupBoxServiceStatus.Controls.Add(this.label6);
            this.groupBoxServiceStatus.Controls.Add(this.textBoxServiceInstallStatus);
            this.groupBoxServiceStatus.Location = new System.Drawing.Point(52, 21);
            this.groupBoxServiceStatus.Name = "groupBoxServiceStatus";
            this.groupBoxServiceStatus.Size = new System.Drawing.Size(292, 130);
            this.groupBoxServiceStatus.TabIndex = 2;
            this.groupBoxServiceStatus.TabStop = false;
            this.groupBoxServiceStatus.Text = "Service Status";
            // 
            // textBoxServiceRunningStatus
            // 
            this.textBoxServiceRunningStatus.Location = new System.Drawing.Point(117, 62);
            this.textBoxServiceRunningStatus.Name = "textBoxServiceRunningStatus";
            this.textBoxServiceRunningStatus.Size = new System.Drawing.Size(141, 20);
            this.textBoxServiceRunningStatus.TabIndex = 3;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(44, 70);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(50, 13);
            this.label7.TabIndex = 2;
            this.label7.Text = "Running:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(41, 29);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(49, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Installed:";
            // 
            // textBoxServiceInstallStatus
            // 
            this.textBoxServiceInstallStatus.Location = new System.Drawing.Point(117, 26);
            this.textBoxServiceInstallStatus.Name = "textBoxServiceInstallStatus";
            this.textBoxServiceInstallStatus.Size = new System.Drawing.Size(141, 20);
            this.textBoxServiceInstallStatus.TabIndex = 0;
            // 
            // buttonUninstallService
            // 
            this.buttonUninstallService.Location = new System.Drawing.Point(388, 76);
            this.buttonUninstallService.Name = "buttonUninstallService";
            this.buttonUninstallService.Size = new System.Drawing.Size(151, 23);
            this.buttonUninstallService.TabIndex = 1;
            this.buttonUninstallService.Text = "Un-Install";
            this.buttonUninstallService.UseVisualStyleBackColor = true;
            this.buttonUninstallService.Click += new System.EventHandler(this.buttonUninstallService_Click);
            // 
            // buttonInstallService
            // 
            this.buttonInstallService.Location = new System.Drawing.Point(388, 47);
            this.buttonInstallService.Name = "buttonInstallService";
            this.buttonInstallService.Size = new System.Drawing.Size(151, 23);
            this.buttonInstallService.TabIndex = 0;
            this.buttonInstallService.Text = "Install";
            this.buttonInstallService.UseVisualStyleBackColor = true;
            this.buttonInstallService.Click += new System.EventHandler(this.buttonInstallService_Click);
            // 
            // tabPageEnterPasswords
            // 
            this.tabPageEnterPasswords.Controls.Add(this.groupBoxViewerPassword);
            this.tabPageEnterPasswords.Controls.Add(this.groupBoxAdminPassword);
            this.tabPageEnterPasswords.Controls.Add(this.label1);
            this.tabPageEnterPasswords.Location = new System.Drawing.Point(4, 22);
            this.tabPageEnterPasswords.Name = "tabPageEnterPasswords";
            this.tabPageEnterPasswords.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageEnterPasswords.Size = new System.Drawing.Size(1240, 613);
            this.tabPageEnterPasswords.TabIndex = 4;
            this.tabPageEnterPasswords.Text = "Passwords";
            this.tabPageEnterPasswords.UseVisualStyleBackColor = true;
            // 
            // groupBoxViewerPassword
            // 
            this.groupBoxViewerPassword.Controls.Add(this.buttonSaveViewerPassword);
            this.groupBoxViewerPassword.Controls.Add(this.textBoxViewerPasswordVerify);
            this.groupBoxViewerPassword.Controls.Add(this.label4);
            this.groupBoxViewerPassword.Controls.Add(this.label5);
            this.groupBoxViewerPassword.Controls.Add(this.textBoxViewerPassword);
            this.groupBoxViewerPassword.Location = new System.Drawing.Point(297, 336);
            this.groupBoxViewerPassword.Name = "groupBoxViewerPassword";
            this.groupBoxViewerPassword.Size = new System.Drawing.Size(321, 183);
            this.groupBoxViewerPassword.TabIndex = 2;
            this.groupBoxViewerPassword.TabStop = false;
            this.groupBoxViewerPassword.Text = "Viewer Password";
            // 
            // buttonSaveViewerPassword
            // 
            this.buttonSaveViewerPassword.Location = new System.Drawing.Point(103, 129);
            this.buttonSaveViewerPassword.Name = "buttonSaveViewerPassword";
            this.buttonSaveViewerPassword.Size = new System.Drawing.Size(129, 23);
            this.buttonSaveViewerPassword.TabIndex = 4;
            this.buttonSaveViewerPassword.Text = "Save";
            this.buttonSaveViewerPassword.UseVisualStyleBackColor = true;
            this.buttonSaveViewerPassword.Click += new System.EventHandler(this.buttonSaveViewerPassword_Click);
            // 
            // textBoxViewerPasswordVerify
            // 
            this.textBoxViewerPasswordVerify.Location = new System.Drawing.Point(147, 78);
            this.textBoxViewerPasswordVerify.Name = "textBoxViewerPasswordVerify";
            this.textBoxViewerPasswordVerify.PasswordChar = '*';
            this.textBoxViewerPasswordVerify.Size = new System.Drawing.Size(141, 20);
            this.textBoxViewerPasswordVerify.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(44, 78);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(85, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Verify Password:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(73, 32);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "Password:";
            // 
            // textBoxViewerPassword
            // 
            this.textBoxViewerPassword.Location = new System.Drawing.Point(147, 32);
            this.textBoxViewerPassword.Name = "textBoxViewerPassword";
            this.textBoxViewerPassword.PasswordChar = '*';
            this.textBoxViewerPassword.Size = new System.Drawing.Size(141, 20);
            this.textBoxViewerPassword.TabIndex = 0;
            // 
            // groupBoxAdminPassword
            // 
            this.groupBoxAdminPassword.Controls.Add(this.buttonSaveAdmin);
            this.groupBoxAdminPassword.Controls.Add(this.textBoxAdminPasswordVerify);
            this.groupBoxAdminPassword.Controls.Add(this.label3);
            this.groupBoxAdminPassword.Controls.Add(this.label2);
            this.groupBoxAdminPassword.Controls.Add(this.textBoxAdminPassword);
            this.groupBoxAdminPassword.Location = new System.Drawing.Point(297, 112);
            this.groupBoxAdminPassword.Name = "groupBoxAdminPassword";
            this.groupBoxAdminPassword.Size = new System.Drawing.Size(321, 183);
            this.groupBoxAdminPassword.TabIndex = 1;
            this.groupBoxAdminPassword.TabStop = false;
            this.groupBoxAdminPassword.Text = "Admin Password";
            // 
            // buttonSaveAdmin
            // 
            this.buttonSaveAdmin.Location = new System.Drawing.Point(103, 129);
            this.buttonSaveAdmin.Name = "buttonSaveAdmin";
            this.buttonSaveAdmin.Size = new System.Drawing.Size(129, 23);
            this.buttonSaveAdmin.TabIndex = 4;
            this.buttonSaveAdmin.Text = "Save";
            this.buttonSaveAdmin.UseVisualStyleBackColor = true;
            this.buttonSaveAdmin.Click += new System.EventHandler(this.buttonSaveAdmin_Click);
            // 
            // textBoxAdminPasswordVerify
            // 
            this.textBoxAdminPasswordVerify.Location = new System.Drawing.Point(147, 78);
            this.textBoxAdminPasswordVerify.Name = "textBoxAdminPasswordVerify";
            this.textBoxAdminPasswordVerify.PasswordChar = '*';
            this.textBoxAdminPasswordVerify.Size = new System.Drawing.Size(141, 20);
            this.textBoxAdminPasswordVerify.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(44, 78);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(85, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Verify Password:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(73, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Password:";
            // 
            // textBoxAdminPassword
            // 
            this.textBoxAdminPassword.Location = new System.Drawing.Point(147, 32);
            this.textBoxAdminPassword.Name = "textBoxAdminPassword";
            this.textBoxAdminPassword.PasswordChar = '*';
            this.textBoxAdminPassword.Size = new System.Drawing.Size(141, 20);
            this.textBoxAdminPassword.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(305, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(174, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Remote Access Passwords";
            // 
            // tabPageChannels
            // 
            this.tabPageChannels.Location = new System.Drawing.Point(4, 22);
            this.tabPageChannels.Name = "tabPageChannels";
            this.tabPageChannels.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageChannels.Size = new System.Drawing.Size(1240, 613);
            this.tabPageChannels.TabIndex = 3;
            this.tabPageChannels.Text = "Source Channels";
            this.tabPageChannels.UseVisualStyleBackColor = true;
            // 
            // tabPageConfigureGPS
            // 
            this.tabPageConfigureGPS.Controls.Add(this.label10);
            this.tabPageConfigureGPS.Controls.Add(this.label9);
            this.tabPageConfigureGPS.Controls.Add(this.label8);
            this.tabPageConfigureGPS.Location = new System.Drawing.Point(4, 22);
            this.tabPageConfigureGPS.Name = "tabPageConfigureGPS";
            this.tabPageConfigureGPS.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageConfigureGPS.Size = new System.Drawing.Size(1240, 613);
            this.tabPageConfigureGPS.TabIndex = 2;
            this.tabPageConfigureGPS.Text = "GPS";
            this.tabPageConfigureGPS.UseVisualStyleBackColor = true;
            this.tabPageConfigureGPS.Click += new System.EventHandler(this.tabPageConfigureGPS_Click);
            // 
            // tabPageConfigureEmail
            // 
            this.tabPageConfigureEmail.Location = new System.Drawing.Point(4, 22);
            this.tabPageConfigureEmail.Name = "tabPageConfigureEmail";
            this.tabPageConfigureEmail.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageConfigureEmail.Size = new System.Drawing.Size(1240, 613);
            this.tabPageConfigureEmail.TabIndex = 1;
            this.tabPageConfigureEmail.Text = "Email";
            this.tabPageConfigureEmail.UseVisualStyleBackColor = true;
            // 
            // tabPageConfigureWatchLists
            // 
            this.tabPageConfigureWatchLists.Location = new System.Drawing.Point(4, 22);
            this.tabPageConfigureWatchLists.Name = "tabPageConfigureWatchLists";
            this.tabPageConfigureWatchLists.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageConfigureWatchLists.Size = new System.Drawing.Size(1240, 613);
            this.tabPageConfigureWatchLists.TabIndex = 0;
            this.tabPageConfigureWatchLists.Text = "Watch Lists";
            this.tabPageConfigureWatchLists.UseVisualStyleBackColor = true;
            // 
            // tabControlMain
            // 
            this.tabControlMain.Controls.Add(this.tabPageChannels);
            this.tabControlMain.Controls.Add(this.tabPageConfigureGPS);
            this.tabControlMain.Controls.Add(this.tabPageConfigureWatchLists);
            this.tabControlMain.Controls.Add(this.tabPageConfigureEmail);
            this.tabControlMain.Controls.Add(this.tabPageEnterPasswords);
            this.tabControlMain.Controls.Add(this.tabPageService);
            this.tabControlMain.Controls.Add(this.tabPageActivation);
            this.tabControlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlMain.Location = new System.Drawing.Point(0, 0);
            this.tabControlMain.Name = "tabControlMain";
            this.tabControlMain.SelectedIndex = 0;
            this.tabControlMain.Size = new System.Drawing.Size(1248, 639);
            this.tabControlMain.TabIndex = 0;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(124, 26);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(77, 13);
            this.label8.TabIndex = 0;
            this.label8.Text = "GPS Setup: ";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(208, 26);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(415, 13);
            this.label9.TabIndex = 1;
            this.label9.Text = "If a GPS receiver is used, no configuration is required. If no receiver is used, " +
                "then enter";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(211, 43);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(369, 13);
            this.label10.TabIndex = 2;
            this.label10.Text = "the fixed site coordinates below, in fractional form or in minutes-seconds form.";
            // 
            // ConfigurePSSMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1248, 639);
            this.Controls.Add(this.tabControlMain);
            this.Name = "ConfigurePSSMainForm";
            this.Text = "LPR Service Control";
            this.tabPageActivation.ResumeLayout(false);
            this.tabPageActivation.PerformLayout();
            this.tabPageService.ResumeLayout(false);
            this.groupBoxServiceStatus.ResumeLayout(false);
            this.groupBoxServiceStatus.PerformLayout();
            this.tabPageEnterPasswords.ResumeLayout(false);
            this.tabPageEnterPasswords.PerformLayout();
            this.groupBoxViewerPassword.ResumeLayout(false);
            this.groupBoxViewerPassword.PerformLayout();
            this.groupBoxAdminPassword.ResumeLayout(false);
            this.groupBoxAdminPassword.PerformLayout();
            this.tabPageConfigureGPS.ResumeLayout(false);
            this.tabPageConfigureGPS.PerformLayout();
            this.tabControlMain.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabPage tabPageActivation;
        private System.Windows.Forms.Button buttonActivate;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TabPage tabPageService;
        private System.Windows.Forms.Button buttonStartStopService;
        private System.Windows.Forms.GroupBox groupBoxServiceStatus;
        private System.Windows.Forms.TextBox textBoxServiceRunningStatus;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxServiceInstallStatus;
        private System.Windows.Forms.Button buttonUninstallService;
        private System.Windows.Forms.Button buttonInstallService;
        private System.Windows.Forms.TabPage tabPageEnterPasswords;
        private System.Windows.Forms.GroupBox groupBoxViewerPassword;
        private System.Windows.Forms.Button buttonSaveViewerPassword;
        private System.Windows.Forms.TextBox textBoxViewerPasswordVerify;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxViewerPassword;
        private System.Windows.Forms.GroupBox groupBoxAdminPassword;
        private System.Windows.Forms.Button buttonSaveAdmin;
        private System.Windows.Forms.TextBox textBoxAdminPasswordVerify;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxAdminPassword;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tabPageChannels;
        private System.Windows.Forms.TabPage tabPageConfigureGPS;
        private System.Windows.Forms.TabPage tabPageConfigureEmail;
        private System.Windows.Forms.TabPage tabPageConfigureWatchLists;
        private System.Windows.Forms.TabControl tabControlMain;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;

    }
}

