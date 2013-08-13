namespace ConfigureGPS_UC
{
    partial class EnterMinSec
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
            this.panelEnterLatitudeMinutes = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxSeconds = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxMinutes = new System.Windows.Forms.TextBox();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxDegrees = new System.Windows.Forms.TextBox();
            this.panelEnterLatitudeMinutes.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelEnterLatitudeMinutes
            // 
            this.panelEnterLatitudeMinutes.Controls.Add(this.label2);
            this.panelEnterLatitudeMinutes.Controls.Add(this.textBoxSeconds);
            this.panelEnterLatitudeMinutes.Controls.Add(this.label1);
            this.panelEnterLatitudeMinutes.Controls.Add(this.textBoxMinutes);
            this.panelEnterLatitudeMinutes.Controls.Add(this.radioButton2);
            this.panelEnterLatitudeMinutes.Controls.Add(this.radioButton1);
            this.panelEnterLatitudeMinutes.Controls.Add(this.label4);
            this.panelEnterLatitudeMinutes.Controls.Add(this.textBoxDegrees);
            this.panelEnterLatitudeMinutes.Location = new System.Drawing.Point(3, 3);
            this.panelEnterLatitudeMinutes.Name = "panelEnterLatitudeMinutes";
            this.panelEnterLatitudeMinutes.Size = new System.Drawing.Size(335, 58);
            this.panelEnterLatitudeMinutes.TabIndex = 8;
            this.panelEnterLatitudeMinutes.Paint += new System.Windows.Forms.PaintEventHandler(this.panelEnterLatitudeMinutes_Paint);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(108, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "Seconds";
            // 
            // textBoxSeconds
            // 
            this.textBoxSeconds.Location = new System.Drawing.Point(108, 5);
            this.textBoxSeconds.Name = "textBoxSeconds";
            this.textBoxSeconds.Size = new System.Drawing.Size(44, 20);
            this.textBoxSeconds.TabIndex = 12;
            this.textBoxSeconds.TextChanged += new System.EventHandler(this.textBoxSeconds_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(58, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Minutes";
            // 
            // textBoxMinutes
            // 
            this.textBoxMinutes.Location = new System.Drawing.Point(58, 5);
            this.textBoxMinutes.Name = "textBoxMinutes";
            this.textBoxMinutes.Size = new System.Drawing.Size(44, 20);
            this.textBoxMinutes.TabIndex = 1;
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(225, 28);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(85, 17);
            this.radioButton2.TabIndex = 3;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "radioButton2";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Location = new System.Drawing.Point(225, 7);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(85, 17);
            this.radioButton1.TabIndex = 2;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "radioButton1";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(5, 28);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Degrees";
            // 
            // textBoxDegrees
            // 
            this.textBoxDegrees.Location = new System.Drawing.Point(8, 5);
            this.textBoxDegrees.Name = "textBoxDegrees";
            this.textBoxDegrees.Size = new System.Drawing.Size(44, 20);
            this.textBoxDegrees.TabIndex = 0;
            // 
            // EnterMinSec
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelEnterLatitudeMinutes);
            this.Name = "EnterMinSec";
            this.Size = new System.Drawing.Size(341, 63);
            this.panelEnterLatitudeMinutes.ResumeLayout(false);
            this.panelEnterLatitudeMinutes.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelEnterLatitudeMinutes;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxDegrees;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxMinutes;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxSeconds;
    }
}
