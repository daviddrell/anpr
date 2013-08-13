// LPROCR_lib_c.h : main header file for the LPROCR_lib_c DLL
//

#pragma once

#ifndef __AFXWIN_H__
	#error "include 'stdafx.h' before including this file for PCH"
#endif

#include "resource.h"		// main symbols


// CLPROCR_lib_cApp
// See LPROCR_lib_c.cpp for the implementation of this class
//

class CLPROCR_lib_cApp : public CWinApp
{
public:
	CLPROCR_lib_cApp();

// Overrides
public:
	virtual BOOL InitInstance();

	DECLARE_MESSAGE_MAP()
};
