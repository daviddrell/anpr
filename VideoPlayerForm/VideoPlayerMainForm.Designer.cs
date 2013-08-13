namespace VideoPlayerForm
{
    partial class VideoPlayerMainForm
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
            this.videoPlayer1 = new VideoPlayerUC.VideoPlayer();
            this.SuspendLayout();
            // 
            // videoPlayer1
            // 
            this.videoPlayer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.videoPlayer1.Location = new System.Drawing.Point(0, 0);
            this.videoPlayer1.Name = "videoPlayer1";
            this.videoPlayer1.Size = new System.Drawing.Size(530, 642);
            this.videoPlayer1.TabIndex = 0;
            this.videoPlayer1.Load += new System.EventHandler(this.videoPlayer1_Load);
            // 
            // VideoPlayerMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(530, 642);
            this.Controls.Add(this.videoPlayer1);
            this.Name = "VideoPlayerMainForm";
            this.Text = "Video Player";
            this.ResumeLayout(false);

        }

        #endregion

        private VideoPlayerUC.VideoPlayer videoPlayer1;
    }
}

