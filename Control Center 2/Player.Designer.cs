namespace Control_Center
{
    partial class Player
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
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.dateTimeStartTime = new System.Windows.Forms.DateTimePicker();
            this.dateTimeEndTime = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonPlay = new System.Windows.Forms.Button();
            this.pictureBoxMainPlayer = new System.Windows.Forms.PictureBox();
            this.listBoxSelectPSS = new System.Windows.Forms.ListBox();
            this.listBoxSelectImageStore = new System.Windows.Forms.ListBox();
            this.listBoxSelectSource = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.labelNumberOfImagesFound = new System.Windows.Forms.Label();
            this.buttonLoadImages = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.labelCurrentFrame = new System.Windows.Forms.Label();
            this.labelPlayTime = new System.Windows.Forms.Label();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.label9 = new System.Windows.Forms.Label();
            this.labelCurrentTime = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMainPlayer)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.SuspendLayout();
            // 
            // dateTimeStartTime
            // 
            this.dateTimeStartTime.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimeStartTime.Location = new System.Drawing.Point(278, 55);
            this.dateTimeStartTime.Name = "dateTimeStartTime";
            this.dateTimeStartTime.Size = new System.Drawing.Size(200, 20);
            this.dateTimeStartTime.TabIndex = 17;
            this.dateTimeStartTime.ValueChanged += new System.EventHandler(this.dateTimeStartTime_ValueChanged);
            // 
            // dateTimeEndTime
            // 
            this.dateTimeEndTime.Location = new System.Drawing.Point(498, 54);
            this.dateTimeEndTime.Name = "dateTimeEndTime";
            this.dateTimeEndTime.Size = new System.Drawing.Size(200, 20);
            this.dateTimeEndTime.TabIndex = 18;
            this.dateTimeEndTime.ValueChanged += new System.EventHandler(this.dateTimeEndTime_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(278, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 19;
            this.label1.Text = "Start Time";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(495, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 13);
            this.label2.TabIndex = 20;
            this.label2.Text = "End Time";
            // 
            // buttonPlay
            // 
            this.buttonPlay.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonPlay.ForeColor = System.Drawing.Color.White;
            this.buttonPlay.Location = new System.Drawing.Point(281, 139);
            this.buttonPlay.Name = "buttonPlay";
            this.buttonPlay.Size = new System.Drawing.Size(96, 23);
            this.buttonPlay.TabIndex = 21;
            this.buttonPlay.Text = "Play";
            this.buttonPlay.UseVisualStyleBackColor = true;
            this.buttonPlay.Click += new System.EventHandler(this.buttonPlay_Click);
            // 
            // pictureBoxMainPlayer
            // 
            this.pictureBoxMainPlayer.BackColor = System.Drawing.Color.Black;
            this.pictureBoxMainPlayer.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBoxMainPlayer.Location = new System.Drawing.Point(281, 168);
            this.pictureBoxMainPlayer.Name = "pictureBoxMainPlayer";
            this.pictureBoxMainPlayer.Size = new System.Drawing.Size(417, 320);
            this.pictureBoxMainPlayer.TabIndex = 22;
            this.pictureBoxMainPlayer.TabStop = false;
            // 
            // listBoxSelectPSS
            // 
            this.listBoxSelectPSS.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.listBoxSelectPSS.ForeColor = System.Drawing.Color.White;
            this.listBoxSelectPSS.FormattingEnabled = true;
            this.listBoxSelectPSS.Location = new System.Drawing.Point(12, 175);
            this.listBoxSelectPSS.Name = "listBoxSelectPSS";
            this.listBoxSelectPSS.Size = new System.Drawing.Size(228, 108);
            this.listBoxSelectPSS.TabIndex = 23;
            // 
            // listBoxSelectImageStore
            // 
            this.listBoxSelectImageStore.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.listBoxSelectImageStore.ForeColor = System.Drawing.Color.White;
            this.listBoxSelectImageStore.FormattingEnabled = true;
            this.listBoxSelectImageStore.Location = new System.Drawing.Point(12, 30);
            this.listBoxSelectImageStore.Name = "listBoxSelectImageStore";
            this.listBoxSelectImageStore.Size = new System.Drawing.Size(228, 108);
            this.listBoxSelectImageStore.TabIndex = 24;
            // 
            // listBoxSelectSource
            // 
            this.listBoxSelectSource.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(90)))));
            this.listBoxSelectSource.ForeColor = System.Drawing.Color.White;
            this.listBoxSelectSource.FormattingEnabled = true;
            this.listBoxSelectSource.Location = new System.Drawing.Point(12, 322);
            this.listBoxSelectSource.Name = "listBoxSelectSource";
            this.listBoxSelectSource.Size = new System.Drawing.Size(228, 108);
            this.listBoxSelectSource.TabIndex = 25;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(12, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(97, 13);
            this.label3.TabIndex = 26;
            this.label3.Text = "Select Image Store";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(12, 159);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(61, 13);
            this.label4.TabIndex = 27;
            this.label4.Text = "Select PSS";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.White;
            this.label5.Location = new System.Drawing.Point(12, 306);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(113, 13);
            this.label5.TabIndex = 28;
            this.label5.Text = "Select Camera Source";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(278, 123);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(187, 13);
            this.label6.TabIndex = 29;
            this.label6.Text = "Number of images found in time frame:";
            // 
            // labelNumberOfImagesFound
            // 
            this.labelNumberOfImagesFound.AutoSize = true;
            this.labelNumberOfImagesFound.ForeColor = System.Drawing.Color.White;
            this.labelNumberOfImagesFound.Location = new System.Drawing.Point(482, 123);
            this.labelNumberOfImagesFound.Name = "labelNumberOfImagesFound";
            this.labelNumberOfImagesFound.Size = new System.Drawing.Size(13, 13);
            this.labelNumberOfImagesFound.TabIndex = 30;
            this.labelNumberOfImagesFound.Text = "0";
            // 
            // buttonLoadImages
            // 
            this.buttonLoadImages.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonLoadImages.ForeColor = System.Drawing.Color.White;
            this.buttonLoadImages.Location = new System.Drawing.Point(278, 81);
            this.buttonLoadImages.Name = "buttonLoadImages";
            this.buttonLoadImages.Size = new System.Drawing.Size(96, 23);
            this.buttonLoadImages.TabIndex = 31;
            this.buttonLoadImages.Text = "Load Images";
            this.buttonLoadImages.UseVisualStyleBackColor = true;
            this.buttonLoadImages.Click += new System.EventHandler(this.buttonLoadImages_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.Color.White;
            this.label7.Location = new System.Drawing.Point(278, 491);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(79, 13);
            this.label7.TabIndex = 32;
            this.label7.Text = "Current Frame: ";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.ForeColor = System.Drawing.Color.White;
            this.label8.Location = new System.Drawing.Point(278, 513);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(52, 13);
            this.label8.TabIndex = 33;
            this.label8.Text = "Play time:";
            // 
            // labelCurrentFrame
            // 
            this.labelCurrentFrame.AutoSize = true;
            this.labelCurrentFrame.ForeColor = System.Drawing.Color.White;
            this.labelCurrentFrame.Location = new System.Drawing.Point(363, 491);
            this.labelCurrentFrame.Name = "labelCurrentFrame";
            this.labelCurrentFrame.Size = new System.Drawing.Size(13, 13);
            this.labelCurrentFrame.TabIndex = 34;
            this.labelCurrentFrame.Text = "_";
            // 
            // labelPlayTime
            // 
            this.labelPlayTime.AutoSize = true;
            this.labelPlayTime.ForeColor = System.Drawing.Color.White;
            this.labelPlayTime.Location = new System.Drawing.Point(364, 513);
            this.labelPlayTime.Name = "labelPlayTime";
            this.labelPlayTime.Size = new System.Drawing.Size(13, 13);
            this.labelPlayTime.TabIndex = 35;
            this.labelPlayTime.Text = "_";
            // 
            // trackBar1
            // 
            this.trackBar1.Location = new System.Drawing.Point(281, 544);
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(417, 45);
            this.trackBar1.TabIndex = 36;
            this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.ForeColor = System.Drawing.Color.White;
            this.label9.Location = new System.Drawing.Point(278, 12);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(70, 13);
            this.label9.TabIndex = 37;
            this.label9.Text = "Current Time:";
            // 
            // labelCurrentTime
            // 
            this.labelCurrentTime.AutoSize = true;
            this.labelCurrentTime.ForeColor = System.Drawing.Color.White;
            this.labelCurrentTime.Location = new System.Drawing.Point(363, 12);
            this.labelCurrentTime.Name = "labelCurrentTime";
            this.labelCurrentTime.Size = new System.Drawing.Size(13, 13);
            this.labelCurrentTime.TabIndex = 38;
            this.labelCurrentTime.Text = "_";
            // 
            // Player
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(90)))));
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
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.listBoxSelectSource);
            this.Controls.Add(this.listBoxSelectImageStore);
            this.Controls.Add(this.listBoxSelectPSS);
            this.Controls.Add(this.pictureBoxMainPlayer);
            this.Controls.Add(this.buttonPlay);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dateTimeEndTime);
            this.Controls.Add(this.dateTimeStartTime);
            this.Name = "Player";
            this.Size = new System.Drawing.Size(720, 631);
            this.Load += new System.EventHandler(this.Player_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxMainPlayer)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.DateTimePicker dateTimeStartTime;
        private System.Windows.Forms.DateTimePicker dateTimeEndTime;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonPlay;
        private System.Windows.Forms.PictureBox pictureBoxMainPlayer;
        private System.Windows.Forms.ListBox listBoxSelectPSS;
        private System.Windows.Forms.ListBox listBoxSelectImageStore;
        private System.Windows.Forms.ListBox listBoxSelectSource;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label labelNumberOfImagesFound;
        private System.Windows.Forms.Button buttonLoadImages;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label labelCurrentFrame;
        private System.Windows.Forms.Label labelPlayTime;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label labelCurrentTime;

    }
}
