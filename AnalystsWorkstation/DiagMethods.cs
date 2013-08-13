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
using PixelLib;


///   this file contains the methods related the Diag tab
///   
namespace AnalystsWorkstation
{
    public partial class AnalystsWorkstationMainForm : Form
    {
        ThreadSafeHashTable m_TabPageLPRDiagnosticsTable;

        void AddPlateTabToDiagPage(PLATE_INFO pi)
        {
            TabPage page = new TabPage();

            LPRInteractiveEditUC.LPRInteractiveEditUC LPRDiagDisplay;
            LPRDiagDisplay = new LPRInteractiveEditUC.LPRInteractiveEditUC(m_AppData);
            LPRDiagDisplay.Location = new Point(10, 10);
            page.Controls.Add(LPRDiagDisplay);
            page.Text = (pi.plateNumber == null )? " ": pi.plateNumber;

            tabControlDiagdisplay.TabPages.Add(page);

            m_TabPageLPRDiagnosticsTable.Add(page, LPRDiagDisplay);

            Bitmap BinarizedPlateImage = CreateTestBinarizedImage(pi);

            // the binarized from the test method in this C# program:
       //     LPRDiagDisplay.PostPicture(BinarizedPlateImage, (pi.plateNumber == null) ? " " : pi.plateNumber);

          LPRDiagDisplay.PostPicture(pi.plateImage, (pi.plateNumber == null )? " ": pi.plateNumber);


            LPRDiagDisplay.PostHistogram(pi.histoBmp, pi.histString);

            for (int c= 0; c < pi.charImages.Count(); c++)
            {
                LPRDiagDisplay.PostCharImage(pi.charImages[c], c, " ");
            }
            


        }

        Bitmap CreateTestBinarizedImage(PLATE_INFO pi)
        {
            // get histogram
            int[,] plum = new int[pi.plateImageUnprocessed.Width, pi.plateImageUnprocessed.Height];

            PixelLib.Pixels.getPixelsFromImage(pi.plateImageUnprocessed, plum);

            CreateHistoBounds(pi);
    
            int[] histo = new int[256];
            int[] integration = new int[256];
            Bitmap hBmp = new Bitmap(256, 100);
            HistoResults histoStats=null;
            historgram(plum, histo, integration, true, hBmp, pi.histoBounds, out histoStats);

            int[,] binarizedLum = new int[pi.plateImageUnprocessed.Width, pi.plateImageUnprocessed.Height];

            BinarizeOnPlate(plum, binarizedLum, histoStats);

            Bitmap binBmp = new Bitmap(pi.plateImageUnprocessed.Width, pi.plateImageUnprocessed.Height);

            Pixels.putPixels(binBmp, binarizedLum);

            return (binBmp);
        }

        void BinarizeOnPlate(int[,] inLum, int[,] outLum, HistoResults histStats)
        {
           
       

            for (int y = 0; y < outLum.GetLength(1); y++)
            {
                for (int x = 0; x < outLum.GetLength(0); x++)
                {
                    if (inLum[x, y] < (histStats.darkEnergyThreshold))
                        outLum[x, y] = 0;
                    else
                        outLum[x, y] = 255;
                   

                  
                }
            }
        }

        enum PIX_TRI_STATE_VAL { WHITE, BLACK, NEITHER }

        int GetLeftEdgeColor(int[,] inLum, int x, int y, HistoResults histStats)
        {

            if (x < 3) x = 3;
            if (x > inLum.GetLength(0)-4) x = inLum.GetLength(0) - 4;

            if (y < 0) y = 0;
            if (y > inLum.GetLength(1) - 1) y = inLum.GetLength(1) - 1;

            int pixAve = (inLum[x - 2, y] + inLum[x - 1, y] + inLum[x, y])/3;

            return (pixAve);
        }

        int GetRightEdgeColor(int[,] inLum, int x, int y, HistoResults histStats)
        {

            if (x < 0) x = 0;
            if (x > inLum.GetLength(0) - 4) x = inLum.GetLength(0) - 4;

            if (y < 0) y = 0;
            if (y > inLum.GetLength(1) - 1) y = inLum.GetLength(1) - 1;

            int pixAve = (inLum[x + 2, y] + inLum[x + 1, y] + inLum[x, y]) / 3;

            return (pixAve);
        }

        void ClearDiagDisplayTable()
        {
            tabControlDiagdisplay.TabPages.Clear();
            m_TabPageLPRDiagnosticsTable = new ThreadSafeHashTable(20);
        }

        bool DiagPB1FullSize = false;
        Point pictureBoxDiag1_Location;
        Size pictureBoxDiag1_Size;
        private void pictureBoxDiag1_Click(object sender, EventArgs e)
        {
            if (DiagPB1FullSize)
            {
                // put it back to normal size

                DiagPB1FullSize = false;
                pictureBoxDiag1.Location = pictureBoxDiag1_Location;
                pictureBoxDiag1.Size = pictureBoxDiag1_Size;
                pictureBoxDiag1.Invalidate();
            }
            else
            {
                // make it really big
                pictureBoxDiag1_Location = pictureBoxDiag1.Location;
                pictureBoxDiag1_Size = pictureBoxDiag1.Size;
                DiagPB1FullSize = true;
                pictureBoxDiag1.Location = new Point(0,0);
                pictureBoxDiag1.Size = new Size(tabPageLPRDiagnostics.Size.Width, tabPageLPRDiagnostics.Size.Height);
                pictureBoxDiag1.BringToFront();
                pictureBoxDiag1.Invalidate();
            }
        }

        bool DiagPB2FullSize = false;
        Point pictureBoxDiag2_Location;
        Size pictureBoxDiag2_Size;
        private void pictureBoxDiag2_Click(object sender, EventArgs e)
        {
          
            if (DiagPB2FullSize)
            {
                // put it back to normal size

                DiagPB2FullSize = false;
                pictureBoxDiag2.Location = pictureBoxDiag2_Location;
                pictureBoxDiag2.Size = pictureBoxDiag2_Size;
                pictureBoxDiag2.Invalidate();
            }
            else
            {
                // make it really big

                pictureBoxDiag2_Location = pictureBoxDiag2.Location;
                pictureBoxDiag2_Size = pictureBoxDiag2.Size;
                DiagPB2FullSize = true;
                pictureBoxDiag2.Location = new Point(0, 0);
                pictureBoxDiag2.Size = new Size(tabPageLPRDiagnostics.Size.Width, tabPageLPRDiagnostics.Size.Height);
                pictureBoxDiag2.BringToFront();
                pictureBoxDiag2.Invalidate();
            }
        }


        bool DiagPB3FullSize = false;
        Point pictureBoxDiag3_Location;
        Size pictureBoxDiag3_Size;
        private void pictureBoxDiag3_Click(object sender, EventArgs e)
        {

            if (DiagPB3FullSize)
            {
                // put it back to normal size

                DiagPB3FullSize = false;
                pictureBoxDiag3.Location = pictureBoxDiag3_Location;
                pictureBoxDiag3.Size = pictureBoxDiag3_Size;
                pictureBoxDiag3.Invalidate();
            }
            else
            {
                // make it really big

                pictureBoxDiag3_Location = pictureBoxDiag3.Location;
                pictureBoxDiag3_Size = pictureBoxDiag3.Size;
                DiagPB3FullSize = true;
                pictureBoxDiag3.Location = new Point(0, 0);
                pictureBoxDiag3.Size = new Size(tabPageLPRDiagnostics.Size.Width, tabPageLPRDiagnostics.Size.Height);
                pictureBoxDiag3.BringToFront();
                pictureBoxDiag3.Invalidate();
            }
        }

        bool pictureBoxDiagHistogramFullSize = false;
        Point pictureBoxDiagHistogram_Location;
        Size pictureBoxDiagHistogram_Size;
        private void pictureBoxDiagHistogram_Click(object sender, EventArgs e)
        {

            if (pictureBoxDiagHistogramFullSize)
            {
                // put it back to normal size

                pictureBoxDiagHistogramFullSize = false;
                pictureBoxDiagHistogram.Location = pictureBoxDiagHistogram_Location;
                pictureBoxDiagHistogram.Size = pictureBoxDiagHistogram_Size;
                pictureBoxDiagHistogram.Invalidate();
            }
            else
            {
                // make it really big

                pictureBoxDiagHistogram_Location = pictureBoxDiagHistogram.Location;
                pictureBoxDiagHistogram_Size = pictureBoxDiagHistogram.Size;
                pictureBoxDiagHistogramFullSize = true;
                pictureBoxDiagHistogram.Location = new Point(0, 0);
                pictureBoxDiagHistogram.Size = new Size(tabPageLPRDiagnostics.Size.Width, tabPageLPRDiagnostics.Size.Height);
                pictureBoxDiagHistogram.BringToFront();
                pictureBoxDiagHistogram.Invalidate();
            }
        }



        public void plotCurve(Bitmap bmp, int[] curve, int height, int yStart)
        {
            // generic curve plotting tool

            int x;
            long min = 99999999;
            long max = -999999999;

            if (yStart + height >= bmp.Height) height = bmp.Height - yStart - 1;

            // find the min and max values in the data
            for (x = 0; x < curve.Length; x++)
            {
                if (curve[x] > max) max = curve[x];
                if (curve[x] < min) min = curve[x];
            }

            if (max == min) return;

            long xRange = max - min;


            // now plot this data in the process window
            int yPlotValue = 0;
            int yPlotValue2 = 0;

            Color c = Color.FromKnownColor(KnownColor.Red);


            for (x = 0; x < (curve.Length - 1) && x < (bmp.Width - 1); x++)
            {
                long curveVal = curve[x];
                // shift the curve so that min point is zero
                curveVal = curveVal - (min);

                yPlotValue = yStart + (int)((curveVal * height) / xRange);


                curveVal = curve[x + 1];
                // shift the curve so that min point is zero
                curveVal = curveVal - (min);

                yPlotValue2 = yStart + (int)((curveVal * height) / xRange);


                Pen p = new Pen(c);
                Graphics g;
                g = Graphics.FromImage(bmp);

                g.DrawLine(p, x, yPlotValue, x + 1, yPlotValue2);

            }

        }


        class HistogramBounds
        {
            public int leftEdge;
            public int rightEdge;
            public int topEdge;
            public int bottomEdge;
        }

        class HistoResults
        {
            public int ave;
            public int darkMoment;
            public int lightMoment;
            public int stdDev;
            public int darkPeakCount;
            public int lightPeakCount;
            public int darkEnergyThreshold;
        }

        string historgram(int[,] Y, int[] histogram,  int[] integration, bool plateHisto,  Bitmap bmp, HistogramBounds bounds, out HistoResults results)
        {
            int xStart;
            int xEnd;
            int yStart;
            int yEnd;

            results = new HistoResults();

            xStart = bounds.leftEdge;
            xEnd = bounds.rightEdge;
            yStart = bounds.topEdge;
            yEnd = bounds.bottomEdge;

            int x = 0;
            int y = 0;

            int ave = 0;
            int aveCnt = 0;

            int sum = 0;

            for (x = xStart; x < xEnd; x++)
            {
                for (y = yStart; y < yEnd; y++)
                {
                    int val = Y[x, y];
                    histogram[val]++;
                    ave += val;
                    aveCnt++;
                }
            }
            int area = ave;
            ave = ave / aveCnt;

            for (int i = 0; i < histogram.Length; i++)
            {
                sum += histogram[i];
                integration[i] = sum;
            }

            // find where the integration hits 10% of the max area
            area = integration[integration.Length - 1];
            results.darkEnergyThreshold = 0;
            for (int i = 0; i < histogram.Length; i++)
            {
                if ( integration[i] > ( (35 * area) / 100))
                {
                    results.darkEnergyThreshold = i;
                    break;
                }
            }


            // get dark moment
            results.darkPeakCount = 0;
            int darkMoment = 0;
            for (int i = 0; i < ave; i++)
            {
                if (histogram[i] > results.darkPeakCount)
                {
                    results.darkPeakCount = histogram[i];
                    darkMoment = i;
                }
            }

            // get light moment
            results.lightPeakCount = 0;
            int lightMoment = 0;
            for (int i = ave; i < histogram.Count(); i++)
            {
                if (histogram[i] > results.lightPeakCount)
                {
                    results.lightPeakCount = histogram[i];
                    lightMoment = i;
                }
            }

            // get standard deviation from the median
            int median = (darkMoment + lightMoment) / 2;
            float stddev = 0;
            for (int i = 0; i < histogram.Count(); i++)
            {
                stddev += histogram[i] * Math.Abs((ave - i));
            }
            if (area == 0) area = 1;
            stddev /= area;
            stddev = stddev * 256;

            string histString ="stddev = "+stddev.ToString()+ "ave = " + ave.ToString() + ", dark mnt = " + darkMoment.ToString() + ", light mnt = " + lightMoment.ToString(); ;

            
            results.ave = ave;
            results.lightMoment = lightMoment;
            results.darkMoment = darkMoment;
            results.stdDev = (int) stddev;

            Color cl;
            cl = Color.FromKnownColor(KnownColor.Yellow);


            plotCurve( bmp, histogram, bmp.Height, 0);

            plotCurve( bmp, integration, bmp.Height, 0);

            // draw a line at 1/2 mark
            plotLine( bmp, bmp.Width / 2, bmp.Width / 2, 0, bmp.Height, cl);
            plotLine( bmp, bmp.Width / 2 + 1, bmp.Width / 2 + 1, 0, bmp.Height, cl);
            plotLine( bmp, bmp.Width / 2 + 2, bmp.Width / 2 + 2, 0, bmp.Height, cl);

            // draw a line at ave

            int aveX = ave;
            cl = Color.FromKnownColor(KnownColor.Green);

            plotLine( bmp, aveX, aveX, 0, bmp.Height - 1, cl);
            plotLine( bmp, aveX + 1, aveX + 1, 0, bmp.Height - 1, cl);
            plotLine(bmp, aveX + 2, aveX + 2, 0, bmp.Height - 1, cl);


            // draw a line at darkEnergyThreshold

            aveX = results.darkEnergyThreshold;
            cl = Color.FromKnownColor(KnownColor.Azure);

            plotLine(bmp, aveX, aveX, 0, bmp.Height - 1, cl);
            plotLine(bmp, aveX + 1, aveX + 1, 0, bmp.Height - 1, cl);
            plotLine(bmp, aveX + 2, aveX + 2, 0, bmp.Height - 1, cl);

            return (histString);
        }




        public void plotLine( Bitmap bmp, int xStart, int xEnd, int yStart, int yEnd, Color c)
        {
            Point p1 = new Point(xStart, yStart);
            Point p2 = new Point(xEnd, yEnd);
            Pen p = new Pen(c);

            Graphics g = Graphics.FromImage(bmp);

            g.DrawLine(p, p1, p2);

            g.Dispose();


        }// end plotLine


    }

}