
#ifndef INCLUDE_ONCE_LPROCR_Diags
#define INCLUDE_ONCE_LPROCR_Diags

struct DIAGCOLOR
{
char red;
char blue;
char green;
};


void  drawBox ( int fullSub, LPROCR_lib_PLATE_LOCATION * pLocation, DIAGCOLOR *c );
void  plotLine ( int fullSub, int xStart, int xEnd, int yStart, int yEnd, DIAGCOLOR *c );
void  LPROCR_diags_init ( int width, int height, int * error);
void  LPROCR_plate_diags_init (  int width, int height, int * diagImage, int * error);
void  LPROCR_plate_diags_putImage( int *plateImageSrc, int * diagImage, int width, int height);
void plate_diags_PlotSumCurve (  int * rowSum  );
void plate_diags_PlotLine ( int xStart, int yStart, int xEnd, int yEnd  );
void  LPROCR_plate_diags_drawBox (  LPROCR_lib_CHAR_LOCATION * cl, DIAGCOLOR *c );

void _stdcall LPROCR_diags_getDiagPlateImageSize( int *w, int *h) ;
int _stdcall LPROCR_diags_GetNumCandidatePlates( ) ;
void _stdcall LPROCR_diags_getPlateImage( int plateIndex, char * red, char * green, char * blue, int * error, int useCandidatePlateList) ;
void _stdcall LPROCR_lib_GetCandidatePlateImageSize ( int plateIndex, int * width, int * height );

void RejectLogAdd(char * string);
int _stdcall LPROCR_diags_GetRejectLogLength ( );
void _stdcall LPROCR_diags_GetRejectLog ( char * inStrMemory );
void ClearLogs( );
void  LPROCR_diags_putEdgeMapFullResImage( int xStart, int yStart, int xEnd, int yEnd);
void LPROCR_diags_plotHorizontalLineSums (int * lineSumCurve,int yBase, int count, int xBase, int xMax);
void _stdcall LPROCR_diags_getIntegralImage(  char * red, char * green, char * blue);
void RejectLogAddPlateIndexLocation(int plateIndex, int x, int y, char * string);
void LPROCR_diags_GetCharImageRGBArray(int plateIndex, int cIndex, byte * rgbValues, int rgbLength, int pixelOffset);
void LPROCR_diags_GetPlateDiagImageRGBArray(int plateIndex, int width, int height, unsigned char * rgbValues,  unsigned char * red, unsigned  char * green, unsigned  char * blue, int pixelOffset);
void LPROCR_diags_PutCharImageToRGBArray(char * imageSrc, int width, int height, unsigned char * rgbValues, int pixelOffset);

#endif