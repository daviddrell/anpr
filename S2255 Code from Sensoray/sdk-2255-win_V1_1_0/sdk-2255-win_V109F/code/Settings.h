#pragma once
#include "afxcmn.h"

#include "s2255.h"
#include "app-2255-demodoc.h"


// CSettings dialog

class CSettings : public CPropertyPage
{
	DECLARE_DYNAMIC(CSettings)

public:
	CSettings();
	virtual ~CSettings();

// Dialog Data
	enum { IDD = IDD_SETTINGS };
    BOOL OnInitDialog();
public:
    int m_iBright;
    int m_iContrast;
    int m_iSat;
    int m_iHue;
    BOOL bChanged;
    int m_channel;
    MODE2255    mode;
    Capp2255demoDoc *pDoc ;
    CSliderCtrl m_sldBright;
    CSliderCtrl m_sldContrast;
    CSliderCtrl m_sldHue;
    CSliderCtrl m_sldSat;

protected:
    CString m_sBright;
    CString m_sHue;
    CString m_sContrast;
    CString m_sSat;
    virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
	DECLARE_MESSAGE_MAP()
    afx_msg void OnBnClickedBtnViddefaults();
    afx_msg void OnHScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar);
};
