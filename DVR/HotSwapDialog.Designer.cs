namespace DVRLib
{
    partial class HotSwapDialog
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.labelCurrentDrive = new System.Windows.Forms.Label();
            this.labelSwappingToDrive = new System.Windows.Forms.Label();
            this.textBoxCurrentDriveStatus = new System.Windows.Forms.TextBox();
            this.textBoxNewDriveStatus = new System.Windows.Forms.TextBox();
            this.textBoxSwapStatus = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(167, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(255, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "HOT SWAP DETECTED !";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(56, 75);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(183, 24);
            this.label2.TabIndex = 1;
            this.label2.Text = "Current Drive In Use:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(55, 236);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(175, 24);
            this.label3.TabIndex = 2;
            this.label3.Text = "Swapping To Drive:";
            // 
            // labelCurrentDrive
            // 
            this.labelCurrentDrive.AutoSize = true;
            this.labelCurrentDrive.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelCurrentDrive.Location = new System.Drawing.Point(254, 75);
            this.labelCurrentDrive.Name = "labelCurrentDrive";
            this.labelCurrentDrive.Size = new System.Drawing.Size(0, 24);
            this.labelCurrentDrive.TabIndex = 3;
            // 
            // labelSwappingToDrive
            // 
            this.labelSwappingToDrive.AutoSize = true;
            this.labelSwappingToDrive.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelSwappingToDrive.Location = new System.Drawing.Point(244, 236);
            this.labelSwappingToDrive.Name = "labelSwappingToDrive";
            this.labelSwappingToDrive.Size = new System.Drawing.Size(0, 24);
            this.labelSwappingToDrive.TabIndex = 4;
            // 
            // textBoxCurrentDriveStatus
            // 
            this.textBoxCurrentDriveStatus.Location = new System.Drawing.Point(60, 102);
            this.textBoxCurrentDriveStatus.MaximumSize = new System.Drawing.Size(4, 100);
            this.textBoxCurrentDriveStatus.MinimumSize = new System.Drawing.Size(200, 100);
            this.textBoxCurrentDriveStatus.Multiline = true;
            this.textBoxCurrentDriveStatus.Name = "textBoxCurrentDriveStatus";
            this.textBoxCurrentDriveStatus.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxCurrentDriveStatus.Size = new System.Drawing.Size(200, 100);
            this.textBoxCurrentDriveStatus.TabIndex = 5;
            // 
            // textBoxNewDriveStatus
            // 
            this.textBoxNewDriveStatus.Location = new System.Drawing.Point(59, 263);
            this.textBoxNewDriveStatus.MaximumSize = new System.Drawing.Size(4, 100);
            this.textBoxNewDriveStatus.MinimumSize = new System.Drawing.Size(200, 100);
            this.textBoxNewDriveStatus.Multiline = true;
            this.textBoxNewDriveStatus.Name = "textBoxNewDriveStatus";
            this.textBoxNewDriveStatus.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxNewDriveStatus.Size = new System.Drawing.Size(200, 100);
            this.textBoxNewDriveStatus.TabIndex = 6;
            // 
            // textBoxSwapStatus
            // 
            this.textBoxSwapStatus.Location = new System.Drawing.Point(60, 435);
            this.textBoxSwapStatus.MaximumSize = new System.Drawing.Size(4, 100);
            this.textBoxSwapStatus.MinimumSize = new System.Drawing.Size(200, 100);
            this.textBoxSwapStatus.Multiline = true;
            this.textBoxSwapStatus.Name = "textBoxSwapStatus";
            this.textBoxSwapStatus.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxSwapStatus.Size = new System.Drawing.Size(200, 100);
            this.textBoxSwapStatus.TabIndex = 8;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(56, 408);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(112, 24);
            this.label4.TabIndex = 7;
            this.label4.Text = "Swap Status";
            // 
            // HotSwapDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(584, 564);
            this.Controls.Add(this.textBoxSwapStatus);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBoxNewDriveStatus);
            this.Controls.Add(this.textBoxCurrentDriveStatus);
            this.Controls.Add(this.labelSwappingToDrive);
            this.Controls.Add(this.labelCurrentDrive);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "HotSwapDialog";
            this.Text = "Hot Swap Dialog";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label labelCurrentDrive;
        private System.Windows.Forms.Label labelSwappingToDrive;
        private System.Windows.Forms.TextBox textBoxCurrentDriveStatus;
        private System.Windows.Forms.TextBox textBoxNewDriveStatus;
        private System.Windows.Forms.TextBox textBoxSwapStatus;
        private System.Windows.Forms.Label label4;
    }
}