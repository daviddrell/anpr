using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using LPRServiceCore;

namespace LPRService
{
    public partial class LPRServiceContainer : ServiceBase
    {
        public LPRServiceContainer()
        {
            InitializeComponent();
            LPRServiceCore = new LPRServiceEntryPoint();

        }

        LPRServiceEntryPoint LPRServiceCore;

        protected override void OnStart(string[] args)
        {
            LPRServiceCore.Start(true);
        }

        protected override void OnStop()
        {
            LPRServiceCore.Stop();
        }
    }
}
