using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace Control_Center
{
    public partial class ListViewControl : UserControl
    {
        public ListViewControl()
        {
            InitializeComponent();
            imageFilePath = Application.StartupPath ;

            plusSign =(Bitmap) Bitmap.FromFile(imageFilePath + "//plus.bmp");
            plusSignKey = "plusSign";
            minusSign = (Bitmap)Bitmap.FromFile(imageFilePath + "//minus.bmp");
            minusSignKey = "minusSign";

            listViewMainBox.View = View.Details;
            listViewMainBox.ItemSelectionChanged += new ListViewItemSelectionChangedEventHandler(listViewMainBox_ItemSelectionChanged);
            AddItems();

        }

        void listViewMainBox_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            e.Item.ImageKey = minusSignKey;

            ListViewControl stats = new ListViewControl();

          //  listViewMainBox.Items.Add(stats);
            
        }

        string imageFilePath;
        Bitmap plusSign;
        string plusSignKey;
        Bitmap minusSign;
        string minusSignKey;

        private void listViewMainBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        void AddItems()
        {
            ImageList expandIcons = new ImageList();
            expandIcons.Images.Add(plusSignKey, plusSign);
            expandIcons.Images.Add(minusSignKey, minusSign);
            listViewMainBox.SmallImageList = expandIcons;
            listViewMainBox.LargeImageList = expandIcons;
           
           
            listViewMainBox.Columns.Add("Stat catagory", 100, HorizontalAlignment.Center);

            ListViewItem listitem = new ListViewItem("LPRStats");
        
            listitem.ImageKey = plusSignKey;

            listViewMainBox.Items.Add(listitem);
            
        }

       
    }
}
