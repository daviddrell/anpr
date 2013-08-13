using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ConfigureGPS_UC
{
    public partial class enterLocationMinSecPage : UserControl
    {
        public enterLocationMinSecPage()
        {
            InitializeComponent();


            latitude = new EnterMinSec(LatitudeLongitudeType.LATITUDE);
            longitude = new EnterMinSec(LatitudeLongitudeType.LONGITUDE);


            latitude.Location = new Point(50, 30);
            longitude.Location = new Point(50, 100);

            this.Controls.Add(latitude);
            this.Controls.Add(longitude);
        }

        EnterMinSec latitude;
        EnterMinSec longitude;

        public override void Refresh()
        {
            base.Refresh();

            if (latitude != null)
                latitude.Refresh();
            if (longitude != null)
                longitude.Refresh();
        }

        public void SaveChanges()
        {
            latitude.SaveChanges();
            longitude.SaveChanges();
        }

        private void enterLocationFractionalPage_Load(object sender, EventArgs e)
        {

        }
    }
}
