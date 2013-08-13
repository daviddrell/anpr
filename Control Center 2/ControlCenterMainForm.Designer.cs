namespace Control_Center
{
    partial class ControlCenterMainForm
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
            this.tabControlMain = new System.Windows.Forms.TabControl();
            this.tabPageSelectServer = new System.Windows.Forms.TabPage();
            this.tabPageViewLive = new System.Windows.Forms.TabPage();
            this.tabControlMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControlMain
            // 
            this.tabControlMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControlMain.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tabControlMain.Controls.Add(this.tabPageSelectServer);
            this.tabControlMain.Controls.Add(this.tabPageViewLive);
            this.tabControlMain.Location = new System.Drawing.Point(1, 0);
            this.tabControlMain.Name = "tabControlMain";
            this.tabControlMain.SelectedIndex = 0;
            this.tabControlMain.Size = new System.Drawing.Size(1166, 638);
            this.tabControlMain.TabIndex = 1;
            // 
            // tabPageSelectServer
            // 
            this.tabPageSelectServer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(90)))));
            this.tabPageSelectServer.Location = new System.Drawing.Point(4, 25);
            this.tabPageSelectServer.Name = "tabPageSelectServer";
            this.tabPageSelectServer.Size = new System.Drawing.Size(1158, 609);
            this.tabPageSelectServer.TabIndex = 2;
            this.tabPageSelectServer.Text = "Select Server";
            this.tabPageSelectServer.UseVisualStyleBackColor = true;
            // 
            // tabPageViewLive
            // 
            this.tabPageViewLive.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(90)))));
            this.tabPageViewLive.Location = new System.Drawing.Point(4, 25);
            this.tabPageViewLive.Name = "tabPageViewLive";
            this.tabPageViewLive.Size = new System.Drawing.Size(1158, 609);
            this.tabPageViewLive.TabIndex = 3;
            this.tabPageViewLive.Text = "View Live";
            this.tabPageViewLive.Click += new System.EventHandler(this.tabPageViewLive_Click);
            // 
            // ControlCenterMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(90)))));
            this.ClientSize = new System.Drawing.Size(1168, 639);
            this.Controls.Add(this.tabControlMain);
            this.Name = "ControlCenterMainForm";
            this.Text = "First Evidence PSS Monitor";
            this.tabControlMain.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControlMain;
        private System.Windows.Forms.TabPage tabPageSelectServer;
        private System.Windows.Forms.TabPage tabPageViewLive;

    }
}

