// S2255Interface.h : main header file for the S2255Interface DLL
//

#pragma once

#ifndef __AFXWIN_H__
	#error "include 'stdafx.h' before including this file for PCH"
#endif

#include "resource.h"		// main symbols
#include "stdafx.h"

#include "s2255f.h"

#include "s2255InterfaceAPI.h"

// CS2255InterfaceApp
// See S2255Interface.cpp for the implementation of this class
//


	int save_image_uncompressed(const unsigned char *image, TCHAR *szFilename, int height, int width, int stride,  int type);
	int save_image_uncompressed_mono(const unsigned char *image, TCHAR *szFilename, int height, int width);
	static DWORD WINAPI  AcquireThread( int * p);
    void StopAcquisition(int deviceIndex, int channel);
	void SetModeStruct ( MODE2255 * modeP, int standard, int compressionMode );




// Overrides
//	virtual BOOL InitInstance();

//	DECLARE_MESSAGE_MAP()


	  // 2255 structures and data
	struct DEVICE 
	{

	//HANDLE m_DeviceHandles[SYS_GRABBERS];  // device handles
//	int * m_CallersWinHandle;   // reference to the originators window handle, needed for the C++ caller
    BUFFER m_buf[MAX_CHANNELS];        // buffer structure per channel
    HANDLE m_hdev;                     // device handle
    HANDLE m_ack_thread[MAX_CHANNELS]; // acquisition thread
    HANDLE m_buf_event[MAX_CHANNELS];  // buffer full
    HANDLE m_stop_event[MAX_CHANNELS]; // stop event for channel
	BOOL   m_StoppingChannel[MAX_CHANNELS];
    BOOL   m_running[MAX_CHANNELS]; // if running
    unsigned char   *image[MAX_CHANNELS];
    int    m_recvd_buf[MAX_CHANNELS];
    MODE2255 m_mode[MAX_CHANNELS];
	int     m_prev_fdec[MAX_CHANNELS];	 /* previous fdec */
	int     m_fps_changed[MAX_CHANNELS]; /* fps changed */
    int     m_numbuf; // num user buffer used in driver
    int     cur_frame_rate[MAX_CHANNELS]; //to keep track of the calculated frame rate

    BOOL    m_bViewFR;
    BOOL    m_bViewImage;
    UINT32  m_framecount;

    // view positions
    RECT    m_posNTSC[MAX_CHANNELS];
    RECT    m_posPAL[MAX_CHANNELS];
    RECT    m_pos[MAX_CHANNELS];

    BOOL    m_bTriggerSnapshot[MAX_CHANNELS];
    CString m_strSnapshot[MAX_CHANNELS];

//	void(__stdcall  *BufferFilledCB[MAX_CHANNELS])(int jpegLength, LPBITMAPINFO binfo,  unsigned char   *image, int cindex , int frameIndex, int * callersWinHandle);
	
	void(__stdcall  *BufferFilledCB[MAX_CHANNELS])(int jpegLength, LPBITMAPINFO binfo,  unsigned char   *image, int cindex , int frameIndex);
	
	} m_Devices[ SYS_GRABBERS];


	struct ACQUIRE_THREAD_PARAMS
	{
		int deviceIndex;
		int channel;
	};

	

