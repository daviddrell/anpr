
// s2255InterfaceAPI.h

#pragma once

#define S2255_MODE_JPEG 1
#define S2255_MODE_BITMAP 2

#define S2255_NTSC 0
#define S2255_PAL 1

	__declspec(dllexport) int __stdcall  Open2255Device(int deviceIndex, int standard);
	__declspec(dllexport) int __stdcall  Close2255Device(int deviceIndex);
	__declspec(dllexport) void __stdcall StartAcquisitionThread( int deviceIndex, int channel, void (  *buffFilledCB)(int jpegLength, LPBITMAPINFO binfo,  unsigned char   *image, int cindex ,int frameIndex));
	__declspec(dllexport) void __stdcall StopAcquisitionThread(int deviceIndex,  int channel);
	__declspec(dllexport) void __stdcall SetChannelMode( int deviceIndex, int channel, int standard, int mode );
	

//	void(__stdcall  *BufferFilledCB)(int jpegLength, LPBITMAPINFO binfo,  unsigned char   *image, int cindex , int frameIndex, int * callersWinHandle);