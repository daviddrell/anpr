// app-2255-demoView.cpp : implementation of the Capp2255demoView class
// Copyright 2007-2009 (C)  Sensoray Corporation Inc.
// D.A.

#include "stdafx.h" 
#include "app-2255-demo.h"
#include "settings.h"
#include "format.h" 
#include "scale.h"
#include "framerate.h"
#include "color.h"
#include "app-2255-demoDoc.h"
#include "app-2255-demoView.h"
#include ".\app-2255-demoview.h"
#include "s2255f.h"
#include "image.h"
#include "assert.h"

// Capp2255demoView
IMPLEMENT_DYNCREATE(Capp2255demoView, CView)


// Command call backs
BEGIN_MESSAGE_MAP(Capp2255demoView, CView)
ON_COMMAND_RANGE(ID_ACQUIRE_START, ID_ACQUIRE_START4, OnAcquireStart)  
ON_COMMAND_RANGE(ID_ACQUIRE_STOP, ID_ACQUIRE_STOP4, OnAcquireStop)
ON_UPDATE_COMMAND_UI_RANGE(ID_ACQUIRE_START, ID_ACQUIRE_START4, OnUpdateAcquireStart)
ON_UPDATE_COMMAND_UI_RANGE(ID_ACQUIRE_STOP, ID_ACQUIRE_STOP4, OnUpdateAcquireStop)
ON_WM_ERASEBKGND()
ON_COMMAND(ID_VIEW_FRAMERATE, OnViewFramerate)
ON_UPDATE_COMMAND_UI(ID_VIEW_FRAMERATE, OnUpdateViewFramerate)
ON_UPDATE_COMMAND_UI(ID_VIEW_DISPLAYIMAGE, OnUpdateViewDisplayimage)
ON_COMMAND(ID_VIEW_DISPLAYIMAGE, OnViewDisplayimage)
ON_COMMAND_RANGE(ID_SETTINGS_CHANNEL1, ID_SETTINGS_CHANNEL4, OnSettingsChannel)
ON_COMMAND(ID_SETTINGS_ALLCHANNELS, OnSettingsAllchannels)
ON_COMMAND(ID_ACQUIRE_STARTALL, OnAcquireStartall)
ON_COMMAND(ID_ACQUIRE_STOPALL, OnAcquireStopall)
ON_BN_CLICKED(ID_TOOLS_QUERYFW, OnQueryFW)
END_MESSAGE_MAP()

// Capp2255demoView construction/destruction
Capp2255demoView::Capp2255demoView()
{
}

Capp2255demoView::~Capp2255demoView()
{
}

BOOL Capp2255demoView::PreCreateWindow(CREATESTRUCT& cs)
{
    // TODO: Modify the Window class or styles here by modifying
    //  the CREATESTRUCT cs
    return CView::PreCreateWindow(cs);
}


// Capp2255demoView drawing
void Capp2255demoView::OnDraw(CDC* pDC)
{
    CView::OnDraw( pDC);
    return;
}


typedef struct 
{
    Capp2255demoDoc  *pDoc;
    Capp2255demoView *pView;
    int              channel;
} acq_thread_param_t;

// acquisition thread.  parameter is type (acq_thread_param_t *).
static DWORD WINAPI AcquireThread( LPVOID lpParameter)
{
    DWORD res;
    int i;
    int images = 0;
    acq_thread_param_t *param = (acq_thread_param_t *) lpParameter;
    DWORD last_tick_count = GetTickCount();
    HANDLE hobjects[2];
    // index to channels start at 0, channels 1-4
    int idx = param->channel - 1;
    int chn = param->channel;
    Capp2255demoDoc  *pDoc = param->pDoc; 
    Capp2255demoView *pView = param->pView;
    
    if( pDoc == NULL) {
        free(param);
        return (DWORD) -1;
    }
    // Check and make sure device handle is valid.
    if( pDoc->m_hdev == NULL) {
        AfxMessageBox(_T("device not open.  Please restart application"));
        free(param);
        pDoc->m_running[idx] = 0;
        pDoc->m_ack_thread[idx] = NULL;
        return (DWORD) -3;
    }
    
    // This capture mode uses continuous capture.  Check to see if occasional snapshots were used with 
    // single mode
    if( pDoc->m_mode[idx].single) {
        pDoc->m_mode[idx].single = 0;
        if( S2255_SetMode( pDoc->m_hdev, idx+1, &pDoc->m_mode[idx]) != 0) {
            AfxMessageBox(_T("Failed to set mode back to continuous capture"));
        }
    }
    
    // Create an event to stop this acquisition thread
    pDoc->m_stop_event[idx] = CreateEvent( NULL, TRUE, FALSE, NULL);
    
    //----------------------------------------
    // allocate space for pDoc->m_numbuf frames
    memset(&pDoc->m_buf[idx], 0, sizeof(pDoc->m_buf[idx]));
    pDoc->m_buf[idx].dwFrames = (ULONG) pDoc->m_numbuf;
    
    UINT32 size;
    size = S2255_get_image_size( &pDoc->m_mode[idx]);
    for( i=0;i<pDoc->m_numbuf;i++) {
        pDoc->m_buf[idx].frame[i].pdata = (char *) malloc(size);
    }
    
    // done setting up buffers
    //----------------------------------------
    // Register user buffer and frames with the API(and driver) 
    if( S2255_RegBuffer( pDoc->m_hdev, idx+1, &pDoc->m_buf[idx], size) != 0) {
        AfxMessageBox(_T("Failed to register buffer"));
        CloseHandle(pDoc->m_stop_event[idx]);
        pDoc->m_stop_event[idx] = NULL;
        pDoc->m_running[idx] = 0;
        
        // free buffers
        for( i=0;i<pDoc->m_numbuf;i++) {
            //pDoc->m_buf[idx].lpbmi[i]->bmiHeader.biSizeImage = 0;
            free( pDoc->m_buf[idx].frame[i].pdata );
        }
        free(param);
        pDoc->m_ack_thread[idx] = NULL;
        return (DWORD) -3;
    }
    
    // Start Acquisition
    if( S2255_StartAcquire( pDoc->m_hdev, chn, &pDoc->m_buf_event[idx]) != 0) {
        AfxMessageBox(_T("Start Acquire Failed"));
        CloseHandle(pDoc->m_stop_event[idx]);
        pDoc->m_stop_event[idx] = NULL;
        pDoc->m_running[idx] = 0;
        // free buffers
        
        for( i=0;i<pDoc->m_numbuf;i++) {
            //pDoc->m_buf[idx].lpbmi[i]->bmiHeader.biSizeImage = 0;
            free( pDoc->m_buf[idx].frame[i].pdata );
        }
        free(param);
        pDoc->m_ack_thread[idx] = NULL;
        return (DWORD) -4;
    }
    // set up events
    hobjects[1] = pDoc->m_buf_event[idx];
    hobjects[0] = pDoc->m_stop_event[idx];
    pDoc->m_recvd_buf[idx] = 0;
    while (1) {  
        res = WaitForMultipleObjects(2, hobjects, FALSE, 6000);
        if( res == WAIT_TIMEOUT) {
            // timeout waiting for frame.  should not get here
            OutputDebugString(_T("2255-demo. frame timeout"));
            continue;
        } else if(res == WAIT_OBJECT_0) {
            // WAIT_OBJECT_0 is stop event.  
            break;
        } else if(res == (WAIT_OBJECT_0 + 1)) {
            // update the frame rate
            DWORD diff;
            int frm_idx = pDoc->m_recvd_buf[idx];
            DWORD tick_count = GetTickCount();
            pDoc->m_framecount++;
            
            images++;
            // approximate the current frame rate
            diff = tick_count - last_tick_count;
            if( (images == 10) && diff ) {
                pDoc->cur_frame_rate[idx] = 10*1000/diff;
                images = 0;
                last_tick_count = tick_count;
            }
            // point to current RGB image for the channel
            pDoc->image[idx] = (unsigned char *)pDoc->m_buf[idx].frame[frm_idx].pdata;
            // do work with image.  EG. if snapshots being taken
            if (pDoc->m_bTriggerSnapshot[idx]) {
                switch (pDoc->m_mode[idx].color & MASK_COLOR) {
                case COLOR_RGB:
                    save_image_uncompressed( pDoc->image[idx], 
                        pDoc->m_strSnapshot[idx].GetBuffer(MAX_PATH),
                        pDoc->m_buf[idx].lpbmi[frm_idx]->bmiHeader.biHeight,
                        pDoc->m_buf[idx].lpbmi[frm_idx]->bmiHeader.biWidth,
                        pDoc->m_buf[idx].lpbmi[frm_idx]->bmiHeader.biWidth*3,
                        0);
                    break;
                case COLOR_Y8:
                    save_image_uncompressed_mono( pDoc->image[idx], 
                        pDoc->m_strSnapshot[idx].GetBuffer(MAX_PATH),
                        pDoc->m_buf[idx].lpbmi[frm_idx]->bmiHeader.biHeight,
                        pDoc->m_buf[idx].lpbmi[frm_idx]->bmiHeader.biWidth);
                    break;
                case COLOR_JPG:
                    {
                        FILE *fout = _tfopen(pDoc->m_strSnapshot[idx].GetBuffer(MAX_PATH), _T("wb+"));
                        if (fout != NULL) {
                            fwrite(pDoc->image[idx], 
                                1, 
                                pDoc->m_buf[idx].lpbmi[frm_idx]->bmiHeader.biSizeImage,
                                fout);
                            fclose(fout);
                        } else {
                            OutputDebugString(_T("could not open file!\n"));
                        }
                    }
                    break;
                }
                pDoc->m_bTriggerSnapshot[idx] = FALSE;
            }
            // draw the image (if displaying the images)
            pView->DrawNewImage(idx, frm_idx);
            // done with buffer, dq it
            S2255_DQBUF( pDoc->m_hdev, chn, pDoc->m_recvd_buf[idx]);
            pDoc->m_recvd_buf[idx]++;
            if((unsigned int) pDoc->m_recvd_buf[idx] == pDoc->m_buf[idx].dwFrames) {
                pDoc->m_recvd_buf[idx] = 0;
            }
        }
    }
    
    // Thread loop exited.  Call S2255_StopAcquire to stop acquisition
    if( S2255_StopAcquire( pDoc->m_hdev, chn ) != 0) {
        //AfxMessageBox("Stop failed");
        OutputDebugString(_T("stop failed!"));
    }
    // Close our stop event
    CloseHandle(pDoc->m_stop_event[idx]);
    // Set event handles to NULL (m_buf_event[idx] closed in StopAcquire)
    pDoc->m_buf_event[idx] = NULL;
    pDoc->m_stop_event[idx] = NULL;
    // free user allocated buffers
    for( i=0;i<pDoc->m_numbuf;i++) {
        pDoc->m_buf[idx].lpbmi[i]->bmiHeader.biSizeImage = 0;
        free( pDoc->m_buf[idx].frame[i].pdata );
        pDoc->m_buf[idx].frame[i].pdata = NULL;
    }
    // Free the acquisition thread parameter
    free(param);
    // Set running state to 0, thread handle to null
    pDoc->m_running[idx] = 0;
    pDoc->m_ack_thread[idx] = NULL;
    InvalidateRect(pView->m_hWnd, &pDoc->m_pos[idx], TRUE);
    // return success
    return 0;
}



// stop acquisition
void Capp2255demoView::OnAcquireStop(UINT nID)
{
    Capp2255demoDoc *pDoc = GetDocument();
    unsigned int idx = nID - ID_ACQUIRE_STOP;
    // register
    if( pDoc == NULL) 
        return;
    
    if (idx >= 4) {
        AfxMessageBox(_T("resource.h IDs must be sequential! failing\n"));
        return;
    }
    
    if( !pDoc->m_running[idx]) {
        AfxMessageBox(_T("S2255: Acquisition thread not running"));
        return;
    }
    SetEvent( pDoc->m_stop_event[idx]);
    // wait for acquisition thread to stop
    WaitForSingleObject( pDoc->m_ack_thread[idx], INFINITE);
    if (pDoc->m_fps_changed[idx]) {
        pDoc->m_mode[idx].fdec = pDoc->m_prev_fdec[idx];
        S2255_SetMode(pDoc->m_hdev, (idx+1), &pDoc->m_mode[idx]);
        pDoc->m_fps_changed[idx] = 0;
    }
    
    pDoc->m_ack_thread[idx] = NULL;
    pDoc->cur_frame_rate[idx] =-1;
    return;
}

//The only reason this function is overriden is to prevent flicker caused by
// painting the background on an active video window.
BOOL Capp2255demoView::OnEraseBkgnd(CDC* pDC)
{   
    CRect rect;
    HRGN rgn;
    int j;
    int width;
    int height;
    Capp2255demoDoc *pDoc = GetDocument();
    GetClientRect(&rect);
    rgn = CreateRectRgn(0,0, rect.Width(), rect.Height());
    // exclude each active draw window
    if (pDoc == NULL) {
        return CView::OnEraseBkgnd(pDC);
    }
    
    for( j = 0; j < MAX_CHANNELS; j++) {
        if( pDoc->m_running[j] ) {
            width = pDoc->m_pos[j].right - pDoc->m_pos[j].left;
            height = pDoc->m_pos[j].bottom - pDoc->m_pos[j].top;
            if ((pDoc->m_mode[j].color & MASK_COLOR) == COLOR_JPG)
                continue;
            if( pDoc->m_mode[j].scale == SCALE_2CIFS) {
                height /= 2;
                width /= 2;
            }
            else if( pDoc->m_mode[j].scale == SCALE_1CIFS) {
                height /=4;
                width /=4;
            }
            ExcludeClipRect( pDC->m_hDC, pDoc->m_pos[j].left, 
                pDoc->m_pos[j].top, 
                pDoc->m_pos[j].left+ width ,
                pDoc->m_pos[j].top+height);
        }
    }
    FillRgn( pDC->m_hDC, rgn, GetSysColorBrush( COLOR_WINDOW));
    return TRUE;
}

// view framerate
void Capp2255demoView::OnViewFramerate()
{
    Capp2255demoDoc *pDoc = GetDocument();
    
    if( pDoc == NULL) {
        return;
    }
    pDoc->m_bViewFR = !pDoc->m_bViewFR;
    Invalidate(TRUE);
}

void Capp2255demoView::OnUpdateViewFramerate(CCmdUI *pCmdUI)
{
    Capp2255demoDoc *pDoc = GetDocument();
    if( pDoc == NULL) {
        return;
    }
    pCmdUI->SetCheck( pDoc->m_bViewFR);
}

void Capp2255demoView::OnUpdateViewDisplayimage(CCmdUI *pCmdUI)
{
    Capp2255demoDoc *pDoc = GetDocument();
    if( pDoc == NULL) {
        return;
    }
    pCmdUI->SetCheck( pDoc->m_bViewImage);
}

void Capp2255demoView::OnViewDisplayimage()
{
    Capp2255demoDoc *pDoc = GetDocument();
    if( pDoc == NULL) {
        return;
    }
    pDoc->m_bViewImage = !pDoc->m_bViewImage;
    Invalidate(TRUE);
}

// initial update.  set the window size to default width,height
void Capp2255demoView::OnInitialUpdate()
{
    Capp2255demoDoc *pDoc = GetDocument();
    ASSERT(pDoc != NULL);
    CView::OnInitialUpdate();
    CRect rect;
    GetWindowRect( &rect);
    CWnd *main = ::AfxGetMainWnd();
    main->SetWindowPos(NULL, 0,0, pDoc->m_pos[0].right,pDoc->m_pos[0].bottom, SWP_NOMOVE);
}

void Capp2255demoView::OnUpdateAcquireStop(CCmdUI *pCmdUI)
{
    Capp2255demoDoc *pDoc = GetDocument();
    unsigned int idx = pCmdUI->m_nID - ID_ACQUIRE_STOP;
    if ((pDoc == NULL) || (idx >= MAX_CHANNELS))
        return;
    pCmdUI->SetCheck(pDoc->m_running[idx] ? 0 : 1);
}


void Capp2255demoView::OnUpdateAcquireStart(CCmdUI *pCmdUI)
{
    Capp2255demoDoc *pDoc = GetDocument();
    unsigned int idx = pCmdUI->m_nID - ID_ACQUIRE_START;
    
    if ((pDoc == NULL) || (idx >= MAX_CHANNELS))
        return;
    pCmdUI->SetCheck( pDoc->m_running[idx] ? 1 : 0);
}



// creates the acquisition thread for capturing and displaying frames
void Capp2255demoView::CreateAcquisitionThread( int channel)
{
    int idx = channel - 1;
    DWORD threadID;
    
    Capp2255demoDoc *pDoc = GetDocument();
    if( pDoc == NULL)
        return;
    
    if( pDoc->m_running[idx]) {
        AfxMessageBox(_T("S2255: Acquisition thread already running"));
        return;
    }
    acq_thread_param_t *prm = (acq_thread_param_t *) malloc( sizeof( acq_thread_param_t ));
    if( prm == NULL) {
        AfxMessageBox(_T("out of memory\n"));
        return;
    }
    prm->channel = idx+1;
    prm->pDoc = pDoc;
    prm->pView = this;
    pDoc->m_running[idx] = 1;
    pDoc->m_ack_thread[idx] = CreateThread(NULL, NULL, &AcquireThread, prm, NULL, &threadID);
}


void Capp2255demoView::OnAcquireStart(UINT nID)
{
    Capp2255demoDoc *pDoc = GetDocument();
    unsigned int idx = nID - ID_ACQUIRE_START;
    BOOL bPAL = FALSE;
    CRect rect;
    CWnd *pWnd = ::AfxGetMainWnd();
    int maxW, maxH;
    
    assert(pDoc != NULL);
    pWnd->GetWindowRect(&rect);
    
    if (idx >= 4) {
        AfxMessageBox(_T("resource.h IDs must be sequential! failing\n"));
        return;
    }
    
    // begin of window resize code
    if( pDoc->m_mode[idx].format == FORMAT_PAL) {
        bPAL = TRUE;
    }
    
    switch (idx) {
    case 0: //channel 1
        maxW = bPAL ? PAL_W + 30 : NTSC_W + 30;
        maxH = bPAL ? PAL_H + 30 : NTSC_H + 30;
        break;
    case 1: //channel 2
        maxW = bPAL ? PAL_W * 2 + 30 : NTSC_W * 2 +30;
        maxH = bPAL ? PAL_H + 30 : NTSC_H + 30;
        break;
    case 2: //channel 3
        maxW = bPAL ? PAL_W + 10: NTSC_W + 10;
        maxH = bPAL ? PAL_H *2 + 30: NTSC_H * 2 + 30;
        break;
    case 3: //channel 4
        maxW = bPAL ? PAL_W * 2 + 30 : NTSC_W * 2 + 30;
        maxH = bPAL ? PAL_H * 2 + 30 : NTSC_H * 2 + 30;
        break;
    default:
        // should not get here
        return;
    }
    if ((rect.Width() <= maxW) || (rect.Height() <= maxH)) {
        // make large enough for all channels
        pWnd->SetWindowPos(NULL, 0, 0,
            (rect.Width() <= maxW) ? maxW : rect.Width(), 
            (rect.Height() <= maxH) ? maxH : rect.Height(),
            SWP_NOMOVE);
        
    }
    // end of window resize code    
    
    // start the acquisition
    CreateAcquisitionThread(idx+1);
    
}


// Called when new settings acquired
// chn 1 - 4 
void Capp2255demoView::OnNewSettings(int chn) 
{
    CPropertySheet sheet;
    CSettings      lvls;
    CFrameRate     rate;
    CFormat        fmt;
    CScale         scl;
    BOOL           bRedraw = FALSE;
    CColor         clr;
    BOOL           bChanged = FALSE;
    int            res;
    BOOL           bALL= FALSE;
    int            idx = 0;
    BOOL           bPAL = FALSE;
    int            j;
    Capp2255demoDoc* pDoc = GetDocument();
    ASSERT_VALID(pDoc);
    if (!pDoc)
        return;
    
    // The following code just gathers all the changed settings and calls S2255_SetMode
    // if anything has changed
    if( chn < 0) {
        int k;
        for (k = 0; k < MAX_CHANNELS; k++) {
            if (pDoc->m_running[k]) {
                AfxMessageBox(_T("You must stop all acquisitions before changing settings for all channels"));
                return;
            }
        }
        bALL = TRUE;
        // work off channel 0 settings
        chn = 1;
    }
    for(j=0;j<MAX_CHANNELS;j++) {
        if( pDoc->m_mode[j].format == FORMAT_PAL) {
            bPAL = TRUE;
        }
    }
    idx = chn - 1;
    sheet.SetTitle(_T("Settings"));

    lvls.m_iBright = (int) pDoc->m_mode[idx].bright;
    lvls.m_iContrast = (int) pDoc->m_mode[idx].contrast;
    lvls.m_iHue = (int) pDoc->m_mode[idx].hue;
    lvls.m_iSat = (int) pDoc->m_mode[idx].saturation;
    
    lvls.pDoc = pDoc;
    lvls.m_channel = chn;
    lvls.mode = pDoc->m_mode[idx];
    
    rate.pDoc = pDoc;
    rate.m_channel = chn;
    rate.mode = pDoc->m_mode[idx];
    
    scl.m_index = (int) (pDoc->m_mode[idx].scale -1);
    fmt.m_index = (int) (pDoc->m_mode[idx].format -1);
    clr.m_index = (int) ((pDoc->m_mode[idx].color & MASK_COLOR) - 1);
    clr.m_iJPG = (pDoc->m_mode[idx].color & MASK_JPG_QUALITY) >> 8;
    // Can't change scale, color or format while running
    // Note: user image memory is allocated in the AcquireThread 
    scl.m_bStreaming = pDoc->m_running[idx];
    clr.m_bStreaming = pDoc->m_running[idx];
    fmt.m_bStreaming = pDoc->m_running[idx];
    rate.m_bStreaming = pDoc->m_running[idx];
    
    if( pDoc->m_mode[idx].fdec != FDEC_5) {
        rate.m_index = (int) (pDoc->m_mode[idx].fdec - 1);
    }
    else {
        rate.m_index = (int) (pDoc->m_mode[idx].fdec - 2);
    }
    
    sheet.AddPage( &lvls);
    sheet.AddPage( &rate);
    sheet.AddPage( &scl);
    sheet.AddPage( &fmt);
    sheet.AddPage( &clr);
    
    res = (int) sheet.DoModal();
    
    if( res == IDOK) {
        int old_jpeg_quality;
        // update changed parameters
        if((unsigned int) lvls.m_iBright != pDoc->m_mode[idx].bright) {
            pDoc->m_mode[idx].bright = (UINT32) lvls.m_iBright;
            bChanged = TRUE;
        }
        if((unsigned int)  lvls.m_iContrast!= pDoc->m_mode[idx].contrast) {
            pDoc->m_mode[idx].contrast = (UINT32) lvls.m_iContrast;
            bChanged = TRUE;
        }
        if((unsigned int)  lvls.m_iHue != pDoc->m_mode[idx].hue) {
            pDoc->m_mode[idx].hue = (UINT32) lvls.m_iHue;
            bChanged = TRUE;
        }
        if((unsigned int)  lvls.m_iSat != pDoc->m_mode[idx].saturation) {
            pDoc->m_mode[idx].saturation = (UINT32) lvls.m_iSat;
            bChanged = TRUE;
        }
        if((unsigned int)  scl.m_index != (pDoc->m_mode[idx].scale - 1)) {
            pDoc->m_mode[idx].scale = (UINT32) (scl.m_index + 1);
            bChanged = TRUE;
            bRedraw = TRUE;
        }
        if((unsigned int)  fmt.m_index != (pDoc->m_mode[idx].format - 1)) {
            pDoc->m_mode[idx].format = (UINT32) (fmt.m_index + 1);
            bChanged = TRUE;
        }
        old_jpeg_quality = (pDoc->m_mode[idx].color & MASK_JPG_QUALITY) >> 8;
        if((unsigned int)  clr.m_index != ((pDoc->m_mode[idx].color & MASK_COLOR) - 1)) {
            pDoc->m_mode[idx].color = (UINT32) (clr.m_index + 1);
            bChanged = TRUE;
        }
        
        if (old_jpeg_quality != clr.m_iJPG) {
            bChanged = TRUE;
        }
        
        pDoc->m_mode[idx].color &= ~ MASK_JPG_QUALITY;
        pDoc->m_mode[idx].color |= (clr.m_iJPG << 8);
        
        if ((unsigned int) rate.m_index != (pDoc->m_mode[idx].fdec - 1)) {
            pDoc->m_mode[idx].fdec = (UINT32) (rate.m_index + 1);
            if( pDoc->m_mode[idx].fdec == 4) {
                pDoc->m_mode[idx].fdec = FDEC_5;
            }
            bChanged = TRUE;
        }
    } else {
        bChanged = lvls.bChanged;
    }
    
    if( !bALL) {
        if( bChanged ) {
            if( S2255_SetMode( pDoc->m_hdev,chn, &pDoc->m_mode[idx]) != 0) {
                AfxMessageBox(_T("Failed to set mode"));
            }
        }
    }
    else {
        for(j=0;j<MAX_CHANNELS;j++) {
            // always change for all channels
            //if( bChanged ) 
            if( res == IDOK)
            { 
                pDoc->m_mode[j] = pDoc->m_mode[0];
                if( S2255_SetMode( pDoc->m_hdev,j+1, &pDoc->m_mode[j]) != 0) {
                    AfxMessageBox(_T("Failed to set mode"));
                }
            }
        }
    }
    if( bChanged) {
        BOOL bPrevPAL = bPAL;
        // search through all channels.  If any pal, use PAL window settings,
        // otherwise use NTSC
        for(j=0;j<MAX_CHANNELS;j++) {
            if( pDoc->m_mode[j].format == FORMAT_PAL) {
                bPAL = TRUE;
            }
        }
        if( bPAL && !bPrevPAL) {
            // switch view positions to PAL
            memcpy( pDoc->m_pos, pDoc->m_posPAL, sizeof(pDoc->m_pos));
            Invalidate(TRUE);
        }
        else if( !bPAL && bPrevPAL) {
            // switch to NTSC
            memcpy( pDoc->m_pos, pDoc->m_posNTSC, sizeof(pDoc->m_pos));
            Invalidate( TRUE);
        } else if( bRedraw) {
            Invalidate( TRUE);
        }
    }
    return;
}

void Capp2255demoView::OnSettingsChannel(UINT nID)
{
    unsigned int idx = nID - ID_SETTINGS_CHANNEL1;
    OnNewSettings(idx + 1);
}


void Capp2255demoView::OnSettingsAllchannels()
{
    OnNewSettings(-1);
}

// start all channels at same time
void Capp2255demoView::OnAcquireStartall()
{
    Capp2255demoDoc *pDoc = GetDocument();
    BOOL bPAL=FALSE;
    int  j;
    int  fullfps = 0; // number of channels at full fps
    CRect rect;
    int idx;
    CWnd *pWnd = ::AfxGetMainWnd();
    if( pDoc == NULL) {
        return;
    }
    pWnd->GetClientRect(&rect);
    // check if trying to start all with full frame rate and color
    // if so, change frame rate to 1/2
    for( j=0; j<MAX_CHANNELS; j++) {
        if( (pDoc->m_mode[j].fdec == FDEC_1 ) && ((pDoc->m_mode[j].color & MASK_COLOR) != COLOR_Y8)  &&
            ((pDoc->m_mode[j].scale == SCALE_4CIFS) || (pDoc->m_mode[j].scale == SCALE_4CIFSI))) {
            fullfps++;
        }
    }
    
    if (fullfps >=2 ) {
        // need to 1/2 the frame rate
        for (j = 0; j < MAX_CHANNELS; j++) {
            pDoc->m_prev_fdec[j] = pDoc->m_mode[j].fdec;
            pDoc->m_fps_changed[j] =1;
            pDoc->m_mode[j].fdec = FDEC_2;
            S2255_SetMode(pDoc->m_hdev, (j+1), &pDoc->m_mode[j]);
        }
    } 
    
    // start all channels
    for (idx = 0;idx < MAX_CHANNELS; idx++) {
        if( pDoc->m_running[idx]) {
            continue;
        }
        OnAcquireStart(ID_ACQUIRE_START + idx);
    }
    
}

// stop all channels
void Capp2255demoView::OnAcquireStopall()
{
    Capp2255demoDoc *pDoc = GetDocument();
    int idx;
    if (pDoc == NULL) 
        return;
    
    for (idx = 0; idx < MAX_CHANNELS; idx++) {
        if( !pDoc->m_running[idx]) {
            continue;
        }
        OnAcquireStop(ID_ACQUIRE_STOP + idx);
    }
    return;
}

void Capp2255demoView::DrawNewImage(int idx, int frm_idx)
{
    Capp2255demoDoc *pDoc = GetDocument();
    CDC* pDC = GetDC();
    BOOL res ;
    HDC hdc = pDC->GetSafeHdc();
    
    // update image if viewing images
    if (pDoc->m_bViewImage) {
        switch (pDoc->m_mode[idx].color & MASK_COLOR) {
        case COLOR_RGB:
            res = SetDIBitsToDevice(hdc, pDoc->m_pos[idx].left, 
                pDoc->m_pos[idx].top, 
                (ULONG) pDoc->m_buf[idx].lpbmi[frm_idx]->bmiHeader.biWidth,
                (ULONG) pDoc->m_buf[idx].lpbmi[frm_idx]->bmiHeader.biHeight,
                0, 
                0, 
                0, 
                (UINT) pDoc->m_buf[idx].lpbmi[frm_idx]->bmiHeader.biHeight,
                pDoc->image[idx],
                pDoc->m_buf[idx].lpbmi[frm_idx], 
                DIB_RGB_COLORS);
            break;
        case COLOR_Y8:
            // StretchDIBits used instead of SetDIBits because for monochrome we need to flip the
            // image(vertical direction)
            res = StretchDIBits( hdc, 
                pDoc->m_pos[idx].left, 
                pDoc->m_pos[idx].top + pDoc->m_buf[idx].lpbmi[frm_idx]->bmiHeader.biHeight - 1, 
                pDoc->m_buf[idx].lpbmi[frm_idx]->bmiHeader.biWidth,
                -pDoc->m_buf[idx].lpbmi[frm_idx]->bmiHeader.biHeight,  //flip Y8 monochrome display
                0,
                0, 
                pDoc->m_buf[idx].lpbmi[frm_idx]->bmiHeader.biWidth,
                pDoc->m_buf[idx].lpbmi[frm_idx]->bmiHeader.biHeight,
                pDoc->image[idx], pDoc->m_buf[idx].lpbmi[frm_idx], 
                DIB_RGB_COLORS, 
                SRCCOPY);
            break;
        case COLOR_JPG:
            TCHAR str[50];
            _stprintf(str, _T("Jpeg size %d"), pDoc->m_buf[idx].lpbmi[frm_idx]->bmiHeader.biSizeImage);
            TextOut(hdc, pDoc->m_pos[idx].left + 25,
                pDoc->m_pos[idx].top + 75,
                str, _tcslen(str));
            
            res = 1;
            break;
        default:
            OutputDebugString(_T("invalid mode\n"));
            res = 1;
        }
        if( res == 0) {
            int err = GetLastError();
            OutputDebugString(_T("failed to set DIBITS\n"));
        }
    }
    // update frame rate if displaying frame rate
    if(( pDoc->cur_frame_rate[idx] != -1) && pDoc->m_bViewFR) {
        TCHAR frame_str[100];
        _stprintf(frame_str, _T("%d fps"), pDoc->cur_frame_rate[idx]);
        pDC->TextOut(pDoc->m_pos[idx].left + 10, pDoc->m_pos[idx].top + 10, frame_str);
    }
    ReleaseDC(pDC);
}

// all code below is not required.  used for debug only


// this function in DLL, but only used for support purposes.  This
// function is not required in your own SDK.
extern "C" int __stdcall S2255_get_fx2fw( HDEVICE hdev, UINT32 *fwver);


// queries state of the buffer.
// returns 0 if unused(free), 1 if currently being filled, 2 if ready, -1 if err

void Capp2255demoView::OnQueryFW(void)
{
    UINT32 fw;
    CString str;
    int res;
    int state;
    TCHAR str_state[50];
    Capp2255demoDoc *pDoc;
    pDoc = (Capp2255demoDoc *) GetDocument();
    res = S2255_get_fx2fw(pDoc->m_hdev, &fw);
    if (res) {
        AfxMessageBox(_T("could not query fw\n"));
    }
    
    // for diagnostics and demonstration only, call S2255_query_buf
    state = S2255_query_buf(pDoc->m_hdev, 1, 0);
    if (state == 0) {
        _tcscpy(str_state, _T("available"));
    } else if (state == 1) {
        _tcscpy(str_state, _T("busy"));
    } else if (state == 2) {
        _tcscpy(str_state, _T("filled"));
    } else {
        _tcscpy(str_state, _T("unknown err"));
    }
    
    str.Format(_T("USB chip: fw version %02d.%02d.  buf_state:%s (chn=1, buf=0)"), (fw >> 8) & 0xff,
                (fw & 0xff),
                str_state);

    AfxMessageBox(str);
    return;
}




