using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices; //For DLL support
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;


namespace LPROCR_Wrapper
{
    public class LPROCR_Lib
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct LPROCR_lib_PLATE_LOCATION
        {
            public int leftEdge;
            public int rightEdge;
            public int topEdge;
            public int bottomEdge;
            public int centerX;
            public int centerY;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct LPR_PROCESS_OPTIONS
        {
            public int EnableRotationRoll;
            public int EnableAutoRotationRoll;
            public float roll;
            public float rotation;
            public int quickPlateTest;
        };


        [DllImport("LPROCR_lib_c.dll")]
        unsafe static extern void LPROCR_lib_Constructor( );

        public LPROCR_Lib()
        {

            //Assembly caller = Assembly.GetCallingAssembly();
            //InitOptionalInfo(caller);

            LPROCR_lib_Constructor();

        }

        //public delegate void MOTION_DETECTED ();
        //MOTION_DETECTED m_MotionDetCallBack;
        //[DllImport("LPROCR_lib_c.dll")]
        //unsafe static extern void LPROCR_lib_RegisterMotionDetectionCB(IntPtr  callback);

        //unsafe public void RegisterMotionDetectCallBack ( MOTION_DETECTED dcb)
        //{
        //    m_MotionDetCallBack = dcb;

        //    unsafe
        //    {
        //        IntPtr callbackPtr = Marshal.GetFunctionPointerForDelegate(m_MotionDetCallBack);
        //        {
        //            LPROCR_lib_RegisterMotionDetectionCB(callbackPtr);
               
        //        }
        //    }
        //}

        private void InitOptionalInfo(Assembly caller)
        {
            string c=null;
           

            Object[] config = caller.GetCustomAttributes(true);
            if (config == null) return;

            //index = 5 = configuration attribute

            // FieldInfo[] fields = obj.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

            for (int i = 0; i < config.Length; i++)
            {

                FieldInfo[] fields = config[i].GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                if (fields == null) return;

                foreach (FieldInfo fi in fields)
                {
          //          if (fi.Name.Contains("m_configuration"))
                    if (fi.Name.Contains("m_LPconfiguration"))
                    {
                        c = (string ) fi.GetValue(config[i]);
                        break;
                    }
                }
            }

            configInfo = c;

         
        }

        private string configInfo;
       
   



        [DllImport("LPROCR_lib_c.dll")]
        unsafe static extern void LPROCR_lib_init( int * image, int width, int height, int diagEnabled, ref int error);
        unsafe static int* ImagePtr;// keep the fixed pointer to the image around indefinetly, out of the hands of the GC
        unsafe public void init(int[,] image, int width, int height, int diagEnabled, ref int error)
        {

            ImagePtr = (int*)Marshal.UnsafeAddrOfPinnedArrayElement(image, 0);

            LPROCR_lib_init(ImagePtr, width, height, diagEnabled, ref error);
        }




        [DllImport("LPROCR_lib_c.dll")]
        static extern void LPROCR_lib_dispose();

        public void dispose( )
        {
        
            LPROCR_lib_dispose( );
        }




        [DllImport("LPROCR_lib_c.dll")]
        unsafe static extern void LPROCR_lib_ExtractFoundPlates(ref int error);

        [DllImport("LPROCR_lib_c.dll")]
        static extern int LPROCR_lib_findThePlates(  ref int error);

        public int findThePlates( ref int error)
        {
            int retVal = LPROCR_lib_findThePlates(  ref  error);

            if ( error == 0 )LPROCR_lib_ExtractFoundPlates(ref  error); // prepare memory structures for the next step of finding chars in the plate

            return (retVal);
      
        }


       //
       //   getPlateLocation

        [DllImport("LPROCR_lib_c.dll")]
        static extern void LPROCR_lib_getPlateLocation( int plateIndex, IntPtr pLocation);

        public void getPlateLocation(int plateIndex, ref LPROCR_lib_PLATE_LOCATION pLocation)
        {

           // Initialize unmanged memory to hold the struct.
           IntPtr pl = Marshal.AllocHGlobal(Marshal.SizeOf(pLocation));

           try
           {

              // Copy the struct to unmanaged memory.
              Marshal.StructureToPtr(pLocation, pl, false);

              LPROCR_lib_getPlateLocation( plateIndex, pl);


              // copy back from unmanaged memory to managed memory
              //
              pLocation =(LPROCR_lib_PLATE_LOCATION) Marshal.PtrToStructure(pl, typeof(LPROCR_lib_PLATE_LOCATION));


           }
           finally
           {
              // Free the unmanaged memory.
              Marshal.FreeHGlobal(pl);
           }




        }






        [DllImport("LPROCR_lib_c.dll")]
        static extern int LPROCR_lib_sharpen(int[,] image, int w, int h);

        public int sharpen(int[,] image, int w, int h)
        {
            return (LPROCR_lib_sharpen(image,  w, h));
        }



        [DllImport("LPROCR_lib_c.dll")]
        unsafe static extern int LPROCR_lib_extractFromBmpDataToLumArray(int* srcPtr, int * dstPtr, 
            int stride, int width, int height, bool invert);

        public static int extractFromBmpDataToLumArray(IntPtr srcPtr, int[,] dstPtr, int stride, int width, int height, bool invert)
        {
            unsafe
            {
            int* iDstPtr = (int*)Marshal.UnsafeAddrOfPinnedArrayElement (dstPtr,0);

            return (LPROCR_lib_extractFromBmpDataToLumArray((int*)srcPtr, iDstPtr, stride, width, height, invert));
            }
        }









        [DllImport("LPROCR_lib_c.dll")]
        static extern void LPROCR_lib_getImage( int[,] Y, // place to put the image
                                    ref int error);


        public void getImage( int[,] Y, ref int error)
        {
            LPROCR_lib_getImage(   Y, ref error);
        }


        [DllImport("LPROCR_lib_c.dll")]
        static extern void LPROCR_lib_getEdgeMapSub( int[,] Y, // place to put the image
                                  ref int error) ;
        
        public void getEdgeMapSub(int[,] Y, ref int error)
        {
            LPROCR_lib_getEdgeMapSub(   Y, ref error);
        }




        [DllImport("LPROCR_lib_c.dll")]
        static extern void LPROCR_lib_getEdgeMapFullRes( int[,] Y, // place to put the image
                                  ref int error) ;
        
        public void getEdgeMapFullRes( int[,] Y, ref int error)
        {
            LPROCR_lib_getEdgeMapFullRes(  Y, ref error);
        }


        //void _stdcall LPROCR_diags_getIntegralImage(  char * red, char * green, char * blue)
        [DllImport("LPROCR_lib_c.dll")]
        static extern void LPROCR_diags_getIntegralImage( Byte[,] red, Byte[,] green, Byte[,] blue);

        public void GetIntegralImage( Byte[,] red, Byte[,] green, Byte[,] blue)
        {
            LPROCR_diags_getIntegralImage( red, green, blue );
        }



        [DllImport("LPROCR_lib_c.dll")]
        static extern void LPROCR_diags_getImage( int fullSub, Byte[,] red, Byte[,] green, Byte[,] blue, ref int error);

        public void diagsGetImage( int fullSub, Byte [,] red, Byte [,] green, Byte [,] blue, ref int error)
        {
            LPROCR_diags_getImage(  fullSub, red,  green,  blue, ref error);
        }


        [DllImport("LPROCR_lib_c.dll")]
        static extern int LPROCR_lib_getMaxCandidatePlates(  );

        public int getMaxCandidatePlates( )
        {
            return (LPROCR_lib_getMaxCandidatePlates( ));
        }


        [DllImport("LPROCR_lib_c.dll")]
        static extern int LPROCR_lib_getMaxActualPlates(  );

        public int getMaxActualPlates( )
        {
            return (LPROCR_lib_getMaxActualPlates( ));
        }

        [DllImport("LPROCR_lib_c.dll")]
        unsafe static extern int LPROCR_lib_scoreMatch(int  * searchStr,  // string user enters to search for
                int sStrLen,
                int * refStr, // string in database to compare to
                int rStrLen);

        public static int scoreMatch( string searchString, string referenceString )
        {
            unsafe
            {
                int * searchStringBytes;
                int * referenceStringBytes;

                searchStringBytes = (int *)Marshal.StringToHGlobalAnsi(searchString);

                referenceStringBytes = (int*)Marshal.StringToHGlobalAnsi(referenceString);

                int ret = LPROCR_lib_scoreMatch(searchStringBytes, searchString.Length, referenceStringBytes, referenceString.Length);
                              

                Marshal.FreeHGlobal((IntPtr)searchStringBytes);
                Marshal.FreeHGlobal((IntPtr)referenceStringBytes);

                return (ret);
            }
        }


        
        [DllImport("LPROCR_lib_c.dll")]
        unsafe static extern void LPROCR_lib_unroll(int* image, int w, int h, float slope, ref int error, int fillValue);
        public static void Roll(int[,] image, float slopeDegrees, ref int error)
        {
          

            unsafe
            {
                int * imagePtr  =(int*) Marshal.UnsafeAddrOfPinnedArrayElement(image, 0);

                LPROCR_lib_unroll(imagePtr, image.GetLength(0), image.GetLength(1), slopeDegrees, ref error, 255);
         
            }
        }

            // char isolation methods
        [DllImport("LPROCR_lib_c.dll")]
        unsafe static extern void LPROCR_lib_RotateImage(int* image, int w, int h, float slope, ref int error);
        public static void Rotate(int[,] image, float slopeRadians, ref int error)
        {
           

            unsafe
            {
                int * imagePtr  =(int*) Marshal.UnsafeAddrOfPinnedArrayElement(image, 0);

                LPROCR_lib_RotateImage(imagePtr, image.GetLength(0), image.GetLength(1), slopeRadians, ref error);
         
            }
        }


        [DllImport("LPROCR_lib_c.dll")]
        unsafe static extern int LPROCR_lib_findCharsInPlate(int plateIndex, ref int error, bool diagsEnabled);


        public int findCharsInPlate(int plateIndex,  ref int error, bool diagEnabled)
        {
            int retVal = 0;
           

            unsafe
            {


                retVal = LPROCR_lib_findCharsInPlate(plateIndex, ref error, diagEnabled);
                
      
            }


           return (retVal);
        }



        [DllImport("LPROCR_lib_c.dll")]
        unsafe static extern int LPROCR_diags_GetNumCandidatePlates();

        public int GetNumCandidatePlates()
        {
            unsafe
            {
                return (LPROCR_diags_GetNumCandidatePlates());
            }
        }


        [DllImport("LPROCR_lib_c.dll")]
        unsafe static extern int LPROCR_diags_getCandidatePlateImage(int plateIndex, char* red, char* green, char* blue, ref int error);

        /// <summary>
        /// returns 1 for sucess, 0 if no candidate plate image exists at plateIndex
        /// </summary>
        /// <param name="plateIndex"></param>
        /// <param name="red"></param>
        /// <param name="green"></param>
        /// <param name="blue"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public int GetDiagCandidatePlateImage(int plateIndex, byte[,] red, byte[,] green, byte[,] blue, ref int error)
        {
            unsafe
            {

                char* redPtr = (char*)Marshal.UnsafeAddrOfPinnedArrayElement(red, 0);
                char* greenPtr = (char*)Marshal.UnsafeAddrOfPinnedArrayElement(green, 0);
                char* bluePtr = (char*)Marshal.UnsafeAddrOfPinnedArrayElement(blue, 0);

                return (LPROCR_diags_getCandidatePlateImage(plateIndex, redPtr, greenPtr, bluePtr, ref error));
            }
        }
     
        [DllImport("LPROCR_lib_c.dll")]
        unsafe static extern void LPROCR_diags_GetPlateDiagImageRGBArray(int plateIndex, int width, int height, byte * rgbValues, byte * red, byte * green, byte * blue, int pixelOffset);

        [DllImport("LPROCR_lib_c.dll")]
        unsafe static extern void LPROCR_lib_GetCandidatePlateImageSize ( int plateIndex, ref int width, ref int height );

        [DllImport("LPROCR_lib_c.dll")]
        unsafe static extern void LPROCR_diags_getPlateImage(int plateIndex, byte * red, byte * green, byte * blue, ref int error, int useCandidatePlateList);

        public void GetDiagPlateImage(int plateIndex, byte[,] red, byte[,] green, byte[,] blue, ref int error)
        {
            unsafe
            {

                byte* redPtr = (byte*)Marshal.UnsafeAddrOfPinnedArrayElement(red, 0);
                byte* greenPtr = (byte*)Marshal.UnsafeAddrOfPinnedArrayElement(green, 0);
                byte* bluePtr = (byte*)Marshal.UnsafeAddrOfPinnedArrayElement(blue, 0);

                LPROCR_diags_getPlateImage(plateIndex, redPtr, greenPtr, bluePtr, ref error, 1);
            }
        }


        public void GetDiagPlateImage(int plateIndex,  out Bitmap bmp)
        {
            bmp = null;
            int error = 0;

            int width = 0, height= 0;
            LPROCR_lib_GetCandidatePlateImageSize ( plateIndex, ref width, ref height );

            byte[,] red = new byte [width, height];
            byte[,] green = new byte [width, height];
            byte[,] blue = new byte [width, height];

            bmp = new Bitmap(width, height);

            unsafe
            {
                GCHandle redHandle = GCHandle.Alloc(red, GCHandleType.Pinned);
                GCHandle greenHandle = GCHandle.Alloc(green, GCHandleType.Pinned);
                GCHandle blueHandle = GCHandle.Alloc(blue, GCHandleType.Pinned);

                byte* redPtr = (byte*)Marshal.UnsafeAddrOfPinnedArrayElement(red, 0);
                byte* greenPtr = (byte*)Marshal.UnsafeAddrOfPinnedArrayElement(green, 0);
                byte* bluePtr = (byte*)Marshal.UnsafeAddrOfPinnedArrayElement(blue, 0);
                               
                LPROCR_diags_getPlateImage(plateIndex, redPtr, greenPtr, bluePtr, ref error, 1);

                // Lock the bitmap's bits.  
                Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                System.Drawing.Imaging.BitmapData bmpData =
                    bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                    bmp.PixelFormat);

                // Get the address of the first line.
                int* iptr = (int*)bmpData.Scan0;

                // Declare an array to hold the bytes of the bitmap.
                int bytes = bmpData.Stride * bmp.Height;

                int pixelOffset = bmpData.Stride / bmp.Width;


                LPROCR_diags_GetPlateDiagImageRGBArray(plateIndex, width, height,(byte*) iptr, redPtr, greenPtr, bluePtr, pixelOffset);
              


                // Unlock the bits.
                bmp.UnlockBits(bmpData);

                redHandle.Free();
                greenHandle.Free();
                blueHandle.Free();
               
            }

        }


 
        [DllImport("LPROCR_lib_c.dll")]
        static extern int LPROCR_lib_getMaxCandidateChars( );

        public int getMaxCandidateChars()
        {
            return (LPROCR_lib_getMaxCandidateChars());
        }


        [DllImport("LPROCR_lib_c.dll")]
        static extern int LPROCR_lib_getAveCharWidth(int plateIndex, int useCandidateList);

        public int GetAveCharWidth(int plateIndex, bool UseCandidatePlates )
        {
            int useCandidatePlates = (UseCandidatePlates) ? 1 : 0;
            return (LPROCR_lib_getAveCharWidth(plateIndex, useCandidatePlates));
        }


        [DllImport("LPROCR_lib_c.dll")]
        static extern void LPROCR_lib_GetMinMaxPlateSize(  ref int minWidth, ref int maxWidth, ref int minHeight, ref int maxHeight);

        public void GetMinMaxPlateSize(ref int minWidth, ref int maxWidth, ref int minHeight, ref int maxHeight)
        {
            LPROCR_lib_GetMinMaxPlateSize(  ref  minWidth, ref  maxWidth, ref minHeight, ref  maxHeight);
        }



        [DllImport("LPROCR_lib_c.dll")]
        static extern int LPROCR_lib_getCharLocation(int plateIndex, int cIndex, ref int left, ref int right, ref int top, ref int bottom, int useCandidatePlateList);

        public int GetCharLocation(int plateIndex, int cIndex, ref int left, ref int right, ref int top, ref int bottom, bool useCandidateList)
        {
            int useCandidatePlateList = (useCandidateList) ? 1 : 0;

            return (LPROCR_lib_getCharLocation(plateIndex, cIndex, ref left, ref  right, ref top, ref bottom, useCandidatePlateList));
        }


        [DllImport("LPROCR_lib_c.dll")]
        unsafe static extern void LPROCR_diags_PutCharImageToRGBArray(byte * imageSrc, int width, int height, byte * rgbValues, int pixelOffset);
        
        public void PutCharImageToBitmap(int[,] cImage,  out Bitmap bmp)
        {
           
            int w = cImage.GetLength(0);
            int h = cImage.GetLength(1);

            bmp = new Bitmap (w,h);

            unsafe
            {
                // Lock the bitmap's bits.  
                Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                System.Drawing.Imaging.BitmapData bmpData =
                    bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                    bmp.PixelFormat);

                // Get the address of the first line.
                int* iptr = (int*)bmpData.Scan0;

                // Declare an array to hold the bytes of the bitmap.
                int bytes = bmpData.Stride * bmp.Height;

                int pixelOffset = bmpData.Stride / bmp.Width;
                fixed (int * inPtr = cImage)
                {
                    LPROCR_diags_PutCharImageToRGBArray((byte*) inPtr, w,  h, (byte *) iptr, pixelOffset);
                }
            }
           
        }

        [DllImport("LPROCR_lib_c.dll")]
        unsafe static extern char LPROCR_lib_ReadThisChar(int* cImageRaw, int w, int h, float* score, int aveCharWidth, int* displayChar, int* error);

        public char ReadThisChar(int[,] cImageRaw,  ref float score,  int aveCharWidth, int[,] displayChar)
        {
            char retval;
            int error = 0;
            int w = cImageRaw.GetLength(0);
            int h = cImageRaw.GetLength(1);

            unsafe
            {
                fixed (int* inPtr = cImageRaw)
                fixed (int* dPtr = displayChar)
                fixed (float* scorePtr = &score)
                {
                    retval = LPROCR_lib_ReadThisChar(inPtr, w, h, scorePtr, aveCharWidth, dPtr, & error);
                }
            }
            return (retval);
        }


        [DllImport("LPROCR_lib_c.dll")]
        static extern void  scaleArray(int[,] inArray, int w, int h, int[,] outArray, int newWidth, int newHeight, ref int error);

        public void ScaleArray(int[,] inArray, int w, int h, int[,] outArray, int newWidth, int newHeight, ref int error)
        {
            scaleArray( inArray, w,  h,  outArray,  newWidth,  newHeight, ref error);
      
        }

                          
        [DllImport("LPROCR_lib_c.dll")]
        static extern int LPROCR_lib_OCR_getFilterName(int m_FilterNumber);

        public int  OCR_getFilterName(int m_FilterNumber)
        {
            return ( LPROCR_lib_OCR_getFilterName(m_FilterNumber));
      
        }

        [DllImport("LPROCR_lib_c.dll")]
        static extern int LPROCR_lib_OCR_getNumberFilters();

        public int  OCR_getNumberFilters()
        {
            return ( LPROCR_lib_OCR_getNumberFilters());
      
        }


        
        [DllImport("LPROCR_lib_c.dll")]
        static extern float structualCorrelation (int [,] src,  int filter,int [,] diagImage);

        public float  OCR_structualCorrelation(int [,] src,  int filter,int [,] diagImage)
        {
            return (  structualCorrelation ( src,  filter, diagImage));
      
        }


        [DllImport("LPROCR_lib_c.dll")]
        static extern void  prepChar(int[,]cImageRaw, int w,int  h, int [,]cImageSS );

        public void OCR_prepChar(int[,] cImageRaw, int w, int h, int[,] cImageSS)
        {
            prepChar(cImageRaw, w, h, cImageSS);
      
        }


        [DllImport("LPROCR_lib_c.dll")]
        static extern void  erode(int[,]image, int w,int  h, int[,]eroded);

        public void OCR_erode(int[,] cImageRaw, int[,] eroded)
        {
            erode(cImageRaw,cImageRaw.GetLength(0),cImageRaw.GetLength(1), eroded );
      
        }


        [DllImport("LPROCR_lib_c.dll")]
        unsafe static extern void LPROCR_lib_LoadImage(int* img, int width, int height, int diagEnabled, LPR_PROCESS_OPTIONS* processOptions, ref int error);

        public void LoadImage(int[,] img, bool diagEnabled, ref LPR_PROCESS_OPTIONS processOptions, ref int error)
        {
            
            int width = img.GetLength(0);
            int height = img.GetLength(1);
            int diag = (diagEnabled) ? 1 : 0;
            unsafe
            {
                fixed (int* ptr = img)
                fixed (LPR_PROCESS_OPTIONS* pOptsPtr = &processOptions)
                {
                    LPROCR_lib_LoadImage(ptr, width, height, diag, pOptsPtr, ref error);
                }
            }

            return;
        }


        [DllImport("LPROCR_lib_c.dll")]
        unsafe static extern int LPROCR_lib_ReadThisImage(int* img, int width, int height, int diagEnabled, LPR_PROCESS_OPTIONS * processOptions, ref int error);
    

        public int ReadThisImage ( int [,] img,  int diagEnabled, ref LPR_PROCESS_OPTIONS  processOptions, ref int error )
        {        
            int n=0;
            int width = img.GetLength(0);
            int height = img.GetLength(1);

            unsafe
            {
                fixed (int* ptr = img) 
                fixed (LPR_PROCESS_OPTIONS * pOptsPtr = & processOptions) 
                {
                    n = LPROCR_lib_ReadThisImage(ptr, width, height, diagEnabled,  pOptsPtr, ref error);
                }
            }
       
            return(n);
        }



        [DllImport("LPROCR_lib_c.dll")]
        static extern int LPROCR_lib_GetNumFoundPlates( );

        public int GetNumFoundPlates( )
        {
            return LPROCR_lib_GetNumFoundPlates();
        }

        [DllImport("LPROCR_lib_c.dll")]
        unsafe static extern void LPROCR_lib_GetPlateImage(int plateIndex, int* img, int candidatePlateList);

        public void GetPlateImage(int plateIndex, int[,] img, bool candidatePlateList)
        {
            int useCandidatePlateList = (candidatePlateList == true) ? 1 : 0;

            unsafe
            {
                int* imgPtr =(int*) Marshal.UnsafeAddrOfPinnedArrayElement(img, 0);
                LPROCR_lib_GetPlateImage(plateIndex, imgPtr, useCandidatePlateList);
            }
        }



        [DllImport("LPROCR_lib_c.dll")]
        static extern void LPROCR_lib_GetPlateImageSize(int plateIndex, ref int width, ref int height, int candidatePlateList);

        public void GetPlateImageSize(int plateIndex, ref int width, ref int height, bool candidatePlateList)
        {
            int useCandidatePlateList = (candidatePlateList == true ) ? 1 : 0;

            LPROCR_lib_GetPlateImageSize(plateIndex, ref  width, ref  height, useCandidatePlateList);
        }


       

        public void GetCandidatePlateImageSize(int plateIndex, ref int width, ref int height)
        {
            LPROCR_lib_GetCandidatePlateImageSize(plateIndex, ref  width, ref  height);
        }


        [DllImport("LPROCR_lib_c.dll")]
        static extern void LPROCR_lib_GetSubImageSize( ref int width, ref int height);

        public void GetSubImageSize( ref int width, ref int height)
        {
            LPROCR_lib_GetSubImageSize( ref width, ref height);
        }



        [DllImport("LPROCR_lib_c.dll")]
        unsafe static extern void LPROCR_diags_GetRejectLog(byte * logString);
        [DllImport("LPROCR_lib_c.dll")]
        unsafe static extern int LPROCR_diags_GetRejectLogLength( );

        public string GetRejectLog( )
        {
           
            unsafe
            {
                // allocate managed memory to hold the string.

                int count = LPROCR_diags_GetRejectLogLength();

                byte[] log = new byte[count];

                fixed (byte* lsPtr = & log[0])                
                {
                    LPROCR_diags_GetRejectLog( lsPtr );

                 

                    string outStr = new string((sbyte *)lsPtr, 0, count);

                    return (outStr);
                }

            }
        }


        [DllImport("LPROCR_lib_c.dll")]
        unsafe static extern void LPROCR_lib_GetPlateString(int plateIndex, char* plateString, float* score);

        public string GetPlateString(int plateIndex, ref float score)
        {
            // there must be a better way, but allocating memory space for the string in managed code, so that I do not end up with unrelease memory in the unmanaged code

            string plateString = "   the max plate string is 10 chars, this will be more than enough room    "; 
            unsafe
            {
                fixed (float* scorePtr = &score)
                {

                    char* psPtr = (char*)Marshal.StringToHGlobalAnsi(plateString);

                    LPROCR_lib_GetPlateString(plateIndex, psPtr, scorePtr);

                    string outStr = Marshal.PtrToStringAnsi((IntPtr)psPtr);

                    Marshal.FreeHGlobal((IntPtr)psPtr);

                    return (outStr);
                }

            }
        }

       



        [DllImport("LPROCR_lib_c.dll")]
        unsafe static extern void LPROCR_lib_GetCharImageLuminance(int plateIndex, int cIndex, int* lumArray,int useCandidateList);

        public void GetCharImage(int plateIndex, int charIndex, out int[,] luminance, bool useCandidatePlateList)
        {
            int left = 0, right = 0, top = 0, bottom = 0;

            int useCandidateList = ( useCandidatePlateList) ? 1 :  0;

            LPROCR_lib_getCharLocation(plateIndex, charIndex, ref left, ref  right, ref top, ref bottom, useCandidateList);

            int cw = right - left;
            int ch = bottom - top;

            luminance = new int[cw, ch];

            unsafe
            {
                fixed (int * lumArray = luminance)
                {
                    LPROCR_lib_GetCharImageLuminance(plateIndex, charIndex, lumArray,  useCandidateList);
                }
            }
        }


        [DllImport("LPROCR_lib_c.dll")]
        unsafe static extern void LPROCR_lib_GetCharImageRGBArray(int plateIndex, int cIndex, int * rgbValues, int width, int height, int rgbLength, int pixelOffset);

        public void GetCharImage(int plateIndex, int charIndex, out Bitmap bmp, bool useCandidatePlateList)
        {
            bmp = null; 

            int left =0, right = 0, top = 0, bottom= 0;
            
            int useCandidateList = (useCandidatePlateList) ? 1 : 0;

            LPROCR_lib_getCharLocation(plateIndex, charIndex, ref left, ref  right, ref top, ref bottom, useCandidateList);

            int cw = right - left;
            int ch = bottom - top;

            if (cw <= 0 || ch <= 0) return;

            bmp = new Bitmap(cw, ch);

            unsafe
            {

                // Lock the bitmap's bits.  
                Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                System.Drawing.Imaging.BitmapData bmpData =
                    bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                    bmp.PixelFormat);

                // Get the address of the first line.
                int* iptr = (int*)bmpData.Scan0;

                // Declare an array to hold the bytes of the bitmap.
                int bytes = bmpData.Stride * bmp.Height;
                byte[] rgbValues = new byte[bytes];

                int pixelOffset = bmpData.Stride / bmp.Width;

               

                //(int cIndex, byte* rgbValues, int width, int height, int rgbLength, int pixelOffset);
                LPROCR_lib_GetCharImageRGBArray(plateIndex, charIndex, iptr, cw, ch, bytes, pixelOffset);


                // Unlock the bits.
                bmp.UnlockBits(bmpData);
            }

        }



        [DllImport("LPROCR_lib_c.dll")]
        unsafe static extern void LPROCR_diags_GetCharImageRGBArray(int plateIndex, int cIndex, int* rgbValues,  int pixelOffset);

        public void GetDiagCharImage(int plateIndex, int charIndex, out Bitmap bmp)
        {
            bmp = null;

         

            int cw = 20;// standard width
            int ch = 40; // standard height


            bmp = new Bitmap(cw, ch,System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            unsafe
            {

                // Lock the bitmap's bits.  
                Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                System.Drawing.Imaging.BitmapData bmpData =
                    bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                    bmp.PixelFormat);

                // Get the address of the first line.
                int* iptr = (int*)bmpData.Scan0;

                // Declare an array to hold the bytes of the bitmap.
                int bytes = bmpData.Stride * bmp.Height;
               
                int pixelOffset = bmpData.Stride / bmp.Width;

                LPROCR_diags_GetCharImageRGBArray(plateIndex, charIndex, iptr,   pixelOffset);


                // Unlock the bits.
                bmp.UnlockBits(bmpData);
            }

        }




        [DllImport("LPROCR_lib_c.dll")]
        unsafe static extern void LPROCR_lib_GetCandidatePlateImageRGBArray(int plateIndex, int* rgbValues, int width, int height, int rgbLength, int pixelOffset);

        public void GetCandidatePlateImage(int plateIndex, out  Bitmap bmp)
        {

            int width = 0, height = 0;
            LPROCR_lib_GetCandidatePlateImageSize(plateIndex, ref width, ref height);

            bmp = new Bitmap(width,height);

            unsafe
            {

                // Lock the bitmap's bits.  
                Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                System.Drawing.Imaging.BitmapData bmpData =
                    bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                    bmp.PixelFormat);

                // Get the address of the first line.
                int* iptr = (int*)bmpData.Scan0;

                // Declare an array to hold the bytes of the bitmap.
                int bytes = bmpData.Stride * bmp.Height;
                byte[] rgbValues = new byte[bytes];

                int pixelOffset = bmpData.Stride / bmp.Width;

                int w = bmp.Width;
                int h = bmp.Height;

                LPROCR_lib_GetCandidatePlateImageRGBArray(plateIndex, iptr, w, h, bytes, pixelOffset);


                // Unlock the bits.
                bmp.UnlockBits(bmpData);
            }

        }
        [DllImport("LPROCR_lib_c.dll")]
        unsafe static extern void LPROCR_lib_GetPlateImageRGBArray(int plateIndex, int * rgbValues, int width, int height, int rgbLength, int pixelOffset);

        public void GetPlateImage(int plateIndex, Bitmap bmp)
        {


            unsafe
            {

                // Lock the bitmap's bits.  
                Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                System.Drawing.Imaging.BitmapData bmpData =
                    bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                    bmp.PixelFormat);

                // Get the address of the first line.
                int* iptr = (int*)bmpData.Scan0;

                // Declare an array to hold the bytes of the bitmap.
                int bytes = bmpData.Stride * bmp.Height;
                byte[] rgbValues = new byte[bytes];

                int pixelOffset = bmpData.Stride / bmp.Width;

                int w = bmp.Width;
                int h = bmp.Height;

                LPROCR_lib_GetPlateImageRGBArray(plateIndex, iptr, w, h, bytes, pixelOffset);
               

                // Unlock the bits.
                bmp.UnlockBits(bmpData);
            }

        }





        [DllImport("LPROCR_lib_c.dll")]
        unsafe static extern void LPROCR_lib_MemCopy(int * src, int * dst, int stride, int width, int height );

       
        public static void PutBmpBufferIntoBmp(IntPtr buff, Bitmap bmp)
        {


            unsafe
            {

                // Lock the bitmap's bits.  
                Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
                System.Drawing.Imaging.BitmapData bmpData =
                    bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                    bmp.PixelFormat);

                // Get the address of the first line.
                int * dstP =(int *) bmpData.Scan0;

                // Declare an array to hold the bytes of the bitmap.
                int bytes = bmpData.Stride * bmp.Height;
             //   byte[] rgbValues = new byte[bytes];

                int pixelOffset = bmpData.Stride / bmp.Width;

                int w = bmp.Width;
                int h = bmp.Height;

                LPROCR_lib_MemCopy((int*)buff, (int*)dstP, bmpData.Stride, bmp.Width, bmp.Height);
                

                // Unlock the bits.
                bmp.UnlockBits(bmpData);
            }

        }

        [DllImport("LPROCR_lib_c.dll")]
        unsafe static extern void  LPROCR_lib_MemCopyInt(void * src, void * dst, int len );
        
        unsafe public static void MemCopyInt(int * src, IntPtr dst, int len)
        {


            unsafe
            {

                LPROCR_lib_MemCopyInt(src, dst.ToPointer(), len);
              
            }
        }


        [DllImport("LPROCR_lib_c.dll")]
        unsafe static extern void LPROCR_lib_MemCopyBytesToInts(int * bmpScan0Src, int* dst, int len, int width, int height);

        unsafe public static void MemCopyByteArrayToIntArray(IntPtr bmpScan0Src, IntPtr dst, int len, int width, int height)
        {
            unsafe
            {
                int * arrayPtr = (int*)bmpScan0Src.ToPointer();
                int* dstPtr = (int*)dst.ToPointer();
                {
                    LPROCR_lib_MemCopyBytesToInts(arrayPtr, dstPtr, len, width, height);
                }

            }
        }
       
         [DllImport("LPROCR_lib_c.dll")]
        unsafe static extern void LPROCR_lib_MemCopyByte(void* src, void* dst, int len, int* detectedNoVideo);
        
        unsafe public static void MemCopyByte(int * src, IntPtr dst, int len, ref bool detectedNoVideo)
        {
            

            unsafe
            {
                int detectedVideoNot;
                int* detectedVideoNotPtr =& detectedVideoNot;

                LPROCR_lib_MemCopyByte(src, dst.ToPointer(), len, detectedVideoNotPtr);
                
                if (detectedVideoNot == 1) detectedNoVideo = true;
                else detectedNoVideo = false;
              
            }

        }

        [DllImport("LPROCR_lib_c.dll")]
        unsafe static extern int LPROCR_lib_DetectMotion(int chan, int * fullscale, int w, int h, ref int error);
        unsafe static int* mdImagePtr;
        unsafe static public bool DetectMotion(int chan, int[,] image, int width, int height, ref int error)
        {

            mdImagePtr = (int*)Marshal.UnsafeAddrOfPinnedArrayElement(image, 0);
            if (LPROCR_lib_DetectMotion(chan, mdImagePtr, width, height, ref error) == 1) return true;
            else return false;

        }

        [DllImport("LPROCR_lib_c.dll")]
        unsafe static extern void Groups_ProcessNewImage(int chan, char* currentReading, int charCount, int serialNumber);
   
        public void PlateGroups_ProcessNewImage(int chan, string currentReading, int charCount, int serialNumber)
        {
            unsafe
            {
                char* ps1Ptr = (char*)Marshal.StringToHGlobalAnsi(currentReading);
              
                Groups_ProcessNewImage(chan, ps1Ptr, charCount, serialNumber);
             
                Marshal.FreeHGlobal((IntPtr)ps1Ptr);
          
            }

        }


        unsafe public delegate void PLATE_GROUP_READY(int chan, char* contatonatedPlateStrings, int serialNumber, int len);
        PLATE_GROUP_READY m_PlateGroupCallBack;
        [DllImport("LPROCR_lib_c.dll")]
        unsafe static extern void LPROCR_lib_RegisterPlateGroupCB(IntPtr callback);

        unsafe public void RegisterPlateGroupCB(PLATE_GROUP_READY dcb)
        {
            m_PlateGroupCallBack = dcb;

            unsafe
            {
                IntPtr callbackPtr = Marshal.GetFunctionPointerForDelegate(m_PlateGroupCallBack);
                {
                    LPROCR_lib_RegisterPlateGroupCB(callbackPtr);
                }
            }
        }

    }
    
}
