/**********************************************************
 * This file contains special types, constants
 * for use with Sensoray sx2255 driver
 *
 * Copyright (C) Sensoray 2007-2008
 *
 * Version 1.0.9D  -mode.single removed at user level.  Do not use mode.single
 *                 -S2255_GetFrame obsoleted.  Use S2255_GrabFrame for single
 *                  frame capture (not for performance capturing)
 *                 error codes from DLL functions clarified.  0 is still success
 *                 
 **********************************************************/


#ifndef SX2255_H
#define SX2255_H


#define S2255_SUCCESS           0L
#define S2255_WAIT_TIMEOUT      258L
#define S2255_FAIL_GENERAL		-1L
#define S2255_INVALID_CHANNEL   -2L
#define S2255_ALREADY_ACQUIRING -3L
#define S2255_TOO_SMALL         -4L /*buffer too small to get frame */
#define S2255_FAIL_REGEVENT     -5L /*driver access problem reg event*/
#define S2255_INVALID_FRAME		-6L
#define S2255_NOT_ACQUIRING		-7L
#define S2255_FAIL_STOP			-8L
#define S2255_INVALID_BOARD     -9L
#define S2255_TOO_MANY_BOARDS   -10L
#define S2255_INVALID_ACCESS    -12L
#define S2255_INVALID_DATA      -13L
#define S2255_OUTOFMEMORY		-14L


#define MAX_CHANNELS 4

#ifndef _BASETSD_H_
typedef	unsigned char		UINT8;
typedef	char				SINT8;
typedef	unsigned short		UINT16;
typedef	short				SINT16;
typedef void				*PVOID;
typedef unsigned long		ULONG;
typedef unsigned char		UCHAR;
typedef	int		            INT32;
typedef void *              HANDLE;
#endif

#if ( defined(WIN32) || defined(OSTYPE_WINDOWS) )

#ifndef _BASETSD_H_
typedef	INT32				SINT32;
typedef	__int64				SINT64;
typedef	unsigned int		UINT32;
#endif

typedef HANDLE              HDEVICE;
#elif ( defined(_LINUX) || defined( OSTYPE_LINUX) )
// Linux, no bitmapinfo
typedef long                LONG;
typedef	unsigned int		UINT32;
typedef	int					SINT32;
typedef void				VOID;
typedef void *				LPVOID;
typedef int					BOOLEAN;
typedef int					BOOL;
typedef unsigned int		DWORD;
typedef unsigned int		NTSTATUS;
typedef int                 HDEVICE;
typedef char                BYTE;
typedef int                 WORD;

typedef struct tagRGBQUAD {
        BYTE    rgbBlue;
        BYTE    rgbGreen;
        BYTE    rgbRed;
        BYTE    rgbReserved;
} RGBQUAD;

typedef struct tagBITMAPINFOHEADER{
        DWORD      biSize;
        LONG       biWidth;
        LONG       biHeight;
        WORD       biPlanes;
        WORD       biBitCount;
        DWORD      biCompression;
        DWORD      biSizeImage;
        LONG       biXPelsPerMeter;
        LONG       biYPelsPerMeter;
        DWORD      biClrUsed;
        DWORD      biClrImportant;
} BITMAPINFOHEADER, *LPBITMAPINFOHEADER, *PBITMAPINFOHEADER;

typedef struct tagBITMAPINFO {
    BITMAPINFOHEADER    bmiHeader;
    RGBQUAD             bmiColors[1];
} BITMAPINFO;

typedef BITMAPINFO * LPBITMAPINFO;
#else
#error "Must define an OS"
#endif

#define SYS_GRABBERS            8       //max frame grabbers in the system;
#define SYS_FRAMES              64		//max frames alloed;
   
typedef struct {                        // frame structure;
    void *pdata;						// pointer to image data;
} FRAME;
   
typedef struct {                        //image buffer structure;
    ULONG dwFrames;                     //number of frames in buffer;
    FRAME frame[SYS_FRAMES];            //array of FRAME structures;
    LPBITMAPINFO lpbmi[SYS_FRAMES];     //pointer to BITMAPINFO structure;
} BUFFER;
 
typedef struct {
 UINT32   format;   //input video format (NTSC, PAL, SECAM)
 UINT32   scale;   //output video scale
 UINT32   color;   //output video color format
 UINT32   fdec;   //frame decimation
 UINT32   bright;   //brightness
 UINT32   contrast;  //contrast
 UINT32   saturation;  //saturation
 UINT32   hue;   //hue (NTSC only)
 UINT32   single;   // Future USE.  Leave as default (0)
} MODE2255;

 
//predefined settings
#define FORMAT_NTSC    1
#define FORMAT_PAL     2
 
#define SCALE_4CIFS    1 //640x480(NTSC) or 704x576(PAL)
#define SCALE_2CIFS    2 //640x240(NTSC) or 704x288(PAL)
#define SCALE_1CIFS    3 //320x240(NTSC) or 352x288(PAL)
#define SCALE_4CIFSI   4 //640x480(NTSC) or 704x576(PAL) interpolated

#define COLOR_YUVPL    1 //YUV planar
#define COLOR_YUY2     2
#define COLOR_YUVPK    2 //YUV packed (YUY2 format 16 bits)
#define COLOR_RGB      3 //RGB (packed, GBR, Windows .bmp compatible 24 bit RGB)
#define COLOR_Y8       4 //monochrome
#define COLOR_JPG      5
#define COLOR_UYVY     32 // UYVY packed 
#define MASK_COLOR		0xff
#define MASK_JPG_QUALITY	0xff00

#define FDEC_1     1     //capture every frame
#define FDEC_2     2     //capture every 2nd frame
#define FDEC_3     3     //capture every 3rd frame
#define FDEC_5     5     //capture every 5th frame

/*-------------------------------------------------------
 * Default mode parameters.
 *-------------------------------------------------------*/
#define DEF_SCALE               SCALE_4CIFS       //default scaling mode, full image;
#define DEF_COLOR               (COLOR_RGB | (50 << 8))     //default color mode, RGB output, default JPG quality 50;
#define DEF_FDEC                FDEC_1
#define DEF_BRIGHT              0
#define DEF_CONTRAST            0x5c
#define DEF_SATURATION          0x80
#define DEF_HUE                 0

#define DEF_MODE_NTSC_CONT  FORMAT_NTSC, DEF_SCALE, DEF_COLOR, DEF_FDEC, DEF_BRIGHT, DEF_CONTRAST, DEF_SATURATION, DEF_HUE, 0
#define DEF_MODE_PAL_CONT   FORMAT_PAL, DEF_SCALE,  DEF_COLOR, DEF_FDEC, DEF_BRIGHT, DEF_CONTRAST, DEF_SATURATION, DEF_HUE, 0
#define DEF_MODE_NTSC_SING  FORMAT_NTSC, DEF_SCALE, DEF_COLOR, DEF_FDEC, DEF_BRIGHT, DEF_CONTRAST, DEF_SATURATION, DEF_HUE, 1
#define DEF_MODE_PAL_SING   FORMAT_PAL, DEF_SCALE,  DEF_COLOR, DEF_FDEC, DEF_BRIGHT, DEF_CONTRAST, DEF_SATURATION, DEF_HUE, 1

/*-------------------------------------------------------
 * Error codes
 *-------------------------------------------------------*/
#define SUCCESS                 0
#define ERR_UNKNOWN             -1

#endif
