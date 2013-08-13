namespace ConfigureGPS_UC
{
    partial class ConfigureGPS_UC
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
            this.groupBoxEnterInMinutesSeconds = new System.Windows.Forms.GroupBox();
            this.groupBoxEnterFractional = new System.Windows.Forms.GroupBox();
            this.buttonSaveMinSec = new System.Windows.Forms.Button();
            this.buttonSaveFrac = new System.Windows.Forms.Button();
            this.groupBoxEnterInMinutesSeconds.SuspendLayout();
            this.groupBoxEnterFractional.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxEnterInMinutesSeconds
            // 
            this.groupBoxEnterInMinutesSeconds.Controls.Add(this.buttonSaveMinSec);
            this.groupBoxEnterInMinutesSeconds.Location = new System.Drawing.Point(107, 14);
            this.groupBoxEnterInMinutesSeconds.Name = "groupBoxEnterInMinutesSeconds";
            this.groupBoxEnterInMinutesSeconds.Size = new System.Drawing.Size(372, 235);
            this.groupBoxEnterInMinutesSeconds.TabIndex = 0;
            this.groupBoxEnterInMinutesSeconds.TabStop = false;
            this.groupBoxEnterInMinutesSeconds.Text = "Enter in Minutes and Seconds";
            // 
            // groupBoxEnterFractional
            // 
            this.groupBoxEnterFractional.Controls.Add(this.buttonSaveFrac);
            this.groupBoxEnterFractional.Location = new System.Drawing.Point(107, 270);
            this.groupBoxEnterFractional.Name = "groupBoxEnterFractional";
            this.groupBoxEnterFractional.Size = new System.Drawing.Size(372, 235);
            this.groupBoxEnterFractional.TabIndex = 1;
            this.groupBoxEnterFractional.TabStop = false;
            this.groupBoxEnterFractional.Text = "Enter Fractional";
            // 
            // buttonSaveMinSec
            // 
            this.buttonSaveMinSec.Location = new System.Drawing.Point(130, 206);
            this.buttonSaveMinSec.Name = "buttonSaveMinSec";
            this.buttonSaveMinSec.Size = new System.Drawing.Size(75, 23);
            this.buttonSaveMinSec.TabIndex = 0;
            this.buttonSaveMinSec.Text = "Save";
            this.buttonSaveMinSec.UseVisualStyleBackColor = true;
            this.buttonSaveMinSec.Click += new System.EventHandler(this.buttonSaveMinSec_Click);
            // 
            // buttonSaveFrac
            // 
            this.buttonSaveFrac.Location = new System.Drawing.Point(130, 206);
            this.buttonSaveFrac.Name = "buttonSaveFrac";
            this.buttonSaveFrac.Size = new System.Drawing.Size(75, 23);
            this.buttonSaveFrac.TabIndex = 0;
            this.buttonSaveFrac.Text = "Save";
            this.buttonSaveFrac.UseVisualStyleBackColor = true;
            this.buttonSaveFrac.Click += new System.EventHandler(this.buttonSaveFrac_Click);
            // 
            // ConfigureGPS_UC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBoxEnterFractional);
            this.Controls.Add(this.groupBoxEnterInMinutesSeconds);
            this.Name = "ConfigureGPS_UC";
            this.Size = new System.Drawing.Size(606, 567);
            this.Load += new System.EventHandler(this.ConfigureGPS_UC_Load);
            this.groupBoxEnterInMinutesSeconds.ResumeLayout(false);
            this.groupBoxEnterFractional.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxEnterInMinutesSeconds;
        private System.Windows.Forms.GroupBox groupBoxEnterFractional;
        private System.Windows.Forms.Button buttonSaveMinSec;
        private System.Windows.Forms.Button buttonSaveFrac;
    }
}
