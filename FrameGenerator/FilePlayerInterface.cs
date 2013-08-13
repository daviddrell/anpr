using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ApplicationDataClass;
using S2255Controller;
using GPSLib;
using System.Threading;
using ErrorLoggingLib;
using System.Drawing;
using Utilities;
using LPROCR_Wrapper;
using UserSettingsLib;
using System.Collections;
using System.Windows.Forms;


namespace FrameGeneratorLib
{


    public delegate void OnNewFrameEvent(FRAME frame);
    public delegate void OnEndOfFile();

    /// <summary>
    /// this interface allows me to build a DirectShow video file player or a jpeg slide show player that both act like the same video source.
    /// </summary>
    public interface IFilePlayerInterface
    {
       

        void Dispose();

        void StopGraph();

        void Start(string file);
        void Start(string[] files);

        event OnNewFrameEvent OnNewFrame;       
        event OnEndOfFile OnEndOfFileEvent;
    }

}
