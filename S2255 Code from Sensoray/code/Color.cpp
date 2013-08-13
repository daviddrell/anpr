// Color.cpp : implementation file
// Copyright Sensoray Company

#include "stdafx.h"
#include "app-2255-demo.h"
#include "s2255.h"
#include "Color.h"
#include ".\color.h"

// CColor dialog
IMPLEMENT_DYNAMIC(CColor, CPropertyPage)
CColor::CColor()
	: CPropertyPage(CColor::IDD)
	, m_iJPG(0)
{
    m_index = 0;
    m_bStreaming = FALSE;
}

CColor::~CColor()
{
}

void CColor::DoDataExchange(CDataExchange* pDX)
{
	CPropertyPage::DoDataExchange(pDX);
	DDX_Control(pDX, IDC_RADIO_YUVPL, m_rad1);
	DDX_Control(pDX, IDC_RADIO_YUVPK, m_rad2);
	DDX_Control(pDX, IDC_RADIO_RGB, m_rad3);
	DDX_Control(pDX, IDC_RADIO_Y8, m_rad4);
	DDX_Control(pDX, IDC_RADIO_JPEG, m_rad5);
	DDX_Radio(pDX, IDC_RADIO_YUVPL, m_index);
	DDX_Slider(pDX, IDC_SLIDER_JPEG_QUALITY, m_iJPG);
	DDX_Control(pDX, IDC_SLIDER_JPEG_QUALITY, m_sldrJpeg);
	DDX_Control(pDX, IDC_STATIC_JPEG_QUALITY, m_statJpeg);
}


BEGIN_MESSAGE_MAP(CColor, CPropertyPage)
	ON_BN_CLICKED(IDC_RADIO_JPEG, OnBnClickedRadioJpeg)
	ON_BN_CLICKED(IDC_RADIO_Y8, OnBnClickedRadioY8)
	ON_BN_CLICKED(IDC_RADIO_RGB, OnBnClickedRadioRgb)
END_MESSAGE_MAP()


// CColor message handlers
BOOL CColor::OnInitDialog()
{
	CPropertyPage::OnInitDialog();

    if( m_bStreaming)
    {
        // disable the controls if currently streaming
		m_rad5.EnableWindow(FALSE);
        m_rad4.EnableWindow(FALSE);
        m_rad3.EnableWindow(FALSE);
        m_rad2.EnableWindow(FALSE);
        m_rad1.EnableWindow(FALSE);
		m_sldrJpeg.ShowWindow(SW_HIDE);
		m_statJpeg.ShowWindow(SW_HIDE);
    }
	// radio buttons MUST be in same order as color formats
	if ((m_index + 1) != COLOR_JPG) {
		m_sldrJpeg.ShowWindow(SW_HIDE);
		m_statJpeg.ShowWindow(SW_HIDE);
	} else {
		m_sldrJpeg.ShowWindow(SW_NORMAL);
		m_statJpeg.ShowWindow(SW_NORMAL);
	}
	return TRUE;  // return TRUE unless you set the focus to a control
	// EXCEPTION: OCX Property Pages should return FALSE
}

void CColor::OnBnClickedRadioJpeg()
{
	m_sldrJpeg.ShowWindow(SW_NORMAL);
	m_statJpeg.ShowWindow(SW_NORMAL);
}

void CColor::OnBnClickedRadioY8()
{
	m_sldrJpeg.ShowWindow(SW_HIDE);
	m_statJpeg.ShowWindow(SW_HIDE);
}

void CColor::OnBnClickedRadioRgb()
{
	m_sldrJpeg.ShowWindow(SW_HIDE);
	m_statJpeg.ShowWindow(SW_HIDE);
}
