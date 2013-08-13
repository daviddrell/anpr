namespace SmartSearchLib
{
    partial class SearchStatusDisplayUC
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
            this.panelStatus = new System.Windows.Forms.Panel();
            this.buttonSearchCompleteIndicator = new System.Windows.Forms.Button();
            this.buttonCancelSearch = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.labelCurrentlyAt = new System.Windows.Forms.Label();
            this.labelSearchPhase = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxErrorText = new System.Windows.Forms.TextBox();
            this.panelStatus.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelStatus
            // 
            this.panelStatus.BackColor = System.Drawing.Color.Gainsboro;
            this.panelStatus.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panelStatus.Controls.Add(this.textBoxErrorText);
            this.panelStatus.Controls.Add(this.label3);
            this.panelStatus.Controls.Add(this.labelSearchPhase);
            this.panelStatus.Controls.Add(this.labelCurrentlyAt);
            this.panelStatus.Controls.Add(this.buttonSearchCompleteIndicator);
            this.panelStatus.Controls.Add(this.buttonCancelSearch);
            this.panelStatus.Controls.Add(this.label2);
            this.panelStatus.Controls.Add(this.label1);
            this.panelStatus.Controls.Add(this.progressBar1);
            this.panelStatus.Location = new System.Drawing.Point(0, 0);
            this.panelStatus.Name = "panelStatus";
            this.panelStatus.Size = new System.Drawing.Size(306, 160);
            this.panelStatus.TabIndex = 27;
            // 
            // buttonSearchCompleteIndicator
            // 
            this.buttonSearchCompleteIndicator.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.buttonSearchCompleteIndicator.Location = new System.Drawing.Point(20, 130);
            this.buttonSearchCompleteIndicator.Name = "buttonSearchCompleteIndicator";
            this.buttonSearchCompleteIndicator.Size = new System.Drawing.Size(111, 23);
            this.buttonSearchCompleteIndicator.TabIndex = 26;
            this.buttonSearchCompleteIndicator.UseVisualStyleBackColor = false;
            this.buttonSearchCompleteIndicator.Click += new System.EventHandler(this.buttonSearchCompleteIndicator_Click);
            // 
            // buttonCancelSearch
            // 
            this.buttonCancelSearch.Location = new System.Drawing.Point(172, 130);
            this.buttonCancelSearch.Name = "buttonCancelSearch";
            this.buttonCancelSearch.Size = new System.Drawing.Size(96, 23);
            this.buttonCancelSearch.TabIndex = 25;
            this.buttonCancelSearch.Text = "Cancel Search";
            this.buttonCancelSearch.UseVisualStyleBackColor = true;
            this.buttonCancelSearch.Click += new System.EventHandler(this.buttonCancelSearch_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 13);
            this.label2.TabIndex = 24;
            this.label2.Text = "Currently At: ";
            this.label2.Click += new System.EventHandler(this.labelCount_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 13);
            this.label1.TabIndex = 23;
            this.label1.Text = "Seach Phase :";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(9, 59);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(282, 23);
            this.progressBar1.TabIndex = 21;
            // 
            // labelCurrentlyAt
            // 
            this.labelCurrentlyAt.AutoSize = true;
            this.labelCurrentlyAt.Location = new System.Drawing.Point(89, 33);
            this.labelCurrentlyAt.Name = "labelCurrentlyAt";
            this.labelCurrentlyAt.Size = new System.Drawing.Size(19, 13);
            this.labelCurrentlyAt.TabIndex = 27;
            this.labelCurrentlyAt.Text = "__";
            // 
            // labelSearchPhase
            // 
            this.labelSearchPhase.AutoSize = true;
            this.labelSearchPhase.Location = new System.Drawing.Point(89, 8);
            this.labelSearchPhase.Name = "labelSearchPhase";
            this.labelSearchPhase.Size = new System.Drawing.Size(19, 13);
            this.labelSearchPhase.TabIndex = 28;
            this.labelSearchPhase.Text = "__";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 85);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 13);
            this.label3.TabIndex = 29;
            this.label3.Text = "Errors: ";
            // 
            // textBoxErrorText
            // 
            this.textBoxErrorText.Location = new System.Drawing.Point(20, 102);
            this.textBoxErrorText.Name = "textBoxErrorText";
            this.textBoxErrorText.Size = new System.Drawing.Size(248, 20);
            this.textBoxErrorText.TabIndex = 30;
            // 
            // SearchStatusDisplayUC
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelStatus);
            this.Name = "SearchStatusDisplayUC";
            this.Size = new System.Drawing.Size(306, 160);
            this.panelStatus.ResumeLayout(false);
            this.panelStatus.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelStatus;
        private System.Windows.Forms.Button buttonSearchCompleteIndicator;
        private System.Windows.Forms.Button buttonCancelSearch;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label labelSearchPhase;
        private System.Windows.Forms.Label labelCurrentlyAt;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxErrorText;
    }
}
