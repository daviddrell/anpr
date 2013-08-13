#include <afx.h>
#include <stdlib.h>
#include <malloc.h>
#include "LPROCR_Public.h"
#include "LPROCR_Structs.h"
#include "LPROCR_Error_Codes.h"
#include "LPROCR_Diags.h"
#include "charIsolationMethods.h"
#include "math.h"
#include "ocr.h"


//   change log
//
//  Sept 2, 2009, 11:50am, starting at  3334, changed binarization thresholds for histo-equalized images, for the case of the low contrast plate by NCIS in washington DC on a rainy friday night at the end of a long hot summer
//



extern libInstanceVariables libData;
bool m_CharIsoDebug;


//  find the chars in a plate image.
//   the caller keeps trac of the plate images - this function has no knowledge of other plates
//    
//   this is not thread safe/re-intrant safe. The plate image and diag plate image are held in a global variable while all the functions 
//    on this calling thread process the image. The next call into this fucntion will re-load the global image pointers to the new image/diag image.
//
//    libData.plateImage = plateImageSrc;
//    
int LPROCR_lib_findCharsInPlate ( int plateIndex,   int * error, bool diagsEnabled )
{


   int w;
   int h;
   int cw, ch;
   float roll;
   bool imageChanged = false;
   int cIndex = 0;
   int frameFound = 0;
   int aveCnt =0;
   int foundCount = 0;
   int * diagImage = 0;
   
   m_CharIsoDebug = diagsEnabled;

   diagImage = libData.foundPlates[plateIndex].diagImage;

   w = libData.plateImageWidth = libData.foundPlates[plateIndex].width;
   h = libData.plateImageHeight = libData.foundPlates[plateIndex].height;
  

   libData.plateImage = libData.foundPlates[plateIndex].image;

   ////////////////////////////////////////////   set        rotation and roll detection/correction

   //libData.processOptions.EnableRotationRoll = 1;

   //libData.processOptions.EnableAutoRotationRoll = 1;

   /////


   if ( libData.processOptions.EnableAutoRotationRoll == 1 )
      libData.plateSlopeT = 0;

   // copy the image to plate diagnostics

   if ( m_CharIsoDebug )
      LPROCR_plate_diags_init ( w, h, diagImage, error);

   if ( *error != 0 ) return(0);

   binarize ( libData.plateImage,  w,  h,  & libData.binarizedImageClean,  error );
   if ( *error != 0 ) return(0);


   LPROCR_plate_diags_putImage(libData.binarizedImageClean, diagImage, w, h);


   if ( libData.processOptions.EnableAutoRotationRoll == 0 )
      frameFound = RemoveBottomFrame(0);

   FindCharsUsingFF ( error);
   if ( *error != 0 ) return(0);

   sortChars ( );



   if ( libData.processOptions.EnableRotationRoll )
   {
      if (libData.processOptions.EnableAutoRotationRoll )
      {
         if ( RemoveBottomFrame(1) == 1 )
         {
            FindCharsUsingFF ( error);
            if ( *error != 0 ) return 0;

            sortChars ( );
            removeTooTallChars (  );
            removeTooOffsetChars (  );
         }

         findTilt (  );
         if ( *error != 0 ) return 0;

         LPROCR_lib_RotateImage ( libData.plateImage, w, h, libData.plateSlopeT, error );

         binarize ( libData.plateImage,  w,  h,  & libData.binarizedImageClean, error );
         if ( *error != 0 ) return 0;

         RemoveBottomFrame(0);

         FindCharsUsingFF ( error);
         if ( *error != 0 ) return 0;

         sortChars ( );

         roll = checkForRollingShutter ( );

      }
      else
      {
         roll = libData.processOptions.roll ;
         libData.plateSlopeB = libData.plateSlopeT = libData.processOptions.rotation;

         if (   libData.plateSlopeT != 0 )
         {
            LPROCR_lib_RotateImage ( libData.plateImage, w, h, libData.plateSlopeT, error );

            binarize ( libData.plateImage,  w,  h,  & libData.binarizedImageClean, error );
            if ( *error != 0 ) return 0;

            RemoveBottomFrame(0);

            imageChanged = true;
         }

      }
      if (roll!= 0 )
      {
         imageChanged = true;

         LPROCR_lib_unroll (libData.plateImage, w, h,roll, error, 255 );
         if ( *error != 0 ) return 0;

      }

   }



   if ( imageChanged )
   {


      binarize ( libData.plateImage,  w,  h,  & libData.binarizedImageClean,  error );
      if ( *error != 0 ) return 0;

      LPROCR_plate_diags_putImage(libData.binarizedImageClean, diagImage, w, h);

      FindCharsUsingFF ( error);
      if ( *error != 0 ) return 0;


      sortChars ( );

   }




   removeTooShortChars( );

   splitDoubleWides (  );

   removeTooTallChars (  );

   removeTooOffsetChars (  );

   removeBlackWhiteFilled (  );


   if ( m_CharIsoDebug )
      drawBoxesAroundChars(  );

   aveCnt = 0;
   libData.foundPlates[plateIndex].aveCharWidth = 0;

   if ( plateIndex < MAX_ACTUAL_PLATES )
   {
      for (cIndex = 0; cIndex < MAX_ACTUAL_CHARS; cIndex++)
      {
         if ( libData.candidateChars[cIndex].leftEdge != -1 ) foundCount++;

         libData.foundPlates[plateIndex].charLocations[cIndex].leftEdge =  libData.candidateChars[cIndex].leftEdge ;
         libData.foundPlates[plateIndex].charLocations[cIndex].rightEdge = libData.candidateChars[cIndex].rightEdge ;
         libData.foundPlates[plateIndex].charLocations[cIndex].topEdge = libData.candidateChars[cIndex].topEdge ;
         libData.foundPlates[plateIndex].charLocations[cIndex].bottomEdge = libData.candidateChars[cIndex].bottomEdge ;

         cw = libData.candidateChars[cIndex].rightEdge - libData.candidateChars[cIndex].leftEdge;
         ch =  libData.candidateChars[cIndex].bottomEdge - libData.candidateChars[cIndex].topEdge;

         if ( libData.candidateChars[cIndex].leftEdge  != -1 )
         {
            libData.foundPlates[plateIndex].aveCharWidth += cw;
            aveCnt ++;
         }

      }
   }

   if  ( aveCnt == 0 )libData.foundPlates[plateIndex].aveCharWidth = 0;
   else  libData.foundPlates[plateIndex].aveCharWidth /= aveCnt;

   return(foundCount);
}


int LPROCR_lib_getAveCharWidth(int plateIndex, int useCandidateList )
{
   if ( useCandidateList == 1)
   {
      return( libData.foundPlates [plateIndex].aveCharWidth);
   }
   else
   {
      return( libData.finalPlateList[plateIndex].aveCharWidth);
   }
}


int RemoveBottomFrame(int possiblyRotatedPlate)
{
   int x,y,xo;
   int w,h;
   int xStart, xStop, yStart, yStop;
   int * image;
   int blackCnt, totalCnt;

   w = libData.plateImageWidth;
   h = libData.plateImageHeight;

   xStart = w/4;
   xStop = (3*w)/4;
   yStart = h/2;
   yStop = h-1;
   image = libData.binarizedImageClean;



   for (y = yStart; y < yStop; y++)
   {
      blackCnt = 0;
      totalCnt = 0;
      for ( x = xStart; x < xStop; x++)
      {
         xo = x * h;
         if ( *(image+xo+y) == 0 ) blackCnt++;
         totalCnt++;
      }

      if ( (float) blackCnt / totalCnt > 0.9 ) 
      {
         if (  possiblyRotatedPlate ) y = y - (h/4);
         else y = y -1;

         for ( x = 0; x < w; x++)
         {
            xo = x * h;
            *(image+xo+y)= 255;
         }
         return(1);
      }
   }
   return(0);
}



void LPROCR_lib_unroll (int * image, int w, int h, float m, int * error, int fillvalue )
{
   int x,y,xo;
   float XCorrected = 0;
   float xDelta = 0;

   int * tempimage;
   float Xnew, Ynew, signX, signY, fractionalY, fractionalX,XoldFloat, YoldFloat;
   int XoldInt, YoldInt;
   float dummy, newVal;

   if ( m == 0 ) return;

   if( image == 0 ) return;

   tempimage = (int *) malloc(sizeof(int)* w * h);
   if ( tempimage == 0 )
   {
      *error = 1;
      return;
   }

   // to prevent dragging in back across, fill in white along the edges

   xo = 0;
   for(y=0;y<h;y++)
   {
      *(image+xo+y )= fillvalue;
   }

   xo = (w-1)*h;
   for(y=0;y<h;y++)
   {
      *(image+xo+y )= fillvalue;
   }

   for(y=0;y<h;y++)
   {
      for ( x =0; x < w; x++)
      {

         xDelta = (( (float)y  ) / m);
         XCorrected = x + xDelta;


         Xnew =(float) x;
         Ynew =(float) y ;

         XoldFloat =(float) XCorrected ;
         YoldFloat =(float) y;


         if ( XoldFloat> 0 ) signX = 1;
         else signX = -1;

         if ( YoldFloat> 0 ) signY = 1;
         else signY = -1;

         fractionalX = abs ( modf(XoldFloat, &dummy));
         fractionalY = abs ( modf(YoldFloat, &dummy));


         XoldInt = (int) XoldFloat ;
         LIMIT(XoldInt,w);
         YoldInt = (int) YoldFloat ;
         LIMIT(YoldInt,h);

         newVal = ( 1 - fractionalX) * (1 - fractionalY) * val ( image, w,h, XoldInt, YoldInt )
            + fractionalX * (1- fractionalY) * val (image, w,h,  XoldInt + (int)signX, YoldInt  )
            + fractionalY *(1-fractionalX) * val (image,w, h,  XoldInt, YoldInt + (int)signY)
            + fractionalY * fractionalX * val (image, w,h,  XoldInt+(int)signX, YoldInt + (int)signY);

         xo = x * h;
         *( tempimage+xo+y) = (int) newVal ;

      }
   }

   for(y=0;y<h;y++)
   {
      for ( x =0; x < w; x++)
      {
         xo = x * h;
         *(image+xo+y) = *(tempimage+xo+y) ;
      }
   }

   freeMemory ( & tempimage);
}


float checkForRollingShutter ( )
{

   float roll;

   roll = testVerticalSlope ( );
   return(roll);

}




struct ROLL_EDGE_FILTER
{
   float m;
   int   b;
};



float testVerticalSlope ( )
{
   int xStart = 0;
   int x,y,xo;
   int w, h;
   float roll;
   int f;

   int * image;
   int source;
   int xol;
   int sum=0;
   int cnt;
   int yi=0;
   int x1,x2,y1,y2;
   float maxSum = 0;
   float secondMaxSum = 0;
   int maxF = -1;
   int secondMaxF = -1;
   int xx, xxl;
   int locationOfPeak=0;
   int rangeHeight;
   int posEdge=0;
   int negEdge=0;
   int maxCnt=0;

#define MAX_X_ANGLE_OFFSET 30
#define NUM_ANGLES 2*MAX_X_ANGLE_OFFSET

   int histo[ NUM_ANGLES];


   ROLL_EDGE_FILTER filter[NUM_ANGLES];


   image = libData.binarizedImageClean;  
   // image = libData.plateImage;

   roll = 0;


   w = libData.plateImageWidth;
   h = libData.plateImageHeight;
   rangeHeight =  libData.lowerCorner -libData.upperCorner ;

   // clear histo
   for (f =0; f < NUM_ANGLES; f++)
      histo[f] = 0;

   // define the filters
   f = 0;
   cnt = 0;
   filter[f].b  = libData.lowerCorner;
   filter[f].m  = 0;
   cnt = 1;
   for( f = 1; f < MAX_X_ANGLE_OFFSET; f++)
   {
      filter[f].b  = libData.lowerCorner;
      filter[f].m  = (float)( libData.lowerCorner  - libData.upperCorner ) / ( 0 - cnt );
      cnt++;
   }

   cnt = 0;
   filter[f].b  = libData.upperCorner;
   filter[f].m  = 0;
   cnt = 1;
   for( f = MAX_X_ANGLE_OFFSET+1; f < MAX_X_ANGLE_OFFSET * 2 ; f++)
   {
      filter[f].b  = libData.upperCorner;
      filter[f].m  = (float)(libData.upperCorner - libData.lowerCorner ) / ( 0 - cnt );
      cnt++;
   }


   // now run the filters 

   maxSum = 0;
   for (xStart = 3; xStart < w-3; xStart++)
   {
      for( f = 0; f < MAX_X_ANGLE_OFFSET * 2; f++)
      {
         // sum = 0;
         posEdge =0;
         negEdge = 0;

         for( yi = libData.upperCorner; yi < libData.lowerCorner ; yi++)
         {

            if (filter[f].m  == 0 )
               x = 0;
            else
               x =(int)( ( (float)yi - filter[f].b) / filter[f].m ) ;

            xxl = (x-1+ xStart);
            LIMIT(xxl,w);
            xx  = (x + xStart );
            LIMIT(xx,w);

            xol = xxl  * h;
            xo =  xx  * h;
            y   = (yi     ) ;

            LIMIT(y,h);


            source = ( - (*(image+xol+y) & 255) + (*(image+xo+y)) & 255) ;
            if ( source  > 0 )
            {
               posEdge += source;
            }

            source = (  (*(image+xol+y) & 255) - (*(image+xo+y)) & 255) ;
            if ( source  > 0 )
            {
               negEdge += (-source);
            }

         }


         if (posEdge > negEdge ) sum = posEdge;
         else sum = negEdge;

         if ( sum > maxSum && sum > ( rangeHeight * 100) )
         {
            secondMaxF = maxF;
            secondMaxSum = maxSum;
            maxSum = (float)sum;
            maxF = f;
            locationOfPeak = xStart;
            histo[maxF] ++;
         }
      }
   }


   if ( maxF == -1 ) return (0);

   // find the angle which had the highest occurance
   maxF = 0;
   maxCnt = 0;
   for (f=0; f < NUM_ANGLES; f++)
   {
      if ( histo[f] > maxCnt ) 
      {
         maxF = f;
         maxCnt = histo[f];
      }
   }



   y1 = libData.upperCorner;
   y2 = libData.lowerCorner;

   if ( filter[maxF].m != 0 )
   {

      x1 =(int)( ( (float)y1 - filter[maxF].b) / filter[maxF].m );


      x2 =(int)( ( (float)y2 - filter[maxF].b) / filter[maxF].m );
   }
   else 
   {
      x1 = x2 = 0;
   }

   plate_diags_PlotLine (x1+locationOfPeak,y1,x2+locationOfPeak,y2);


   /* if ( secondMaxF != -1 )
   {
   if ( abs (  secondMaxSum - maxSum ) <  abs((18*maxSum)/100) && abs ( maxF - secondMaxF) < 2 )
   {
   return ( (filter[ maxF ].m + filter[ secondMaxF ].m) / 2 );
   }
   }*/
   return(filter[ maxF ].m );
}


// Feb 24 - worked well 80% of the time - looks for an edge on each end of plate, averages two best
//float testVerticalSlope ( )
//{
//    int xStart = 0;
//    int x,y,xo;
//    int w, h;
//    bool whiteFound;
//    bool blackFound;
//    int deltaX, deltaY;
//    float roll;
//    int f;
//  
//    int * image;
//    int source, sourceLeft;
//    int xol;
//    int valTop, valMiddle, valBottom;
//    int sum=0;
//    int cnt;
//    int yi=0;
//    int x1,x2,y1,y2;
//    float maxSum = 0;
//    float secondMaxSum = 0;
//    int maxF = -1;
//    int secondMaxF = -1;
//    int xx, xxl;
//    int locationOfPeak=0;
//    int rangeHeight;
//
//#define MAX_X_ANGLE_OFFSET 30
//
//    ROLL_EDGE_FILTER filter[MAX_X_ANGLE_OFFSET  * 2];
//
//
//    image = libData.binarizedImageClean;  
//   // image = libData.plateImage;
//
//    roll = 0;
//
//
//    w = libData.plateImageWidth;
//    h = libData.plateImageHeight;
//    rangeHeight =  libData.lowerCorner -libData.upperCorner ;
//
//   // define the filters
//    f = 0;
//    cnt = 0;
//    filter[f].b  = (3*h)/4 -1;
//    filter[f].m  = 0;
//    cnt = 1;
//    for( f = 1; f < MAX_X_ANGLE_OFFSET; f++)
//    {
//       filter[f].b  = libData.lowerCorner;
//       filter[f].m  = (float)(libData.upperCorner - libData.lowerCorner) / ( 0 - cnt );
//       cnt++;
//    }
//
//    //cnt = 0;
//    //filter[f].b  = h/4;
//    //filter[f].m  = 0;
//    cnt = 1;
//    for( f = MAX_X_ANGLE_OFFSET; f < MAX_X_ANGLE_OFFSET * 2 ; f++)
//    {
//       filter[f].b  = libData.upperCorner;
//       filter[f].m  = (float)(libData.upperCorner - libData.lowerCorner ) / ( 0 - cnt );
//       cnt++;
//    }
//
//
//    // now run the filters on the left side
//
//    maxSum = 0;
//    for (xStart = 0; xStart < libData.leftMostCorner + 5; xStart++)
//    {
//        for( f = 0; f < MAX_X_ANGLE_OFFSET * 2; f++)
//        {
//            sum = 0;
//
//            for( yi = libData.upperCorner; yi < libData.lowerCorner ; yi++)
//            {
//
//                if (filter[f].m  == 0 )
//                    x = 0;
//                else
//                    x =(int)( ( (float)yi - filter[f].b) / filter[f].m ) ;
//
//                xxl = (x-1+ xStart);
//                LIMIT(xxl,w);
//                xx  = (x+1 + xStart );
//                LIMIT(xx,w);
//
//                xol = xxl  * h;
//                xo =  xx  * h;
//                y   = (yi     ) ;
//
//                LIMIT(y,h);
//
//
//            //if ( xStart == 1 && f == 40 )
//            //{
//            //    x1 = xx; 
//            //    y1 = yi;
//            //    y2 = yi;
//            //    x2 = xxl  ;
//
//            //    plate_diags_PlotLine (x1 ,y1,x2 ,y2);
//            //}
//
//
//               
//                source = ( - (*(image+xol+y) & 255) + (*(image+xo+y)) & 255) ;
//                if ( abs(source) > 40 )
//                {
//                    sum += source;
//                   
//         
//                }
//
//               
//            }
//
//           
//
//            sum = abs (sum);
//
//
//            if ( sum > maxSum && sum > ( rangeHeight * 100 ) )
//            {
//                secondMaxF = maxF;
//                secondMaxSum = maxSum;
//                maxSum = sum;
//                maxF = f;
//                locationOfPeak = xStart;
//            }
//        }
//    }
//    
//
//     
//     // now run the filters on the right side
//
//    maxSum = 0;
//    for (xStart = libData.rightMostCorner - 5; xStart < w-1; xStart++)
//    {
//        for( f = 0; f < MAX_X_ANGLE_OFFSET * 2; f++)
//        {
//            sum = 0;
//
//            for( yi = libData.upperCorner; yi < libData.lowerCorner ; yi++)
//            {
//
//                if (filter[f].m  == 0 )
//                    x = 0;
//                else
//                    x =(int)( ( (float)yi - filter[f].b) / filter[f].m ) ;
//
//        
//                xxl = (x-1+ xStart);
//                LIMIT(xxl,w);
//                xx  = (x+1 + xStart );
//                LIMIT(xx,w);
//
//                xol = xxl  * h;
//                xo =  xx  * h;
//                y   = (yi     ) ;
//
//                LIMIT(y,h);
//
//
//            //if ( xStart == 1 && f == 40 )
//            //{
//            //    x1 = xx; 
//            //    y1 = yi;
//            //    y2 = yi;
//            //    x2 = xxl  ;
//
//            //    plate_diags_PlotLine (x1 ,y1,x2 ,y2);
//            //}
//
//
//               
//                source = ( + (*(image+xol+y) & 255) - (*(image+xo+y)) & 255) ;
//                if ( abs(source) > 40 )
//                {
//                    sum += source;
//                   
//         
//                }
//
//               
//            }
//
//           
//
//            sum = abs (sum);
//
//
//            if ( sum > maxSum && sum > ( rangeHeight * 100 ) )
//            {
//                secondMaxF = maxF;
//                secondMaxSum = maxSum;
//                maxSum = sum;
//                maxF = f;
//                locationOfPeak = xStart;
//            }
//        }
//    }
//
//   if ( maxF == -1 ) return (0);
//
//
//   y1 = h/4;
//   y2 = 3*h/4;
//   if ( filter[maxF].m != 0 )
//   {
//
//       x1 =(int)( ( (float)y1 - filter[maxF].b) / filter[maxF].m );
//
//
//       x2 =(int)( ( (float)y2 - filter[maxF].b) / filter[maxF].m );
//   }
//   else 
//   {
//       x1 = x2 = 0;
//   }
//
//   plate_diags_PlotLine (x1+locationOfPeak,y1,x2+locationOfPeak,y2);
//   
//
//   if ( secondMaxF != -1 )
//   {
//        if ( abs (  secondMaxSum - maxSum ) <  abs((18*maxSum)/100) && abs ( maxF - secondMaxF) < 2 )
//        {
//            return ( (filter[ maxF ].m + filter[ secondMaxF ].m) / 2 );
//        }
//   }
//    return(filter[ maxF ].m );
//}
//

void sortChars ( )
{
   int c=0;
   int i=0;
   int indexOrder[ MAX_CANDIDATE_CHARS];
   int marked[ MAX_CANDIDATE_CHARS];

   LPROCR_lib_CHAR_LOCATION tempArray [ MAX_CANDIDATE_CHARS];

   int leftMost = libData.plateImageWidth;

   for ( c = 0; c < MAX_CANDIDATE_CHARS; c++)
   { 
      indexOrder[c] = -1;
      marked[c] = 0;
      tempArray [c].leftEdge = -1;
      tempArray [c].topEdge = -1;
      tempArray [c].bottomEdge = -1;
      tempArray [c].rightEdge = -1;
   }

   for ( i =0; i < MAX_CANDIDATE_CHARS; i++)
   {
      leftMost = libData.plateImageWidth;
      for ( c =0; c < MAX_CANDIDATE_CHARS; c++)
      {
         if ( libData.candidateChars[c].leftEdge != -1 && libData.candidateChars[c].leftEdge  < leftMost && marked[c] != 1 ) 
         {
            leftMost = libData.candidateChars[c].leftEdge;
            indexOrder[i] = c;
         }
      }
      if ( indexOrder[i] != -1 ) marked[indexOrder[i]] = 1;
   }

   for ( c = 0; c < MAX_CANDIDATE_CHARS; c++)
   { 
      if (  indexOrder[c] == -1  )
      {
         tempArray [c].leftEdge = -1;
         tempArray [c].topEdge = -1;
         tempArray [c].bottomEdge = -1;
         tempArray [c].rightEdge = -1;
      }
      else
      {
         tempArray [c].leftEdge = libData.candidateChars[indexOrder[c]].leftEdge;
         tempArray [c].topEdge = libData.candidateChars[indexOrder[c]].topEdge;
         tempArray [c].bottomEdge = libData.candidateChars[indexOrder[c]].bottomEdge;
         tempArray [c].rightEdge = libData.candidateChars[indexOrder[c]].rightEdge;
      }

   }

   libData.leftMostCorner = 99999;
   libData.rightMostCorner = 0;
   libData.upperCorner = 99999;
   libData.lowerCorner = 0;

   for ( c = 0; c < MAX_CANDIDATE_CHARS; c++)
   { 
      libData.candidateChars[c].leftEdge= tempArray [c].leftEdge;
      libData.candidateChars[c].topEdge= tempArray [c].topEdge;
      libData.candidateChars[c].bottomEdge= tempArray [c].bottomEdge;
      libData.candidateChars[c].rightEdge= tempArray [c].rightEdge;

      if (  libData.candidateChars[c].leftEdge != -1 )
      {
         if (   libData.candidateChars[c].leftEdge   < libData.leftMostCorner )   libData.leftMostCorner = libData.candidateChars[c].leftEdge;
         if (   libData.candidateChars[c].rightEdge  > libData.rightMostCorner )   libData.rightMostCorner = libData.candidateChars[c].rightEdge;
         if (   libData.candidateChars[c].bottomEdge > libData.lowerCorner )   libData.lowerCorner = libData.candidateChars[c].bottomEdge;
         if (   libData.candidateChars[c].topEdge    < libData.upperCorner )   libData.upperCorner = libData.candidateChars[c].topEdge;
      }

   }

   if (libData.leftMostCorner > libData.plateImageWidth ) libData.leftMostCorner = 0;
   if (libData.upperCorner > libData.plateImageHeight ) libData.upperCorner = 0;


}




void FindCharsUsingFF ( int * error)
{

   int x,y,xo;
   int *markupImage;
   int w,h; 
   int c = 0;


   w = libData.plateImageWidth;
   h = libData.plateImageHeight;

   markupImage = (int *)malloc ( sizeof(int) * w * h);
   if ( markupImage == 0 )
   {
      *error = 1;
      return;
   }



   // need the clean but binarized image for markup
   CopyImage (libData.binarizedImageClean, markupImage,  w,  h);

   for ( c =0; c < MAX_CANDIDATE_CHARS; c++)
   {
      libData.candidateChars[c].leftEdge   = -1 ;
      libData.candidateChars[c].rightEdge  = -1 ;
      libData.candidateChars[c].topEdge    = -1 ;
      libData.candidateChars[c].bottomEdge = -1 ;
   }

   // start in the middle and move left, looking for a black pixel
   // left edge
   y = h/2;
   c = 0;
   for ( x = w/2; x >= 2 ; x--)
   {
      xo = x * h;

      if (  *(markupImage+xo+y) == 0 ) 
      {
         libData.candidateChars[c].leftEdge  = x;
         libData.candidateChars[c].rightEdge  = x;
         libData.candidateChars[c].topEdge  = y;
         libData.candidateChars[c].bottomEdge  = y;

         charFF( markupImage, x, y,  0, & libData.candidateChars[c] );
         //  if ( libData.candidateChars[c].rightEdge  - libData.candidateChars[c].leftEdge   < MINIMUM_CHAR_WIDTH) libData.candidateChars[c].leftEdge = -1;
         if ( libData.candidateChars[c].rightEdge  - libData.candidateChars[c].leftEdge   > MAX_CHAR_WIDTH+10) libData.candidateChars[c].leftEdge = -1;
         if ( libData.candidateChars[c].bottomEdge  - libData.candidateChars[c].topEdge   < MINIMUM_CHAR_HEIGHT-2) libData.candidateChars[c].leftEdge = -1;
         if ( libData.candidateChars[c].bottomEdge  - libData.candidateChars[c].topEdge   > MAX_CHAR_HEIGHT-2) libData.candidateChars[c].leftEdge = -1;

         if ( libData.candidateChars[c].leftEdge  != -1 ) c++;
         if ( c >= MAX_CANDIDATE_CHARS ) break;
      }
   }

   y = h/2;
   if ( c < MAX_CANDIDATE_CHARS )
   {
      for ( x = w/2+1; x < w-10 ; x++)
      {
         xo = x * h;

         if (  *(markupImage+xo+y) == 0 ) 
         {
            libData.candidateChars[c].leftEdge  = x;
            libData.candidateChars[c].rightEdge  = x;
            libData.candidateChars[c].topEdge  = y;
            libData.candidateChars[c].bottomEdge  = y;
            charFF(markupImage,  x, y, 0, & libData.candidateChars[c] );
            //      if ( libData.candidateChars[c].rightEdge  - libData.candidateChars[c].leftEdge   < MINIMUM_CHAR_WIDTH) libData.candidateChars[c].leftEdge = -1;
            if ( libData.candidateChars[c].rightEdge  - libData.candidateChars[c].leftEdge   > MAX_CHAR_WIDTH+10) libData.candidateChars[c].leftEdge = -1;
            if ( libData.candidateChars[c].bottomEdge  - libData.candidateChars[c].topEdge   < MINIMUM_CHAR_HEIGHT-2) libData.candidateChars[c].leftEdge = -1;
            if ( libData.candidateChars[c].bottomEdge  - libData.candidateChars[c].topEdge   > MAX_CHAR_HEIGHT-2) libData.candidateChars[c].leftEdge = -1;

            if ( libData.candidateChars[c].leftEdge  != -1 ) c++;
            if ( c >= MAX_CANDIDATE_CHARS ) break;
         }
      }
   }

   for ( c =0; c < MAX_CANDIDATE_CHARS; c++)
   {
      if ( libData.candidateChars[c].leftEdge == -1 ) continue;

      //   trimEdges ( & libData.candidateChars[c] );

      if ( libData.candidateChars[c].leftEdge > 1 )
         libData.candidateChars[c].leftEdge   = libData.candidateChars[c].leftEdge -1;

      if ( libData.candidateChars[c].rightEdge <  libData.plateImageWidth-2)
         libData.candidateChars[c].rightEdge  +=1;

      if ( libData.candidateChars[c].topEdge > 1 )
         libData.candidateChars[c].topEdge    -=1;

      if ( libData.candidateChars[c].rightEdge <  libData.plateImageHeight-2)
         libData.candidateChars[c].bottomEdge += 1;
   }


   freeMemory ( & markupImage );
}


void trimEdges (LPROCR_lib_CHAR_LOCATION * candidateChar )
{
   int w,h;
   int x,y,xo;
   int darkCount= 0;

   w = libData.plateImageWidth;
   h = libData.plateImageHeight;

   if (candidateChar->rightEdge -  candidateChar->leftEdge < 6 ) return;

   // run the left edge
   x = candidateChar->leftEdge;
   xo = x * h;
   darkCount = 0;
   for ( y = candidateChar->topEdge; y <= candidateChar->bottomEdge; y++)
   {

      if ( *(libData.binarizedImageClean+xo+y) == 0 )
      {
         darkCount ++;
      }
   }
   if ( darkCount < 3 ) 
      candidateChar->leftEdge ++;


   // run the right edge
   x = candidateChar->rightEdge;
   xo = x * h;
   darkCount = 0;
   for ( y = candidateChar->topEdge; y <= candidateChar->bottomEdge; y++)
   {

      if ( *(libData.binarizedImageClean+xo+y) == 0 )
      {
         darkCount ++;
      }
   }
   if ( darkCount < 3 ) 
      candidateChar->rightEdge --;



   // run the top edge
   y = candidateChar->topEdge;

   darkCount = 0;
   for ( x = candidateChar->leftEdge; x <= candidateChar->rightEdge; x++)
   {
      xo = x * h;
      if ( *(libData.binarizedImageClean+xo+y) == 0 )
      {
         darkCount ++;
      }
   }
   if ( darkCount < 3 ) 
      candidateChar->topEdge ++;




   // run the bottom edge
   y = candidateChar->bottomEdge;

   darkCount = 0;
   for ( x = candidateChar->leftEdge; x <= candidateChar->rightEdge; x++)
   {
      xo = x * h;
      if ( *(libData.binarizedImageClean+xo+y) == 0 )
      {
         darkCount ++;
      }
   }
   if ( darkCount < 3 ) 
      candidateChar->bottomEdge --;

}


void charFF ( int * markup, int startX, int startY,  int recursion , LPROCR_lib_CHAR_LOCATION * candidateChar)
{
   int w,h;
   int x,y,xo;
   int leftExtent = startX;
   int rightExtent = startX;


   /* if ( recursion > MAX_CHAR_HEIGHT) 
   {
   candidateChar->leftEdge = -1;
   return;
   }*/

   w = libData.plateImageWidth;
   h = libData.plateImageHeight;

   if ( startY < 0 )
   {
      candidateChar->leftEdge = -1;
      return;
   }
   if ( startY >= h )
   {
      candidateChar->leftEdge = -1;
      return;
   }


   y = startY;



   // take input Y and if black mark top/bottom edge of char
   xo = startX * h;
   if (  *(markup+xo+y) == 0 )
   {
      if ( candidateChar->topEdge == -1 ) candidateChar->topEdge = y;
      if ( candidateChar->bottomEdge == -1 ) candidateChar->bottomEdge = y;

      if ( y < candidateChar->topEdge ) candidateChar->topEdge = y;
      if ( y > candidateChar->bottomEdge ) candidateChar->bottomEdge = y;
   }
   else
      return;


   if (candidateChar->bottomEdge - candidateChar->topEdge > MAX_CHAR_HEIGHT)   
   {
      candidateChar->leftEdge = -1;
      return;
   }

   // find left extant  - mark left edge of char
   leftExtent = startX;
   for ( x = startX; x >= 0 ; x--)
   {
      xo = x * h;
      if ( *(markup+xo+y) == 0 )
      {
         if ( x < leftExtent ) leftExtent = x;
      }
      else
         break;
   }
   if ( leftExtent  == 0 )
   {
      candidateChar->leftEdge = -1;
      return;
   }

   rightExtent = startX;
   for ( x = startX+1; x < w ; x++)
   {
      xo = x * h;
      if ( *(markup+xo+y) == 0 )
      {
         if ( x > rightExtent ) rightExtent = x;
      }
      else
         break;
   }
   if ( rightExtent >= w-1 )
   {
      candidateChar->leftEdge = -1;
      return;
   }

   if ( rightExtent - leftExtent > (MAX_CHAR_WIDTH + 10) )   // allow to go over max-width so that double-wides can get filtered later
   {
      candidateChar->leftEdge = -1;
      return;
   }


   if ( leftExtent < candidateChar->leftEdge ) candidateChar->leftEdge = leftExtent;
   if ( rightExtent > candidateChar->rightEdge ) candidateChar->rightEdge = rightExtent;

   for ( x = leftExtent; x <= rightExtent; x++)
   {
      xo = x * h;
      if ( *(markup+xo+y) == 0 )
      {
         *(markup+xo+y) = 128;


         plate_diags_PlotLine (  x, y, x+1,  y  );

         // recurse one below and one above

         recursion ++;

         charFF ( markup,  x,  startY-1,  recursion , candidateChar);
         if ( candidateChar->leftEdge == -1 ) 
            return;

         charFF ( markup,  x,  startY+1, recursion , candidateChar);
      }
   }
}

float val ( int * image, int w, int h, int x, int y)
{

   LIMIT(x,w);
   LIMIT(y,h);

   return ( (float) *(image+(x * h )+y ));
}


void LPROCR_lib_RotateImage (int * image, int w, int h, float slope, int * error )
{

   // slope is +/- delta y / delta x.  range between 1.0 and -1.0 are reasonable values

   int x,y,xo;


   double theta;


   int Xnew, Ynew, XoldInt, YoldInt;
   float XoldFloat, YoldFloat;


   float  dummy, newVal ;
   int *tempimage ;
   int centerX, centerY;



   int signX ;
   int signY ;
   float fractionalX, fractionalY;

   int wPad, hPad,xPadOffset,yPadOffset;

   // wPad = (10*w)/100;
   // hPad = (10*h)/100;

   wPad  = 0;
   hPad = 0;

   centerX = w/2;
   centerY = h/2;


   tempimage = (int *) malloc(sizeof(int) * w * h);
   if ( tempimage == 0 )
   {
      *error = 1;
      return;
   }


   theta = atan ( -1.0 * slope);


   xPadOffset = wPad / 2;
   yPadOffset = hPad /2 ;

   // put white along top and bottom edges to pull in white to fill gaps during rotation
   for ( x = 0; x < w; x++)
   {
      xo =x * h;
      *(image+xo) = 255;
      *(image+xo+h-1) = 255;
   }

   // walk through the new pixels, projecting back to the old locations
   for ( x =0; x < (w-xPadOffset); x++)
   {
      xo = (x+xPadOffset) * h;
      for(y=0;y< (h-yPadOffset);y++)
      {

         Xnew = x - centerX;
         Ynew = y - centerY;

         XoldFloat = (float)Xnew * (float)cos (theta) - (float)Ynew * (float)sin(theta);
         YoldFloat = (-1.0f)* (float)Xnew * (float)sin(theta) + (float)Ynew * (float)cos(theta);
         
     /*    XoldFloat = (float)Xnew * (float)cos (theta) - (float)Ynew * (float)sin(theta);
         YoldFloat =  (float)Xnew * (float)sin(theta) + (float)Ynew * (float)cos(theta);*/

         if ( XoldFloat> 0 ) signX = 1;
         else signX = -1;

         if ( YoldFloat> 0 ) signY = 1;
         else signY = -1;


         fractionalX = abs ( modf(XoldFloat, &dummy));
         fractionalY = abs ( modf(YoldFloat, &dummy));


         XoldInt = (int) XoldFloat + centerX;
         YoldInt = (int) YoldFloat + centerY;

         LIMIT(XoldInt,w);
         LIMIT(YoldInt,h);

         newVal = ( 1 - fractionalX) * (1 - fractionalY) * val ( image,w, h, XoldInt, YoldInt )
            + fractionalX * (1- fractionalY) * val (image,w, h,  XoldInt + signX, YoldInt  )
            + fractionalY *(1-fractionalX) * val (image,w, h,  XoldInt, YoldInt + signY)
            + fractionalY * fractionalX * val (image,w, h,  XoldInt+signX, YoldInt + signY);


         *( tempimage+xo+y+yPadOffset) = (int) newVal ;

      }
   }

   for ( x =0; x < w; x++)
   {
      xo = x * h;
      for(y=0;y<h;y++)
      {
         *(image+xo+y) = *(tempimage+xo+y);
      }
   }

   freeMemory ( & tempimage);



}


void findTilt (  )
{


   int w,h;
   int c;
   int  yAtX;
   float sumT = 0, sumB=0;
   int cnt = 0;
   float slopeT = 0, slopeB=0;
   int xStart = 0;
   int xEnd =0;


   float slopesT[ MAX_CANDIDATE_CHARS ];
   float slopesB[ MAX_CANDIDATE_CHARS ];


   w = libData.plateImageWidth;
   h = libData.plateImageHeight;

   if ( libData.candidateChars[0].leftEdge == -1 || libData.candidateChars[0].topEdge == -1) return;

   for ( c =0; c < MAX_CANDIDATE_CHARS; c++)
   {
      slopesT[c] = -1;
      slopesB[c] = -1;
   }

   for ( c =1; c < MAX_CANDIDATE_CHARS; c++)
   {
      if ( libData.candidateChars[c].leftEdge == -1 || libData.candidateChars[c].topEdge == -1) break;

      if ( libData.candidateChars[c].leftEdge - libData.candidateChars[c-1].leftEdge == 0 )
      {
         slopesT[c-1] = -1;
         continue;
      }


      slopesT[c-1] = (float)(libData.candidateChars[c].topEdge - libData.candidateChars[c-1].topEdge ) /(libData.candidateChars[c].leftEdge - libData.candidateChars[c-1].leftEdge );
      slopesB[c-1] = (float)(libData.candidateChars[c].bottomEdge - libData.candidateChars[c-1].bottomEdge ) /(libData.candidateChars[c].leftEdge - libData.candidateChars[c-1].leftEdge );
   }

   cnt = 0;
   for ( c =0; c < MAX_CANDIDATE_CHARS; c++)
   {
      if ( slopesT[c] != -1 )
      {
         sumT += slopesT[c];
         sumB += slopesB[c];
         cnt++;
      }
   }

   if ( cnt != 0 )
   {
      slopeT = sumT/(float)cnt;
      slopeB = sumT/(float)cnt;
   }
   else 
   {
      slopeT = 0;
      slopeB = 0;
   }

   //// if any slope is off by more than 10% of the average, eliminate it
   //cnt = 0;
   //for ( c =0; c < MAX_CANDIDATE_CHARS; c++)
   //{
   //    if ( slopesT[c] - slopeT > (0.1f * slopeT ))
   //    {
   //        slopesB[c]= slopesT[c] = -1;
   //    }
   //}

   //cnt = 0;
   //sumT = sumB = 0;
   //for ( c =0; c < MAX_CANDIDATE_CHARS; c++)
   //{
   //    if ( slopesT[c] != -1 )
   //    {
   //        sumT += slopesT[c];
   //        sumB += slopesB[c];
   //        cnt++;
   //    }
   //}


   //if ( cnt != 0 )
   //{
   //    slopeT = sumT/(float)cnt;
   //    slopeB = sumT/(float)cnt;
   //}
   //else 
   //{
   //    slopeT = 0;
   //    slopeB = 0;
   //}

   /* for ( c =1; c < MAX_CANDIDATE_CHARS; c++)
   {
   if ( libData.candidateChars[c].leftEdge == -1 || libData.candidateChars[c].topEdge == -1) continue;

   libData.bT =  libData.candidateChars[0].topEdge - ( slopeT * libData.candidateChars[0].leftEdge ) ;
   libData.bB =  libData.candidateChars[0].bottomEdge - ( slopeT * libData.candidateChars[0].leftEdge ) ;

   break;
   }*/


   xStart =0;
   xEnd = w -1;

   yAtX = (int)(slopeT * (float)xEnd ) + 20;

   LIMIT(yAtX,h);

   libData.plateSlopeT = slopeT;
   libData.plateSlopeB = slopeB;

   plate_diags_PlotLine (  xStart, 20, xEnd,  yAtX  );

}



void floodfill ( int startColor, int endColor, int xStart, int yStart, int * image, int recursion)
{

   int x,y, xo;

   int w,h; 

   int leftExtent;
   int rightExtent;

   if ( recursion > 60) return; //20
   recursion++;


   w = libData.plateImageWidth;
   h = libData.plateImageHeight;

   if ( xStart >= w) return;
   if ( xStart < 0 ) return;

   if ( yStart >= h ) return;
   if ( yStart < 0 ) return;


   xo = xStart * h;
   if ( *(image + xo + yStart) != startColor ) return;

   // find left extent
   x = xStart;
   y = yStart;

   while ( x > -1 )
   {
      xo = x * h;
      if ( *(image+ xo + y) != startColor ) break;
      x--;
   }
   leftExtent = x + 1;
   LIMIT(leftExtent,w);




   // find right extent
   x = xStart;
   y = yStart;

   while ( x < w/ 4)   // only care about the leading dark which gets interpreted as a leading 1
      // while ( x < w )
   {
      xo = x * h;
      if ( *(image + xo + y) != startColor ) break;
      x++;
   }
   rightExtent = x - 1;
   LIMIT(rightExtent,w);


   // loop from left to right
   for (x = leftExtent; x <= rightExtent; x++)
   {

      xo = x * h;

      if ( *(image + xo + y) == startColor ) 
      {

         // if current is startcolor, 

         // change to end color
         *(image + xo + y) = endColor;

         *(libData.plateImage+ xo + y) = endColor;

         // flood fill one below
         floodfill ( startColor, endColor,  x, yStart -1, image, recursion);

         // flood fill one above
         floodfill ( startColor, endColor,  x, yStart + 1, image, recursion);

      }

   }
}



void drawBoxesAroundChars(  )
{

   int c = 0;

   DIAGCOLOR color;

   color.blue = 0;
   color.red = 255;
   color.green = 0;

   for ( c =0; c < MAX_CANDIDATE_CHARS; c++)
   {
      if ( libData.candidateChars[c].leftEdge != -1 )
      {
         LPROCR_plate_diags_drawBox ( & libData.candidateChars[c], & color );
      }
   }
}


int  LPROCR_lib_getMaxCandidateChars ( )
{
   return ( MAX_CANDIDATE_CHARS );

}


// gets the char locations from the candidate plates

void GetCharLocationCandidatePlates (int plateIndex,  int cIndex, int * left, int *right, int *top, int *bottom   )
{
   if ( cIndex >= MAX_CANDIDATE_CHARS) return;

   *left    =libData.foundPlates[plateIndex].charLocations[cIndex].leftEdge;
   *right   = libData.foundPlates[plateIndex].charLocations[cIndex].rightEdge;
   *top     = libData.foundPlates[plateIndex].charLocations[cIndex].topEdge;
   *bottom  =libData.foundPlates[plateIndex].charLocations[cIndex].bottomEdge;

}


// gets the char locations from the final plate list

void LPROCR_lib_getCharLocation (int plateIndex,  int cIndex, int * left, int *right, int *top, int *bottom , int useCandidatePlateList  )
{

   if ( useCandidatePlateList == 1 )
   {
      if ( cIndex >= MAX_CANDIDATE_CHARS) return;

      *left    =libData.foundPlates[plateIndex].charLocations[cIndex].leftEdge;
      *right   = libData.foundPlates[plateIndex].charLocations[cIndex].rightEdge;
      *top     = libData.foundPlates[plateIndex].charLocations[cIndex].topEdge;
      *bottom  =libData.foundPlates[plateIndex].charLocations[cIndex].bottomEdge;
   }
   else
   {
      if ( cIndex >= MAX_CANDIDATE_CHARS) return;

      *left    =libData.finalPlateList[plateIndex].charLocations[cIndex].leftEdge;
      *right   = libData.finalPlateList[plateIndex].charLocations[cIndex].rightEdge;
      *top     = libData.finalPlateList[plateIndex].charLocations[cIndex].topEdge;
      *bottom  =libData.finalPlateList[plateIndex].charLocations[cIndex].bottomEdge;
   }

}
//
//void isolateChars (    )
//{
//
//    int x,y, xo;
//
//    int * blobedImage; 
//
//    int w,h;
//    int cc = 0;
//
//
//    w = libData.plateImageWidth;
//    h = libData.plateImageHeight;
//    blobedImage = libData.binarizedImageBlobed;
//
//    y = h / 2;
//
//    for ( cc =0; cc < MAX_CANDIDATE_CHARS; cc++)
//    {
//        libData.candidateChars[cc].leftEdge   = -1 ;
//        libData.candidateChars[cc].rightEdge  = -1 ;
//        libData.candidateChars[cc].topEdge    = -1 ;
//        libData.candidateChars[cc].bottomEdge = -1 ;
//    }
//
//
//    // find first black blob on left of plate and keep going to right
//
//    cc = 0;
//
//    for ( x=0; x < w; x++)
//    {
//        xo = x * h;
//        if ( *(blobedImage+xo+y) == 0 && x < w-1)
//        {
//            libData.candidateChars[cc].leftEdge = x ;
//
//            while ( *(blobedImage+xo+y) == 0 && x < w-1 )
//            {
//                x++;
//                xo = x * h;
//            }
//            LIMIT(x,w);
//            libData.candidateChars[cc].rightEdge = x ;
//            cc++;
//            if ( cc == MAX_CANDIDATE_CHARS) break;
//        }
//    }
//
//
//
//    for ( cc =0; cc < MAX_CANDIDATE_CHARS; cc++)
//    {
//        if ( libData.candidateChars[cc].leftEdge   != -1 )
//        {
//            if ( libData.candidateChars[cc].rightEdge   == -1)
//            {
//                libData.candidateChars[cc].leftEdge = -1;
//            }
//            else
//            {
//                findCharTop (  cc );
//
//                findCharBottom ( cc);
//            }
//        }
//    }
//
//
//    // now clean up the candidate chars
//
//    removeTooShortChars ();
//
//    removeTooTallChars (  );
//
//    reviseSlope (  ); // need better slope estimate for the trimTallChars step
//
//    trimTallChars (  );
//
//    splitDoubleWides (  );
//
//    removeTooNarrowChars ();
//
//    removeBlackWhiteFilled (  );
//
//
//
//}




void reviseSlope( )
{
   //  use estimated char tops and bottoms to estimate the slope, then it will be used 
   // later to remove outliers

   int cc;

   int firstX;
   int firstY;

   float slope;
   int slopeCnt;

   cc = 0;
   while ( cc < MAX_CANDIDATE_CHARS-1 && libData.candidateChars[cc].leftEdge == -1 )
   {
      cc++;
   }

   firstX = libData.candidateChars[cc].leftEdge;
   firstY = libData.candidateChars[cc].topEdge;

   slopeCnt=0;
   slope = 0;
   for ( cc =1; cc < MAX_CANDIDATE_CHARS; cc++)
   {
      if ( libData.candidateChars[cc].leftEdge != -1  )
      {
         slopeCnt ++;
         slope  += ((float)libData.candidateChars[cc].topEdge - firstY) / ( (float)libData.candidateChars[cc].leftEdge -  firstX );
      }
   }
   if ( slopeCnt == 0 ) slope = 1.f;
   else slope /= slopeCnt;


   if ( slope < MIN_SLOPE) slope = MIN_SLOPE;
   if ( slope > MAX_SLOPE ) slope = MAX_SLOPE;

   libData.plateSlopeT = slope;

}


void removeTooTallChars()
{

   // char height histogram
   int i;
   int charHisto[MAX_CHAR_HEIGHT / 3];
   int histoSize = MAX_CHAR_HEIGHT / 3; // each bin is two pixels wide
   LPROCR_lib_CHAR_LOCATION tempChars[MAX_CANDIDATE_CHARS];
   int charHeightBin;

   int cc;
   int ccNew;


   for (cc=0; cc < histoSize; cc++)
   {
      charHisto[cc]=0;
   }

   // generate char Height hisogram
   for ( cc =0; cc < MAX_CANDIDATE_CHARS; cc++)
   {
      tempChars[cc].leftEdge = -1 ; // initialize this array for later use
      tempChars[cc].rightEdge = -1 ;
      tempChars[cc].topEdge = -1 ;
      tempChars[cc].bottomEdge = -1 ;


      if ( libData.candidateChars[cc].leftEdge   != -1 )
      {
         charHeightBin = (libData.candidateChars[cc].bottomEdge - libData.candidateChars[cc].topEdge )/3;

         LIMIT(charHeightBin,histoSize);

         charHisto[charHeightBin]++;

      }
   }

   // find the most common char Height

   int mostCommonHeightBin = 0;
   int peak=0;
   for (i=0; i < histoSize; i++)
   {
      if ( charHisto[i] > peak )
      {
         peak = charHisto[i];
         mostCommonHeightBin = i;
      }
   }



   ccNew = 0;
   for ( cc =0; cc < MAX_CANDIDATE_CHARS; cc++)
   {
      if ( libData.candidateChars[cc].leftEdge   != -1 )
      {
         charHeightBin = (libData.candidateChars[cc].bottomEdge - libData.candidateChars[cc].topEdge )/3;

         if ( charHeightBin > (int) ( (float) mostCommonHeightBin * 1.2f ) ) 
            continue ; // just dump this char

         tempChars[ccNew].leftEdge  = libData.candidateChars[cc].leftEdge ;
         tempChars[ccNew].rightEdge  = libData.candidateChars[cc].rightEdge;
         tempChars[ccNew].topEdge  = libData.candidateChars[cc].topEdge  ;
         tempChars[ccNew].bottomEdge  = libData.candidateChars[cc].bottomEdge;
         ccNew++;
      }
   }



      // clear out the original char array
   for ( cc =0; cc < MAX_CANDIDATE_CHARS; cc++)
   {    
      libData.candidateChars[cc].leftEdge = -1;
      libData.candidateChars[cc].rightEdge  = -1;
      libData.candidateChars[cc].topEdge    = -1;
      libData.candidateChars[cc].bottomEdge = -1;
   }

       // copy the temp char array back 
   for ( cc =0; cc < MAX_CANDIDATE_CHARS; cc++)
   {    

      libData.candidateChars[cc].leftEdge   = tempChars[cc].leftEdge;
      libData.candidateChars[cc].rightEdge  = tempChars[cc].rightEdge;
      libData.candidateChars[cc].topEdge    = tempChars[cc].topEdge;
      libData.candidateChars[cc].bottomEdge = tempChars[cc].bottomEdge;
   }
   

}



void removeTooOffsetChars ()
{

#define GRANULARITY 3
   // char height histogram
   int i;
   int charHisto[MAX_PLATE_HEIGHT / GRANULARITY];
   int histoSize = MAX_PLATE_HEIGHT / GRANULARITY; // each bin is two pixels wide
   LPROCR_lib_CHAR_LOCATION tempChars[MAX_CANDIDATE_CHARS];
   int charHeightBin;

   int cc;
   int ccNew;


   for (cc=0; cc < histoSize; cc++)
   {
      charHisto[cc]=0;
   }

   // generate char Height hisogram
   for ( cc =0; cc < MAX_CANDIDATE_CHARS; cc++)
   {
      tempChars[cc].leftEdge = -1 ; // initialize this array for later use
      tempChars[cc].rightEdge = -1 ;
      tempChars[cc].topEdge = -1 ;
      tempChars[cc].bottomEdge = -1 ;


      if ( libData.candidateChars[cc].leftEdge   != -1 )
      {
         charHeightBin = (libData.candidateChars[cc].topEdge )/GRANULARITY;

         LIMIT(charHeightBin,histoSize);

         charHisto[charHeightBin]++;

      }
   }

   // find the most common char Height

   int mostCommonHeightBin = 0;
   int peak=0;
   for (i=0; i < histoSize; i++)
   {
      if ( charHisto[i] > peak )
      {
         peak = charHisto[i];
         mostCommonHeightBin = i;
      }
   }



   ccNew = 0;
   for ( cc =0; cc < MAX_CANDIDATE_CHARS; cc++)
   {
      if ( libData.candidateChars[cc].leftEdge   != -1 )
      {
         charHeightBin = (libData.candidateChars[cc].topEdge )/GRANULARITY;

         if ( charHeightBin > (int) ( (float) mostCommonHeightBin * 1.5f ) ) 
         {
            RejectLogAdd("char top offset variance above limit");
            continue ; // just dump this char
         }

         if ( charHeightBin < (int) ( (float) mostCommonHeightBin * 0.7f ) ) 
         {
            RejectLogAdd("char top offset variance below limit");
            continue ; // just dump this char
         } 

         tempChars[ccNew].leftEdge  = libData.candidateChars[cc].leftEdge ;
         tempChars[ccNew].rightEdge  = libData.candidateChars[cc].rightEdge;
         tempChars[ccNew].topEdge  = libData.candidateChars[cc].topEdge  ;
         tempChars[ccNew].bottomEdge  = libData.candidateChars[cc].bottomEdge;
         ccNew++;
      }
   }



  
      // clear out the original char array
   for ( cc =0; cc < MAX_CANDIDATE_CHARS; cc++)
   {    
      libData.candidateChars[cc].leftEdge = -1;
      libData.candidateChars[cc].rightEdge  = -1;
      libData.candidateChars[cc].topEdge    = -1;
      libData.candidateChars[cc].bottomEdge = -1;
   }

       // copy the temp char array back 
   for ( cc =0; cc < MAX_CANDIDATE_CHARS; cc++)
   {    

      libData.candidateChars[cc].leftEdge   = tempChars[cc].leftEdge;
      libData.candidateChars[cc].rightEdge  = tempChars[cc].rightEdge;
      libData.candidateChars[cc].topEdge    = tempChars[cc].topEdge;
      libData.candidateChars[cc].bottomEdge = tempChars[cc].bottomEdge;
   }

}



void removeTooNarrowChars( )
{
   // be carefull, 1's are very narrow.

   LPROCR_lib_CHAR_LOCATION tempChars[MAX_CANDIDATE_CHARS];

   int cc, ccNew;


   float ratio;
   float height;
   float width;


   for ( cc =0; cc < MAX_CANDIDATE_CHARS; cc++)
   {
      tempChars[cc].leftEdge = -1 ; // initialize this array for later use
      tempChars[cc].rightEdge = -1 ;
      tempChars[cc].topEdge = -1 ;
      tempChars[cc].bottomEdge = -1 ;
   }



   ccNew = 0;
   for ( cc =0; cc < MAX_CANDIDATE_CHARS; cc++)
   {
      if ( libData.candidateChars[cc].leftEdge   != -1 )
      {

         height = (float)libData.candidateChars[cc].bottomEdge - libData.candidateChars[cc].topEdge ;
         width  =(float)libData.candidateChars[cc].rightEdge - libData.candidateChars[cc].leftEdge ;
         ratio = height / width;

         if ( ratio > 20 ) 
         {
            RejectLogAdd("char too narrow");
            continue ; // just dump this char
         }
         tempChars[ccNew].leftEdge  = libData.candidateChars[cc].leftEdge ;
         tempChars[ccNew].rightEdge  = libData.candidateChars[cc].rightEdge;
         tempChars[ccNew].topEdge  = libData.candidateChars[cc].topEdge  ;
         tempChars[ccNew].bottomEdge  = libData.candidateChars[cc].bottomEdge;
         ccNew++;
      }
   }



   // copy the temp char array back 


   for ( cc =0; cc < MAX_CANDIDATE_CHARS; cc++)
   {    
      libData.candidateChars[cc].leftEdge   = tempChars[cc].leftEdge;
      libData.candidateChars[cc].rightEdge  = tempChars[cc].rightEdge;
      libData.candidateChars[cc].topEdge    = tempChars[cc].topEdge;
      libData.candidateChars[cc].bottomEdge = tempChars[cc].bottomEdge;
   }

}





void removeTooShortChars( )
{
   // if the char is much shorter than the others, its noise, remove it


   // char height histogram
   int i;
   int charHisto[MAX_CHAR_HEIGHT / 2];
   int histoSize = MAX_CHAR_HEIGHT / 2; // each bin is two pixels wide
   LPROCR_lib_CHAR_LOCATION tempChars[MAX_CANDIDATE_CHARS];
   int charHeightBin;

   int cc;
   int ccNew;

   for (cc=0; cc < histoSize; cc++)
   {
      charHisto[cc]=0;
   }

   // generate char Height hisogram
   for ( cc =0; cc < MAX_CANDIDATE_CHARS; cc++)
   {
      tempChars[cc].leftEdge = -1 ; // initialize this array for later use
      tempChars[cc].rightEdge = -1 ;
      tempChars[cc].topEdge = -1 ;
      tempChars[cc].bottomEdge = -1 ;


      if ( libData.candidateChars[cc].leftEdge   != -1 )
      {
         charHeightBin = (libData.candidateChars[cc].bottomEdge - libData.candidateChars[cc].topEdge )/2;

         LIMIT(charHeightBin,histoSize);

         charHisto[charHeightBin]++;

      }
   }

   // find the most common char Height

   int mostCommonHeightBin = 0;
   int peak=0;
   for (i=0; i < histoSize; i++)
   {
      if ( charHisto[i] > peak )
      {
         peak = charHisto[i];
         mostCommonHeightBin = i;
      }
   }

   // see if any chars are less than the most common width

   ccNew = 0;
   for ( cc =0; cc < MAX_CANDIDATE_CHARS; cc++)
   {
      if ( libData.candidateChars[cc].leftEdge   != -1 )
      {
         charHeightBin = (libData.candidateChars[cc].bottomEdge - libData.candidateChars[cc].topEdge )/2;

         if ( charHeightBin <= (int) ( (float) mostCommonHeightBin * 0.8f ) ) 
         {
            RejectLogAdd("char too short");
            continue ; // just dump this char
         }

         tempChars[ccNew].leftEdge  = libData.candidateChars[cc].leftEdge ;
         tempChars[ccNew].rightEdge  = libData.candidateChars[cc].rightEdge;
         tempChars[ccNew].topEdge  = libData.candidateChars[cc].topEdge  ;
         tempChars[ccNew].bottomEdge  = libData.candidateChars[cc].bottomEdge;
         ccNew++;
      }
   }



      // clear out the original char array
   for ( cc =0; cc < MAX_CANDIDATE_CHARS; cc++)
   {    
      libData.candidateChars[cc].leftEdge = -1;
      libData.candidateChars[cc].rightEdge  = -1;
      libData.candidateChars[cc].topEdge    = -1;
      libData.candidateChars[cc].bottomEdge = -1;
   }

       // copy the temp char array back 
   for ( cc =0; cc < MAX_CANDIDATE_CHARS; cc++)
   {    

      libData.candidateChars[cc].leftEdge   = tempChars[cc].leftEdge;
      libData.candidateChars[cc].rightEdge  = tempChars[cc].rightEdge;
      libData.candidateChars[cc].topEdge    = tempChars[cc].topEdge;
      libData.candidateChars[cc].bottomEdge = tempChars[cc].bottomEdge;
   }
   

}




void removeBlackWhiteFilled ( )
{
   // if thar char is mostly white or mostly black, its noise, remove it


   // char width histogram

   int charHisto[MAX_CHAR_WIDTH / 2];
   int histoSize = MAX_CHAR_WIDTH / 2; // each bin is two pixels wide
   LPROCR_lib_CHAR_LOCATION tempChars[MAX_CANDIDATE_CHARS];


   int cc;
   int ccNew;
   int percentBlackFill;
   int percentWhiteFill;
   int charHeight;
   int charWidth;
   float hToWRatio;

   for (cc=0; cc < histoSize; cc++)
   {
      charHisto[cc]=0;
   }

   // init temp array
   for ( cc =0; cc < MAX_CANDIDATE_CHARS; cc++)
   {
      tempChars[cc].leftEdge = -1 ;
      tempChars[cc].rightEdge = -1 ;
      tempChars[cc].topEdge = -1 ;
      tempChars[cc].bottomEdge = -1 ;
   }




   ccNew = 0;
   for ( cc =0; cc < MAX_CANDIDATE_CHARS; cc++)
   {
      if ( libData.candidateChars[cc].leftEdge   != -1 )
      {
         charWidth = libData.candidateChars[cc].rightEdge - libData.candidateChars[cc].leftEdge ;
         charHeight = libData.candidateChars[cc].bottomEdge -  libData.candidateChars[cc].topEdge ;
         hToWRatio = (float)charHeight / (float)charWidth ;

         // Eurpean ones are skinny and are full of black, so do not eliminate those
         if ( hToWRatio < 3.5 )
         {
            percentBlackFill = calculateBlackFill (  cc );
         }
         else
            percentBlackFill = 50;


         percentWhiteFill = calculateWhiteFill (  cc );

         if ( percentBlackFill > 90 || percentWhiteFill > 80) continue; // do not put this on in the temp array, its garbage

         tempChars[ccNew].leftEdge  = libData.candidateChars[cc].leftEdge ;
         tempChars[ccNew].rightEdge  = libData.candidateChars[cc].rightEdge;
         tempChars[ccNew].topEdge  = libData.candidateChars[cc].topEdge  ;
         tempChars[ccNew].bottomEdge  = libData.candidateChars[cc].bottomEdge;
         ccNew++;


      }
   }



      // clear out the original char array
   for ( cc =0; cc < MAX_CANDIDATE_CHARS; cc++)
   {    
      libData.candidateChars[cc].leftEdge = -1;
      libData.candidateChars[cc].rightEdge  = -1;
      libData.candidateChars[cc].topEdge    = -1;
      libData.candidateChars[cc].bottomEdge = -1;
   }

       // copy the temp char array back 
   for ( cc =0; cc < MAX_CANDIDATE_CHARS; cc++)
   {    

      libData.candidateChars[cc].leftEdge   = tempChars[cc].leftEdge;
      libData.candidateChars[cc].rightEdge  = tempChars[cc].rightEdge;
      libData.candidateChars[cc].topEdge    = tempChars[cc].topEdge;
      libData.candidateChars[cc].bottomEdge = tempChars[cc].bottomEdge;
   }
   

}


void trimTallChars ()
{


   // some chars get connected to plate noise on top or below, if its too tall compared to the typical char, make it the same as typical

   // char top histogram
   int i;
   int charHisto[MAX_PLATE_HEIGHT ];
   int histoSize = MAX_PLATE_HEIGHT ; // each bin is two pixels 
   int charBin;
   float slope;
   int cc;
   int projectedY, offset;
   int mostCommonTop, mostCommonBottom;


   // need to compensate for plate slope, use 0,0 as the anchor point for line projection
   slope = libData.plateSlopeT;  


   // generate char top hisogram



   for (cc=0; cc < histoSize; cc++)
   {
      charHisto[cc]=0;
   }

   // generate hisogram
   for ( cc =0; cc < MAX_CANDIDATE_CHARS; cc++)
   {

      if ( libData.candidateChars[cc].leftEdge   != -1 )
      {

         projectedY =(int)( slope * (float)libData.candidateChars[cc].leftEdge);
         offset = libData.candidateChars[cc].topEdge - projectedY;
         if ( offset <  0 ) offset = 0;

         charBin = offset ;

         LIMIT(charBin,histoSize);

         charHisto[charBin]++;

      }
   }

   // find the most common top

   int mostCommonBin=0;
   int peak=0;
   for (i=0; i < histoSize; i++)
   {
      if ( charHisto[i] > peak )
      {
         peak = charHisto[i];
         mostCommonBin = i;
      }
   }



   // see if any char tops are off from the projected most common top

   if ( peak > 2 ) // more than two chars are required to set a trend
      for ( cc =0; cc < MAX_CANDIDATE_CHARS; cc++)
      {
         if ( libData.candidateChars[cc].leftEdge   != -1 )
         {
            projectedY = (int)(slope * (float)libData.candidateChars[cc].leftEdge);
            mostCommonTop = ( mostCommonBin) + projectedY;

            if ( libData.candidateChars[cc].topEdge  < mostCommonTop-1 && libData.candidateChars[cc].topEdge  > mostCommonTop+1 )
            {
               libData.candidateChars[cc].topEdge   = mostCommonTop;
            }

         }
      }


      //   char bottom


      // generate char bottom hisogram

      cc = 0;
      while ( cc < MAX_CANDIDATE_CHARS-1 && libData.candidateChars[cc].leftEdge != -1 )
      {
         cc++;
      }


      for (cc=0; cc < histoSize; cc++)
      {
         charHisto[cc]=0;
      }

      // generate hisogram
      for ( cc =0; cc < MAX_CANDIDATE_CHARS; cc++)
      {

         if ( libData.candidateChars[cc].leftEdge   != -1 )
         {

            projectedY =(int)( slope * (float)libData.candidateChars[cc].leftEdge);
            offset = libData.candidateChars[cc].bottomEdge - projectedY;
            if ( offset <  0 ) offset = 0;

            charBin = offset ;

            LIMIT(charBin,histoSize);

            charHisto[charBin]++;

         }
      }

      // find the most common bottom

      mostCommonBin=0;
      peak=0;
      for (i=0; i < histoSize; i++)
      {
         if ( charHisto[i] > peak )
         {
            peak = charHisto[i];
            mostCommonBin = i;
         }
      }



      // see if any char tops are off from the projected most common bottom
      if ( peak > 2 ) // more than two chars are required to set a trend
         for ( cc =0; cc < MAX_CANDIDATE_CHARS; cc++)
         {
            if ( libData.candidateChars[cc].leftEdge   != -1 )
            {
               projectedY = (int)(slope * (float)libData.candidateChars[cc].leftEdge);
               mostCommonBottom = ( mostCommonBin) + projectedY;

               int * srcP;
               int upper, lower, charLeft, charRight, blkCnt,h;

               if ( libData.candidateChars[cc].bottomEdge  < mostCommonBottom-1 || libData.candidateChars[cc].bottomEdge  > mostCommonBottom+1 )
               {
                  // we are adding space to the bottom of the character, make sure its not white space we are adding, that would be a mistake

                  h = libData.plateImageHeight;
                  srcP = libData.binarizedImageClean ;
                  charLeft = libData.candidateChars[cc].leftEdge;
                  charRight = libData.candidateChars[cc].rightEdge;
                  if ( libData.candidateChars[cc].bottomEdge > mostCommonBottom )
                  {
                     upper = mostCommonBottom ;
                     lower = libData.candidateChars[cc].bottomEdge;
                  }
                  else
                  {
                     lower = mostCommonBottom ;
                     upper = libData.candidateChars[cc].bottomEdge;
                  }

                  blkCnt = 0;
                  int x, xo, y;
                  for ( x = charLeft; x <= charRight; x++)
                  {
                     xo = x * h;
                     for ( y = upper; y <= lower; y++)
                        if (  *(srcP + xo + y ) == 0 ) blkCnt++;
                  }

                  if ( blkCnt != 0 )  libData.candidateChars[cc].bottomEdge   = mostCommonBottom;
               }

            }
         }


}



void splitDoubleWides ( )
{
   // some chars get connected by noise, split them in two


   // char width histogram
   int i;
   int charHisto[MAX_CHAR_WIDTH / 2];
   int histoSize = MAX_CHAR_WIDTH / 2; // each bin is two pixels wide
   LPROCR_lib_CHAR_LOCATION tempChars[MAX_CANDIDATE_CHARS];
   int charWidthBin;

   int histEntryCount = 0;// total histo weight
   int cc;
   int ccNew;

   float height;
   float width;
   float ratio;

   for (cc=0; cc < histoSize; cc++)
   {
      charHisto[cc]=0;
   }

   // generate char width hisogram
   for ( cc =0; cc < MAX_CANDIDATE_CHARS; cc++)
   {
      tempChars[cc].leftEdge = -1 ; // initialize this array for later use
      tempChars[cc].rightEdge = -1 ;
      tempChars[cc].topEdge = -1 ;
      tempChars[cc].bottomEdge = -1 ;

      if ( libData.candidateChars[cc].leftEdge   != -1 )
      {

         // if its a one, do not use it in the histogram

         height = (float)libData.candidateChars[cc].bottomEdge - libData.candidateChars[cc].topEdge ;
         width  =(float)libData.candidateChars[cc].rightEdge - libData.candidateChars[cc].leftEdge ;
         ratio = height / width;

         if ( ratio > 4 ) continue ; // just skip

         histEntryCount++ ; // how many times did we count a char width

         charWidthBin = (libData.candidateChars[cc].rightEdge - libData.candidateChars[cc].leftEdge )/2;

         LIMIT(charWidthBin,histoSize);

         charHisto[charWidthBin]++;

      }
   }

   // find the most common char width

   int mostCommonWidthBin=0;
   int peak=0;
   for (i=0; i < histoSize; i++)
   {
      if ( charHisto[i] > peak )
      {
         peak = charHisto[i];
         mostCommonWidthBin = i;
      }
   }

   // see if any chars are 1.5 times wider than the most common width

   ccNew = 0;
   float threshold;
   for ( cc =0; cc < MAX_CANDIDATE_CHARS && ccNew < MAX_CANDIDATE_CHARS; cc++)
   {
      if ( libData.candidateChars[cc].leftEdge   != -1 )
      {
         charWidthBin = (libData.candidateChars[cc].rightEdge - libData.candidateChars[cc].leftEdge )/2;

         threshold = (float) mostCommonWidthBin * 1.8f;
         if ( (float) charWidthBin >=  threshold  && histEntryCount > 4 )  // histEntryCount > 4 ensures we have a realy history to use
         {
            // need to split this char into two chars

            tempChars[ccNew].leftEdge  = libData.candidateChars[cc].leftEdge;
            tempChars[ccNew].rightEdge  = (libData.candidateChars[cc].leftEdge + libData.candidateChars[cc].rightEdge)/2  - 2;
            tempChars[ccNew].topEdge  = libData.candidateChars[cc].topEdge  ;
            tempChars[ccNew].bottomEdge  = libData.candidateChars[cc].bottomEdge;

            ccNew++;

            tempChars[ccNew].leftEdge  = (libData.candidateChars[cc].leftEdge + libData.candidateChars[cc].rightEdge)/2 + 2;
            tempChars[ccNew].rightEdge  = libData.candidateChars[cc].rightEdge;
            tempChars[ccNew].topEdge  = libData.candidateChars[cc].topEdge  ;
            tempChars[ccNew].bottomEdge  = libData.candidateChars[cc].bottomEdge;
            ccNew++;

         }
         else  // just copy to the temp array
         {            
            tempChars[ccNew].leftEdge  = libData.candidateChars[cc].leftEdge ;
            tempChars[ccNew].rightEdge  = libData.candidateChars[cc].rightEdge;
            tempChars[ccNew].topEdge  = libData.candidateChars[cc].topEdge  ;
            tempChars[ccNew].bottomEdge  = libData.candidateChars[cc].bottomEdge;
            ccNew++;
         }


      }
   }



   
      // clear out the original char array
   for ( cc =0; cc < MAX_CANDIDATE_CHARS; cc++)
   {    
      libData.candidateChars[cc].leftEdge = -1;
      libData.candidateChars[cc].rightEdge  = -1;
      libData.candidateChars[cc].topEdge    = -1;
      libData.candidateChars[cc].bottomEdge = -1;
   }

       // copy the temp char array back 
   for ( cc =0; cc < MAX_CANDIDATE_CHARS; cc++)
   {    

      libData.candidateChars[cc].leftEdge   = tempChars[cc].leftEdge;
      libData.candidateChars[cc].rightEdge  = tempChars[cc].rightEdge;
      libData.candidateChars[cc].topEdge    = tempChars[cc].topEdge;
      libData.candidateChars[cc].bottomEdge = tempChars[cc].bottomEdge;
   }
   



}


int calculateWhiteFill (  int cc )
{

   int xStart;
   int xEnd;
   int yStart;
   int yEnd;
   int x,y,offset;
   int h;

   int whiteCnt=0;
   int totalCnt = 0;

   int *srcP;

   xStart   = libData.candidateChars[cc].leftEdge;
   xEnd     = libData.candidateChars[cc].rightEdge;
   yStart   = libData.candidateChars[cc].topEdge ;
   yEnd     = libData.candidateChars[cc].bottomEdge;

   srcP = libData.binarizedImageClean ;
   h =  libData.plateImageHeight;

   for ( x=xStart; x < xEnd; x ++)
   {
      offset = x * h;
      for (y=yStart; y < yEnd; y++)
      {
         totalCnt++;
         if ( *(srcP+y+offset)== 255 ) whiteCnt++;  
      }
   }

   if ( totalCnt == 0 ) return (100);

   return (  (100 * whiteCnt)/totalCnt );
}



int calculateBlackFill (   int cc )
{

   int xStart;
   int xEnd;
   int yStart;
   int yEnd;
   int x,y,offset;
   int h;

   int blackCnt=0;
   int totalCnt = 0;

   int *srcP;

   xStart   = libData.candidateChars[cc].leftEdge;
   xEnd     = libData.candidateChars[cc].rightEdge;
   yStart   = libData.candidateChars[cc].topEdge ;
   yEnd     = libData.candidateChars[cc].bottomEdge;

   srcP = libData.binarizedImageClean ;
   h =  libData.plateImageHeight;

   for ( x=xStart; x < xEnd; x ++)
   {
      offset = x * h;
      for (y=yStart; y < yEnd; y++)
      {
         totalCnt++;
         if ( *(srcP+y+offset)== 0 ) blackCnt++;  
      }
   }

   if ( totalCnt == 0 ) return (100);

   return (  (100*blackCnt)/totalCnt );
}





void findCharTop (  int cc )
{
   int x, y, xo;
   int w, h;
   int line=0;
   int width;
   int threshold;
   int * cleanImage;

   cleanImage = libData.binarizedImageClean;

   w = libData.plateImageWidth;
   h = libData.plateImageHeight;

   width = libData.candidateChars[cc].rightEdge - libData.candidateChars[cc].leftEdge;

   threshold = (width) * 255;

   for ( y = h/2; y > 0; y--)
   {
      line = 0;
      for ( x = libData.candidateChars[cc].leftEdge; x < libData.candidateChars[cc].rightEdge; x++)
      {
         xo = x * h;
         line += *(cleanImage+xo+y);
      }
      if ( line >= threshold ) break;
   }

   libData.candidateChars[cc].topEdge = y;


}



void findCharBottom ( int cc)
{
   int x, y, xo;
   int w, h;
   int line=0;
   int width;
   int threshold;
   int * cleanImage;

   cleanImage = libData.binarizedImageClean;

   w = libData.plateImageWidth;
   h = libData.plateImageHeight;

   width = libData.candidateChars[cc].rightEdge - libData.candidateChars[cc].leftEdge;

   threshold = (width) * 255;// was (width) * 255


   for ( y = h/2; y < h; y++)
   {
      line = 0;
      for ( x = libData.candidateChars[cc].leftEdge; x < libData.candidateChars[cc].rightEdge; x++)
      {
         xo = x * h;
         line += *(cleanImage+xo+y);
      }
      if ( line >= threshold ) break;
   }

   libData.candidateChars[cc].bottomEdge = y ;
}





//void findTilt (  int * error)
//{
//
//    int x, y,xo;
//    int w,h;
//    int *image;
//    int maxSum;
//    int maxAngle=0;
//    int bestAngle=0;
//    int bestY;
//    int yAtXAbove,  yAtXBelow, yAtX;
//
//
//    float slope;
//
//    // look for the best edge with black on top and white on bottom we can find near the top of the plate
//    //  if the edge is not strong enough just assume zero slope.
//
//    int angle[] = {-14, -12,-10, -8, -6, -7, -5, -3, -2, 0, + 2, + 3, +5, +6,7, + 8, 10, 12, 14};
//    int angle0  = 9;
//    int numAngles = 19;
//    int bestSumPerAngle [19];
//    int bestYPerAngle [19];
//
//    int a;
//    int yStart, yEnd;
//    int xStart, xEnd;
//    int *lineSum;
//
//
//
//    w = libData.plateImageWidth;
//    h = libData.plateImageHeight;
//    image = libData.plateImage;
//    // image = binImage;
//    int bestWhiteLine =0;
//    
//    libData.plateSlope =0;
//
//    lineSum = (int *)malloc ( sizeof(int) * (h +1));
//    if ( lineSum == 0 )
//    {
//        *error = 1;
//        return;
//    }
//
//   
//    xStart = 10;
//    xEnd = w - 10;
//
//    bestWhiteLine = (xEnd - xStart) * 255;
//
//    
//
//    yStart = 0;
//    yEnd = h/4;
//
//
//    for (a=0; a < numAngles; a++)
//    {
//        bestSumPerAngle [a] = 0;
//
//        slope = (float)angle[a] / (float)w;
//        if ( slope > MAX_SLOPE ) slope =  MAX_SLOPE ;
//        if ( slope < MIN_SLOPE) slope = MIN_SLOPE ;
//
//        for ( y = yStart; y < yEnd ; y+=2)
//        {
//            lineSum[y] =0;
//
//            for (x=xStart; x < xEnd; x++)
//            {
//                xo= x * h;
//
//                yAtXAbove =(int)( (slope * (float)x ))+ y;
//                yAtXBelow =(int)( (slope * (float)x ))+ y + 5;
//
//                LIMIT(yAtXAbove,h);
//                LIMIT(yAtXBelow,h);
//
//                // white minus black = postive number
//                lineSum[y] +=*(image+ xo + yAtXBelow) -  *(image+ xo + yAtXAbove);
//            }
//        }
//
//        // find the best line sum for this angle
//
//        maxSum = 0;
//
//      //  bestY = angle0 ;//???????????????/ why did I do this?
//        bestY = 0;
//
//        for ( y = yStart; y < yEnd ; y++)
//        {
//            if ( lineSum[y]> maxSum )
//            {
//                maxSum = lineSum[y];
//                bestY = y;
//            }
//         
//        }
//
//        bestSumPerAngle [a] = maxSum;
//        bestYPerAngle [a] = bestY;
//
//    }
//
//    // find the best angle 
//
//    maxAngle = 0;
//    for (a=0; a< numAngles; a++)
//    {
//
//        if ( bestSumPerAngle[a] > maxAngle)
//        {
//            maxAngle = bestSumPerAngle[a];
//            bestAngle  = a;
//        }
//    }
//
//    if ( bestSumPerAngle[bestAngle] < 7500)
//    {
//        // the edge was not strong enought to qualify as a good quality angle estimate to bale out
//        bestAngle = angle0;
//    }
//
//    // if there is a tie with 0, prefer 0
//
//    if ( bestSumPerAngle[angle0] == maxAngle ) bestAngle = angle0;
//
//    slope =  (float)angle[bestAngle] / (float)w;
//    if ( slope > 0.063f ) slope = 0.063f ;
//    if ( slope < -0.063f) slope = -0.063f;
//
//    LIMIT(bestYPerAngle[bestAngle],h);
//
//    yAtX = (int)(slope * (float)xEnd ) + bestYPerAngle[bestAngle];
//
//    LIMIT(yAtX,h);
//
//    libData.plateSlope = slope;
//
//    plate_diags_PlotLine (  xStart, bestYPerAngle[bestAngle] , xEnd,  yAtX  );
//
//    freeMemory (& lineSum);
//
//}


void blobLetters (  int *binImage )
{

   int x,y, xo, yy,xx;
   bool whiteToLeft, whiteToRight;
   int xom, xop;
   int * image;
   int w,h;
   int top, bottom;
   int blackCnt=0;
   int distanceOnTop = 0;
   int distanceOnBottom = 0;
   int yAtX=0;
   float slope;
   int blkCntLastCol = 0;
   float blkCntGradient;

   w = libData.plateImageWidth;
   h = libData.plateImageHeight;
   image = libData.plateImage;
   top = libData.estCharTop;
   bottom = libData.estCharBottom;
   slope = libData.plateSlopeT;



   // make black blobs of columns between top and bottom that have some black in them

   xo = 0;
   blackCnt =9999;

   for ( x=2; x < w-2; x++)
   {

      xo = x * h;
      blkCntLastCol = blackCnt;
      blackCnt = 0;
      for(y=top; y < bottom; y++)
      {
         yAtX = (int)(slope * (float)x) + y;
         LIMIT(yAtX,h);

         if ( *(binImage+xo+yAtX) == 0) blackCnt++;

      }

      if ( blkCntLastCol == 0 )
         blkCntGradient = 1.0f;
      else
         blkCntGradient = (float)blackCnt / (float)blkCntLastCol;

      if ( blackCnt > 1 /* && ( blkCntGradient > .1f    ) */)  // was  blackCnt > 0
      {
         for(y=top; y < bottom; y++)
         {
            yAtX =(int) (slope * (float)x) + y;
            LIMIT(yAtX,h);

            *(binImage+xo+yAtX) = 0 ;
         }   
      }
      else // make it white
      {
         for(y=top; y < bottom; y++)
         {
            yAtX = (int)(slope * (float)x) + y;
            LIMIT(yAtX,h);

            *(binImage+xo+yAtX) = 255 ;
         }  

      }

   }

   // now go back and find any vertical black lines/blobs that are 4 pixels or less wide, and delete them


   xo = 0;
   for ( x=5; x < w-5; x++)
   {

      xo = x * h;
      blackCnt = 0;

      y = (top + bottom) / 2;

      if ( *(binImage+xo+y) == 0)
      {
         whiteToLeft = false;
         whiteToRight = false;

         // this col is black, is there a white to the left?

         for ( xx =x-3; xx < x; xx++)
         {
            xom = xx * h;

            if ( *(binImage+xom+y) == 255)
            {
               whiteToLeft = true;
            }
         }

         for ( xx =x+1; xx < x+3; xx++)
         {
            xop = xx * h;
            if ( *(binImage+xop+y) == 255)
            {
               whiteToRight = true;
            }
         }

         if ( whiteToLeft  && whiteToRight)
         {
            for(yy=top; yy < bottom; yy++)
            {
               *(binImage+xo+yy) = 255 ;
            }   
         }
      }
   }

}




void removeDots(  int *binImage )
{

   int x,y, offset;
   int * image;
   int w,h;

   w = libData.plateImageWidth;
   h = libData.plateImageHeight;
   image = libData.plateImage;


   int xm2, xp2, ym2, yp2;
   int d1,d2,d3,d4,d5,d6,d7,d8;
   int xx, yy, xxo;
   int xom2, xop2;
   int sum;



   offset = 0;
   for ( x=2; x < w-2; x++)
   {

      xm2 = x - 2;
      xp2 = x + 2;
      xom2 = xm2 * h;
      xop2 = xp2 * h;

      offset = x * h;

      for(y=2; y < h-2; y++)
      {

         ym2 = y - 2;
         yp2 = y + 2;

         d1 = *(binImage+xom2+ym2) - *(binImage+offset+y)>0? 1: 0;
         d2 = *(binImage+offset+ym2) - *(binImage+offset+y)>0? 1: 0;
         d3 = *(binImage+xop2+ym2) - *(binImage+offset+y)>0? 1: 0;
         d4 = *(binImage+xop2+y) - *(binImage+offset+y)>0? 1: 0;
         d5 = *(binImage+xop2+yp2) - *(binImage+offset+y)>0? 1: 0;
         d6 = *(binImage+offset+yp2) - *(binImage+offset+y)>0? 1: 0;
         d7 = *(binImage+xom2+yp2) - *(binImage+offset+y)>0? 1: 0;
         d8 = *(binImage+xom2+y) - *(binImage+offset+y)>0? 1: 0;


         sum = d1 + d2 + d3 + d4 + d5 + d6 + d7 + d8;

         if ( sum > 7 )
         {
            for ( xx= x -2 ; xx < x + 2; xx++)
            {
               xxo = x * h;
               for ( yy= y-2; yy < y + 2; yy++)
               {
                  *(image+xxo+y) = 255;
                  *(binImage+xxo+y) = 255;
               }

            }

         }

      }

   }

}


void removeBolts(  int *binImage )
{

   int x,y, offset;
   int * image;
   int w,h;

   w = libData.plateImageWidth;
   h = libData.plateImageHeight;
   image = libData.plateImage;


   int xm3, xp3, ym3, yp3;
   int d1,d2,d3,d4,d5,d6,d7,d8;
   int xx, yy, xxo;
   int xom3, xop3;
   int sum;


   int span=6;

   offset = 0;
   for ( x=span; x < w-span; x++)
   {

      xm3 = x - span;
      xp3 = x + span;
      xom3 = xm3 * h;
      xop3 = xp3 * h;

      offset = x * h;

      for(y=span; y < h-span; y++)
      {

         ym3 = y - span;
         yp3 = y + span;

         d1 = *(binImage+xom3+ym3) - *(binImage+offset+y)>0? 1: 0;
         d2 = *(binImage+offset+ym3) - *(binImage+offset+y)>0? 1: 0;
         d3 = *(binImage+xop3+ym3) - *(binImage+offset+y)>0? 1: 0;
         d4 = *(binImage+xop3+y) - *(binImage+offset+y)>0? 1: 0;
         d5 = *(binImage+xop3+yp3) - *(binImage+offset+y)>0? 1: 0;
         d6 = *(binImage+offset+yp3) - *(binImage+offset+y)>0? 1: 0;
         d7 = *(binImage+xom3+yp3) - *(binImage+offset+y)>0? 1: 0;
         d8 = *(binImage+xom3+y) - *(binImage+offset+y)>0? 1: 0;


         sum = d1 + d2 + d3 + d4 + d5 + d6 + d7 + d8;

         if ( sum > 5)
         {
            for ( xx= x -span ; xx < x + span; xx++)
            {
               xxo = x * h;
               for ( yy= y-span; yy < y + span; yy++)
               {
                  *(image+xxo+y) = 255;
                  *(binImage+xxo+y) = 255;
               }

            }

         }

      }

   }



}





void findEstimatedTopAndBottom(   int * inPtr,  int * error)
{

   int x,y, offset;
   int * rowSum;

   int topPeak;
   //  int topMin=0;
   //  int bottomMin=0;
   int bottomPeak;

   int threshold;
   int topOfChars=0;
   int bottomOfChars;
   int yStart;
   int yStop;
   int yMiddleTop;
   int yMiddleBottom;
   int w, h;
   int yAtX;
   int d=0;

   float slope;

   //    int debugArray[500];

   slope = libData.plateSlopeT;

   w = libData.plateImageWidth;
   h = libData.plateImageHeight;

   // sum all the rows 

   rowSum = (int *) malloc ( sizeof(int) * h );
   if (rowSum  == 0  )
   {
      *error = 1;
      return;
   }

   offset = 0;
   int prevVal = 0;

   if ( libData.USStyle == true)
   {
      yStart = h/16;
      yStop = h - (h/16);
   }
   else
   {
      yStart = 0;
      yStop = h;
   }

   //test
   yStart = h/8;


   yMiddleTop = (h/2) - (h/16);
   yMiddleBottom = (h/2)+ (h/16);

   for(y=yStart; y < yStop; y++)
   {
      rowSum[y] = 0;

      for ( x=0; x < w; x++)
      {
         offset = x * h;

         yAtX = (int)(slope * (float)x) + y;
         LIMIT(yAtX,h);


         // sum the differences i.e. edges along a row

         d = ( *(inPtr+offset+yAtX) - prevVal );
         if ( d < 0 ) d = -1 * d;
         rowSum[y] +=  d;
         prevVal = *(inPtr+offset+yAtX);
      }
   }


   // plot the row sums

   plate_diags_PlotSumCurve (  rowSum  );



   topPeak =0;

   for(y=yStart; y < yMiddleTop; y++)
   {
      if ( rowSum[y] != 0 )
         if (  rowSum[y] > topPeak )
            topPeak = rowSum[y];
   }


   // find the top - start in the middle and move up to 
   threshold = (30 * topPeak) / 100;  // 
   for(y=yMiddleTop; y >= yStart ; y--)
   {
      if (  rowSum[y] <= threshold )
      {
         topOfChars = y;
         break;
      }
   }


   libData.estCharTop = topOfChars;

   // plot the top of chars

   yAtX = (int)(slope * (float)(w - w/5)) + topOfChars;
   LIMIT(yAtX,h);


   plate_diags_PlotLine (  0, topOfChars, w - w/5, yAtX  );



   // get the peak sum on the bottom half


   bottomPeak = 0;

   for(y=yMiddleBottom; y < yStop; y++)
   {
      if ( rowSum[y] != 0 )
         if (  rowSum[y] > bottomPeak )
            bottomPeak = rowSum[y];
   }


   // find the bottom - start in the middle and move up to 
   threshold = (30 * bottomPeak) / 100; //95

   bottomOfChars = h-1;

   for(y=yMiddleBottom; y < yStop; y++)
   {
      if (  rowSum[y] < threshold )
      {
         bottomOfChars = y;
         break;
      }
   }


   libData.estCharBottom = bottomOfChars;


   // plot the bottom of chars

   yAtX = (int)(slope * (float)(w - w/5)) + bottomOfChars;
   LIMIT(yAtX,h);

   plate_diags_PlotLine (   0, bottomOfChars, w - w/5, yAtX  );



   freeMemory(& rowSum);

}



void histogram ( int * inPtr, int w, int h, int * outPtr, int * integration)
{

   int V;
   int x,y, offset;
   int i;
   int sum = 0;
   int xStart=0;
   int xEnd = 0;
   int yStart = 0;
   int yEnd = 0;


   for ( V=0; V < 256; V++)
   {
      (outPtr)[V]=0;
   }

   // Xcenter +/- 50
   xStart = (w/2) - 50;
   if (xStart < 0 ) xStart = 0;

   xEnd = (w/2) + 50;
   if ( xEnd >= w ) xEnd = w-1;

   // Y center +/- 10
   yStart = h/2 - 10;  //-10
   if ( yStart < 0 ) yStart = 0;

   yEnd = h/2 + 10;   // + 10
   if ( yEnd >= h) yEnd = h-1;

   offset = 0;
   for ( x=xStart; x < xEnd; x++)
   {
      offset = x * h;
      for(y=yStart; y < yEnd; y++)
      {

         V = *(inPtr+offset+y) & 255;

         (outPtr)[V]++;

      }
   }

   sum = 0;
   for (i = 0; i < 256; i++)
   {
      sum += (outPtr)[i];
      integration[i] = sum;
   }


}


void binarize( int * inPtr, int w, int h, int **binImage, int * error)
{


   int x, y,xo,j;
   int area = 0;

   int histo[256];

   int integration[256];

   int darkEnergyThreshold=0;

   freeMemory (binImage );
   *binImage = (int *) malloc ( sizeof(int) * w * h);
   if (*binImage == 0  )
   {
      *error = 1;
      return;
   }

   // find the brightest and darkest pixels

   histogram ( inPtr,  w, h, histo, integration);


   // find where the integration hits some % of the max area
   area = integration[255];
   darkEnergyThreshold = 0;
   for ( j = 0; j < 256; j++)
   {
      if ( integration[j] > ( (35 * area) / 100))
      {
         darkEnergyThreshold = j;
         break;
      }
   }



   for ( x=0; x < w; x++)
   {
      xo = x * h;
      for(y=0; y < h; y++)
      {  
         if ( *( inPtr + xo +y) <  darkEnergyThreshold )
            *(*binImage+ xo +y) = 0;
         else  
            *(*binImage+ xo +y) = 255;
      }
   }

}

//void binarize( int * inPtr, int w, int h, int **binImage, int majicNumber, int * error)
//{
//
//    int x,y, offset;
//    int ave=0;
//    int aveCnt = 0;
//
//    int cutoff1 = 0;
//    int cutoff2 = 0;
//
//    int xStart=0;
//    int xEnd = 0;
//    int yStart = 0;
//    int yEnd = 0;
//    int histo[256];
//
//
//
//    freeMemory (binImage );
//    *binImage = (int *) malloc ( sizeof(int) * w * h);
//    if (*binImage == 0  )
//    {
//        *error = 1;
//        return;
//    }
//
//    histogram ( inPtr,  w, h, histo, & ave);
//
//
//    cutoff1  = ave - majicNumber; //was 35, 30 was the majic number from the July release
//    cutoff2 = cutoff1 + 1;
//
//
//    offset = 0;
//    for ( x=0; x < w; x++)
//    {
//        offset = x * h;
//        for(y=0; y < h; y++)
//        {  
//
//            if ( *(inPtr+offset+y) <= cutoff1 )
//                *(*binImage+offset+y) = 0;
//            else  if ( *(inPtr+offset+y) >= cutoff2 )
//                *(*binImage+offset+y) = 255;
//
//        }
//    }
//
//
//}

void CopyImage (int * src , int * dst, int w, int h)
{
   int x,xo,y;

   for ( x=0; x < w; x++)
   {
      xo = x * h;
      for(y=0; y < h; y++)
      {  
         *(dst+xo+y) = *(src+xo+y) ;
      }
   }

}