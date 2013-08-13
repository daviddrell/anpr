namespace Control_Center
{
    partial class SelectRemoteSystem
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
            this.dataGridViewRemoteHostList = new System.Windows.Forms.DataGridView();
            this.colRemoteName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colIPAddress = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.buttonLoadHosts = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxSelectedSystem = new System.Windows.Forms.TextBox();
            this.buttonLoginAsAdmin = new System.Windows.Forms.Button();
            this.buttonLoginAsViewer = new System.Windows.Forms.Button();
            this.buttonLogout = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRemoteHostList)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewRemoteHostList
            // 
            this.dataGridViewRemoteHostList.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dataGridViewRemoteHostList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewRemoteHostList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colRemoteName,
            this.colIPAddress,
            this.colDescription,
            this.colStatus});
            this.dataGridViewRemoteHostList.Location = new System.Drawing.Point(41, 57);
            this.dataGridViewRemoteHostList.Name = "dataGridViewRemoteHostList";
            this.dataGridViewRemoteHostList.Size = new System.Drawing.Size(687, 307);
            this.dataGridViewRemoteHostList.TabIndex = 6;
            this.dataGridViewRemoteHostList.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewRemoteHostList_CellContentClick);
            // 
            // colRemoteName
            // 
            this.colRemoteName.HeaderText = "Remote Name";
            this.colRemoteName.Name = "colRemoteName";
            this.colRemoteName.ReadOnly = true;
            this.colRemoteName.Width = 145;
            // 
            // colIPAddress
            // 
            this.colIPAddress.HeaderText = "IP Address";
            this.colIPAddress.Name = "colIPAddress";
            this.colIPAddress.ReadOnly = true;
            // 
            // colDescription
            // 
            this.colDescription.HeaderText = "Description";
            this.colDescription.Name = "colDescription";
            this.colDescription.ReadOnly = true;
            this.colDescription.Width = 200;
            // 
            // colStatus
            // 
            this.colStatus.HeaderText = "Status";
            this.colStatus.Name = "colStatus";
            this.colStatus.ReadOnly = true;
            this.colStatus.Width = 239;
            // 
            // buttonLoadHosts
            // 
            this.buttonLoadHosts.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonLoadHosts.ForeColor = System.Drawing.Color.White;
            this.buttonLoadHosts.Location = new System.Drawing.Point(41, 394);
            this.buttonLoadHosts.Name = "buttonLoadHosts";
            this.buttonLoadHosts.Size = new System.Drawing.Size(165, 23);
            this.buttonLoadHosts.TabIndex = 5;
            this.buttonLoadHosts.Text = "LoadHostsFile";
            this.buttonLoadHosts.UseVisualStyleBackColor = true;
            this.buttonLoadHosts.Click += new System.EventHandler(this.buttonLoadHosts_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(41, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(129, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Selected Remote System:";
            // 
            // textBoxSelectedSystem
            // 
            this.textBoxSelectedSystem.Location = new System.Drawing.Point(177, 20);
            this.textBoxSelectedSystem.Name = "textBoxSelectedSystem";
            this.textBoxSelectedSystem.Size = new System.Drawing.Size(382, 20);
            this.textBoxSelectedSystem.TabIndex = 8;
            this.textBoxSelectedSystem.TextChanged += new System.EventHandler(this.textBoxSelectedSystem_TextChanged);
            // 
            // buttonLoginAsAdmin
            // 
            this.buttonLoginAsAdmin.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonLoginAsAdmin.ForeColor = System.Drawing.Color.White;
            this.buttonLoginAsAdmin.Location = new System.Drawing.Point(580, 20);
            this.buttonLoginAsAdmin.Name = "buttonLoginAsAdmin";
            this.buttonLoginAsAdmin.Size = new System.Drawing.Size(96, 23);
            this.buttonLoginAsAdmin.TabIndex = 9;
            this.buttonLoginAsAdmin.Text = "Login as Admin";
            this.buttonLoginAsAdmin.UseVisualStyleBackColor = true;
            this.buttonLoginAsAdmin.Click += new System.EventHandler(this.buttonLoginAsAdmin_Click);
            // 
            // buttonLoginAsViewer
            // 
            this.buttonLoginAsViewer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonLoginAsViewer.ForeColor = System.Drawing.Color.White;
            this.buttonLoginAsViewer.Location = new System.Drawing.Point(704, 20);
            this.buttonLoginAsViewer.Name = "buttonLoginAsViewer";
            this.buttonLoginAsViewer.Size = new System.Drawing.Size(96, 23);
            this.buttonLoginAsViewer.TabIndex = 10;
            this.buttonLoginAsViewer.Text = "Login as Viewer";
            this.buttonLoginAsViewer.UseVisualStyleBackColor = true;
            this.buttonLoginAsViewer.Click += new System.EventHandler(this.buttonLoginAsViewer_Click);
            // 
            // buttonLogout
            // 
            this.buttonLogout.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonLogout.ForeColor = System.Drawing.Color.White;
            this.buttonLogout.Location = new System.Drawing.Point(828, 20);
            this.buttonLogout.Name = "buttonLogout";
            this.buttonLogout.Size = new System.Drawing.Size(96, 23);
            this.buttonLogout.TabIndex = 11;
            this.buttonLogout.Text = "Logout";
            this.buttonLogout.UseVisualStyleBackColor = true;
            this.buttonLogout.Click += new System.EventHandler(this.buttonLogout_Click);
            // 
            // SelectRemoteSystem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(90)))));
            this.Controls.Add(this.buttonLogout);
            this.Controls.Add(this.buttonLoginAsViewer);
            this.Controls.Add(this.buttonLoginAsAdmin);
            this.Controls.Add(this.textBoxSelectedSystem);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dataGridViewRemoteHostList);
            this.Controls.Add(this.buttonLoadHosts);
            this.Name = "SelectRemoteSystem";
            this.Size = new System.Drawing.Size(978, 466);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewRemoteHostList)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridViewRemoteHostList;
        private System.Windows.Forms.Button buttonLoadHosts;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRemoteName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colIPAddress;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDescription;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStatus;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxSelectedSystem;
        private System.Windows.Forms.Button buttonLoginAsAdmin;
        private System.Windows.Forms.Button buttonLoginAsViewer;
        private System.Windows.Forms.Button buttonLogout;
    }
}
