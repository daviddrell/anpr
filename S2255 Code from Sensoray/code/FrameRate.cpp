// FrameRate.cpp : implementation file
// Copyright Sensoray Company 2007-2009

#include "stdafx.h"
#include "app-2255-demo.h"
#include "FrameRate.h"
#include ".\framerate.h"
#include "s2255f.h"

// CFrameRate dialog
IMPLEMENT_DYNAMIC(CFrameRate, CPropertyPage)
CFrameRate::CFrameRate()
	: CPropertyPage(CFrameRate::IDD)
{
    m_index = 0;
    m_bStreaming = FALSE;
    m_channel = 1;
    pDoc = NULL;
    memset( &mode, 0, sizeof(mode));
}

CFrameRate::~CFrameRate()
{
    pDoc = NULL;
}

void CFrameRate::DoDataExchange(CDataExchange* pDX)
{
	CPropertyPage::DoDataExchange(pDX);
    DDX_Control(pDX, IDC_RADIO_FR1, m_rad1);
    DDX_Control(pDX, IDC_RADIO_FR2, m_rad2);
    DDX_Control(pDX, IDC_RADIO_FR3, m_rad3);
    DDX_Control(pDX, IDC_RADIO_FR5, m_rad5);
    DDX_Radio(pDX, IDC_RADIO_FR1, m_index);
}


BEGIN_MESSAGE_MAP(CFrameRate, CPropertyPage)
    ON_BN_CLICKED(IDC_RADIO_FR1, OnBnClickedRadioFr1)
    ON_BN_CLICKED(IDC_RADIO_FR2, OnBnClickedRadioFr2)
    ON_BN_CLICKED(IDC_RADIO_FR3, OnBnClickedRadioFr3)
    ON_BN_CLICKED(IDC_RADIO_FR5, OnBnClickedRadioFr5)
END_MESSAGE_MAP()


// CFrameRate message handlers
BOOL CFrameRate::OnInitDialog()
{
	CPropertyPage::OnInitDialog();
#if 0
    if( m_bStreaming)
    {
        // disable the controls if currently streaming
        m_rad5.EnableWindow(FALSE);
        m_rad3.EnableWindow(FALSE);
        m_rad2.EnableWindow(FALSE);
        m_rad1.EnableWindow(FALSE);
    }
#endif

	return TRUE;  // return TRUE unless you set the focus to a control
	// EXCEPTION: OCX Property Pages should return FALSE
}


void CFrameRate::OnBnClickedRadioFr1()
{
    SetModified( TRUE);
}

void CFrameRate::OnBnClickedRadioFr2()
{
    SetModified( TRUE);
}

void CFrameRate::OnBnClickedRadioFr3()
{
    SetModified( TRUE);
}

void CFrameRate::OnBnClickedRadioFr5()
{
    SetModified( TRUE);
}

BOOL CFrameRate::OnApply()
{
    UpdateData(TRUE);
    if( pDoc == NULL)
        return FALSE;

    mode.fdec = (UINT32) (m_index+1);
    if( m_index == 4) {
        mode.fdec = FDEC_5;
    }
    S2255_SetMode( pDoc->m_hdev, m_channel, &mode);
    return TRUE;
}
 