namespace LPR_GoNoGoStatus
{
    partial class GoNoGoStatusMainForm
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
            this.panelVidDisplayPanel = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonLPRServiceStatusIndicator = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.buttonFGStatus_1 = new System.Windows.Forms.Button();
            this.buttonFGStatus_0 = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.buttonChannel4 = new System.Windows.Forms.Button();
            this.buttonChannel3 = new System.Windows.Forms.Button();
            this.buttonChannel2 = new System.Windows.Forms.Button();
            this.buttonChannel1 = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.buttonGPSStatus = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.textBoxHotSwapStatus = new System.Windows.Forms.TextBox();
            this.buttonDiskDrive = new System.Windows.Forms.Button();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.buttonSaveCurrentImage = new System.Windows.Forms.Button();
            this.groupBoxPlateProcessQueLevel = new System.Windows.Forms.GroupBox();
            this.labelServiceVersion = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelVidDisplayPanel
            // 
            this.panelVidDisplayPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelVidDisplayPanel.Location = new System.Drawing.Point(288, 32);
            this.panelVidDisplayPanel.Name = "panelVidDisplayPanel";
            this.panelVidDisplayPanel.Size = new System.Drawing.Size(590, 548);
            this.panelVidDisplayPanel.TabIndex = 20;
            this.panelVidDisplayPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.panelVidDisplayPanel_Paint);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.labelServiceVersion);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.buttonLPRServiceStatusIndicator);
            this.groupBox1.Location = new System.Drawing.Point(13, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(240, 77);
            this.groupBox1.TabIndex = 21;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "LPR Service";
            // 
            // buttonLPRServiceStatusIndicator
            // 
            this.buttonLPRServiceStatusIndicator.Location = new System.Drawing.Point(34, 19);
            this.buttonLPRServiceStatusIndicator.Name = "buttonLPRServiceStatusIndicator";
            this.buttonLPRServiceStatusIndicator.Size = new System.Drawing.Size(177, 32);
            this.buttonLPRServiceStatusIndicator.TabIndex = 0;
            this.buttonLPRServiceStatusIndicator.Text = "Not Running";
            this.buttonLPRServiceStatusIndicator.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.buttonFGStatus_1);
            this.groupBox2.Controls.Add(this.buttonFGStatus_0);
            this.groupBox2.Location = new System.Drawing.Point(13, 97);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(240, 101);
            this.groupBox2.TabIndex = 22;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Frame Grabbers";
            // 
            // buttonFGStatus_1
            // 
            this.buttonFGStatus_1.Location = new System.Drawing.Point(34, 57);
            this.buttonFGStatus_1.Name = "buttonFGStatus_1";
            this.buttonFGStatus_1.Size = new System.Drawing.Size(177, 32);
            this.buttonFGStatus_1.TabIndex = 2;
            this.buttonFGStatus_1.Text = "Not Connected";
            this.buttonFGStatus_1.UseVisualStyleBackColor = true;
            // 
            // buttonFGStatus_0
            // 
            this.buttonFGStatus_0.Location = new System.Drawing.Point(34, 19);
            this.buttonFGStatus_0.Name = "buttonFGStatus_0";
            this.buttonFGStatus_0.Size = new System.Drawing.Size(177, 32);
            this.buttonFGStatus_0.TabIndex = 1;
            this.buttonFGStatus_0.Text = "Not Connected";
            this.buttonFGStatus_0.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.buttonChannel4);
            this.groupBox3.Controls.Add(this.buttonChannel3);
            this.groupBox3.Controls.Add(this.buttonChannel2);
            this.groupBox3.Controls.Add(this.buttonChannel1);
            this.groupBox3.Location = new System.Drawing.Point(13, 205);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(240, 147);
            this.groupBox3.TabIndex = 23;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Video Cables";
            // 
            // buttonChannel4
            // 
            this.buttonChannel4.Location = new System.Drawing.Point(34, 107);
            this.buttonChannel4.Name = "buttonChannel4";
            this.buttonChannel4.Size = new System.Drawing.Size(177, 23);
            this.buttonChannel4.TabIndex = 3;
            this.buttonChannel4.Text = "Channel 4 Not Connected";
            this.buttonChannel4.UseVisualStyleBackColor = true;
            // 
            // buttonChannel3
            // 
            this.buttonChannel3.Location = new System.Drawing.Point(34, 78);
            this.buttonChannel3.Name = "buttonChannel3";
            this.buttonChannel3.Size = new System.Drawing.Size(177, 23);
            this.buttonChannel3.TabIndex = 2;
            this.buttonChannel3.Text = "Channel 3 Not Connected";
            this.buttonChannel3.UseVisualStyleBackColor = true;
            // 
            // buttonChannel2
            // 
            this.buttonChannel2.Location = new System.Drawing.Point(34, 49);
            this.buttonChannel2.Name = "buttonChannel2";
            this.buttonChannel2.Size = new System.Drawing.Size(177, 23);
            this.buttonChannel2.TabIndex = 1;
            this.buttonChannel2.Text = "Channel 2 Not Connected";
            this.buttonChannel2.UseVisualStyleBackColor = true;
            // 
            // buttonChannel1
            // 
            this.buttonChannel1.Location = new System.Drawing.Point(34, 20);
            this.buttonChannel1.Name = "buttonChannel1";
            this.buttonChannel1.Size = new System.Drawing.Size(177, 23);
            this.buttonChannel1.TabIndex = 0;
            this.buttonChannel1.Text = "Channel 1 Not Connected";
            this.buttonChannel1.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.buttonGPSStatus);
            this.groupBox4.Location = new System.Drawing.Point(13, 359);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(240, 79);
            this.groupBox4.TabIndex = 24;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "GPS";
            // 
            // buttonGPSStatus
            // 
            this.buttonGPSStatus.Location = new System.Drawing.Point(6, 23);
            this.buttonGPSStatus.Name = "buttonGPSStatus";
            this.buttonGPSStatus.Size = new System.Drawing.Size(228, 32);
            this.buttonGPSStatus.TabIndex = 3;
            this.buttonGPSStatus.Text = "Not Connected";
            this.buttonGPSStatus.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.textBoxHotSwapStatus);
            this.groupBox5.Controls.Add(this.buttonDiskDrive);
            this.groupBox5.Location = new System.Drawing.Point(13, 456);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(240, 89);
            this.groupBox5.TabIndex = 25;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Disk Drive";
            // 
            // textBoxHotSwapStatus
            // 
            this.textBoxHotSwapStatus.Location = new System.Drawing.Point(34, 58);
            this.textBoxHotSwapStatus.Name = "textBoxHotSwapStatus";
            this.textBoxHotSwapStatus.Size = new System.Drawing.Size(175, 20);
            this.textBoxHotSwapStatus.TabIndex = 4;
            // 
            // buttonDiskDrive
            // 
            this.buttonDiskDrive.Location = new System.Drawing.Point(32, 23);
            this.buttonDiskDrive.Name = "buttonDiskDrive";
            this.buttonDiskDrive.Size = new System.Drawing.Size(177, 25);
            this.buttonDiskDrive.TabIndex = 3;
            this.buttonDiskDrive.Text = "Drive Not Connected";
            this.buttonDiskDrive.UseVisualStyleBackColor = true;
            // 
            // buttonSaveCurrentImage
            // 
            this.buttonSaveCurrentImage.Location = new System.Drawing.Point(47, 663);
            this.buttonSaveCurrentImage.Name = "buttonSaveCurrentImage";
            this.buttonSaveCurrentImage.Size = new System.Drawing.Size(175, 23);
            this.buttonSaveCurrentImage.TabIndex = 26;
            this.buttonSaveCurrentImage.Text = "Save Current Image";
            this.buttonSaveCurrentImage.UseVisualStyleBackColor = true;
            this.buttonSaveCurrentImage.Click += new System.EventHandler(this.buttonSaveCurrentImage_Click);
            // 
            // groupBoxPlateProcessQueLevel
            // 
            this.groupBoxPlateProcessQueLevel.Location = new System.Drawing.Point(19, 552);
            this.groupBoxPlateProcessQueLevel.Name = "groupBoxPlateProcessQueLevel";
            this.groupBoxPlateProcessQueLevel.Size = new System.Drawing.Size(234, 100);
            this.groupBoxPlateProcessQueLevel.TabIndex = 29;
            this.groupBoxPlateProcessQueLevel.TabStop = false;
            this.groupBoxPlateProcessQueLevel.Text = "Plate Process Queue Level";
            // 
            // labelServiceVersion
            // 
            this.labelServiceVersion.AutoSize = true;
            this.labelServiceVersion.Location = new System.Drawing.Point(95, 54);
            this.labelServiceVersion.Name = "labelServiceVersion";
            this.labelServiceVersion.Size = new System.Drawing.Size(31, 13);
            this.labelServiceVersion.TabIndex = 33;
            this.labelServiceVersion.Text = "____";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 54);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 13);
            this.label1.TabIndex = 32;
            this.label1.Text = "Service version:";
            // 
            // GoNoGoStatusMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(909, 735);
            this.Controls.Add(this.groupBoxPlateProcessQueLevel);
            this.Controls.Add(this.buttonSaveCurrentImage);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panelVidDisplayPanel);
            this.Name = "GoNoGoStatusMainForm";
            this.Text = "LPR Go No-go Status";
            this.Load += new System.EventHandler(this.GoNoGoStatusMainForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelVidDisplayPanel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonLPRServiceStatusIndicator;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button buttonFGStatus_1;
        private System.Windows.Forms.Button buttonFGStatus_0;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button buttonChannel4;
        private System.Windows.Forms.Button buttonChannel3;
        private System.Windows.Forms.Button buttonChannel2;
        private System.Windows.Forms.Button buttonChannel1;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button buttonGPSStatus;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Button buttonDiskDrive;
        private System.Windows.Forms.TextBox textBoxHotSwapStatus;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Button buttonSaveCurrentImage;
        private System.Windows.Forms.GroupBox groupBoxPlateProcessQueLevel;
        private System.Windows.Forms.Label labelServiceVersion;
        private System.Windows.Forms.Label label1;
    }
}

