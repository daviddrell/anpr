// Settings.cpp : implementation file
// Copyright Sensoray Company 2007-2009

#include "stdafx.h"
#include "app-2255-demo.h"
#include "Settings.h"
#include ".\settings.h"
#include "s2255f.h"


// CSettings dialog
IMPLEMENT_DYNAMIC(CSettings, CPropertyPage)
CSettings::CSettings()
	: CPropertyPage(CSettings::IDD)
    , m_iBright(0)
    , m_iContrast(0x5c)
    , m_iSat(0x80)
    , m_iHue(0)
    , m_sBright(_T(""))
{
    bChanged = FALSE;
    m_channel = 1;
    pDoc = NULL;
    memset( &mode, 0, sizeof(mode));
}

CSettings::~CSettings()
{
    pDoc = NULL;
}

void CSettings::DoDataExchange(CDataExchange* pDX)
{
    CPropertyPage::DoDataExchange(pDX);
    DDX_Slider(pDX, IDC_SLIDER_BRIGHT, m_iBright);
    DDX_Slider(pDX, IDC_SLIDER_CONTRAST, m_iContrast);
    DDX_Slider(pDX, IDC_SLIDER_SAT, m_iSat);
    DDX_Slider(pDX, IDC_SLIDER_HUE, m_iHue);
    DDX_Control(pDX, IDC_SLIDER_BRIGHT, m_sldBright);
    DDX_Control(pDX, IDC_SLIDER_CONTRAST, m_sldContrast);
    DDX_Control(pDX, IDC_SLIDER_SAT, m_sldSat);
    DDX_Control(pDX, IDC_SLIDER_HUE, m_sldHue);
    DDX_Text(pDX, IDC_STATIC_BRIGHT, m_sBright);
    DDX_Text(pDX, IDC_STATIC_HUE, m_sHue);
    DDX_Text(pDX, IDC_STATIC_SAT, m_sSat);
    DDX_Text(pDX, IDC_STATIC_CONTRAST, m_sContrast);
}


BEGIN_MESSAGE_MAP(CSettings, CPropertyPage)
    ON_BN_CLICKED(IDC_BTN_VIDDEFAULTS, OnBnClickedBtnViddefaults)
    ON_WM_HSCROLL()
END_MESSAGE_MAP()


// CSettings message handlers
BOOL CSettings::OnInitDialog() 
{
    CPropertyPage::OnInitDialog();
    m_sldBright.SetRange(-128, 127, TRUE);
    m_sldContrast.SetRange(0,0xff,TRUE);
    m_sldHue.SetRange(-128,127,TRUE);
    m_sldSat.SetRange(0,255,TRUE);
    m_sBright.Format(_T("%d"), m_iBright);
    m_sHue.Format(_T("%d"), m_iHue);
    m_sContrast.Format(_T("%d"), m_iContrast);
    m_sSat.Format(_T("%d"), m_iSat);
    UpdateData( FALSE);
    return TRUE;
}

void CSettings::OnBnClickedBtnViddefaults()
{
    mode.bright = DEF_BRIGHT;
    mode.contrast = DEF_CONTRAST;
    mode.saturation = DEF_SATURATION;
    mode.hue = DEF_HUE;
    if( pDoc ) {
        S2255_SetMode( pDoc->m_hdev, m_channel, &mode);
    }
    bChanged = TRUE;
    m_iBright = (int) mode.bright;
    m_iContrast = (int) mode.contrast;
    m_iHue = (int) mode.hue;
    m_iSat = (int) mode.saturation; 
    m_sBright.Format(_T("%d"), m_iBright);
    m_sHue.Format(_T("%d"), m_iHue);
    m_sContrast.Format(_T("%d"), m_iContrast); 
    m_sSat.Format(_T("%d"), m_iSat);
    UpdateData(FALSE);
}

void CSettings::OnHScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar)
{
	if( pScrollBar)
	{
        BOOL bmoved = FALSE;
        switch( nSBCode )
        {
        case SB_LEFT:
            bmoved = TRUE;
            break;
        case SB_RIGHT:
            bmoved = TRUE;
            break;
        case SB_LINELEFT:
            bmoved = TRUE;
            break;
        case SB_LINERIGHT:
            bmoved = TRUE;
            break;
        case SB_PAGELEFT:
            bmoved = TRUE;
            break;
        case SB_PAGERIGHT:
            bmoved = TRUE;
            break;
        case SB_THUMBPOSITION:
            break;
        case SB_THUMBTRACK:
            bmoved = TRUE;
            break;
        case SB_ENDSCROLL:
            break;
        default:
            break;
        }

        if( bmoved) {
            int id;
            UpdateData(TRUE);
            id = pScrollBar->GetDlgCtrlID();
            if( id == m_sldBright.GetDlgCtrlID())
            {
                mode.bright = (UINT32) m_sldBright.GetPos();
                m_sBright.Format(_T("%d"), m_iBright);
            }
            else if( id == m_sldContrast.GetDlgCtrlID())
            {
                mode.contrast = (UINT32) m_sldContrast.GetPos();
                m_sContrast.Format(_T("%d"), m_iContrast);
            }
            else if( id == m_sldHue.GetDlgCtrlID())
            {
                mode.hue = (UINT32) m_sldHue.GetPos();
                m_sHue.Format(_T("%d"), m_iHue);
            }
            else if( id == m_sldSat.GetDlgCtrlID())
            {
                mode.saturation = (UINT32) m_sldSat.GetPos();
                m_sSat.Format(_T("%d"), m_iSat);
            }
            if( pDoc) {
                S2255_SetMode( pDoc->m_hdev, m_channel, &mode);
            }
            bChanged = TRUE;
            UpdateData(FALSE);

        }
    }
    CPropertyPage::OnHScroll(nSBCode, nPos, pScrollBar);
}
