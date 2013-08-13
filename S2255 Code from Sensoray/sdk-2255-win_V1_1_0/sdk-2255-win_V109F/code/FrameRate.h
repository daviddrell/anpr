#pragma once

#include "s2255.h"
#include "app-2255-demodoc.h"

// CFrameRate dialog

class CFrameRate : public CPropertyPage
{
	DECLARE_DYNAMIC(CFrameRate)

public:
	CFrameRate();
	virtual ~CFrameRate();
    int m_index;
    BOOL m_bStreaming;
// Dialog Data
	enum { IDD = IDD_PROPPAGE_DECIMATION };
    int m_channel;
    MODE2255    mode;
    Capp2255demoDoc *pDoc ;
    BOOL OnApply();
protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
    CButton m_rad1;
    CButton m_rad2;
    CButton m_rad3;
    CButton m_rad5;
    
	DECLARE_MESSAGE_MAP()
public:
    afx_msg void OnBnClickedRadioFr1();
    afx_msg void OnBnClickedRadioFr2();
    afx_msg void OnBnClickedRadioFr3();
    afx_msg void OnBnClickedRadioFr5();
    afx_msg BOOL OnInitDialog();
};
