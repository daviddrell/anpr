// s2255.dll Please see manual at http://www.sensoray.com/2255
// V1.0.9F adds new query buf function(see below)
#ifndef S2255F_H
#define S2255F_H

#include "s2255.h"
#define S2255_SPEC __declspec(dllimport)

extern "C" {

S2255_SPEC int __stdcall S2255_DeviceOpen(int board, HANDLE *hdev);
S2255_SPEC int __stdcall S2255_DeviceClose(HANDLE hdev);
S2255_SPEC int __stdcall S2255_SetMode(HANDLE hdev, int channel, MODE2255 *mode);
S2255_SPEC int __stdcall S2255_RegBuffer(HANDLE hdev, int channel, BUFFER *pBuf, UINT32 bufsize);
S2255_SPEC int __stdcall S2255_DQBUF(HANDLE hdev, int channel,int frmnum );
S2255_SPEC int __stdcall S2255_StartAcquire(HANDLE hdev, int channel, HANDLE *phevent);
S2255_SPEC int __stdcall S2255_StopAcquire(HANDLE hdev, int channel);
S2255_SPEC int __stdcall S2255_GetStatus(HANDLE hdev);

// blocking call to capture single frame.
// less efficient than StartAcquire/StopAcquire looping capture
// please see manual.  does not require
S2255_SPEC int __stdcall S2255_GrabFrame(HDEVICE hdev, int chn, unsigned char *pFrame, unsigned long size, unsigned long timeout, BITMAPINFO *lpbmi);

// S2255_GetFrame obsoleted.  please upgrade to S2255_GrabFrame for easier access to BITMAPINFO
//S2255_SPEC int __stdcall S2255_GetFrame(HANDLE hdev, int channel, unsigned char *pFrame, unsigned long size, unsigned long timeout);


S2255_SPEC UINT32 __stdcall S2255_get_image_size( MODE2255 *mode);
S2255_SPEC UINT32 __stdcall S2255_get_vid_status( HDEVICE hdev, int chn, UINT32 *pStatus);


// New in V1.0.9F
// queries state of the buffer.
// returns:
// 0 if unused, available or free
// 1 if currently being filled  (should not use buffer in this case)
// 2 if ready or filled (needs DEQUEUED after use).  Will get an signal when this happens
//-1 if err
S2255_SPEC int __stdcall S2255_query_buf(HDEVICE hdev, int chn, int frmnum);


}
#endif
