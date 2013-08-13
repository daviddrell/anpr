// ocr.h
#ifndef INCLUDE_ONCE_OCR
#define INCLUDE_ONCE_OCR




// prototypes
int OCR_lib_TestRotationBestFit ( int * cImageRaw, int w, int h, int * error  );
int OCR_lib_GetNumberRotations ( );
void scoreMatchOnRotationTestChar( int * in, int w, int h , float * score );
char LPROCR_lib_ReadThisChar ( int * cImageRaw, int w, int h, float * score, int aveCharWidth, int * displayChar, int * error   );
void prepChar ( int * cImageRaw, int w, int h, int * cImageSS, int * error);
void normalizeToWideSpectrum(int* luminance8bit, int w, int h, int* luminance32bit);
void normalizeToWideSpectrum2(int* luminance8bit, int w, int h, int * luminance32bit );
void scaleArray(int* inArray, int w, int h, int* outArray, int newWidth, int newHeight, int * error);
void padWidthUp(int* inArray, int w, int h, int* outArray, int newWidth);
void scaleWidthUp(int* inArray,int w,int h, int* outArray, int newWidth, int newHeight);
void scaleWidthDown(int* inArray, int w, int h, int* outArray, int newWidth );
void scaleHeightDown(int* inArray,  int h, int* outArray,int newWidth, int newHeight);
void scaleHeightUp(int*inArray,  int h, int* outArray,int newWidth, int newHeight);
void copyChar (int *in, int * out, int w, int h);
int findBestMatch ( int * in, int w, int h , int * diagChar, float * score );
//void initMasks ( );
void autoCorrelateMasks ( );
void blurrChar (int *in,  int w, int h);
void invertMasks ( );
void erodeMasks ( );
int secondPassCheck ( int * in,int w, int  h, int bestIndex, int recursion, int * diagChar, float * score );
int isOorD(int * Yx, int w, int h);
int lookUpByName ( int name );
int secondPassCorrelation (int * src, int index, int filter);
float correlateWithFilter (int * src, int charIndex, int filter);
bool matchAnyThisFilter (char name, int filter );
int structualAnalysis ( int * in,int w, int  h,int * diagImage );
float structualCorrelation (int * src,  int filter,int * diagImage);
//int LPROCR_lib_OCR_getFilterName ( int filter );
//int LPROCR_lib_OCR_getNumberFilters();
void erode(int*image, int w, int h, int * eroded );
void createDiagImage  (int * src,  int filter,int * diagImage);
void  binarizeChar (int* image, int w, int h);
void DrawDiagCharBestCharAgainstInput( int * in,int w, int  h , int  bestIndex );

int groupHN ( int * in,int w, int  h );
int group1X ( int * in,int w, int  h );
int group2Z ( int * in,int w, int  h );
int groupQ0D ( int * in,int w, int  h );
int group7T ( int * in,int w, int  h );
int groupGC ( int * in,int w, int  h );
void createDiagImage  (int * src,  int filter,int * diagImage);
int groupJ3 ( int * in,int w, int  h );
int groupC0 ( int * in,int w, int  h );
int groupD0 ( int * in,int w, int  h );
bool isFilledCornerUpperLeft ( int * in,int w, int  h );
bool isFilledCornerLowerLeft ( int * in,int w, int  h );
int groupB8 ( int * in,int w, int  h );
int groupBH ( int * in,int w, int  h );
int groupG0 ( int * in,int w, int  h );
int groupPF ( int * in, int w, int  h );
int groupD0U( int * in, int w, int  h );
int groupSB ( int * in, int w, int  h );
int groupP2 ( int * in, int w, int  h );
int group56 ( int * in, int w, int  h );
int group2E ( int * in, int w, int  h );
int groupPH ( int * in, int w, int  h );
int groupB6 ( int * in,int w, int  h );
int groupUL ( int * in,int w, int  h );
//int group389B ( int * in,int w, int  h );
int group38B ( int * in,int w, int  h );
int group39 ( int * in,int w, int  h );
int group28 ( int * in,int w, int  h );
int group68 ( int * in,int w, int  h );
int group7Z ( int * in,int w, int  h );
int group5S ( int * in,int w, int  h );
float isFilledCornerLowerRight ( int * in,int w, int  h );
int group14 ( int * in, int w, int  h );
int group0S ( int * in,int w, int  h );
int groupMNW (int * in,int w, int  h );
int groupP8 (int * in,int w, int  h );
int groupRK (int * in,int w, int  h );
int groupMK ( int * in, int w, int  h );
int group7P ( int * in, int w, int  h );
int groupEF ( int * in, int w, int  h );
int group1T ( int * in,int w, int  h );
int groupEL ( int * in,int w, int  h );
int groupHM ( int * in,int w, int  h );
int groupHW ( int * in, int w, int  h );
int group8P ( int * in,int w, int  h );
int groupHU ( int * in,int w, int  h );
int group89 ( int * in,int w, int  h );
int group5D ( int * in,int w, int  h );
int group7A ( int * in,int w, int  h );
int group4Z ( int * in,int w, int  h );
int group4A ( int * in,int w, int  h );
int group37 ( int * in,int w, int  h );
int group9R ( int * in,int w, int  h );
int group9B ( int * in,int w, int  h );
int groupBP ( int * in,int w, int  h );
int groupDP ( int * in,int w, int  h );
int groupNY ( int * in,int w, int  h );
int group4L ( int * in,int w, int  h );
int group6B ( int * in,int w, int  h );
int group9P ( int * in,int w, int  h );
int group6G ( int * in,int w, int  h );

#include "OCR_charLib.h"



#endif