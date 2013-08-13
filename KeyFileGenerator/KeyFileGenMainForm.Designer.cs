namespace KeyFileGenerator
{
    partial class KeyFileGenMainForm
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
            this.textBoxSerialNumber = new System.Windows.Forms.TextBox();
            this.buttonGenFile = new System.Windows.Forms.Button();
            this.labelFileLocation = new System.Windows.Forms.Label();
            this.buttonFileLocation = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.label3 = new System.Windows.Forms.Label();
            this.labelStatus = new System.Windows.Forms.Label();
            this.textBoxCustomerName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxEnterOrderNumber = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.labelKeyOwnerString = new System.Windows.Forms.Label();
            this.radioButtonDemoKey = new System.Windows.Forms.RadioButton();
            this.radioButtonProductionKey = new System.Windows.Forms.RadioButton();
            this.comboBoxProductTypeList = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(30, 143);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(104, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Enter Serial Number:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(50, 174);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Select Product:";
            // 
            // textBoxSerialNumber
            // 
            this.textBoxSerialNumber.Location = new System.Drawing.Point(140, 140);
            this.textBoxSerialNumber.Name = "textBoxSerialNumber";
            this.textBoxSerialNumber.Size = new System.Drawing.Size(191, 20);
            this.textBoxSerialNumber.TabIndex = 2;
            this.textBoxSerialNumber.TextChanged += new System.EventHandler(this.textBoxSerialNumber_TextChanged);
            // 
            // buttonGenFile
            // 
            this.buttonGenFile.Location = new System.Drawing.Point(177, 286);
            this.buttonGenFile.Name = "buttonGenFile";
            this.buttonGenFile.Size = new System.Drawing.Size(154, 23);
            this.buttonGenFile.TabIndex = 4;
            this.buttonGenFile.Text = "Generate Key ";
            this.buttonGenFile.UseVisualStyleBackColor = true;
            this.buttonGenFile.Click += new System.EventHandler(this.buttonGenFile_Click);
            // 
            // labelFileLocation
            // 
            this.labelFileLocation.AutoSize = true;
            this.labelFileLocation.Location = new System.Drawing.Point(138, 219);
            this.labelFileLocation.Name = "labelFileLocation";
            this.labelFileLocation.Size = new System.Drawing.Size(35, 13);
            this.labelFileLocation.TabIndex = 6;
            this.labelFileLocation.Text = "label4";
            // 
            // buttonFileLocation
            // 
            this.buttonFileLocation.Location = new System.Drawing.Point(52, 214);
            this.buttonFileLocation.Name = "buttonFileLocation";
            this.buttonFileLocation.Size = new System.Drawing.Size(75, 23);
            this.buttonFileLocation.TabIndex = 7;
            this.buttonFileLocation.Text = "folder";
            this.buttonFileLocation.UseVisualStyleBackColor = true;
            this.buttonFileLocation.Click += new System.EventHandler(this.buttonFileLocation_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(177, 330);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Status: ";
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Location = new System.Drawing.Point(242, 330);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(35, 13);
            this.labelStatus.TabIndex = 9;
            this.labelStatus.Text = "label4";
            // 
            // textBoxCustomerName
            // 
            this.textBoxCustomerName.Location = new System.Drawing.Point(140, 113);
            this.textBoxCustomerName.Name = "textBoxCustomerName";
            this.textBoxCustomerName.Size = new System.Drawing.Size(191, 20);
            this.textBoxCustomerName.TabIndex = 1;
            this.textBoxCustomerName.TextChanged += new System.EventHandler(this.textBoxCustomerName_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(22, 116);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(113, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Enter Customer Name:";
            // 
            // textBoxEnterOrderNumber
            // 
            this.textBoxEnterOrderNumber.Location = new System.Drawing.Point(140, 87);
            this.textBoxEnterOrderNumber.Name = "textBoxEnterOrderNumber";
            this.textBoxEnterOrderNumber.Size = new System.Drawing.Size(191, 20);
            this.textBoxEnterOrderNumber.TabIndex = 0;
            this.textBoxEnterOrderNumber.TextChanged += new System.EventHandler(this.textBoxEnterOrderNumber_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(31, 90);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(101, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Enter Oder Number:";
            // 
            // labelKeyOwnerString
            // 
            this.labelKeyOwnerString.AutoSize = true;
            this.labelKeyOwnerString.Location = new System.Drawing.Point(138, 13);
            this.labelKeyOwnerString.Name = "labelKeyOwnerString";
            this.labelKeyOwnerString.Size = new System.Drawing.Size(35, 13);
            this.labelKeyOwnerString.TabIndex = 13;
            this.labelKeyOwnerString.Text = "label6";
            this.labelKeyOwnerString.Click += new System.EventHandler(this.labelKeyOwnerString_Click);
            // 
            // radioButtonDemoKey
            // 
            this.radioButtonDemoKey.AutoSize = true;
            this.radioButtonDemoKey.Location = new System.Drawing.Point(62, 40);
            this.radioButtonDemoKey.Name = "radioButtonDemoKey";
            this.radioButtonDemoKey.Size = new System.Drawing.Size(74, 17);
            this.radioButtonDemoKey.TabIndex = 14;
            this.radioButtonDemoKey.TabStop = true;
            this.radioButtonDemoKey.Text = "Demo Key";
            this.radioButtonDemoKey.UseVisualStyleBackColor = true;
            // 
            // radioButtonProductionKey
            // 
            this.radioButtonProductionKey.AutoSize = true;
            this.radioButtonProductionKey.Location = new System.Drawing.Point(180, 40);
            this.radioButtonProductionKey.Name = "radioButtonProductionKey";
            this.radioButtonProductionKey.Size = new System.Drawing.Size(97, 17);
            this.radioButtonProductionKey.TabIndex = 15;
            this.radioButtonProductionKey.TabStop = true;
            this.radioButtonProductionKey.Text = "Production Key";
            this.radioButtonProductionKey.UseVisualStyleBackColor = true;
            // 
            // comboBoxProductTypeList
            // 
            this.comboBoxProductTypeList.FormattingEnabled = true;
            this.comboBoxProductTypeList.Location = new System.Drawing.Point(141, 171);
            this.comboBoxProductTypeList.Name = "comboBoxProductTypeList";
            this.comboBoxProductTypeList.Size = new System.Drawing.Size(190, 21);
            this.comboBoxProductTypeList.TabIndex = 16;
            // 
            // KeyFileGenMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(601, 358);
            this.Controls.Add(this.comboBoxProductTypeList);
            this.Controls.Add(this.radioButtonProductionKey);
            this.Controls.Add(this.radioButtonDemoKey);
            this.Controls.Add(this.labelKeyOwnerString);
            this.Controls.Add(this.textBoxEnterOrderNumber);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBoxCustomerName);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.buttonFileLocation);
            this.Controls.Add(this.labelFileLocation);
            this.Controls.Add(this.buttonGenFile);
            this.Controls.Add(this.textBoxSerialNumber);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "KeyFileGenMainForm";
            this.Text = "Key File Generator";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxSerialNumber;
        private System.Windows.Forms.Button buttonGenFile;
        private System.Windows.Forms.Label labelFileLocation;
        private System.Windows.Forms.Button buttonFileLocation;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.TextBox textBoxCustomerName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxEnterOrderNumber;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label labelKeyOwnerString;
        private System.Windows.Forms.RadioButton radioButtonDemoKey;
        private System.Windows.Forms.RadioButton radioButtonProductionKey;
        private System.Windows.Forms.ComboBox comboBoxProductTypeList;
    }
}

