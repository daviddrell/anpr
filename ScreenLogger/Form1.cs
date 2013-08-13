using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ScreenLoggerLib
{
    public partial class ScreenLogger : Form
    {
        public ScreenLogger()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        public void Log(string text)
        {
            
            this.BeginInvoke((MethodInvoker)delegate { this.PutText(text); });
        }

        public void ShowForm()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate { this._Show(); });
            }
            else
            {
                _Show();
            }
        }

        void _Show()
        {
            this.Show();
        }

        void PutText(string text)
        {
            listBox1.Items.Add(text);
            listBox1.SelectedIndex = listBox1.Items.Count - 1;
            if (listBox1.Items.Count > 1000)
            {
                listBox1.Items.RemoveAt(0);
            }
        }
    }
}
