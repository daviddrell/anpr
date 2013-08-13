using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using ApplicationDataClass;
using LPROCR_Wrapper;
using LPREngineLib;
using System.IO;

namespace AnalystsWorkstation
{
    class GenerateOCRLib
    {
        public GenerateOCRLib(APPLICATION_DATA appData, string srcDir, string outDir)
        {
            m_AppData = appData;
            m_LPREngine = (LPREngine)m_AppData.LPREngine;
            m_LPR_C_Lib = m_LPREngine.m_LPRFuntions;

            m_SourceDirectory = srcDir;

            m_DesitnationDirectory = outDir;
            m_DestinationFile = m_SourceDirectory + "\\OCR_charLib.h";
        }

        LPREngine m_LPREngine;
        APPLICATION_DATA m_AppData;
        string m_DestinationFile;
        string m_DesitnationDirectory;
        string m_SourceDirectory;
        string m_VersionString = "not implemented";
        string m_LibName = "not implemented";
        string[] m_CharFileList;
        LPROCR_Lib m_LPR_C_Lib;

        public void GenerateLibrary()
        {

            DeleteOldFile();

            GetCharCount();

            WriteFileHeader();

            WriteCharData();

            WriteFileFooter();

        }



        void WriteCharData()
        {
            bool lastChar = false;

            foreach (string file in m_CharFileList)
            {
                if (m_CharFileList.ElementAt(m_CharFileList.Count() - 1).Equals(file))
                    lastChar = true;

                WriteMaskFile(file, lastChar);
            }
        }

        void WriteMaskFile(string path, bool lastChar)
        {
            Bitmap bmp = new Bitmap(path);



            FileInfo fi = new FileInfo(path);

            string inputNameOnly = fi.Name;
            string intputNameNoExtention;

            if (inputNameOnly.Contains("_"))
            {
                string[] s = inputNameOnly.Split('_');
                intputNameNoExtention = s[0];
            }
            else
            {
                string[] s = inputNameOnly.Split('.');
                intputNameNoExtention = s[0];
            }

            string inputDirectory = fi.DirectoryName;

            string outPath = m_DestinationFile;

            int[,] charY = new int[bmp.Width, bmp.Height];

            getPixelsFromImageInY(bmp, ref charY);

            if (bmp.Width != 20 || bmp.Height != 40)
            {
                LPROCR_Lib LPR_C_Lib = new LPROCR_Lib();

                int[,] newScaledArray = new int[20, 40];

                LPR_C_Lib.OCR_prepChar(charY, charY.GetLength(0), charY.GetLength(1), newScaledArray);

                charY = newScaledArray;

                bmp = new Bitmap(20, 40);

                putPixelsFromImageinYBW(ref bmp, charY);

            }




            string line1 = "{\"" + intputNameNoExtention + "\", //name \r\n";
            string line2 = "{ \r\n";

            File.AppendAllText(outPath, line1);
            File.AppendAllText(outPath, line2);

            StringBuilder sb;


            for (int x = 0; x < charY.GetLength(0); x++)
            {
                sb = new StringBuilder();

                sb.Append("{ ");

                for (int y = 0; y < charY.GetLength(1); y++)
                {
                    int val;
                    if (charY[x, y] <= 0) val = -1;
                    else val = 1;

                    //   int val = charY[x, y];

                    sb.Append(val.ToString());

                    if (y != charY.GetLength(1) - 1) sb.Append(", ");
                    else sb.Append(" ");
                }

                if (x != charY.GetLength(0) - 1) sb.Append(" },\r\n");  // not last data line for this data entry
                else sb.Append(" }\r\n");   // is last data line 

                File.AppendAllText(outPath, sb.ToString());
            }

            if (!lastChar) line1 = " }},\r\n"; // close this array entry, but not last array data entry
            else line1 = " }}\r\n";

            File.AppendAllText(outPath, line1);

        }

        void WriteFileFooter()
        {
            string[] footerLines = {  
                "    }; \r\n",  
                "#endif\r\n" , "#endif\r\n" };

            foreach (string s in footerLines)
            {
                File.AppendAllText(m_DestinationFile, s);
            }


        }

        void WriteFileHeader()
        {

            string numCharline = "#define NUM_CHARS  " + m_CharFileList.Count().ToString();
            string versionStringline = "char * m_CharLibVersion=\"" + m_VersionString + "\";\r\n";
            string libNameline = "char * m_CharLibName=\"" + m_LibName + "\";\r\n";
            string buildDateline = "char * m_BuildDate=\"" + DateTime.Now.ToLocalTime() + "\";\r\n";

            string[] headerLines = {
                    "// OCR_charLib.h\r\n",
                    "#ifndef INCLUDE_ONCE_OCR_CHARLIB\r\n",
                    "#define INCLUDE_ONCE_OCR_CHARLIB\r\n",
                    " \r\n",
                    numCharline ,
                    " \r\n",
                    "struct CHAR_MASKS\r\n",
                    " {\r\n",
                    "    char *name;\r\n",
                    "    int  mask[ STANDARD_WIDTH * STANDARD_HEIGHT ];\r\n",
                    "    int  maskInverse[ STANDARD_WIDTH * STANDARD_HEIGHT ];\r\n",
                    "}; \r\n",
                                                  " \r\n",
                    "struct CHAR_DATA\r\n",
                    "{\r\n",
                    "    char *name;\r\n",
                    "    int data[20][40];\r\n",
                    "};\r\n",            

                    "\r\n",
                    "#ifdef OCR_MAIN\r\n",

                    "struct CHAR_MASKS masks[NUM_CHARS];\r\n",
                        " \r\n",
                        " \r\n",
                         " \r\n",
                    versionStringline ,
                    libNameline ,
                    buildDateline,
                    "CHAR_DATA  m_CharLibData[NUM_CHARS]={\r\n",
                                };

            foreach (string s in headerLines)
            {
                File.AppendAllText(m_DestinationFile, s);
            }

        }

        void GetCharCount()
        {

            string[] bmpfiles = Directory.GetFiles(m_SourceDirectory, "*.bmp");
            string[] jpgfiles = Directory.GetFiles(m_SourceDirectory, "*.jpg");

            m_CharFileList = new string[bmpfiles.Length + jpgfiles.Length];
            bmpfiles.CopyTo(m_CharFileList, 0);
            jpgfiles.CopyTo(m_CharFileList, bmpfiles.Length);




        }

        void DeleteOldFile()
        {
            if (File.Exists(m_DestinationFile))
                File.Delete(m_DestinationFile);
        }


        void putPixelsFromImageinYBW(ref Bitmap bmp, int[,] Y)
        {
            int x = 0;
            int y = 0;

            //         Color p;

            byte[,] r = new byte[Y.GetLength(0), Y.GetLength(1)];
            byte[,] g = new byte[Y.GetLength(0), Y.GetLength(1)];
            byte[,] b = new byte[Y.GetLength(0), Y.GetLength(1)];

            for (y = 0; y < Y.GetLength(1); y++)
            {
                for (x = 0; x < Y.GetLength(0); x++)
                {

                    r[x, y] = (byte)Y[x, y];
                    b[x, y] = (byte)Y[x, y];
                    g[x, y] = (byte)Y[x, y];

                    //               p = Color.FromArgb(r, b, g);
                    //             bmp.SetPixel(x, y, p);
                }
            }

            putPixels(bmp, r, g, b);


        }// end putPixelsFromImageinY


        void getPixelsFromImageInY(Bitmap bmp, ref int[,] Y)
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
                    // if (b + 2 >= bytes) break;
                }
                while (b % 4 != 0)
                    b++;
            }


            // Unlock the bits.
            bmp.UnlockBits(bmpData);

        }



        void putPixels(Bitmap bmp, byte[,] r, byte[,] g, byte[,] b)
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


            int x = 0;
            int y = 0;
            int bi = 0;

            for (y = 0; y < r.GetLength(1); y++)
            {
                for (x = 0; x < r.GetLength(0); x++)
                {
                    // some guy on code project says the values are in B G R order
                    rgbValues[bi] = (byte)b[x, y];
                    rgbValues[bi + 1] = (byte)g[x, y];
                    rgbValues[bi + 2] = (byte)r[x, y];
                    rgbValues[bi + 3] = 255; // alpha
                    bi += pixelOffset;
                }
                while (bi % 4 != 0) bi++;
            }

            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);

            // Unlock the bits.
            bmp.UnlockBits(bmpData);

        }


    }
}
