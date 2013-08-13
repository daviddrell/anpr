//lprMethods.h
#ifndef LPRMETHODS_ONCE
#define LPRMETHODS_ONCE


libInstanceVariables libData;




// for debug
//
//int LPROCR_lib_ReadThisImage_P1 (  int * img, int width, int height, int diagEnabled, int interlaced, int *error);
//int LPROCR_lib_ReadThisImage_P2 (  );
//int LPROCR_lib_ReadThisImage_P3 (  );


// prototypes

void _stdcall subscale ( int * array2dLarge, int * array2dSmall, 
                        int largeWidth,
                        int largeHeight,
                        int Xdivisor, 
                        int Ydivisor );
void normalizeToWideSpectrum(int * a, int w, int h, int * error);
void createEdgeContourMap(int *image, int xStart, int yStart, int xEnd, int yEnd, 
                          int *edgeMap, 
                          int w,   // image width 
                          int h,   // image height
                          int secondOrder, // should add in adjacent egde energy?
                          int * error);
int runVerticalEdgeFinderConvolution(int xStart, int yStart, int blockW, int blockH, 
                                     int imageWidth,
                                     int imageHeight,
                                     int *image);
void findPlateEdges( LPROCR_lib_PLATE_LOCATION *pLocationList,  int * error);
void findBrightestSpot(int index, int * image, int w, int h, int * XatMax, int * YatMax);
void findExactEdges ( LPROCR_lib_PLATE_LOCATION *pl, int * error);
void   initMemory (int initValue, int * image, int w, int h);

void linkCols ( int *colSum, int  maxPlateWidth, int * threshold );
void setEdgesToMinus1 (LPROCR_lib_PLATE_LOCATION *pl);
int LPROCR_lib_getMaxCandidatePlates ( );
int LPROCR_lib_getMaxActualPlates ( );
char * ReadThisImage (  int * img, int width, int height, int diagEnabled);
void _stdcall LPROCR_lib_init( int * img, int width, int height, int diagEnabled, int *error);
void   initMemory (int initValue, int * image, int w, int h);
void _stdcall LPROCR_lib_dispose(int instIndex);
void _stdcall LPROCR_lib_getImage( int * Y, // place to put the image
                                  int * error) ;
void _stdcall LPROCR_lib_getEdgeMapFullRes( int * Y, // place to put the image
                                  int * error) ;
void _stdcall LPROCR_lib_getEdgeMapSub( int * Y, // place to put the image
                                  int * error) ;
void LPROCR_lib_getPlateLocation( int plateIndex, LPROCR_lib_PLATE_LOCATION *pLocation);
int _stdcall LPROCR_lib_findThePlates  (  int * error);
void _stdcall subscale (  int * array2dLarge, int * array2dSmall, 
                        int largeWidth,
                        int largeHeight,
                        int Xdivisor, 
                        int Ydivisor );


void setEdgesToMinus1 (LPROCR_lib_PLATE_LOCATION *pl);


void   sumCols ( int blockWidth, 
                  int leftEdge, int rightEdge,
                  int topEdge, int bottomEdge, int * colSum, int * peakVal );


void createEdgeContourMap( int *image, int xStart, int yStart, int xEnd, int yEnd, 
                          int *edgeMap, 
                          int w,   // image width 
                          int h,   // image height
                          int secondOrder, // should add in adjacent egde energy?
                          int * error);
int runVerticalEdgeFinderConvolution(int xStart, int yStart, int blockW, int blockH, 
                                     int w,
                                     int h,
                                     int *image);
//void findBrightestSpot(int * image, int w, int h, int * XatMax, int * YatMax);
void LPROCR_lib_sharpen ( int * image, int w, int h );
char * getErrorString(int error);
void LPROCR_lib_extractFromBmpDataToLumArray( int * srcPtr, int * dstPtr, 
                                             int stride, int width, int height, int invert);
int LPROCR_lib_getMaxCandidatePlates ( );
int LPROCR_lib_getMaxActualPlates ( );
void extractSubImage(int* largeImage, int w, int h, int leftEdge, int rightEdge, int topEdge,int bottomEdge, int * subImage);
int LPROCR_lib_ReadThisImage (  int * img, int width, int height, int diagEnabled);
int _stdcall LPROCR_lib_GetNumFoundPlates ( );
void _stdcall LPROCR_lib_GetPlateImage ( int plateIndex, int * img, int useCandidatePlateList );
void _stdcall LPROCR_lib_GetPlateImageSize ( int plateIndex, int * width, int * height, int useCandidatePlateList );
void _stdcall LPROCR_lib_GetPlateString  (int plateIndex, char * inStrMemory, float * score );
void fineTuneLeftRightEdges ( int * leftEdge, int *rightEdge,int *topEdge, int *bottomEdge);
float correlateFlat ( int * image, int w, int h, int xCenter, int yCenter );
void CopyString (char * src, char * dst, int count);
void LPROCR_lib_GetPlateImageRGBArray(int plateIndex, byte * rgbValues, int width, int height, int rgbLength, int pixelOffset);
void LPROCR_lib_GetCharImageRGBArray(int plateIndex, int cIndex, byte * rgbValues, int width, int height, int rgbLength, int pixelOffset);
void _stdcall LPROCR_lib_GetSubImageSize (  int * width, int * height );
int LPROCR_lib_GetConfig (char * s1, char * s2 );
void CopyCLocations ( LPROCR_lib_CHAR_LOCATION *src,  LPROCR_lib_CHAR_LOCATION *dest );
void CopyScores ( float *src, float *dst, int count);
int quickPlateTest ( LPROCR_lib_PLATE_LOCATION *pl);
void _stdcall LPROCR_lib_RegisterMotionDetectionCB(  void (*callback)());
int  _stdcall LPROCR_lib_DetectMotion ( int chan, int * fullscale, int w, int h, int *error);
void  _stdcall LPROCR_lib_MemCopyInt(int * src, int * dst, int len );
void  _stdcall LPROCR_lib_MemCopyByte(char * src, char * dst, int len, int * detectedNoVideo );
void  _stdcall LPROCR_lib_MemCopyBytesToInts(char * src, int * dst, int len,int width, int height );
void SubScaleImageLocal(int * fullscale, int w, int h, int ** subscaled, int *sw, int *sh, int *error);
void LPROCR_lib_GetMinMaxPlateSize( int * minWidth, int * maxWidth,  int * minHeight, int * maxHeight);
void CreateIntegralImage(int xStart, int xEnd, int yStart, int yEnd);
void   initMemoryDouble (double initValue, double * image, int w, int h);
int FindHoriztonalCenterofMass (/* define the window we are operating in: */ int xStart, int xEnd, int yStart, int yEnd );
void LPROCR_lib_GetCandidatePlateImageRGBArray(int plateIndex, byte * rgbValues, int width, int height, int rgbLength, int pixelOffset);

void LPROCR_lib_LoadImage (  int * img, int width, int height, int diagEnabled, LPR_PROCESS_OPTIONS * processOptions,  int *error);
void LPROCR_lib_GetCharImageLuminance(int plateIndex, int cIndex, int * lumArray, int useCandidateList);
int LPROCR_lib_ExtractFoundPlates ( int * error );

#endif // ONCE