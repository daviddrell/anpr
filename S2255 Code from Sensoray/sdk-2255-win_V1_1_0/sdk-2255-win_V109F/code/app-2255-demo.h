// app-2255-demo.h : main header file for the app-2255-demo application
//
#pragma once

#ifndef __AFXWIN_H__
	#error include 'stdafx.h' before including this file for PCH
#endif

#include "resource.h"       // main symbols


// Capp2255demoApp:
// See app-2255-demo.cpp for the implementation of this class
//

class Capp2255demoApp : public CWinApp
{
public:
	Capp2255demoApp();


// Overrides
public:
	virtual BOOL InitInstance();

// Implementation
	afx_msg void OnAppAbout();
	DECLARE_MESSAGE_MAP()
};

extern Capp2255demoApp theApp;