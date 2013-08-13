namespace VideoPlayerUC
{
    partial class VideoPlayer
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.labelCurrentTime = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.labelPlayTime = new System.Windows.Forms.Label();
            this.labelCurrentFrame = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.buttonLoadImages = new System.Windows.Forms.Button();
            this.labelNumberOfImagesFound = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.pictureBoxMainPlayer = new System.Windows.Forms.PictureBox();
            this.buttonPlay = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.dateTimeEndTime = new System.Windows.Forms.DateTimePicker();
            this.dateTimeStartTime = new System.Windows.Forms.DateTimePicker();
            this.buttonBackOneFrame = new System.Windows.Forms.Button();
            this.buttonForwardOneFrame = new System.Windows.Forms.Button();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.buttonExportRange = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMainPlayer)).BeginInit();
            this.SuspendLayout();
            // 
            // labelCurrentTime
            // 
            this.labelCurrentTime.AutoSize = true;
            this.labelCurrentTime.ForeColor = System.Drawing.Color.Black;
            this.labelCurrentTime.Location = new System.Drawing.Point(127, 17);
            this.labelCurrentTime.Name = "labelCurrentTime";
            this.labelCurrentTime.Size = new System.Drawing.Size(13, 13);
            this.labelCurrentTime.TabIndex = 54;
            this.labelCurrentTime.Text = "_";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.ForeColor = System.Drawing.Color.Black;
            this.label9.Location = new System.Drawing.Point(18, 17);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(101, 13);
            this.label9.TabIndex = 53;
            this.label9.Text = "Current Time (UTC):";
            // 
            // trackBar1
            // 
            this.trackBar1.Location = new System.Drawing.Point(21, 549);
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(417, 45);
            this.trackBar1.TabIndex = 52;
            this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll_1);
            // 
            // labelPlayTime
            // 
            this.labelPlayTime.AutoSize = true;
            this.labelPlayTime.ForeColor = System.Drawing.Color.Black;
            this.labelPlayTime.Location = new System.Drawing.Point(104, 518);
            this.labelPlayTime.Name = "labelPlayTime";
            this.labelPlayTime.Size = new System.Drawing.Size(13, 13);
            this.labelPlayTime.TabIndex = 51;
            this.labelPlayTime.Text = "_";
            // 
            // labelCurrentFrame
            // 
            this.labelCurrentFrame.AutoSize = true;
            this.labelCurrentFrame.ForeColor = System.Drawing.Color.Black;
            this.labelCurrentFrame.Location = new System.Drawing.Point(103, 496);
            this.labelCurrentFrame.Name = "labelCurrentFrame";
            this.labelCurrentFrame.Size = new System.Drawing.Size(13, 13);
            this.labelCurrentFrame.TabIndex = 50;
            this.labelCurrentFrame.Text = "_";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.ForeColor = System.Drawing.Color.Black;
            this.label8.Location = new System.Drawing.Point(18, 518);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(52, 13);
            this.label8.TabIndex = 49;
            this.label8.Text = "Play time:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.Color.Black;
            this.label7.Location = new System.Drawing.Point(18, 496);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(79, 13);
            this.label7.TabIndex = 48;
            this.label7.Text = "Current Frame: ";
            // 
            // buttonLoadImages
            // 
            this.buttonLoadImages.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonLoadImages.ForeColor = System.Drawing.Color.Black;
            this.buttonLoadImages.Location = new System.Drawing.Point(18, 86);
            this.buttonLoadImages.Name = "buttonLoadImages";
            this.buttonLoadImages.Size = new System.Drawing.Size(96, 23);
            this.buttonLoadImages.TabIndex = 47;
            this.buttonLoadImages.Text = "Load Images";
            this.buttonLoadImages.UseVisualStyleBackColor = true;
            this.buttonLoadImages.Click += new System.EventHandler(this.buttonLoadImages_Click);
            // 
            // labelNumberOfImagesFound
            // 
            this.labelNumberOfImagesFound.AutoSize = true;
            this.labelNumberOfImagesFound.ForeColor = System.Drawing.Color.Black;
            this.labelNumberOfImagesFound.Location = new System.Drawing.Point(222, 128);
            this.labelNumberOfImagesFound.Name = "labelNumberOfImagesFound";
            this.labelNumberOfImagesFound.Size = new System.Drawing.Size(13, 13);
            this.labelNumberOfImagesFound.TabIndex = 46;
            this.labelNumberOfImagesFound.Text = "0";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.Color.Black;
            this.label6.Location = new System.Drawing.Point(18, 128);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(187, 13);
            this.label6.TabIndex = 45;
            this.label6.Text = "Number of images found in time frame:";
            // 
            // pictureBoxMainPlayer
            // 
            this.pictureBoxMainPlayer.BackColor = System.Drawing.Color.Black;
            this.pictureBoxMainPlayer.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBoxMainPlayer.Location = new System.Drawing.Point(21, 173);
            this.pictureBoxMainPlayer.Name = "pictureBoxMainPlayer";
            this.pictureBoxMainPlayer.Size = new System.Drawing.Size(417, 320);
            this.pictureBoxMainPlayer.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxMainPlayer.TabIndex = 44;
            this.pictureBoxMainPlayer.TabStop = false;
            this.pictureBoxMainPlayer.Click += new System.EventHandler(this.pictureBoxMainPlayer_Click);
            // 
            // buttonPlay
            // 
            this.buttonPlay.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonPlay.ForeColor = System.Drawing.Color.Black;
            this.buttonPlay.Location = new System.Drawing.Point(21, 144);
            this.buttonPlay.Name = "buttonPlay";
            this.buttonPlay.Size = new System.Drawing.Size(96, 23);
            this.buttonPlay.TabIndex = 43;
            this.buttonPlay.Text = "Play";
            this.buttonPlay.UseVisualStyleBackColor = true;
            this.buttonPlay.Click += new System.EventHandler(this.buttonPlay_Click_1);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(235, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 13);
            this.label2.TabIndex = 42;
            this.label2.Text = "End Time (UTC)";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(18, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 13);
            this.label1.TabIndex = 41;
            this.label1.Text = "Start Time (UTC)";
            // 
            // dateTimeEndTime
            // 
            this.dateTimeEndTime.Location = new System.Drawing.Point(238, 59);
            this.dateTimeEndTime.Name = "dateTimeEndTime";
            this.dateTimeEndTime.Size = new System.Drawing.Size(200, 20);
            this.dateTimeEndTime.TabIndex = 40;
            // 
            // dateTimeStartTime
            // 
            this.dateTimeStartTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimeStartTime.Location = new System.Drawing.Point(18, 60);
            this.dateTimeStartTime.Name = "dateTimeStartTime";
            this.dateTimeStartTime.Size = new System.Drawing.Size(200, 20);
            this.dateTimeStartTime.TabIndex = 39;
            // 
            // buttonBackOneFrame
            // 
            this.buttonBackOneFrame.Location = new System.Drawing.Point(144, 600);
            this.buttonBackOneFrame.Name = "buttonBackOneFrame";
            this.buttonBackOneFrame.Size = new System.Drawing.Size(75, 23);
            this.buttonBackOneFrame.TabIndex = 55;
            this.buttonBackOneFrame.Text = "<(1)";
            this.buttonBackOneFrame.UseVisualStyleBackColor = true;
            this.buttonBackOneFrame.Click += new System.EventHandler(this.buttonBackOneFrame_Click);
            // 
            // buttonForwardOneFrame
            // 
            this.buttonForwardOneFrame.Location = new System.Drawing.Point(225, 600);
            this.buttonForwardOneFrame.Name = "buttonForwardOneFrame";
            this.buttonForwardOneFrame.Size = new System.Drawing.Size(75, 23);
            this.buttonForwardOneFrame.TabIndex = 56;
            this.buttonForwardOneFrame.Text = "(1)>";
            this.buttonForwardOneFrame.UseVisualStyleBackColor = true;
            this.buttonForwardOneFrame.Click += new System.EventHandler(this.buttonForwardOneFrame_Click);
            // 
            // buttonExportRange
            // 
            this.buttonExportRange.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonExportRange.ForeColor = System.Drawing.Color.Black;
            this.buttonExportRange.Location = new System.Drawing.Point(323, 144);
            this.buttonExportRange.Name = "buttonExportRange";
            this.buttonExportRange.Size = new System.Drawing.Size(115, 23);
            this.buttonExportRange.TabIndex = 57;
            this.buttonExportRange.Text = "Export Range";
            this.buttonExportRange.UseVisualStyleBackColor = true;
            this.buttonExportRange.Click += new System.EventHandler(this.buttonExportRange_Click);
            // 
            // VideoPlayer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.buttonExportRange);
            this.Controls.Add(this.buttonForwardOneFrame);
            this.Controls.Add(this.buttonBackOneFrame);
            this.Controls.Add(this.labelCurrentTime);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.trackBar1);
            this.Controls.Add(this.labelPlayTime);
            this.Controls.Add(this.labelCurrentFrame);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.buttonLoadImages);
            this.Controls.Add(this.labelNumberOfImagesFound);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.pictureBoxMainPlayer);
            this.Controls.Add(this.buttonPlay);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dateTimeEndTime);
            this.Controls.Add(this.dateTimeStartTime);
            this.Name = "VideoPlayer";
            this.Size = new System.Drawing.Size(472, 689);
            this.Load += new System.EventHandler(this.VideoPlayer_Load);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMainPlayer)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelCurrentTime;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.Label labelPlayTime;
        private System.Windows.Forms.Label labelCurrentFrame;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button buttonLoadImages;
        private System.Windows.Forms.Label labelNumberOfImagesFound;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.PictureBox pictureBoxMainPlayer;
        private System.Windows.Forms.Button buttonPlay;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dateTimeEndTime;
        private System.Windows.Forms.DateTimePicker dateTimeStartTime;
        private System.Windows.Forms.Button buttonBackOneFrame;
        private System.Windows.Forms.Button buttonForwardOneFrame;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Button buttonExportRange;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
    }
}
