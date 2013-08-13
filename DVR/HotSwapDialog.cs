using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ApplicationDataClass;

namespace DVRLib
{
    // ///////////
    //
    //   design
    //
    //      notifies the user that we have detected that the user plugged in a new USB drive and intends to hot swap
    //
    //         gets the current drive letter from DriveManager and displays to the user
    //   
    //         gets the new detected drive letter from th DriveManager and displays to the user
    //
    //         gets the drive statistics from each drive itself and displays to the user
    //
    //         as DriveManager pauses the DVR and makes the switch, the DriveManager sends text updates.
    //
    //         when the old drive is ready for removal, the DriveManger tells this dialog to display PROCEED TO REMOVE OLD DRIVE
    //
    //         when the 
    //
    public partial class HotSwapDialog : Form
    {
        public HotSwapDialog(APPLICATION_DATA appData)
        {
            InitializeComponent();
            m_AppData = appData;
        }

        APPLICATION_DATA m_AppData;
        const string newline = "\r\n";
        public void PostOriginalDriveName(string name)
        {
            this.BeginInvoke((MethodInvoker)delegate { labelCurrentDrive.Text += name+newline; }); 
        }
        public void PostOriginalDriveStatus(string status)
        {
            this.BeginInvoke((MethodInvoker)delegate { textBoxCurrentDriveStatus.Text += status + newline; }); 
        }

        public void PostNewDriveName(string name)
        {
            this.BeginInvoke((MethodInvoker)delegate { labelSwappingToDrive.Text += name + newline; }); 
        }
        public void PostNewDriveStatus(string status)
        {
            this.BeginInvoke((MethodInvoker)delegate { textBoxNewDriveStatus.Text += status + newline; }); 
        }
        public void PostSwapStatus(string status)
        {
            this.BeginInvoke((MethodInvoker)delegate { textBoxSwapStatus.Text += status + newline; }); 
        }
        public void PostSucessDone()
        {
            this.BeginInvoke((MethodInvoker)delegate { MessageBox.Show("Swap Sucessfull"); this.Close(); });
        }

    }

}
