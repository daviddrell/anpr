using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ApplicationDataClass;
using System.Threading;
using ErrorLoggingLib;
using System.Drawing;
using Utilities;
using LPROCR_Wrapper;
using UserSettingsLib;
using System.Collections;
using DirectShowLib;
using System.Windows.Forms;
using System.IO;



namespace FrameGeneratorLib
{
    // play a list of jpeg images as if from a video source, for batch mode processing

    class PlayJpegsInBatchMode : IFilePlayerInterface
    {
        public PlayJpegsInBatchMode(APPLICATION_DATA appData, Panel parentPanel, Form parentForm, int chan)
        {

            m_AppData = appData;
            m_AppData.AddOnClosing(OnStop, APPLICATION_DATA.CLOSE_ORDER.FIRST);
            m_Log = (ErrorLog)m_AppData.Logger;

            m_AppData.DVRMode = APPLICATION_DATA.DVR_MODE.STORE_ON_PLATE_FOUND; // need this set for LPR to not consolidate multiple video frames

            m_Channel = chan;
            m_ParentForm = parentForm;
            m_ParentPanel = parentPanel;


            m_DisplayImagePB = new PictureBox();
            m_DisplayImagePB.SizeMode = PictureBoxSizeMode.StretchImage;
            m_DisplayImagePB.BackColor = Color.Black;
            m_DisplayImagePB.Size = m_ParentPanel.Size;

            m_ParentPanel.Controls.Add(m_DisplayImagePB);
            m_DisplayImagePB.BringToFront();
          

        }

        PictureBox m_DisplayImagePB;
        int m_Channel;
        ErrorLog m_Log;
        APPLICATION_DATA m_AppData;

        Thread m_PlayFilesThread;

        Panel m_ParentPanel;
        Form m_ParentForm;

        bool playing = false;

        bool m_Stop=false;
        void OnStop()
        {
            m_Stop = true;
        }


        public void Dispose()
        {
            if (m_DisplayImagePB == null) return;


            m_ParentPanel.Parent.BeginInvoke ((MethodInvoker) delegate{ m_ParentPanel.Controls.Remove(m_DisplayImagePB);});
            m_ParentPanel.Parent.BeginInvoke((MethodInvoker)delegate { m_ParentPanel.Refresh(); });

            m_ParentPanel.Parent.BeginInvoke ((MethodInvoker) delegate{ m_ParentPanel.Invalidate();});
        }

        public void StopGraph()
        {
            OnStop();
            Dispose();
        }

        public void Start(string directory)
        {
            m_FilesToPlay = BatchLoadJpegsFromDirectory(directory);

            m_PlayFilesThread = new Thread(PlayFilesLoop);
            m_PlayFilesThread.Start();

            playing = true;
        }

        string[] m_FilesToPlay;

        public void Start(string[] files)
        {
           
        }

        public event FrameGeneratorLib.OnNewFrameEvent OnNewFrame;
        public event FrameGeneratorLib.OnEndOfFile OnEndOfFileEvent;

        ThreadSafeQueue<Bitmap> m_BitmapsForDisplayQ;

        void PushBitmapToDisplay()
        {
            Bitmap bmp= m_BitmapsForDisplayQ.Dequeue();
            if (bmp != null)
                m_DisplayImagePB.Image = bmp;
        }

        void PlayFilesLoop()
        {
            int frameRate = 100;
            int frameDelay = ( 1000 / frameRate);
            int currentFrame=0;
            Bitmap bmp= null;

            m_BitmapsForDisplayQ = new ThreadSafeQueue<Bitmap>(30);

            while (!m_Stop)
            {
                while (playing)
                {
                    if (m_Stop) break;
                    Thread.Sleep(frameDelay);

                    // when processing plate image jpegs (i.e. just the clipped out plate in a very small image) the player can go too fast for the image processing
                    //   resulting in Que overruns, so slow down to prevent loss
                    if (m_AppData.LPRGettingBehind ||  m_AppData.MotionDetectionGettingBehind || m_AppData.DVRStoringLPRRecordsGettingBehind) 
                        Thread.Sleep(250); // slow down a bit

                    if (m_FilesToPlay == null) continue;
                    if (m_FilesToPlay.Count() < 1) continue;

                    try
                    {
                        bmp =(Bitmap) Bitmap.FromFile(m_FilesToPlay[currentFrame]);
                        
                    }
                    catch { }

                    if (bmp == null) continue;


                    Bitmap forDisplayToOwn = new Bitmap(bmp);
                    m_BitmapsForDisplayQ.Enqueue(forDisplayToOwn);
                    m_ParentPanel.Parent.BeginInvoke((MethodInvoker)delegate { PushBitmapToDisplay(); });

                    FRAME frame = new FRAME(m_AppData);

                    frame.NotVideoEachFrameIsUniqueSize = true;

                    int[,] lum = new int[bmp.Width, bmp.Height];
                    
                    getPixelsFromImageInY(bmp, ref lum);

                    frame.Luminance = lum;
                    frame.TimeStamp = new FileInfo(m_FilesToPlay[currentFrame]).LastWriteTimeUtc;
                    frame.Bmp = bmp;
                    frame.SourceChannel = m_Channel;
              //      frame.SourceName = ((m_FilesToPlay[currentFrame].Replace("\\", "_")).Replace(":", "_")).Replace(".", "_");


                    frame.SourceName = m_AppData.UserSpecifiedCameraName == null ? ("stilljpg") : m_AppData.UserSpecifiedCameraName;
                  
                
                    frame.SetFileName();
                    OnNewFrame(frame);
                  

                    currentFrame++;
                    if (currentFrame == m_FilesToPlay.Length)
                    {
                        OnEndOfFileEvent(); // we are done.
                        playing = false;
                        m_Stop = true;// this is a one-time use thread
                        break;
                    }

                  
                   
                }
                Thread.Sleep(1);
            }
        }

        
        string[] BatchLoadJpegsFromDirectory(string path)
        {
            string[] jpegsToProcess = null;

            try
            {

                string[] sa1 = Directory.GetFiles(path, "*.jpg");
                string[] sa2 = Directory.GetFiles(path, "*.jpeg");
                string[] sa3 = Directory.GetFiles(path, "*.bmp");

                jpegsToProcess = new string[sa1.Length + sa2.Length + sa3.Length];

                sa1.CopyTo(jpegsToProcess, 0);
                sa2.CopyTo(jpegsToProcess, sa1.Length);
                sa3.CopyTo(jpegsToProcess, sa2.Length);



                if (jpegsToProcess == null)
                {
                    MessageBox.Show("No images found");
                 
                    return null;
                }

                if (jpegsToProcess.Count() == 0)
                {
                    MessageBox.Show("No images found");
                  
                    return null;
                }

                if (jpegsToProcess.Count() > m_AppData.MAX_IMAGES_TO_EDIT)
                {
                    MessageBox.Show("Too many images to load, max is " + m_AppData.MAX_IMAGES_TO_EDIT.ToString());
                   
                    return null;
                }

            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }

            return (jpegsToProcess);
        }

     

        public void getPixelsFromImageInY(Bitmap bmp, ref int[,] Y)
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
            byte[] rgbValues = new byte[bytes];

            int pixelOffset = bmpData.Stride / bmp.Width;

            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

            int x = 0;
            int y = 0;
            int b = 0;
            int bv = 0;
            int rv = 0;
            int gv = 0;

            for (y = 0; y < Y.GetLength(1); y++)
            {
                for (x = 0; x < Y.GetLength(0); x++)
                {
                    // some guy on code project says the values are in B G R order
                    bv = rgbValues[b] * 114;
                    gv = rgbValues[b + 1] * 587;
                    rv = rgbValues[b + 2] * 299;

                    Y[x, y] = ((((bv + rv + gv) / 1000)));

                    b += pixelOffset;
                }
                while (b % 4 != 0) b++;
            }


            // Unlock the bits.
            bmp.UnlockBits(bmpData);

        }




    }
}
