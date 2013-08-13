

// this file contains methods for matching strings

#include <afx.h>
#include <stdlib.h>
#include <malloc.h>


// prototype

bool isWeakMatch ( char t1, char t2 );

int CountOrderedMatches  (char *longerString, char * shorterString, int longLen, int shortLen);


int LPROCR_lib_scoreMatch ( char * searchStr,  // string user enters to search for
                           int sStrLen,
                           char * refStr, // string in database to compare to
                           int rStrLen, int * error )
{

    char * longerString;
    char * longerStringCopy;
    char * shorterString;
    int longLen;
    int shortLen;
    int i;
    int j;
    int strongMatches = 0;
    int weakMatches = 0;
    int score = 0;
   
    int maxOrderedMatches =0;


    // which is longer ?

    if ( sStrLen >= rStrLen )
    {
        longerString = searchStr;
        longLen = sStrLen;

        shorterString = refStr;
        shortLen = rStrLen;
    }
    else
    {
        longerString = refStr;
        longLen = rStrLen;

        shorterString = searchStr;
        shortLen = sStrLen;
    }

    if ( longLen == 0 || shortLen == 0 )
    {
        return (0);
    }

    // copy the longer string to have a mark up copy

    longerStringCopy = (char *) malloc ( sizeof(char) * (longLen+1));
    if ( longerStringCopy == 0 ) 
    {
        *error = 1;
        return (0);
    }

    for ( j=0; j < longLen; j++)
    {
        longerStringCopy[j] = longerString[j];
    }
    longerStringCopy[j]=0;



    // count matches
    for (i=0; i < shortLen; i++)
    {
        for (j=0; j < longLen; j++)
        {
            if ( shorterString[i] == longerStringCopy[j] ) 
            {
                longerStringCopy[j] = 0; // dont count this one more than once
                strongMatches ++;

                break;// if match found, move to the next test char
            }

            // look for weak matches
            if ( isWeakMatch ( shorterString[i], longerStringCopy[j]))
            {
                longerStringCopy[j] = 0; // dont count this one more than once
                weakMatches ++;
                break;// if match found, move to the next test char
            }


        }// for (j
    }// for (i


    maxOrderedMatches  = CountOrderedMatches ( longerString, shorterString, longLen,  shortLen);
    
   


    if ( maxOrderedMatches == 0) maxOrderedMatches = 1;


    if (  maxOrderedMatches == longLen ) return(100);

	float matches  = (float)(strongMatches + weakMatches);
	if (matches == 0 ) matches = 10000;

    float orderedRatio = (float)maxOrderedMatches / (float)(strongMatches + weakMatches);


    float t = (((float)strongMatches + (float)weakMatches) / longLen) * orderedRatio;

    if ( t < .9 )
    {
        if ( orderedRatio == 1.0 && (strongMatches + weakMatches) >= 6 )
        {
            t = t + ( (1-t)/2  );
        }
        
    }

    if ( t == 1.0f && weakMatches > 0 ) t = .95f;

    score = (int) (t * 100);

    free ( longerStringCopy);

    return (score);
}


int CountOrderedMatches  (char *longerString, char * shorterString, int longLen, int shortLen)
{
    int s = 0;
    int l=0;
    int i=0;
    int orderedMatchCount = 0;
    int longOffset;
    int maxOrderedMatches = 0;
   

    l=0;
    for (s=0; s < shortLen && l < longLen; s++,l++)
    {
        if ( shorterString[s] == longerString[l] || isWeakMatch( shorterString[s], longerString[l]))
            orderedMatchCount++;
    }

    if ( orderedMatchCount >= shortLen) return ( orderedMatchCount);

    if ( orderedMatchCount > maxOrderedMatches ) maxOrderedMatches = orderedMatchCount;
   
 

    int interationCount = (longLen / 2)+1;

   
    for (i=0; i < interationCount; i++)
    {
        longOffset = i;
        l = longOffset;
        if ( longLen - longOffset <  maxOrderedMatches ) break;
        orderedMatchCount = 0;
        for (s=0; s < shortLen && l < longLen; s++,l++)
        {
            if ( shorterString[s] == longerString[l] || isWeakMatch( shorterString[s], longerString[l]))
                orderedMatchCount++;
        }
        if ( orderedMatchCount > maxOrderedMatches ) maxOrderedMatches = orderedMatchCount;
    }
   

    return(maxOrderedMatches);
}





bool isWeakMatch ( char t1, char t2 )
{

    bool isWeakMatch = false;

    // look for weak matches

    if ( t1 == 'B' && t2 == '8' )
    {  
        isWeakMatch = true;
        return (isWeakMatch);
    }


    if ( t1 == '8' && t2 == 'B' )
    {  
        isWeakMatch = true;
        return (isWeakMatch);
    }



    if ( t1 == '5' && t2 == 'S' )
    {  
        isWeakMatch = true;
        return (isWeakMatch);
    }


    if ( t1 == 'S' && t2 == '5' )
    {  
        isWeakMatch = true;
        return (isWeakMatch);
    }



    if ( t1 == 'D' && t2 == '0' )
    {  
        isWeakMatch = true;
        return (isWeakMatch);
    }


    if ( t1 == '0' && t2 == 'D' )
    {  
        isWeakMatch = true;
        return (isWeakMatch);
    }


    if ( t1 == '2' && t2 == 'Z' )
    {  
        isWeakMatch = true;
        return (isWeakMatch);
    }


    if ( t1 == 'Z' && t2 == '2' )
    {  
        isWeakMatch = true;
        return (isWeakMatch);
    }


    
    if ( t1 == 'Q' && t2 == '0' )
    {  
        isWeakMatch = true;
        return (isWeakMatch);
    }
        
    if ( t1 == '0' && t2 == 'Q' )
    {  
        isWeakMatch = true;
        return (isWeakMatch);
    }

    //if ( t1 == 'H' && t2 == 'K' )
    //{  
    //    isWeakMatch = true;
    //    return (isWeakMatch);
    //}


  /*  if ( t1 == 'K' && t2 == 'H' )
    {  
        isWeakMatch = true;
        return (isWeakMatch);
    }*/

    return (isWeakMatch);
}