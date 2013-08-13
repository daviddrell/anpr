// S2255Interface.cpp : Defines the initialization routines for the DLL.
//

#include "stdafx.h"
#include "S2255Interface.h"


#ifdef _DEBUG
#define new DEBUG_NEW
#endif

//
//TODO: If this DLL is dynamically linked against the MFC DLLs,
//		any functions exported from this DLL which call into
//		MFC must have the AFX_MANAGE_STATE macro added at the
//		very beginning of the function.
//
//		For example:
//
//		extern "C" BOOL PASCAL EXPORT ExportedFunction()
//		{
//			AFX_MANAGE_STATE(AfxGetStaticModuleState());
//			// normal function body here
//		}
//
//		It is very important that this macro appear in each
//		function, prior to any calls into MFC.  This means that
//		it must appear as the first statement within the 
//		function, even before any object variable declarations
//		as their constructors may generate calls into the MFC
//		DLL.
//
//		Please see MFC Technical Notes 33 and 58 for additional
//		details.
//



__declspec(dllexport) int __stdcall Open2255Device(int deviceIndex, int standard)
{
	if ( deviceIndex < 0  || deviceIndex >= SYS_GRABBERS) return(-1);

	int i;
	
	if ( standard == 0 ) MODE2255 mode_sing = { DEF_MODE_NTSC_SING };
	else MODE2255 mode_sing = { DEF_MODE_PAL_SING };

	m_Devices[deviceIndex].m_hdev = 0;

//	m_Devices[deviceIndex].m_numbuf = SYS_FRAMES;

   // m_Devices[deviceIndex].m_bViewFR
//m_Devices[deviceIndex].m_bViewImage
  //  m_Devices[deviceIndex].m_fps_changed
//m_Devices[deviceIndex].m_framecount
//m_Devices[deviceIndex].m_mode
//m_Devices[deviceIndex].m_numbuf
//m_Devices[deviceIndex].m_pos
//m_Devices[deviceIndex].m_posNTSC
//m_Devices[deviceIndex].m_posPAL
//m_Devices[deviceIndex].m_prev_fdec

	m_Devices[deviceIndex].m_numbuf = 8;  // use only four frames to keep the latency low

	for (i = 0; i < MAX_CHANNELS; i++) {
		m_Devices[deviceIndex].m_StoppingChannel[i] = false;
		m_Devices[deviceIndex].m_stop_event[i] = NULL;
		m_Devices[deviceIndex].m_ack_thread[i] = NULL;
		m_Devices[deviceIndex].m_buf_event[i] = NULL;
		m_Devices[deviceIndex].m_stop_event[i] = NULL; // stop event for channel
		m_Devices[deviceIndex].m_running[i] = FALSE;
		m_Devices[deviceIndex].image[i] = NULL;
		m_Devices[deviceIndex].cur_frame_rate[i] = -1;
		memset( &m_Devices[deviceIndex].m_buf[i], 0, sizeof(m_Devices[deviceIndex].m_buf[i]));
		m_Devices[deviceIndex].m_recvd_buf[i] = 0;
	//	m_Devices[deviceIndex].m_mode[i] = mode_sing; // mode to be set from consumer app call to SetChannelMode
		m_Devices[deviceIndex].m_bTriggerSnapshot[i] = FALSE;
		m_Devices[deviceIndex].m_fps_changed[i] = 0;
	}   
	m_Devices[deviceIndex].m_bViewImage = TRUE;
	m_Devices[deviceIndex].m_bViewFR = TRUE;
	m_Devices[deviceIndex].m_framecount = 0;

	//Open the device
	if( S2255_DeviceOpen( deviceIndex, &m_Devices[deviceIndex].m_hdev) != 0) 
	{
		return -1;
    }  


   // setup control struct

	

	

	return (1);
}

__declspec(dllexport) int __stdcall Close2255Device(int deviceIndex)
{
	if ( deviceIndex < 0  || deviceIndex > SYS_GRABBERS) return(-1);

	int retval =  S2255_DeviceClose(  m_Devices[deviceIndex].m_hdev);

	if( retval != 0) 
	{
		return -1;
	}  
	else return (1);
}


void SetModeStruct ( MODE2255 * modeP, int standard, int compressionMode )
{

	modeP->format = standard;
	modeP->scale = SCALE_4CIFSI;//   SCALE_4CIFS
	modeP->fdec = DEF_FDEC;
	modeP->bright = DEF_BRIGHT;
	modeP->contrast = DEF_CONTRAST;
	modeP->saturation = DEF_SATURATION;
	modeP->hue = DEF_HUE;
	modeP->single = 0;

	 //----------------------------------------------------------------
    // Set up acquisition modes
  
	if ( compressionMode == S2255_MODE_BITMAP)
	{
		// make display mode RGB
		modeP->color &= ~MASK_COLOR;
   	//    modeP->color |= (COLOR_RGB & MASK_COLOR);
		modeP->color |= (COLOR_Y8 & MASK_COLOR);

	}
	else
	{
		//modeP->fdec = FDEC_2; // jpeg frame rate is 1/2 of normal

		// make default display mode JPEG
		modeP->color = 0;// clear all the bits
		//modeP->color |= (COLOR_JPG | (MASK_JPG_QUALITY  & (90 << 8) ));
		modeP->color |= (COLOR_JPG | (MASK_JPG_QUALITY  & (90 << 8) ));
	}
}

__declspec(dllexport) void __stdcall StopAcquisitionThread( int device, int channel)
{
	StopAcquisition(device, channel);
}
// creates the acquisition thread for capturing and displaying frames


__declspec(dllexport) void __stdcall SetChannelMode( int deviceIndex, int channel, int standard, int mode )
{
	if ( deviceIndex < 0  || deviceIndex > SYS_GRABBERS) return;
	if (  channel < 1  ||  channel > 4) return;

	int s = ( standard == S2255_NTSC)?  FORMAT_NTSC: FORMAT_PAL;
	SetModeStruct ( & m_Devices[deviceIndex].m_mode[channel-1], s, mode) ;

}


//__declspec(dllexport) void __stdcall StartAcquisitionThread( int deviceIndex, int channel, void  (__stdcall *buffFilledCB)(int jpegLength, LPBITMAPINFO binfo,  unsigned char   *image, int cindex ,int frameIndex, int * callersWinHandle), int * callersWinHandle)
__declspec(dllexport) void __stdcall StartAcquisitionThread( int deviceIndex, int channel, void  (__stdcall *buffFilledCB)(int jpegLength, LPBITMAPINFO binfo,  unsigned char   *image, int cindex ,int frameIndex))
{
    int idx = channel - 1;
	
	if ( m_Devices[deviceIndex].m_running[idx] == 1 ) return;// already running

	m_Devices[deviceIndex].m_StoppingChannel[idx] = false;

    DWORD threadID;

	m_Devices[deviceIndex].BufferFilledCB[idx] = buffFilledCB;

	struct ACQUIRE_THREAD_PARAMS * params = ( ACQUIRE_THREAD_PARAMS * ) malloc (sizeof (ACQUIRE_THREAD_PARAMS  ));
	if ( params == 0 ) return;

	params->deviceIndex = deviceIndex;
	params->channel = channel;
	m_Devices[deviceIndex].m_ack_thread[idx] = CreateThread(NULL, NULL, (LPTHREAD_START_ROUTINE) &AcquireThread, (LPVOID) params, NULL, &threadID);
}


// acquisition thread.  parameter is type (acq_thread_param_t *).

static DWORD WINAPI  AcquireThread(int * p)
{
    DWORD res;
    int i;
    int images = 0;
   	
	struct ACQUIRE_THREAD_PARAMS * params = (ACQUIRE_THREAD_PARAMS * )p;
	int deviceIndex = params->deviceIndex;
	int channel = params->channel;
	
    DWORD last_tick_count = GetTickCount();
    HANDLE hobjects[2];
    // index to channels start at 0, channels 1-4
    int idx = channel-1;
    int chn = channel;
 
    m_Devices[deviceIndex].m_running[idx] = 1;
   
    // Check and make sure device handle is valid.
    if( m_Devices[deviceIndex].m_hdev == NULL) {
        AfxMessageBox(_T("device not open.  Please restart application"));
      
        m_Devices[deviceIndex].m_ack_thread[idx] = NULL;
        return (DWORD) -3;
    }
  

	if( S2255_SetMode( m_Devices[deviceIndex].m_hdev, idx+1, &m_Devices[deviceIndex].m_mode[idx]) != 0) {
		AfxMessageBox(_T("Failed to set mode"));
	}

    // Create an event to stop this acquisition thread
    m_Devices[deviceIndex].m_stop_event[idx] = CreateEvent( NULL, TRUE, FALSE, NULL);
  

    //----------------------------------------
    // allocate space for pDoc->m_numbuf frames
    memset(&m_Devices[deviceIndex].m_buf[idx], 0, sizeof(m_Devices[deviceIndex].m_buf[idx]));
    m_Devices[deviceIndex].m_buf[idx].dwFrames = (ULONG) m_Devices[deviceIndex].m_numbuf;
    
    UINT32 size;
    
	size = S2255_get_image_size( &m_Devices[deviceIndex].m_mode[idx]);

	//size = 81920 + 64 ;// big enough for jpegs

    for( i=0;i<m_Devices[deviceIndex].m_numbuf;i++) {   
        m_Devices[deviceIndex].m_buf[idx].frame[i].pdata = (char *) malloc(size);
    }
    
    // done setting up buffers
    //----------------------------------------
    // Register user buffer and frames with the API(and driver) 
    if( S2255_RegBuffer( m_Devices[deviceIndex].m_hdev, idx+1, &m_Devices[deviceIndex].m_buf[idx], size) != 0) {
        AfxMessageBox(_T("Failed to register buffer"));
        CloseHandle(m_Devices[deviceIndex].m_stop_event[idx]);
        m_Devices[deviceIndex].m_stop_event[idx] = NULL;
      
         m_Devices[deviceIndex].m_running[idx] = 0;
        // free buffers
        for( i=0;i<m_Devices[deviceIndex].m_numbuf;i++) {
            //pDoc->m_buf[idx].lpbmi[i]->bmiHeader.biSizeImage = 0;
            free( m_Devices[deviceIndex].m_buf[idx].frame[i].pdata );
        }
        m_Devices[deviceIndex].m_ack_thread[idx] = NULL;
        return (DWORD) -3;
    }
    
    // Start Acquisition
	int retVal = S2255_StartAcquire( m_Devices[deviceIndex].m_hdev, chn, &m_Devices[deviceIndex].m_buf_event[idx]) ;
    if( retVal != 0) {
        AfxMessageBox(_T("Start Acquire Failed"));
        CloseHandle(m_Devices[deviceIndex].m_stop_event[idx]);
        m_Devices[deviceIndex].m_stop_event[idx] = NULL;
        m_Devices[deviceIndex].m_running[idx] = 0;

        // free buffers
        
        for( i=0;i<m_Devices[deviceIndex].m_numbuf;i++) {
            //pDoc->m_buf[idx].lpbmi[i]->bmiHeader.biSizeImage = 0;
            free( m_Devices[deviceIndex].m_buf[idx].frame[i].pdata );
        }
        m_Devices[deviceIndex].m_ack_thread[idx] = NULL;
        return (DWORD) -4;
    }
    // set up events
    hobjects[1] = m_Devices[deviceIndex].m_buf_event[idx];
    hobjects[0] = m_Devices[deviceIndex].m_stop_event[idx];
    m_Devices[deviceIndex].m_recvd_buf[idx] = 0;
    while (1) {  
        res = WaitForMultipleObjects(2, hobjects, FALSE, 6000);
        if( res == WAIT_TIMEOUT) {
            // timeout waiting for frame.  should not get here
            OutputDebugString(_T("2255-demo. frame timeout"));
            continue;
        } else if(res == WAIT_OBJECT_0) {
            // WAIT_OBJECT_0 is stop event.  
            break;
        } else if(res == (WAIT_OBJECT_0 + 1)) {
            // update the frame rate
            DWORD diff;
            int frm_idx = m_Devices[deviceIndex].m_recvd_buf[idx];
            DWORD tick_count = GetTickCount();
            m_Devices[deviceIndex].m_framecount++;
            
            images++;
            // approximate the current frame rate
            diff = tick_count - last_tick_count;
            if( (images == 10) && diff ) {
                m_Devices[deviceIndex].cur_frame_rate[idx] = 10*1000/diff;
                images = 0;
                last_tick_count = tick_count;
            }
            // point to current RGB image for the channel
            m_Devices[deviceIndex].image[idx] = (unsigned char *)m_Devices[deviceIndex].m_buf[idx].frame[frm_idx].pdata;
            // do work with image.  EG. if snapshots being taken
        
           
			// do something with the image here
			int jpegLength = 0;
			if ( (m_Devices[deviceIndex].m_mode[idx].color & MASK_COLOR) == COLOR_JPG )
			{
				if (  & m_Devices[deviceIndex].m_buf[idx].lpbmi[frm_idx]->bmiHeader != 0)	
					jpegLength = m_Devices[deviceIndex].m_buf[idx].lpbmi[frm_idx]->bmiHeader.biSizeImage;
			}

		// pass the new frame to the upper layer via the callback

			if ( ! m_Devices[deviceIndex].m_StoppingChannel[idx] )
				m_Devices[deviceIndex].BufferFilledCB[idx](jpegLength,  m_Devices[deviceIndex].m_buf[idx].lpbmi[frm_idx], m_Devices[deviceIndex].image[idx], idx, frm_idx);

            // done with buffer, dq it
            S2255_DQBUF( m_Devices[deviceIndex].m_hdev, chn, m_Devices[deviceIndex].m_recvd_buf[idx]);

            m_Devices[deviceIndex].m_recvd_buf[idx]++;
            if((unsigned int) m_Devices[deviceIndex].m_recvd_buf[idx] == m_Devices[deviceIndex].m_buf[idx].dwFrames) {
                m_Devices[deviceIndex].m_recvd_buf[idx] = 0;
            }
        }
    }
    
	SetEvent( m_Devices[deviceIndex].m_ack_thread[idx]);

    // Thread loop exited.  Call S2255_StopAcquire to stop acquisition
    if( S2255_StopAcquire( m_Devices[deviceIndex].m_hdev, chn ) != 0) {
        //AfxMessageBox("Stop failed");
        OutputDebugString(_T("stop failed!"));
    }
    // Close our stop event
    CloseHandle(m_Devices[deviceIndex].m_stop_event[idx]);
    // Set event handles to NULL (m_buf_event[idx] closed in StopAcquire)
    m_Devices[deviceIndex].m_buf_event[idx] = NULL;
    m_Devices[deviceIndex].m_stop_event[idx] = NULL;
	m_Devices[deviceIndex].m_running[idx] = 0;

    // free user allocated buffers
    for( i=0;i<m_Devices[deviceIndex].m_numbuf;i++) {
        m_Devices[deviceIndex].m_buf[idx].lpbmi[i]->bmiHeader.biSizeImage = 0;
        free( m_Devices[deviceIndex].m_buf[idx].frame[i].pdata );
        m_Devices[deviceIndex].m_buf[idx].frame[i].pdata = NULL;
    }
   
   
   if (m_Devices[deviceIndex].m_fps_changed[idx]) {
        m_Devices[deviceIndex].m_mode[idx].fdec = m_Devices[deviceIndex].m_prev_fdec[idx];
        S2255_SetMode(m_Devices[deviceIndex].m_hdev, (idx+1), &m_Devices[deviceIndex].m_mode[idx]);
        m_Devices[deviceIndex].m_fps_changed[idx] = 0;
    }
    
    m_Devices[deviceIndex].m_ack_thread[idx] = NULL;
    m_Devices[deviceIndex].cur_frame_rate[idx] =-1;
 
	free ( params);

	
    return 0;
}

__declspec(dllexport) int __stdcall GetVideoConnectionStatus (int deviceIndex, int channel)
{

	unsigned int status = 0;


	S2255_get_vid_status ( m_Devices[deviceIndex].m_hdev, channel, & status);

	if ( (status & 0x1 )== 1 ) return(1);
	else return(0);
}



// stop acquisition
void StopAcquisition( int deviceIndex, int channel)
{
  
    unsigned int idx = channel - 1;
   
    
    if (idx >= 4) {
        AfxMessageBox(_T("resource.h IDs must be sequential! failing\n"));
        return;
    }

	m_Devices[deviceIndex].m_StoppingChannel[idx] = true;


	SetEvent( m_Devices[deviceIndex].m_stop_event[idx]);
   
	// wait for acquisition thread to stop
    WaitForSingleObject( m_Devices[deviceIndex].m_ack_thread[idx], INFINITE);
   
    return;

}

// CS2255InterfaceApp initialization

//BOOL CS2255InterfaceApp::InitInstance()
//{
//	CWinApp::InitInstance();
//
//	return TRUE;
//}
