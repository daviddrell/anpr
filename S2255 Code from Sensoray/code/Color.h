#pragma once
#include "afxcmn.h"
#include "afxwin.h"


// CColor dialog

class CColor : public CPropertyPage
{
	DECLARE_DYNAMIC(CColor)

public:
	CColor();
	virtual ~CColor();
    int m_index;
    BOOL m_bStreaming;
// Dialog Data
	enum { IDD = IDD_PROPPAGE_COLOR };
    BOOL OnInitDialog();

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
    CButton m_rad1;
    CButton m_rad2;
    CButton m_rad3;
    CButton m_rad4;
	CButton m_rad5;
    DECLARE_MESSAGE_MAP()
public:
	int m_iJPG;
	CSliderCtrl m_sldrJpeg;
	CStatic m_statJpeg;
	afx_msg void OnBnClickedRadioJpeg();
	afx_msg void OnBnClickedRadioY8();
	afx_msg void OnBnClickedRadioRgb();
};
