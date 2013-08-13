#pragma once


// CFormat dialog

class CFormat : public CPropertyPage
{
	DECLARE_DYNAMIC(CFormat)

public:
	CFormat();
	virtual ~CFormat();
    BOOL m_bStreaming;
    int  m_index;
    BOOL OnInitDialog();

    // Dialog Data
	enum { IDD = IDD_PROPPAGE_FORMAT };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // DDX/DDV support
    CButton m_rad1;
    CButton m_rad2;
    CButton m_rad3;
	DECLARE_MESSAGE_MAP()


};
