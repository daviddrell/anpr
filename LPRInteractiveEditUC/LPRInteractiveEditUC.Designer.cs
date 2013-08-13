namespace LPRInteractiveEditUC
{
    partial class LPRInteractiveEditUC
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
            this.pictureBoxPlateDisplay = new System.Windows.Forms.PictureBox();
            this.labelPlateNumbers = new System.Windows.Forms.Label();
            this.pictureBoxHistogram = new System.Windows.Forms.PictureBox();
            this.labelHistoString = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPlateDisplay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxHistogram)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBoxPlateDisplay
            // 
            this.pictureBoxPlateDisplay.BackColor = System.Drawing.Color.Black;
            this.pictureBoxPlateDisplay.Location = new System.Drawing.Point(31, 16);
            this.pictureBoxPlateDisplay.Name = "pictureBoxPlateDisplay";
            this.pictureBoxPlateDisplay.Size = new System.Drawing.Size(272, 110);
            this.pictureBoxPlateDisplay.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxPlateDisplay.TabIndex = 0;
            this.pictureBoxPlateDisplay.TabStop = false;
            this.pictureBoxPlateDisplay.Click += new System.EventHandler(this.pictureBoxPlateDisplay_Click);
            // 
            // labelPlateNumbers
            // 
            this.labelPlateNumbers.AutoSize = true;
            this.labelPlateNumbers.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelPlateNumbers.Location = new System.Drawing.Point(40, 143);
            this.labelPlateNumbers.Name = "labelPlateNumbers";
            this.labelPlateNumbers.Size = new System.Drawing.Size(51, 20);
            this.labelPlateNumbers.TabIndex = 1;
            this.labelPlateNumbers.Text = "label1";
            // 
            // pictureBoxHistogram
            // 
            this.pictureBoxHistogram.Location = new System.Drawing.Point(371, 16);
            this.pictureBoxHistogram.Name = "pictureBoxHistogram";
            this.pictureBoxHistogram.Size = new System.Drawing.Size(266, 110);
            this.pictureBoxHistogram.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxHistogram.TabIndex = 2;
            this.pictureBoxHistogram.TabStop = false;
            this.pictureBoxHistogram.Click += new System.EventHandler(this.pictureBoxHistogram_Click);
            // 
            // labelHistoString
            // 
            this.labelHistoString.AutoSize = true;
            this.labelHistoString.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelHistoString.Location = new System.Drawing.Point(371, 143);
            this.labelHistoString.Name = "labelHistoString";
            this.labelHistoString.Size = new System.Drawing.Size(51, 20);
            this.labelHistoString.TabIndex = 3;
            this.labelHistoString.Text = "label1";
            // 
            // LPRInteractiveEditUC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.labelHistoString);
            this.Controls.Add(this.pictureBoxHistogram);
            this.Controls.Add(this.labelPlateNumbers);
            this.Controls.Add(this.pictureBoxPlateDisplay);
            this.Name = "LPRInteractiveEditUC";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.Size = new System.Drawing.Size(999, 442);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxPlateDisplay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxHistogram)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxPlateDisplay;
        private System.Windows.Forms.Label labelPlateNumbers;
        private System.Windows.Forms.PictureBox pictureBoxHistogram;
        private System.Windows.Forms.Label labelHistoString;
    }
}
