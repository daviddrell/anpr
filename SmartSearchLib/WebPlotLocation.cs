using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SmartSearchLib
{
    public partial class WebPlotLocation : Form
    {
        string URL;

        public WebPlotLocation(string url)
        {
            InitializeComponent();

            URL = url;
            PutData();
        }

        public void SetNewURL ( string url)
        {
            URL = url;
            PutData();
        }

      

        void PutData( )
        {
            if (webBrowser1.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate { PutData(); });

            }
            else
            {
               
                webBrowser1.Navigate(URL);
                
            }
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

        }
    }
}
