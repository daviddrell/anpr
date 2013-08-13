using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SmartSearchLib;
using ApplicationDataClass;
using PathsLib;
using ErrorLoggingLib;
using ScreenLoggerLib;
using FrameGeneratorLib;
using LPRServiceCore;
using System.Threading;
using UserSettingsLib;
using LPREngineLib;
using System.IO;
using Utilities;
using LPRInteractiveEditUC;
using LPROCR_Wrapper;


///   this file contains the methods related the Edit Mode tab functions
///   
namespace AnalystsWorkstation
{
    public partial class AnalystsWorkstationMainForm : Form
    {

        string[] jpegsToProcess;
        int m_CurrentIndex = 0;
        

        private void buttonStoreResults_Click(object sender, EventArgs e)
        {
           

            // pick up string changes the user may have made

            for (int t = 0; t < tabControlLPRResults.TabCount; t++)
            {
                LPRInteractiveEditUC.LPRInteractiveEditUC lprEditor = (LPRInteractiveEditUC.LPRInteractiveEditUC)m_TabPageLPREditorTable[tabControlLPRResults.TabPages[t]];
                m_CurrentImageResult[t].plateNumber = lprEditor.GetCurrentPlateString();
            }
           
            StoreResult();
            m_HaveNewDataToStore = false;
            buttonStoreResults.Enabled = false;// gets re-enbalbed by the CheckRepositoryLoop() based on the HaveDataToStore flag
        }

  

        private void buttonLoadSingleFile_Click(object sender, EventArgs e)
        {
            string lastDir = UserSettings.Get(UserSettingTags.AW_SingleFileLastLocation);
            if (lastDir != null)
            {
                openFileDialog1.InitialDirectory = lastDir;
            }

            openFileDialog1.FileName = "";
            openFileDialog1.Filter = "Image files |*.jpg;*.jpeg;*.bmp;|All Files|*.*";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                UserSettings.Set(UserSettingTags.AW_SingleFileLastLocation, (new FileInfo(openFileDialog1.FileName)).DirectoryName);
                LoadJpegsFromDirectory( openFileDialog1.FileName );
            }
        }

       
        private void buttonLoadDirectory_Click(object sender, EventArgs e)
        {
            string lastDir = UserSettings.Get(UserSettingTags.AW_LastDirectory);
            if (lastDir != null)
            {
                folderBrowserDialog1.SelectedPath = lastDir;
            }
           
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                UserSettings.Set(UserSettingTags.AW_LastDirectory, folderBrowserDialog1.SelectedPath);

                LoadJpegsFromDirectory(folderBrowserDialog1.SelectedPath);
            }
        }

        private void buttonLoadListFromSearchResults_Click(object sender, EventArgs e)
        {
            LoadJpegsFromSearchTool();
        }

        private void buttonPrevious_Click(object sender, EventArgs e)
        {
            if (jpegsToProcess == null) return;
            if (jpegsToProcess.Count() == 0) return;

            try
            {
                m_CurrentIndex--;
                if (m_CurrentIndex < 0) m_CurrentIndex = jpegsToProcess.Count() - 1;

                DisplayImage();
            }
            catch (Exception ex) { labelCurrentFileName.Text = "Exception on file open :" + ex.Message; }
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            if (jpegsToProcess == null) return;
            if (jpegsToProcess.Count() == 0) return;

            try
            {
                m_CurrentIndex++;
                if (m_CurrentIndex >= jpegsToProcess.Count()) m_CurrentIndex = 0;


                DisplayImage();
            }
            catch (Exception ex) { labelCurrentFileName.Text = "Exception on file open :" + ex.Message; }
        }

        void DisplayImage()
        {
            pictureBoxCurrentImage.Image = Bitmap.FromFile(jpegsToProcess[m_CurrentIndex]);
            labelCurrentFileName.Text = jpegsToProcess[m_CurrentIndex];
            labelCurrentIndex.Text = m_CurrentIndex.ToString();

            if (checkBoxProcessWhenOpened.Checked)
            {
                ProcessImage((Bitmap)pictureBoxCurrentImage.Image);
            }
        }

        void LoadJpegsFromDirectory(string path)
        {
            try
            {
                // is the path a single file or a directory?

                bool isADirectory = Directory.Exists(path);

                if (isADirectory)
                {
                    string[] sa1 = Directory.GetFiles(path, "*.jpg");
                    string[] sa2 = Directory.GetFiles(path, "*.jpeg");
                    string[] sa3 = Directory.GetFiles(path, "*.bmp");

                    jpegsToProcess = new string[sa1.Length + sa2.Length + sa3.Length];

                    sa1.CopyTo(jpegsToProcess, 0);
                    sa2.CopyTo(jpegsToProcess, sa1.Length);
                    sa3.CopyTo(jpegsToProcess, sa2.Length);
                }
                else
                {

                    jpegsToProcess = new string[1];
                    jpegsToProcess[0] = path;
                }

                if (jpegsToProcess == null)
                {
                    MessageBox.Show("No images found");
                    labelNumberOfImagesLoaded.Text = "No images loaded";
                    return;
                }

                if (jpegsToProcess.Count() == 0)
                {
                    MessageBox.Show("No images found");
                    labelNumberOfImagesLoaded.Text = "No images loaded";
                    return;
                }

                if (jpegsToProcess.Count() > m_AppData.MAX_IMAGES_TO_EDIT)
                {
                    MessageBox.Show("Too many images to load, max is " + m_AppData.MAX_IMAGES_TO_EDIT.ToString());
                    labelNumberOfImagesLoaded.Text = "No images loaded";
                    return;
                }

                labelNumberOfImagesLoaded.Text = "loaded " + jpegsToProcess.Count().ToString() + " images";
                m_CurrentIndex = 0;

                DisplayImage();

                LoadListView();


            }
            catch (Exception ex) { labelCurrentFileName.Text = "Exception on file open :" + ex.Message; }
        }

        void LoadJpegsFromSearchTool()
        {
            try
            {
                jpegsToProcess = null;

                jpegsToProcess = m_SmartSearchUC.GetAllFilePathsInResultsTable();
                if (jpegsToProcess == null)
                {
                    MessageBox.Show("No images found");
                    labelNumberOfImagesLoaded.Text = "No images loaded";
                    return;
                }

                if (jpegsToProcess.Count() == 0)
                {
                    MessageBox.Show("No images found");
                    labelNumberOfImagesLoaded.Text = "No images loaded";
                    return;
                }

                if (jpegsToProcess.Count() > m_AppData.MAX_IMAGES_TO_EDIT)
                {
                    MessageBox.Show("Too many images to load, max is " + m_AppData.MAX_IMAGES_TO_EDIT.ToString());
                    labelNumberOfImagesLoaded.Text = "No images loaded";
                    return;
                }

                labelNumberOfImagesLoaded.Text = "loaded " + jpegsToProcess.Count().ToString() + " images";
                m_CurrentIndex = 0;

                DisplayImage();

                LoadListView();

            }
            catch (Exception ex) { labelCurrentFileName.Text = "Exception on file open :" + ex.Message; }
        }


        Thread m_LoadListViewThread;
        void LoadListView()
        {
            m_LoadListViewThread = new Thread(LoadListViewLoop);
            m_LoadListViewThread.Start();
        }

        void LoadListViewLoop()
        {
            ImageList imageList = new ImageList();
            int width = 128;
            int height = 128;
            imageList.ImageSize = new Size(width, height);
            imageList.ColorDepth = ColorDepth.Depth24Bit;

            for (int i = 0; i < jpegsToProcess.Count(); i++)
            {
                try
                {
                    Image thumbNail = Image.FromFile(jpegsToProcess[i]).GetThumbnailImage(width, height, null, IntPtr.Zero);
                    imageList.Images.Add(thumbNail);

                    ListViewItem item = new ListViewItem(jpegsToProcess[i]);
                    item.ImageIndex = i;



                    this.BeginInvoke((MethodInvoker)delegate { m_EditModePictureSelectionViewer.Items.Add(item); });
                }
                catch { }

           
                if (m_Stop) return;
            }


            this.BeginInvoke((MethodInvoker)delegate { m_EditModePictureSelectionViewer.LargeImageList = imageList; });
        }



        void m_EditModePictureSelectionViewer_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            // user selected an image

            if (e.IsSelected)
            {
                m_CurrentIndex = e.Item.ImageIndex;
                this.BeginInvoke((MethodInvoker)delegate { DisplayImage(); });
            }
        }

        string m_ShowThumbNails = "Show Thumbnails";
        string m_NotShowThumbNails = "Hide Thumbnails";
        private void buttonListViewVisible_Click(object sender, EventArgs e)
        {
            if (buttonListViewVisible.Text == m_ShowThumbNails)
            {
                buttonListViewVisible.Text = m_NotShowThumbNails;

                m_EditModePictureSelectionViewer.Visible = true;
                m_EditModePictureSelectionViewer.Enabled = true;
                m_EditModePictureSelectionViewer.BringToFront();
            }
            else
            {
                buttonListViewVisible.Text = m_ShowThumbNails;

                m_EditModePictureSelectionViewer.Visible = false;
                m_EditModePictureSelectionViewer.Enabled = false;
                m_EditModePictureSelectionViewer.SendToBack();
            }
        }



        private void buttonProcessNow_Click(object sender, EventArgs e)
        {
            ProcessImage((Bitmap)pictureBoxCurrentImage.Image);
        }

        class PLATE_INFO
        {
            public HistogramBounds histoBounds;
            public string plateNumber;
            public Bitmap plateImage;
            public Bitmap plateImageUnprocessed;
            public int numChars;
            public Bitmap[] charImages;
            public Bitmap sourceImage;
            public bool resultHasBeenSaved=false;
            public string sourceFileName;
            public Bitmap histoBmp;
            public string histString;
            public PLATE_INFO()
            {
                charImages = new Bitmap[10];
            }
        }

        PLATE_INFO[]  m_CurrentImageResult;

        void ProcessImage(Bitmap bmp)
        {
            try
            {

                listBoxRejectLog.Items.Clear();

                if (bmp == null) return;

                // get the luminance array

                int[,] lum = GetLuminanceArray(bmp);

    

                int error = 0;

                LPROCR_Lib.LPR_PROCESS_OPTIONS processOptions;
                processOptions = new LPROCR_Lib.LPR_PROCESS_OPTIONS();
                processOptions.EnableAutoRotationRoll = 1;
                processOptions.EnableRotationRoll = 1;

                int diagEnabled = 0;
                if (m_AppData.LPRDiagEnabled) diagEnabled = 1;

                // read the image
                int plateCount = m_LPREngine.m_LPRFuntions.ReadThisImage(lum,  (int)diagEnabled, ref processOptions, ref error);


                // extract the plate images and detected strings            

                PLATE_INFO[] plateInfos = new PLATE_INFO[plateCount];


                // get the diag images (not plate, but full images)

                if (m_AppData.LPRDiagEnabled)
                {
                    ClearDiagDisplayTable();

                    byte[,] r = new byte[bmp.Width, bmp.Height];
                    byte[,] g = new byte[bmp.Width, bmp.Height];
                    byte[,] b = new byte[bmp.Width, bmp.Height];
                    error = 0;
                    m_LPREngine.m_LPRFuntions.diagsGetImage(1, r, g, b, ref error);// get the full resolution image

                    Bitmap imageBmp = new Bitmap(bmp.Width, bmp.Height);

                    putPixelsIntoBmp(imageBmp, r, g, b);

                    pictureBoxDiag1.Image = imageBmp;

                    int w = 0;
                    int h = 0;

                    w = bmp.Width;
                    h = bmp.Height;



                    //  m_LPREngine.m_LPRFuntions.GetSubImageSize(ref w, ref h);

                    Bitmap subscalledBmp = new Bitmap(w, h);
                    r = new byte[w, h];
                    g = new byte[w, h];
                    b = new byte[w, h];

                    //m_LPREngine.m_LPRFuntions.diagsGetImage(0, r, g, b, ref error);// get the subscalled  image

                    int[,] edge = new int[w, h];

                    //                m_LPREngine.m_LPRFuntions.getEdgeMapSub(edge, ref error);
                    m_LPREngine.m_LPRFuntions.getEdgeMapFullRes(edge, ref error);

                    ColorizeEdgeMap(edge, r, g, b);

                    putPixelsIntoBmp(subscalledBmp, r, g, b);

                    pictureBoxDiag2.Image = subscalledBmp;


                    //               get the intergal image

                    r = new byte[bmp.Width, bmp.Height];
                    g = new byte[bmp.Width, bmp.Height];
                    b = new byte[bmp.Width, bmp.Height];
                    imageBmp = new Bitmap(bmp.Width, bmp.Height);

                    m_LPREngine.m_LPRFuntions.GetIntegralImage(r, g, b);
                    putPixelsIntoBmp(imageBmp, r, g, b);
                    pictureBoxDiag3.Image = imageBmp;
                }


                //   get the plates

                for (int p = 0; p < plateCount; p++)
                {
                    float score = 0;

                    plateInfos[p] = new PLATE_INFO();

                    plateInfos[p].sourceImage = bmp;
                    plateInfos[p].sourceFileName = jpegsToProcess[m_CurrentIndex];

                    // get the string from the plate

                    plateInfos[p].plateNumber = m_LPREngine.m_LPRFuntions.GetPlateString(p, ref score);


                    // exract a bitmap of the plate itself
                    int pw = 0;
                    int ph = 0;
                    m_LPREngine.m_LPRFuntions.GetPlateImageSize(p, ref pw, ref ph, false);


                    plateInfos[p].plateImage = new Bitmap(pw, ph);

                    m_LPREngine.m_LPRFuntions.GetPlateImage(p, plateInfos[p].plateImage);

                    plateInfos[p].numChars = plateInfos[p].plateNumber.Length;
                    plateInfos[p].charImages = new Bitmap[plateInfos[p].numChars];

                    for (int c = 0; c < plateInfos[p].numChars; c++)
                    {
                        plateInfos[p].charImages[c] = null;

                        try
                        {
                            m_LPREngine.m_LPRFuntions.GetCharImage(p, c, out plateInfos[p].charImages[c], false);
                        }
                        catch (Exception ex) { m_Log.Trace(ex, ErrorLog.LOG_TYPE.INFORMATIONAL); }
                    }




                }// end for each plate found

                if (plateCount == 0)
                {
                    // perhaps we really screwed up and did not find a plate when one is really there,
                    // allow the user to manually add the plate numbers

                    plateInfos = new PLATE_INFO[1];

                    plateInfos[0] = new PLATE_INFO();

                    plateInfos[0].sourceImage = bmp;
                    plateInfos[0].sourceFileName = jpegsToProcess[m_CurrentIndex];

                    // get the string from the plate

                    plateInfos[0].plateNumber = "0";


                    plateInfos[0].plateImage = bmp;// use the main picture to fill the space


                    plateInfos[0].numChars = 0;

                    plateCount = 1;
                }

                // create a display
                ClearDisplayTable();
                for (int p = 0; p < plateCount; p++)
                {
                    AddPlateTabToEditPage(plateInfos[p]);
                }

                m_CurrentImageResult = plateInfos;

                m_HaveNewDataToStore = true;



                if (m_AppData.LPRDiagEnabled)
                {
                    try
                    {
                        // get the reject log
                        string log = m_LPREngine.m_LPRFuntions.GetRejectLog();
                        string[] sp = log.Split(',');
                        foreach (string s in sp) listBoxRejectLog.Items.Add(s);

                        //   get the plate diagnostic images, and run a diagnostic histo on original plate images

                        int count = m_LPREngine.m_LPRFuntions.GetNumCandidatePlates();

                        for (int i = 0; i < count; i++)
                        {
                            int w = 0;
                            int h = 0;
                            m_LPREngine.m_LPRFuntions.GetCandidatePlateImageSize(i, ref w, ref h);

                            byte[,] r = new byte[w, h];
                            byte[,] g = new byte[w, h];
                            byte[,] b = new byte[w, h];

                            error = 0;

                            int sucess = m_LPREngine.m_LPRFuntions.GetDiagCandidatePlateImage(i, r, g, b, ref error);// get the full resolution image
                            if (sucess == 0) continue;

                            Bitmap bt = new Bitmap(w, h);

                            putPixelsIntoBmp(bt, r, g, b);

                            PLATE_INFO plateDiagInfo = new PLATE_INFO();

                            plateDiagInfo.plateImage = bt;

                            DrawHistoBoundsOnPlateImage(plateDiagInfo);



                            // get the diagnostic char images from the OCR lib

                            for (int c = 0; c < plateDiagInfo.charImages.Count(); c++)
                            {
                                Bitmap cBmp = null;
                                m_LPREngine.m_LPRFuntions.GetDiagCharImage(i, c, out cBmp);
                                plateDiagInfo.charImages[c] = cBmp;
                            }

                            // run diag histogram display on each plate

                            {

                                Bitmap plateImageUnprocessed = new Bitmap(w, h);

                                m_LPREngine.m_LPRFuntions.GetCandidatePlateImage(i, out plateImageUnprocessed);


                                plateDiagInfo.plateImageUnprocessed = plateImageUnprocessed;

                                int[,] plum = new int[plateImageUnprocessed.Width, plateImageUnprocessed.Height];

                                PixelLib.Pixels.getPixelsFromImage(plateImageUnprocessed, plum);

                                int[] histo = new int[256];
                                int[] integration = new int[256];
                                Bitmap hBmp = new Bitmap(256, 100);
                                HistoResults histStats = null;
                                plateDiagInfo.histString = historgram(plum, histo, integration, true, hBmp, plateDiagInfo.histoBounds, out histStats);

                                plateDiagInfo.histoBmp = hBmp;
                            }


                            AddPlateTabToDiagPage(plateDiagInfo);
                        }
                    }
                    catch (Exception ex) { MessageBox.Show(ex.Message + ex.StackTrace); }

                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message + ex.StackTrace); }

        }


        void CreateHistoBounds(PLATE_INFO plateDiagInfo)
        {
            int w = plateDiagInfo.plateImage.Width;
            int h = plateDiagInfo.plateImage.Height;

            int centerX = w / 2;
            int centerY = h / 2;
            plateDiagInfo.histoBounds = new HistogramBounds();
            plateDiagInfo.histoBounds.leftEdge = centerX - 50;
            if (plateDiagInfo.histoBounds.leftEdge < 0) plateDiagInfo.histoBounds.leftEdge = 10;
            plateDiagInfo.histoBounds.rightEdge = centerX + 50;
            if (plateDiagInfo.histoBounds.rightEdge >= w) plateDiagInfo.histoBounds.rightEdge = w - 10;

            plateDiagInfo.histoBounds.topEdge = centerY - 10;
            if (plateDiagInfo.histoBounds.topEdge < 0) plateDiagInfo.histoBounds.topEdge = 0;

            plateDiagInfo.histoBounds.bottomEdge = centerY + 10;
            if (plateDiagInfo.histoBounds.bottomEdge < 0) plateDiagInfo.histoBounds.bottomEdge = 0;


        }

        void DrawHistoBoundsOnPlateImage(PLATE_INFO plateDiagInfo)
        {
            CreateHistoBounds(plateDiagInfo);

            Graphics g = Graphics.FromImage(plateDiagInfo.plateImage);

            Point p1 = new Point( plateDiagInfo.histoBounds.leftEdge, plateDiagInfo.histoBounds.topEdge);
            Point p2 = new Point(plateDiagInfo.histoBounds.rightEdge, plateDiagInfo.histoBounds.topEdge);

            g.DrawLine(new Pen(Color.Blue), p1, p2);

            p1 = new Point(plateDiagInfo.histoBounds.rightEdge, plateDiagInfo.histoBounds.topEdge);
            p2 = new Point(plateDiagInfo.histoBounds.rightEdge, plateDiagInfo.histoBounds.bottomEdge);
            g.DrawLine(new Pen(Color.Blue), p1, p2);

            p1 = new Point(plateDiagInfo.histoBounds.rightEdge, plateDiagInfo.histoBounds.bottomEdge);
            p2 = new Point(plateDiagInfo.histoBounds.leftEdge, plateDiagInfo.histoBounds.bottomEdge);
            g.DrawLine(new Pen(Color.Blue), p1, p2);

            p1 = new Point(plateDiagInfo.histoBounds.leftEdge, plateDiagInfo.histoBounds.bottomEdge);
            p2 = new Point(plateDiagInfo.histoBounds.leftEdge, plateDiagInfo.histoBounds.topEdge);

            g.DrawLine(new Pen(Color.Blue), p1, p2);


        }

        ThreadSafeHashTable m_TabPageLPREditorTable;

        void AddPlateTabToEditPage(PLATE_INFO pi)
        {
            TabPage page = new TabPage();

            LPRInteractiveEditUC.LPRInteractiveEditUC LPREditor;
            LPREditor = new LPRInteractiveEditUC.LPRInteractiveEditUC(m_AppData);
            LPREditor.Location = new Point(10, 10);
            page.Controls.Add(LPREditor);
            page.Text = pi.plateNumber;

            tabControlLPRResults.TabPages.Add(page);

            m_TabPageLPREditorTable.Add(page, LPREditor);

            LPREditor.PostPicture(pi.plateImage, pi.plateNumber);
            char[] plateCharsArray = pi.plateNumber.ToCharArray();
            for (int c = 0; c < pi.numChars; c++)
            {
                LPREditor.PostCharImage(pi.charImages[c], c, plateCharsArray[c].ToString());
            }

         
         
        }

        void ClearDisplayTable()
        {
            tabControlLPRResults.TabPages.Clear();
            m_TabPageLPREditorTable = new ThreadSafeHashTable(20);
        }

        int[,] GetLuminanceArray(Bitmap bmp)
        {


            int[,] ipDest = new int[bmp.Width, bmp.Height];

            // Lock the bitmap's bits.  
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            System.Drawing.Imaging.BitmapData bmpData =
                bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                bmp.PixelFormat);

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            bool invert = false;

            LPROCR_Wrapper.LPROCR_Lib.extractFromBmpDataToLumArray(ptr, ipDest, bmpData.Stride, bmp.Width, bmp.Height, invert);

            bmp.UnlockBits(bmpData);

            return (ipDest);


        }

       
        void StoreResult()
        {
            if (m_CurrentImageResult == null) return;


            for (int p = 0; p < m_CurrentImageResult.Count(); p++)
            {
                if (m_CurrentImageResult[p].resultHasBeenSaved) return;

                FRAME frame = new FRAME(m_AppData);

                // manufacture a jpeg into memory from the source image bitmap
                Image image = m_CurrentImageResult[p].sourceImage;
                MemoryStream stream = new MemoryStream();
                image.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
                frame.Jpeg = stream.ToArray();

                frame.PlateNativeLanguage = "LATIN";

                string[] plateNumbers = new string[1];
                plateNumbers[0] = m_CurrentImageResult[p].plateNumber;

                frame.PlateNumberLatin = plateNumbers;

                frame.PlateNumberNativeLanguage = plateNumbers;
                   
                frame.SourceChannel = 0;
                frame.SourceName = ((m_CurrentImageResult[p].sourceFileName.Replace("\\", "_")).Replace(":", "_")).Replace(".", "_");

                frame = m_FrameGenerator.CompleteFrameDataToByPassLPR(frame);

                frame.JpegFileRelativePath = m_PathManager.GetJpegRelateivePath(frame);

                m_LPREngine.PushHandEditedPlate(frame); // generate the LPR result event into the storage repository

                m_DVR.SendFrameDirectlyToStorage(frame); // send the image directly into the storage repository
           
                m_CurrentImageResult[p].resultHasBeenSaved = true;
            }
        
        }



        void ColorizeEdgeMap(int[,] lum, byte[,] r, byte[,] g, byte[,] b)
        {
            int min = Int32.MaxValue;
            int max = Int32.MinValue;

            for (int x = 0; x < lum.GetLength(0); x++)
            {
                for (int y = 0; y < lum.GetLength(1); y++)
                {
                    if (lum[x, y] > max) max = lum[x, y];
                    if (lum[x, y] < min) min = lum[x, y];
                }
            }

            double[,] lumf = new double[lum.GetLength(0), lum.GetLength(1)];
            double span = max - min;

            for (int x = 0; x < lum.GetLength(0); x++)
            {
                for (int y = 0; y < lum.GetLength(1); y++)
                {
                    lumf[x,y] = (double) lum[x, y] - min;
                    lumf[x, y] = ((double)lumf[x, y] *  768.0) / span;

                    if (lumf[x, y] < 256.0)
                    {
                        g[x, y] = (byte)lumf[x, y];
                        r[x,y] = 0;
                        b[x,y] = 0;
                    }
                    else if (lumf[x, y] < 512.0)
                    {
                        g[x, y] = 255;
                        r[x, y] = 0;
                        b[x, y] = (byte)lumf[x, y];
                    }
                    else 
                    {
                        g[x, y] = 255;
                        r[x, y] = (byte)lumf[x, y];
                        b[x, y] = 255;
                    }
                }
            }
        }

        public static void putPixelsIntoBmp(Bitmap bmp, byte[,] r, byte[,] g, byte[,] b)
        {

            unsafe
            {
                try
                {
                    // Lock the bitmap's bits.  
                    Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                    System.Drawing.Imaging.BitmapData bmpData =
                        bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                        bmp.PixelFormat);

                    // Get the address of the first line.
                    IntPtr ptr = bmpData.Scan0;

                    // Declare an array to hold the bytes of the bitmap.
                    int bytes = bmpData.Stride * bmp.Height;
                    //  byte[] rgbValues = new byte[bytes];

                    byte* rgbValues = (byte*)ptr.ToPointer();

                    int pixelOffset = bmpData.Stride / bmp.Width;


                    int x = 0;
                    int y = 0;
                    int bi = 0;


                    for (y = 0; y < bmp.Height; y++)
                    {
                        for (x = 0; x < bmp.Width; x++)
                        {
                            // some guy on code project says the values are in B G R order
                            rgbValues[bi] = (byte)r[x, y];
                            rgbValues[bi + 1] = (byte)g[x, y];
                            rgbValues[bi + 2] = (byte)b[x, y];
                            if (pixelOffset > 3) rgbValues[bi + 3] = 255; // alpha
                            bi += pixelOffset;
                        }
                    }
                    // Copy the RGB values into the array.
                    //   System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);

                    // Unlock the bits.
                    bmp.UnlockBits(bmpData);
                }
                catch (Exception ex) { MessageBox.Show(ex.Message + ex.StackTrace); }

            }
        }

    }

}