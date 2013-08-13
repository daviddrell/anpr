
#ifndef INCLUDE_ONCE_LPROCR_Structs
#define INCLUDE_ONCE_LPROCR_Structs


#define LIMIT(t,tLimit) if(t<0)t=0;if(t>=tLimit)t=tLimit-1;

#define MAX_CHANNELS 8

#define STANDARD_WIDTH 20
#define STANDARD_HEIGHT  40
#define MINIMUM_CHAR_HEIGHT  14
#define MINIMUM_CHAR_WIDTH   6
#define MAX_CHAR_WIDTH       40
#define MAX_CHAR_HEIGHT      70

#define MIN_PLATE_HEIGHT  25          //50, 25
#define MIN_PLATE_WIDTH  60         //120, 60

#define MAX_PLATE_HEIGHT  120      //120
#define MAX_PLATE_WIDTH  380        //420, 380, 320


#define X_DIVISOR 5  // was 5, 
#define Y_DIVISOR 5  // was 5, 

#define MAX_CANDIDATE_PLATES 5      // 6 max plates to be tested per frame
#define MAX_ACTUAL_PLATES 2        // 3 max final list of plates in one frame


#define MAX_CANDIDATE_CHARS 20
#define MAX_ACTUAL_CHARS 8

//#define MAX_AGGREGATE_STRINGS 20   

#ifdef DECLARE_ERROR_ARRAY
char *ERROR_CODE_STRINGS[] = {
    "NO_ERROR_SUCCESS",
    "ERROR_1",
    "ERROR_scaleHeightUp_ARRAY_TOO_TALL",
    "ERROR_scaleHeightUp_ARRAY_TOO_SHORT",
    "ERROR_scaleHeightDown_ARRAY_TOO_SHORT",
    "ERROR_scaleWidthDown_ARRAY_TOO_SHORT",
    "ERROR_scaleWidthUp_ARRAY_TOO_LONG",
    "ERROR_scaleWidthUp_ARRAY_TOO_SHORT",
    "ERROR_normalizeToNarrowSpectrum_BRIGHTEST_IS_ZERO",
    "ERROR_correlate3Group_ARRAY_SIZE_MISMATCH",
    "ERROR_correlateOGroup_ARRAY_SIZE_MISMATCH",
    "ERROR_correlate_ARRAY_SIZE_MISMATCH",
    "ERROR_rgbToY_ARRAY_SIZE_MISMATCH",
    "ERROR_addCharToLib_FILEIO_ERROR",
    "ERROR_getDisplayableSSChar_ARRAY_SIZE_MISMATCH",
    "ERROR_getSSCharSize_CHAR_DATA_NOT_INITIALIZED",
    "ERROR_scaleHeightDown_ARRAY_SIZE_MISMATCH",
    "ERROR_getDisplayableImage_DID_NOT_FIND_NAME",
    "ERROR_getDisplayableImage_ARRAY_NOT_STANDARD_SIZE",
    "ERROR_extractSubImage_ARRAY_SIZE_MISMATCH",
    "ERROR_run2DEdgeConvolution_ARRAY_SIZE_MISMATCH",
    "ERROR_extractContourLine_ARRAY_SIZE_MISMATCH",
    "ERROR_extractContourColunm_ARRAY_SIZE_MISMATCH",
    "ERROR_memoryNotAllocated"
};
#endif


struct PLATE_INFO_STRUCT
{
    int *image;
    int *diagImage;
    int width;
    int height;
    int leftEdge;
    int rightEdge;
    int bottomEdge;
    int topEdge;
    int aveCharWidth;
    char plateChars[MAX_CANDIDATE_CHARS];
    float charScores[MAX_CANDIDATE_CHARS];
    LPROCR_lib_CHAR_LOCATION charLocations[MAX_CANDIDATE_CHARS];
    int *diagCharStdSize[MAX_CANDIDATE_CHARS];
};

struct LPR_PROCESS_OPTIONS
{
    int EnableRotationRoll;
    int EnableAutoRotationRoll;
    float roll;
    float rotation;
    int quickPlateTest;
};

#define MAX_LOG_STRING_LENGTH 256
#define MAX_LOGS_PER_IMAGE 32

struct REJECT_LOG_STRUCT
{
	char logString[MAX_LOG_STRING_LENGTH];
};

//  struct libInstanceVariables
// for thread-safety for Microsoft platforms, C-Lib DLL must be static, OS does not provide
// seperate instance copies as it does for new managed code environments such as C#

struct libInstanceVariables
{
    // plate finding
    int *imageFullRes;    // luminance array of image
	 int *imageFullResDiag; // used for diag mark up
    int *imageSub; // subsampled image
	 int *imageSubDiag; // used for diag mark up
	 int *imageSubPrev; // previous subsampled image - used for motion detection
    int *edgeMapSub; // edgecontour map of subsampled image, plate finding
    int *edgeMapFullRes; // edgecontour map of full resolution image, used for exact plate location finding
	 int *edgeMapFullResDiag; // allows drawing plot lines overlaid onto the edge map

    double *integralTableFullRes; // used in plate edge finding

    int imageWidth;
    int imageHeight;
    int imageSubWidth;
    int imageSubHeight;
	 void (*MotionDetectedCallBack)() ;
    LPR_PROCESS_OPTIONS processOptions;

    // char isolation
    int *plateImage;   // luminance array of plate image
	 int *plateImageDiag ; // color version, gets malloced/cast as DIAG_COLOR_IMAGE
    int plateImageWidth;
    int plateImageHeight;
    int estCharTop;
    int estCharBottom;
    int * binarizedImageBlobed;
    int * binarizedImageClean;
    int InterFrameStringBuffIndex;
    int leftMostCorner;
    int rightMostCorner;
    int upperCorner;
    int lowerCorner;

    float plateSlopeT;
    float plateSlopeB;
  /*  float bT;
    float bB;*/

    int memoryAllocated;
    int diagEnabled;
	int diagnosticsInitialized ;

    bool USStyle;

    int numFoundPlates;

	int currentLogIndex; // used to index into logs[]
 

    LPROCR_lib_PLATE_LOCATION pLocationList [MAX_CANDIDATE_PLATES];

    LPROCR_lib_CHAR_LOCATION candidateChars[MAX_CANDIDATE_CHARS];

    LPROCR_lib_CHAR_LOCATION chars[MAX_ACTUAL_CHARS];

    PLATE_INFO_STRUCT foundPlates[ MAX_CANDIDATE_PLATES ];

    PLATE_INFO_STRUCT finalPlateList[MAX_ACTUAL_PLATES];
	
	REJECT_LOG_STRUCT logs[MAX_LOGS_PER_IMAGE]; // used to record string messages of why a plate was rejected
  
};


void freeMemory(int **p);
void freeMemoryDouble(double **p);

#endif