namespace Control_Center
{
    partial class FirstTimeLogin
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
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxFirstPassword = new System.Windows.Forms.TextBox();
            this.textBoxSecondPassword = new System.Windows.Forms.TextBox();
            this.buttonCommittPassword = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxStatus = new System.Windows.Forms.TextBox();
            this.labelMainControlText = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Enter Password:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 76);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Re-Enter Password:";
            // 
            // textBoxFirstPassword
            // 
            this.textBoxFirstPassword.Location = new System.Drawing.Point(139, 37);
            this.textBoxFirstPassword.Name = "textBoxFirstPassword";
            this.textBoxFirstPassword.Size = new System.Drawing.Size(137, 20);
            this.textBoxFirstPassword.TabIndex = 0;
            this.textBoxFirstPassword.TextChanged += new System.EventHandler(this.textBoxFirstPassword_TextChanged);
            // 
            // textBoxSecondPassword
            // 
            this.textBoxSecondPassword.Location = new System.Drawing.Point(138, 73);
            this.textBoxSecondPassword.Name = "textBoxSecondPassword";
            this.textBoxSecondPassword.Size = new System.Drawing.Size(137, 20);
            this.textBoxSecondPassword.TabIndex = 1;
            this.textBoxSecondPassword.TextChanged += new System.EventHandler(this.textBoxSecondPassword_TextChanged);
            // 
            // buttonCommittPassword
            // 
            this.buttonCommittPassword.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonCommittPassword.Location = new System.Drawing.Point(123, 115);
            this.buttonCommittPassword.Name = "buttonCommittPassword";
            this.buttonCommittPassword.Size = new System.Drawing.Size(75, 23);
            this.buttonCommittPassword.TabIndex = 2;
            this.buttonCommittPassword.Text = "Commit";
            this.buttonCommittPassword.UseVisualStyleBackColor = true;
            this.buttonCommittPassword.Click += new System.EventHandler(this.buttonCommittPassword_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 157);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(43, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Status: ";
            // 
            // textBoxStatus
            // 
            this.textBoxStatus.BackColor = System.Drawing.Color.White;
            this.textBoxStatus.ForeColor = System.Drawing.Color.Red;
            this.textBoxStatus.Location = new System.Drawing.Point(66, 154);
            this.textBoxStatus.Name = "textBoxStatus";
            this.textBoxStatus.Size = new System.Drawing.Size(222, 20);
            this.textBoxStatus.TabIndex = 4;
            this.textBoxStatus.TextChanged += new System.EventHandler(this.textBoxStatus_TextChanged);
            // 
            // labelMainControlText
            // 
            this.labelMainControlText.AutoSize = true;
            this.labelMainControlText.Location = new System.Drawing.Point(90, 7);
            this.labelMainControlText.Name = "labelMainControlText";
            this.labelMainControlText.Size = new System.Drawing.Size(35, 13);
            this.labelMainControlText.TabIndex = 5;
            this.labelMainControlText.Text = "label4";
            // 
            // FirstTimeLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(90)))));
            this.Controls.Add(this.labelMainControlText);
            this.Controls.Add(this.textBoxStatus);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.buttonCommittPassword);
            this.Controls.Add(this.textBoxSecondPassword);
            this.Controls.Add(this.textBoxFirstPassword);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "FirstTimeLogin";
            this.Size = new System.Drawing.Size(302, 182);
            this.Load += new System.EventHandler(this.FirstTimeLogin_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxFirstPassword;
        private System.Windows.Forms.TextBox textBoxSecondPassword;
        private System.Windows.Forms.Button buttonCommittPassword;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxStatus;
        private System.Windows.Forms.Label labelMainControlText;
    }
}
