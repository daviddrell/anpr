namespace SmartSearchLib
{
   partial class SmartSearchLibUC
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
          this.components = new System.ComponentModel.Container();
          System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
          this.timer1 = new System.Windows.Forms.Timer(this.components);
          this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
          this.panelEnterSearch = new System.Windows.Forms.Panel();
          this.label6 = new System.Windows.Forms.Label();
          this.textBoxCameraNameFilter = new System.Windows.Forms.TextBox();
          this.textBoxMinMatch = new System.Windows.Forms.TextBox();
          this.label7 = new System.Windows.Forms.Label();
          this.doSearchButton = new System.Windows.Forms.Button();
          this.searchStringTextBox = new System.Windows.Forms.TextBox();
          this.label1 = new System.Windows.Forms.Label();
          this.panelTime = new System.Windows.Forms.Panel();
          this.labelLocalTime = new System.Windows.Forms.Label();
          this.label9 = new System.Windows.Forms.Label();
          this.labelTimeZoneName = new System.Windows.Forms.Label();
          this.label8 = new System.Windows.Forms.Label();
          this.currentTimeLabel = new System.Windows.Forms.Label();
          this.label5 = new System.Windows.Forms.Label();
          this.label3 = new System.Windows.Forms.Label();
          this.label2 = new System.Windows.Forms.Label();
          this.dateTimePicker2 = new System.Windows.Forms.DateTimePicker();
          this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
          this.panelResults = new System.Windows.Forms.Panel();
          this.buttonUndoDelete = new System.Windows.Forms.Button();
          this.buttonExportTable = new System.Windows.Forms.Button();
          this.buttonPlayVideo = new System.Windows.Forms.Button();
          this.m_ZoomPictureControl = new ScalablePictureBoxTool.ScalablePictureBox();
          this.trackBarZoomControl = new System.Windows.Forms.TrackBar();
          this.buttonPlotLocation = new System.Windows.Forms.Button();
          this.buttonExportImage = new System.Windows.Forms.Button();
          this.numItemsFoundLabel = new System.Windows.Forms.Label();
          this.label4 = new System.Windows.Forms.Label();
          this.dataGridView1 = new System.Windows.Forms.DataGridView();
          this.timerStopForm = new System.Windows.Forms.Timer(this.components);
          this.buttonDumpMotionEvents = new System.Windows.Forms.Button();
          this.buttonAllImagesInRange = new System.Windows.Forms.Button();
          this.buttonDumpRange = new System.Windows.Forms.Button();
          this.groupBox1 = new System.Windows.Forms.GroupBox();
          this.labelRepository = new System.Windows.Forms.Label();
          this.SearchMatchLikelyhood = new System.Windows.Forms.DataGridViewTextBoxColumn();
          this.ScannedNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
          this.time = new System.Windows.Forms.DataGridViewTextBoxColumn();
          this.PSS = new System.Windows.Forms.DataGridViewTextBoxColumn();
          this.cameraName = new System.Windows.Forms.DataGridViewTextBoxColumn();
          this.imagePath = new System.Windows.Forms.DataGridViewTextBoxColumn();
          this.GPSLocation = new System.Windows.Forms.DataGridViewTextBoxColumn();
          this.panelEnterSearch.SuspendLayout();
          this.panelTime.SuspendLayout();
          this.panelResults.SuspendLayout();
          ((System.ComponentModel.ISupportInitialize)(this.trackBarZoomControl)).BeginInit();
          ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
          this.groupBox1.SuspendLayout();
          this.SuspendLayout();
          // 
          // timer1
          // 
          this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
          // 
          // saveFileDialog1
          // 
          this.saveFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.saveFileDialog1_FileOk);
          // 
          // panelEnterSearch
          // 
          this.panelEnterSearch.BackColor = System.Drawing.SystemColors.ControlLight;
          this.panelEnterSearch.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
          this.panelEnterSearch.Controls.Add(this.label6);
          this.panelEnterSearch.Controls.Add(this.textBoxCameraNameFilter);
          this.panelEnterSearch.Controls.Add(this.textBoxMinMatch);
          this.panelEnterSearch.Controls.Add(this.label7);
          this.panelEnterSearch.Controls.Add(this.doSearchButton);
          this.panelEnterSearch.Controls.Add(this.searchStringTextBox);
          this.panelEnterSearch.Controls.Add(this.label1);
          this.panelEnterSearch.Location = new System.Drawing.Point(12, 11);
          this.panelEnterSearch.Name = "panelEnterSearch";
          this.panelEnterSearch.Size = new System.Drawing.Size(355, 95);
          this.panelEnterSearch.TabIndex = 23;
          // 
          // label6
          // 
          this.label6.AutoSize = true;
          this.label6.Location = new System.Drawing.Point(3, 68);
          this.label6.Name = "label6";
          this.label6.Size = new System.Drawing.Size(119, 13);
          this.label6.TabIndex = 29;
          this.label6.Text = "Filter On Camera Name:";
          // 
          // textBoxCameraNameFilter
          // 
          this.textBoxCameraNameFilter.Location = new System.Drawing.Point(143, 65);
          this.textBoxCameraNameFilter.Name = "textBoxCameraNameFilter";
          this.textBoxCameraNameFilter.Size = new System.Drawing.Size(190, 20);
          this.textBoxCameraNameFilter.TabIndex = 28;
          // 
          // textBoxMinMatch
          // 
          this.textBoxMinMatch.Location = new System.Drawing.Point(143, 33);
          this.textBoxMinMatch.Name = "textBoxMinMatch";
          this.textBoxMinMatch.Size = new System.Drawing.Size(57, 20);
          this.textBoxMinMatch.TabIndex = 27;
          // 
          // label7
          // 
          this.label7.AutoSize = true;
          this.label7.Location = new System.Drawing.Point(3, 37);
          this.label7.Name = "label7";
          this.label7.Size = new System.Drawing.Size(134, 13);
          this.label7.TabIndex = 26;
          this.label7.Text = "Minimum Match Threshold:";
          // 
          // doSearchButton
          // 
          this.doSearchButton.Location = new System.Drawing.Point(273, 6);
          this.doSearchButton.Name = "doSearchButton";
          this.doSearchButton.Size = new System.Drawing.Size(75, 20);
          this.doSearchButton.TabIndex = 25;
          this.doSearchButton.Text = "Search";
          this.doSearchButton.UseVisualStyleBackColor = true;
          this.doSearchButton.Click += new System.EventHandler(this.doSearchButton_Click);
          // 
          // searchStringTextBox
          // 
          this.searchStringTextBox.Location = new System.Drawing.Point(143, 6);
          this.searchStringTextBox.Name = "searchStringTextBox";
          this.searchStringTextBox.Size = new System.Drawing.Size(124, 20);
          this.searchStringTextBox.TabIndex = 24;
          this.searchStringTextBox.TextChanged += new System.EventHandler(this.searchStringTextBox_TextChanged);
          // 
          // label1
          // 
          this.label1.AutoSize = true;
          this.label1.Location = new System.Drawing.Point(3, 9);
          this.label1.Name = "label1";
          this.label1.Size = new System.Drawing.Size(134, 13);
          this.label1.TabIndex = 23;
          this.label1.Text = "Search For Plate Numbers:";
          // 
          // panelTime
          // 
          this.panelTime.BackColor = System.Drawing.SystemColors.ControlLight;
          this.panelTime.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
          this.panelTime.Controls.Add(this.labelLocalTime);
          this.panelTime.Controls.Add(this.label9);
          this.panelTime.Controls.Add(this.labelTimeZoneName);
          this.panelTime.Controls.Add(this.label8);
          this.panelTime.Controls.Add(this.currentTimeLabel);
          this.panelTime.Controls.Add(this.label5);
          this.panelTime.Controls.Add(this.label3);
          this.panelTime.Controls.Add(this.label2);
          this.panelTime.Controls.Add(this.dateTimePicker2);
          this.panelTime.Controls.Add(this.dateTimePicker1);
          this.panelTime.Location = new System.Drawing.Point(12, 112);
          this.panelTime.Name = "panelTime";
          this.panelTime.Size = new System.Drawing.Size(556, 117);
          this.panelTime.TabIndex = 24;
          // 
          // labelLocalTime
          // 
          this.labelLocalTime.AutoSize = true;
          this.labelLocalTime.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
          this.labelLocalTime.Location = new System.Drawing.Point(354, 95);
          this.labelLocalTime.Name = "labelLocalTime";
          this.labelLocalTime.Size = new System.Drawing.Size(2, 15);
          this.labelLocalTime.TabIndex = 24;
          // 
          // label9
          // 
          this.label9.AutoSize = true;
          this.label9.Location = new System.Drawing.Point(290, 95);
          this.label9.Name = "label9";
          this.label9.Size = new System.Drawing.Size(62, 13);
          this.label9.TabIndex = 23;
          this.label9.Text = "LocalTime :";
          // 
          // labelTimeZoneName
          // 
          this.labelTimeZoneName.AutoSize = true;
          this.labelTimeZoneName.Location = new System.Drawing.Point(386, 76);
          this.labelTimeZoneName.Name = "labelTimeZoneName";
          this.labelTimeZoneName.Size = new System.Drawing.Size(35, 13);
          this.labelTimeZoneName.TabIndex = 22;
          this.labelTimeZoneName.Text = "label9";
          // 
          // label8
          // 
          this.label8.AutoSize = true;
          this.label8.Location = new System.Drawing.Point(290, 76);
          this.label8.Name = "label8";
          this.label8.Size = new System.Drawing.Size(90, 13);
          this.label8.TabIndex = 21;
          this.label8.Text = "Local Time Zone:";
          // 
          // currentTimeLabel
          // 
          this.currentTimeLabel.AutoSize = true;
          this.currentTimeLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
          this.currentTimeLabel.Location = new System.Drawing.Point(104, 95);
          this.currentTimeLabel.Name = "currentTimeLabel";
          this.currentTimeLabel.Size = new System.Drawing.Size(2, 15);
          this.currentTimeLabel.TabIndex = 20;
          // 
          // label5
          // 
          this.label5.AutoSize = true;
          this.label5.Location = new System.Drawing.Point(0, 95);
          this.label5.Name = "label5";
          this.label5.Size = new System.Drawing.Size(98, 13);
          this.label5.TabIndex = 19;
          this.label5.Text = "Current UTC Time :";
          // 
          // label3
          // 
          this.label3.AutoSize = true;
          this.label3.Location = new System.Drawing.Point(337, 10);
          this.label3.Name = "label3";
          this.label3.Size = new System.Drawing.Size(83, 13);
          this.label3.TabIndex = 18;
          this.label3.Text = "End Time (UTC)";
          // 
          // label2
          // 
          this.label2.AutoSize = true;
          this.label2.Location = new System.Drawing.Point(12, 10);
          this.label2.Name = "label2";
          this.label2.Size = new System.Drawing.Size(89, 13);
          this.label2.TabIndex = 17;
          this.label2.Text = "Start Time (UTC) ";
          // 
          // dateTimePicker2
          // 
          this.dateTimePicker2.CalendarFont = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
          this.dateTimePicker2.Location = new System.Drawing.Point(340, 31);
          this.dateTimePicker2.Name = "dateTimePicker2";
          this.dateTimePicker2.Size = new System.Drawing.Size(200, 20);
          this.dateTimePicker2.TabIndex = 16;
          // 
          // dateTimePicker1
          // 
          this.dateTimePicker1.CalendarFont = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
          this.dateTimePicker1.Location = new System.Drawing.Point(12, 29);
          this.dateTimePicker1.Name = "dateTimePicker1";
          this.dateTimePicker1.Size = new System.Drawing.Size(200, 20);
          this.dateTimePicker1.TabIndex = 15;
          // 
          // panelResults
          // 
          this.panelResults.BackColor = System.Drawing.SystemColors.ControlLight;
          this.panelResults.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
          this.panelResults.Controls.Add(this.buttonUndoDelete);
          this.panelResults.Controls.Add(this.buttonExportTable);
          this.panelResults.Controls.Add(this.buttonPlayVideo);
          this.panelResults.Controls.Add(this.m_ZoomPictureControl);
          this.panelResults.Controls.Add(this.trackBarZoomControl);
          this.panelResults.Controls.Add(this.buttonPlotLocation);
          this.panelResults.Controls.Add(this.buttonExportImage);
          this.panelResults.Controls.Add(this.numItemsFoundLabel);
          this.panelResults.Controls.Add(this.label4);
          this.panelResults.Controls.Add(this.dataGridView1);
          this.panelResults.Location = new System.Drawing.Point(12, 235);
          this.panelResults.Name = "panelResults";
          this.panelResults.Size = new System.Drawing.Size(1140, 447);
          this.panelResults.TabIndex = 25;
          // 
          // buttonUndoDelete
          // 
          this.buttonUndoDelete.Location = new System.Drawing.Point(4, 418);
          this.buttonUndoDelete.Name = "buttonUndoDelete";
          this.buttonUndoDelete.Size = new System.Drawing.Size(114, 23);
          this.buttonUndoDelete.TabIndex = 23;
          this.buttonUndoDelete.Text = "Undo Last Delete";
          this.buttonUndoDelete.UseVisualStyleBackColor = true;
          this.buttonUndoDelete.Click += new System.EventHandler(this.buttonUndoDelete_Click);
          // 
          // buttonExportTable
          // 
          this.buttonExportTable.Location = new System.Drawing.Point(544, 18);
          this.buttonExportTable.Name = "buttonExportTable";
          this.buttonExportTable.Size = new System.Drawing.Size(91, 23);
          this.buttonExportTable.TabIndex = 22;
          this.buttonExportTable.Text = "ExportTable";
          this.buttonExportTable.UseVisualStyleBackColor = true;
          this.buttonExportTable.Click += new System.EventHandler(this.buttonExportTable_Click);
          // 
          // buttonPlayVideo
          // 
          this.buttonPlayVideo.Location = new System.Drawing.Point(1007, 418);
          this.buttonPlayVideo.Name = "buttonPlayVideo";
          this.buttonPlayVideo.Size = new System.Drawing.Size(117, 23);
          this.buttonPlayVideo.TabIndex = 21;
          this.buttonPlayVideo.Text = "Play Video";
          this.buttonPlayVideo.UseVisualStyleBackColor = true;
          this.buttonPlayVideo.Click += new System.EventHandler(this.buttonPlayVideo_Click);
          // 
          // m_ZoomPictureControl
          // 
          this.m_ZoomPictureControl.Location = new System.Drawing.Point(671, 80);
          this.m_ZoomPictureControl.Name = "m_ZoomPictureControl";
          this.m_ZoomPictureControl.Size = new System.Drawing.Size(453, 325);
          this.m_ZoomPictureControl.TabIndex = 19;
          // 
          // trackBarZoomControl
          // 
          this.trackBarZoomControl.Location = new System.Drawing.Point(671, 51);
          this.trackBarZoomControl.Name = "trackBarZoomControl";
          this.trackBarZoomControl.Size = new System.Drawing.Size(438, 45);
          this.trackBarZoomControl.TabIndex = 20;
          this.trackBarZoomControl.TickStyle = System.Windows.Forms.TickStyle.None;
          this.trackBarZoomControl.Scroll += new System.EventHandler(this.trackBarZoomControl_Scroll);
          // 
          // buttonPlotLocation
          // 
          this.buttonPlotLocation.Location = new System.Drawing.Point(518, 418);
          this.buttonPlotLocation.Name = "buttonPlotLocation";
          this.buttonPlotLocation.Size = new System.Drawing.Size(117, 23);
          this.buttonPlotLocation.TabIndex = 17;
          this.buttonPlotLocation.Text = "Plot Single Location";
          this.buttonPlotLocation.UseVisualStyleBackColor = true;
          this.buttonPlotLocation.Click += new System.EventHandler(this.buttonPlotLocation_Click);
          // 
          // buttonExportImage
          // 
          this.buttonExportImage.Location = new System.Drawing.Point(812, 22);
          this.buttonExportImage.Name = "buttonExportImage";
          this.buttonExportImage.Size = new System.Drawing.Size(111, 23);
          this.buttonExportImage.TabIndex = 16;
          this.buttonExportImage.Text = "Export Image";
          this.buttonExportImage.UseVisualStyleBackColor = true;
          this.buttonExportImage.Click += new System.EventHandler(this.buttonExportImage_Click);
          // 
          // numItemsFoundLabel
          // 
          this.numItemsFoundLabel.AutoSize = true;
          this.numItemsFoundLabel.Location = new System.Drawing.Point(112, 27);
          this.numItemsFoundLabel.Name = "numItemsFoundLabel";
          this.numItemsFoundLabel.Size = new System.Drawing.Size(31, 13);
          this.numItemsFoundLabel.TabIndex = 13;
          this.numItemsFoundLabel.Text = "none";
          // 
          // label4
          // 
          this.label4.AutoSize = true;
          this.label4.Location = new System.Drawing.Point(16, 27);
          this.label4.Name = "label4";
          this.label4.Size = new System.Drawing.Size(74, 13);
          this.label4.TabIndex = 12;
          this.label4.Text = "Items Found : ";
          // 
          // dataGridView1
          // 
          this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
          this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.SearchMatchLikelyhood,
            this.ScannedNumber,
            this.time,
            this.PSS,
            this.cameraName,
            this.imagePath,
            this.GPSLocation});
          this.dataGridView1.Location = new System.Drawing.Point(3, 47);
          this.dataGridView1.Name = "dataGridView1";
          this.dataGridView1.Size = new System.Drawing.Size(632, 358);
          this.dataGridView1.TabIndex = 11;
          this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
          // 
          // buttonDumpMotionEvents
          // 
          this.buttonDumpMotionEvents.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(150)))));
          this.buttonDumpMotionEvents.ForeColor = System.Drawing.Color.White;
          this.buttonDumpMotionEvents.Location = new System.Drawing.Point(390, 46);
          this.buttonDumpMotionEvents.Name = "buttonDumpMotionEvents";
          this.buttonDumpMotionEvents.Size = new System.Drawing.Size(178, 23);
          this.buttonDumpMotionEvents.TabIndex = 32;
          this.buttonDumpMotionEvents.Text = "All Motion Events In Range";
          this.buttonDumpMotionEvents.UseVisualStyleBackColor = false;
          this.buttonDumpMotionEvents.Click += new System.EventHandler(this.buttonDumpMotionEvents_Click);
          // 
          // buttonAllImagesInRange
          // 
          this.buttonAllImagesInRange.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(150)))));
          this.buttonAllImagesInRange.ForeColor = System.Drawing.Color.White;
          this.buttonAllImagesInRange.Location = new System.Drawing.Point(390, 83);
          this.buttonAllImagesInRange.Name = "buttonAllImagesInRange";
          this.buttonAllImagesInRange.Size = new System.Drawing.Size(178, 23);
          this.buttonAllImagesInRange.TabIndex = 33;
          this.buttonAllImagesInRange.Text = "All Image In Range";
          this.buttonAllImagesInRange.UseVisualStyleBackColor = false;
          this.buttonAllImagesInRange.Click += new System.EventHandler(this.buttonAllImagesInRange_Click);
          // 
          // buttonDumpRange
          // 
          this.buttonDumpRange.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(90)))), ((int)(((byte)(90)))), ((int)(((byte)(150)))));
          this.buttonDumpRange.ForeColor = System.Drawing.Color.White;
          this.buttonDumpRange.Location = new System.Drawing.Point(390, 12);
          this.buttonDumpRange.Name = "buttonDumpRange";
          this.buttonDumpRange.Size = new System.Drawing.Size(178, 23);
          this.buttonDumpRange.TabIndex = 34;
          this.buttonDumpRange.Text = "All Plates in Range";
          this.buttonDumpRange.UseVisualStyleBackColor = false;
          this.buttonDumpRange.Click += new System.EventHandler(this.buttonDumpRange_Click_1);
          // 
          // groupBox1
          // 
          this.groupBox1.Controls.Add(this.labelRepository);
          this.groupBox1.Location = new System.Drawing.Point(594, 12);
          this.groupBox1.Name = "groupBox1";
          this.groupBox1.Size = new System.Drawing.Size(558, 36);
          this.groupBox1.TabIndex = 37;
          this.groupBox1.TabStop = false;
          this.groupBox1.Text = "Current Repository";
          // 
          // labelRepository
          // 
          this.labelRepository.AutoSize = true;
          this.labelRepository.Location = new System.Drawing.Point(21, 16);
          this.labelRepository.Name = "labelRepository";
          this.labelRepository.Size = new System.Drawing.Size(132, 13);
          this.labelRepository.TabIndex = 36;
          this.labelRepository.Text = "Current Repository: not set";
          // 
          // SearchMatchLikelyhood
          // 
          this.SearchMatchLikelyhood.HeaderText = "Search Match Likelyhood";
          this.SearchMatchLikelyhood.Name = "SearchMatchLikelyhood";
          this.SearchMatchLikelyhood.Width = 62;
          // 
          // ScannedNumber
          // 
          dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.TopCenter;
          this.ScannedNumber.DefaultCellStyle = dataGridViewCellStyle2;
          this.ScannedNumber.HeaderText = "Search Tag";
          this.ScannedNumber.Name = "ScannedNumber";
          this.ScannedNumber.Width = 75;
          // 
          // time
          // 
          this.time.HeaderText = "Time";
          this.time.Name = "time";
          this.time.Width = 207;
          // 
          // PSS
          // 
          this.PSS.HeaderText = "PSS";
          this.PSS.Name = "PSS";
          this.PSS.Width = 98;
          // 
          // cameraName
          // 
          this.cameraName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
          this.cameraName.HeaderText = "Camera Name";
          this.cameraName.Name = "cameraName";
          // 
          // imagePath
          // 
          this.imagePath.HeaderText = "ImagePath";
          this.imagePath.Name = "imagePath";
          this.imagePath.Visible = false;
          // 
          // GPSLocation
          // 
          this.GPSLocation.HeaderText = "GPSLocation";
          this.GPSLocation.Name = "GPSLocation";
          this.GPSLocation.Visible = false;
          // 
          // SmartSearchLibUC
          // 
          this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
          this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
          this.Controls.Add(this.groupBox1);
          this.Controls.Add(this.buttonDumpRange);
          this.Controls.Add(this.buttonAllImagesInRange);
          this.Controls.Add(this.buttonDumpMotionEvents);
          this.Controls.Add(this.panelResults);
          this.Controls.Add(this.panelTime);
          this.Controls.Add(this.panelEnterSearch);
          this.ForeColor = System.Drawing.Color.Black;
          this.Name = "SmartSearchLibUC";
          this.Size = new System.Drawing.Size(1192, 702);
          this.Load += new System.EventHandler(this.search_Load);
          this.panelEnterSearch.ResumeLayout(false);
          this.panelEnterSearch.PerformLayout();
          this.panelTime.ResumeLayout(false);
          this.panelTime.PerformLayout();
          this.panelResults.ResumeLayout(false);
          this.panelResults.PerformLayout();
          ((System.ComponentModel.ISupportInitialize)(this.trackBarZoomControl)).EndInit();
          ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
          this.groupBox1.ResumeLayout(false);
          this.groupBox1.PerformLayout();
          this.ResumeLayout(false);

      }

      #endregion

      private System.Windows.Forms.Timer timer1;
      private System.Windows.Forms.SaveFileDialog saveFileDialog1;
      private System.Windows.Forms.Panel panelEnterSearch;
      private System.Windows.Forms.TextBox textBoxMinMatch;
      private System.Windows.Forms.Label label7;
      private System.Windows.Forms.Button doSearchButton;
      private System.Windows.Forms.TextBox searchStringTextBox;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.Panel panelTime;
      private System.Windows.Forms.Label currentTimeLabel;
      private System.Windows.Forms.Label label5;
      private System.Windows.Forms.Label label3;
      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.DateTimePicker dateTimePicker2;
      private System.Windows.Forms.DateTimePicker dateTimePicker1;
      private System.Windows.Forms.Panel panelResults;
      private System.Windows.Forms.Label numItemsFoundLabel;
      private System.Windows.Forms.Label label4;
      private System.Windows.Forms.DataGridView dataGridView1;
      private System.Windows.Forms.Button buttonExportImage;
      private System.Windows.Forms.Timer timerStopForm;
      private System.Windows.Forms.Button buttonPlotLocation;
      private ScalablePictureBoxTool.ScalablePictureBox m_ZoomPictureControl;
      private System.Windows.Forms.TrackBar trackBarZoomControl;
      private System.Windows.Forms.Button buttonPlayVideo;
      private System.Windows.Forms.Button buttonDumpMotionEvents;
      private System.Windows.Forms.Button buttonAllImagesInRange;
      private System.Windows.Forms.Button buttonDumpRange;
      private System.Windows.Forms.Button buttonUndoDelete;
      private System.Windows.Forms.Button buttonExportTable;
      private System.Windows.Forms.Label label6;
      private System.Windows.Forms.TextBox textBoxCameraNameFilter;
      private System.Windows.Forms.Label labelLocalTime;
      private System.Windows.Forms.Label label9;
      private System.Windows.Forms.Label labelTimeZoneName;
      private System.Windows.Forms.Label label8;
      private System.Windows.Forms.GroupBox groupBox1;
      private System.Windows.Forms.Label labelRepository;
      private System.Windows.Forms.DataGridViewTextBoxColumn SearchMatchLikelyhood;
      private System.Windows.Forms.DataGridViewTextBoxColumn ScannedNumber;
      private System.Windows.Forms.DataGridViewTextBoxColumn time;
      private System.Windows.Forms.DataGridViewTextBoxColumn PSS;
      private System.Windows.Forms.DataGridViewTextBoxColumn cameraName;
      private System.Windows.Forms.DataGridViewTextBoxColumn imagePath;
      private System.Windows.Forms.DataGridViewTextBoxColumn GPSLocation;
   }
}