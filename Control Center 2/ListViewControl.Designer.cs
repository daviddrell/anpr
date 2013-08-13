namespace Control_Center
{
    partial class ListViewControl
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
            this.listViewMainBox = new System.Windows.Forms.ListView();
            this.SuspendLayout();
            // 
            // listViewMainBox
            // 
            this.listViewMainBox.Location = new System.Drawing.Point(4, 4);
            this.listViewMainBox.Name = "listViewMainBox";
            this.listViewMainBox.Size = new System.Drawing.Size(521, 386);
            this.listViewMainBox.TabIndex = 0;
            this.listViewMainBox.UseCompatibleStateImageBehavior = false;
            this.listViewMainBox.SelectedIndexChanged += new System.EventHandler(this.listViewMainBox_SelectedIndexChanged);
            // 
            // ListViewControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.listViewMainBox);
            this.Name = "ListViewControl";
            this.Size = new System.Drawing.Size(521, 386);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listViewMainBox;
    }
}
