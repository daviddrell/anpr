namespace AnalystsWorkstation
{
    partial class ImportImageDrive
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
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxCentralRepository = new System.Windows.Forms.TextBox();
            this.textBoxFieldDrive = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonStartImport = new System.Windows.Forms.Button();
            this.listBoxStatus = new System.Windows.Forms.ListBox();
            this.labelStatus = new System.Windows.Forms.Label();
            this.buttonStopImport = new System.Windows.Forms.Button();
            this.labelDriveFreeSpace = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(182, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(133, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Current Central Repository:";
            // 
            // textBoxCentralRepository
            // 
            this.textBoxCentralRepository.Location = new System.Drawing.Point(185, 54);
            this.textBoxCentralRepository.Name = "textBoxCentralRepository";
            this.textBoxCentralRepository.Size = new System.Drawing.Size(193, 20);
            this.textBoxCentralRepository.TabIndex = 1;
            // 
            // textBoxFieldDrive
            // 
            this.textBoxFieldDrive.Location = new System.Drawing.Point(185, 135);
            this.textBoxFieldDrive.Name = "textBoxFieldDrive";
            this.textBoxFieldDrive.Size = new System.Drawing.Size(193, 20);
            this.textBoxFieldDrive.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(182, 118);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(107, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Detected Field Drive:";
            // 
            // buttonStartImport
            // 
            this.buttonStartImport.Location = new System.Drawing.Point(185, 171);
            this.buttonStartImport.Name = "buttonStartImport";
            this.buttonStartImport.Size = new System.Drawing.Size(193, 23);
            this.buttonStartImport.TabIndex = 4;
            this.buttonStartImport.Text = "Start Import";
            this.buttonStartImport.UseVisualStyleBackColor = true;
            this.buttonStartImport.Click += new System.EventHandler(this.buttonStartImport_Click);
            // 
            // listBoxStatus
            // 
            this.listBoxStatus.FormattingEnabled = true;
            this.listBoxStatus.Location = new System.Drawing.Point(185, 269);
            this.listBoxStatus.Name = "listBoxStatus";
            this.listBoxStatus.Size = new System.Drawing.Size(518, 186);
            this.listBoxStatus.TabIndex = 6;
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Location = new System.Drawing.Point(185, 250);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(40, 13);
            this.labelStatus.TabIndex = 7;
            this.labelStatus.Text = "Status:";
            // 
            // buttonStopImport
            // 
            this.buttonStopImport.Location = new System.Drawing.Point(185, 209);
            this.buttonStopImport.Name = "buttonStopImport";
            this.buttonStopImport.Size = new System.Drawing.Size(193, 23);
            this.buttonStopImport.TabIndex = 8;
            this.buttonStopImport.Text = "Stop Import";
            this.buttonStopImport.UseVisualStyleBackColor = true;
            this.buttonStopImport.Click += new System.EventHandler(this.buttonStopImport_Click);
            // 
            // labelDriveFreeSpace
            // 
            this.labelDriveFreeSpace.AutoSize = true;
            this.labelDriveFreeSpace.Location = new System.Drawing.Point(185, 77);
            this.labelDriveFreeSpace.Name = "labelDriveFreeSpace";
            this.labelDriveFreeSpace.Size = new System.Drawing.Size(13, 13);
            this.labelDriveFreeSpace.TabIndex = 9;
            this.labelDriveFreeSpace.Text = "0";
            // 
            // ImportImageDrive
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.labelDriveFreeSpace);
            this.Controls.Add(this.buttonStopImport);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.listBoxStatus);
            this.Controls.Add(this.buttonStartImport);
            this.Controls.Add(this.textBoxFieldDrive);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxCentralRepository);
            this.Controls.Add(this.label1);
            this.Name = "ImportImageDrive";
            this.Size = new System.Drawing.Size(1019, 487);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxCentralRepository;
        private System.Windows.Forms.TextBox textBoxFieldDrive;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonStartImport;
        private System.Windows.Forms.ListBox listBoxStatus;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.Button buttonStopImport;
        private System.Windows.Forms.Label labelDriveFreeSpace;
    }
}
