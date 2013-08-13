using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace PixelLib
{
    public class Pixels
    {

        public static void putPixels(Bitmap bmp, byte[,] r, byte[,] g, byte[,] b)
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

        public static void putPixels(Bitmap bmp, int[,] lum)
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

            for (y = 0; y < lum.GetLength(1); y++)
            {
                for (x = 0; x < lum.GetLength(0); x++)
                {
                    // some guy on code project says the values are in B G R order
                    rgbValues[bi] = (byte)lum[x, y];
                    rgbValues[bi + 1] = (byte)lum[x, y];
                    rgbValues[bi + 2] = (byte)lum[x, y];
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


        public static void getPixelsFromImage(Bitmap bmp, int[,] Y)
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
