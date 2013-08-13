namespace ConfigureGPS_UC
{
    partial class enterLocationFractionalPage
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
            this.labelLatitude = new System.Windows.Forms.Label();
            this.labelLongitutde = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelLatitude
            // 
            this.labelLatitude.AutoSize = true;
            this.labelLatitude.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelLatitude.Location = new System.Drawing.Point(79, 23);
            this.labelLatitude.Name = "labelLatitude";
            this.labelLatitude.Size = new System.Drawing.Size(53, 13);
            this.labelLatitude.TabIndex = 0;
            this.labelLatitude.Text = "Latitude";
            // 
            // labelLongitutde
            // 
            this.labelLongitutde.AutoSize = true;
            this.labelLongitutde.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelLongitutde.Location = new System.Drawing.Point(78, 89);
            this.labelLongitutde.Name = "labelLongitutde";
            this.labelLongitutde.Size = new System.Drawing.Size(67, 13);
            this.labelLongitutde.TabIndex = 1;
            this.labelLongitutde.Text = "Longitutde";
            // 
            // enterLocationFractionalPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.labelLongitutde);
            this.Controls.Add(this.labelLatitude);
            this.Name = "enterLocationFractionalPage";
            this.Size = new System.Drawing.Size(352, 179);
            this.Load += new System.EventHandler(this.enterLocationFractionalPage_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelLatitude;
        private System.Windows.Forms.Label labelLongitutde;
    }
}
