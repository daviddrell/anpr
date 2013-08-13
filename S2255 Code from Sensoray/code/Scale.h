#pragma once


// CScale dialog

class CScale : public CPropertyPage
{
	DECLARE_DYNAMIC(CScale)

public:
	CScale();
	virtual ~CScale();

// Dialog Data
	enum { IDD = IDD_PROPPAGE_SCALE };
    int     m_index;
    BOOL    m_bStreaming;
    BOOL OnInitDialog();
protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
    CButton m_rad1;
    CButton m_rad2;
    CButton m_rad4;
    CButton m_rad4I;

	DECLARE_MESSAGE_MAP()
    afx_msg void OnBnClickedRad1();
    afx_msg void OnBnClickedRad2();
    afx_msg void OnBnClickedRad4();
	afx_msg void OnBnClickedRad4I();

};
