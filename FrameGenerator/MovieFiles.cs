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
using System.IO;


namespace FrameGeneratorLib
{
    public class MovieFiles
    {

        public MovieFiles(APPLICATION_DATA appData)
        {
            m_AppData = appData;
            m_AppData.AddOnClosing(OnClose, APPLICATION_DATA.CLOSE_ORDER.MIDDLE);
            m_Log = (ErrorLog)m_AppData.Logger;
            MovieControlSingleton = new object();
            m_ProcessThread = new Thread(ProcessLoop);
            m_PlayCommandsQ = new ThreadSafeQueue<MOVIE_PLAY_CONTROL_OBJECT>(20);
            m_NewFramesToPushQ = new ThreadSafeQueue<FRAME>(240);
            m_PushFramesToCosumersThread = new Thread(PushFramesToConsumersLoop);

            m_MasterFileList = new ThreadSafeHashTable(1000);


            vChans = new VIRTUAL_CHANNEL[m_AppData.MAX_VIRTUAL_CHANNELS];
          
         
        }

        VIRTUAL_CHANNEL[] vChans;

        ThreadSafeHashTable m_MasterFileList; // keeps track of which virtual channel the file was assigned to


        ThreadSafeQueue<FRAME> m_NewFramesToPushQ;
        Thread m_PushFramesToCosumersThread;

        ErrorLog m_Log;
        APPLICATION_DATA m_AppData;
        object MovieControlSingleton;
        Thread m_ProcessThread;
        ThreadSafeQueue<MOVIE_PLAY_CONTROL_OBJECT> m_PlayCommandsQ;

        public delegate void OnNewImageEvent(FRAME frame);
        public event OnNewImageEvent OnNewImage;   // allow consumers to register to receive a new image extracted from the video file

        public delegate void OnStatusUpdateFromPlayer(string file, VIRTUAL_CHANNEL.PLAY_STATUS_UPDATE status, VIRTUAL_CHANNEL.PlayNextFileDelegate playnext);
        public event OnStatusUpdateFromPlayer OnStatusUpdateFromPlayerEvent;

        public enum MOVIE_PLAY_COMMANDS:int { Load, Play, PlayAll, Stop}
        public class MOVIE_PLAY_CONTROL_OBJECT
        {
            public MOVIE_PLAY_COMMANDS command;
            public string[] filesToPlay;
        }

        Panel[] m_DisplayPanels;
        public Panel[] DisplayPanels
        {
            set
            {
                lock (MovieControlSingleton)
                {
                    
                    m_DisplayPanels = value;

                    // create a player for each virtual channel and assign it a panel to diplay on
                    for (int c = 0; c < m_AppData.MAX_VIRTUAL_CHANNELS;c++)
                    {
                        vChans[c] = new VIRTUAL_CHANNEL(m_AppData, c, m_DisplayPanels[c], m_NewFramesToPushQ);
                        vChans[c].OnStatusUpdate += new VIRTUAL_CHANNEL.OnStatusUpdateEvent(MovieFiles_OnStatusUpdate);
                    }
                }
            }
        }

        void MovieFiles_OnStatusUpdate(string file, MovieFiles.VIRTUAL_CHANNEL.PLAY_STATUS_UPDATE status, MovieFiles.VIRTUAL_CHANNEL.PlayNextFileDelegate playnext)
        {
            if (OnStatusUpdateFromPlayerEvent != null) OnStatusUpdateFromPlayerEvent(file, status, playnext);
        }

        bool m_Stop = false;
        void OnClose()
        {
            m_Stop = true;
        }


        bool m_StopPlay = false;

        public void Stop()
        {
            m_StopPlay = true;

            foreach (VIRTUAL_CHANNEL vc in vChans)
            {
                vc.Stop();
              
            }
        


            foreach (VIRTUAL_CHANNEL vc in vChans)
            {
               
                vc.Close();
              
            }
    
        

            foreach (VIRTUAL_CHANNEL vc in vChans)
            {
               
                vc.Clear();
            }

            m_StopPlay = false;
        }

        public void Start()
        {
            m_ProcessThread.Start();
            m_PushFramesToCosumersThread.Start();
        }

    
        void LoadFiles(MOVIE_PLAY_CONTROL_OBJECT command)
        {
            // distribute the files over the virtual channels
            int c = 0;
            for (c = 0; c < m_AppData.MAX_VIRTUAL_CHANNELS; c++)
            {
                vChans[c].Clear();
            }

            c = 0;
            foreach (string file in command.filesToPlay)
            {
                vChans[c].AddFile(file);
                m_MasterFileList.Add(file, c);
                c++;
                if (c >= m_AppData.MAX_VIRTUAL_CHANNELS) c = 0;

                if (m_Stop) break;
            }
        }
       
     
        public void LoadFiles(string[] files)
        {
            if (files.Length > m_AppData.MAX_MOVIE_FILES_TO_LOAD)
            {
                MessageBox.Show("too many files to load, max number of files is: " + m_AppData.MAX_MOVIE_FILES_TO_LOAD.ToString());
                return;
            }

            lock (MovieControlSingleton)
            {
                MOVIE_PLAY_CONTROL_OBJECT command = new MOVIE_PLAY_CONTROL_OBJECT();
                command.command = MOVIE_PLAY_COMMANDS.Load;
                command.filesToPlay = files;
                m_PlayCommandsQ.Enqueue(command);
             }
        }

        void PlayAll()
        {
            foreach (VIRTUAL_CHANNEL vc in vChans)
            {

                Application.DoEvents();
                
                if (m_StopPlay) return;// user interrrupted the start process

                vc.PlayNext();
            }
        }

        public void Play()
        {

            lock (MovieControlSingleton)
            {

                // crazy windows architecture - you get an illegal cross thread op if you try and start a directshow player on a panel
                // from the UI from any other thread. so do it here and not on the process thread (directshow players run on their own internal threads anyway

                   PlayAll();

                //MOVIE_PLAY_CONTROL_OBJECT command = new MOVIE_PLAY_CONTROL_OBJECT();
                //command.command = MOVIE_PLAY_COMMANDS.PlayAll;
        
                //m_PlayCommandsQ.Enqueue(command);
                
            }
        }



        void PushFramesToConsumersLoop()
        {
            while (!m_Stop)
            {
                Thread.Sleep(1);

                FRAME frame = m_NewFramesToPushQ.Dequeue();

                if (frame != null)
                {
                    OnNewImage(frame);
                }


            }
        }

        public void StopPlay()
        {
            lock (MovieControlSingleton)
            {
                MOVIE_PLAY_CONTROL_OBJECT command = new MOVIE_PLAY_CONTROL_OBJECT();

                command.command = MOVIE_PLAY_COMMANDS.Stop;
                m_PlayCommandsQ.Enqueue(command);
            }
        }

        void ProcessLoop()
        {
            while (!m_Stop)
            {
                Thread.Sleep(1);

                MOVIE_PLAY_CONTROL_OBJECT command = m_PlayCommandsQ.Dequeue();

                if (command == null) continue;

                switch (command.command)
                {
                    case MOVIE_PLAY_COMMANDS.Stop:

                        break;

                  
                        // crazy windows architecture - you get an illegal cross thread op if you try and start a directshow player on a panel
                        // from the UI from any other thread.

                    //case MOVIE_PLAY_COMMANDS.PlayAll:
                    //    PlayAll();
                    //    break;

                    case MOVIE_PLAY_COMMANDS.Load:
                        LoadFiles(command);
                    
                        break;

                }
            }
        }



        public class VIRTUAL_CHANNEL
        {
            public VIRTUAL_CHANNEL(APPLICATION_DATA appData, int chan, Panel panel, ThreadSafeQueue<FRAME> NewFramesToPushQ)
            {
                AppData = appData;
                singleton = new object();
                Chan = chan;
                Panel = panel;
                FramesToPushQ = NewFramesToPushQ;
                FilesToPlay = new List<string>();
            }
            Panel Panel;
            int Chan;
            APPLICATION_DATA AppData;
            public List<string> FilesToPlay;
            
            public int CurrentFileIndex;
            ThreadSafeQueue<FRAME> FramesToPushQ;
            public enum PLAY_STATUS_UPDATE { PLAYING, COMPLETED, STOPPED }

            public delegate void PlayNextFileDelegate();

            public delegate void OnStatusUpdateEvent(string file, PLAY_STATUS_UPDATE status,PlayNextFileDelegate playnext);
            public event OnStatusUpdateEvent OnStatusUpdate;
            object singleton;

            public void Clear()
            {
                lock (singleton)
                {
                    FilesToPlay = new List<string>();
                    CurrentFileIndex = 0;
                    Playing = false;
                   
                    Stop();
                    Close();

                    AllFilesDone = false;
                }
            }

            public void AddFile(string file)
            {
                lock (singleton)
                {
                    FilesToPlay.Add(file);
                }
            }

            IFilePlayerInterface FileDecoder;
            bool Playing = false;
            bool AllFilesDone = false;
            public void PlayNext()
            {
                lock (singleton)
                {
                    if (Playing || AllFilesDone) return;

                    if (FilesToPlay.Count < 1) return;

                    PlayFile(FilesToPlay[CurrentFileIndex]);
                    CurrentFileIndex++;
                    if (CurrentFileIndex == FilesToPlay.Count)
                    {
                        CurrentFileIndex = 0;
                        AllFilesDone = true;
                    }
                }
            }


            public void Stop()
            {
                lock (singleton)
                {
                    AllFilesDone = true;

                    if (FileDecoder == null) return;

                    FileDecoder.StopGraph();
                
                }
            }

            public void Close()
            {
                if (FileDecoder == null) return;

                FileDecoder.Dispose();
                FileDecoder = null;

            }


            void PlayFile(string file)
            {
                lock (singleton)
                {
                    if (AllFilesDone) return;

                    try
                    {
                       
                        // is this a directory of jpegs or a video file?
                        if (Directory.Exists(file))
                        {
                            // this is a directory of jpegs
                            FileDecoder = new PlayJpegsInBatchMode(AppData, Panel, (Form)AppData.MoviePlayerParentForm, Chan);
                        }
                        else
                        {
                            FileDecoder = new DSFileDecoder(AppData, Panel, (Form)AppData.MoviePlayerParentForm, Chan);
                        }

                        FileDecoder.OnNewFrame += new FrameGeneratorLib.OnNewFrameEvent(fileDecoder_OnNewFrame);
                        FileDecoder.OnEndOfFileEvent += new FrameGeneratorLib.OnEndOfFile(fileDecoder_OnEndOfFileEvent);
                        FileDecoder.Start(file);

                        if (OnStatusUpdate != null)
                            OnStatusUpdate(file, PLAY_STATUS_UPDATE.PLAYING, null);

                        Playing = true;
                    }
                    catch (Exception ex) { ((ErrorLog)AppData.Logger).Trace(ex, ErrorLog.LOG_TYPE.FATAL); }
                }
            }

           

            void fileDecoder_OnEndOfFileEvent()
            {
                ///lock (singleton)  // bad bad, causes thread dead lock since this gets called on the DS thread, which is connected to the UI thread
                {
                    Playing = false;

                    FileDecoder.StopGraph();

                    int indexOfLastFile = CurrentFileIndex - 1; ;
                    if (indexOfLastFile < 0) indexOfLastFile = 0;

                    if (OnStatusUpdate != null)
                        OnStatusUpdate(FilesToPlay[indexOfLastFile], PLAY_STATUS_UPDATE.COMPLETED, PlayNext);

                  
                }

            }

           

            void fileDecoder_OnNewFrame(FRAME frame)
            {
               // lock (singleton)
                {
                    FramesToPushQ.Enqueue(frame);
                }
            }

          
        }
    }
}
