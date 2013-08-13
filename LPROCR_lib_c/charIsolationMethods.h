// charIsolationMethods.h
#ifndef INCLUDE_ONCE_CHARISOLATIONMETHODS
#define INCLUDE_ONCE_CHARISOLATIONMETHODS


#define MIN_SLOPE -0.063f
#define MAX_SLOPE 0.063f



// prototypes
int LPROCR_lib_findCharsInPlate ( int plateIndex,  int * error, bool diagsEnabled );
void histogram ( int * inPtr, int w, int h, int * outPtr, int * ave, int * area,int * median);
void binarize( int * inPtr, int w, int h, int **binImage,  int * error);
void findEstimatedTopAndBottom(  int * inPtr , int * error);
int  LPROCR_lib_getMaxCandidateChars ( );
void LPROCR_lib_getCharLocation (int plateIndex,  int cIndex, int * left, int *right, int *top, int *bottom  , int useCandidatePlateList  );
void GetCharLocationCandidatePlates (int plateIndex,  int cIndex, int * left, int *right, int *top, int *bottom   );
void removeBolts(  int *binImage );
void removeDots(  int *binImage );
void blobLetters (  int *binImage );
void findTilt( );
void isolateChars (   );
void findCharTop ( int cc);
void findCharBottom ( int cc );
void drawBoxesAroundChars(  );
void removeBlackEdges (  int * image );
int calculateBlackFill (  int cc );
int calculateWhiteFill (  int cc );
void splitDoubleWides ( );
void removeBlackWhiteFilled ( );
void removeTooShortChars( );
void floodfill ( int startColor, int endColor, int xStart, int yStart, int * image,int recusion);
void removeTooNarrowChars( );
void removeTooTallChars( );
void trimTallChars ( );
void reviseSlope( );
void CopyImage (int * src , int * dst, int w, int h);
int groupEG ( int * in,int w, int  h );
void charFF ( int * markup, int startX, int startY, int recursion , LPROCR_lib_CHAR_LOCATION * candidateChar);
void FindCharsUsingFF ( int * error);
void sortChars ( );
void LPROCR_lib_RotateImage (int * image, int w, int h, float slope, int * error );
float checkForRollingShutter ( );
float testVerticalSlope (  );
void LPROCR_lib_unroll (int * image, int w, int h, float m, int * error, int fillValue );
float val ( int * image,int w, int h, int x, int y);
void trimEdges (LPROCR_lib_CHAR_LOCATION * candidateChar );
void removeTooOffsetChars ();
int RemoveBottomFrame(int possiblyRotatedPlate);
int LPROCR_lib_getAveCharWidth(int plateIndex, int useCandidateList );

#endif
