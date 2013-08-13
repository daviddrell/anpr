
#include <afx.h>
#include <stdlib.h>
#include <malloc.h>
#include "LPROCR_Public.h"
#include "LPROCR_Structs.h"
#include "plateGroupings.h"

/*

Algorithm pseudo code:

Adding a plate to a group means adding the extracted string and a  frame serial number. The frame seial number
allows the upper layer management code to associate the reading with a particular jpeg image (file).

Each channel will have its own group manager such that readings on different camera sources are not mixed.

The number of groups will be MAX_PLATES_PER_FRAME.
The number of alternate readings in a group will be MAX_ALTERNATE_READINGS

Groups will contain two languages. Each string will be stored under language 0 or 1.
The upper layer will associate languages with the language indexes (0 or 1).

Before processing any frames, open a new group (empty). 
Read a frame. 

	if this frame contains a plate
	{

		If this is the first plate since the last empty frame
		{ 
			add it to the current group.
		}
		Else
		{	
			compare and score this new string to all strings in the current group
			If match score == 100 to a plate in the group
			{
				Drop it, it’s a duplicate
			}
			If match score > 50 && < 100
			{
				Add it as an alternate reading to the group
			}
			Else // it score is 0 to 50
			{
				for each other group that exists
				{
					Get string compare to plates in the group
					If match score == 100 to a plate in the group
					{
						Drop it, it’s a duplicate
					}
				}
				if it did not match any existing group
				{
					create a new group and add it
				}
			}
		}

	}
	else
	{
		// this frame contained no plates
		expire all groups and deliver them to the consumer of plate readings
	}

	definitions:
	1. Plate Group - a set of numbers which are similar indicating these are varient/alternate readings of the same plate from different frames. Since
	one frame can have multiple plates, there can be multiple groups per frame. the number of viable current groups is MAX_PLATES_PER_FRAME.

	2. Plate Group Circular Buffer - a buffer to hold group results from previous frames while the system is processing new frames, so that
	old results are not overridden before the results can be reported up the chain. 

	3. Plate Group control - internal controls for a single group


*/

GROUP_BY_CHANNEL gcbc[MAX_CHANNELS];

int GetNewGroupIndex (int chan)
{
	int index = 0;

	index = gcbc[chan].gc.CurrentNewGroup;
	
	gcbc[chan].gc.CurrentNewGroup ++;

	if (gcbc[chan].gc.CurrentNewGroup == MAX_ACTUAL_PLATES) gcbc[chan].gc.CurrentNewGroup = 0;

	return(index);
}

int GetNextAltReadingIndex (int chan, int groupIndex)
{
	int index = gcbc[chan].gc.Groups[groupIndex].AltReadingIndex;
	gcbc[chan].gc.Groups[groupIndex].AltReadingIndex++;
	if (gcbc[chan].gc.Groups[groupIndex].AltReadingIndex == MAX_ALTERNATE_READINGS) gcbc[chan].gc.Groups[groupIndex].AltReadingIndex= 0;
	return(index);
}

void AddToGroup (int chan, int gIndex, char * currentReading,int charCount,int serialNumber)
{
	int altReadingIndex;

	altReadingIndex = GetNextAltReadingIndex (chan, gIndex);

    CopyString( currentReading , gcbc[chan].gc.Groups[	gIndex ].AlternateReading[ altReadingIndex ].str , charCount);
	
	gcbc[chan].gc.Groups[	gIndex ].AlternateReading[ altReadingIndex ].count = charCount;

	gcbc[chan].gc.Groups[	gIndex ].SerialNum = serialNumber;
	gcbc[chan].gc.Groups[	gIndex ].hasAString = true;

}

void AddToNewGroup (int chan, char * currentReading, int charCount, int serialNumber )
{
	int newGroupIndex = 0;
	int altReadingIndex = 0;

	newGroupIndex = GetNewGroupIndex(chan);
	altReadingIndex = GetNextAltReadingIndex (chan, newGroupIndex);

	CopyString( currentReading , gcbc[chan].gc.Groups[	newGroupIndex ].AlternateReading[ altReadingIndex ].str , charCount);
	
	gcbc[chan].gc.Groups[	newGroupIndex ].AlternateReading[ altReadingIndex ].count = charCount;

	gcbc[chan].gc.Groups[	newGroupIndex ].SerialNum = serialNumber;
	gcbc[chan].gc.Groups[	newGroupIndex ].hasAString = true;
}

void Groups_Init()
{
	int gindex;
	int chan;
	

	PlateGroupCallBackRegistered = false;

	for (chan = 0; chan < MAX_CHANNELS; chan++)
	{
		gcbc[chan].lastFrameWasEmpty = true;

		gcbc[chan].gc.CurrentNewGroup  = 0;

		for (gindex = 0; gindex <MAX_ACTUAL_PLATES ; gindex++) //for each other group that exists
		{
			ClearGroup (& gcbc[chan].gc.Groups[gindex] );
		}
	}
}

void _stdcall LPROCR_lib_RegisterPlateGroupCB(  void (*callback)(int chan, char *contatonatedPlateStrings, int serialNumber, int len))
{
	PlateGroupCallBack = callback;
	PlateGroupCallBackRegistered = true;

}

void ExpireAllOpenGroups(int chan)
{
	int gindex;
	int i=0;
	int currentCatStrLen = 0;
	char * concatString [  MAX_ALTERNATE_READINGS * MAX_CANDIDATE_CHARS ];
	int concatMaxLen = MAX_ALTERNATE_READINGS * MAX_CANDIDATE_CHARS;

	gcbc[chan].gc.CurrentNewGroup  = 0;
	for (gindex = 0; gindex <MAX_ACTUAL_PLATES ; gindex++) //for each other group that exists
	{
		for (i= 0; i <  MAX_ALTERNATE_READINGS; i++)
		{
			currentCatStrLen = ConCat ( (char*) concatString, concatMaxLen, currentCatStrLen, gcbc[chan].gc.Groups[gindex].AlternateReading[i].str, gcbc[chan].gc.Groups[gindex].AlternateReading[i].count);
		}
		if ( gcbc[chan].gc.Groups[	gindex ].hasAString == true)
		{                                                                      /// use the serial number from the first sighting
			if (PlateGroupCallBackRegistered )PlateGroupCallBack (chan, (char*)concatString,  gcbc[chan].gc.Groups[gindex].SerialNum, currentCatStrLen );
		}
		ClearGroup (& gcbc[chan].gc.Groups[gindex] );
	}
}

int ConCat ( char * outstring, int outMaxLen, int offset, char * inStr, int inStrLen)
{
	int i = 0;
	int j = 0;

	if ( inStrLen == 0 ) return 0;

	if ( offset >= outMaxLen ) return 0;
	

	for(i=offset; i < outMaxLen; )
	{
		if (  inStr[j] != 0 )
		{
			outstring[i] = inStr[j];
			i++;
		}
		j++;
	
		if ( j == inStrLen )
		{
			if ( i < outMaxLen)
				outstring[i++] = ','; // string break delimiter
			break;
		}
	}

	if ( i < outMaxLen )
		outstring[i] = 0; // null term
	else
		outstring[i-1] = 0; //unlikely case, but over write the last char with the null term
		
	return(i);
}

extern int LPROCR_lib_scoreMatch ( char * searchStr, int sStrLen, char * refStr, int rStrLen, int * error );// defined in stringMethods.c of this library

int CompareToGroupAllStrings (int chan, int gIndex, char * currentReading, int charCount)
{
	int i;
	int score=0;
	int bestScore=0;
	int error = 0;

	for (i = 0; i < MAX_ALTERNATE_READINGS; i++)
	{
		score = LPROCR_lib_scoreMatch ( currentReading, 
										charCount,
										gcbc[chan].gc.Groups[gIndex].AlternateReading[i].str, 
										gcbc[chan].gc.Groups[gIndex].AlternateReading[i].count, 
										&error );

		if ( score > bestScore)
			bestScore = score;
	}
	return(bestScore);
}

void ClearGroup (GROUP * group )
{	
	int i;
	group->hasAString = false;
   group->SerialNum  = 0;

	for (i = 0; i < MAX_ALTERNATE_READINGS; i++)
	{
		ClearString ( group->AlternateReading[i].str, MAX_CANDIDATE_CHARS);
      
		group->AlternateReading[i].count = 0;
	}
}

void ClearString(char * s, int count)
{
	int i = 0;
	for ( i = 0; i < count; i++)
		s[i] = 0;
}

extern void CopyString(char * s, char * d, int count); // defined in lprMethods.c in this same lib
//void CopyString(char * s, char * d, int count)
//{
//	int i = 0;
//	for ( i = 0; i < count; i++)
//		d[i] = s[i];
//}



void Groups_ProcessNewImage(int chan, char * currentReading, int charCount, int serialNumber)
{
	int bestScore=0 ;
	int bestGroupMatch = 0;
	int gindex = 0;
	int score=0;

	if ( chan < 0 || chan > MAX_CHANNELS) return;

	if ( charCount == 0 ) // no plate was found in this frame
	{
	  ExpireAllOpenGroups(chan);
	  gcbc[chan].lastFrameWasEmpty = true;
	  return;
	}

	// a plate was found in this frame

	if ( gcbc[chan].lastFrameWasEmpty ) //If this is the first plate since the last empty frame
	{ 
		AddToNewGroup ( chan, currentReading, charCount, serialNumber);
	}
	else
	{	
		for (gindex = 0; gindex <MAX_ACTUAL_PLATES; gindex++) //for each other group that exists
		{
			// Get string compare to plates in the group
			score = CompareToGroupAllStrings (chan, gindex, currentReading, charCount);
			if ( score > bestScore)
			{
				bestScore = score;
				bestGroupMatch = gindex;
			}


		}
		if( bestScore == 100 ) //If match score == 100 to a plate in the group
		{
			//Drop it, it’s a duplicate
		}
		else if ( bestScore > 50 ) // less than 100 but greater than 50
		{
			// add it to the best group
			AddToGroup (chan, bestGroupMatch, currentReading, charCount, serialNumber);
		}
		else // if it did not match any existing group
		{
			AddToNewGroup (chan,  currentReading, charCount, serialNumber);
		}
	}
	
	
	gcbc[chan].lastFrameWasEmpty = false;
}