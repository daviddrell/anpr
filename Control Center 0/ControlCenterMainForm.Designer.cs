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
            this.tabPageWorkLocally_Search = new System.Windows.Forms.TabPage();
            this.tabPageWorkLocally_ProcessImages = new System.Windows.Forms.TabPage();
            this.tabPageSelectServer = new System.Windows.Forms.TabPage();
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
            this.tabControlMain.Controls.Add(this.tabPageWorkLocally_Search);
            this.tabControlMain.Controls.Add(this.tabPageWorkLocally_ProcessImages);
            this.tabControlMain.Location = new System.Drawing.Point(1, 0);
            this.tabControlMain.Name = "tabControlMain";
            this.tabControlMain.SelectedIndex = 0;
            this.tabControlMain.Size = new System.Drawing.Size(998, 524);
            this.tabControlMain.TabIndex = 1;
            // 
            // tabPageWorkLocally_Search
            // 
            this.tabPageWorkLocally_Search.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(90)))));
            this.tabPageWorkLocally_Search.Location = new System.Drawing.Point(4, 25);
            this.tabPageWorkLocally_Search.Name = "tabPageWorkLocally_Search";
            this.tabPageWorkLocally_Search.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageWorkLocally_Search.Size = new System.Drawing.Size(990, 495);
            this.tabPageWorkLocally_Search.TabIndex = 0;
            this.tabPageWorkLocally_Search.Text = "Search";
            this.tabPageWorkLocally_Search.UseVisualStyleBackColor = true;
            // 
            // tabPageWorkLocally_ProcessImages
            // 
            this.tabPageWorkLocally_ProcessImages.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(90)))));
            this.tabPageWorkLocally_ProcessImages.Location = new System.Drawing.Point(4, 25);
            this.tabPageWorkLocally_ProcessImages.Name = "tabPageWorkLocally_ProcessImages";
            this.tabPageWorkLocally_ProcessImages.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageWorkLocally_ProcessImages.Size = new System.Drawing.Size(990, 495);
            this.tabPageWorkLocally_ProcessImages.TabIndex = 1;
            this.tabPageWorkLocally_ProcessImages.Text = "Process Images";
            this.tabPageWorkLocally_ProcessImages.UseVisualStyleBackColor = true;
            // 
            // tabPageSelectServer
            // 
            this.tabPageSelectServer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(90)))));
            this.tabPageSelectServer.Location = new System.Drawing.Point(4, 25);
            this.tabPageSelectServer.Name = "tabPageSelectServer";
            this.tabPageSelectServer.Size = new System.Drawing.Size(990, 495);
            this.tabPageSelectServer.TabIndex = 2;
            this.tabPageSelectServer.Text = "Select Server";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(90)))));
            this.ClientSize = new System.Drawing.Size(999, 523);
            this.Controls.Add(this.tabControlMain);
            this.Name = "Form1";
            this.Text = "First Evidence Control Center";
            this.tabControlMain.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControlMain;
        private System.Windows.Forms.TabPage tabPageWorkLocally_Search;
        private System.Windows.Forms.TabPage tabPageWorkLocally_ProcessImages;
        private System.Windows.Forms.TabPage tabPageSelectServer;

    }
}

