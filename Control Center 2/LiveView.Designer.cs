namespace Control_Center
{
    partial class LiveView
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
            this.comboBoxSourceChannelList = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxCurrentRemoteServer = new System.Windows.Forms.TextBox();
            this.pictureBoxVideoDisplay0 = new System.Windows.Forms.PictureBox();
            this.buttonPlayPause = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.labelDisplayFrameTime0 = new System.Windows.Forms.Label();
            this.labelPlateReadings0 = new System.Windows.Forms.Label();
            this.pictureBoxVideoDisplay1 = new System.Windows.Forms.PictureBox();
            this.pictureBoxVideoDisplay2 = new System.Windows.Forms.PictureBox();
            this.pictureBoxVideoDisplay3 = new System.Windows.Forms.PictureBox();
            this.labelPlateReadings1 = new System.Windows.Forms.Label();
            this.labelDisplayFrameTime1 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.labelPlateReadings2 = new System.Windows.Forms.Label();
            this.labelDisplayFrameTime2 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.labelPlateReadings3 = new System.Windows.Forms.Label();
            this.labelDisplayFrameTime3 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxVideoDisplay0)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxVideoDisplay1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxVideoDisplay2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxVideoDisplay3)).BeginInit();
            this.SuspendLayout();
            // 
            // comboBoxSourceChannelList
            // 
            this.comboBoxSourceChannelList.FormattingEnabled = true;
            this.comboBoxSourceChannelList.Location = new System.Drawing.Point(126, 38);
            this.comboBoxSourceChannelList.Name = "comboBoxSourceChannelList";
            this.comboBoxSourceChannelList.Size = new System.Drawing.Size(257, 21);
            this.comboBoxSourceChannelList.TabIndex = 9;
            this.comboBoxSourceChannelList.SelectedIndexChanged += new System.EventHandler(this.comboBoxSourceChannelList_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(20, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(86, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Source Channel:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(20, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Current Server:";
            // 
            // textBoxCurrentRemoteServer
            // 
            this.textBoxCurrentRemoteServer.Location = new System.Drawing.Point(126, 11);
            this.textBoxCurrentRemoteServer.Name = "textBoxCurrentRemoteServer";
            this.textBoxCurrentRemoteServer.Size = new System.Drawing.Size(257, 20);
            this.textBoxCurrentRemoteServer.TabIndex = 6;
            // 
            // pictureBoxVideoDisplay0
            // 
            this.pictureBoxVideoDisplay0.BackColor = System.Drawing.Color.Black;
            this.pictureBoxVideoDisplay0.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBoxVideoDisplay0.Location = new System.Drawing.Point(11, 89);
            this.pictureBoxVideoDisplay0.Name = "pictureBoxVideoDisplay0";
            this.pictureBoxVideoDisplay0.Size = new System.Drawing.Size(229, 184);
            this.pictureBoxVideoDisplay0.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxVideoDisplay0.TabIndex = 10;
            this.pictureBoxVideoDisplay0.TabStop = false;
            // 
            // buttonPlayPause
            // 
            this.buttonPlayPause.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonPlayPause.ForeColor = System.Drawing.Color.White;
            this.buttonPlayPause.Location = new System.Drawing.Point(399, 38);
            this.buttonPlayPause.Name = "buttonPlayPause";
            this.buttonPlayPause.Size = new System.Drawing.Size(63, 23);
            this.buttonPlayPause.TabIndex = 11;
            this.buttonPlayPause.Text = "Play";
            this.buttonPlayPause.UseVisualStyleBackColor = true;
            this.buttonPlayPause.Click += new System.EventHandler(this.buttonPlayPause_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(27, 287);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Frame Time:";
            // 
            // labelDisplayFrameTime0
            // 
            this.labelDisplayFrameTime0.AutoSize = true;
            this.labelDisplayFrameTime0.ForeColor = System.Drawing.Color.White;
            this.labelDisplayFrameTime0.Location = new System.Drawing.Point(98, 287);
            this.labelDisplayFrameTime0.Name = "labelDisplayFrameTime0";
            this.labelDisplayFrameTime0.Size = new System.Drawing.Size(13, 13);
            this.labelDisplayFrameTime0.TabIndex = 13;
            this.labelDisplayFrameTime0.Text = "0";
            // 
            // labelPlateReadings0
            // 
            this.labelPlateReadings0.AutoSize = true;
            this.labelPlateReadings0.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPlateReadings0.ForeColor = System.Drawing.Color.White;
            this.labelPlateReadings0.Location = new System.Drawing.Point(19, 309);
            this.labelPlateReadings0.Name = "labelPlateReadings0";
            this.labelPlateReadings0.Size = new System.Drawing.Size(18, 20);
            this.labelPlateReadings0.TabIndex = 14;
            this.labelPlateReadings0.Text = "_";
            // 
            // pictureBoxVideoDisplay1
            // 
            this.pictureBoxVideoDisplay1.BackColor = System.Drawing.Color.Black;
            this.pictureBoxVideoDisplay1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBoxVideoDisplay1.Location = new System.Drawing.Point(254, 89);
            this.pictureBoxVideoDisplay1.Name = "pictureBoxVideoDisplay1";
            this.pictureBoxVideoDisplay1.Size = new System.Drawing.Size(229, 184);
            this.pictureBoxVideoDisplay1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxVideoDisplay1.TabIndex = 15;
            this.pictureBoxVideoDisplay1.TabStop = false;
            // 
            // pictureBoxVideoDisplay2
            // 
            this.pictureBoxVideoDisplay2.BackColor = System.Drawing.Color.Black;
            this.pictureBoxVideoDisplay2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBoxVideoDisplay2.Location = new System.Drawing.Point(497, 89);
            this.pictureBoxVideoDisplay2.Name = "pictureBoxVideoDisplay2";
            this.pictureBoxVideoDisplay2.Size = new System.Drawing.Size(229, 184);
            this.pictureBoxVideoDisplay2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxVideoDisplay2.TabIndex = 16;
            this.pictureBoxVideoDisplay2.TabStop = false;
            // 
            // pictureBoxVideoDisplay3
            // 
            this.pictureBoxVideoDisplay3.BackColor = System.Drawing.Color.Black;
            this.pictureBoxVideoDisplay3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBoxVideoDisplay3.Location = new System.Drawing.Point(742, 89);
            this.pictureBoxVideoDisplay3.Name = "pictureBoxVideoDisplay3";
            this.pictureBoxVideoDisplay3.Size = new System.Drawing.Size(229, 184);
            this.pictureBoxVideoDisplay3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxVideoDisplay3.TabIndex = 17;
            this.pictureBoxVideoDisplay3.TabStop = false;
            // 
            // labelPlateReadings1
            // 
            this.labelPlateReadings1.AutoSize = true;
            this.labelPlateReadings1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPlateReadings1.ForeColor = System.Drawing.Color.White;
            this.labelPlateReadings1.Location = new System.Drawing.Point(256, 309);
            this.labelPlateReadings1.Name = "labelPlateReadings1";
            this.labelPlateReadings1.Size = new System.Drawing.Size(18, 20);
            this.labelPlateReadings1.TabIndex = 20;
            this.labelPlateReadings1.Text = "_";
            // 
            // labelDisplayFrameTime1
            // 
            this.labelDisplayFrameTime1.AutoSize = true;
            this.labelDisplayFrameTime1.ForeColor = System.Drawing.Color.White;
            this.labelDisplayFrameTime1.Location = new System.Drawing.Point(340, 287);
            this.labelDisplayFrameTime1.Name = "labelDisplayFrameTime1";
            this.labelDisplayFrameTime1.Size = new System.Drawing.Size(13, 13);
            this.labelDisplayFrameTime1.TabIndex = 19;
            this.labelDisplayFrameTime1.Text = "0";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.Color.White;
            this.label6.Location = new System.Drawing.Point(269, 287);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 13);
            this.label6.TabIndex = 18;
            this.label6.Text = "Frame Time:";
            // 
            // labelPlateReadings2
            // 
            this.labelPlateReadings2.AutoSize = true;
            this.labelPlateReadings2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPlateReadings2.ForeColor = System.Drawing.Color.White;
            this.labelPlateReadings2.Location = new System.Drawing.Point(501, 311);
            this.labelPlateReadings2.Name = "labelPlateReadings2";
            this.labelPlateReadings2.Size = new System.Drawing.Size(18, 20);
            this.labelPlateReadings2.TabIndex = 23;
            this.labelPlateReadings2.Text = "_";
            // 
            // labelDisplayFrameTime2
            // 
            this.labelDisplayFrameTime2.AutoSize = true;
            this.labelDisplayFrameTime2.ForeColor = System.Drawing.Color.White;
            this.labelDisplayFrameTime2.Location = new System.Drawing.Point(581, 287);
            this.labelDisplayFrameTime2.Name = "labelDisplayFrameTime2";
            this.labelDisplayFrameTime2.Size = new System.Drawing.Size(13, 13);
            this.labelDisplayFrameTime2.TabIndex = 22;
            this.labelDisplayFrameTime2.Text = "0";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.ForeColor = System.Drawing.Color.White;
            this.label9.Location = new System.Drawing.Point(510, 287);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(65, 13);
            this.label9.TabIndex = 21;
            this.label9.Text = "Frame Time:";
            // 
            // labelPlateReadings3
            // 
            this.labelPlateReadings3.AutoSize = true;
            this.labelPlateReadings3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPlateReadings3.ForeColor = System.Drawing.Color.White;
            this.labelPlateReadings3.Location = new System.Drawing.Point(748, 304);
            this.labelPlateReadings3.Name = "labelPlateReadings3";
            this.labelPlateReadings3.Size = new System.Drawing.Size(18, 20);
            this.labelPlateReadings3.TabIndex = 26;
            this.labelPlateReadings3.Text = "_";
            this.labelPlateReadings3.Click += new System.EventHandler(this.label10_Click);
            // 
            // labelDisplayFrameTime3
            // 
            this.labelDisplayFrameTime3.AutoSize = true;
            this.labelDisplayFrameTime3.ForeColor = System.Drawing.Color.White;
            this.labelDisplayFrameTime3.Location = new System.Drawing.Point(835, 287);
            this.labelDisplayFrameTime3.Name = "labelDisplayFrameTime3";
            this.labelDisplayFrameTime3.Size = new System.Drawing.Size(13, 13);
            this.labelDisplayFrameTime3.TabIndex = 25;
            this.labelDisplayFrameTime3.Text = "0";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.ForeColor = System.Drawing.Color.White;
            this.label12.Location = new System.Drawing.Point(764, 287);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(65, 13);
            this.label12.TabIndex = 24;
            this.label12.Text = "Frame Time:";
            // 
            // LiveView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(90)))));
            this.Controls.Add(this.labelPlateReadings3);
            this.Controls.Add(this.labelDisplayFrameTime3);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.labelPlateReadings2);
            this.Controls.Add(this.labelDisplayFrameTime2);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.labelPlateReadings1);
            this.Controls.Add(this.labelDisplayFrameTime1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.pictureBoxVideoDisplay3);
            this.Controls.Add(this.pictureBoxVideoDisplay2);
            this.Controls.Add(this.pictureBoxVideoDisplay1);
            this.Controls.Add(this.labelPlateReadings0);
            this.Controls.Add(this.labelDisplayFrameTime0);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.buttonPlayPause);
            this.Controls.Add(this.pictureBoxVideoDisplay0);
            this.Controls.Add(this.comboBoxSourceChannelList);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxCurrentRemoteServer);
            this.Name = "LiveView";
            this.Size = new System.Drawing.Size(990, 517);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxVideoDisplay0)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxVideoDisplay1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxVideoDisplay2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxVideoDisplay3)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBoxSourceChannelList;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxCurrentRemoteServer;
        private System.Windows.Forms.PictureBox pictureBoxVideoDisplay0;
        private System.Windows.Forms.Button buttonPlayPause;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label labelDisplayFrameTime0;
        private System.Windows.Forms.Label labelPlateReadings0;
        private System.Windows.Forms.PictureBox pictureBoxVideoDisplay1;
        private System.Windows.Forms.PictureBox pictureBoxVideoDisplay2;
        private System.Windows.Forms.PictureBox pictureBoxVideoDisplay3;
        private System.Windows.Forms.Label labelPlateReadings1;
        private System.Windows.Forms.Label labelDisplayFrameTime1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label labelPlateReadings2;
        private System.Windows.Forms.Label labelDisplayFrameTime2;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label labelPlateReadings3;
        private System.Windows.Forms.Label labelDisplayFrameTime3;
        private System.Windows.Forms.Label label12;
    }
}
