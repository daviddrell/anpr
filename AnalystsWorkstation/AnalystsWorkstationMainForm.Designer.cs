namespace AnalystsWorkstation
{
    partial class AnalystsWorkstationMainForm
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
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.tabPageOCRLib = new System.Windows.Forms.TabPage();
            this.buttonOCRGenLib = new System.Windows.Forms.Button();
            this.labelOCRDestinationFolder = new System.Windows.Forms.Label();
            this.labelOCRSourceFolder = new System.Windows.Forms.Label();
            this.buttonSelectOCRSourceFolder = new System.Windows.Forms.Button();
            this.buttonSelectOCRDestinationFolder = new System.Windows.Forms.Button();
            this.tabPageLPRDiagnostics = new System.Windows.Forms.TabPage();
            this.pictureBoxDiagHistogram = new System.Windows.Forms.PictureBox();
            this.pictureBoxDiag3 = new System.Windows.Forms.PictureBox();
            this.listBoxRejectLog = new System.Windows.Forms.ListBox();
            this.pictureBoxDiag2 = new System.Windows.Forms.PictureBox();
            this.tabControlDiagdisplay = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.pictureBoxDiag1 = new System.Windows.Forms.PictureBox();
            this.tabPageImportImages = new System.Windows.Forms.TabPage();
            this.tabPageEditMode = new System.Windows.Forms.TabPage();
            this.labelRepositoryStatus = new System.Windows.Forms.Label();
            this.buttonStoreResults = new System.Windows.Forms.Button();
            this.buttonLoadSingleFile = new System.Windows.Forms.Button();
            this.buttonLoadDirectory = new System.Windows.Forms.Button();
            this.buttonProcessNow = new System.Windows.Forms.Button();
            this.checkBoxProcessWhenOpened = new System.Windows.Forms.CheckBox();
            this.tabControlLPRResults = new System.Windows.Forms.TabControl();
            this.tabPageLPRResults = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.buttonListViewVisible = new System.Windows.Forms.Button();
            this.labelCurrentFileName = new System.Windows.Forms.Label();
            this.labelCurrentIndex = new System.Windows.Forms.Label();
            this.buttonPrevious = new System.Windows.Forms.Button();
            this.buttonNext = new System.Windows.Forms.Button();
            this.labelNumberOfImagesLoaded = new System.Windows.Forms.Label();
            this.pictureBoxCurrentImage = new System.Windows.Forms.PictureBox();
            this.buttonLoadListFromSearchResults = new System.Windows.Forms.Button();
            this.tabPageBatchProcess = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBoxUserSpecifiedStorageLocation = new System.Windows.Forms.TextBox();
            this.radioButtonToUserSpecifiedStorage = new System.Windows.Forms.RadioButton();
            this.radioToRepository = new System.Windows.Forms.RadioButton();
            this.listBoxDisplayTimeStamps = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxUserSpecifiedCameraName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBoxVideoFiles = new System.Windows.Forms.GroupBox();
            this.buttonSelectJpegDirectory = new System.Windows.Forms.Button();
            this.buttonAddDirectory = new System.Windows.Forms.Button();
            this.buttonSelectFile = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButtonStoreOnPlate = new System.Windows.Forms.RadioButton();
            this.radioButtonStoreOnMotion = new System.Windows.Forms.RadioButton();
            this.buttonStart = new System.Windows.Forms.Button();
            this.buttonStop = new System.Windows.Forms.Button();
            this.dataGridViewFilesInProcess = new System.Windows.Forms.DataGridView();
            this.tabPageSearch = new System.Windows.Forms.TabPage();
            this.tabControlMain = new System.Windows.Forms.TabControl();
            this.tabPageOCRLib.SuspendLayout();
            this.tabPageLPRDiagnostics.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDiagHistogram)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDiag3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDiag2)).BeginInit();
            this.tabControlDiagdisplay.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDiag1)).BeginInit();
            this.tabPageEditMode.SuspendLayout();
            this.tabControlLPRResults.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCurrentImage)).BeginInit();
            this.tabPageBatchProcess.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBoxVideoFiles.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewFilesInProcess)).BeginInit();
            this.tabControlMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // tabPageOCRLib
            // 
            this.tabPageOCRLib.Controls.Add(this.buttonOCRGenLib);
            this.tabPageOCRLib.Controls.Add(this.labelOCRDestinationFolder);
            this.tabPageOCRLib.Controls.Add(this.labelOCRSourceFolder);
            this.tabPageOCRLib.Controls.Add(this.buttonSelectOCRSourceFolder);
            this.tabPageOCRLib.Controls.Add(this.buttonSelectOCRDestinationFolder);
            this.tabPageOCRLib.Location = new System.Drawing.Point(4, 22);
            this.tabPageOCRLib.Name = "tabPageOCRLib";
            this.tabPageOCRLib.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageOCRLib.Size = new System.Drawing.Size(1196, 691);
            this.tabPageOCRLib.TabIndex = 5;
            this.tabPageOCRLib.Text = "OCR lib";
            this.tabPageOCRLib.UseVisualStyleBackColor = true;
            // 
            // buttonOCRGenLib
            // 
            this.buttonOCRGenLib.Location = new System.Drawing.Point(157, 278);
            this.buttonOCRGenLib.Name = "buttonOCRGenLib";
            this.buttonOCRGenLib.Size = new System.Drawing.Size(180, 23);
            this.buttonOCRGenLib.TabIndex = 4;
            this.buttonOCRGenLib.Text = "Generate Library";
            this.buttonOCRGenLib.UseVisualStyleBackColor = true;
            this.buttonOCRGenLib.Click += new System.EventHandler(this.buttonOCRGenLib_Click);
            // 
            // labelOCRDestinationFolder
            // 
            this.labelOCRDestinationFolder.AutoSize = true;
            this.labelOCRDestinationFolder.Location = new System.Drawing.Point(360, 168);
            this.labelOCRDestinationFolder.Name = "labelOCRDestinationFolder";
            this.labelOCRDestinationFolder.Size = new System.Drawing.Size(25, 13);
            this.labelOCRDestinationFolder.TabIndex = 3;
            this.labelOCRDestinationFolder.Text = "___";
            // 
            // labelOCRSourceFolder
            // 
            this.labelOCRSourceFolder.AutoSize = true;
            this.labelOCRSourceFolder.Location = new System.Drawing.Point(360, 125);
            this.labelOCRSourceFolder.Name = "labelOCRSourceFolder";
            this.labelOCRSourceFolder.Size = new System.Drawing.Size(19, 13);
            this.labelOCRSourceFolder.TabIndex = 2;
            this.labelOCRSourceFolder.Text = "__";
            // 
            // buttonSelectOCRSourceFolder
            // 
            this.buttonSelectOCRSourceFolder.Location = new System.Drawing.Point(157, 120);
            this.buttonSelectOCRSourceFolder.Name = "buttonSelectOCRSourceFolder";
            this.buttonSelectOCRSourceFolder.Size = new System.Drawing.Size(180, 23);
            this.buttonSelectOCRSourceFolder.TabIndex = 1;
            this.buttonSelectOCRSourceFolder.Text = "Select Source Folder";
            this.buttonSelectOCRSourceFolder.UseVisualStyleBackColor = true;
            this.buttonSelectOCRSourceFolder.Click += new System.EventHandler(this.buttonSelectOCRSourceFolder_Click);
            // 
            // buttonSelectOCRDestinationFolder
            // 
            this.buttonSelectOCRDestinationFolder.Location = new System.Drawing.Point(157, 163);
            this.buttonSelectOCRDestinationFolder.Name = "buttonSelectOCRDestinationFolder";
            this.buttonSelectOCRDestinationFolder.Size = new System.Drawing.Size(180, 23);
            this.buttonSelectOCRDestinationFolder.TabIndex = 0;
            this.buttonSelectOCRDestinationFolder.Text = "Select Destination Folder";
            this.buttonSelectOCRDestinationFolder.UseVisualStyleBackColor = true;
            this.buttonSelectOCRDestinationFolder.Click += new System.EventHandler(this.buttonSelectOCRDestinationFolder_Click);
            // 
            // tabPageLPRDiagnostics
            // 
            this.tabPageLPRDiagnostics.Controls.Add(this.pictureBoxDiagHistogram);
            this.tabPageLPRDiagnostics.Controls.Add(this.pictureBoxDiag3);
            this.tabPageLPRDiagnostics.Controls.Add(this.listBoxRejectLog);
            this.tabPageLPRDiagnostics.Controls.Add(this.pictureBoxDiag2);
            this.tabPageLPRDiagnostics.Controls.Add(this.tabControlDiagdisplay);
            this.tabPageLPRDiagnostics.Controls.Add(this.pictureBoxDiag1);
            this.tabPageLPRDiagnostics.Location = new System.Drawing.Point(4, 22);
            this.tabPageLPRDiagnostics.Name = "tabPageLPRDiagnostics";
            this.tabPageLPRDiagnostics.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageLPRDiagnostics.Size = new System.Drawing.Size(1196, 691);
            this.tabPageLPRDiagnostics.TabIndex = 4;
            this.tabPageLPRDiagnostics.Text = "Diagnostics";
            this.tabPageLPRDiagnostics.UseVisualStyleBackColor = true;
            // 
            // pictureBoxDiagHistogram
            // 
            this.pictureBoxDiagHistogram.Location = new System.Drawing.Point(617, 24);
            this.pictureBoxDiagHistogram.Name = "pictureBoxDiagHistogram";
            this.pictureBoxDiagHistogram.Size = new System.Drawing.Size(169, 170);
            this.pictureBoxDiagHistogram.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxDiagHistogram.TabIndex = 13;
            this.pictureBoxDiagHistogram.TabStop = false;
            this.pictureBoxDiagHistogram.Click += new System.EventHandler(this.pictureBoxDiagHistogram_Click);
            // 
            // pictureBoxDiag3
            // 
            this.pictureBoxDiag3.Location = new System.Drawing.Point(425, 24);
            this.pictureBoxDiag3.Name = "pictureBoxDiag3";
            this.pictureBoxDiag3.Size = new System.Drawing.Size(169, 170);
            this.pictureBoxDiag3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxDiag3.TabIndex = 12;
            this.pictureBoxDiag3.TabStop = false;
            this.pictureBoxDiag3.Click += new System.EventHandler(this.pictureBoxDiag3_Click);
            // 
            // listBoxRejectLog
            // 
            this.listBoxRejectLog.FormattingEnabled = true;
            this.listBoxRejectLog.Location = new System.Drawing.Point(991, 24);
            this.listBoxRejectLog.Name = "listBoxRejectLog";
            this.listBoxRejectLog.Size = new System.Drawing.Size(343, 186);
            this.listBoxRejectLog.TabIndex = 11;
            // 
            // pictureBoxDiag2
            // 
            this.pictureBoxDiag2.Location = new System.Drawing.Point(230, 24);
            this.pictureBoxDiag2.Name = "pictureBoxDiag2";
            this.pictureBoxDiag2.Size = new System.Drawing.Size(169, 170);
            this.pictureBoxDiag2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxDiag2.TabIndex = 10;
            this.pictureBoxDiag2.TabStop = false;
            this.pictureBoxDiag2.Click += new System.EventHandler(this.pictureBoxDiag2_Click);
            // 
            // tabControlDiagdisplay
            // 
            this.tabControlDiagdisplay.Controls.Add(this.tabPage1);
            this.tabControlDiagdisplay.Controls.Add(this.tabPage3);
            this.tabControlDiagdisplay.Location = new System.Drawing.Point(30, 230);
            this.tabControlDiagdisplay.Name = "tabControlDiagdisplay";
            this.tabControlDiagdisplay.SelectedIndex = 0;
            this.tabControlDiagdisplay.Size = new System.Drawing.Size(1007, 400);
            this.tabControlDiagdisplay.TabIndex = 9;
            // 
            // tabPage1
            // 
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(999, 374);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(999, 374);
            this.tabPage3.TabIndex = 1;
            this.tabPage3.Text = "tabPage3";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // pictureBoxDiag1
            // 
            this.pictureBoxDiag1.Location = new System.Drawing.Point(34, 24);
            this.pictureBoxDiag1.Name = "pictureBoxDiag1";
            this.pictureBoxDiag1.Size = new System.Drawing.Size(170, 170);
            this.pictureBoxDiag1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxDiag1.TabIndex = 0;
            this.pictureBoxDiag1.TabStop = false;
            this.pictureBoxDiag1.Click += new System.EventHandler(this.pictureBoxDiag1_Click);
            // 
            // tabPageImportImages
            // 
            this.tabPageImportImages.Location = new System.Drawing.Point(4, 22);
            this.tabPageImportImages.Name = "tabPageImportImages";
            this.tabPageImportImages.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageImportImages.Size = new System.Drawing.Size(1196, 691);
            this.tabPageImportImages.TabIndex = 3;
            this.tabPageImportImages.Text = "Import Images";
            this.tabPageImportImages.UseVisualStyleBackColor = true;
            // 
            // tabPageEditMode
            // 
            this.tabPageEditMode.Controls.Add(this.labelRepositoryStatus);
            this.tabPageEditMode.Controls.Add(this.buttonStoreResults);
            this.tabPageEditMode.Controls.Add(this.buttonLoadSingleFile);
            this.tabPageEditMode.Controls.Add(this.buttonLoadDirectory);
            this.tabPageEditMode.Controls.Add(this.buttonProcessNow);
            this.tabPageEditMode.Controls.Add(this.checkBoxProcessWhenOpened);
            this.tabPageEditMode.Controls.Add(this.tabControlLPRResults);
            this.tabPageEditMode.Controls.Add(this.buttonListViewVisible);
            this.tabPageEditMode.Controls.Add(this.labelCurrentFileName);
            this.tabPageEditMode.Controls.Add(this.labelCurrentIndex);
            this.tabPageEditMode.Controls.Add(this.buttonPrevious);
            this.tabPageEditMode.Controls.Add(this.buttonNext);
            this.tabPageEditMode.Controls.Add(this.labelNumberOfImagesLoaded);
            this.tabPageEditMode.Controls.Add(this.pictureBoxCurrentImage);
            this.tabPageEditMode.Controls.Add(this.buttonLoadListFromSearchResults);
            this.tabPageEditMode.Location = new System.Drawing.Point(4, 22);
            this.tabPageEditMode.Name = "tabPageEditMode";
            this.tabPageEditMode.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageEditMode.Size = new System.Drawing.Size(1196, 691);
            this.tabPageEditMode.TabIndex = 2;
            this.tabPageEditMode.Text = "Edit Mode";
            this.tabPageEditMode.UseVisualStyleBackColor = true;
            this.tabPageEditMode.Click += new System.EventHandler(this.tabPageEditMode_Click);
            // 
            // labelRepositoryStatus
            // 
            this.labelRepositoryStatus.AutoSize = true;
            this.labelRepositoryStatus.Location = new System.Drawing.Point(32, 355);
            this.labelRepositoryStatus.Name = "labelRepositoryStatus";
            this.labelRepositoryStatus.Size = new System.Drawing.Size(132, 13);
            this.labelRepositoryStatus.TabIndex = 14;
            this.labelRepositoryStatus.Text = "Repository Not Connected";
            // 
            // buttonStoreResults
            // 
            this.buttonStoreResults.Location = new System.Drawing.Point(26, 329);
            this.buttonStoreResults.Name = "buttonStoreResults";
            this.buttonStoreResults.Size = new System.Drawing.Size(154, 23);
            this.buttonStoreResults.TabIndex = 13;
            this.buttonStoreResults.Text = "Store Results";
            this.buttonStoreResults.UseVisualStyleBackColor = true;
            this.buttonStoreResults.Click += new System.EventHandler(this.buttonStoreResults_Click);
            // 
            // buttonLoadSingleFile
            // 
            this.buttonLoadSingleFile.Location = new System.Drawing.Point(26, 31);
            this.buttonLoadSingleFile.Name = "buttonLoadSingleFile";
            this.buttonLoadSingleFile.Size = new System.Drawing.Size(154, 23);
            this.buttonLoadSingleFile.TabIndex = 12;
            this.buttonLoadSingleFile.Text = "Load File";
            this.buttonLoadSingleFile.UseVisualStyleBackColor = true;
            this.buttonLoadSingleFile.Click += new System.EventHandler(this.buttonLoadSingleFile_Click);
            // 
            // buttonLoadDirectory
            // 
            this.buttonLoadDirectory.Location = new System.Drawing.Point(26, 58);
            this.buttonLoadDirectory.Name = "buttonLoadDirectory";
            this.buttonLoadDirectory.Size = new System.Drawing.Size(154, 23);
            this.buttonLoadDirectory.TabIndex = 11;
            this.buttonLoadDirectory.Text = "Load Directory";
            this.buttonLoadDirectory.UseVisualStyleBackColor = true;
            this.buttonLoadDirectory.Click += new System.EventHandler(this.buttonLoadDirectory_Click);
            // 
            // buttonProcessNow
            // 
            this.buttonProcessNow.Location = new System.Drawing.Point(26, 239);
            this.buttonProcessNow.Name = "buttonProcessNow";
            this.buttonProcessNow.Size = new System.Drawing.Size(154, 23);
            this.buttonProcessNow.TabIndex = 10;
            this.buttonProcessNow.Text = "Process Now";
            this.buttonProcessNow.UseVisualStyleBackColor = true;
            this.buttonProcessNow.Click += new System.EventHandler(this.buttonProcessNow_Click);
            // 
            // checkBoxProcessWhenOpened
            // 
            this.checkBoxProcessWhenOpened.AutoSize = true;
            this.checkBoxProcessWhenOpened.Location = new System.Drawing.Point(26, 216);
            this.checkBoxProcessWhenOpened.Name = "checkBoxProcessWhenOpened";
            this.checkBoxProcessWhenOpened.Size = new System.Drawing.Size(138, 17);
            this.checkBoxProcessWhenOpened.TabIndex = 9;
            this.checkBoxProcessWhenOpened.Text = "Process when Selected";
            this.checkBoxProcessWhenOpened.UseVisualStyleBackColor = true;
            // 
            // tabControlLPRResults
            // 
            this.tabControlLPRResults.Controls.Add(this.tabPageLPRResults);
            this.tabControlLPRResults.Controls.Add(this.tabPage2);
            this.tabControlLPRResults.Location = new System.Drawing.Point(222, 217);
            this.tabControlLPRResults.Name = "tabControlLPRResults";
            this.tabControlLPRResults.SelectedIndex = 0;
            this.tabControlLPRResults.Size = new System.Drawing.Size(1007, 468);
            this.tabControlLPRResults.TabIndex = 8;
            // 
            // tabPageLPRResults
            // 
            this.tabPageLPRResults.Location = new System.Drawing.Point(4, 22);
            this.tabPageLPRResults.Name = "tabPageLPRResults";
            this.tabPageLPRResults.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageLPRResults.Size = new System.Drawing.Size(999, 442);
            this.tabPageLPRResults.TabIndex = 0;
            this.tabPageLPRResults.Text = "tabPage1";
            this.tabPageLPRResults.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(999, 442);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage2";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // buttonListViewVisible
            // 
            this.buttonListViewVisible.Location = new System.Drawing.Point(26, 171);
            this.buttonListViewVisible.Name = "buttonListViewVisible";
            this.buttonListViewVisible.Size = new System.Drawing.Size(154, 23);
            this.buttonListViewVisible.TabIndex = 7;
            this.buttonListViewVisible.Text = "ShowThumbnails";
            this.buttonListViewVisible.UseVisualStyleBackColor = true;
            this.buttonListViewVisible.Click += new System.EventHandler(this.buttonListViewVisible_Click);
            // 
            // labelCurrentFileName
            // 
            this.labelCurrentFileName.AutoSize = true;
            this.labelCurrentFileName.Location = new System.Drawing.Point(223, 26);
            this.labelCurrentFileName.Name = "labelCurrentFileName";
            this.labelCurrentFileName.Size = new System.Drawing.Size(70, 13);
            this.labelCurrentFileName.TabIndex = 6;
            this.labelCurrentFileName.Text = "no file loaded";
            // 
            // labelCurrentIndex
            // 
            this.labelCurrentIndex.AutoSize = true;
            this.labelCurrentIndex.Location = new System.Drawing.Point(223, 13);
            this.labelCurrentIndex.Name = "labelCurrentIndex";
            this.labelCurrentIndex.Size = new System.Drawing.Size(13, 13);
            this.labelCurrentIndex.TabIndex = 5;
            this.labelCurrentIndex.Text = "0";
            // 
            // buttonPrevious
            // 
            this.buttonPrevious.Location = new System.Drawing.Point(58, 268);
            this.buttonPrevious.Name = "buttonPrevious";
            this.buttonPrevious.Size = new System.Drawing.Size(36, 23);
            this.buttonPrevious.TabIndex = 4;
            this.buttonPrevious.Text = "<";
            this.buttonPrevious.UseVisualStyleBackColor = true;
            this.buttonPrevious.Click += new System.EventHandler(this.buttonPrevious_Click);
            // 
            // buttonNext
            // 
            this.buttonNext.Location = new System.Drawing.Point(100, 268);
            this.buttonNext.Name = "buttonNext";
            this.buttonNext.Size = new System.Drawing.Size(36, 23);
            this.buttonNext.TabIndex = 3;
            this.buttonNext.Text = ">";
            this.buttonNext.UseVisualStyleBackColor = true;
            this.buttonNext.Click += new System.EventHandler(this.buttonNext_Click);
            // 
            // labelNumberOfImagesLoaded
            // 
            this.labelNumberOfImagesLoaded.AutoSize = true;
            this.labelNumberOfImagesLoaded.Location = new System.Drawing.Point(55, 155);
            this.labelNumberOfImagesLoaded.Name = "labelNumberOfImagesLoaded";
            this.labelNumberOfImagesLoaded.Size = new System.Drawing.Size(97, 13);
            this.labelNumberOfImagesLoaded.TabIndex = 2;
            this.labelNumberOfImagesLoaded.Text = "No Images Loaded";
            // 
            // pictureBoxCurrentImage
            // 
            this.pictureBoxCurrentImage.BackColor = System.Drawing.Color.Black;
            this.pictureBoxCurrentImage.Location = new System.Drawing.Point(222, 42);
            this.pictureBoxCurrentImage.Name = "pictureBoxCurrentImage";
            this.pictureBoxCurrentImage.Size = new System.Drawing.Size(286, 154);
            this.pictureBoxCurrentImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBoxCurrentImage.TabIndex = 1;
            this.pictureBoxCurrentImage.TabStop = false;
            this.pictureBoxCurrentImage.Click += new System.EventHandler(this.pictureBoxCurrentImage_Click);
            // 
            // buttonLoadListFromSearchResults
            // 
            this.buttonLoadListFromSearchResults.Location = new System.Drawing.Point(26, 87);
            this.buttonLoadListFromSearchResults.Name = "buttonLoadListFromSearchResults";
            this.buttonLoadListFromSearchResults.Size = new System.Drawing.Size(154, 23);
            this.buttonLoadListFromSearchResults.TabIndex = 0;
            this.buttonLoadListFromSearchResults.Text = "Load Search Results";
            this.buttonLoadListFromSearchResults.UseVisualStyleBackColor = true;
            this.buttonLoadListFromSearchResults.Click += new System.EventHandler(this.buttonLoadListFromSearchResults_Click);
            // 
            // tabPageBatchProcess
            // 
            this.tabPageBatchProcess.Controls.Add(this.groupBox2);
            this.tabPageBatchProcess.Controls.Add(this.listBoxDisplayTimeStamps);
            this.tabPageBatchProcess.Controls.Add(this.label2);
            this.tabPageBatchProcess.Controls.Add(this.textBoxUserSpecifiedCameraName);
            this.tabPageBatchProcess.Controls.Add(this.label1);
            this.tabPageBatchProcess.Controls.Add(this.groupBoxVideoFiles);
            this.tabPageBatchProcess.Controls.Add(this.groupBox1);
            this.tabPageBatchProcess.Controls.Add(this.buttonStart);
            this.tabPageBatchProcess.Controls.Add(this.buttonStop);
            this.tabPageBatchProcess.Controls.Add(this.dataGridViewFilesInProcess);
            this.tabPageBatchProcess.Location = new System.Drawing.Point(4, 22);
            this.tabPageBatchProcess.Name = "tabPageBatchProcess";
            this.tabPageBatchProcess.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageBatchProcess.Size = new System.Drawing.Size(1196, 691);
            this.tabPageBatchProcess.TabIndex = 1;
            this.tabPageBatchProcess.Text = "Batch Process";
            this.tabPageBatchProcess.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBoxUserSpecifiedStorageLocation);
            this.groupBox2.Controls.Add(this.radioButtonToUserSpecifiedStorage);
            this.groupBox2.Controls.Add(this.radioToRepository);
            this.groupBox2.Location = new System.Drawing.Point(53, 266);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(196, 139);
            this.groupBox2.TabIndex = 14;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Select Output";
            // 
            // textBoxUserSpecifiedStorageLocation
            // 
            this.textBoxUserSpecifiedStorageLocation.Location = new System.Drawing.Point(7, 102);
            this.textBoxUserSpecifiedStorageLocation.Name = "textBoxUserSpecifiedStorageLocation";
            this.textBoxUserSpecifiedStorageLocation.Size = new System.Drawing.Size(179, 20);
            this.textBoxUserSpecifiedStorageLocation.TabIndex = 2;
            // 
            // radioButtonToUserSpecifiedStorage
            // 
            this.radioButtonToUserSpecifiedStorage.AutoSize = true;
            this.radioButtonToUserSpecifiedStorage.Location = new System.Drawing.Point(26, 61);
            this.radioButtonToUserSpecifiedStorage.Name = "radioButtonToUserSpecifiedStorage";
            this.radioButtonToUserSpecifiedStorage.Size = new System.Drawing.Size(101, 17);
            this.radioButtonToUserSpecifiedStorage.TabIndex = 1;
            this.radioButtonToUserSpecifiedStorage.TabStop = true;
            this.radioButtonToUserSpecifiedStorage.Text = "Specify Folder...";
            this.radioButtonToUserSpecifiedStorage.UseVisualStyleBackColor = true;
            this.radioButtonToUserSpecifiedStorage.CheckedChanged += new System.EventHandler(this.radioButtonToUserSpecifiedStorage_CheckedChanged);
            // 
            // radioToRepository
            // 
            this.radioToRepository.AutoSize = true;
            this.radioToRepository.Location = new System.Drawing.Point(26, 31);
            this.radioToRepository.Name = "radioToRepository";
            this.radioToRepository.Size = new System.Drawing.Size(91, 17);
            this.radioToRepository.TabIndex = 0;
            this.radioToRepository.TabStop = true;
            this.radioToRepository.Text = "To Repository";
            this.radioToRepository.UseVisualStyleBackColor = true;
            this.radioToRepository.CheckedChanged += new System.EventHandler(this.radioToRepository_CheckedChanged);
            // 
            // listBoxDisplayTimeStamps
            // 
            this.listBoxDisplayTimeStamps.FormattingEnabled = true;
            this.listBoxDisplayTimeStamps.Location = new System.Drawing.Point(33, 548);
            this.listBoxDisplayTimeStamps.Name = "listBoxDisplayTimeStamps";
            this.listBoxDisplayTimeStamps.Size = new System.Drawing.Size(220, 121);
            this.listBoxDisplayTimeStamps.TabIndex = 13;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(30, 522);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(103, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Image Time Stamps:";
            // 
            // textBoxUserSpecifiedCameraName
            // 
            this.textBoxUserSpecifiedCameraName.Location = new System.Drawing.Point(30, 489);
            this.textBoxUserSpecifiedCameraName.Name = "textBoxUserSpecifiedCameraName";
            this.textBoxUserSpecifiedCameraName.Size = new System.Drawing.Size(222, 20);
            this.textBoxUserSpecifiedCameraName.TabIndex = 11;
            this.textBoxUserSpecifiedCameraName.TextChanged += new System.EventHandler(this.textBoxUserSpecifiedCameraName_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, 463);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(99, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Use Camera Name:";
            // 
            // groupBoxVideoFiles
            // 
            this.groupBoxVideoFiles.Controls.Add(this.buttonSelectJpegDirectory);
            this.groupBoxVideoFiles.Controls.Add(this.buttonAddDirectory);
            this.groupBoxVideoFiles.Controls.Add(this.buttonSelectFile);
            this.groupBoxVideoFiles.Location = new System.Drawing.Point(52, 19);
            this.groupBoxVideoFiles.Name = "groupBoxVideoFiles";
            this.groupBoxVideoFiles.Size = new System.Drawing.Size(200, 135);
            this.groupBoxVideoFiles.TabIndex = 8;
            this.groupBoxVideoFiles.TabStop = false;
            this.groupBoxVideoFiles.Text = "Select Batch Source";
            // 
            // buttonSelectJpegDirectory
            // 
            this.buttonSelectJpegDirectory.Location = new System.Drawing.Point(12, 83);
            this.buttonSelectJpegDirectory.Name = "buttonSelectJpegDirectory";
            this.buttonSelectJpegDirectory.Size = new System.Drawing.Size(175, 23);
            this.buttonSelectJpegDirectory.TabIndex = 6;
            this.buttonSelectJpegDirectory.Text = "Select Directory of Jpegs";
            this.buttonSelectJpegDirectory.UseVisualStyleBackColor = true;
            this.buttonSelectJpegDirectory.Click += new System.EventHandler(this.buttonSelectJpegDirectory_Click);
            // 
            // buttonAddDirectory
            // 
            this.buttonAddDirectory.Location = new System.Drawing.Point(12, 54);
            this.buttonAddDirectory.Name = "buttonAddDirectory";
            this.buttonAddDirectory.Size = new System.Drawing.Size(175, 23);
            this.buttonAddDirectory.TabIndex = 5;
            this.buttonAddDirectory.Text = "Select Directory of Video Files";
            this.buttonAddDirectory.UseVisualStyleBackColor = true;
            this.buttonAddDirectory.Click += new System.EventHandler(this.buttonAddDirectory_Click);
            // 
            // buttonSelectFile
            // 
            this.buttonSelectFile.Location = new System.Drawing.Point(12, 24);
            this.buttonSelectFile.Name = "buttonSelectFile";
            this.buttonSelectFile.Size = new System.Drawing.Size(175, 23);
            this.buttonSelectFile.TabIndex = 4;
            this.buttonSelectFile.Text = "Select Single Video File";
            this.buttonSelectFile.UseVisualStyleBackColor = true;
            this.buttonSelectFile.Click += new System.EventHandler(this.buttonSelectFile_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButtonStoreOnPlate);
            this.groupBox1.Controls.Add(this.radioButtonStoreOnMotion);
            this.groupBox1.Location = new System.Drawing.Point(53, 160);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(196, 100);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Frame Storage Mode";
            // 
            // radioButtonStoreOnPlate
            // 
            this.radioButtonStoreOnPlate.AutoSize = true;
            this.radioButtonStoreOnPlate.Location = new System.Drawing.Point(14, 62);
            this.radioButtonStoreOnPlate.Name = "radioButtonStoreOnPlate";
            this.radioButtonStoreOnPlate.Size = new System.Drawing.Size(141, 17);
            this.radioButtonStoreOnPlate.TabIndex = 1;
            this.radioButtonStoreOnPlate.TabStop = true;
            this.radioButtonStoreOnPlate.Text = "Store On Plate Detected";
            this.radioButtonStoreOnPlate.UseVisualStyleBackColor = true;
            this.radioButtonStoreOnPlate.CheckedChanged += new System.EventHandler(this.radioButtonStoreOnPlate_CheckedChanged);
            // 
            // radioButtonStoreOnMotion
            // 
            this.radioButtonStoreOnMotion.AutoSize = true;
            this.radioButtonStoreOnMotion.Location = new System.Drawing.Point(14, 30);
            this.radioButtonStoreOnMotion.Name = "radioButtonStoreOnMotion";
            this.radioButtonStoreOnMotion.Size = new System.Drawing.Size(149, 17);
            this.radioButtonStoreOnMotion.TabIndex = 0;
            this.radioButtonStoreOnMotion.TabStop = true;
            this.radioButtonStoreOnMotion.Text = "Store On Motion Detected";
            this.radioButtonStoreOnMotion.UseVisualStyleBackColor = true;
            this.radioButtonStoreOnMotion.CheckedChanged += new System.EventHandler(this.radioButtonStoreOnMotion_CheckedChanged);
            // 
            // buttonStart
            // 
            this.buttonStart.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.buttonStart.DialogResult = System.Windows.Forms.DialogResult.Abort;
            this.buttonStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonStart.Location = new System.Drawing.Point(1042, 483);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(145, 52);
            this.buttonStart.TabIndex = 4;
            this.buttonStart.Text = "Start";
            this.buttonStart.UseVisualStyleBackColor = true;
            this.buttonStart.Click += new System.EventHandler(this.buttonStart_Click);
            // 
            // buttonStop
            // 
            this.buttonStop.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.buttonStop.DialogResult = System.Windows.Forms.DialogResult.Abort;
            this.buttonStop.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonStop.Location = new System.Drawing.Point(1042, 548);
            this.buttonStop.Name = "buttonStop";
            this.buttonStop.Size = new System.Drawing.Size(145, 52);
            this.buttonStop.TabIndex = 0;
            this.buttonStop.Text = "Stop";
            this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
            // 
            // dataGridViewFilesInProcess
            // 
            this.dataGridViewFilesInProcess.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewFilesInProcess.Location = new System.Drawing.Point(273, 483);
            this.dataGridViewFilesInProcess.Name = "dataGridViewFilesInProcess";
            this.dataGridViewFilesInProcess.Size = new System.Drawing.Size(686, 197);
            this.dataGridViewFilesInProcess.TabIndex = 2;
            this.dataGridViewFilesInProcess.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewFilesInProcess_CellContentClick);
            // 
            // tabPageSearch
            // 
            this.tabPageSearch.Location = new System.Drawing.Point(4, 22);
            this.tabPageSearch.Name = "tabPageSearch";
            this.tabPageSearch.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageSearch.Size = new System.Drawing.Size(1196, 691);
            this.tabPageSearch.TabIndex = 0;
            this.tabPageSearch.Text = "Search";
            this.tabPageSearch.UseVisualStyleBackColor = true;
            // 
            // tabControlMain
            // 
            this.tabControlMain.Controls.Add(this.tabPageSearch);
            this.tabControlMain.Controls.Add(this.tabPageBatchProcess);
            this.tabControlMain.Controls.Add(this.tabPageEditMode);
            this.tabControlMain.Controls.Add(this.tabPageImportImages);
            this.tabControlMain.Controls.Add(this.tabPageLPRDiagnostics);
            this.tabControlMain.Controls.Add(this.tabPageOCRLib);
            this.tabControlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlMain.Location = new System.Drawing.Point(0, 0);
            this.tabControlMain.Name = "tabControlMain";
            this.tabControlMain.SelectedIndex = 0;
            this.tabControlMain.Size = new System.Drawing.Size(1204, 717);
            this.tabControlMain.TabIndex = 0;
            // 
            // AnalystsWorkstationMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1204, 717);
            this.Controls.Add(this.tabControlMain);
            this.Name = "AnalystsWorkstationMainForm";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.AnalystsWorkstationMainForm_Load);
            this.tabPageOCRLib.ResumeLayout(false);
            this.tabPageOCRLib.PerformLayout();
            this.tabPageLPRDiagnostics.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDiagHistogram)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDiag3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDiag2)).EndInit();
            this.tabControlDiagdisplay.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxDiag1)).EndInit();
            this.tabPageEditMode.ResumeLayout(false);
            this.tabPageEditMode.PerformLayout();
            this.tabControlLPRResults.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxCurrentImage)).EndInit();
            this.tabPageBatchProcess.ResumeLayout(false);
            this.tabPageBatchProcess.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBoxVideoFiles.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewFilesInProcess)).EndInit();
            this.tabControlMain.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.TabPage tabPageOCRLib;
        private System.Windows.Forms.Button buttonOCRGenLib;
        private System.Windows.Forms.Label labelOCRDestinationFolder;
        private System.Windows.Forms.Label labelOCRSourceFolder;
        private System.Windows.Forms.Button buttonSelectOCRSourceFolder;
        private System.Windows.Forms.Button buttonSelectOCRDestinationFolder;
        private System.Windows.Forms.TabPage tabPageLPRDiagnostics;
        private System.Windows.Forms.PictureBox pictureBoxDiagHistogram;
        private System.Windows.Forms.PictureBox pictureBoxDiag3;
        private System.Windows.Forms.ListBox listBoxRejectLog;
        private System.Windows.Forms.PictureBox pictureBoxDiag2;
        private System.Windows.Forms.TabControl tabControlDiagdisplay;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.PictureBox pictureBoxDiag1;
        private System.Windows.Forms.TabPage tabPageImportImages;
        private System.Windows.Forms.TabPage tabPageEditMode;
        private System.Windows.Forms.Label labelRepositoryStatus;
        private System.Windows.Forms.Button buttonStoreResults;
        private System.Windows.Forms.Button buttonLoadSingleFile;
        private System.Windows.Forms.Button buttonLoadDirectory;
        private System.Windows.Forms.Button buttonProcessNow;
        private System.Windows.Forms.CheckBox checkBoxProcessWhenOpened;
        private System.Windows.Forms.TabControl tabControlLPRResults;
        private System.Windows.Forms.TabPage tabPageLPRResults;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button buttonListViewVisible;
        private System.Windows.Forms.Label labelCurrentFileName;
        private System.Windows.Forms.Label labelCurrentIndex;
        private System.Windows.Forms.Button buttonPrevious;
        private System.Windows.Forms.Button buttonNext;
        private System.Windows.Forms.Label labelNumberOfImagesLoaded;
        private System.Windows.Forms.PictureBox pictureBoxCurrentImage;
        private System.Windows.Forms.Button buttonLoadListFromSearchResults;
        private System.Windows.Forms.TabPage tabPageBatchProcess;
        private System.Windows.Forms.GroupBox groupBoxVideoFiles;
        private System.Windows.Forms.Button buttonAddDirectory;
        private System.Windows.Forms.Button buttonSelectFile;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButtonStoreOnPlate;
        private System.Windows.Forms.RadioButton radioButtonStoreOnMotion;
        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.DataGridView dataGridViewFilesInProcess;
        private System.Windows.Forms.TabPage tabPageSearch;
        private System.Windows.Forms.TabControl tabControlMain;
        private System.Windows.Forms.TextBox textBoxUserSpecifiedCameraName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox listBoxDisplayTimeStamps;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton radioButtonToUserSpecifiedStorage;
        private System.Windows.Forms.RadioButton radioToRepository;
        private System.Windows.Forms.TextBox textBoxUserSpecifiedStorageLocation;
        private System.Windows.Forms.Button buttonSelectJpegDirectory;
    }
}

