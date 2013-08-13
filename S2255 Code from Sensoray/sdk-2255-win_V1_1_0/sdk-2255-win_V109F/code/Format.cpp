// Format.cpp : implementation file
// Copyright Sensoray Company 2007-2009

#include "stdafx.h"
#include "app-2255-demo.h"
#include "Format.h"


// CFormat dialog
IMPLEMENT_DYNAMIC(CFormat, CPropertyPage)
CFormat::CFormat()
	: CPropertyPage(CFormat::IDD)
{
    m_bStreaming = FALSE;
    m_index = 0;
}

CFormat::~CFormat()
{
}

void CFormat::DoDataExchange(CDataExchange* pDX)
{
	CPropertyPage::DoDataExchange(pDX);
    DDX_Control(pDX, IDC_RADIO_NTSC, m_rad1);
    DDX_Control(pDX, IDC_RADIO_PAL, m_rad2);
    DDX_Control(pDX, IDC_RADIO_SECAM, m_rad3);

    DDX_Radio(pDX, IDC_RADIO_NTSC, m_index);
}


BEGIN_MESSAGE_MAP(CFormat, CPropertyPage)
END_MESSAGE_MAP()


// CFormat message handlers
BOOL CFormat::OnInitDialog()
{
	CPropertyPage::OnInitDialog();

    if( m_bStreaming)
    {
        // disable the controls if currently streaming
        m_rad3.EnableWindow(FALSE);
        m_rad2.EnableWindow(FALSE);
        m_rad1.EnableWindow(FALSE);
    }

	return TRUE;  // return TRUE unless you set the focus to a control
	// EXCEPTION: OCX Property Pages should return FALSE
}
