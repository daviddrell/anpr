#include <afx.h>
#include <stdlib.h>
#include <malloc.h>
#include "LPROCR_Public.h"
#include "LPROCR_Structs.h"
#include "LPROCR_Error_Codes.h"
#include "LPROCR_Diags.h"
#include "charIsolationMethods.h"

#define OCR_MAIN
#include "ocr.h"


int * diagnosticChar=0;

#define NUM_ROTATIONS 7
double rotations[NUM_ROTATIONS] = {15,14,13,  0,-13,-14,-15}; // for unrolling
// unrolling works better for chars -  double rotations[NUM_ROTATIONS] = { .1, 0.05, 0.04, 0, -0.05, -0.04, -.1}; // for rotation


int OCR_lib_GetNumberRotations ( )
{
    return (  NUM_ROTATIONS);
}



int OCR_lib_TestRotationBestFit ( int * cImageRaw, int w, int h, int * error  )
{
    float score=0;
    float maxScore =0;
    int bestRotation= -1;

    int rotationIndex = 0;

    int * cImageSS ; // character image scaled to standard h & w
    int * rotatedImage;


    cImageSS = (int *) malloc ( sizeof(int) * STANDARD_WIDTH * STANDARD_HEIGHT);
    if ( cImageSS == 0 )
    {
        *error = 1;
        return (0);
    }

    rotatedImage = (int *) malloc ( sizeof(int) * STANDARD_WIDTH * STANDARD_HEIGHT);
    if (rotatedImage == 0 )
    {
        *error = 1;
        return (0);
    }


    int sw, sh;
    sw = STANDARD_WIDTH;
    sh = STANDARD_HEIGHT;


    prepChar(cImageRaw, w, h, cImageSS, error );


    maxScore = 0.0;
    bestRotation = -1;

    for (rotationIndex = 0; rotationIndex < NUM_ROTATIONS; rotationIndex ++)
    {

        copyChar (cImageSS, rotatedImage, sw, sh);
        
        //LPROCR_lib_RotateImage (rotatedImage,  sw,  sh, rotations[ rotationIndex],  error );//
        
        LPROCR_lib_unroll (rotatedImage,  sw,  sh, (float) rotations[ rotationIndex],  error, 0  );

        scoreMatchOnRotationTestChar(rotatedImage, sw, sh,  & score);

        if ( score > maxScore)
        {
            maxScore =  score ;
            bestRotation = rotationIndex ;
        }
    }

    freeMemory (&cImageSS);
    freeMemory (&rotatedImage);

    return(bestRotation );

}


char LPROCR_lib_ReadThisChar ( int * cImageRaw, int w, int h, float * score, int aveCharWidth,  int * displayChar, int * error )
{
   
    int bestMatch;  
 
    int * cImageSS=0 ; // character image scaled to standard h & w
 

    cImageSS = (int *) malloc ( sizeof(int) * STANDARD_WIDTH * STANDARD_HEIGHT);
    if ( cImageSS == 0 )
    {
        *error = 1;
        *score = 0;
        return (0);
    }


    int sw, sh;
    sw = STANDARD_WIDTH;
    sh = STANDARD_HEIGHT;

    if ( w == 0 || h == 0 ) return(0);

    if ((10*h) / w > 34)  // at 30, some new york chars get turned into ones
    {
        if (  aveCharWidth - w > 2 )
        {
            bestMatch = '1';
            *score = 100000;
            freeMemory (&cImageSS);
            return ((char)bestMatch);
        }
    }


    prepChar(cImageRaw, w, h, cImageSS, error);


    bestMatch  = findBestMatch(cImageSS, sw, sh, displayChar, score);

    freeMemory (&cImageSS);
 

    return((char)bestMatch);

}

void scoreMatchOnRotationTestChar( int * in, int w, int h , float * score )
{ 
    
    int x;
    int y;
    int xo;
    float bestPossibleScore=0;
    float sum=0;
    int val = 0;

       

    sum = 0;

    for ( x = 0; x < 4; x++)
    {
        xo = x * h;
        for(y=0; y < 7; y++)
        {
            if (  * (in+xo+y) < 0 ) //its black
                val = 500;
            else
                val = - 500;
            sum +=  val;
            bestPossibleScore +=  500;
        }
    }

    for ( x = 0; x < 4; x++)
    {
        xo = x * h;
        for(y=34; y < 40; y++)
        {
            if (  * (in+xo+y)  < 0 ) //its black
                val = 500;
            else
                val = - 500;

            sum +=  val;
            bestPossibleScore +=  500;
        }
    }

    
    for ( x = 17; x < 20; x++)
    {
        xo = x * h;
        for(y=0; y < 7; y++)
        {
            if (  * (in+xo+y) < 0 ) //its black
                val = 500;
            else
                val = - 500;
            sum +=  val;
            bestPossibleScore +=  500;
        }
    }

    for ( x = 17; x < 20; x++)
    {
        xo = x * h;
        for(y=34; y < 40; y++)
        {
            if (  * (in+xo+y)  < 0 ) //its black
                val = 500;
            else
                val = - 500;

            sum +=  val;
            bestPossibleScore +=  500;
        }
    }
   

    *score = sum;
 
  
}


int findBestMatch ( int * in, int w, int h , int * diagChar, float * score )
{
    int x;
    int y;
    int xo;
    int c;

    float sum[NUM_CHARS];

    float maxSum;
    int bestIndex;

    for ( c  =0 ; c < NUM_CHARS; c++)
    {

        sum[c] = 0;

        for ( x = 0; x < w; x++)
        {
            xo = x * h;
            for(y=0; y < h; y++)
            {
                sum[c] +=  m_CharLibData[c].data[x] [y]  * in[xo + y];
            }
        }

    }


    maxSum = 0;
    bestIndex = 0;
    for ( c  = 0 ; c < NUM_CHARS; c++)
    {
        if ( (sum[c] ) > maxSum)
        {
            maxSum = sum[c];
            bestIndex = c;
        }
    }

    *score = (float) maxSum;

    if ( maxSum < 30000) //150000
        return ( '-'); // the correlation is too weak, its noise, not a character

    bestIndex = secondPassCheck (  in, w,  h,  bestIndex, 0 ,  diagChar, score);



    //  createDiagImage  ( in,  bestIndex ,  diagChar);


    if ( bestIndex == -1 ) return ('-');

    if ( m_CharLibData[bestIndex].name[0] == 'O' ) return ('0'); // replace oh with zero

    return((int) m_CharLibData[bestIndex].name[0]);
}





void blurrChar (int *in,  int w, int h)
{

    int x, xo;
    int y;
    int distance;
    int left, right, above, below;

    distance = 1;

    for (x=distance; x < w-distance; x++)
    {
        xo = x * h;
        left = (x -distance) * h;
        right = (x+distance) * h;

        for(y=0; y < h; y++)
        {
            above = y -1;
            below = y + 2;

            in[xo+y] = (in[left+y]+ in[right+y]+ in[xo+above]+ in[left+below])/4;
        }
    }
}



void copyChar (int *in, int * out, int w, int h)
{

    int x, xo;
    int y;

    for (x=0; x < w; x++)
    {
        xo = x * h;
        for(y=0; y < h; y++)
        {
            out[xo+y] = in[xo+y];
        }
    }
}



void prepChar ( int * cImageRaw, int w, int h, int * cImageSS, int *error   )
{
    

    int * temp;
    temp = (int *) malloc ( sizeof(int) *STANDARD_WIDTH *STANDARD_HEIGHT);
    if ( temp == 0 ) 
    {
        *error = 1;
        return;
    }



    scaleArray(cImageRaw, w, h, temp , STANDARD_WIDTH, STANDARD_HEIGHT, error);


    normalizeToWideSpectrum2(temp,STANDARD_WIDTH, STANDARD_HEIGHT,cImageSS); // stretch the values from 0-255 to -500 to + 500 for the correlations to work


    free (temp);
}


void  binarizeChar (int* image, int w, int h)
{

    int x, y,xo;

    for (x = 0; x < w; x++)
    {
        xo = x * h;
        for (y = 0; y < h; y++)
        {
            if ( image[xo+ y] > 0 ) image[xo+ y]=  500;
            else  image[xo+ y]=  -500;
        }
    }


}// end binarizeChar

void Chistogram ( int * inPtr, int w, int h, int * outPtr, int * ave, int * area)
{

    int V;
    int x,y, offset;
    int aveCnt;

    int xStart=0;
    int xEnd = 0;
    int yStart = 0;
    int yEnd = 0;



    *ave=0;
    aveCnt=0;
    offset = 0;

    for ( V=0; V < 256; V++)
    {
        (outPtr)[V]=0;
    }


    *area = 0;

    offset = 0;
    for ( x=0; x < w; x++)
    {
        offset = x * h;
        for(y=0; y < h; y++)
        {
            (*area) ++;

            V = *(inPtr+offset+y) & 255;

            (outPtr)[V]++;
            *ave += V;
            aveCnt++;
        }
    }

    if ( aveCnt == 0 )
    {
        *ave = 0;
        return;
    }

    *ave = *ave/aveCnt;

}
//
//void normalizeToWideSpectrum2(int* luminance8bit, int w, int h, int * luminance32bit )
//{
//    
//  
//    int x, y,xo;
//    int histo[256];
//    float m;
//    int b;
//    int ave  =0;
//    int area = 0;
//    int v =0;
//    int j=0;
//    int min = 255, max = 0;
//    int darkPeak = 0, lightPeak = 0;
//    int darkPeakVal = 0, lightPeakVal = 0;
//    int threshold =0;
//   
//    m = 1000.0f / 255.0f;
//    b = -500;
//
//       
//     // is the char washed out and in need of some equalization?
//   /* for ( x=0; x < w; x++)
//    {
//        xo = x * h;
//        for(y=0; y < h; y++)
//        {  
//            if ( *(luminance8bit+xo+y)> max ) max = *(luminance8bit+xo+y);
//            if ( *(luminance8bit+xo+y)< min ) min = *(luminance8bit+xo+y);
//            ave += *(luminance8bit+xo+y);
//            area++;
//        }
//    }
//    ave = ave / area;*/
//
//    Chistogram (luminance8bit,  w,  h, histo, & ave, &area);
//
//    for ( j =0; j < ave; j++)
//    {
//        if ( histo[j] > darkPeak )
//        {
//            darkPeak = histo[j];
//            darkPeakVal = j;
//        }
//    }
//
//    for ( j =ave; j < 256; j++)
//    {
//        if ( histo[j] > lightPeak )
//        {
//            lightPeak = histo[j];
//            lightPeakVal = j;
//        }
//    }
//    
//
//  //  threshold = (darkPeakVal + ave ) / 2;
//  threshold = ave;
//
//    for ( x=0; x < w; x++)
//    {
//        xo = x * h;
//        for(y=0; y < h; y++)
//        {  
//            if ( *(luminance8bit+xo+y) <= threshold - 40  )
//                *(luminance8bit+xo+y) = (*(luminance8bit+xo+y) /2 ) & 255;
//            else if ( *(luminance8bit+xo+y) >= threshold)
//              //   *(luminance8bit+xo+y) = (*(luminance8bit+xo+y) + (255-lightPeakVal)) ;
//               *(luminance8bit+xo+y) = 255;
//
//             v = *(luminance8bit+xo+y);
//             v =  v * m + b;
//            *(luminance32bit+xo+y) =v ;
//        
//        }
//    }
//
//
//}// end normalizeToWideSpectrum


void normalizeToWideSpectrum2(int* luminance8bit, int w, int h, int * luminance32bit )
{


    int x, y;
    int offset=0;
    int scaleFactor=0;
    int histo[256];
    int ave = 0;
    int v = 0;
    int vw = 0;
    int sum=0;
    int threshold = 0;
    int peak = 0;
    int area = 0;
    bool useEqualized = false;
    int * imageSrc;
    int median = 0;
    int cutoff1, cutoff2;


    // find the brightest and darkest pixels

    Chistogram ( luminance8bit,  w, h, histo, & ave, & area);



    // now binarize

    imageSrc = luminance8bit;

    cutoff1  = ave - 25 ; // was just ave
    cutoff2 = cutoff1 + 1;// wascutoff1 + 1




    for ( x=0; x < w; x++)
    {
        offset = x * h;
        for(y=0; y < h; y++)
        {  

            if ( *(imageSrc+offset+y) <= cutoff1 )
                *(luminance32bit+offset+y) = -500;
            else  if ( *(imageSrc+offset+y) >= cutoff2 )
                *(luminance32bit+offset+y) = +500;
        }
    }


}// end normalizeToWideSpectrum


void normalizeToWideSpectrum(int* luminance8bit, int w, int h, int * luminance32bit )
{
    int brightest = 0;
    int darkest = 255;
    int x, y,xo;
    int offset=0;
    int scaleFactor=0;



    // find the brightest and darkest pixels
    for (x = 0; x < w; x++)
    {
        xo = x * h;
        for (y = 0; y < h; y++)
        {
            if (luminance8bit[xo+ y] > brightest) brightest = luminance8bit[xo+ y];
            if (luminance8bit[xo+ y] < darkest) darkest = luminance8bit[xo+ y];
        }
    }

    // if the image is all the same then do nothing and return
    if ((brightest - darkest)  == 0) return;
    ///
    /// 
    offset = -((brightest - darkest) / 2) - darkest; // center the curve around zero
    scaleFactor = (500) / (((brightest - darkest)) / 2);

    for (x = 0; x < w; x++)
    {
        xo = x * h;
        for (y = 0; y < h; y++)
        {
            luminance32bit[xo+ y] = scaleFactor * (luminance8bit[xo+ y] + offset);
        }
    }


}// end normalizeToWideSpectrum



void scaleArray(int* inArray, int w, int h, int* outArray, int newWidth, int newHeight, int * error)
{

    int*  tempArray;

    tempArray = (int *) malloc ( sizeof(int) * newWidth * h ); // scale the width first
    if ( tempArray == 0 )
    {
        *error = 1;
        return;
    }

    // do width first

    // before scaling : if this is very skinny, then its a one, do not widen, but pad on either side
    if (newWidth - w > 0)
    {
        if (h / w >= 5)  // this is a one (tall and very skinny)
            padWidthUp(inArray, w, h, tempArray, newWidth);
        else
            scaleWidthUp(inArray, w, h, tempArray, newWidth, h);
    }
    else
        scaleWidthDown(inArray,w,h, tempArray, newWidth );

    // do height
    if (newHeight - h > 0)
        scaleHeightUp(tempArray,  h, outArray, newWidth, newHeight );
    else
        scaleHeightDown(tempArray, h, outArray, newWidth, newHeight );

    free (tempArray);

} // end scaleArray


void padWidthUp(int* inArray, int w, int h, int* outArray, int newWidth)
{


    int x = 0;
    int xo = 0;
    int xxo=0;
    int y = 0;


    // how much padd on eachs side?
    int pad = (newWidth - w) / 2;

    for (x = 0; x < pad; x++)
    {
        xo = x * h;
        for (y = 0; y < h; y++)
            outArray[xo+ y] = 255; // white
    }

    int xx = 0;
    for (x = pad; x < newWidth-pad; x++)
    {
        xo = x * h;
        xxo = xx * h;
        for (y = 0; y < h; y++)
            outArray[xo+ y] = inArray[xxo+ y];

        xx++;
        if (xx >= w) xx = w-1;
    }

    for (x = newWidth-pad; x < newWidth; x++)
    {
        xo = x * h;
        for (y = 0; y < h; y++)
            outArray[xo+ y] = 255; // white
    }

}


void scaleWidthUp(int* inArray,int w,int h, int* outArray, int outWidth, int outHeight)
{
    int y = 0;
    int x = 0;
    int inX =0;
    int inXo =0;
    int outX = 0;
    int left = 0;
    int right=0;

    int outXo=0;

    float scaleFactor=0;

    scaleFactor =  (float)outWidth / w;

    for (x = 0; x < outWidth; x++)
    {
        outXo = x * outHeight;
        for(y=0; y < outHeight; y++)
        {
            *(outArray+outXo+ y) = -1;
        }
    }

    for (y = 0; y < h; y++)
    {

        for (inX = 0; inX < w; inX++)
        {

            outX = (int)((float)inX * scaleFactor);

            if ( outX >= outWidth) outX = outWidth - 1;

            outXo = outX * outHeight;

            inXo = inX * h;

            *(outArray + outXo + y) = *(inArray + inXo + y);
        }
    }


    for (x = 1; x < outWidth-1; x++)
    {
        outXo = x * outHeight;
        for(y=0; y < outHeight; y++)
        {

            if ( *(outArray +outXo + y) == -1 )
            {
                left = (x - 1) * outHeight;
                while (  *(outArray +left+ y) == -1 && left > 0 )
                {
                    left = left - outHeight;
                }
                if ( left < 0 ) left = 0;

                right = (x + 1) * outHeight;

                while (  *(outArray + right+ y) == -1  && right < (outWidth *outHeight))
                {
                    right = right + outHeight;
                    if ( right >= (outWidth *outHeight) ) break;
                }
                if ( right >= (outWidth *outHeight) ) break;

                *(outArray+outXo+ y)   = ( *(outArray+ left + y) +   *(outArray + right + y)  )/2; 
            }

        }
    }

    /* for (x = outWidth-2; x < outWidth; x++)
    {
    outXo = x * outHeight;
    for(y=0; y < outHeight; y++)
    {
    if (  *(outArray+outXo+ y) == -1 )
    {
    *(outArray+outXo+ y)   = 255;                
    }
    }
    }*/

    for (x = 0; x < outWidth; x++)
    {
        outXo = x * outHeight;
        for(y=0; y < outHeight; y++)
        {
            if (  *(outArray+outXo+ y) == -1 )
            {
                *(outArray+outXo+ y)   = 255;                
            }
        }
    }

} //  end scaleWidthUp




void scaleWidthDown(int* inArray, int w, int h, int* outArray, int newWidth )
{
    int y = 0;
    int xIn = 0;
    int xoIn = 0;
    int xOut=0;
    int xoOut=0;

    float scaleFactor = (float)newWidth / w;

    for (y = 0; y < h; y++)
    {  
        for (xOut = 0; xOut < newWidth; xOut++)
        {

            xIn = (int)((float) xOut / scaleFactor);

            if (xIn >= w) xIn = w-1;  // bounds protection

            xoOut = xOut * h;

            xoIn = xIn * h;

            *(outArray+xoOut+ y) = *(inArray+xoIn+ y);

        }
    }
}//  end scaleWidthDown


void scaleHeightDown(int* inArray,  int h, int* outArray, int newWidth, int newHeight)
{
    int x = 0;
    int xoIn =0;
    int xoOut = 0;
    int yIn = 0;

    int yOut=0;


    float scaleFactor = (float)newHeight / h;

    for (x = 0; x < newWidth; x++)
    {

        xoOut = x * newHeight;
        xoIn = x * h;

        for (yOut = 0; yOut < newHeight; yOut++)
        {

            yIn = (int)((float) yOut / scaleFactor);

            if (yIn >= h) yIn = yIn = h-1;  // bounds protection

            *(outArray+xoOut+ yOut) = *(inArray+xoIn+ yIn);

        }
    }

}//  end scaleHeightDown  



void scaleHeightUp(int*inArray,  int h, int* outArray,int outWidth, int outHeight)
{
    int x = 0;
    int xoIn=0;
    int xoOut=0;

    int inY;
    int outY;
    int above;
    int below;

    float scaleFactor=0;


    scaleFactor = (float)outHeight  /(float) h ; 

    for (x = 0; x < outWidth; x++)
    {
        xoOut = x * outHeight;

        for (outY = 0; outY < outHeight; outY++)
        {   
            outArray[xoOut + outY] = -1;
        }
    }


    for (x = 0; x < outWidth; x++)
    {
        xoIn = x * h;
        xoOut = x * outHeight;

        for (inY = 0; inY < h; inY++)
        {
            outY =  (int)((float)inY * scaleFactor);

            if( outY >= outHeight ) outY = outHeight - 1;

            outArray[xoOut+ outY] = inArray[xoIn+ inY];

        }
    }

    for (x = 0; x < outWidth; x++)
    {
        xoOut = x * outHeight;

        for (outY = 0; outY < outHeight-1; outY++)
        {   

            if ( outArray[xoOut + outY] == -1 )
            {
                // look for neighbors that are not -1
                above = outY -1;
                below = outY+1;

                while ( outArray[xoOut + above] == -1  && above > 0 )
                    above --;

                while ( outArray[xoOut + below] == -1  && below < outHeight )
                    below++;

                if ( above <  0 ) above = 0;

                if ( below < outHeight)
                    outArray[xoOut + outY] = (outArray[xoOut + above] + outArray[xoOut + below])/2;
            }
        }
    }

    for (x = 0; x < outWidth; x++)
    {
        xoOut = x * outHeight;

        for (outY = outHeight-2; outY < outHeight; outY++)
        {   

            if ( outArray[xoOut + outY] == -1 )
            {
                outArray[xoOut + outY] = 255;
            }
        }
    }
}// end scaleHeightUp


void erode(int*image, int w, int h, int *eroded )
{
    // expects binarized image with black being the letter to be eroded and background being white

    int x, y;
    int x0,xm1,xp1;
    int y0,ym1,yp1;
    int val =0;

    int *i;
    i = image;

    for (x = 0; x < w; x++)
    {
        x0 = x*h;
        xm1 = (x-1)*h;
        if  ( x-1 < 0 ) xm1 = 0;

        xp1 = (x +1)*h;
        if ( x +1 >= w ) xp1 = (w-1) * h;

        for (y = 0; y < h; y++)
        {
            y0 = y;
            ym1 = y-1;
            if( ym1 < 0 ) ym1 = 0;

            yp1 = y+1;
            if ( yp1 >= h ) yp1 = h-1;

            // are any pixels white (background) ?

            val = *(i+xm1+ym1 ) + *(i+x0+ym1) + *(i+xp1+ym1) +    *(i+xm1+y) +*(i+x0+y0) + *(i+xp1+y0) +  *(i+xm1+yp1) +*(i+x0+yp1) + *(i+xp1+yp1);
            if ( val > 0 ) *(eroded+x0 + y0 ) = 255;

        }
    }


}// end







