
#include <afx.h>
#include <stdlib.h>
#include <malloc.h>
#include "LPROCR_Public.h"
#include "LPROCR_Structs.h"
#include "LPROCR_Error_Codes.h"
#include "LPROCR_Diags.h"
#include "charIsolationMethods.h"
#include "ocr.h"


extern CHAR_DATA  m_CharLibData[];

float scores[NUM_CHARS] ;

int * m_diagnosticCharPtr;
float m_MaxScore =0 ;

int secondPassCheck ( int * in,int w, int  h, int bestIndex, int recursion, int * diagChar , float * score)
{
   
    int prevIndex;
    bool converted = false;
    
    m_diagnosticCharPtr = diagChar;
   
    prevIndex = bestIndex;

    recursion++;
    if ( recursion > 3 ) return (bestIndex);



    if ( m_CharLibData[bestIndex].name[0] == 'N' || m_CharLibData[bestIndex].name[0] == 'H' ) 
    {
        bestIndex = groupHN(in,  w,  h);
        if ( prevIndex != bestIndex ) converted = true;
    }
    if ( m_CharLibData[bestIndex].name[0] == 'Q' || m_CharLibData[bestIndex].name[0] == '0' 
        || m_CharLibData[bestIndex].name[0] == 'D') 
    {
        bestIndex = groupQ0D(in,  w,  h);
        if ( prevIndex != bestIndex ) converted = true;
    }
    if (m_CharLibData[bestIndex].name[0] == 'U'  || m_CharLibData[bestIndex].name[0] == '0' || m_CharLibData[bestIndex].name[0] == 'D') 
    {
        bestIndex = groupD0U(in,  w,  h);
        if ( prevIndex != bestIndex ) converted = true;
    }

    if ( (m_CharLibData[bestIndex].name[0] == 'D' || m_CharLibData[bestIndex].name[0] == '0') && recursion > 1  ) 
    {
        bestIndex = groupD0(in,  w,  h);
        if ( prevIndex != bestIndex ) converted = true;
        recursion = 99999;// we are done
    }
    if ( m_CharLibData[bestIndex].name[0] == 'S' || m_CharLibData[bestIndex].name[0] == 'B'  || m_CharLibData[bestIndex].name[0] == '8') 
    {
        bestIndex = groupSB(in,  w,  h);
        if ( prevIndex != bestIndex ) converted = true;
    }
    if ( m_CharLibData[bestIndex].name[0] == 'B' || m_CharLibData[bestIndex].name[0] == 'H' ) 
    {
        bestIndex = groupBH(in,  w,  h);
        if ( prevIndex != bestIndex ) converted = true;
    }
    if ( (m_CharLibData[bestIndex].name[0] == 'B' || m_CharLibData[bestIndex].name[0] == '8') && recursion > 1 ) 
    {
        bestIndex = groupB8(in,  w,  h);
        if ( prevIndex != bestIndex ) converted = true;
        recursion = 99999;// we are done
    }
    if ( m_CharLibData[bestIndex].name[0] == '7' || m_CharLibData[bestIndex].name[0] == 'T' ) 
    {
        bestIndex = group7T(in,  w,  h);
        if ( prevIndex != bestIndex ) converted = true;
    }
    if ( m_CharLibData[bestIndex].name[0] == 'G' || m_CharLibData[bestIndex].name[0] == 'C' ) 
    {
        bestIndex = groupGC(in,  w,  h);
        if ( prevIndex != bestIndex ) converted = true;
    }
    if ( m_CharLibData[bestIndex].name[0] == 'G' || m_CharLibData[bestIndex].name[0] == '0' ) 
    {
        bestIndex = groupG0(in,  w,  h);
        if ( prevIndex != bestIndex ) converted = true;
    }
    if ( m_CharLibData[bestIndex].name[0] == 'J' || m_CharLibData[bestIndex].name[0] == '3' ) 
    {
        bestIndex = groupJ3(in,  w,  h);
        if ( prevIndex != bestIndex ) converted = true;
    }
    if ( m_CharLibData[bestIndex].name[0] == 'C' || m_CharLibData[bestIndex].name[0] == '0') 
    {
        bestIndex = groupC0(in,  w,  h);
        if ( prevIndex != bestIndex ) converted = true;
    }
    if ( m_CharLibData[bestIndex].name[0] == 'P' || m_CharLibData[bestIndex].name[0] == 'F') 
    {
        bestIndex = groupPF(in,  w,  h);
        if ( prevIndex != bestIndex ) converted = true;
    }
    if ( m_CharLibData[bestIndex].name[0] == 'P' || m_CharLibData[bestIndex].name[0] == '2') 
    {
        bestIndex = groupP2(in,  w,  h);
        if ( prevIndex != bestIndex ) converted = true;
    }
    if ( m_CharLibData[bestIndex].name[0] == '5' || m_CharLibData[bestIndex].name[0] == '6') 
    {
        bestIndex = group56(in,  w,  h);
        if ( prevIndex != bestIndex ) converted = true;
    }
    if ( m_CharLibData[bestIndex].name[0] == '2' || m_CharLibData[bestIndex].name[0] == 'E') 
    {
        bestIndex = group2E(in,  w,  h);
        if ( prevIndex != bestIndex ) converted = true;
    }
    if ( m_CharLibData[bestIndex].name[0] == 'P' || m_CharLibData[bestIndex].name[0] == 'H') 
    {
        bestIndex = groupPH(in,  w,  h);
        if ( prevIndex != bestIndex ) converted = true;
    }
    if ( m_CharLibData[bestIndex].name[0] == 'B' || m_CharLibData[bestIndex].name[0] == '6') 
    {
        bestIndex = groupB6(in,  w,  h);
        if ( prevIndex != bestIndex ) converted = true;
    }
    if ( m_CharLibData[bestIndex].name[0] == 'U' || m_CharLibData[bestIndex].name[0] == 'L') 
    {
        bestIndex = groupUL(in,  w,  h);
        if ( prevIndex != bestIndex ) converted = true;
    }
   /*this does not work if ( m_CharLibData[bestIndex].name[0] == '3' || m_CharLibData[bestIndex].name[0] == '9'|| m_CharLibData[bestIndex].name[0] == '8'|| m_CharLibData[bestIndex].name[0] == 'B') 
    {
        bestIndex = group389B(in,  w,  h);
        if ( prevIndex != bestIndex ) converted = true;
    }*/
    if ( m_CharLibData[bestIndex].name[0] == '3' || m_CharLibData[bestIndex].name[0] == 'B'|| m_CharLibData[bestIndex].name[0] == '8') 
    {
        bestIndex = group38B(in,  w,  h);
        if ( prevIndex != bestIndex ) converted = true;
    }
    if ( m_CharLibData[bestIndex].name[0] == '2' || m_CharLibData[bestIndex].name[0] == '8') 
    {
        bestIndex = group28(in,  w,  h);
        if ( prevIndex != bestIndex ) converted = true;
    }
    if ( m_CharLibData[bestIndex].name[0] == '6' || m_CharLibData[bestIndex].name[0] == '8') 
    {
        bestIndex = group68(in,  w,  h);
        if ( prevIndex != bestIndex ) converted = true;
    }
    if ( m_CharLibData[bestIndex].name[0] == '7' || m_CharLibData[bestIndex].name[0] == 'Z') 
    {
        bestIndex = group7Z(in,  w,  h);
        if ( prevIndex != bestIndex ) converted = true;
    }
    if ( m_CharLibData[bestIndex].name[0] == '5' || m_CharLibData[bestIndex].name[0] == 'S') 
    {
        bestIndex = group5S(in,  w,  h);
        if ( prevIndex != bestIndex ) converted = true;
    }
    if ( m_CharLibData[bestIndex].name[0] == '1' || m_CharLibData[bestIndex].name[0] == '4') 
    {
        bestIndex = group14(in,  w,  h);
        if ( prevIndex != bestIndex ) converted = true;
    }
    if ( m_CharLibData[bestIndex].name[0] == '0' || m_CharLibData[bestIndex].name[0] == 'S') 
    {
        bestIndex = group0S(in,  w,  h);
        if ( prevIndex != bestIndex ) converted = true;
    }

    if ( m_CharLibData[bestIndex].name[0] == 'M' || m_CharLibData[bestIndex].name[0] == 'N'|| m_CharLibData[bestIndex].name[0] == 'W') 
    {
        bestIndex = groupMNW (in,  w,  h);
        if ( prevIndex != bestIndex ) converted = true;
    }
    if ( m_CharLibData[bestIndex].name[0] == 'P' || m_CharLibData[bestIndex].name[0] == '8') 
    {
        bestIndex = groupP8 (in,  w,  h);
        if ( prevIndex != bestIndex ) converted = true;
    }
    /*if ( m_CharLibData[bestIndex].name[0] == 'R' || m_CharLibData[bestIndex].name[0] == 'K') 
    {
        bestIndex = groupRK (in,  w,  h);
        if ( prevIndex != bestIndex ) converted = true;
    }*/
    if ( m_CharLibData[bestIndex].name[0] == 'M' || m_CharLibData[bestIndex].name[0] == 'K') 
    {
        bestIndex = groupMK (in,  w,  h);
        if ( prevIndex != bestIndex ) converted = true;
    }
    if ( m_CharLibData[bestIndex].name[0] == 'P' || m_CharLibData[bestIndex].name[0] == '7') 
    {
        bestIndex = group7P (in,  w,  h);
        if ( prevIndex != bestIndex ) converted = true;
    }
    if ( m_CharLibData[bestIndex].name[0] == 'E' || m_CharLibData[bestIndex].name[0] == 'F') 
    {
        bestIndex = groupEF (in,  w,  h);
        if ( prevIndex != bestIndex ) converted = true;
    }
    if ( m_CharLibData[bestIndex].name[0] == 'E' || m_CharLibData[bestIndex].name[0] == 'G') 
    {
        bestIndex = groupEG (in,  w,  h);
        if ( prevIndex != bestIndex ) converted = true;
    }
    if ( m_CharLibData[bestIndex].name[0] == 'E' || m_CharLibData[bestIndex].name[0] == 'L') 
    {
        bestIndex = groupEL (in,  w,  h);
        if ( prevIndex != bestIndex ) converted = true;
    }
    if ( m_CharLibData[bestIndex].name[0] == '1' || m_CharLibData[bestIndex].name[0] == 'T') 
    {
        bestIndex = group1T (in,  w,  h);
        if ( prevIndex != bestIndex ) converted = true;
    }
    if ( m_CharLibData[bestIndex].name[0] == 'H' || m_CharLibData[bestIndex].name[0] == 'M') 
    {
        bestIndex = groupHM (in,  w,  h);
        if ( prevIndex != bestIndex ) converted = true;
    }
     if ( m_CharLibData[bestIndex].name[0] == 'H' || m_CharLibData[bestIndex].name[0] == 'W') 
    {
        bestIndex = groupHW (in,  w,  h);
        if ( prevIndex != bestIndex ) converted = true;
    }
    if ( m_CharLibData[bestIndex].name[0] == '8' || m_CharLibData[bestIndex].name[0] == 'P') 
    {
        bestIndex = group8P (in,  w,  h);
        if ( prevIndex != bestIndex ) converted = true;
    }
    if ( m_CharLibData[bestIndex].name[0] == 'H' || m_CharLibData[bestIndex].name[0] == 'U') 
    {
        bestIndex = groupHU (in,  w,  h);
        if ( prevIndex != bestIndex ) converted = true;
    }
    if ( m_CharLibData[bestIndex].name[0] == '8' || m_CharLibData[bestIndex].name[0] == '9') 
    {
        bestIndex = group89 (in,  w,  h);
        if ( prevIndex != bestIndex ) converted = true;
    }
    if ( m_CharLibData[bestIndex].name[0] == '5' || m_CharLibData[bestIndex].name[0] == 'D') 
    {
        bestIndex = group5D (in,  w,  h);
        if ( prevIndex != bestIndex ) converted = true;
    }
    if ( m_CharLibData[bestIndex].name[0] == '7' || m_CharLibData[bestIndex].name[0] == 'A') 
    {
        bestIndex = group7A (in,  w,  h);
        if ( prevIndex != bestIndex ) converted = true;
    }    
    if ( m_CharLibData[bestIndex].name[0] == '4' || m_CharLibData[bestIndex].name[0] == 'Z') 
    {
        bestIndex = group4Z (in,  w,  h);
        if ( prevIndex != bestIndex ) converted = true;
    }
    if ( m_CharLibData[bestIndex].name[0] == '4' || m_CharLibData[bestIndex].name[0] == 'A') 
    {
        bestIndex = group4A (in,  w,  h);
        if ( prevIndex != bestIndex ) converted = true;
    }
    if ( m_CharLibData[bestIndex].name[0] == '3' || m_CharLibData[bestIndex].name[0] == '7') 
    {
        bestIndex = group37 (in,  w,  h);
        if ( prevIndex != bestIndex ) converted = true;
    }
    if ( m_CharLibData[bestIndex].name[0] == '9' || m_CharLibData[bestIndex].name[0] == 'R') 
    {
        bestIndex = group9R (in,  w,  h);
        if ( prevIndex != bestIndex ) converted = true;
    }
    if ( m_CharLibData[bestIndex].name[0] == '9' || m_CharLibData[bestIndex].name[0] == 'B') 
    {
        bestIndex = group9B (in,  w,  h);
        if ( prevIndex != bestIndex ) converted = true;
    }
    if ( m_CharLibData[bestIndex].name[0] == 'B' || m_CharLibData[bestIndex].name[0] == 'P') 
    {
        bestIndex = groupBP (in,  w,  h);
        if ( prevIndex != bestIndex ) converted = true;
    }
    if ( m_CharLibData[bestIndex].name[0] == 'D' || m_CharLibData[bestIndex].name[0] == 'P') 
    {
        bestIndex = groupDP (in,  w,  h);
        if ( prevIndex != bestIndex ) converted = true;
    }
    if ( m_CharLibData[bestIndex].name[0] == 'N' || m_CharLibData[bestIndex].name[0] == 'Y') 
    {
        bestIndex = groupNY (in,  w,  h);
        if ( prevIndex != bestIndex ) converted = true;
    }
    if ( m_CharLibData[bestIndex].name[0] == 'L' || m_CharLibData[bestIndex].name[0] == '4') 
    {
        bestIndex = group4L (in,  w,  h);
        if ( prevIndex != bestIndex ) converted = true;
    }
    if ( m_CharLibData[bestIndex].name[0] == '6' || m_CharLibData[bestIndex].name[0] == 'B') 
    {
        bestIndex = group6B (in,  w,  h);
        if ( prevIndex != bestIndex ) converted = true;
    }
    if ( m_CharLibData[bestIndex].name[0] == '9' || m_CharLibData[bestIndex].name[0] == 'P') 
    {
        bestIndex = group9P (in,  w,  h);
        if ( prevIndex != bestIndex ) converted = true;
    }
    if ( m_CharLibData[bestIndex].name[0] == 'G' || m_CharLibData[bestIndex].name[0] == '6') 
    {
        bestIndex = group6G (in,  w,  h);
        if ( prevIndex != bestIndex ) converted = true;
    }
    if ( m_CharLibData[bestIndex].name[0] == '1' || m_CharLibData[bestIndex].name[0] == 'X') 
    {
        bestIndex = group1X (in,  w,  h);
        if ( prevIndex != bestIndex ) converted = true;
    }
    /* if ( m_CharLibData[bestIndex].name[0] == '2' || m_CharLibData[bestIndex].name[0] == 'Z') 
    {
        bestIndex = group2Z (in,  w,  h);
        if ( prevIndex != bestIndex ) converted = true;
    }*/

    if ( converted == true )
        bestIndex = secondPassCheck ( in, w,   h,  bestIndex,  recursion, diagChar, score );

    *score = m_MaxScore;

    if ( m_diagnosticCharPtr  !=  0 ) DrawDiagCharBestCharAgainstInput( in, w,   h ,  bestIndex );

    return ( bestIndex );
}

//===========================================================

void DrawDiagCharBestCharAgainstInput( int * in,int w, int  h , int  bestIndex )
{
    int x,y,xo;
    int *charData;
    charData = (int*) m_CharLibData[ bestIndex ].data;

    for (x=0; x < w; x++)
    {
        xo = x * h;

        for (y=0; y < h; y++)
        {
       //  *(m_diagnosticCharPtr+xo+y)  = (  *(in+xo+y)) * (*(charData+xo+y));
           *(m_diagnosticCharPtr+xo+y)  = (  *(in+xo+y));
        }
    }

}





int groupHN ( int * in,int w, int  h )
{

   
    int c;
    int x,y,xo;
    int *charData;
   

    m_MaxScore = -999999999.f;
    int bestIndex=-1;


    for ( c=0; c < NUM_CHARS; c++)
    {                 
        scores[c] = 0;
        charData = (int*) m_CharLibData[c].data;
        if ( m_CharLibData[c].name[0] != 'N' && m_CharLibData[c].name[0] != 'H' ) continue;
     
        for (x=5; x < 10; x++)
        {
            xo = x * h;

            for (y=0; y < 20; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
        }

        
        for (x=15; x < 20; x++)
        {
            xo = x * h;

            for (y=25; y < 35; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
        }


    }


  
    for ( c=0; c < NUM_CHARS; c++)
    {
        if (  scores[c] > m_MaxScore )
        {
            m_MaxScore = scores[c];
            bestIndex = c;
        }  
    }

  

    return ( bestIndex  );

}



int group1X ( int * in,int w, int  h )
{

   
    int c;
    int x,y,xo;
    int *charData;
   

    m_MaxScore = -999999999.f;
    int bestIndex=-1;


    for ( c=0; c < NUM_CHARS; c++)
    {                 
        scores[c] = 0;
        charData = (int*) m_CharLibData[c].data;
        if ( m_CharLibData[c].name[0] != '1' && m_CharLibData[c].name[0] != 'X' ) continue;
     
        for (x=13; x < 20; x++)
        {
            xo = x * h;

            for (y=0; y < 8; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
        }

        
        for (x=0; x < 20; x++)
        {
            xo = x * h;

            for (y=25; y < 35; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
        }


    }


  
    for ( c=0; c < NUM_CHARS; c++)
    {
        if (  scores[c] > m_MaxScore )
        {
            m_MaxScore = scores[c];
            bestIndex = c;
        }  
    }

  

    return ( bestIndex  );

}




int group2Z ( int * in,int w, int  h )
{

   
    int c;
    int x,y,xo;
    int *charData;
   

    m_MaxScore = -999999999.f;
    int bestIndex=-1;


    for ( c=0; c < NUM_CHARS; c++)
    {                 
        scores[c] = 0;
        charData = (int*) m_CharLibData[c].data;
        if ( m_CharLibData[c].name[0] != '2' && m_CharLibData[c].name[0] != 'Z' ) continue;
     
        for (x=0; x < 7; x++)
        {
            xo = x * h;

            for (y=0; y < 8; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
        }

        
        for (x=18; x < 20; x++)
        {
            xo = x * h;

            for (y=0; y < 5; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
        }


        for (x=2; x < 6; x++)
        {
            xo = x * h;

            for (y=25; y < 36; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
        }
    }


  
    for ( c=0; c < NUM_CHARS; c++)
    {
        if (  scores[c] > m_MaxScore )
        {
            m_MaxScore = scores[c];
            bestIndex = c;
        }  
    }

  

    return ( bestIndex  );

}

int groupB8 ( int * in,int w, int  h )
{

   
    int c;
    int x,y,xo;
    int *charData;
   

    m_MaxScore = -999999999.f;
    int bestIndex=-1;


    for ( c=0; c < NUM_CHARS; c++)
    {                 
        scores[c] = 0;
        charData = (int*) m_CharLibData[c].data;
        if ( m_CharLibData[c].name[0] != 'B' && m_CharLibData[c].name[0] != '8' ) continue;
     
        for (x=0; x < 5; x++)
        {
            xo = x * h;

            for (y=0; y < 40; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
        }
    }


  
    for ( c=0; c < NUM_CHARS; c++)
    {
        if (  scores[c] > m_MaxScore )
        {
            m_MaxScore = scores[c];
            bestIndex = c;
        }  
    }

  

    return ( bestIndex  );

}


int groupD0 ( int * in,int w, int  h )
{

    int c;
    int x,y,xo;
    int *charData;
   

    m_MaxScore = -999999999.f;
    int bestIndex=-1;


    for ( c=0; c < NUM_CHARS; c++)
    {                 
        scores[c] = 0;
        charData = (int*) m_CharLibData[c].data;
        if ( m_CharLibData[c].name[0] != 'D' && m_CharLibData[c].name[0] != '0' ) continue;
     
        for (x=0; x < 7; x++)
        {
            xo = x * h;

            for (y=0; y < 10; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }

            
            for (y=30; y < 40; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
        }
    }


  
    for ( c=0; c < NUM_CHARS; c++)
    {
        if (  scores[c] > m_MaxScore )
        {
            m_MaxScore = scores[c];
            bestIndex = c;
        }  
    }

  

    return ( bestIndex  );

}


//===========================================================

//int groupD0 ( int * in,int w, int  h )
//{
//
//    int c;
//   
//    int bestIndex=-1;
//
//    int isDScore = 0;
//    bool isD = false;
//
//
//    
//    //debug
//  //  return(0);
//
//
//
//    if ( isFilledCornerUpperLeft ( in, w,  h ) )
//        isDScore++;
//
//    if ( isFilledCornerLowerLeft ( in, w,  h ) )
//        isDScore++;
//
//    if ( isDScore == 2 ) isD = true;
//
//    for ( c=0; c < NUM_CHARS; c++)
//    {
//        if (  isD )
//        {
//            if ( m_CharLibData[c].name[0] == 'D' )
//            {
//                bestIndex = c;
//                break;
//            }
//        }  
//        else
//        {
//            if ( m_CharLibData[c].name[0] == '0' )
//            {
//                bestIndex = c;
//                break;
//            }
//        }  
//
//    }
//
//
//    return ( bestIndex  );
//
//}
//


bool isFilledCornerLowerLeft ( int * in,int w, int  h )
{
    int blackCnt = 0;
    int x,y,xo;
    int lineX, lineY;

    float slope;
    float b;
    int cornerDepth = 4;//3
    int blkThresh = 2;

    int yBottom, xBottom;
    int cnt;
    float leftSide;
    float rightSide;

    ///
    ///  LOWER LEFT


    // find y at the bottom of the character
    blackCnt = 0;
    for (y=h-1; y >= (h/2); y--)
    {
        if ( blackCnt > blkThresh) break;
        blackCnt = 0;
        for (x= 0; x < w/2; x++)
        {
            xo= x * h;
            if (  *(in+xo+y ) < 0 )
                blackCnt++;

            if ( blackCnt > blkThresh) break;
        }
    }
    yBottom = y+1;
    if ( yBottom >= h ) yBottom = h-1;


    // find x  at the lower left of the character
    blackCnt = 0;
    for (x= 0; x < w/2; x++)
    {
        if ( blackCnt > blkThresh) break;
        blackCnt = 0;
        for (y=h-1; y >= (h/2); y--)
        {
            xo= x * h;
            if (  *(in+xo+y ) < 0 )
                blackCnt++;

            if ( blackCnt > blkThresh) break;
        }
    }
    xBottom = x-1;
    if (xBottom < 0 ) xBottom = 0;

    slope = 1;

    b = yBottom-cornerDepth - slope * xBottom;


    // now count the black space in the corner
    blackCnt = 0;
    cnt = 0;
    for (x= xBottom; x < xBottom+cornerDepth; x++)
    {
        for (y=yBottom; y >= yBottom-cornerDepth; y--)
        {
            lineY =(int) (slope * x + b);
            if ( y < lineY ) continue;
            lineX =(int) ((y - b) / slope);
            if ( x > lineX ) continue;

            xo= x * h;
            cnt++;
            if (  *(in+xo+y ) < 0 )
                blackCnt++;
        }
    }

    leftSide = (float)blackCnt / cnt;
  
    if ( leftSide < .9 )
    {
        // if this is a B skewed due to roller shutter, make sure that there is more fill on the left than right side
        rightSide =  isFilledCornerLowerRight( in, w,  h ) ;
       

        if ( leftSide > ( rightSide) ) return (true);
        else return (false);
    }
    else
    {
        if ( (float)blackCnt / cnt > 0.5 ) return (true);
        else return  (false);
    }


}




float isFilledCornerLowerRight ( int * in,int w, int  h )
{
    int blackCnt = 0;
    int x,y,xo;
    int lineX, lineY;

    float slope;
    float b;
    int cornerDepth = 5;//3
    int blkThresh = 2;

    int yBottom, xBottom;
    int cnt;

    ///
    ///  LOWER RIGHT


    // find y at the bottom of the character
    blackCnt = 0;
    for (y=h-1; y >= h - (h/2); y--)
    {
        if ( blackCnt > blkThresh) break;
        blackCnt = 0;
        for (x= w-1; x > w/2; x--)
        {
            xo= x * h;
            if (  *(in+xo+y ) < 0 )
                blackCnt++;

            if ( blackCnt > blkThresh) break;
        }
    }
    yBottom = y+1;
    if ( yBottom >= h ) yBottom = h-1;


    // find x  at the lower right of the character
    blackCnt = 0;
    for (x= w-1; x > w/2; x--)
    {
        if ( blackCnt > blkThresh) break;
        blackCnt = 0;
        for (y=h-1; y >= h - (h/2); y--)
        {
            xo= x * h;
            if (  *(in+xo+y ) < 0 )
                blackCnt++;

            if ( blackCnt > blkThresh) break;
        }
    }
    xBottom = x+1;
    if (xBottom >= w ) xBottom = w-1;

    slope = -1;

    b = yBottom-cornerDepth - slope * xBottom;


    // now count the black space in the corner
    blackCnt = 0;
    cnt = 0;
    for (x= xBottom; x >= xBottom-cornerDepth; x--)
    {
        for (y=yBottom; y >= yBottom-cornerDepth; y--)
        {
            lineY =(int) (slope * x + b);
            if ( y < lineY ) continue;
            lineX =(int) ((y - b) / slope);
            if ( x < lineX ) continue;

            xo= x * h;
            cnt++;
            if (  *(in+xo+y ) < 0 )
                blackCnt++;
        }
    }

    return ((float)blackCnt / cnt );
    


}




bool isFilledCornerUpperLeft ( int * in,int w, int  h )
{
    int blackCnt = 0;
    int x,y,xo;
    int lineX, lineY;

    float slope;
    float b;
    int cornerDepth = 4;//3
    int blkThresh = 2;

    int yTop, xTop;
    int cnt;


    blackCnt = 0;
    for (y=0; y < h/2; y++)
    {
        if ( blackCnt > blkThresh) break;
        blackCnt = 0;
        for (x= 0; x < w; x++)
        {
            xo= x * h;
            if (  *(in+xo+y ) < 0 )
                blackCnt++;

            if ( blackCnt > blkThresh) break;
        }

    }
    yTop = y-1;
    if ( yTop < 0 ) yTop = 0;


    // find x  at the upper left of the character
    blackCnt = 0;
    for (x= 0; x < w/2; x++)
    {
        xo= x * h;

        if ( blackCnt > blkThresh) break;
        blackCnt = 0;
        for (y=0; y < h/2; y++)
        {

            if (  *(in+xo+y ) < 0 )
                blackCnt++;

            if ( blackCnt > blkThresh) break;
        }
    }
    xTop = x-1;
    if (xTop < 0 ) xTop = 0;

    slope = -1;

    b = yTop+cornerDepth - slope * xTop;

    // now count the black space in the corner
    blackCnt = 0;
    cnt = 0;
    for (x= xTop; x < xTop+cornerDepth; x++)
    {
        for (y=yTop; y < yTop+cornerDepth; y++)
        {
            lineY =(int) (slope * x + b);
            if ( y > lineY ) continue;
            lineX =(int) ((y - b) / slope);
            if ( x > lineX ) continue;

            xo= x * h;
            cnt++;
            if (  *(in+xo+y ) < 0 )
                blackCnt++;
        }
    }

    if ( (float)blackCnt / cnt > 0.5 ) return ( true);
    else return  (false);

}


int groupBH ( int * in, int w, int  h )
{

    int c;
    int x,y,xo;
    int *charData;

    m_MaxScore = -999999999.f;
    int bestIndex=-1;


    for ( c=0; c < NUM_CHARS; c++)
    {                 
        scores[c] = 0;
        charData = (int*) m_CharLibData[c].data;
        if ( m_CharLibData[c].name[0] != 'B' && m_CharLibData[c].name[0] != 'H') continue;

        for (x=6; x < 14; x++)
        {
            xo = x * h;

            for (y=0; y < 6; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
        }

        for (x=6; x < 14; x++)
        {
            xo = x * h;

            for (y=35; y < 39; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
        }
    }


    for ( c=0; c < NUM_CHARS; c++)
    {
        if (  scores[c] > m_MaxScore )
        {
            m_MaxScore = scores[c];
            bestIndex = c;
        }  
    }


    return ( bestIndex  );

}



int groupMNW  ( int * in,int w, int  h )
{

    int c;
    int x,y,xo;
    int *charData;
   

    m_MaxScore = -999999999.f;
    int bestIndex=-1;


    for ( c=0; c < NUM_CHARS; c++)
    {                 
        scores[c] = 0;
        charData = (int*) m_CharLibData[c].data;
        if ( m_CharLibData[c].name[0] != 'M' && m_CharLibData[c].name[0] != 'N' && m_CharLibData[c].name[0] != 'W') continue;
     
        for (x=5; x < 15; x++)
        {
            xo = x * h;

            for (y=5; y < 34; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
        }
    }


  
    for ( c=0; c < NUM_CHARS; c++)
    {
        if (  scores[c] > m_MaxScore )
        {
            m_MaxScore = scores[c];
            bestIndex = c;
        }  
    }

  

    return ( bestIndex  );

}

int groupSB ( int * in, int w, int  h )
{

    int c;
    int x,y,xo;
    int *charData;

    m_MaxScore = -999999999.f;
    int bestIndex=-1;


    for ( c=0; c < NUM_CHARS; c++)
    {                 
        scores[c] = 0;
        charData = (int*) m_CharLibData[c].data;
        if ( m_CharLibData[c].name[0] != 'S' && m_CharLibData[c].name[0] != 'B' ) continue;

        for (x=15; x < 20; x++)
        {
            xo = x * h;

            for (y=7; y < 15; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
        }


        for (x=0; x < 5; x++)
        {
            xo = x * h;

            for (y=25; y < 34; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
        }

       
    }


    for ( c=0; c < NUM_CHARS; c++)
    {
        if (  scores[c] > m_MaxScore )
        {
            m_MaxScore = scores[c];
            bestIndex = c;
        }  
    }


    return ( bestIndex  );

}



int group14 ( int * in, int w, int  h )
{

    int c;
    int x,y,xo;
    int *charData;

    m_MaxScore = -999999999.f;
    int bestIndex=-1;


    for ( c=0; c < NUM_CHARS; c++)
    {                 
        scores[c] = 0;
        charData = (int*) m_CharLibData[c].data;
        if ( m_CharLibData[c].name[0] != '1' && m_CharLibData[c].name[0] != '4') continue;

        for (x=0; x < 20; x++)
        {
            xo = x * h;

            for (y=12; y < 27; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
        }
      
    }


    for ( c=0; c < NUM_CHARS; c++)
    {
        if (  scores[c] > m_MaxScore )
        {
            m_MaxScore = scores[c];
            bestIndex = c;
        }  
    }


    return ( bestIndex  );

}


int groupRK ( int * in, int w, int  h )
{

    int c;
    int x,y,xo;
    int *charData;

    m_MaxScore = -999999999.f;
    int bestIndex=-1;


    for ( c=0; c < NUM_CHARS; c++)
    {                 
        scores[c] = 0;
        charData = (int*) m_CharLibData[c].data;
        if ( m_CharLibData[c].name[0] != 'R' && m_CharLibData[c].name[0] != 'K') continue;

        for (x=5; x < 15; x++)
        {
            xo = x * h;

            for (y=0; y < 7; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
        }
      
    }


    for ( c=0; c < NUM_CHARS; c++)
    {
        if (  scores[c] > m_MaxScore )
        {
            m_MaxScore = scores[c];
            bestIndex = c;
        }  
    }


    return ( bestIndex  );

}


int groupHW ( int * in, int w, int  h )
{

    int c;
    int x,y,xo;
    int *charData;

    m_MaxScore = -999999999.f;
    int bestIndex=-1;


    for ( c=0; c < NUM_CHARS; c++)
    {                 
        scores[c] = 0;
        charData = (int*) m_CharLibData[c].data;
        if ( m_CharLibData[c].name[0] != 'H' && m_CharLibData[c].name[0] != 'W') continue;

        for (x=6; x < 14; x++)
        {
            xo = x * h;

            for (y=0; y < 15; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }

            for (y=25; y < 40; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }

        }
      
    }


    for ( c=0; c < NUM_CHARS; c++)
    {
        if (  scores[c] > m_MaxScore )
        {
            m_MaxScore = scores[c];
            bestIndex = c;
        }  
    }


    return ( bestIndex  );

}





int groupHM ( int * in, int w, int  h )
{

    int c;
    int x,y,xo;
    int *charData;

    m_MaxScore = -999999999.f;
    int bestIndex=-1;


    for ( c=0; c < NUM_CHARS; c++)
    {                 
        scores[c] = 0;
        charData = (int*) m_CharLibData[c].data;
        if ( m_CharLibData[c].name[0] != 'H' && m_CharLibData[c].name[0] != 'M') continue;

        for (x=5; x < 15; x++)
        {
            xo = x * h;

            for (y=10; y < 30; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
        }
      
    }


    for ( c=0; c < NUM_CHARS; c++)
    {
        if (  scores[c] > m_MaxScore )
        {
            m_MaxScore = scores[c];
            bestIndex = c;
        }  
    }


    return ( bestIndex  );

}




int groupMK ( int * in, int w, int  h )
{

    int c;
    int x,y,xo;
    int *charData;

    m_MaxScore = -999999999.f;
    int bestIndex=-1;


    for ( c=0; c < NUM_CHARS; c++)
    {                 
        scores[c] = 0;
        charData = (int*) m_CharLibData[c].data;
        if ( m_CharLibData[c].name[0] != 'M' && m_CharLibData[c].name[0] != 'K') continue;

        for (x=12; x < 20; x++)
        {
            xo = x * h;

            for (y=7; y < 20; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
        }
      
    }


    for ( c=0; c < NUM_CHARS; c++)
    {
        if (  scores[c] > m_MaxScore )
        {
            m_MaxScore = scores[c];
            bestIndex = c;
        }  
    }


    return ( bestIndex  );

}



int groupP2 ( int * in, int w, int  h )
{

    int c;
    int x,y,xo;
    int *charData;

    m_MaxScore = -999999999.f;
    int bestIndex=-1;


    for ( c=0; c < NUM_CHARS; c++)
    {                 
        scores[c] = 0;
        charData = (int*) m_CharLibData[c].data;
        if ( m_CharLibData[c].name[0] != 'P' && m_CharLibData[c].name[0] != '2') continue;

        for (x=6; x < 20; x++)
        {
            xo = x * h;

            for (y=34; y < 40; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
        }
      

        for (x=0; x < 6; x++)
        {
            xo = x * h;

            for (y=11; y < 15; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
        }
    }


    for ( c=0; c < NUM_CHARS; c++)
    {
        if (  scores[c] > m_MaxScore )
        {
            m_MaxScore = scores[c];
            bestIndex = c;
        }  
    }


    return ( bestIndex  );

}


int groupPH ( int * in, int w, int  h )
{

    int c;
    int x,y,xo;
    int *charData;

    m_MaxScore = -999999999.f;
    int bestIndex=-1;


    for ( c=0; c < NUM_CHARS; c++)
    {                 
        scores[c] = 0;
        charData = (int*) m_CharLibData[c].data;
        if ( m_CharLibData[c].name[0] != 'P' && m_CharLibData[c].name[0] != 'H') continue;

        for (x=6; x < 15; x++)
        {
            xo = x * h;

            for (y=0; y < 6; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
        }
      

        for (x=15; x < 20; x++)
        {
            xo = x * h;

            for (y=27; y < 40; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
        }
    }


    for ( c=0; c < NUM_CHARS; c++)
    {
        if (  scores[c] > m_MaxScore )
        {
            m_MaxScore = scores[c];
            bestIndex = c;
        }  
    }


    return ( bestIndex  );

}



int group2E ( int * in, int w, int  h )
{

    int c;
    int x,y,xo;
    int *charData;

    m_MaxScore = -999999999.f;
    int bestIndex=-1;


    for ( c=0; c < NUM_CHARS; c++)
    {                 
        scores[c] = 0;
        charData = (int*) m_CharLibData[c].data;
        if ( m_CharLibData[c].name[0] != '2' && m_CharLibData[c].name[0] != 'E') continue;

        for (x=0; x < 20; x++)
        {
            xo = x * h;

            for (y=6; y < 16; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
        }

    }


    for ( c=0; c < NUM_CHARS; c++)
    {
        if (  scores[c] > m_MaxScore )
        {
            m_MaxScore = scores[c];
            bestIndex = c;
        }  
    }


    return ( bestIndex  );

}



int group56 ( int * in, int w, int  h )
{

    int c;
    int x,y,xo;
    int *charData;

    m_MaxScore = -999999999.f;
    int bestIndex=-1;


    for ( c=0; c < NUM_CHARS; c++)
    {                 
        scores[c] = 0;
        charData = (int*) m_CharLibData[c].data;
        if ( m_CharLibData[c].name[0] != '5' && m_CharLibData[c].name[0] != '6') continue;


        for (x=0; x < 7; x++)
        {
            xo = x * h;

            for (y=0; y < 10; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
        }
      

        for (x=0; x < 10; x++)
        {
            xo = x * h;

            for (y=20; y < 30; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
        }
      
    }


    for ( c=0; c < NUM_CHARS; c++)
    {
        if (  scores[c] > m_MaxScore )
        {
            m_MaxScore = scores[c];
            bestIndex = c;
        }  
    }


    return ( bestIndex  );

}





int groupPF ( int * in, int w, int  h )
{

    int c;
    int x,y,xo;
    int *charData;

    m_MaxScore = -999999999.f;
    int bestIndex=-1;


    for ( c=0; c < NUM_CHARS; c++)
    {                 
        scores[c] = 0;
        charData = (int*) m_CharLibData[c].data;
        if ( m_CharLibData[c].name[0] != 'P' && m_CharLibData[c].name[0] != 'F') continue;

        for (x=15; x < 20; x++)
        {
            xo = x * h;

            for (y=7; y < 15; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
        }
      
    }


    for ( c=0; c < NUM_CHARS; c++)
    {
        if (  scores[c] > m_MaxScore )
        {
            m_MaxScore = scores[c];
            bestIndex = c;
        }  
    }


    return ( bestIndex  );

}


int groupG0 ( int * in,int w, int  h )
{

    int c;
    int x,y,xo;
    int *charData;

    m_MaxScore = -999999999.f;
    int bestIndex=-1;


    for ( c=0; c < NUM_CHARS; c++)
    {                 
        scores[c] = 0;
        charData = (int*) m_CharLibData[c].data;
        if ( m_CharLibData[c].name[0] != 'G' && m_CharLibData[c].name[0] != '0') continue;

        for (x=14; x < 20; x++)
        {
            xo = x * h;

            for (y=7; y < 15; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
        }

        for (x=10; x < 16; x++)
        {
            xo = x * h;

            for (y=20; y < 28; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
        }
      
    }


    for ( c=0; c < NUM_CHARS; c++)
    {
        if (  scores[c] > m_MaxScore )
        {
            m_MaxScore = scores[c];
            bestIndex = c;
        }  
    }


    return ( bestIndex  );

}



int groupC0 ( int * in,int w, int  h )
{

    int c;
    int x,y,xo;
    int *charData;

    m_MaxScore = -999999999.f;
    int bestIndex=-1;


    for ( c=0; c < NUM_CHARS; c++)
    {                 
        scores[c] = 0;
        charData = (int*) m_CharLibData[c].data;
        if ( m_CharLibData[c].name[0] != 'C' && m_CharLibData[c].name[0] != '0') continue;

        for (x=15; x < 20; x++)
        {
            xo = x * h;

            for (y=9; y < 27; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
        }
    }


    for ( c=0; c < NUM_CHARS; c++)
    {
        if (  scores[c] > m_MaxScore )
        {
            m_MaxScore = scores[c];
            bestIndex = c;
        }  
    }


    return ( bestIndex  );

}




int group7Z ( int * in,int w, int  h )
{

    int c;
    int x,y,xo;
    int *charData;

    m_MaxScore = -999999999.f;
    int bestIndex=-1;


    for ( c=0; c < NUM_CHARS; c++)
    {                 
        scores[c] = 0;
        charData = (int*) m_CharLibData[c].data;
        if ( m_CharLibData[c].name[0] != '7' && m_CharLibData[c].name[0] != 'Z') continue;

        for (x=0; x < 20; x++)
        {
            xo = x * h;

            for (y=34; y < 40; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
        }
    }


    for ( c=0; c < NUM_CHARS; c++)
    {
        if (  scores[c] > m_MaxScore )
        {
            m_MaxScore = scores[c];
            bestIndex = c;
        }  
    }


    return ( bestIndex  );

}


int group5S ( int * in,int w, int  h )
{

    int c;
    int x,y,xo;
    int *charData;

    m_MaxScore = -999999999.f;
    int bestIndex=-1;


    for ( c=0; c < NUM_CHARS; c++)
    {                 
        scores[c] = 0;
        charData = (int*) m_CharLibData[c].data;
        if ( m_CharLibData[c].name[0] != '5' && m_CharLibData[c].name[0] != 'S') continue;

        for (x=1; x < 19; x++)
        {
            xo = x * h;

            for (y=1; y < 39; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
        }
        /*
        for (x=1; x < 20; x++)
        {
            xo = x * h;

            for (y=1; y < 7; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
        }

     
        for (x=0; x < 8; x++)
        {
            xo = x * h;

            for (y=0; y < 22; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
        }

        for (x=0; x < 20; x++)
        {
            xo = x * h;

            for (y=15; y < 25; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
        }*/
    }


    for ( c=0; c < NUM_CHARS; c++)
    {
        if (  scores[c] > m_MaxScore )
        {
            m_MaxScore = scores[c];
            bestIndex = c;
        }  
    }

    //if (  m_diagnosticCharPtr != 0)
    //{
    //    charData = (int*) m_CharLibData[bestIndex ].data;
    //    //  createDiagImage  ( in, c ,   m_diagnosticCharPtr);
    //    for (x=0; x < STANDARD_WIDTH; x++)
    //    {
    //        xo = x * STANDARD_HEIGHT;

    //        for (y=0; y < STANDARD_HEIGHT; y++)
    //        { 
    //            *(m_diagnosticCharPtr+xo+y) =(int) ( (float) *(in+xo+y)) * ((float)*(charData+xo+y)) ;
    //        }
    //    }
    //}


    return ( bestIndex  );

}


int group0S ( int * in,int w, int  h )
{

    int c;
    int x,y,xo;
    int *charData;

    m_MaxScore = -999999999.f;
    int bestIndex=-1;


    for ( c=0; c < NUM_CHARS; c++)
    {                 
        scores[c] = 0;
        charData = (int*) m_CharLibData[c].data;
        if ( m_CharLibData[c].name[0] != '0' && m_CharLibData[c].name[0] != 'S') continue;

        for (x=6; x < 13; x++)
        {
            xo = x * h;

            for (y=6; y < 30; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
        }

    
    }


    for ( c=0; c < NUM_CHARS; c++)
    {
        if (  scores[c] > m_MaxScore )
        {
            m_MaxScore = scores[c];
            bestIndex = c;
        }  
    }


    return ( bestIndex  );

}




int group38B ( int * in,int w, int  h )
{
    int c;
    int x,y,xo;
    int *charData;

    m_MaxScore = -999999999.f;
    int bestIndex=-1;


    for ( c=0; c < NUM_CHARS; c++)
    {                 
        scores[c] = 0;
        charData = (int*) m_CharLibData[c].data;
        if ( m_CharLibData[c].name[0] != '3' && m_CharLibData[c].name[0] != '8'  && m_CharLibData[c].name[0] != 'B') continue;

        for (x=0; x < 6; x++)
        {
            xo = x * h;

            for (y=14; y < 25; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
        }

    }


    for ( c=0; c < NUM_CHARS; c++)
    {
        if (  scores[c] > m_MaxScore )
        {
            m_MaxScore = scores[c];
            bestIndex = c;
        }  
    }

    return(bestIndex);
}



int groupP8 ( int * in,int w, int  h )
{

    int c;
    int x,y,xo;
    int *charData;

    m_MaxScore = -999999999.f;
    int bestIndex=-1;


    for ( c=0; c < NUM_CHARS; c++)
    {                 
        scores[c] = 0;
        charData = (int*) m_CharLibData[c].data;
        if ( m_CharLibData[c].name[0] != 'P' && m_CharLibData[c].name[0] != '8' ) continue;

        for (x=15; x < 20; x++)
        {
            xo = x * h;

            for (y=20; y < 40; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
        }


        for (x=0; x < 20; x++)
        {
            xo = x * h;

            for (y=31; y < 40; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
        }
    }


    for ( c=0; c < NUM_CHARS; c++)
    {
        if (  scores[c] > m_MaxScore )
        {
            m_MaxScore = scores[c];
            bestIndex = c;
        }  
    }


    return ( bestIndex  );

}





int group28 ( int * in,int w, int  h )
{

    int c;
    int x,y,xo;
    int *charData;

    m_MaxScore = -999999999.f;
    int bestIndex=-1;


    for ( c=0; c < NUM_CHARS; c++)
    {                 
        scores[c] = 0;
        charData = (int*) m_CharLibData[c].data;
        if ( m_CharLibData[c].name[0] != '2' && m_CharLibData[c].name[0] != '8') continue;

        for (x=0; x < 6; x++)
        {
            xo = x * h;

            for (y=4; y < 16; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
        }

        for (x=15; x < 20; x++)
        {
            xo = x * h;

            for (y=25; y < 34; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
        }

    }


    for ( c=0; c < NUM_CHARS; c++)
    {
        if (  scores[c] > m_MaxScore )
        {
            m_MaxScore = scores[c];
            bestIndex = c;
        }  
    }


    return ( bestIndex  );

}





int group68 ( int * in,int w, int  h )
{

    int c;
    int x,y,xo;
    int *charData;

    m_MaxScore = -999999999.f;
    int bestIndex=-1;


    for ( c=0; c < NUM_CHARS; c++)
    {                 
        scores[c] = 0;
        charData = (int*) m_CharLibData[c].data;
        if ( m_CharLibData[c].name[0] != '6' && m_CharLibData[c].name[0] != '8') continue;

        for (x=9; x < 20; x++)
        {
            xo = x * h;

            for (y=4; y < 16; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
        }

    }


    for ( c=0; c < NUM_CHARS; c++)
    {
        if (  scores[c] > m_MaxScore )
        {
            m_MaxScore = scores[c];
            bestIndex = c;
        }  
    }


    return ( bestIndex  );

}





int groupUL ( int * in,int w, int  h )
{

    int c;
    int x,y,xo;
    int *charData;

    m_MaxScore = -999999999.f;
    int bestIndex=-1;


    for ( c=0; c < NUM_CHARS; c++)
    {                 
        scores[c] = 0;
        charData = (int*) m_CharLibData[c].data;
        if ( m_CharLibData[c].name[0] != 'U' && m_CharLibData[c].name[0] != 'L') continue;

        for (x=14; x < 20; x++)
        {
            xo = x * h;

            for (y=0; y < 33; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
        }
    }


    for ( c=0; c < NUM_CHARS; c++)
    {
        if (  scores[c] > m_MaxScore )
        {
            m_MaxScore = scores[c];
            bestIndex = c;
        }  
    }


    return ( bestIndex  );

}



int groupB6 ( int * in,int w, int  h )
{

    int c;
    int x,y,xo;
    int *charData;

    m_MaxScore = -999999999.f;
    int bestIndex=-1;


    for ( c=0; c < NUM_CHARS; c++)
    {                 
        scores[c] = 0;
        charData = (int*) m_CharLibData[c].data;
        if ( m_CharLibData[c].name[0] != 'B' && m_CharLibData[c].name[0] != '6') continue;

        for (x=15; x < 20; x++)
        {
            xo = x * h;

            for (y=4; y < 18; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
        }
     
        for (x=0; x < 8; x++)
        {
            xo = x * h;

            for (y=0; y < 12; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
        }
    }


    for ( c=0; c < NUM_CHARS; c++)
    {
        if (  scores[c] > m_MaxScore )
        {
            m_MaxScore = scores[c];
            bestIndex = c;
        }  
    }


    return ( bestIndex  );

}


int groupJ3 ( int * in,int w, int  h )
{

    int c;
    int x,y,xo;
    int *charData;

    m_MaxScore = -999999999.f;
    int bestIndex=-1;


    for ( c=0; c < NUM_CHARS; c++)
    {                 
        scores[c] = 0;
        charData = (int*) m_CharLibData[c].data;
        if ( m_CharLibData[c].name[0] != 'J' && m_CharLibData[c].name[0] != '3') continue;

        for (x=0; x < 13; x++)
        {
            xo = x * h;

            for (y=0; y < 7; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
        }
    }


    for ( c=0; c < NUM_CHARS; c++)
    {
        if (  scores[c] > m_MaxScore )
        {
            m_MaxScore = scores[c];
            bestIndex = c;
        }  
    }


    return ( bestIndex  );

}


int groupGC ( int * in,int w, int  h )
{

    int c;
    int x,y,xo;
    int pointX, pointY;
    int *charData;
    int startX, endX, startY, endY;

    m_MaxScore = -999999999.f;
    int bestIndex=-1;


    pointX = 0;
    pointY = 0;

    // find first dark pixel on the G/C hook 
    for (x=10; x < 20; x++)
    {
        xo = x * h;

        for (y=12; y < 28; y++)
        {
            if (  *(in+xo+y) < 0)
            {
                pointX = x;
                pointY = y;
                x = 20;
                y = 28;
            }
        }
    }

    if ( pointX == 0 && pointY == 0 )
    {
        return ( lookUpByName ( 'C' ));
    }

    startX = pointX - 5;
    endX = pointX + 5;
    if ( endX > 19 ) endX = 19;

    startY = pointY-5;
    endY = pointY+5;

    for ( c=0; c < NUM_CHARS; c++)
    {                 
        scores[c] = 0;
        charData = (int*) m_CharLibData[c].data;
        if ( m_CharLibData[c].name[0] != 'G' && m_CharLibData[c].name[0] != 'C') continue;

        for (x=startX; x < endX; x++)
        {
            xo = x * h;

            for (y=startY; y < endY; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
        }
    }


    for ( c=0; c < NUM_CHARS; c++)
    {
        if (  scores[c] > m_MaxScore )
        {
            m_MaxScore = scores[c];
            bestIndex = c;
        }  
    }


    return ( bestIndex  );

}



int groupD0U( int * in,int w, int  h )
{

    int c;
    int x,y,xo;
    int *charData;

    m_MaxScore = -999999999.f;
    int bestIndex=-1;


    for ( c=0; c < NUM_CHARS; c++)
    {                 
        scores[c] = 0;
        charData = (int*) m_CharLibData[c].data;
        if (m_CharLibData[c].name[0] != 'U'  && m_CharLibData[c].name[0] != '0' && m_CharLibData[c].name[0] != 'D') continue;
      


        for (x=3; x < 17; x++)
        {
            xo = x * h;

            for (y=0; y < 5; y++)
            {
                scores[c] += ((float) *(in+xo+y)) * ((float) *(charData+xo+y));
            }
        }

        for (x=6; x < 13; x++)
        {
            xo = x * h;

            for (y=5; y < 10; y++)
            {
                scores[c] += ((float) *(in+xo+y)) * ((float) *(charData+xo+y));
            }
        }


    }


    for ( c=0; c < NUM_CHARS; c++)
    {
        if (  scores[c] > m_MaxScore )
        {
            m_MaxScore = scores[c];
            bestIndex = c;
        }  
    }


    return ( bestIndex  );

}

int groupQ0D ( int * in,int w, int  h )
{

    int c;
    int x,y,xo;
    int *charData;

    m_MaxScore = -999999999.f;
    int bestIndex=-1;


    for ( c=0; c < NUM_CHARS; c++)
    {                 
        scores[c] = 0;
        charData = (int*) m_CharLibData[c].data;
        if ( m_CharLibData[c].name[0] != 'Q' && m_CharLibData[c].name[0] != 'D' && m_CharLibData[c].name[0] != '0') continue;
      

        // lower right Q segment
        for (x=10; x < 18; x++)
        {
            xo = x * h;

            for (y=24; y < 36; y++)
            {
                scores[c] += ((float) *(in+xo+y)) * ((float) *(charData+xo+y));
            }
        }


    }


    for ( c=0; c < NUM_CHARS; c++)
    {
        if (  scores[c] > m_MaxScore )
        {
            m_MaxScore = scores[c];
            bestIndex = c;
        }  
    }


    return ( bestIndex  );

}


int group7T ( int * in,int w, int  h )
{

    int c;
    int x,y,xo;
    int *charData;

    m_MaxScore = -999999999.f;
    int bestIndex=-1;


    for ( c=0; c < NUM_CHARS; c++)
    {                 
        scores[c] = 0;
        charData = (int*) m_CharLibData[c].data;
        if ( m_CharLibData[c].name[0] != '7' && m_CharLibData[c].name[0] != 'T') continue;


        for (x=5; x < 19; x++)
        {
            xo = x * h;

            for (y=6; y < 16; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
        }
    }


    for ( c=0; c < NUM_CHARS; c++)
    {
        if (  scores[c] > m_MaxScore )
        {
            m_MaxScore = scores[c];
            bestIndex = c;
        }  
    }


    return ( bestIndex  );

}



int group7P ( int * in,int w, int  h )
{

    int c;
    int x,y,xo;
    int *charData;

    m_MaxScore = -999999999.f;
    int bestIndex=-1;


    for ( c=0; c < NUM_CHARS; c++)
    {                 
        scores[c] = 0;
        charData = (int*) m_CharLibData[c].data;
        if ( m_CharLibData[c].name[0] != '7' && m_CharLibData[c].name[0] != 'P') continue;


        for (x=0; x < 7; x++)
        {
            xo = x * h;

            for (y=5; y < 25; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
        }

    }


    for ( c=0; c < NUM_CHARS; c++)
    {
        if (  scores[c] > m_MaxScore )
        {
            m_MaxScore = scores[c];
            bestIndex = c;
        }  
    }


    return ( bestIndex  );

}

int groupEF ( int * in,int w, int  h )
{

    int c;
    int x,y,xo;
    int *charData;

    m_MaxScore = -999999999.f;
    int bestIndex=-1;


    for ( c=0; c < NUM_CHARS; c++)
    {                 
        scores[c] = 0;
        charData = (int*) m_CharLibData[c].data;
        if ( m_CharLibData[c].name[0] != 'E' && m_CharLibData[c].name[0] != 'F') continue;


        for (x=6; x < 20; x++)
        {
            xo = x * h;

            for (y=32; y < 40; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
        }

    }


    for ( c=0; c < NUM_CHARS; c++)
    {
        if (  scores[c] > m_MaxScore )
        {
            m_MaxScore = scores[c];
            bestIndex = c;
        }  
    }


    return ( bestIndex  );

}


int groupEG ( int * in,int w, int  h )
{

    int c;
    int x,y,xo;
    int *charData;

    m_MaxScore = -999999999.f;
    int bestIndex=-1;


    for ( c=0; c < NUM_CHARS; c++)
    {                 
        scores[c] = 0;
        charData = (int*) m_CharLibData[c].data;
        if ( m_CharLibData[c].name[0] != 'E' && m_CharLibData[c].name[0] != 'G') continue;


        for (x=15; x < 20; x++)
        {
            xo = x * h;

            for (y=22; y < 37; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
        }

    }


    for ( c=0; c < NUM_CHARS; c++)
    {
        if (  scores[c] > m_MaxScore )
        {
            m_MaxScore = scores[c];
            bestIndex = c;
        }  
    }


    return ( bestIndex  );

}


int groupEL ( int * in,int w, int  h )
{

    int c;
    int x,y,xo;
    int *charData;

    m_MaxScore = -999999999.f;
    int bestIndex=-1;


    for ( c=0; c < NUM_CHARS; c++)
    {                 
        scores[c] = 0;
        charData = (int*) m_CharLibData[c].data;
        if ( m_CharLibData[c].name[0] != 'E' && m_CharLibData[c].name[0] != 'L') continue;


        for (x=5; x < 20; x++)
        {
            xo = x * h;

            for (y=0; y < 9; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
            
            for (y=13; y < 28; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
        }

    }


    for ( c=0; c < NUM_CHARS; c++)
    {
        if (  scores[c] > m_MaxScore )
        {
            m_MaxScore = scores[c];
            bestIndex = c;
        }  
    }


    return ( bestIndex  );

}


int group8P ( int * in,int w, int  h )
{

    int c;
    int x,y,xo;
    int *charData;

    m_MaxScore = -999999999.f;
    int bestIndex=-1;


    for ( c=0; c < NUM_CHARS; c++)
    {                 
        scores[c] = 0;
        charData = (int*) m_CharLibData[c].data;
        if ( m_CharLibData[c].name[0] != '8' && m_CharLibData[c].name[0] != 'P') continue;


        for (x=12; x < 19; x++)
        {
            xo = x * h;

            for (y=24; y < 37; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
        }

    }


    for ( c=0; c < NUM_CHARS; c++)
    {
        if (  scores[c] > m_MaxScore )
        {
            m_MaxScore = scores[c];
            bestIndex = c;
        }  
    }


    return ( bestIndex  );

}



int group1T ( int * in,int w, int  h )
{

    int c;
    int x,y,xo;
    int *charData;

    m_MaxScore = -999999999.f;
    int bestIndex=-1;


    for ( c=0; c < NUM_CHARS; c++)
    {                 
        scores[c] = 0;
        charData = (int*) m_CharLibData[c].data;
        if ( m_CharLibData[c].name[0] != '1' && m_CharLibData[c].name[0] != 'T') continue;


        for (x=13; x < 20; x++)
        {
            xo = x * h;

            for (y=0; y < 9; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
            
        }

    }


    for ( c=0; c < NUM_CHARS; c++)
    {
        if (  scores[c] > m_MaxScore )
        {
            m_MaxScore = scores[c];
            bestIndex = c;
        }  
    }


    return ( bestIndex  );

}





int groupHU ( int * in,int w, int  h )
{

    int c;
    int x,y,xo;
    int *charData;

    m_MaxScore = -999999999.f;
    int bestIndex=-1;


    for ( c=0; c < NUM_CHARS; c++)
    {                 
        scores[c] = 0;
        charData = (int*) m_CharLibData[c].data;
        if ( m_CharLibData[c].name[0] != 'H' && m_CharLibData[c].name[0] != 'U') continue;


        for (x=8; x < 12; x++)
        {
            xo = x * h;

            for (y=17; y < 40; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
            
        }

    }


    for ( c=0; c < NUM_CHARS; c++)
    {
        if (  scores[c] > m_MaxScore )
        {
            m_MaxScore = scores[c];
            bestIndex = c;
        }  
    }


    return ( bestIndex  );

}



int group89 ( int * in,int w, int  h )
{

    int c;
    int x,y,xo;
    int *charData;

    m_MaxScore = -999999999.f;
    int bestIndex=-1;


    for ( c=0; c < NUM_CHARS; c++)
    {                 
        scores[c] = 0;
        charData = (int*) m_CharLibData[c].data;
        if ( m_CharLibData[c].name[0] != '8' && m_CharLibData[c].name[0] != '9') continue;


        for (x=0; x < 8; x++)
        {
            xo = x * h;

            for (y=22; y < 35; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
            
        }

    }


    for ( c=0; c < NUM_CHARS; c++)
    {
        if (  scores[c] > m_MaxScore )
        {
            m_MaxScore = scores[c];
            bestIndex = c;
        }  
    }


    return ( bestIndex  );

}



int group5D ( int * in,int w, int  h )
{

    int c;
    int x,y,xo;
    int *charData;

    m_MaxScore = -999999999.f;
    int bestIndex=-1;


    for ( c=0; c < NUM_CHARS; c++)
    {                 
        scores[c] = 0;
        charData = (int*) m_CharLibData[c].data;
        if ( m_CharLibData[c].name[0] != '5' && m_CharLibData[c].name[0] != 'D') continue;


        for (x=0; x < 8; x++)
        {
            xo = x * h;

            for (y=25; y < 35; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
            
        }

        
        for (x=14; x < 19; x++)
        {
            xo = x * h;

            for (y=5; y < 18; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
            
        }

    }


    for ( c=0; c < NUM_CHARS; c++)
    {
        if (  scores[c] > m_MaxScore )
        {
            m_MaxScore = scores[c];
            bestIndex = c;
        }  
    }


    return ( bestIndex  );

}



int group37 ( int * in,int w, int  h )
{

    int c;
    int x,y,xo;
    int *charData;

    m_MaxScore = -999999999.f;
    int bestIndex=-1;


    for ( c=0; c < NUM_CHARS; c++)
    {                 
        scores[c] = 0;
        charData = (int*) m_CharLibData[c].data;
        if ( m_CharLibData[c].name[0] != '7' && m_CharLibData[c].name[0] != '3') continue;


        for (x=5; x < 20; x++)
        {
            xo = x * h;

            for (y=30; y < 40; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
            
        }

    }


    for ( c=0; c < NUM_CHARS; c++)
    {
        if (  scores[c] > m_MaxScore )
        {
            m_MaxScore = scores[c];
            bestIndex = c;
        }  
    }


    return ( bestIndex  );

}

int group39 ( int * in,int w, int  h )
{

    int c;
    int x,y,xo;
    int *charData;

    m_MaxScore = -999999999.f;
    int bestIndex=-1;


    for ( c=0; c < NUM_CHARS; c++)
    {                 
        scores[c] = 0;
        charData = (int*) m_CharLibData[c].data;
        if ( m_CharLibData[c].name[0] != '3' && m_CharLibData[c].name[0] != '9') continue;


        for (x=0; x < 6; x++)
        {
            xo = x * h;

            for (y=3; y < 16; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
            
        }

    }


    for ( c=0; c < NUM_CHARS; c++)
    {
        if (  scores[c] > m_MaxScore )
        {
            m_MaxScore = scores[c];
            bestIndex = c;
        }  
    }


    return ( bestIndex  );

}



int group7A ( int * in,int w, int  h )
{

    int c;
    int x,y,xo;
    int *charData;

    m_MaxScore = -999999999.f;
    int bestIndex=-1;


    for ( c=0; c < NUM_CHARS; c++)
    {                 
        scores[c] = 0;
        charData = (int*) m_CharLibData[c].data;
        if ( m_CharLibData[c].name[0] != '7' && m_CharLibData[c].name[0] != 'A') continue;


        for (x=0; x < 8; x++)
        {
            xo = x * h;

            for (y=0; y < 6; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
            
        }

        
        for (x=14; x < 19; x++)
        {
            xo = x * h;

            for (y=35; y < 39; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
            
        }

    }


    for ( c=0; c < NUM_CHARS; c++)
    {
        if (  scores[c] > m_MaxScore )
        {
            m_MaxScore = scores[c];
            bestIndex = c;
        }  
    }


    return ( bestIndex  );

}


int group4Z ( int * in,int w, int  h )
{

    int c;
    int x,y,xo;
    int *charData;

    m_MaxScore = -999999999.f;
    int bestIndex=-1;


    for ( c=0; c < NUM_CHARS; c++)
    {                 
        scores[c] = 0;
        charData = (int*) m_CharLibData[c].data;
        if ( m_CharLibData[c].name[0] != '4' && m_CharLibData[c].name[0] != 'Z') continue;


        for (x=0; x < 10; x++)
        {
            xo = x * h;

            for (y=0; y < 6; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }

        }


        for (x=12; x < 20; x++)
        {
            xo = x * h;

            for (y=20; y < 30; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }

        }

    }


    for ( c=0; c < NUM_CHARS; c++)
    {
        if (  scores[c] > m_MaxScore )
        {
            m_MaxScore = scores[c];
            bestIndex = c;
        }  
    }


    return ( bestIndex  );

}

int group9R ( int * in,int w, int  h )
{

    int c;
    int x,y,xo;
    int *charData;

    m_MaxScore = -999999999.f;
    int bestIndex=-1;


    for ( c=0; c < NUM_CHARS; c++)
    {                 
        scores[c] = 0;
        charData = (int*) m_CharLibData[c].data;
        if ( m_CharLibData[c].name[0] != '9' && m_CharLibData[c].name[0] != 'R') continue;


        for (x=0; x < 6; x++)
        {
            xo = x * h;

            for (y=20; y < 40; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
            
        }

    }


    for ( c=0; c < NUM_CHARS; c++)
    {
        if (  scores[c] > m_MaxScore )
        {
            m_MaxScore = scores[c];
            bestIndex = c;
        }  
    }


    return ( bestIndex  );

}



int group4A ( int * in,int w, int  h )
{

    int c;
    int x,y,xo;
    int *charData;

    m_MaxScore = -999999999.f;
    int bestIndex=-1;


    for ( c=0; c < NUM_CHARS; c++)
    {                 
        scores[c] = 0;
        charData = (int*) m_CharLibData[c].data;
        if ( m_CharLibData[c].name[0] != '4' && m_CharLibData[c].name[0] != 'A') continue;


        for (x=0; x < 8; x++)
        {
            xo = x * h;

            for (y=34; y < 40; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
            
        }

    }


    for ( c=0; c < NUM_CHARS; c++)
    {
        if (  scores[c] > m_MaxScore )
        {
            m_MaxScore = scores[c];
            bestIndex = c;
        }  
    }


    return ( bestIndex  );

}


int group9B ( int * in,int w, int  h )
{

    int c;
    int x,y,xo;
    int *charData;

    m_MaxScore = -999999999.f;
    int bestIndex=-1;


    for ( c=0; c < NUM_CHARS; c++)
    {                 
        scores[c] = 0;
        charData = (int*) m_CharLibData[c].data;
        if ( m_CharLibData[c].name[0] != '9' && m_CharLibData[c].name[0] != 'B') continue;


        for (x=0; x < 8; x++)
        {
            xo = x * h;

            for (y=20; y < 40; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
            
        }

    }


    for ( c=0; c < NUM_CHARS; c++)
    {
        if (  scores[c] > m_MaxScore )
        {
            m_MaxScore = scores[c];
            bestIndex = c;
        }  
    }


    return ( bestIndex  );

}



int groupBP ( int * in,int w, int  h )
{

    int c;
    int x,y,xo;
    int *charData;

    m_MaxScore = -999999999.f;
    int bestIndex=-1;


    for ( c=0; c < NUM_CHARS; c++)
    {                 
        scores[c] = 0;
        charData = (int*) m_CharLibData[c].data;
        if ( m_CharLibData[c].name[0] != 'B' && m_CharLibData[c].name[0] != 'P') continue;


        for (x=14; x < 20; x++)
        {
            xo = x * h;

            for (y=20; y < 40; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
            
        }

        for (x=10; x < 20; x++)
        {
            xo = x * h;

            for (y=32; y < 40; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
            
        }

    }


    for ( c=0; c < NUM_CHARS; c++)
    {
        if (  scores[c] > m_MaxScore )
        {
            m_MaxScore = scores[c];
            bestIndex = c;
        }  
    }


    return ( bestIndex  );

}




int groupDP ( int * in,int w, int  h )
{

    int c;
    int x,y,xo;
    int *charData;

    m_MaxScore = -999999999.f;
    int bestIndex=-1;


    for ( c=0; c < NUM_CHARS; c++)
    {                 
        scores[c] = 0;
        charData = (int*) m_CharLibData[c].data;
        if ( m_CharLibData[c].name[0] != 'D' && m_CharLibData[c].name[0] != 'P') continue;


        for (x=14; x < 20; x++)
        {
            xo = x * h;

            for (y=20; y < 40; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
            
        }

        for (x=10; x < 20; x++)
        {
            xo = x * h;

            for (y=32; y < 40; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
            
        }

        for (x=8; x < 12; x++)
        {
            xo = x * h;

            for (y=16; y < 25; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
            
        }
    }


    for ( c=0; c < NUM_CHARS; c++)
    {
        if (  scores[c] > m_MaxScore )
        {
            m_MaxScore = scores[c];
            bestIndex = c;
        }  
    }


    return ( bestIndex  );

}




int groupNY ( int * in,int w, int  h )
{

    int c;
    int x,y,xo;
    int *charData;

    m_MaxScore = -999999999.f;
    int bestIndex=-1;


    for ( c=0; c < NUM_CHARS; c++)
    {                 
        scores[c] = 0;
        charData = (int*) m_CharLibData[c].data;
        if ( m_CharLibData[c].name[0] != 'N' && m_CharLibData[c].name[0] != 'Y') continue;


        for (x=0; x < 8; x++)
        {
            xo = x * h;

            for (y=20; y < 40; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
        }

    }


    for ( c=0; c < NUM_CHARS; c++)
    {
        if (  scores[c] > m_MaxScore )
        {
            m_MaxScore = scores[c];
            bestIndex = c;
        }  
    }


    return ( bestIndex  );

}


int group4L ( int * in,int w, int  h )
{

    int c;
    int x,y,xo;
    int *charData;

    m_MaxScore = -999999999.f;
    int bestIndex=-1;


    for ( c=0; c < NUM_CHARS; c++)
    {                 
        scores[c] = 0;
        charData = (int*) m_CharLibData[c].data;
        if ( m_CharLibData[c].name[0] != 'L' && m_CharLibData[c].name[0] != '4') continue;


        for (x=0; x < 5; x++)
        {
            xo = x * h;

            for (y=0; y < 10; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
        }

        for (x=0; x < 9; x++)
        {
            xo = x * h;

            for (y=0; y < 4; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
        }
        
        for (x=0; x < 20; x++)
        {
            xo = x * h;

            for (y=36; y < 40; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
        }

    }


    for ( c=0; c < NUM_CHARS; c++)
    {
        if (  scores[c] > m_MaxScore )
        {
            m_MaxScore = scores[c];
            bestIndex = c;
        }  
    }


    return ( bestIndex  );

}


int group6B ( int * in,int w, int  h )
{

    int c;
    int x,y,xo;
    int *charData;

    m_MaxScore = -999999999.f;
    int bestIndex=-1;


    for ( c=0; c < NUM_CHARS; c++)
    {                 
        scores[c] = 0;
        charData = (int*) m_CharLibData[c].data;
        if ( m_CharLibData[c].name[0] != '6' && m_CharLibData[c].name[0] != 'B') continue;


        for (x=12; x < 20; x++)
        {
            xo = x * h;

            for (y=5; y < 20; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
        }

       

    }


    for ( c=0; c < NUM_CHARS; c++)
    {
        if (  scores[c] > m_MaxScore )
        {
            m_MaxScore = scores[c];
            bestIndex = c;
        }  
    }


    return ( bestIndex  );

}


int group9P ( int * in,int w, int  h )
{

    int c;
    int x,y,xo;
    int *charData;

    m_MaxScore = -999999999.f;
    int bestIndex=-1;


    for ( c=0; c < NUM_CHARS; c++)
    {                 
        scores[c] = 0;
        charData = (int*) m_CharLibData[c].data;
        if ( m_CharLibData[c].name[0] != '9' && m_CharLibData[c].name[0] != 'P') continue;


        for (x=0; x < 7; x++)
        {
            xo = x * h;

            for (y=25; y < 40; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
        }

       
        for (x=14; x < 20; x++)
        {
            xo = x * h;

            for (y=25; y < 40; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
        }


    }


    for ( c=0; c < NUM_CHARS; c++)
    {
        if (  scores[c] > m_MaxScore )
        {
            m_MaxScore = scores[c];
            bestIndex = c;
        }  
    }


    return ( bestIndex  );

}



int group6G ( int * in,int w, int  h )
{

    int c;
    int x,y,xo;
    int *charData;

    m_MaxScore = -999999999.f;
    int bestIndex=-1;


    for ( c=0; c < NUM_CHARS; c++)
    {                 
        scores[c] = 0;
        charData = (int*) m_CharLibData[c].data;
        if ( m_CharLibData[c].name[0] != '6' && m_CharLibData[c].name[0] != 'G') continue;


        for (x=4; x < 8; x++)
        {
            xo = x * h;

            for (y=16; y < 26; y++)
            {
                scores[c] += ( (float) *(in+xo+y)) * ((float)*(charData+xo+y));
            }
        }

    }


    for ( c=0; c < NUM_CHARS; c++)
    {
        if (  scores[c] > m_MaxScore )
        {
            m_MaxScore = scores[c];
            bestIndex = c;
        }  
    }


    return ( bestIndex  );

}




void createDiagImage  (int * src,  int c,int * diagImage)
{
    int x,xo,y;

    int * charData;

    if ( diagImage == 0 ) return;

    charData = (int*) m_CharLibData[c].data;

    for (x=0; x < STANDARD_WIDTH; x++)
    {
        xo = x * STANDARD_HEIGHT;

        for (y=0; y < STANDARD_HEIGHT; y++)
        { 
            *(diagImage+xo+y) = (*(src+xo+y)) * (*(charData+xo+y));
        }
    }
}


int lookUpByName ( int name )
{
    int c;

    for ( c=0; c < NUM_CHARS; c++)
    {
        if ( m_CharLibData[c].name[0] == name )
        {
            return ( c );
        }
    }

    return(0);
}


