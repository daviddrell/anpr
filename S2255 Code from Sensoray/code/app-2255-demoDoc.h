// Copyright Sensoray Company 2007.
// @author: D.A.
// app-2255-demoDoc.h : interface of the Capp2255demoDoc class
//

#pragma once

#include "s2255.h"

// NUM_FRAMES is the number of user buffers.
// Can be changed up to a maximum of SYS_FRAMES, should be at least 2.
// For low latency display applications, use a minimal number of frames.
// For applications where all frames must be captured, use a higher number.
#define NUM_FRAMES 2

#define WINDOW_X 20				// view offset
#define WINDOW_Y 30				// view offset
#define NTSC_W 640
#define PAL_W  704
#define NTSC_H 480
#define PAL_H  (288*2)



class Capp2255demoDoc : public CDocument
{
protected: // create from serialization only
	Capp2255demoDoc();
	DECLARE_DYNCREATE(Capp2255demoDoc)
// Attributes
public:
    // 2255 structures and data
    BUFFER m_buf[MAX_CHANNELS];        // buffer structure per channel
    HANDLE m_hdev;                     // device handle
    HANDLE m_ack_thread[MAX_CHANNELS]; // acquisition thread
    HANDLE m_buf_event[MAX_CHANNELS];  // buffer full
    HANDLE m_stop_event[MAX_CHANNELS]; // stop event for channel
    BOOL   m_running[MAX_CHANNELS]; // if running
    unsigned char   *image[MAX_CHANNELS];
    int    m_recvd_buf[MAX_CHANNELS];
    MODE2255 m_mode[MAX_CHANNELS];
	int     m_prev_fdec[MAX_CHANNELS];	 /* previous fdec */
	int     m_fps_changed[MAX_CHANNELS]; /* fps changed */
    int     m_numbuf; // num user buffer used in driver
    int     cur_frame_rate[MAX_CHANNELS]; //to keep track of the calculated frame rate

    BOOL    m_bViewFR;
    BOOL    m_bViewImage;
    UINT32  m_framecount;

    // view positions
    RECT    m_posNTSC[MAX_CHANNELS];
    RECT    m_posPAL[MAX_CHANNELS];
    RECT    m_pos[MAX_CHANNELS];

    BOOL    m_bTriggerSnapshot[MAX_CHANNELS];
    CString m_strSnapshot[MAX_CHANNELS];
// Operations
public:
    // Overrides
	public:
	virtual BOOL OnNewDocument();
	virtual void Serialize(CArchive& ar);
    void OnCloseDocument();
// Implementation
public:
	virtual ~Capp2255demoDoc();
#ifdef _DEBUG
	virtual void AssertValid() const;
	virtual void Dump(CDumpContext& dc) const;
#endif
protected:
// Generated message map functions
protected:
	DECLARE_MESSAGE_MAP()
public:
	void DocSnapshotCommon(int idx);
    afx_msg void OnSnapshotsChannel1();
    afx_msg void OnSnapshotsChannel2();
    afx_msg void OnSnapshotsChannel3();
    afx_msg void OnSnapshotsChannel4();
    int SingleSnapshot( CString fname, int idx);
	afx_msg void OnToolsVideostatus();
};


