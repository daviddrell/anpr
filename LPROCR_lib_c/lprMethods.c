

#include <afx.h>
#include <stdlib.h>
#include <malloc.h>
#define DECLARE_ERROR_ARRAY
#include "LPROCR_Public.h"
#include "LPROCR_Structs.h"
#include "LPROCR_Error_Codes.h"
#include "LPROCR_Diags.h"
#include "lprMethods.h"
#include "LPROCR_Diags.h"
#include "ocr.h"
#include "charIsolationMethods.h"


    //   change long

// line 1860, 2 Sep 2009: changed the edge energy threshold to 100 from 400 for a small plate to pick up:  if (*(Vedge+x) > 100)// was 400



int m_FrameCount=0;
extern void Groups_Init();//  defined in plateGroupings.c


//  called once by the C# lib wrapper constructor

void LPROCR_lib_Constructor ( )
{
    int p;
    int c = 0;

    libData.imageFullRes =0;    
    libData.imageFullResDiag =0; 
    libData.imageSub =0; 
    libData.imageSubDiag =0; 
    libData.imageSubPrev = 0;
    libData.edgeMapSub =0; 
    libData.edgeMapFullRes =0; 
    libData.binarizedImageBlobed =0;
    libData.binarizedImageClean =0;
    libData.MotionDetectedCallBack = 0;

    for ( p =0; p <  MAX_CANDIDATE_PLATES ; p++)
    {
        libData.foundPlates[p].image = 0;
        libData.foundPlates[p].diagImage = 0;
        for ( c = 0; c < MAX_CANDIDATE_CHARS; c++)
        {
            libData.foundPlates[ p].diagCharStdSize [c] = 0;
        }  
    }

    for (p = 0; p < MAX_ACTUAL_PLATES ; p++)
    {
        libData.finalPlateList[p].image = 0;
        libData.finalPlateList[p].diagImage = 0;
        for ( c = 0; c < MAX_CANDIDATE_CHARS; c++)
        {
            libData.foundPlates[ p].diagCharStdSize [c] = 0;
        }
    }

   

    Groups_Init();// plate group processing
}


// load image and init memory, but do no processing, to allow C# code to call processing stages incrementally

void LPROCR_lib_LoadImage (  int * img, int width, int height, int diagEnabled, LPR_PROCESS_OPTIONS * processOptions, int *error)
{
   int  p = 0;
   int  c = 0;

   LPROCR_lib_init( img, width, height, diagEnabled,  error);

   libData.processOptions.EnableRotationRoll     = processOptions->EnableRotationRoll;
   libData.processOptions.EnableAutoRotationRoll = processOptions->EnableAutoRotationRoll;
   libData.processOptions.roll                   = processOptions->roll;
   libData.processOptions.rotation               = processOptions->rotation;
   libData.processOptions.quickPlateTest         = processOptions->quickPlateTest;


   for ( p =0; p <  MAX_CANDIDATE_PLATES ; p++)
   {
      libData.foundPlates[p].leftEdge =-1;
      libData.foundPlates[p].rightEdge =-1;
      libData.foundPlates[p].topEdge=-1;
      libData.foundPlates[p].bottomEdge=-1;
      freeMemory ( & libData.foundPlates[p].image);
      freeMemory ( & libData.foundPlates[p].diagImage);
      for (c=0; c < MAX_ACTUAL_CHARS; c++)
      {
         libData.foundPlates[p].plateChars[c] = -1;
         libData.foundPlates[p].charScores[c] = 0;
      }
   }


  

}

// load the found plate images into memory for incremental processing from c#

int LPROCR_lib_ExtractFoundPlates ( int * error )
{
   int p = 0;
   int c = 0;

   for (p = 0; p < libData.numFoundPlates; p++)
   {
      

      int pLeft   = libData.foundPlates[p].leftEdge;
      int pRight  = libData.foundPlates[p].rightEdge;
      int pTop    = libData.foundPlates[p].topEdge;
      int pBottom = libData.foundPlates[p].bottomEdge;



      int pw  = libData.foundPlates[p].width;
      int ph = libData.foundPlates[p].height;

      freeMemory ( & libData.foundPlates[p].image  );
      libData.foundPlates[p].image = (int *) malloc ( sizeof(int) * pw * ph  );
      if ( libData.foundPlates[p].image == 0 )
      {
         *error = 1;
         return(0);
      }

      if ( libData.diagEnabled  == 1 )
      {
         freeMemory ( & libData.foundPlates[p].diagImage  );
         libData.foundPlates[p].diagImage= (int *) malloc ( sizeof(int) * pw * ph * 3 ); // the factor of 3 is because this is going to be  DIAG_COLOR_IMAGE struct
         if ( libData.foundPlates[p].diagImage == 0 )
         {
            *error = 1;
            return(0);
         }
      }


      //  extract a full resolution plate image into a seperate plate image buffer, this plate image may get rotated/roll correction in the findCharsInPlate function.

      extractSubImage(libData.imageFullRes,  libData.imageWidth , libData.imageHeight, pLeft, pRight, pTop, pBottom, libData.foundPlates[p].image);


   }
}

//  primary input to the plate reading lib, pass in a luminance array [x,y] of the entire image and the plates will be read
//  all-in-one LPR method, pass in image, then string results are ready get with subsequent calls

int LPROCR_lib_ReadThisImage (  int * img, int width, int height, int diagEnabled, LPR_PROCESS_OPTIONS * processOptions, int *error)
{

    int p;
    int pLeft, pRight, pTop, pBottom;

    int candidateChars[MAX_CANDIDATE_CHARS];

    int finalIndex;

    int pw = 0;
    int ph = 0;


    int cLeft = 0;
    int cRight = 0;
    int cTop = 0;
    int cBottom = 0;
    int c;
    int cw, ch;
    int* charImg;
    float score =0;
 
    int actualCharIndex  = 0;
    int noisyCharCount = 0;
 


    libData.processOptions.EnableRotationRoll     = processOptions->EnableRotationRoll;
    libData.processOptions.EnableAutoRotationRoll = processOptions->EnableAutoRotationRoll;
    libData.processOptions.roll                   = processOptions->roll;
    libData.processOptions.rotation               = processOptions->rotation;
    libData.processOptions.quickPlateTest         = processOptions->quickPlateTest;

    // init memory

    LPROCR_lib_init( img, width, height, diagEnabled,  error);

    if ( *error != 0 ) return (0);


    for ( p =0; p <  MAX_CANDIDATE_PLATES ; p++)
    {
        libData.foundPlates[p].leftEdge =-1;
        libData.foundPlates[p].rightEdge =-1;
        libData.foundPlates[p].topEdge=-1;
        libData.foundPlates[p].bottomEdge=-1;
        freeMemory ( & libData.foundPlates[p].image);
        freeMemory ( & libData.foundPlates[p].diagImage);
        for (c=0; c < MAX_ACTUAL_CHARS; c++)
        {
            libData.foundPlates[p].plateChars[c] = -1;
            libData.foundPlates[p].charScores[c] = 0;
        }
    }


    if ( width <= 400 ) // its already a plate comming from a test application
    {
        libData.numFoundPlates = 1;
        libData.foundPlates[0].leftEdge = 0;
        libData.foundPlates[0].rightEdge = width-1;
        libData.foundPlates[0].topEdge = 0;
        libData.foundPlates[0].bottomEdge = height-1;
        libData.foundPlates[0].width = width-1;
        libData.foundPlates[0].height = height-1;
    }
    else
    {

        // find the plates...

        libData.numFoundPlates = LPROCR_lib_findThePlates( error);
        if ( *error != 0 )
        {
            RejectLogAdd("memory error on _findThePlates");
            return(0);
        }

        if (libData.numFoundPlates == 0 ) 
        {
           RejectLogAdd("_findThePlates found 0 plates");

           return(0);
        }

        for (p = 0; p < MAX_ACTUAL_PLATES ; p++)
        {
            libData.finalPlateList[p].image =0;
            libData.finalPlateList[p].diagImage =0;
            libData.finalPlateList[p].leftEdge =-1;
            libData.finalPlateList[p].rightEdge =-1;
            libData.finalPlateList[p].topEdge=-1;
            libData.finalPlateList[p].bottomEdge=-1;

            for (c=0; c < MAX_ACTUAL_CHARS; c++)
            {
                libData.finalPlateList[p].plateChars[c] = -1;
                libData.finalPlateList[p].charScores[c] = 0;
            }
        }

    }

  

    ////////////////////////////////////////////////////////////////////////////
    //
    //
    // for each candidate plate, look for chars and for each char found, do an OCR
    //  we will decide if this plate is a keeper after the OCR process

    for (p = 0; p < libData.numFoundPlates; p++)
    {
        noisyCharCount = 0;

        pLeft   = libData.foundPlates[p].leftEdge;
        pRight  = libData.foundPlates[p].rightEdge;
        pTop    = libData.foundPlates[p].topEdge;
        pBottom = libData.foundPlates[p].bottomEdge;


        for (c=0; c < MAX_CANDIDATE_CHARS; c++)
        {
            candidateChars[c] = -1;
        }


        pw  = libData.foundPlates[p].width;
        ph = libData.foundPlates[p].height;

        freeMemory ( & libData.foundPlates[p].image  );
        libData.foundPlates[p].image = (int *) malloc ( sizeof(int) * pw * ph  );
        if ( libData.foundPlates[p].image == 0 )
        {
            *error = 1;
            return(0);
        }

        if ( diagEnabled == 1 )
        {
            freeMemory ( & libData.foundPlates[p].diagImage  );
            libData.foundPlates[p].diagImage= (int *) malloc ( sizeof(int) * pw * ph * 3 ); // the factor of 3 is because this is going to be  DIAG_COLOR_IMAGE struct
            if ( libData.foundPlates[p].diagImage == 0 )
            {
                *error = 1;
                return(0);
            }
        }


        //  extract a full resolution plate image into a seperate plate image buffer, this plate image may get rotated/roll correction in the findCharsInPlate function.

        extractSubImage(libData.imageFullRes, libData.imageWidth , libData.imageHeight, pLeft, pRight, pTop, pBottom, libData.foundPlates[p].image);


        //  find the chars in the plate image

        LPROCR_lib_findCharsInPlate( p, error, (bool) diagEnabled );
       
        if ( *error != 0 ) return(0);


        //// OCR the chars
   



        actualCharIndex = 0;

        for ( c = 0; c < MAX_CANDIDATE_CHARS; c++)
        {
            charImg = 0;

            cLeft =-1;
            cRight =-1;
            cTop=-1;
            cBottom=-1;

            GetCharLocationCandidatePlates(p, c, & cLeft, & cRight, & cTop, & cBottom);


            if (cLeft < 0 || cRight <0 || cTop < 0 || cBottom < 0) continue;

            cw = cRight - cLeft;
            ch = cBottom - cTop;

            if ( cw <= 0 || ch <= 0 ) continue;

            if ( ch / cw > 6 && c == 0  ) continue; // its a leading plate edge not a char

            charImg = (int *)malloc (sizeof(int) * cw * ch);
            if ( charImg == 0 ) 
            {
                *error = 1;
                return(0);
            }

            // extract a full resolution image of the char from the plate image
            extractSubImage( libData.foundPlates[p].image, pw, ph, cLeft, cRight, cTop, cBottom, charImg);


            candidateChars[c] = LPROCR_lib_ReadThisChar( charImg, cw, ch, & score, libData.foundPlates[p].aveCharWidth , libData.foundPlates[ p].diagCharStdSize [c], error);
            libData.foundPlates[p].charScores[c] = score;

            freeMemory ( & charImg );

            libData.foundPlates[p].charLocations[c].leftEdge = cLeft;
            libData.foundPlates[p].charLocations[c].rightEdge = cRight;
            libData.foundPlates[p].charLocations[c].topEdge = cTop;
            libData.foundPlates[p].charLocations[c].bottomEdge = cBottom;


            if ( *error != 0 ) return(0);


            if ( candidateChars[c]  == '-' ) // not a real character, its noise 
            {
                noisyCharCount++;
            }


        }




        // copy out the valid characters
        actualCharIndex = 0 ;
        for (c=0; c < MAX_CANDIDATE_CHARS; c++)
        {
            if (  candidateChars[c] != -1 )
            {
                libData.foundPlates[p].plateChars[actualCharIndex] =(char)  candidateChars[c];
                actualCharIndex ++;
            }
            if ( actualCharIndex == MAX_ACTUAL_CHARS )  break;
        }

        if ( actualCharIndex <= 3 ||  noisyCharCount > 1 ) // throw it away
            //   if (   noisyCharCount > 1 )
        {
            libData.foundPlates[p].leftEdge = -1;
        }

    }



    //////////////////////////////////////////////
    //
    //
    // now form the final plate list - the keepers

    finalIndex = 0;
    for (p = 0; p < libData.numFoundPlates; p++)
    {

        if ( libData.foundPlates[p].leftEdge != -1 )
        {
            // keep it
            libData.finalPlateList[finalIndex].leftEdge    = libData.foundPlates[p].leftEdge;
            libData.finalPlateList[finalIndex].rightEdge   = libData.foundPlates[p].rightEdge;
            libData.finalPlateList[finalIndex].topEdge     = libData.foundPlates[p].topEdge;
            libData.finalPlateList[finalIndex].bottomEdge  = libData.foundPlates[p].bottomEdge;
            libData.finalPlateList[finalIndex].image       = libData.foundPlates[p].image;
            libData.finalPlateList[finalIndex].width       = libData.foundPlates[p].width;
            libData.finalPlateList[finalIndex].height      = libData.foundPlates[p].height ;
            libData.finalPlateList[finalIndex].diagImage   = libData.foundPlates[p].diagImage ;
            libData.finalPlateList[finalIndex].aveCharWidth =  libData.foundPlates[p].aveCharWidth;

            CopyScores ( libData.foundPlates[p].charScores, libData.finalPlateList[p].charScores,MAX_ACTUAL_CHARS  );

            CopyString ( libData.foundPlates[p].plateChars,  libData.finalPlateList[finalIndex].plateChars, MAX_ACTUAL_CHARS );
            CopyCLocations ( libData.foundPlates[p].charLocations,  libData.finalPlateList[finalIndex].charLocations );

            if ( libData.diagEnabled == 1 )
            {
               for(c=0; c < MAX_ACTUAL_CHARS; c++)
               {
                  libData.finalPlateList[finalIndex].diagCharStdSize[c] = libData.foundPlates[p].diagCharStdSize[c]; // point to the diag char image
               }
            }

            finalIndex++;

            if ( finalIndex >= MAX_ACTUAL_PLATES)
            {
                break;
            }
        }
    }

    libData.numFoundPlates = finalIndex;


    return (  finalIndex );

}


void CopyScores ( float *src, float *dst, int count)
{
    int c;

    for (c=0;c< count;c++)
    {
        dst[c] = src[c] ;
    }
}

void CopyString (char * src, char * dst, int count)
{
    int c;

    for (c=0;c< count;c++)
    {
        if ( src[c] != -1 ) 
        {
            dst[c]  = src[c];
        }
        else
            break;
    }
    if ( c >= MAX_ACTUAL_CHARS) dst[c-1] = 0;
    else dst[c] = 0;// add null to the end of the string 


}

void CopyCLocations ( LPROCR_lib_CHAR_LOCATION *src,  LPROCR_lib_CHAR_LOCATION *dest )
{

    int c = 0;
    for ( c = 0; c < MAX_CANDIDATE_CHARS; c++)
    {
        // init the values as un-used
        dest[c].leftEdge = -1;
        dest[c].rightEdge =-1;
        dest[c].topEdge = -1;
        dest[c].bottomEdge =-1;

        if ( src[c].leftEdge != -1 )
        {
            dest[c].leftEdge = src[c].leftEdge;
            dest[c].rightEdge =src[c].rightEdge ;
            dest[c].topEdge = src[c].topEdge ;
            dest[c].bottomEdge =src[c].bottomEdge;
        }
    }
}




int _stdcall LPROCR_lib_GetNumFoundPlates (  )
{
    return( libData.numFoundPlates );
}




void _stdcall LPROCR_lib_GetPlateString (int plateIndex, char * inStrMemory, float * score )
{
    int c=0;
    int scoreCnt = 0;

    // plateChars[MAX_ACTUAL_CHARS+1]

    for (c=0; c< MAX_ACTUAL_CHARS-1; c++)
    {
        inStrMemory[c] = 0;
    }

    for (c=0; c< MAX_ACTUAL_CHARS-1; c++)
    {
        if ( libData.finalPlateList[plateIndex].plateChars[c] == -1 )
            break;

        inStrMemory[c] =libData.finalPlateList[plateIndex].plateChars[c];
        if ( libData.finalPlateList[plateIndex].charScores[c] != 0 )
        {
            (*score) += libData.finalPlateList[plateIndex].charScores[c];
            scoreCnt++;
        }
    }


    inStrMemory[c] = 0;// ensure ends with null char

    if ( scoreCnt == 0 )
        *score = 0;
    else
        *score = *score / scoreCnt;
}



void _stdcall LPROCR_lib_GetPlateImage ( int plateIndex, int * img, int useCandidatePlateList )
{

    int x,xo,y;
    int w,h;
    int *src;

    if ( img == 0 ) return;

    if ( useCandidatePlateList == 1 )
    {
       if ( plateIndex >= MAX_CANDIDATE_PLATES ) return;
       w = libData.foundPlates[plateIndex ].width;
       h = libData.foundPlates[plateIndex ].height;
  
       src = libData.foundPlates[plateIndex].image;

       if ( src == 0 ) return;

       for (x=0; x< w; x++)
       {
          xo = x* h;
          for ( y=0; y< h; y++)
          {
             img[xo+y] = src[xo+y];
          }
       }
   
    }
    else
    {
       if ( plateIndex >= MAX_ACTUAL_PLATES ) return;
       w = libData.finalPlateList[plateIndex ].width;
       h = libData.finalPlateList[plateIndex ].height;

       src = libData.finalPlateList[plateIndex].image;
       if ( src == 0 ) return;

       for (x=0; x< w; x++)
       {
          xo = x* h;
          for ( y=0; y< h; y++)
          {
             img[xo+y] = src[xo+y];
          }
       }

    }

  

}


void _stdcall LPROCR_lib_GetPlateImageSize ( int plateIndex, int * width, int * height, int useCandidatePlateList )
{

    if ( plateIndex >= MAX_ACTUAL_PLATES ) return;

    if ( useCandidatePlateList == 1 )
    {
       *width = libData.foundPlates[plateIndex ].width;
       *height = libData.foundPlates[plateIndex ].height;
    }
    else
    {
       *width = libData.finalPlateList[plateIndex ].width;
       *height = libData.finalPlateList[plateIndex ].height;
    }
}

void _stdcall LPROCR_lib_GetSubImageSize ( int * width, int * height )
{

    *width = libData.imageSubWidth;
    *height = libData.imageSubHeight;
}


// to allocate memory for the images and keep diagnostic arrays around for probing and displaying

void _stdcall LPROCR_lib_init( int * img, int width, int height, int diagEnabled, int *error)
{

    // this function is called on every image to be read,
    //  when an image is processed, memory is malloced. This malloced memory is left around
    //  until this function is called at the begining of the  next image. then memory from
    //   the previous call is freed and new memory is allocated (per image size).
    //
    //   this will leave one image hanging around when the program is closed
    //  todo: implement the dispose

    int i;
  
    libData.diagnosticsInitialized  = 0;

    libData.currentLogIndex = 0;

    libData.InterFrameStringBuffIndex = 0 ;

    libData.diagEnabled = diagEnabled;

    // the libdata is a static array of structures; one for each instance

    libData.imageWidth = width;
    libData.imageHeight = height;

    libData.imageSubWidth  = libData.imageWidth  / X_DIVISOR;
    libData.imageSubHeight = libData.imageHeight / Y_DIVISOR;


    // local point points to source image passed in from caller

    libData.imageFullRes = img;  


    // alloc memory for the subscalled version of the full image

    freeMemory (  & libData.imageSubPrev  );
    libData.imageSubPrev = libData.imageSub; // store the previous image for motion detection
    libData.imageSub = (int *) malloc ( sizeof(int) * libData.imageSubWidth * libData.imageSubHeight); 


    if ( libData.imageSub == 0 )
    {
        *error = ERROR_memoryNotAllocated ;
        return;
    }

    // alloc memory for the subscaled size edgeMap

    freeMemory (  & libData.edgeMapSub);
    libData.edgeMapSub =(int *) malloc ( sizeof(int) * libData.imageSubWidth *  libData.imageSubHeight);


    if ( libData.edgeMapSub == 0 )
    {
        *error = ERROR_memoryNotAllocated ;
        return;
    }

    // alloc memory for the full resolution edgeMap

    freeMemory ( & libData.edgeMapFullRes  );

    libData.edgeMapFullRes = (int *) malloc ( sizeof(int) * libData.imageWidth *   libData.imageHeight);

    if ( libData.edgeMapFullRes == 0 )
    {
        *error = ERROR_memoryNotAllocated ;
        return;
    }

    if ( libData.diagEnabled ==1) initMemory (0, libData.edgeMapFullRes, libData.imageWidth,   libData.imageHeight); // for diag mode, clear the whole map - see ContourEdge



     // alloc memory for the full resolution integral table (the whole image is not used, only regions around plates)

    freeMemoryDouble ( & libData.integralTableFullRes  );

    libData.integralTableFullRes = (double *) malloc ( sizeof(double) * libData.imageWidth *   libData.imageHeight);

    if ( libData.integralTableFullRes == 0 )
    {
        *error = ERROR_memoryNotAllocated ;
        return;
    }
    if ( libData.diagEnabled ==1) initMemoryDouble (0, libData.integralTableFullRes, libData.imageWidth,   libData.imageHeight); // for diag mode, clear the whole map so unused pixels will display


    //   now generate the subscalled image

    subscale ( libData.imageFullRes, libData.imageSub, libData.imageWidth, libData.imageHeight, X_DIVISOR, Y_DIVISOR );

    for (i=0; i < MAX_CANDIDATE_PLATES; i++)
    {
        libData.pLocationList[i].leftEdge = -1;
        libData.pLocationList[i].topEdge = -1;
        libData.pLocationList[i].bottomEdge = -1;
        libData.pLocationList[i].rightEdge = -1;
    }



    libData.memoryAllocated = 1;

    if (libData.diagEnabled )
        LPROCR_diags_init (   width,  height, error);
}


void   initMemory (int initValue, int * image, int w, int h)
{
    int x,y, offset;

    offset = 0;
    for ( x=0; x < w; x++)
    {
        for(y=0; y < h; y++)
        {
            *(image +offset+y) = initValue;
        }
        offset += y;
    }

}


void   initMemoryDouble (double initValue, double * image, int w, int h)
{
    int x,y, offset;

    offset = 0;
    for ( x=0; x < w; x++)
    {
        for(y=0; y < h; y++)
        {
            *(image +offset+y) = initValue;
        }
        offset += y;
    }

}




void _stdcall LPROCR_lib_dispose()
{
    int p;
    int c = 0;

  
    libData.memoryAllocated = 0;

    freeMemory ( & libData.imageSub  );

    freeMemory (  & libData.edgeMapSub);

    freeMemory (  & libData.edgeMapFullRes );

    for ( p =0; p <  MAX_CANDIDATE_PLATES ; p++)
    {
        freeMemory (  &libData.foundPlates[p].image );
        freeMemory (  &libData.foundPlates[p].diagImage );

        for ( c = 0; c < MAX_CANDIDATE_CHARS; c++)
        {
            freeMemory (  & libData.foundPlates[ p].diagCharStdSize [c] );
        }  
    }

    for (p = 0; p < MAX_ACTUAL_PLATES ; p++)
    {
        freeMemory ( & libData.finalPlateList[p].image );
        freeMemory ( & libData.finalPlateList[p].diagImage );

        for ( c = 0; c < MAX_CANDIDATE_CHARS; c++)
        {
            freeMemory (  & libData.foundPlates[ p].diagCharStdSize [c] );
        }
    }

}






//
// method LPROCR_lib_getImage
//

void _stdcall LPROCR_lib_getImage( int * Y, int * error) 
{
    int x,y, offset;


    if ( libData.memoryAllocated == 0 )
    {
        *error = ERROR_memoryNotAllocated  ;
        return;
    }

    // copy luminance array
    offset = 0;
    for ( x=0; x < libData.imageWidth/X_DIVISOR; x++)
    {
        for(y=0; y < libData.imageHeight/Y_DIVISOR; y++)
        {
            *(Y+offset+y) = *(libData.imageSub+offset+y);
        }
        offset += y;
    }

}



//
// method LPROCR_lib_getEdgeMapFullRes
//

void _stdcall LPROCR_lib_getEdgeMapFullRes( int * Y,  int * error) 
{
    int x,y, offset;


    if ( libData.memoryAllocated == 0 )
    {
        *error = ERROR_memoryNotAllocated  ;
        return;
    }
    if ( Y == 0 )
    {
        *error = 2;
        return;
    }


    // copy luminance array
    offset = 0;
    for ( x=0; x < libData.imageWidth; x++)
    {
        for(y=0; y < libData.imageHeight; y++)
        {
            *(Y+offset+y) = *(libData.edgeMapFullRes+offset+y);
        }
        offset += y;
    }

}








//
// method LPROCR_lib_getlibData.edgeMapSub
//

void _stdcall LPROCR_lib_getEdgeMapSub( int * outarray,  int * error) 
{
    int x,y, offset;


    if ( libData.memoryAllocated == 0 )
    {
        *error = ERROR_memoryNotAllocated  ;
        return;
    }

    // copy luminance array
    offset = 0;
    for ( x=0; x < libData.imageSubWidth; x++)
    {
        offset = x * libData.imageSubHeight;
        for(y=0; y < libData.imageSubHeight; y++)
        {
            *(outarray+offset+y) = *(libData.edgeMapSub+offset+y);
        }
    }

}

//
// method getPlateLocation
//

void LPROCR_lib_getPlateLocation( int plateIndex, LPROCR_lib_PLATE_LOCATION *pLocation)
{

    if ( plateIndex >= MAX_CANDIDATE_PLATES) return;

    pLocation->leftEdge = libData.finalPlateList [plateIndex].leftEdge;
    pLocation->rightEdge = libData.finalPlateList[plateIndex].rightEdge;
    pLocation->topEdge = libData.finalPlateList [plateIndex].topEdge;
    pLocation->bottomEdge = libData.finalPlateList [plateIndex].bottomEdge;


}

//
// method findThePlates
//
int _stdcall LPROCR_lib_findThePlates (  int * error)
{
    int p = 0;

    if ( libData.memoryAllocated == 0 )
    {
        *error = ERROR_memoryNotAllocated  ;
        return(0);
    }


    // if the entire image is smaller than a plate, do nothing with this.

    if ( libData.imageWidth < MAX_PLATE_WIDTH)
    {
        for ( p =0; p < MAX_CANDIDATE_PLATES ; p++)
        {
            libData.pLocationList[p].leftEdge = -1;
            libData.pLocationList[p].rightEdge = -1;
            libData.pLocationList[p].topEdge = -1;
            libData.pLocationList[p].bottomEdge = -1;
        }
        RejectLogAdd("libData.imageWidth < MAX_PLATE_WIDTH");
        return (0);
    }

    if ( libData.imageHeight < MAX_PLATE_HEIGHT)
    {
        for ( p =0; p < MAX_CANDIDATE_PLATES ; p++)
        {
            libData.pLocationList[p].leftEdge = -1;
            libData.pLocationList[p].rightEdge = -1;
            libData.pLocationList[p].topEdge = -1;
            libData.pLocationList[p].bottomEdge = -1;
        }
        RejectLogAdd("libData.imageHeight < MAX_PLATE_HEIGHT");

        return (0);
    }


    findPlateEdges( libData.pLocationList, error);



    return (  libData.numFoundPlates );
}




//
// method subscale
//


void _stdcall subscale ( int * array2dLarge, int * array2dSmall, 
                        int largeWidth,
                        int largeHeight,
                        int Xdivisor, 
                        int Ydivisor )
{


  
    int y = 0;
    int lOffset = 0;
    int sOffset = 0;
    int sy = 0;
    int sx = 0;
    int smallWidth = largeWidth / Xdivisor;
    int smallHeight = largeHeight / Ydivisor;

    for (sx = 0; sx < smallWidth; sx ++)
    {
        y = 0;
        for (sy = 0; sy < smallHeight; sy++)
        {
            *(array2dSmall + sOffset + sy) = *(array2dLarge + lOffset  + y);
            y = y + Ydivisor;
        }
        sOffset += smallHeight;
        lOffset += (largeHeight * Xdivisor);
    }

}



//
// method normalizeToWideSpectrum
//


void normalizeToWideSpectrum( int * a, int w, int h)
{
    int brightest = 0;
    int darkest = 255;
    int x, y, xo;


    // find the brightest and darkest pixels
    xo = 0;
    for (x = 0; x < w; x++)
    {
        for (y = 0; y < h; y++)
        {
            if ( *(a+y+xo) > brightest) brightest = *(a+y+xo);
            if ( *(a+y+xo) < darkest) darkest = *(a+y+xo);
        }
        xo  += h;
    }

    // if the image is all the same then do nothing and return
    if ( (brightest - darkest) / 2 == 0 ) return;
    ///
    /// 
    int offset = -((brightest - darkest) / 2) - darkest; // center the curve around zero
    int scaleFactor = (500) / (((brightest - darkest)) / 2);

    xo = 0;
    for (x = 0; x < w; x++)
    {
        for (y = 0; y < h; y++)
        {
            *(a+y+xo) = scaleFactor * (*(a+y+xo) + offset);
        }
        xo  += h;
    }


}// end normalizeToWideSpectrum




//
// method findPlateEdges
//



void findPlateEdges( LPROCR_lib_PLATE_LOCATION *pLocationList, int * error)
{
    int boxColorsPerPlate [] = { 100, 130, 160, 190, 220, 255};
    int numBoxColors = 6;

    DIAGCOLOR color;

    int cp, fp;


    //  int yEnd = libData.imageSubHeight - 1;

    for ( cp= 0; cp < MAX_CANDIDATE_PLATES; cp++)
    {
        pLocationList[cp].leftEdge = -1;
        pLocationList[cp].rightEdge = -1;
        pLocationList[cp].topEdge = -1;
        pLocationList[cp].bottomEdge = -1;
    }



    int secondOrder = 1;  // add adjacent pixel edge energy


    int xStart;
    int xEnd;
    int yStart;
    int yEnd;

    int dx,dy;

    if ( libData.imageWidth <= 640 )
    {
        xStart =  0;
        xEnd =  libData.imageSubWidth -1;
        yStart = 0 ;
        yEnd =  libData.imageSubHeight -1;

    }
    else
    {
        xStart = (10 * libData.imageSubWidth)/100;
        xEnd = (90 * libData.imageSubWidth)/100;
        yStart = (10* libData.imageSubHeight) /100;
        yEnd = (90 * libData.imageSubHeight) /100;
    }



    createEdgeContourMap( libData.imageSub, xStart, yStart,
        xEnd, yEnd, 
        libData.edgeMapSub, libData.imageSubWidth, 
        libData.imageSubHeight, 
        secondOrder, error);

    if ( *error != 0 ) return;



    // find the brightest spot and draw a box around it (exclude the edges of the frame which often bring in edge noise

    for ( fp= 0; fp < MAX_ACTUAL_PLATES; fp++)
    {
        libData.foundPlates[fp].leftEdge   = -1;
        libData.foundPlates[fp].rightEdge  = -1;
        libData.foundPlates[fp].topEdge    = -1;
        libData.foundPlates[fp].bottomEdge = -1;
    }
    libData.numFoundPlates = 0;
    fp = 0;
    for ( cp= 0; cp < MAX_CANDIDATE_PLATES; cp++)
    {



        findBrightestSpot(cp, libData.edgeMapSub, libData.imageSubWidth, 
            libData.imageSubHeight,
            &pLocationList[cp].centerX,
            &pLocationList[cp].centerY);


        if ( pLocationList[cp].centerX < 0 || pLocationList[cp].centerY<0)
        {
            pLocationList[cp].leftEdge =-1 ;
            RejectLogAdd("pLocationList[cp].centerX < 0 || pLocationList[cp].centerY<0");

            continue; 
        }



        // create edge boundrys in the sub sampled space for drawing the box

        pLocationList[cp].leftEdge = pLocationList[cp].centerX - (MAX_PLATE_WIDTH/X_DIVISOR/2);
        pLocationList[cp].rightEdge = pLocationList[cp].centerX + (MAX_PLATE_WIDTH/X_DIVISOR/2);
        pLocationList[cp].topEdge = pLocationList[cp].centerY - (MAX_PLATE_HEIGHT/Y_DIVISOR/2);
        pLocationList[cp].bottomEdge = pLocationList[cp].centerY + (MAX_PLATE_HEIGHT/Y_DIVISOR/2);



        if ( pLocationList[cp].leftEdge < 0 )
            pLocationList[cp].leftEdge =0 ;

        if ( pLocationList[cp].rightEdge >= libData.imageSubWidth )  
            pLocationList[cp].rightEdge = libData.imageSubWidth - 1;

        if ( pLocationList[cp].topEdge < 0 )
            pLocationList[cp].topEdge= 0 ;


        if ( pLocationList[cp].bottomEdge >= libData.imageSubHeight )  
            pLocationList[cp].bottomEdge = libData.imageSubHeight - 1;

        if ( pLocationList[cp].rightEdge - pLocationList[cp].leftEdge  > MAX_PLATE_WIDTH/X_DIVISOR )
        {
            pLocationList[cp].leftEdge =-1 ;
            RejectLogAdd("pLocationList[cp].rightEdge - pLocationList[cp].leftEdge  > MAX_PLATE_WIDTH/X_DIVISOR");

            continue; 
        }
        if ( pLocationList[cp].rightEdge - pLocationList[cp].leftEdge  < MIN_PLATE_WIDTH/X_DIVISOR)
        {
            pLocationList[cp].leftEdge =-1 ;
            RejectLogAdd("pLocationList[cp].rightEdge - pLocationList[cp].leftEdge  < MIN_PLATE_WIDTH/X_DIVISOR");

            continue; 
        }
        if ( pLocationList[cp].bottomEdge  -  pLocationList[cp].topEdge < MIN_PLATE_HEIGHT/Y_DIVISOR)
        {
            pLocationList[cp].leftEdge =-1 ;
            RejectLogAdd("pLocationList[cp].bottomEdge  -  pLocationList[cp].topEdge < MIN_PLATE_HEIGHT/Y_DIVISOR");

            continue; 
        }
        if ( pLocationList[cp].bottomEdge  -  pLocationList[cp].topEdge > MAX_PLATE_HEIGHT/Y_DIVISOR)
        {
            pLocationList[cp].leftEdge =-1 ;
            RejectLogAdd("pLocationList[cp].bottomEdge  -  pLocationList[cp].topEdge > MAX_PLATE_HEIGHT/Y_DIVISOR");

            continue; 
        }


        if ( libData.diagEnabled == 1)
        {
            /*         int boxColorsPerPlate [] = { 100, 130, 160, 190, 220, 255};
            int numBoxColors = 6;*/

            if ( cp >= numBoxColors )
                color.blue = 255;
            else
                color.blue = boxColorsPerPlate[cp];

            color.green = 0;
            color.red = 0;
            drawBox ( 0, & pLocationList[cp], &color);
        }



        // get more exact plate boundry



        findExactEdges ( & pLocationList[cp], error);

        if ( *error != 0 )
        {
            RejectLogAdd("findExactEdges *error != 0" );

            pLocationList[cp].leftEdge = -1;
            return;
        }

        if ( pLocationList[cp].topEdge < 0 )
        {
            RejectLogAdd("pLocationList[cp].topEdge < 0");
            continue;
        }
        if ( pLocationList[cp].leftEdge < 0 ) 
        {
            RejectLogAdd("pLocationList[cp].leftEdge < 0 ");
            continue;
        }
        if ( pLocationList[cp].rightEdge < 0 ) 
        {
            RejectLogAdd("pLocationList[cp].rightEdge < 0");
            continue;
        }
        if ( pLocationList[cp].bottomEdge < 0 )  
        {
            RejectLogAdd("pLocationList[cp].bottomEdge < 0");
            continue;
        }

        // sometimes we find the same plate twice because the find-exact edges opens the search window alot.
        if ( cp > 0 )
        {
            dy = pLocationList[cp].centerY - pLocationList[cp-1].centerY ;
            if ( dy < 0 ) dy = -1 * dy;
            dx = pLocationList[cp].centerX - pLocationList[cp-1].centerX ;
            if ( dx < 0 ) dx = -1 * dx;

            if ( dy < 50 && dx < 100 )
            {
                RejectLogAdd("dy < 50 && dx < 100");

                pLocationList[cp].leftEdge = -1;
            }
        }

        if ( pLocationList[cp].leftEdge < 0 )  continue;
        if ( pLocationList[cp].rightEdge < 0 )  continue;
        if ( pLocationList[cp].topEdge < 0 )  continue;
        if ( pLocationList[cp].bottomEdge < 0 )  continue;

        if ( pLocationList[cp].topEdge >= libData.imageHeight )  continue;
        if ( pLocationList[cp].leftEdge >= libData.imageWidth )  continue;
        if ( pLocationList[cp].rightEdge >= libData.imageWidth  )  continue;
        if ( pLocationList[cp].bottomEdge >= libData.imageHeight  )  continue;

        int maxWidth = MAX_PLATE_WIDTH + 20;
        int minWidth = MIN_PLATE_WIDTH - 10;
        int maxHeight = MAX_PLATE_HEIGHT + 10;
        int minHeight = MIN_PLATE_HEIGHT - 5;

        if ( pLocationList[cp].bottomEdge - pLocationList[cp].topEdge > maxHeight )
            RejectLogAdd("pLocationList[cp].bottomEdge - pLocationList[cp].topEdge > MAX_PLATE_HEIGHT");

        if (  pLocationList[cp].bottomEdge - pLocationList[cp].topEdge < minHeight)
            RejectLogAdd(" pLocationList[cp].bottomEdge - pLocationList[cp].topEdge < MIN_PLATE_HEIGHT");

        if ( pLocationList[cp].rightEdge - pLocationList[cp].leftEdge > maxWidth)
            RejectLogAdd("pLocationList[cp].rightEdge - pLocationList[cp].leftEdge > MAX_PLATE_WIDTH");

        if (  pLocationList[cp].rightEdge - pLocationList[cp].leftEdge < minWidth )
            RejectLogAdd(" pLocationList[cp].rightEdge - pLocationList[cp].leftEdge < MIN_PLATE_WIDTH");

        if ( pLocationList[cp].bottomEdge - pLocationList[cp].topEdge < maxHeight && pLocationList[cp].bottomEdge - pLocationList[cp].topEdge > minHeight)
        {
            if ( pLocationList[cp].rightEdge - pLocationList[cp].leftEdge < maxWidth && pLocationList[cp].rightEdge - pLocationList[cp].leftEdge >minWidth)
            {
                // keep this plate
                libData.foundPlates[fp].leftEdge   = pLocationList[cp].leftEdge;
                libData.foundPlates[fp].rightEdge  = pLocationList[cp].rightEdge;
                libData.foundPlates[fp].topEdge    = pLocationList[cp].topEdge;
                libData.foundPlates[fp].bottomEdge = pLocationList[cp].bottomEdge;

                libData.foundPlates[fp].width  =  pLocationList[cp].rightEdge - pLocationList[cp].leftEdge;
                libData.foundPlates[fp].height =  pLocationList[cp].bottomEdge - pLocationList[cp].topEdge;

                fp++;
                if ( fp == MAX_ACTUAL_PLATES ) break;
            }
        }
    }

    libData.numFoundPlates = fp;


}

double SampleIntegralImage(int X0, int Y0, int X1, int Y1)
{
    int w,h;
    int xo0, xo1;
    double *table;
    w = libData.imageWidth;
    h = libData.imageHeight;

    table = libData.integralTableFullRes;


    X0--;
    Y0--;
    if (X0 < 0) X0 = 0;
    if (Y0 < 0) Y0 = 0;

    if (X1 >= w) X1 = w - 1;
    if (Y1 >= h) Y1 = h - 1;

    xo0 = X0 * h;
    xo1 = X1 * h;

    double Sum = table[xo1+ Y1] - table[xo1+ Y0] - table[xo0+ Y1] + table[xo0+ Y0];

    return Sum;
}

// CreateIntegralImage  a.k.a. create  Summed area table from the edge contour map

void CreateIntegralImage(int xStart, int xEnd, int yStart, int yEnd)
{
    int w,h;
    int x,y,xo;
    double V;
    double PrevV;
    double *table;
    int *image;

    w = libData.imageWidth;
    h = libData.imageHeight;

    table = libData.integralTableFullRes;
    image = libData.edgeMapFullRes;

    // Top Left Corner stays the same

    if ( xStart < 0) xStart = 0;
    if ( xEnd >= w ) xEnd = w -1;
    if ( yStart < 0 ) yStart =0;
    if ( yEnd >= h ) yEnd = h-1;

    x = xStart;
    xo = x * h;
    y = yStart;
    table[xo + y] =(double) image[0];

    // Prime top horizontal line
    PrevV = table[xo + y] ;
    y =yStart;
    for (x = xStart+1; x < xEnd; x++)
    {
        xo = x * h;
        V = (double) image[xo+y] + PrevV;
        table[xo + y] = V;
        PrevV = V;
    }

    // Prime left vertical line
    x = xStart;
    xo = x * h;
    y = yStart;
    PrevV = table[xo + y] ;
    for (y = yStart+1; y < yEnd; y++)
    {
        V = (double)image[xo+y] + PrevV;
        table[xo + y] = V;
        PrevV = V;
    }

    int xom1;
    // Loop through other regions
    for (y = yStart+1; y < yEnd; y++)
    {
        for (x = xStart+1; x < xEnd; x++)
        {
            xo = x * h;
            xom1 = (x-1) * h;

            V = table[xom1 + y ] +
                table[xo + (y - 1) ] -
                table[ xom1 + (y - 1)] +
                (double)image[xo+y];
            table[xo + y ] = V;
        }
    }


}


int FindHoriztonalCenterofMass (/* define the window we are operating in: */ int xStart, int xEnd, int yStart, int yEnd )
{
    int h;
    int x,xo;
  
   int boxW, boxH;

    double boxOverlayValue, maxValue;
    int xCenterOfMass;

    // define the four corners of the box when at any given location within the window:
    int X0, X1;
    int yCenter, yHalf;

  
    h = libData.imageHeight;

    boxW = MIN_PLATE_WIDTH;
    boxH = MIN_PLATE_HEIGHT;

  
    // slide the box from left to right, 
    // get the total edge energy in that box and record that for each X position in the slide

    // slide the box down the center horizontal line of the window
    yHalf = (yEnd - yStart )/2;
    yCenter = yStart + yHalf;

    maxValue = 0;
    xCenterOfMass = (xEnd- xStart)/2; // default value

    for ( x = xStart; x < xEnd; x++)
    {
        X0 = x;
        X1 = x + boxW;

        xo = x * h;

        boxOverlayValue = SampleIntegralImage(X0, yCenter - (boxH/2), X1, yCenter + (boxH/2));

        if ( boxOverlayValue > maxValue )
        {
            maxValue = boxOverlayValue;
            xCenterOfMass = x;
        }

    }


    // return that x position


    return(xCenterOfMass + (boxW/2));
}



void findExactEdges (LPROCR_lib_PLATE_LOCATION *pl, int * error)
{
    DIAGCOLOR color;

    int s1, s2, s3;
    int yPeakHorizontal;
    int x,y,xo;
    int xStart, xEnd;
    int lineIndexAtPeak;

    int peakVal;

    int secondOrder;
    int threshold ;  // used for different thesholds as the algorithm progresses

    int lineIndex;  // used to index through local buffers that are just MAX_PLATE_WIDTH long
    int colIndex;

    int *leftEdge;
    int *rightEdge;
    int *topEdge;
    int *bottomEdge;


    // remeber the outer edge of the search window
    int plateWinLeftEdge ;
    int plateWinRightEdge ;
    int plateWinTopEdge ;
    int plateWinBottomEdge;


    int maxPlateWidth = MAX_PLATE_WIDTH;
    int maxPlateHeight = MAX_PLATE_HEIGHT;

    int lineSum [MAX_PLATE_HEIGHT+1];
    int cSum    [MAX_PLATE_WIDTH+1]; // sum of each column


    int peakLineSumVal;


    // translate the coordinates back to full image coordinates

    pl->centerX = pl->centerX  * X_DIVISOR;
    pl->centerY = pl->centerY  * Y_DIVISOR;

    if ( pl->centerX   - (MAX_PLATE_WIDTH / 2) <= 0 ) pl->centerX =(MAX_PLATE_WIDTH / 2)+1; 

    if ( pl->centerX   + (MAX_PLATE_WIDTH / 2) >= libData.imageWidth ) 
        pl->centerX = libData.imageWidth - (MAX_PLATE_WIDTH / 2)-1; 


    if ( pl->centerY   - (MAX_PLATE_HEIGHT / 2) <= 0 ) pl->centerY =(MAX_PLATE_HEIGHT / 2)+1; 

    if ( pl->centerY   + (MAX_PLATE_HEIGHT / 2) >= libData.imageHeight ) 
        pl->centerY = libData.imageHeight - (MAX_PLATE_HEIGHT / 2)-1; 

    pl->leftEdge = pl->centerX   - (MAX_PLATE_WIDTH / 2 );
    pl->rightEdge = pl->centerX  + (MAX_PLATE_WIDTH / 2 );
    pl->topEdge = pl->centerY    - (MAX_PLATE_HEIGHT / 2 );
    pl->bottomEdge = pl->centerY + (MAX_PLATE_HEIGHT / 2 );



    if ( pl->leftEdge < 0 )
    {  // give up
        setEdgesToMinus1 (pl);
        return ;
    }


    if (  pl->leftEdge >= libData.imageWidth )  
    {  // give up
        setEdgesToMinus1 (pl);
        return ;
    }

    if (pl->rightEdge >= libData.imageWidth )
    {  // give up
        setEdgesToMinus1 (pl);
        return ;
    }

    if (pl->rightEdge < 0 )
    {  // give up
        setEdgesToMinus1 (pl);
        return ;
    }


    if ( pl->topEdge < 0 )     
    {  // give up
        setEdgesToMinus1 (pl);
        return ;
    }

    if (pl->topEdge >= libData.imageWidth)     
    {  // give up
        setEdgesToMinus1 (pl);
        return ;
    }

    if (pl->bottomEdge >= libData.imageHeight ) 
    {  // give up
        setEdgesToMinus1 (pl);
        return ;
    }

    if (pl->bottomEdge < 0  )            
    {  // give up
        setEdgesToMinus1 (pl);
        return ;
    }




    // these edges start out as the outer limits of the plate search window
    // but as the algorithm proceeds down, these limits move inward to the true plate
    leftEdge = & pl->leftEdge;
    rightEdge = & pl->rightEdge;
    topEdge = & pl->topEdge;
    bottomEdge = & pl->bottomEdge;


    // remeber the outer edge of the search window
    plateWinLeftEdge = *leftEdge;
    plateWinRightEdge = *rightEdge;
    plateWinTopEdge = *topEdge;
    plateWinBottomEdge = *bottomEdge;


    if ((*rightEdge - *leftEdge < MIN_PLATE_WIDTH) || (*bottomEdge - *topEdge < MIN_PLATE_HEIGHT))
    {  // give up
        setEdgesToMinus1 (pl);
        return ;
    }


    //if ((*rightEdge - *leftEdge > MAX_PLATE_WIDTH) || (*bottomEdge - *topEdge > MAX_PLATE_HEIGHT))
    //{  // give up
    //   setEdgesToMinus1 (pl);
    //   return ;
    //}

   
    secondOrder = 0;  // do not add adjacent pixel edge energy




               ////        Create the High Resolution  Edge Contour Map
               ///
               ///
               ///

    createEdgeContourMap(libData.imageFullRes, plateWinLeftEdge , plateWinTopEdge, 
        plateWinRightEdge, plateWinBottomEdge,
        libData.edgeMapFullRes,
        libData.imageWidth, libData.imageHeight, secondOrder, error);

    if ( *error != 0 )
    {
        setEdgesToMinus1 (pl);
        return;
    }

    if ( libData.diagEnabled == 1 ) LPROCR_diags_putEdgeMapFullResImage( plateWinLeftEdge , plateWinTopEdge, plateWinRightEdge, plateWinBottomEdge);


    

                 ///     Start Looking for More Exact TOP and BOTTOM  of plate
                 ///
                 ///

    // find top and bottom edges of the plate by looking at the horizontal sums of edge energy

  
    // when looking for top/bottom, ignore egdge energy on the outer left and right extremes of the window
 //   xStart = plateWinLeftEdge + (MAX_PLATE_WIDTH/4);
    xStart = plateWinLeftEdge;
    if ( xStart < 0 )
    {  // give up
        setEdgesToMinus1 (pl);
        return ;
    }

    if ( xStart >= libData.imageWidth)
    {  // give up
        setEdgesToMinus1 (pl);
        return ;
    }

  //  xEnd   = plateWinRightEdge - (MAX_PLATE_WIDTH/4);
      xEnd   = plateWinRightEdge ;

    if ( xEnd >= libData.imageWidth)
    {  // give up
        setEdgesToMinus1 (pl);
        return ;
    }

    if ( xEnd < 0 )
    {  // give up
        setEdgesToMinus1 (pl);
        return ;
    }

    peakLineSumVal = -999999999;

    lineIndex = 0;
    lineIndexAtPeak = 0;

     
            /// sum up edge energy horizontally

    for (y = plateWinTopEdge; y < plateWinBottomEdge; y++)
    {
        xo = plateWinLeftEdge * libData.imageHeight;
        lineSum[lineIndex] = 0;


        for (x = xStart; x < xEnd; x++)
        {
            lineSum[lineIndex] += *(libData.edgeMapFullRes + xo + y);

            if (lineSum[lineIndex] > peakLineSumVal) 
            {
                peakLineSumVal = lineSum[lineIndex];
                lineIndexAtPeak = lineIndex;
            }

            xo += libData.imageHeight;
        }
        lineIndex++;
    }


    // DRAW THE HORIZONTAL CENTER LINE of the max size plate window

    if ( libData.diagEnabled == 1  )
    {
        color.blue = 0;
        color.green = 0;
        color.red = 255;
        plotLine (  1, *leftEdge, *rightEdge, maxPlateHeight/2+10 + *topEdge, maxPlateHeight/2 + *topEdge+10, &color );

        // plot the line sums
        
        LPROCR_diags_plotHorizontalLineSums (lineSum,plateWinTopEdge, plateWinBottomEdge - plateWinTopEdge , xStart,  xEnd);
    }


    // use a sample line, hopefully throuhg the thick of the text, to derive the threshold of a line consiting mostly
    //    of white space. at this point we do not know where the center of the text is exactly.
    // for small plates, the center line may fall above or below the plate text since the starting window is so large
    //  so check the largest of three sample lines

    // the vertical offset of 10 pixels is due to the edge convolution algorithm which pull energy down
    // by 10 pixels

    s1= lineSum[maxPlateHeight/3+10];
    s2= lineSum[(maxPlateHeight/2)+10];
    s3= lineSum[ ((2 * maxPlateHeight)/3)+10];

    // default value if a better one is not found below 
    yPeakHorizontal = maxPlateHeight/3;
    threshold = (60 * s1) / 100;

    // look for optimal value

    if ( s1 > s2 && s1 > s3 )
    {
        yPeakHorizontal = maxPlateHeight/3+10;
        threshold = (60 * s1) / 100;
    }

    if ( s2 > s1 && s2 > s3 )
    {      
        yPeakHorizontal = (maxPlateHeight/2)+10;
        threshold = (60 * s2) / 100;
    }

    if ( s3 > s1 && s3 > s2 )
    {
        yPeakHorizontal = ((2 * maxPlateHeight)/3)+10;
        threshold = (60 * s3) / 100;
    }


    // top edge


    *topEdge = plateWinTopEdge;

    for (lineIndex = yPeakHorizontal ; lineIndex > 0 ; lineIndex--)
    {
        if (lineSum[lineIndex] < threshold)
        {
            *topEdge = (lineIndex + plateWinTopEdge) - 15;
            break;
        }
    }

    if (*topEdge < 0) *topEdge = 0;


    //   BOTTOM EDGE

    *bottomEdge = *topEdge  + maxPlateHeight  - 1;// incase we dont find a real one, this is an estimate

    for (lineIndex = yPeakHorizontal ; lineIndex < maxPlateHeight ; lineIndex++) 
    {
        if (lineSum[lineIndex] < threshold)
        {
            *bottomEdge = (lineIndex + plateWinTopEdge) + 15;
            break;
        }
    }


    if ( *bottomEdge >= libData.imageHeight) *bottomEdge = libData.imageHeight - 1;


    // plot the top edge line
    if ( libData.diagEnabled == 1  )
    {
        color.blue = 150;
        color.green = 150;
        color.red = 0;

        plotLine ( 1, *leftEdge, *rightEdge, *topEdge, *topEdge, &color );
    }

    // plot the bottom edge line
    if ( libData.diagEnabled == 1  )
    {
        plotLine (  1, *leftEdge, *rightEdge, *bottomEdge, *bottomEdge, &color );
    }

    // draw a vertical line through the middle of the plate region
    if ( libData.diagEnabled == 1  )
    {
        plotLine ( 1, (*rightEdge + *leftEdge)/2, (*rightEdge + *leftEdge)/2, *topEdge, *bottomEdge, &color );
    }



    ///
         ///   Start looking for the more exact LEFT and RIGHT edges of the plate
    ///



    CreateIntegralImage( plateWinLeftEdge, plateWinRightEdge,  *topEdge-1, *bottomEdge+2);


    int xCenterOfMass;

    xCenterOfMass = FindHoriztonalCenterofMass ( plateWinLeftEdge, plateWinRightEdge,  *topEdge, *bottomEdge );

     // draw the center of mass line

    if ( libData.diagEnabled == 1 )
    {
        color.blue  = 150;
        color.green = 150;
        color.red   = 255;

        plotLine ( 1, xCenterOfMass, xCenterOfMass, *topEdge, *bottomEdge, &color );
    }


    // get the colunm sums
    int blockWidth = 40;

    sumCols (   blockWidth, plateWinLeftEdge, plateWinRightEdge, *topEdge, *bottomEdge, cSum, &peakVal );


    // left edge

    threshold = peakVal / 5;  




    // start in the xCenterOfMass, move to the left
   
  
    for (colIndex = xCenterOfMass -  plateWinLeftEdge ; colIndex > 0 ; colIndex--)
    {
        if (cSum[colIndex] < threshold)
        {
            *leftEdge = plateWinLeftEdge + colIndex;
            break;
        }
    }

    *leftEdge += (blockWidth /2); // the sumCols groups column into blocks 20 pixe wide, and reports the result for the leftmost column.

    if (*leftEdge < 0) *leftEdge = 0;

    // draw the left edge
    if ( libData.diagEnabled == 1  )
    {
        color.blue = 0;
        color.green = 0;
        color.red = 255;
        plotLine (  1, *leftEdge, *leftEdge, *topEdge, *bottomEdge, &color );
    }



    // right edge


    // start in the xCenterOfMass, move to the right

  

    for (colIndex = xCenterOfMass -  plateWinLeftEdge ; colIndex < maxPlateWidth ; colIndex++)
    {
        if (cSum[colIndex] < threshold)
        {
            *rightEdge =  plateWinLeftEdge + colIndex;
            break;
        }
    }

    *rightEdge += (blockWidth /2);

    // make the plate are  a little wider & taller to accomodate highly tilted/rotated plates
    *rightEdge = *rightEdge + 5;
    if (*rightEdge < 0) *rightEdge = 0;
    if (*rightEdge >= libData.imageWidth ) *rightEdge = libData.imageWidth-1;

    *leftEdge = *leftEdge - 5;
    if (*leftEdge < 0) *leftEdge = 0;
    if (*leftEdge >= libData.imageWidth ) *leftEdge = libData.imageWidth-1;

    *topEdge = *topEdge - 10;
    if (*topEdge < 0) *topEdge = 0;
    if (*topEdge >= libData.imageHeight ) *topEdge = libData.imageHeight-1;

    *bottomEdge = *bottomEdge + 10;
    if (*bottomEdge < 0) *bottomEdge = 0;
    if (*bottomEdge >= libData.imageHeight ) *bottomEdge = libData.imageHeight-1;




    // draw the right edge
    if ( libData.diagEnabled == 1 )
    {
        color.blue = 0;
        color.green = 255;
        color.red = 0;

        plotLine ( 1, *rightEdge, *rightEdge, *topEdge, *bottomEdge, &color );
    }

    if ( *rightEdge -  *leftEdge < MIN_PLATE_WIDTH)
    {  // give up
        setEdgesToMinus1 (pl);
        return ;
    }


    if ( *bottomEdge -  *topEdge < MIN_PLATE_HEIGHT)
    {  // give up
        setEdgesToMinus1 (pl);
        return ;
    }


    // sometimes the above method cannot find the true left and right plate edges resulting in a very long plate.
    // if this happens, use this method to resolve

    //if ( (*rightEdge -  *leftEdge ) / (  *bottomEdge - *topEdge ) > 6  || (*rightEdge -  *leftEdge ) >= MAX_PLATE_WIDTH)
    //{

    //  // test for speed  fineTuneLeftRightEdges ( leftEdge, rightEdge, topEdge, bottomEdge );

    //}
    //


    // get rid of some false plates:
    //   a real plate is wider than it is tall (except for certain UK square plates, which I am not handling now
    //   hopefully in the future develop better methods for rejecting false plates

    if ( *leftEdge != -1 && ( *bottomEdge - *topEdge) != 0 )
    {
        if (  100 *(*rightEdge - *leftEdge) / (*bottomEdge - *topEdge) > 20 )
        {
            // it is a plate
        }
        else  // its not a plate
        {
            *leftEdge = -1;
            *rightEdge = -1;
        }
    }



} // end findExactEdges



void fineTuneLeftRightEdges ( int * leftEdge, int *rightEdge,int *topEdge, int *bottomEdge, int * error)
{

    int w, h;
    int x, y;
    int j;
    float * correlationScores;
    //  float  correlationScores[1280];
    int * subImage=0;
    int sw, sh;
    float min;
    float max;
    int xAtMax=0;
    float threshold;
    int startingLeftEdge;

    startingLeftEdge = *leftEdge;

    sw = *rightEdge - *leftEdge;
    sh = *bottomEdge - *topEdge;

    w = libData.imageWidth;
    h = libData.imageHeight;

    correlationScores = (float *) malloc ( sw * sizeof(float) );
    if ( correlationScores == 0 )
    {
        *error = 1;
        goto done;
    }

    subImage = (int *) malloc (  sw * sh * sizeof(int) );
    if ( subImage == 0 )    
    {
        *error = 1;
        goto done;
    }

    extractSubImage(libData.imageFullRes,  w, h,  *leftEdge, *rightEdge,  *topEdge, *bottomEdge,  subImage);

    normalizeToWideSpectrum( subImage,  sw,  sh,  error);

    for ( x = 0; x < sw; x++) correlationScores[x] = 0;

    // run a correlation with all 1's (simple sum) of blocks about the size of typical characters
    //  if the area is a flat surface such as a bumper, the sum will be high, but  if its a plate
    //  number area then the negatives and postives of black and white area will cancel and reduce
    //  the correlation score. look for large change in score to mark the edges of the plate

    y = sh/2;

    for ( x = 0; x < sw; x += 30 )
    {
        correlationScores[x] = correlateFlat ( subImage, sw, sh, x , y );
        for ( j=0; j < 30 && x+j < sw; j++)
            correlationScores[x+j] = correlationScores[x];
    }

    min = 99999999999.f;
    max = 0;

    for ( x = 0; x < sw; x ++ )
    {
        if ( correlationScores[x] < min )
        {
            min = correlationScores[x];
        }
        if ( correlationScores[x] > max ) 
        {
            xAtMax = x;
            max = correlationScores[x];
        }
    }

    threshold = ( min + max ) / 4;

    for ( x =xAtMax; x > 0 ; x -- )
    {
        if ( correlationScores[x] < threshold) 
        {
            *leftEdge = startingLeftEdge + x -10 ;
            if ( *leftEdge < 0 ) *leftEdge = 0;
            break;
        }
    }

    for ( x = xAtMax; x < sw ; x ++ )
    {
        if ( correlationScores[x] < threshold) 
        {
            *rightEdge = startingLeftEdge + x ;
            if ( *rightEdge >= w ) *rightEdge = w-1;
            break;
        }
    }



done:
    if  ( correlationScores != 0  ) free ( correlationScores );
    if ( subImage != 0 )            free ( subImage );
}




float correlateFlat ( int * image, int w, int h, int xCenter, int yCenter )
{
    // sum edge energy in a block about the size of a typical character

    int x,xo,x1o, y;
    int xStart, xEnd;
    int yStart, yEnd;
    float correlationScore=0;

    if ( xCenter - 30 < 0 ) return (0);
    if ( xCenter + 30 >= w ) return(0);

    xStart = xCenter - 25;
    xEnd = xCenter + 25;
    yStart = yCenter - 15;
    yEnd = yCenter + 15;

    LIMIT(xStart,w);
    LIMIT(xEnd,w);
    LIMIT(yStart,h);
    LIMIT(yEnd,h);

    for ( x = xStart; x < xEnd-2; x++)
    {
        xo = (x) * h;
        x1o = (x+2) * h;
        for (y = yStart; y < yEnd; y++)
        {
            if ( abs( *(image + xo + y)  - *(image + x1o + y)) > 100 )
                correlationScore ++;
        }
    }

    return ( correlationScore);
}




void setEdgesToMinus1 (LPROCR_lib_PLATE_LOCATION *pl)
{
    pl->leftEdge = -1;
    pl->rightEdge = -1;
    pl->bottomEdge = -1;
    pl->topEdge = -1;
}




//
// method sumCols
//
void   sumCols (int blockWidth,
                int leftEdge, int rightEdge,
                int topEdge, int bottomEdge, int * colSum, int * peakVal )
{
   

    //   slide a narrow box (20 pixels wide) across the plate window
    //     getting an area energy sum for each box position 

    int cIndex;
  
    int x;
   
   
    int boxW;
    int boxH;
  

    // define the four corners of the box when at any given location within the window:
    int X0, X1;
  

    
    *peakVal = 0;


   
    boxW = blockWidth;
    boxH = bottomEdge - topEdge;

  


    // slide the box from left to right, 
    // get the total edge energy in that box and record that for each X position in the slide

    
    cIndex = 0;

    for ( x = leftEdge; x < rightEdge; x++)
    {
        X0 = x;
        X1 = x + boxW;


        colSum[cIndex] = (int) SampleIntegralImage(X0, topEdge, X1, bottomEdge);
        
        if ( colSum[cIndex]> *peakVal) *peakVal =  colSum[cIndex];

        cIndex ++;

    }



}

//void   sumCols ( int * edgeMapFullRes,  int h, 
//                int leftEdge, int rightEdge,
//                int topEdge, int bottomEdge, int * colSum, int * peakVal )
//{
//    int x, y, xo, cIndex;
//
//    *peakVal = 0;
//
//    cIndex = 0;
//    xo = leftEdge * h;
//    for (x=leftEdge; x < rightEdge; x++)
//    {
//        *(colSum+cIndex) = 0;
//        for (y = topEdge; y < bottomEdge; y++)
//        {
//            *(colSum+cIndex) += *(edgeMapFullRes + xo + y );
//        }
//
//        if ( *(colSum+cIndex) > *peakVal ) *peakVal = *(colSum+cIndex);
//
//        xo += h;
//        cIndex ++;
//    }
//
//}


//
// method createEdgeContourMap
//

void createEdgeContourMap( int *image, int xStart, int yStart, int xEnd, int yEnd, 
                          int *edgeMap, 
                          int w,   // image width 
                          int h,   // image height
                          int secondOrder, // should add in adjacent egde energy?
                          int * error)
{




    int * Vedge;

    int x = 0;
    int xo = 0;
    int y = 0;
    int xx = 0;
    int xLimit = 0;

    Vedge = (int *) malloc ( sizeof(int) * w );
    if ( Vedge == 0 ) 
    {
        *error = ERROR_memoryNotAllocated  ;
        goto done;
    }



    if ( xStart >=w || xEnd > w || yStart >= h || yEnd > h)
        goto done;



    if ( xStart <0 || xEnd <0 || yStart <0 || yEnd <0)
        goto done;



    for (y = 0; y < h; y++)
    {

        for (x = 0; x < w; x++)
        {
            xo = x  * h;

          // if ( libData.diagEnabled == 0 )  *(edgeMap+ xo + y) = 0; // clear the edge map
             *(edgeMap+ xo + y) = 0; // clear the edge map

            if ( x < xStart || x > xEnd || y < yStart || y > yEnd)
            {
                *(Vedge+x) =0;
                continue;
            }

             // see all the candidate plates edge data, can affect results
           // if ( libData.diagEnabled == 1 ) *(edgeMap+ xo + y) = 0; // clear the edge map

            *(Vedge+x) = runVerticalEdgeFinderConvolution( x, y, 1, 10, w, h, image);

            // sum the horizontally adjacent edge enegery to create bright blobs where there is a lot of edge intensity in an area
            if (*(Vedge+x) > 100)// was 400
            {

                if (secondOrder)
                {
                    xLimit = 0;
                    xx = 0;

                    for (xx = x - 50; xx < x + 1; xx++)
                    {
                        xLimit = xx;
                        if (xLimit < 0) xLimit = 0;
                        if (xLimit >= w) xLimit = w - 1;

                        *(edgeMap+ xo + y) += *(Vedge+xLimit);
                    }
                }
                else
                {
                    *(edgeMap+ xo + y) = *(Vedge+x);
                }
            }
            else
            {
                *(edgeMap+ xo + y) = 0;
            }
            // xo += h;
        }// end for(x



    }// end for(y

done:
    freeMemory ( &  Vedge);

}




//
// method runVerticalEdgeFinderConvolution
//

int runVerticalEdgeFinderConvolution(int xStart, int yStart, int blockW, int blockH, 
                                     int w,
                                     int h,
                                     int *image)
{

    int y1 = 0;
    int y2 = 0;

    int x;
    int xo;
    int xoPlus1;
    int y = 0;
    int blockSum = 0;


    int ss1 = 0;

    int xEnd = xStart + blockW;
    int yEnd = yStart + blockH;

    int yLimit = 0;

    if (xStart + 2 >= w)
    {
        return(0);
    }

    if (xEnd + 2 > w) xEnd = w - 2;

    if (yStart + 2 >= h) yStart = h - 3;

    if (yEnd + 2 > h) yEnd = h - 2;;

    xo = (xStart * h);
    xoPlus1 = xo + h;

    for (x = xStart; x < xEnd; x++)
    {

        for (y = yStart; y < yEnd; y++)
        {

            yLimit = y;
            if (yLimit >= h) yLimit = h - 1;

            y1 = *(image + xo + yLimit);

            if (y1 > 400)
                y1 = 2000;

            if (y1 < -400)
                y1 = -2000;

            y2 = *(image + xoPlus1 + yLimit);

            if (y2 > 400)
                y2 = 2000;

            if (y2 < -400)
                y2 = -2000;

            ss1 += y2 - y1;

            if (ss1 < 0) ss1 = -1 * ss1;

            blockSum += ss1;
        }

        xo += h;
        xoPlus1 += h;
    }


    return (blockSum);

}// end method runVerticalEdgeFinderConvolution



//
// method findBrightestSpot ()
//
void findBrightestSpot(int index, int * image, int w, int h, int * XatMax, int * YatMax)
{
    int x = 0;
    int xo = 0;
    int y = 0;
    //  int maxPlateHeightFactor = MAX_PLATE_HEIGHT / Y_DIVISOR;
    //  int maxPlateWidthFactor = MAX_PLATE_WIDTH / X_DIVISOR;

    int xStart = w / 10;
    int xEnd = (90 * w) / 100;

    int yStart = (h / 10);
    int yEnd = (90 * h) / 100;

    int maxEdgeEnergy = 0;

    *XatMax = -1;
    *YatMax = -1;



    for (y = yStart; y < yEnd; y++)
    {
        for (x = xStart; x < xEnd; x++)
        {
            xo = x * h;

            if (maxEdgeEnergy < *(image + xo + y))
            {
                maxEdgeEnergy = *(image + xo + y);
                *XatMax = x - 20;  // it is alway skewed to the right, need to correct
                *YatMax = y;
            }
        }
    }

    if ( *YatMax == -1 )
    {
        * XatMax =-1;
      
        RejectLogAddPlateIndexLocation(index,0, 0,"could not find any more bright spots");

        return;
    }

    RejectLogAddPlateIndexLocation(index,* XatMax, *YatMax ,"found a bright spot");

    if ( maxEdgeEnergy < 10000) //30,000 
    {
        * XatMax =-1;
        * YatMax =-1;
          RejectLogAddPlateIndexLocation(index,* XatMax, *YatMax ,"bright spot under threshold, reject");
        return;
    }

    if ( *XatMax < 0 ) *XatMax = 0;
    if ( *XatMax >= w ) *XatMax = w - 1;

    if ( *YatMax < 0 ) *YatMax = 0;
    if ( *YatMax >= h ) *YatMax = h - 1;

    // for second pass plate searches, dont want this one to show up again, so zero out this spot
    // and adjacent area


    xStart = *XatMax - (w / 4);
    if (xStart < 0) xStart = 0;

    xEnd = *XatMax + (w / 4);
    if (xEnd > w - 1) xEnd = w - 1;

    yStart = *YatMax - (h / 4);
    if (yStart < 0) yStart = 0;

    yEnd = *YatMax + (h / 4);
    if (yEnd > h - 1) yEnd = h - 1;

    // once we find the brightess spot, zero it out so the second pass
    // will not find this one again
    for (x = xStart; x < xEnd; x++)
    {
        xo = x * h;
        for (y = yStart; y < yEnd; y++)   
        {
            *(image + xo + y) = 0;
        }
    }


}



//
// method sharpen
//
void LPROCR_lib_sharpen ( int * image, int w, int h )
{
    // this is a low memory implementation, do not create
    // a duplicate of the entire image.
    //   only create a temp buffer of filter-block size to hold
    //   results until the fitler block passes, then copy the results
    // back to the source array

    int x,y;
    int Xm2, Xm1, X0, X1;
    int Ym2, Ym1, Y0, Y1;
    int output, output1, output2;
    int filterSum;
    int maxValue=0;
    int minValue=99999999;

    output1 = 0;
    output2 = 0;


    // fx_y = filter[x,y]   f= filter,  m = minus

    int fm1_m1, f0_m1, f1_m1;
    int fm1_0,  f0_0,  f1_0;
    int fm1_1,  f0_1,  f1_1;

    // the filter:
    fm1_m1 = -1;  f0_m1 = -1;  f1_m1 = -1;
    fm1_0 = -1;    f0_0 = 10 ;  f1_0 = -1;
    fm1_1 = -1;    f0_1 = -1;   f1_1 = -1;

    //filterSum = 1;
    filterSum = fm1_m1 +  f0_m1 +  f1_m1 +
        fm1_0  +  f0_0  +  f1_0  +
        fm1_1  +  f0_1  +  f1_1;

    for ( x = 2; x < w - 1; x++)
    {

        Xm2 = (x - 2) * h; // update the source image pixel after we are finshed using it for the filter computation

        Xm1 = (x - 1) * h;
        X0  = (x) * h;
        X1 = (x+1) * h;

        for ( y = 2; y < h - 1; y++)
        {


            Ym2 = (y - 2);// update the source image pixel after we are finshed using it for the filter computation

            Ym1 = (y - 1);
            Y0  = (y);
            Y1 = (y+1);

            if ( *(image + X0 + Y0) > maxValue ) maxValue = *(image + X0 + Y0);
            if ( *(image + X0 + Y0) < minValue ) minValue = *(image + X0 + Y0);


            output = (  (*(image + Xm1 + Ym1) * fm1_m1)+ (*(image + X0 + Ym1)* f0_m1)+ (*(image + X1 + Ym1)* f1_m1)+
                (*(image + Xm1 + Y0)  * fm1_0)+  (*(image + X0 + Y0) * f0_0) + (*(image + X1 + Y0) * f1_0)+
                (*(image + Xm1 + Y1)  * fm1_1)+  (*(image + X0 + Y1) * f0_1) + (*(image + X1 + Y1) * f1_1 )

                ) / filterSum;

            output2 = output1;
            output1 = output;

            if ( output2 > maxValue ) output2 = maxValue;
            if ( output2 < minValue ) output2 = minValue;

            *(image + Xm2 + Ym2) = output2;

        }

    }


}


char * getErrorString(int error)
{
    return (ERROR_CODE_STRINGS[error]);
}



// used to extract the video frame from the fitler graph video sample callback to a lumiance array for LPR processing

void LPROCR_lib_extractFromBmpDataToLumArray( int * srcPtr, int * dstPtr, 
                                             int stride, int width, int height, int invert)
{


    int pixelOffset = stride / width;


    char * rgbValues;

    rgbValues = (char *) srcPtr;

    int x = 0;
    int y = 0;
    int b = 0;
    int bv = 0;
    int rv = 0;
    int gv = 0;

    int xOffset = 0;


    if ( ! invert )
    {
        for (y = 0; y < height; y++)
        {
            for (x = 0; x < width; x++)
            {
                xOffset = x * height;

                bv = ((int)rgbValues[b+2] & 0xff) * 114;
                gv = ((int)rgbValues[b + 1]& 0xff) * 587;
                rv = ((int)rgbValues[b ]& 0xff) * 299;

                *(dstPtr+xOffset +y ) = ((((bv + rv + gv) / 1000))) & 0xff;

                b += pixelOffset;
               
               
            }
             //ptr += data.Stride - data.Width * 3;
             b += stride - (  width *  pixelOffset); // for cases where there is extra space inserted in the rows to make it a factor of 4
        }
    }
    else 
    {
        for (y = height-1; y >= 0 ; y--)
        {
            for (x = 0; x < width; x++)
            {
                xOffset = x * height;

                bv = ((int)rgbValues[b+2] & 0xff) * 114;
                gv = ((int)rgbValues[b + 1]& 0xff) * 587;
                rv = ((int)rgbValues[b ]& 0xff) * 299;

                *(dstPtr+xOffset +y ) = ((((bv + rv + gv)))/1000) & 0xff;

              
                b += pixelOffset;
               
            }
            b += stride - (  width *  pixelOffset); // for cases where there is extra space inserted in the rows to make it a factor of 4
        }



    }

}



int LPROCR_lib_getMaxCandidatePlates ( )
{
    return ( MAX_CANDIDATE_PLATES );
}

int LPROCR_lib_getMaxActualPlates ( )
{
    return ( MAX_ACTUAL_PLATES );
}


void extractSubImage(int* largeImage, int w, int h, int leftEdge, int rightEdge, int topEdge,int bottomEdge, int * subImage)
{
    int x = 0;
    int xo = 0;
    int y = 0;
    int nx = 0;
    int ny = 0;
    int subHeight;

    if ( leftEdge < 0 ) return;
    if ( topEdge < 0 ) return;
    if ( bottomEdge >= h ) return;
    if ( rightEdge >= w ) return;

    subHeight = bottomEdge - topEdge ;

    nx = 0;
    for (x = leftEdge; x < rightEdge; x++)
    {
        ny = 0;   
        xo = x * h;
        for (y = topEdge; y < bottomEdge; y++)
        {
            subImage[nx + ny] = largeImage[xo + y];
            ny++;
        }
        nx += subHeight;
    }

}

void freeMemory(int **p) 
{
    if(*p!=0)
    {
        free(*p);
        *p=0;
    }
}

void freeMemoryDouble(double **p) 
{
    if(*p!=0)
    {
        free(*p);
        *p=0;
    }
}





void LPROCR_lib_GetCharImageLuminance(int plateIndex, int cIndex, int * lumArray, int useCandidateList)
{

   // lumArray needs to be pre-allocated 

   int cleft = 0, cright = 0, ctop = 0, cbottom = 0; // char edge locations relative to the plate

   LPROCR_lib_getCharLocation (plateIndex, cIndex, & cleft, & cright, & ctop, & cbottom , useCandidateList );

   if (  useCandidateList == 1 )
   {
      extractSubImage(libData.foundPlates[plateIndex].image, 
         libData.foundPlates[plateIndex].width ,
         libData.foundPlates[plateIndex].height, 
         cleft, cright, ctop,  cbottom, lumArray);
   }
   else
   {
      extractSubImage(libData.finalPlateList[plateIndex].image, 
         libData.finalPlateList[plateIndex].width ,
         libData.finalPlateList[plateIndex].height, 
         cleft, cright, ctop,  cbottom, lumArray);
   }


}



void LPROCR_lib_GetCharImageRGBArray(int plateIndex, int cIndex, byte * rgbValues, int width, int height, int rgbLength, int pixelOffset)
{

    int x,xo,y;

    int *src;
    int bi;
    int cw, ch;
  

    int cleft = 0, cright = 0, ctop = 0, cbottom = 0; // char edge locations relative to the plate

    if ( rgbValues == 0 ) return;

    LPROCR_lib_getCharLocation (plateIndex, cIndex, & cleft, & cright, & ctop, & cbottom, 0   );
    cw = cright - cleft;
    ch = cbottom - ctop;

    src = (int *) malloc ( sizeof(int) * cw * ch );
    if ( src == 0 )
    {
        return;
    }

  


    extractSubImage(libData.finalPlateList[plateIndex].image, 
                        libData.finalPlateList[plateIndex].width ,
                        libData.finalPlateList[plateIndex].height, 
                        cleft, cright, ctop,  cbottom, src );


    bi=0;
    for (y = 0; y < height; y++)
    {
        for (x = 0; x < width; x++)
        {
            xo = x * height;
            if ( bi >= rgbLength-pixelOffset ) return;
            rgbValues[bi] = (byte)src[xo+y];
            rgbValues[bi + 1] = (byte)src[xo+y];
            rgbValues[bi + 2] = (byte)src[xo+y];
            if ( pixelOffset == 4 ) rgbValues[bi + 3] = (byte) 255; // alpha
            bi += pixelOffset;
        }
        while (bi % 4 != 0) bi++;  
    }

    freeMemory ( & src);

}






void LPROCR_lib_GetCandidatePlateImageRGBArray(int plateIndex, byte * rgbValues, int width, int height, int rgbLength, int pixelOffset)
{

    int x,xo,y;

    int *src;
    int bi;


    src = libData.foundPlates[plateIndex].image;
    if ( src == 0 ) return;

    if ( rgbValues == 0 ) return;

    rgbLength = rgbLength-4;

    bi=0;
    for (y = 0; y < height; y++)
    {
        for (x = 0; x < width; x++)
        {
            xo = x * height;
            if ( bi >= rgbLength-3 ) return;
            rgbValues[bi] = (byte)src[xo+y];
            rgbValues[bi + 1] = (byte)src[xo+y];
            rgbValues[bi + 2] = (byte)src[xo+y];
            if ( pixelOffset == 4 ) rgbValues[bi + 3] = (byte) 255; // alpha
            bi += pixelOffset;
        }
        while (bi % 4 != 0) 
            bi++;


    }

}



void LPROCR_lib_GetPlateImageRGBArray(int plateIndex, byte * rgbValues, int width, int height, int rgbLength, int pixelOffset)
{

    int x,xo,y;

    int *src;
    int bi;


    src = libData.finalPlateList[plateIndex].image;
    if ( src == 0 ) return;

    if ( rgbValues == 0 ) return;

    rgbLength = rgbLength-4;

    bi=0;
    for (y = 0; y < height; y++)
    {
        for (x = 0; x < width; x++)
        {
            xo = x * height;
            if ( bi >= rgbLength-3 ) return;
            rgbValues[bi] = (byte)src[xo+y];
            rgbValues[bi + 1] = (byte)src[xo+y];
            rgbValues[bi + 2] = (byte)src[xo+y];
            if ( pixelOffset == 4 ) rgbValues[bi + 3] = (byte) 255; // alpha
            bi += pixelOffset;
        }
        while (bi % 4 != 0) 
            bi++;


    }

}




int quickPlateTest ( LPROCR_lib_PLATE_LOCATION *pl)
{

    int transitonCount =0;
    int x,y,xo,xo1;
    int V,V1;
    int aveCnt=0;
    int ave = 0;


    // translate the coordinates back to full image coordinates

    pl->centerX = pl->centerX  * X_DIVISOR;
    pl->centerY = pl->centerY  * Y_DIVISOR;

    if ( pl->centerX   - (MAX_PLATE_WIDTH / 2) <= 0 ) pl->centerX =(MAX_PLATE_WIDTH / 2)+1; 

    if ( pl->centerX   + (MAX_PLATE_WIDTH / 2) >= libData.imageWidth ) 
        pl->centerX = libData.imageWidth - (MAX_PLATE_WIDTH / 2)-1; 


    if ( pl->centerY   - (MAX_PLATE_HEIGHT / 2) <= 0 ) pl->centerY =(MAX_PLATE_HEIGHT / 2)+1; 

    if ( pl->centerY   + (MAX_PLATE_HEIGHT / 2) >= libData.imageHeight ) 
        pl->centerY = libData.imageHeight - (MAX_PLATE_HEIGHT / 2)-1; 

    pl->leftEdge = pl->centerX   - (MAX_PLATE_WIDTH / 2);
    pl->rightEdge = pl->centerX  + (MAX_PLATE_WIDTH / 2);
    pl->topEdge = pl->centerY    - (MAX_PLATE_HEIGHT / 2);
    pl->bottomEdge = pl->centerY + (MAX_PLATE_HEIGHT / 2);

    // count the black and white transitions
    transitonCount = 0;
    y = (pl->bottomEdge + pl->topEdge ) / 2;



    for (x =pl->leftEdge; x < pl->rightEdge  - 5; x++)
    {
        xo = x * libData.imageHeight ;
        V = *( libData.imageFullRes + xo + y ) & 255;
        ave += V;
        aveCnt ++;
    }


    ave = ave / aveCnt;

    for (x =pl->leftEdge; x < pl->rightEdge  - 5; x++)
    {
        xo = x * libData.imageHeight ;
        xo1 = (x+5) * libData.imageHeight ;

        if ( *( libData.imageFullRes + xo + y ) < ave-35 ) V = 0;
        else V = 255;

        if ( *( libData.imageFullRes + xo1  + y ) < ave-35 ) V1 = 0;
        else V1 = 255;

        if ( V != V1 ) transitonCount++;

    }

    if ( transitonCount > 10 && transitonCount < 100 )
        return(1);
    else
        return(0);

}


void CopyLineBackwards ( char * src,char *  dst, int stride, int width )
{
    int bytesPerPixel;

    int numPixels = width;
    int p ;
   
    int srcByteLocation;
    int dstByteLocation;

    bytesPerPixel = stride / width;

    dstByteLocation = 0;
    // srcByteLocation = (numPixels-1) * bytesPerPixel;
    srcByteLocation = 0;
    for ( p = numPixels -1 ; p >= 0 ; p --)
    {
        dst[dstByteLocation] = src[srcByteLocation ];
        dst[dstByteLocation+1] = src[srcByteLocation+1];
        dst[dstByteLocation+2] = src[srcByteLocation+2];
        dstByteLocation += bytesPerPixel;
        srcByteLocation += bytesPerPixel;
    }
}


void  _stdcall LPROCR_lib_MemCopy(char * src, char * dst, int stride, int width, int height )
{
    int srcLineCnt  =0;
    int dstLineCnt =0;

  
    int bytesPerPixel = 0;
    int lineCount =0;


    lineCount = height;
    bytesPerPixel = stride / width;


    // copying from a directshow buffer into a bitmap, 
    // need to invert image (top to bottom) and mirror reverse image (left to right)
    // to display in picture box correctly.

    // copy the last line into the first line, 
    //  copy the last pixel into the first

    dstLineCnt = lineCount  - 1;
    for ( srcLineCnt = 0; srcLineCnt < lineCount; srcLineCnt ++)
    {
        CopyLineBackwards ( (char*)& src[srcLineCnt * stride],(char*) & dst[dstLineCnt* stride],  stride,  width );
        dstLineCnt --;
    }

}


void  _stdcall LPROCR_lib_MemCopyInt(int * src, int * dst, int len )
{
    int i;

    for(i=0; i < len; i++)
        *(dst++) = *(src++);

}

void  _stdcall LPROCR_lib_MemCopyByte(unsigned char * src,unsigned char * dst, int len, int * detectedNoVideo )
{
    int i;
    char lastVal=0;
    *detectedNoVideo = 1;
    lastVal= *(src);

    for(i=0; i < len; i++)
    {
        if ( *(src) - lastVal != 0 ) *detectedNoVideo = 0;
        *(dst++) = *(src++);
        lastVal = *(src-1);
    }
}

void  _stdcall LPROCR_lib_MemCopyBytesToInts(unsigned char * src,unsigned int * dst, int len, int width, int height  )
{
    int i;
    int xo = 0;
    int x = 0; 
    int y = 0;

    // this maps a 8 bpp bitmap from the S2255 to a two dimnesional luminance array. 
    for(i=0; i < len; i++)
    {
        xo = x * height;
        *(dst + xo +y) = *(src++);
        x ++;
        if ( x ==  width) 
        {
            x = 0;
            y++;
        }
    }
}

void _stdcall LPROCR_lib_RegisterMotionDetectionCB(  void (*callback)())
{
    libData.MotionDetectedCallBack = callback;

}

#define MAX_CHANNELS 8
int * prevImage[MAX_CHANNELS];
bool FIRST_MOTION_DETECTION = true;

int _stdcall LPROCR_lib_DetectMotion (int chan, int * fullscale, int w, int h, int *error)
{

    int x,y, offset;
    int diff = 0;
    int delta;
    int threshold;
    int * subscaled;
    int sw=0;
    int sh=0;
    error = 0;

    if ( chan >= MAX_CHANNELS ) return 0;

    if ( FIRST_MOTION_DETECTION )
    {
        FIRST_MOTION_DETECTION = false;
        for (x = 0; x < MAX_CHANNELS; x++)
            prevImage[x] = 0;

     
    }

    SubScaleImageLocal( fullscale, w,  h, & subscaled, &sw, &sh, error);// mallocs memory for subscaled

    if ( prevImage[chan] == 0 )
    {
        prevImage[chan] = subscaled;// this is the first time called, load up the previous image and come back laters
        return 1;   // first frame must have motion detection true
    }

    offset = 0;
    for ( x=0; x < sw; x++)
    {
        offset = x * sh;
        for(y=0; y < sh; y++)
        {

            delta = *(prevImage[chan]+offset+y) - *(subscaled+offset+y);
            if ( delta > 50 )
                diff += abs (delta);
        }
    }

    freeMemory (  & prevImage[chan]);// free the memory from last frame

    prevImage[chan] = subscaled; // move current frame to last frame, there will always be one frame of unfreed memory hanging around

    threshold = (5 * 255 * libData.imageSubHeight * libData.imageSubWidth ) / 1000; // 0.5% of maximum possible diff value
    threshold = threshold / 4;

    if ( diff > threshold ) 
    {
        //	if ( libData.MotionDetectedCallBack !=  0 )
        //libData.MotionDetectedCallBack();
        return(1);
    }

    return(0);
}


void SubScaleImageLocal(int * fullscale, int w, int h, int ** subscaled, int *sw, int *sh, int *error)
{
    *sw = w  / X_DIVISOR;
    *sh = h / Y_DIVISOR;

    *subscaled= (int *) malloc ( sizeof(int) * (*sw) * (*sh)); 

    if ( *subscaled == 0 )
    {
        *error = ERROR_memoryNotAllocated ;
        return;
    }


    subscale ( fullscale, *subscaled, w, h, X_DIVISOR, Y_DIVISOR );

}


void LPROCR_lib_GetMinMaxPlateSize( int * minWidth, int * maxWidth,  int * minHeight, int * maxHeight)
{

    *minWidth = MIN_PLATE_WIDTH;
    *maxWidth = MAX_PLATE_WIDTH;
    *minHeight = MIN_PLATE_HEIGHT;
    *maxHeight = MAX_PLATE_HEIGHT ;

}