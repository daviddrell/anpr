#include <afx.h>
#include <stdlib.h>
#include <malloc.h>
#include "LPROCR_Public.h"
#include "LPROCR_Error_Codes.h"
#include "LPROCR_Diags.h"
#include "LPROCR_Structs.h"


//extern int * imageFullRes; // main source image
//extern int *imageSub; // subscaled main source image
//extern int memoryAllocated;

extern libInstanceVariables libData;

struct DIAG_COLOR_IMAGE  // allows drawing colored lines over the diag image (which starts as monochrome)
{
    int red;
    int blue;
    int green;
};

//
// method LPROCR_diags_getImage
//

void _stdcall LPROCR_diags_getImage( int fullSub, char * red, char * green, char * blue, int * error) 
{
    int x,y, offset;
    int w, h;

    DIAG_COLOR_IMAGE *image;

    *error = ERROR_NO_ERROR;

    if ( libData.diagnosticsInitialized == 0 )
    {
        *error = ERROR_memoryNotAllocated  ;
        return;
    }

    if ( fullSub == 1 )
    {
        // full scale image
       // image = (DIAG_COLOR_IMAGE *)libData.imageFullResDiag;
         image = (DIAG_COLOR_IMAGE *)libData.edgeMapFullResDiag;
        w  = libData.imageWidth;
        h = libData.imageHeight;
    }
    else
    {      // subscalled image
        image = (DIAG_COLOR_IMAGE *)libData.imageSubDiag;
        w  = libData.imageSubWidth;
        h = libData.imageSubHeight;
    }



    // copy luminance array
    offset = 0;
    for ( x=0; x < w; x++)
    {
        for(y=0; y < h; y++)
        {
            *(red+offset+y) = (char)(image+offset+y)->blue;
            *(green+offset+y) = (char)(image+offset+y)->green;
            *(blue+offset+y) = (char)(image+offset+y)->red;  
        }
        offset += y;
    }

}


int _stdcall LPROCR_diags_GetNumCandidatePlates( ) 
{
    return (MAX_CANDIDATE_PLATES );
}



void _stdcall LPROCR_lib_GetCandidatePlateImageSize ( int plateIndex, int * width, int * height )
{

    if ( plateIndex >= MAX_ACTUAL_PLATES ) return;

    *width = libData.foundPlates[plateIndex ].width;
    *height = libData.foundPlates[plateIndex ].height;
}


//
// LPROCR_diags_getCandidatePlateImage, returns 0 if no valid candidate plate
//

int _stdcall LPROCR_diags_getCandidatePlateImage( int plateIndex, char * red, char * green, char * blue, int * error) 
{
    int x,y, offset;
    int w, h;

    DIAG_COLOR_IMAGE *image;

    *error = ERROR_NO_ERROR;

    if ( libData.diagnosticsInitialized != 1 )
    {
        return 0;
    }

    if ( libData.foundPlates[plateIndex].diagImage == 0 ) return 0;

    //   if ( libData.foundPlates[plateIndex].leftEdge == -1 ) return 0;

    image = (DIAG_COLOR_IMAGE *)libData.foundPlates[plateIndex].diagImage;
    w  = libData.foundPlates[plateIndex].width ;
    h  = libData.foundPlates[plateIndex].height ;


    // copy luminance array
    offset = 0;
    for ( x=0; x < w; x++)
    {
        for(y=0; y < h; y++)
        {
            *(red+offset+y) =(char) (image+offset+y)->red;
            *(green+offset+y) = (char)(image+offset+y)->green;
            *(blue+offset+y) = (char)(image+offset+y)->blue;
        }
        offset += y;
    }

    return(1);
}



//
// method LPROCR_diags_getIntegralImage
//

void _stdcall LPROCR_diags_getIntegralImage(  char * red, char * green, char * blue) 
{
    int x,xo, y;
    int w, h;
    double intermediate;
    double span;
    double min = 99999999;
    double max = 0;
    double *imageSrc;

    if ( libData.diagnosticsInitialized != 1 )
    {
        return;
    }



    imageSrc = (double *)libData.integralTableFullRes;

    w  = libData.imageWidth;
    h = libData.imageHeight;


    for (x = 0; x < w; x++)
    {
        xo = x * h;
        for (int y = 0; y < h; y++)
        {
            if (imageSrc[xo +y] > max) max = imageSrc[xo +y];
            if (imageSrc[xo +y] < min) min = imageSrc[xo +y];
        }
    }


    span = max - min;
    if ( span == 0 ) span = 1;

    for ( x=0; x < w; x++)
    {
        xo = x * h;
        for(y=0; y < h; y++)
        {
            intermediate = (double) imageSrc[xo +y] - min;
            intermediate = ((double)intermediate *  765.0) / span;

            if (intermediate > 765.0) intermediate = 765.0;

            // colorize

            if (intermediate < 255.0)
            {
                *(green+xo+y)= (unsigned char)intermediate;
                *(red+xo+y)  = 0;
                *(blue+xo+y) = 0;
            }
            else if (intermediate < 510.0)
            {
                *(green+xo+y) = (unsigned char)255;
                *(red+xo+y)  = 0;
                *(blue+xo+y) = (unsigned char)(intermediate-255.0);
            }
            else 
            {
                *(green+xo+y) =(unsigned char) 255;
                *(red+xo+y)  = (unsigned char)(intermediate-510.0);
                *(blue+xo+y) = (unsigned char)255;
            }
        }

    }


}








//
// method LPROCR_diags_getPlateImage
//

void _stdcall LPROCR_diags_getPlateImage( int plateIndex, char * red, char * green, char * blue, int * error, int useCandidatePlateList) 
{
    int x,y, offset;
    int w, h;

    DIAG_COLOR_IMAGE *image;

    *error = ERROR_NO_ERROR;

    if ( libData.diagnosticsInitialized != 1 )
    {
        return;
    }

    if( useCandidatePlateList == 1)
    {
       image = (DIAG_COLOR_IMAGE *)libData.foundPlates[plateIndex].diagImage;
       w  = libData.foundPlates[plateIndex].width ;
       h = libData.foundPlates[plateIndex].height ;
    }
    else
    {
       image = (DIAG_COLOR_IMAGE *)libData.finalPlateList[plateIndex].diagImage;
       w  = libData.finalPlateList[plateIndex].width ;
       h = libData.finalPlateList[plateIndex].height ;
    }

    if ( image  == 0 ) return;

   

    // copy luminance array
    offset = 0;
    for ( x=0; x < w; x++)
    {
        for(y=0; y < h; y++)
        {
            *(red+offset+y) =(char) (image+offset+y)->red;
            *(green+offset+y) = (char)(image+offset+y)->green;
            *(blue+offset+y) = (char)(image+offset+y)->blue;
        }
        offset += y;
    }

}






//
// method LPROCR_diags_init
//
void  LPROCR_diags_init ( int width, int height, int * error)
{
    DIAG_COLOR_IMAGE * dImage;
    int w;
    int h;
    int offset;
    int x, y;


    ClearLogs( );

    freeMemory ( (int**)& libData.imageFullResDiag); // free the previous usage from  the previous frame

    libData.imageFullResDiag =  (int *) malloc (sizeof(DIAG_COLOR_IMAGE) * width * height );
    if ( libData.imageFullResDiag == 0 )
    {
        *error = ERROR_memoryNotAllocated  ;
        return;
    }

    freeMemory ( (int**)&  libData.edgeMapFullResDiag);
    libData.edgeMapFullResDiag =  (int *) malloc (sizeof(DIAG_COLOR_IMAGE) * width * height );
    if ( libData.edgeMapFullResDiag == 0 )
    {
        *error = ERROR_memoryNotAllocated  ;
        return;
    }


    freeMemory (  & libData.imageSubDiag );

    libData.imageSubDiag = (int *) malloc ( sizeof(DIAG_COLOR_IMAGE) * libData.imageSubWidth * libData.imageSubHeight); 


    if ( libData.imageSubDiag == 0 )
    {
        *error = ERROR_memoryNotAllocated ;
        return;
    }

    // init  diag images with copies of the input images

    dImage =(DIAG_COLOR_IMAGE *) libData.imageFullResDiag;
    w  = width;
    h =  height;



    offset = 0;
    for ( x=0; x < w; x++)
    {
        for(y=0; y < h; y++)
        {
            (dImage+offset+y)->red   = *(libData.imageFullRes+offset+y);
            (dImage+offset+y)->green = *(libData.imageFullRes+offset+y);
            (dImage+offset+y)->blue  = *(libData.imageFullRes+offset+y);
        }
        offset += y;
    }




    dImage = (DIAG_COLOR_IMAGE *)libData.imageSubDiag;
    w  = libData.imageSubWidth;
    h  = libData.imageSubHeight;

    offset = 0;
    for ( x=0; x < w; x++)
    {
        for(y=0; y < h; y++)
        {
            (dImage+offset+y)->red   = *(libData.imageSub+offset+y);
            (dImage+offset+y)->green = *(libData.imageSub+offset+y);
            (dImage+offset+y)->blue  = *(libData.imageSub+offset+y);
        }
        offset += y;
    }


    int p =0;
    int c= 0;
    int xo=0;
    for (p=0; p < MAX_CANDIDATE_PLATES ; p++)
    {
        for ( c = 0; c < MAX_CANDIDATE_CHARS; c++)
        {
            if ( libData.foundPlates[ p].diagCharStdSize [c] == 0 )
                libData.foundPlates[ p].diagCharStdSize [c] = (int *) malloc(sizeof(int)* STANDARD_WIDTH * STANDARD_HEIGHT);

            for ( x = 0; x < STANDARD_WIDTH; x++)
            {
                xo = x * STANDARD_HEIGHT;
                for(y = 0; y < STANDARD_HEIGHT; y++)
                {
                    *(libData.foundPlates[ p].diagCharStdSize [c] + xo + y )  = 0;
                }
            }
        }   
    }
    for (p=0; p < MAX_ACTUAL_PLATES; p++)
    {
        for ( c = 0; c < MAX_CANDIDATE_CHARS; c++)
        {
            if ( libData.finalPlateList[ p].diagCharStdSize [c] == 0 )
                libData.finalPlateList[ p].diagCharStdSize [c] = (int *) malloc(sizeof(int)* STANDARD_WIDTH * STANDARD_HEIGHT);

            for ( x = 0; x < STANDARD_WIDTH; x++)
            {
                xo = x * STANDARD_HEIGHT;
                for(y = 0; y < STANDARD_HEIGHT; y++)
                {
                    *(libData.foundPlates[ p].diagCharStdSize [c] + xo + y )  = 0;
                }
            }
        }   
    }

    libData.diagnosticsInitialized = 1;

}


//
// method LPROCR_plate_diags_init, memory has been pre-allocated by the caller
//
void  LPROCR_plate_diags_init ( int width, int height, int * diagImage,  int * error)
{
    DIAG_COLOR_IMAGE * dImage;
    int w;
    int h;
    int offset;
    int x, y;



    if ( diagImage == 0 ) return;


    *error = ERROR_NO_ERROR;


    // init images



    dImage = (DIAG_COLOR_IMAGE *)diagImage;
    w  = width;
    h =  height;



    offset = 0;
    for ( x=0; x < w; x++)
    {
        for(y=0; y < h; y++)
        {
            (dImage+offset+y)->red   = *(libData.plateImage+offset+y);
            (dImage+offset+y)->green = *(libData.plateImage+offset+y);
            (dImage+offset+y)->blue  = *(libData.plateImage+offset+y);
        }
        offset += y;
    }


    libData.plateImageDiag = diagImage; // hold a reference to this image globally while this plate is being processed
}


//
// method LPROCR_plate_diags_putImage - used to copy a particular image into the diag buffer. 
//
void  LPROCR_plate_diags_putImage( int *plateImageSrc, int * diagImage, int width, int height)
{
    DIAG_COLOR_IMAGE * dImage;
    int w;
    int h;
    int offset;
    int x, y;


    if (libData.diagEnabled != 1 ) // init main LPROCR first
    {
        return;
    }



    // init images

    dImage = (DIAG_COLOR_IMAGE * ) diagImage;
    w  = width;
    h =  height;



    offset = 0;
    for ( x=0; x < w; x++)
    {
        for(y=0; y < h; y++)
        {
            (dImage+offset+y)->red   = *(plateImageSrc+offset+y);
            (dImage+offset+y)->green = *(plateImageSrc+offset+y);
            (dImage+offset+y)->blue  = *(plateImageSrc+offset+y);
        }
        offset += y;
    }

}


//
// method LPROCR_diags_putEdgeMapFullResImage - copies the newly created edge map into a diagnostic buffer, color plots can be overlaid onto this buffer.
//
void  LPROCR_diags_putEdgeMapFullResImage( int xStart, int yStart, int xEnd, int yEnd)                    
{
    DIAG_COLOR_IMAGE * dImage;
    int w;
    int h;
    int  xo;
    int *imageSrc;
    double intermediate;
    double span;

    int min = 999999999;
    int max = -999999999;

    if (libData.diagEnabled != 1 ) // init main LPROCR first
    {
        return;
    }

    if (libData.edgeMapFullResDiag ==0 ) 
    {
        return;
    }


    // init images

    imageSrc = libData.edgeMapFullRes;

    dImage = (DIAG_COLOR_IMAGE * ) libData.edgeMapFullResDiag;
    w  = libData.imageWidth;
    h =  libData.imageHeight;



    for (int x = xStart; x < xEnd; x++)
    {
        xo = x * h;
        for (int y = yStart; y < yEnd; y++)
        {
            if (imageSrc[xo +y] > max) max = imageSrc[xo +y];
            if (imageSrc[xo +y] < min) min = imageSrc[xo +y];
        }
    }


    span = max - min;

    for (int x = xStart; x < xEnd; x++)
    {
        xo = x * h;
        for (int y = yStart; y < yEnd; y++)
        {
            intermediate = (double) imageSrc[xo +y] - min;
            intermediate = ((double)intermediate *  765.0) / span;

            if (intermediate < 256.0)
            {
                (dImage+xo+y)->green= (char)intermediate;
                (dImage+xo+y)->red = 0;
                (dImage+xo+y)->blue = 0;
            }
            else if (intermediate < 512.0)
            {
                (dImage+xo+y)->green = 255;
                (dImage+xo+y)->red  = 0;
                (dImage+xo+y)->blue = (char)(intermediate-255);
            }
            else 
            {
                (dImage+xo+y)->green = 255;
                (dImage+xo+y)->red  = (char)(intermediate-510);
                (dImage+xo+y)->blue = 255;
            }
        }
    }


}


//
// method drawBox - used to draw a box in either the full scale image or subscalled image, draws onto the diag image buffer
//
//   fullSub = 0 use subscalled diag image
//   fullSbu = 1 use full resolution diag image

/*   example call:
color.blue = 255;
color.green = 0;
color.red = 0;
drawBox ( 0, & pLocationList[cp], &color);*/
//
void  drawBox ( int fullSub, LPROCR_lib_PLATE_LOCATION * pLocation, DIAGCOLOR *c )
{
    int x ;
    int y ;
    int w,h;
    int xo ;
    int xStart;
    int xEnd;
    int yStart;
    int yEnd;

    DIAG_COLOR_IMAGE * image;

    if ( fullSub == 1 )
    {
        image = (DIAG_COLOR_IMAGE *)libData.imageFullResDiag;
        w = libData.imageWidth;
        h = libData.imageHeight;
    }
    else
    {
        image =(DIAG_COLOR_IMAGE *) libData.imageSubDiag;
        w = libData.imageSubWidth;
        h = libData.imageSubHeight;
    }


    // follow the top edge, starting at left edge, moving to right edge, putting a white pixel
    xStart = pLocation->leftEdge;
    xEnd = pLocation->rightEdge;
    yStart = pLocation->topEdge;
    yEnd = pLocation->topEdge;

    xo = xStart * h;
    y = yStart;
    LIMIT(xEnd,w);
    for (x = xStart; x < xEnd; x++)
    {

        (image + xo + y )->red = c->red;
        (image + xo + y )->blue = c->blue;
        (image + xo + y )->green = c->green;
        xo += h;

    }

    // follow the bottom edge, starting at left edge, moving to right edge, putting a white pixel
    xStart = pLocation->leftEdge;
    xEnd = pLocation->rightEdge;
    yStart = pLocation->bottomEdge;
    yEnd = pLocation->bottomEdge;

    xo = xStart * h;
    y = yStart;
    LIMIT(xEnd,w);
    for (x = xStart; x < xEnd; x++)
    {
        (image + xo + y )->red = c->red;
        (image + xo + y )->blue = c->blue;
        (image + xo + y )->green = c->green;
        xo += h;
    }


    // follow the left edge, starting at top edge, moving to bottom edge, putting a white pixel
    xStart = pLocation->leftEdge;
    xEnd = pLocation->leftEdge;
    yStart = pLocation->topEdge;
    yEnd = pLocation->bottomEdge;

    xo = xStart * h;
    for(y = yStart; y < yEnd; y++)
    {
        LIMIT(y,h);
        (image + xo + y )->red = c->red;
        (image + xo + y )->blue = c->blue;
        (image + xo + y )->green = c->green;
    }

    // follow the right edge, starting at top edge, moving to bottom edge, putting a white pixel
    xStart = pLocation->rightEdge;
    xEnd = pLocation->rightEdge;
    yStart = pLocation->topEdge;
    yEnd = pLocation->bottomEdge;

    xo = xStart * h;

    for(y = yStart; y < yEnd; y++)
    {
        LIMIT(y,h);
        (image + xo + y )->red = c->red;
        (image + xo + y )->blue = c->blue;
        (image + xo + y )->green = c->green;
    }



}



//
// method LPROCR_plate_diags_drawBox
//
void  LPROCR_plate_diags_drawBox (  LPROCR_lib_CHAR_LOCATION * cl, DIAGCOLOR *c )
{
    int x ;
    int y ;
    int xo ;
    int xStart;
    int xEnd;
    int yStart;
    int yEnd;
    int w,h;

    DIAG_COLOR_IMAGE * image;

    if (libData.diagnosticsInitialized != 1)
    {
        return;
    }


    image = (DIAG_COLOR_IMAGE * )libData.plateImageDiag; // get the global referecne to the current diag plate image
    h = libData.plateImageHeight;
    w = libData.plateImageWidth;



    // follow the top edge, starting at left edge, moving to right edge, putting a white pixel
    xStart = cl->leftEdge;
    xEnd = cl->rightEdge;
    yStart = cl->topEdge;
    yEnd = cl->topEdge;

    xo = xStart * h;
    y = yStart;
    for (x = xStart; x < xEnd; x++)
    {
        (image + xo + y )->red = c->red;
        (image + xo + y )->blue = c->blue;
        (image + xo + y )->green = c->green;
        xo += h;
    }

    // follow the bottom edge, starting at left edge, moving to right edge, putting a white pixel
    xStart = cl->leftEdge;
    LIMIT(xStart,w);
    xEnd = cl->rightEdge;
    LIMIT(xEnd,w);
    yStart = cl->bottomEdge;
    LIMIT(yStart,h);
    yEnd = cl->bottomEdge;
    LIMIT(yEnd,h);

    xo = xStart * h;
    y = yStart;
    for (x = xStart; x < xEnd; x++)
    {
        (image + xo + y )->red = c->red;
        (image + xo + y )->blue = c->blue;
        (image + xo + y )->green = c->green;
        xo += h;
    }


    // follow the left edge, starting at top edge, moving to bottom edge, putting a white pixel
    xStart = cl->leftEdge;
    xEnd = cl->leftEdge;
    yStart = cl->topEdge;
    yEnd = cl->bottomEdge;
    LIMIT(xStart,w);
    LIMIT(xEnd,w);
    LIMIT(yStart,h);
    LIMIT(yEnd,h);

    xo = xStart * h;
    for(y = yStart; y < yEnd; y++)
    {
        (image + xo + y )->red = c->red;
        (image + xo + y )->blue = c->blue;
        (image + xo + y )->green = c->green;
    }

    // follow the right edge, starting at top edge, moving to bottom edge, putting a white pixel
    xStart = cl->rightEdge;
    xEnd = cl->rightEdge;
    yStart = cl->topEdge;
    yEnd = cl->bottomEdge;
    LIMIT(xStart,w);
    LIMIT(xEnd,w);
    LIMIT(yStart,h);
    LIMIT(yEnd,h);

    xo = xStart * h;

    for(y = yStart; y < yEnd; y++)
    {
        (image + xo + y )->red = c->red;
        (image + xo + y )->blue = c->blue;
        (image + xo + y )->green = c->green;
    }

}




// plots a line in the current plate diag image buffer

void plate_diags_PlotLine (  int xStart, int yStart, int xEnd, int yEnd  )
{
    int x ;
    int y ;
    int h; 
    int w;

    int xo;

    float m; // slope
    int b; // y intercept

    DIAG_COLOR_IMAGE * image;

    if (libData.diagnosticsInitialized == 0)
    {
        return;
    }



    image = (DIAG_COLOR_IMAGE * )libData.plateImageDiag; // get the global referecne to the current diag plate image
    h = libData.plateImageHeight;
    w = libData.plateImageWidth;


    if ( xEnd >= w ) xEnd = w -1;
    if ( xEnd < 0 ) xEnd = 0;

    if ( xStart >= w ) xStart = w -1;
    if ( xStart < 0 ) xStart = 0;

    if ( yEnd >= h ) yEnd = h -1;
    if ( yEnd < 0 ) yEnd = 0;

    if ( yStart >= h ) yStart = h -1;
    if ( yStart < 0 ) yStart = 0;

    // is this  a vertical line ?
    if ( (xEnd - xStart) == 0 )
    {
        xo = xEnd * h;
        for (y = yStart; y < yEnd; y++)
        {
            (image + xo + y )->red = 0;
            (image + xo + y )->blue = 255;
            (image + xo + y )->green = 0;
        }

        return;
    }

    // else its not a vertical line

    m = (float)( yEnd - yStart) / (float)( xEnd - xStart);

    if ( xEnd > xStart )
        b = (int)((float)yStart - ( (float)xStart * m   ));
    else
        b = (int)((float)yEnd - ( (float)xEnd * m   ));



    xo = xStart * h;

    if ( xEnd > xStart)
    {
        for (x = xStart; x < xEnd; x++)
        {
            xo = x * h;
            y = (int) (m * (float)x) + b;
            (image + xo + y )->red = 0;
            (image + xo + y )->blue = 255;
            (image + xo + y )->green = 0;

        }
    }
    else
    {
        for (x = xEnd; x < xStart; x++)
        {
            xo = x * h;
            y = (int) (m * (float)x) + b;
            (image + xo + y )->red = 0;
            (image + xo + y )->blue = 255;
            (image + xo + y )->green = 0;

        }
    }

}




//
// method plotLine
//
//    plots a line in either the image full resolution or subscalled image diag buffer
//
//    fullSub = 1 means full resolution image
//    fullSub = 0 means subscalled image
//
void  plotLine (  int fullSub, int xStart, int xEnd, int yStart, int yEnd, DIAGCOLOR *c )
{
    int x ;
    int y ;
    int w,h;
    int xo;

    int m; // slope
    int b; // y intercept

    DIAG_COLOR_IMAGE * image;

    if ( libData.diagnosticsInitialized == 0)
    {
        return;
    }



    if ( fullSub == 1 )
    {
       // image = (DIAG_COLOR_IMAGE *)libData.imageFullResDiag;
        image =  (DIAG_COLOR_IMAGE *)libData.edgeMapFullResDiag;
        w = libData.imageWidth;
        h = libData.imageHeight;
    }
    else
    {
        image =(DIAG_COLOR_IMAGE *) libData.imageSubDiag;
        w = libData.imageSubWidth;
        h = libData.imageSubHeight;
    }

    // draw a vertical line
    if ( (xEnd - xStart) == 0 )
    {
        xo = xEnd * h;
        for (y = yStart; y < yEnd; y++)
        {
            (image + xo + y )->red = c->red;
            (image + xo + y )->blue = c->blue;
            (image + xo + y )->green = c->green;
        }

        return;
    }

    // else its not a vertical line

    m = ( yEnd - yStart) / ( xEnd - xStart);

    b = yStart - ( xStart * m   );

    xo = xStart * h;

    for (x = xStart; x < xEnd; x++)
    {
        y = (m * x) + b;
        (image + xo + y )->red = c->red;
        (image + xo + y )->blue = c->blue;
        (image + xo + y )->green = c->green;
        xo += h;
    }


}


//  method plate_diags_PlotSumCurve - draws into the current plate diag image buffer


void plate_diags_PlotSumCurve (  int * rowSum  )
{
    int x ;
    int y ;
    int w,h;
    int xo;
    int peakVal;



    DIAG_COLOR_IMAGE * image;

    if (libData.diagnosticsInitialized == 0)
    {
        return;
    }



    image = (DIAG_COLOR_IMAGE *)libData.plateImageDiag;
    w = libData.plateImageWidth;
    h = libData.plateImageHeight;

    // get the peak value (assume min value is zero, no negatives)
    peakVal = 0;
    for (y=0; y < h; y++)
    {
        if ( rowSum[y] > peakVal)
        {
            peakVal = rowSum[y];
        }
    }

    if (peakVal  == 0 ) peakVal = 1;


    for (y=0; y < h; y++)
    {
        x = rowSum[y] * w / peakVal;

        if ( x < 10 ) x  = 10;
        LIMIT(x,w);
        xo = x * h;
        (image + xo + y )->red = 255;
        (image + xo + y )->blue = 0;
        (image + xo + y )->green = 0;


    }

}

void LPROCR_diags_plotHorizontalLineSums (int * lineSumCurve, int yBase, int count, int xBase, int xMax)
{

    int x ;
    int y ;
    int w,h;
    int xo;
    int peakVal;
    int range;
    int lineIndex = 0;


    DIAG_COLOR_IMAGE * image;

    if (libData.diagnosticsInitialized == 0)
    {
        return;
    }

    range = xMax - xBase;

    image = (DIAG_COLOR_IMAGE *)libData.edgeMapFullResDiag;
    w = libData.imageWidth;
    h = libData.imageHeight;

  // get the peak value (assume min value is zero, no negatives)
    peakVal = 0;
    for (y=0; y < count; y++)
    {
        if ( lineSumCurve[y] > peakVal)
        {
            peakVal = lineSumCurve[y];
        }
    }

    if (peakVal  == 0 ) peakVal = 1;

    lineIndex = 0;

    for (y=yBase; y < yBase + count; y++)
    {
        x = ((lineSumCurve[lineIndex] * range) / peakVal) + xBase;

        lineIndex ++;

        if ( x < 10 ) x  = 10;
        LIMIT(x,w);
        xo = x * h;
        (image + xo + y )->red = 255;
        (image + xo + y )->blue = 0;
        (image + xo + y )->green = 0;
    }

}

void RejectLogAdd(char * string)
{
    //define MAX_LOG_STRING_LENGTH 256
    //#define MAX_LOGS_PER_IMAGE 32

    if ( libData.diagnosticsInitialized == 0 ) return;

    int i = 0;
    for (i = 0; i < MAX_LOG_STRING_LENGTH; i++)
    {
        libData.logs[libData.currentLogIndex].logString[i] = 0;
    }

    for (i = 0; i < MAX_LOG_STRING_LENGTH-1; i++) 
    {
        libData.logs[libData.currentLogIndex].logString[i] = string[i];
        if ( string[i] == 0 ) break;
    }

    // just in case there was no null term in the input string
    libData.logs[libData.currentLogIndex].logString[MAX_LOG_STRING_LENGTH-1] = 0;

    libData.currentLogIndex++;
    if ( libData.currentLogIndex == MAX_LOGS_PER_IMAGE) libData.currentLogIndex = 0;

}

void RejectLogAddPlateIndexLocation(int plateIndex, int x, int y, char * string)
{
    int i=0, j= 0;
    //define MAX_LOG_STRING_LENGTH 256
    //#define MAX_LOGS_PER_IMAGE 32

    char plateIndexStr[32];
    char locationString[32];
    

    if ( libData.diagnosticsInitialized == 0 ) return;

    for (i=0; i < 32;i++ ) plateIndexStr[i] = 0;
    for (i=0; i < 32;i++ ) locationString[i] = 0;

    sprintf(plateIndexStr,"pi = %2d",plateIndex);
    sprintf(locationString, "x= %4d; y= %4d",x,y);

    i = 0;
    for (i = 0; i < MAX_LOG_STRING_LENGTH; i++)
    {
        libData.logs[libData.currentLogIndex].logString[i] = 0;
    }

    j = 0;
    i = 0;
    while ( plateIndexStr[j] != 0 && i < MAX_LOG_STRING_LENGTH-2)
    {
        libData.logs[libData.currentLogIndex].logString[i++]  =  plateIndexStr[j++] ;
    }
    libData.logs[libData.currentLogIndex].logString[i++] =' ';

    j = 0;
    while ( locationString[j] != 0 && i < MAX_LOG_STRING_LENGTH-2)
    {
        libData.logs[libData.currentLogIndex].logString[i++]  =  locationString[j++] ;
    }
    libData.logs[libData.currentLogIndex].logString[i++] =' ';

    j = 0;
    while ( string[j] != 0 && i < MAX_LOG_STRING_LENGTH-2)
    {
        libData.logs[libData.currentLogIndex].logString[i++]  =  string[j++] ;
    }
    libData.logs[libData.currentLogIndex].logString[i++] = 0;



    libData.currentLogIndex++;
    if ( libData.currentLogIndex == MAX_LOGS_PER_IMAGE) libData.currentLogIndex = 0;

}

int _stdcall LPROCR_diags_GetRejectLogLength ( )
{
    return ( MAX_LOGS_PER_IMAGE * MAX_LOG_STRING_LENGTH);
}

// mamaged code allocates memory for the string

void _stdcall LPROCR_diags_GetRejectLog ( char * inStrMemory )
{
    int c = 0;
    int l =0, i = 0;
    int lineLen = 0;

    int logLen = MAX_LOGS_PER_IMAGE * MAX_LOG_STRING_LENGTH;

    for (c=0; c <  logLen; c++)
    {
        inStrMemory[c] = 0;
    }



    c = 0;


    for ( l = 0; l < MAX_LOGS_PER_IMAGE; l++)
    {
        lineLen =0;

        for (i = 0 ; i <MAX_LOG_STRING_LENGTH; i++ )
        {                  
            if ( c >= logLen-2) break;

            if ( libData.logs[l].logString[i ] != 0 ) 
            {
                inStrMemory[c++] = libData.logs[l].logString[i ] ;
                lineLen ++;
            }
            else
            {
                inStrMemory[c++] = ',';

                break; // move to the next log
            }  
        }

        if ( lineLen == 0 ) c--; // remove the last comma, its an empty line

        if ( c >= logLen-2) break;

    }

    inStrMemory[c] = 0;
}

void ClearLogs( )
{
    int l,i;

    for ( l = 0; l < MAX_LOGS_PER_IMAGE; l++)
    {
        for (i = 0 ; i < MAX_LOG_STRING_LENGTH; i++ )
        {                  
            libData.logs[l].logString[ i ] = 0;
        }
    }

}



void LPROCR_diags_GetPlateDiagImageRGBArray(int plateIndex, int width, int height, unsigned char * rgbValues,  unsigned char * red, unsigned  char * green, unsigned  char * blue, int pixelOffset)
{

    int x,xo,y;
    int w,h;
    int span;
   
    int bi;
   
    int testByte;

    w  = width;
    h = height;

    bi =0;
    for (y = 0; y < h; y++)
    {
        for (x = 0; x < w;x++)
        {
            xo = x * h; 
  
            testByte =(int) blue[xo+y];

            rgbValues[bi] = (byte)blue[xo+y];
            rgbValues[bi + 1] = (byte)green[xo+y];
            rgbValues[bi + 2] = (byte)red[xo+y];
            if ( pixelOffset == 4 ) rgbValues[bi + 3] = (byte) 255; // alpha
            bi += pixelOffset;
        }
         while (bi % 4 != 0) 
            bi++;

    }



}







void LPROCR_diags_GetCharImageRGBArray(int plateIndex, int cIndex, unsigned char * rgbValues, int pixelOffset)
{

    int x,xo,y;
    int w,h;
    int span;
    int *imageSrc;
    int bi;
    unsigned char red, green, blue;
    int min = 999999999;
    int max = -999999999;
    double  intermediate;

    imageSrc = libData.foundPlates[plateIndex].diagCharStdSize[cIndex];

    if ( imageSrc == 0 ) return;

    w  = STANDARD_WIDTH;
    h = STANDARD_HEIGHT;


    for (x = 0; x < w; x++)
    {
        xo = x * h;
        for (int y = 0; y < h; y++)
        {
            if (imageSrc[xo +y] > max) max = imageSrc[xo +y];
            if (imageSrc[xo +y] < min) min = imageSrc[xo +y];
        }
    }


    span = max - min;
    if ( span == 0 ) span = 1;

    bi =0;
    for (y = 0; y < h; y++)
    {
        for (x = 0; x < w;x++)
        {
            xo = x * h;

            intermediate = (double) imageSrc[xo +y] - min;
            intermediate = ((double)intermediate *  765.0) / span;
          
            if (intermediate > 765.0) intermediate = 765.0;

            // colorize

            red = 0;
            green = 0;
            blue = 0;
   

            if (intermediate < 255.0)
            {
                green = (unsigned char)intermediate;
                red  = 0;
                blue = 0;
            }
            else if (intermediate < 510.0)
            {
                green = 255;
                red  = 0;
                blue = (unsigned char)(intermediate-255.0);
            }
            else 
            {
                green = 255;
                red  = (unsigned char)(intermediate-510.0);
                blue = 255;
            }

           
            rgbValues[bi] = (byte)blue;
            rgbValues[bi + 1] = (byte)green;
            rgbValues[bi + 2] = (byte)red;
            if ( pixelOffset == 4 ) rgbValues[bi + 3] = (byte) 255; // alpha
            bi += pixelOffset;
        }
        while (bi % 4 != 0) 
           bi++;
    }



}













void LPROCR_diags_PutCharImageToRGBArray(char * imageSrc, int width, int height, unsigned char * rgbValues, int pixelOffset)
{

    int x,xo,y;
    int w,h;
    int span;
    int bi;
    unsigned char red, green, blue;
    int min = 999999999;
    int max = -999999999;
    double  intermediate;


    if ( imageSrc == 0 ) return;

    w  = width;
    h = height;


    for (x = 0; x < w; x++)
    {
        xo = x * h;
        for (int y = 0; y < h; y++)
        {
            if (imageSrc[xo +y] > max) max = imageSrc[xo +y];
            if (imageSrc[xo +y] < min) min = imageSrc[xo +y];
        }
    }


    span = max - min;
    if ( span == 0 ) span = 1;

    bi =0;
    for (y = 0; y < h; y++)
    {
        for (x = 0; x < w;x++)
        {
            xo = x * h;

            intermediate = (double) imageSrc[xo +y] - min;
            intermediate = ((double)intermediate *  765.0) / span;
          
            if (intermediate > 765.0) intermediate = 765.0;

            // colorize

            red = 0;
            green = 0;
            blue = 0;
   

            if (intermediate < 255.0)
            {
                green = (unsigned char)intermediate;
                red  = 0;
                blue = 0;
            }
            else if (intermediate < 510.0)
            {
                green = 255;
                red  = 0;
                blue = (unsigned char)(intermediate-255.0);
            }
            else 
            {
                green = 255;
                red  = (unsigned char)(intermediate-510.0);
                blue = 255;
            }

           
            rgbValues[bi] = (byte)blue;
            rgbValues[bi + 1] = (byte)green;
            rgbValues[bi + 2] = (byte)red;
            if ( pixelOffset == 4 ) rgbValues[bi + 3] = (byte) 255; // alpha
            bi += pixelOffset;
        }
        while (bi % 4 != 0) 
           bi++;
    }



}








