namespace SmartSearchLib
{
    partial class ExportStatus
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
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.labelStatusCount = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(131, 64);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(233, 23);
            this.progressBar1.TabIndex = 0;
            // 
            // labelStatusCount
            // 
            this.labelStatusCount.AutoSize = true;
            this.labelStatusCount.Location = new System.Drawing.Point(131, 45);
            this.labelStatusCount.Name = "labelStatusCount";
            this.labelStatusCount.Size = new System.Drawing.Size(0, 13);
            this.labelStatusCount.TabIndex = 1;
            // 
            // ExportStatus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(499, 139);
            this.Controls.Add(this.labelStatusCount);
            this.Controls.Add(this.progressBar1);
            this.Name = "ExportStatus";
            this.Text = "Export Status";
            this.Load += new System.EventHandler(this.ExportStatus_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label labelStatusCount;
    }
}