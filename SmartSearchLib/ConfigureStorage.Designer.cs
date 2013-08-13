namespace SmartSearchLib
{
   partial class ConfigureStorageForm
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
          this.finishedButton = new System.Windows.Forms.Button();
          this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
          this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
          this.selectStorageLocationButton = new System.Windows.Forms.Button();
          this.storageLocationPathLabel = new System.Windows.Forms.Label();
          this.label1 = new System.Windows.Forms.Label();
          this.SuspendLayout();
          // 
          // finishedButton
          // 
          this.finishedButton.Location = new System.Drawing.Point(247, 188);
          this.finishedButton.Name = "finishedButton";
          this.finishedButton.Size = new System.Drawing.Size(75, 23);
          this.finishedButton.TabIndex = 2;
          this.finishedButton.Text = "Finished";
          this.finishedButton.UseVisualStyleBackColor = true;
          this.finishedButton.Click += new System.EventHandler(this.finishedButton_Click);
          // 
          // openFileDialog1
          // 
          this.openFileDialog1.FileName = "openFileDialog1";
          this.openFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog1_FileOk);
          // 
          // folderBrowserDialog1
          // 
          this.folderBrowserDialog1.HelpRequest += new System.EventHandler(this.folderBrowserDialog1_HelpRequest);
          // 
          // selectStorageLocationButton
          // 
          this.selectStorageLocationButton.Location = new System.Drawing.Point(221, 23);
          this.selectStorageLocationButton.Name = "selectStorageLocationButton";
          this.selectStorageLocationButton.Size = new System.Drawing.Size(126, 46);
          this.selectStorageLocationButton.TabIndex = 5;
          this.selectStorageLocationButton.Text = "Select Storage Location";
          this.selectStorageLocationButton.UseVisualStyleBackColor = true;
          this.selectStorageLocationButton.Click += new System.EventHandler(this.selectStorageLocationButton_Click);
          // 
          // storageLocationPathLabel
          // 
          this.storageLocationPathLabel.AutoSize = true;
          this.storageLocationPathLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
          this.storageLocationPathLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
          this.storageLocationPathLabel.Location = new System.Drawing.Point(158, 119);
          this.storageLocationPathLabel.Name = "storageLocationPathLabel";
          this.storageLocationPathLabel.Size = new System.Drawing.Size(13, 15);
          this.storageLocationPathLabel.TabIndex = 6;
          this.storageLocationPathLabel.Text = " ";
          this.storageLocationPathLabel.Click += new System.EventHandler(this.storageLocationPathLabel_Click);
          // 
          // label1
          // 
          this.label1.AutoSize = true;
          this.label1.Location = new System.Drawing.Point(21, 121);
          this.label1.Name = "label1";
          this.label1.Size = new System.Drawing.Size(131, 13);
          this.label1.TabIndex = 9;
          this.label1.Text = "Current Storage Location :";
          // 
          // ConfigureStorageForm
          // 
          this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
          this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
          this.BackColor = System.Drawing.SystemColors.ControlLightLight;
          this.ClientSize = new System.Drawing.Size(594, 264);
          this.ControlBox = false;
          this.Controls.Add(this.label1);
          this.Controls.Add(this.storageLocationPathLabel);
          this.Controls.Add(this.selectStorageLocationButton);
          this.Controls.Add(this.finishedButton);
          this.MaximizeBox = false;
          this.MinimizeBox = false;
          this.Name = "ConfigureStorageForm";
          this.Text = "Configure Storage";
          this.Load += new System.EventHandler(this.ConfigureStorageForm_Load);
          this.ResumeLayout(false);
          this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.Button finishedButton;
      private System.Windows.Forms.OpenFileDialog openFileDialog1;
      private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
      private System.Windows.Forms.Button selectStorageLocationButton;
      private System.Windows.Forms.Label storageLocationPathLabel;
      private System.Windows.Forms.Label label1;
   }
}