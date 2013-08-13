namespace ConfigWatchListsUC
{
    partial class ConfigWatchListsUC
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
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.buttonAddNew = new System.Windows.Forms.Button();
            this.buttonDeleteSelected = new System.Windows.Forms.Button();
            this.listBoxCurrentlyLoadedWatchLists = new System.Windows.Forms.ListBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.buttonCancelChanges = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.textBoxListCount = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.labelListStatus = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.buttonSelectWatchFile = new System.Windows.Forms.Button();
            this.buttonSelectedEmailList = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxAlertMatchThreshold = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxWatchListFile = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxEmailListFile = new System.Windows.Forms.TextBox();
            this.buttonSave = new System.Windows.Forms.Button();
            this.textBoxName = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox1.Controls.Add(this.buttonAddNew);
            this.groupBox1.Controls.Add(this.buttonDeleteSelected);
            this.groupBox1.Controls.Add(this.listBoxCurrentlyLoadedWatchLists);
            this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.groupBox1.Location = new System.Drawing.Point(23, 65);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(292, 348);
            this.groupBox1.TabIndex = 41;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Currently Loaded Watch Lists";
            // 
            // buttonAddNew
            // 
            this.buttonAddNew.Location = new System.Drawing.Point(155, 296);
            this.buttonAddNew.Name = "buttonAddNew";
            this.buttonAddNew.Size = new System.Drawing.Size(128, 23);
            this.buttonAddNew.TabIndex = 23;
            this.buttonAddNew.Text = "Add New List";
            this.buttonAddNew.UseVisualStyleBackColor = true;
            this.buttonAddNew.Click += new System.EventHandler(this.buttonAddNew_Click);
            // 
            // buttonDeleteSelected
            // 
            this.buttonDeleteSelected.Location = new System.Drawing.Point(6, 296);
            this.buttonDeleteSelected.Name = "buttonDeleteSelected";
            this.buttonDeleteSelected.Size = new System.Drawing.Size(128, 23);
            this.buttonDeleteSelected.TabIndex = 22;
            this.buttonDeleteSelected.Text = "Delete Selected List";
            this.buttonDeleteSelected.UseVisualStyleBackColor = true;
            this.buttonDeleteSelected.Click += new System.EventHandler(this.buttonDeleteSelected_Click);
            // 
            // listBoxCurrentlyLoadedWatchLists
            // 
            this.listBoxCurrentlyLoadedWatchLists.FormattingEnabled = true;
            this.listBoxCurrentlyLoadedWatchLists.Location = new System.Drawing.Point(44, 36);
            this.listBoxCurrentlyLoadedWatchLists.Name = "listBoxCurrentlyLoadedWatchLists";
            this.listBoxCurrentlyLoadedWatchLists.Size = new System.Drawing.Size(189, 251);
            this.listBoxCurrentlyLoadedWatchLists.TabIndex = 24;
            this.listBoxCurrentlyLoadedWatchLists.SelectedIndexChanged += new System.EventHandler(this.listBoxCurrentlyLoadedWatchLists_SelectedIndexChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.White;
            this.groupBox2.Controls.Add(this.buttonCancelChanges);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.textBoxListCount);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.labelListStatus);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.buttonSelectWatchFile);
            this.groupBox2.Controls.Add(this.buttonSelectedEmailList);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.textBoxAlertMatchThreshold);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.textBoxWatchListFile);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.textBoxEmailListFile);
            this.groupBox2.Controls.Add(this.buttonSave);
            this.groupBox2.Controls.Add(this.textBoxName);
            this.groupBox2.Location = new System.Drawing.Point(334, 65);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(826, 348);
            this.groupBox2.TabIndex = 42;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Edit Watch List";
            // 
            // buttonCancelChanges
            // 
            this.buttonCancelChanges.Location = new System.Drawing.Point(440, 296);
            this.buttonCancelChanges.Name = "buttonCancelChanges";
            this.buttonCancelChanges.Size = new System.Drawing.Size(105, 23);
            this.buttonCancelChanges.TabIndex = 48;
            this.buttonCancelChanges.Text = "Cancel Changes";
            this.buttonCancelChanges.UseVisualStyleBackColor = true;
            this.buttonCancelChanges.Click += new System.EventHandler(this.buttonCancelChanges_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(304, 159);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(60, 13);
            this.label8.TabIndex = 57;
            this.label8.Text = "10 to 100%";
            // 
            // textBoxListCount
            // 
            this.textBoxListCount.Location = new System.Drawing.Point(198, 126);
            this.textBoxListCount.Name = "textBoxListCount";
            this.textBoxListCount.ReadOnly = true;
            this.textBoxListCount.Size = new System.Drawing.Size(100, 20);
            this.textBoxListCount.TabIndex = 56;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(48, 128);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(109, 16);
            this.label7.TabIndex = 55;
            this.label7.Text = "Watch List Count:";
            // 
            // labelListStatus
            // 
            this.labelListStatus.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.labelListStatus.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelListStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelListStatus.Location = new System.Drawing.Point(162, 26);
            this.labelListStatus.Name = "labelListStatus";
            this.labelListStatus.Size = new System.Drawing.Size(550, 18);
            this.labelListStatus.TabIndex = 54;
            this.labelListStatus.Text = "List Status: ";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(85, -27);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(74, 16);
            this.label6.TabIndex = 53;
            this.label6.Text = "List Status: ";
            // 
            // buttonSelectWatchFile
            // 
            this.buttonSelectWatchFile.Location = new System.Drawing.Point(719, 96);
            this.buttonSelectWatchFile.Name = "buttonSelectWatchFile";
            this.buttonSelectWatchFile.Size = new System.Drawing.Size(75, 23);
            this.buttonSelectWatchFile.TabIndex = 45;
            this.buttonSelectWatchFile.Text = "select file";
            this.buttonSelectWatchFile.UseVisualStyleBackColor = true;
            this.buttonSelectWatchFile.Click += new System.EventHandler(this.buttonSelectWatchFile_Click);
            // 
            // buttonSelectedEmailList
            // 
            this.buttonSelectedEmailList.Location = new System.Drawing.Point(719, 196);
            this.buttonSelectedEmailList.Name = "buttonSelectedEmailList";
            this.buttonSelectedEmailList.Size = new System.Drawing.Size(75, 23);
            this.buttonSelectedEmailList.TabIndex = 46;
            this.buttonSelectedEmailList.Text = "select file";
            this.buttonSelectedEmailList.UseVisualStyleBackColor = true;
            this.buttonSelectedEmailList.Click += new System.EventHandler(this.buttonSelectedEmailList_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(45, 156);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(141, 16);
            this.label5.TabIndex = 52;
            this.label5.Text = "Alert Match Threshold:";
            // 
            // textBoxAlertMatchThreshold
            // 
            this.textBoxAlertMatchThreshold.Location = new System.Drawing.Point(198, 156);
            this.textBoxAlertMatchThreshold.Name = "textBoxAlertMatchThreshold";
            this.textBoxAlertMatchThreshold.Size = new System.Drawing.Size(100, 20);
            this.textBoxAlertMatchThreshold.TabIndex = 43;
            this.textBoxAlertMatchThreshold.TextChanged += new System.EventHandler(this.textBoxAlertMatchThreshold_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(80, 74);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(112, 16);
            this.label4.TabIndex = 51;
            this.label4.Text = "Watch List Name:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(92, 103);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(97, 16);
            this.label3.TabIndex = 50;
            this.label3.Text = "Watch List File:";
            // 
            // textBoxWatchListFile
            // 
            this.textBoxWatchListFile.Location = new System.Drawing.Point(198, 100);
            this.textBoxWatchListFile.Name = "textBoxWatchListFile";
            this.textBoxWatchListFile.Size = new System.Drawing.Size(514, 20);
            this.textBoxWatchListFile.TabIndex = 42;
            this.textBoxWatchListFile.TextChanged += new System.EventHandler(this.textBoxWatchListFile_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(24, 199);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(162, 16);
            this.label2.TabIndex = 49;
            this.label2.Text = "Email Notification List File:";
            // 
            // textBoxEmailListFile
            // 
            this.textBoxEmailListFile.Location = new System.Drawing.Point(195, 199);
            this.textBoxEmailListFile.Name = "textBoxEmailListFile";
            this.textBoxEmailListFile.Size = new System.Drawing.Size(517, 20);
            this.textBoxEmailListFile.TabIndex = 44;
            this.textBoxEmailListFile.TextChanged += new System.EventHandler(this.textBoxEmailListFile_TextChanged);
            // 
            // buttonSave
            // 
            this.buttonSave.Location = new System.Drawing.Point(307, 296);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(105, 23);
            this.buttonSave.TabIndex = 47;
            this.buttonSave.Text = "Save Changes";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // textBoxName
            // 
            this.textBoxName.Location = new System.Drawing.Point(198, 74);
            this.textBoxName.Name = "textBoxName";
            this.textBoxName.Size = new System.Drawing.Size(273, 20);
            this.textBoxName.TabIndex = 41;
            // 
            // ConfigWatchListsUC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "ConfigWatchListsUC";
            this.Size = new System.Drawing.Size(1198, 650);
            this.Load += new System.EventHandler(this.ConfigWatchListsUC_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonAddNew;
        private System.Windows.Forms.Button buttonDeleteSelected;
        private System.Windows.Forms.ListBox listBoxCurrentlyLoadedWatchLists;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button buttonCancelChanges;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBoxListCount;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label labelListStatus;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button buttonSelectWatchFile;
        private System.Windows.Forms.Button buttonSelectedEmailList;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxAlertMatchThreshold;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxWatchListFile;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxEmailListFile;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.TextBox textBoxName;
    }
}
