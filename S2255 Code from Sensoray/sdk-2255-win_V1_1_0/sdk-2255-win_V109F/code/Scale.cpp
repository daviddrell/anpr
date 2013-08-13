// Scale Settings
// Scale.cpp : implementation file
// Copyright Sensoray Company 2007-2009

#include "stdafx.h"
#include "app-2255-demo.h"
#include "Scale.h"


// CScale dialog

IMPLEMENT_DYNAMIC(CScale, CPropertyPage)
CScale::CScale()
	: CPropertyPage(CScale::IDD)
{
    m_bStreaming = FALSE;
    m_index = 0;
}

CScale::~CScale()
{
}

void CScale::DoDataExchange(CDataExchange* pDX)
{
	CPropertyPage::DoDataExchange(pDX);
    DDX_Control(pDX, IDC_RADIO_4CIFS, m_rad4);
    DDX_Control(pDX, IDC_RADIO_2CIFS, m_rad2);
    DDX_Control(pDX, IDC_RADIO_1CIFS, m_rad1);
    DDX_Control(pDX, IDC_RADIO_4CIFSI, m_rad4I);
    DDX_Radio(pDX, IDC_RADIO_4CIFS, m_index);
}


BEGIN_MESSAGE_MAP(CScale, CPropertyPage)
    ON_BN_CLICKED(IDC_RADIO_1CIFS, OnBnClickedRad1)
    ON_BN_CLICKED(IDC_RADIO_2CIFS, OnBnClickedRad2)
    ON_BN_CLICKED(IDC_RADIO_4CIFS, OnBnClickedRad4)
END_MESSAGE_MAP()




// CScale message handlers
BOOL CScale::OnInitDialog()
{
	CPropertyPage::OnInitDialog();

    if( m_bStreaming) {
        // disable the controls if currently streaming
        m_rad4.EnableWindow(FALSE);
        m_rad2.EnableWindow(FALSE);
        m_rad1.EnableWindow(FALSE);
		m_rad4I.EnableWindow(FALSE);
    }

	return TRUE;  // return TRUE unless you set the focus to a control
	// EXCEPTION: OCX Property Pages should return FALSE
}



void CScale::OnBnClickedRad1()
{
}

void CScale::OnBnClickedRad2()
{
}

void CScale::OnBnClickedRad4()
{
}
