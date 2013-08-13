using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices; //For DLL support
using System.Threading;
using System.IO;
using S2255Controller;
using LPROCR_Wrapper;
using ApplicationDataClass;
using ErrorLoggingLib;

namespace Test2255InterfaceCS
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            m_AppData = new APPLICATION_DATA();

            m_Log = new ErrorLog(m_AppData);

            m_AppData.Logger = m_Log;
   
            this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);

            m_S2255Controller = new S2255Controller.S2255Controller(S2255Controller.S2255Controller.PAL_NTSC_MODE.NTSC, m_AppData);
           
        }

        S2255Controller.S2255Controller m_S2255Controller;
        ErrorLog m_Log;
        bool m_Stopping = false;
        Thread m_StopProgram;

     
        APPLICATION_DATA m_AppData;

        void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
          

            if (!m_Stopping)
            {
                e.Cancel = true;
                m_StopProgram = new Thread(StopProgram);
                m_StopProgram.Start();
            }
            
        }

        void StopProgram()
        {
            m_S2255Controller.StopThreads();

            m_Stopping = true;
            this.BeginInvoke((MethodInvoker)delegate { this.Close(); });

        }

        private void Form1_Load(object sender, EventArgs e)
        {
          
        }

        bool busy = false;

        void HandleNewFramePairs(FRAME_PAIR fp)
        {
            if (busy) return;

            busy = true;

            ConvertBmp(fp.portPairIndex, (Bitmap)fp.bmp);

            DisplayJpeg(fp.portPairIndex, fp.jpeg);

            busy = false;
        }

        object olock = new object();

        void ConvertBmp(int index, Bitmap bmp)
        {
            lock (olock)
            {
                int[,] luminance = new int[bmp.Width, bmp.Height];

                getPixelsFromImageInY(bmp, ref luminance);

                Bitmap nBmp = new Bitmap(bmp.Width, bmp.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                putPixelsIntoBmp(ref nBmp, luminance);



                DisplayBmp(index, (Bitmap)nBmp);
            }

        }

        public delegate void PrettyMuchUseless_DisplayJpeg_CBDelegate(int index, byte[] jpeg);

        void DisplayJpeg(int index, byte[] jpeg)
        {
            PictureBox pb = null;
            if (index == 0) pb = pictureBox2;
            else pb = pictureBox4;

            if (pb.InvokeRequired)
            {
                this.Invoke(new PrettyMuchUseless_DisplayJpeg_CBDelegate(DisplayJpeg), new object[] {index, jpeg });
            }
            else
            {
                pb.Image = Bitmap.FromStream(new MemoryStream(jpeg));
                //DateTime current = DateTime.Now;

                //string fileName = "C:\\Users\\David\\Pictures\\test\\" + (current.ToLongTimeString().Replace(':', '_')) +"_" +current.Millisecond.ToString().Replace(':', '_') + ".jpg";
                //try
                //{
                //    FileStream fs = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite);
                //    BinaryWriter bw = new BinaryWriter(fs);
                //    bw.Write(jpeg);
                //    bw.Close();

                //}
                //catch (Exception ex)
                //{
                //    Console.WriteLine(ex.Message);
                //}
            }
        }

        public delegate void PrettyMuchUseless_DisplayBmp_CBDelegate(int index, Bitmap bmp);

        void DisplayBmp(int index, Bitmap bmp)
        {
            PictureBox pb = null;
            if (index == 0) pb = pictureBox1;
            else pb = pictureBox3;

           

            if (pb.InvokeRequired)
            {
                this.Invoke(new PrettyMuchUseless_DisplayBmp_CBDelegate(DisplayBmp), new object[] { index, bmp });
            }
            else
            {
                pb.Image = (Image)bmp;
               // this.BeginInvoke((MethodInvoker)delegate { this.LPREngineProcessFrame(bmp); });
            }
        }


        unsafe public void getPixelsFromImageInY(Bitmap bmp, ref int[,] Y)
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

            int pixelOffset = bmpData.Stride / bmp.Width;

            fixed (int* arrayPtr = Y)
            {
                int* srcPtr = (int*)ptr;
                IntPtr dest = new IntPtr((void*)arrayPtr);


                // Copy the RGB values into the array.
                //                LPROCR_Lib.MemCopyInt(srcPtr, dest, bytes / 4);
                LPROCR_Lib.MemCopyByteArrayToIntArray(bmpData.Scan0, dest, bmp.Width * bmp.Height, bmp.Width, bmp.Height);
            }

            // Unlock the bits.
            bmp.UnlockBits(bmpData);

        }


        public void putPixelsIntoBmp(ref Bitmap bmp, int[,] Y)
        {

            unsafe
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


                for (y = 0; y < Y.GetLength(1); y++)

                {
                    for (x = 0; x < Y.GetLength(0); x++)
                    {
                        // some guy on code project says the values are in B G R order
                        rgbValues[bi] = (byte)Y[x, y];
                        rgbValues[bi + 1] = (byte)Y[x, y];
                        rgbValues[bi + 2] = (byte)Y[x, y];
                        rgbValues[bi + 3] = 255; // alpha
                        bi += pixelOffset;
                    }
                    while (bi % 4 != 0) bi++;
                }
                // Copy the RGB values into the array.
                //   System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);

                // Unlock the bits.
                bmp.UnlockBits(bmpData);

            }
        }


       
    }
}
