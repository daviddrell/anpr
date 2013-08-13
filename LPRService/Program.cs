using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace LPRServiceCore
{
    class Program
    {
        static void Main(string[] args)
        {
            LPRServiceCore.LPRServiceEntryPoint m_LPRService;
            
            m_LPRService = new LPRServiceEntryPoint();
            
           
            Console.CancelKeyPress += delegate(object sender, ConsoleCancelEventArgs e) {  m_LPRService.Stop(); };// shutdown all threads on control-c event

            m_LPRService.OnSelfDestruct += Stop;
         
            m_LPRService.Start(true);


            while (! m_Stop)
            {
                Thread.Sleep(1);/// the console app stays alive until a control-C event kills this thread
            }

          
            
        }

        static bool m_Stop = false;
        static void Stop()
        {
            m_Stop = true;
        }

        





        // service controls
        //protected override void OnStart(string[] args)
        //{
        //    // create or open the file. Default path is "C:\windows\System32\"
        
        //}

        //protected override void OnStop()
        //{

        //}

    }
}
