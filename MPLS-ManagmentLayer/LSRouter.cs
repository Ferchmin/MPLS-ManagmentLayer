﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace MPLS_ManagmentLayer
{
    class LSRouter
    {
        private string ipAdress;
        private bool isActive;

        public System.Timers.Timer keepAliveTimer;

        public string IpAddress
        {
            get { return ipAdress; }
            set { ipAdress = value; }
        }

        public bool IsActive
        {
            get { return isActive; }
        }

        public LSRouter(string ipAdress)
        {
            this.ipAdress = ipAdress;
            isActive = true;

            keepAliveTimer = new System.Timers.Timer();
            keepAliveTimer.Elapsed += new System.Timers.ElapsedEventHandler(OnTimedEvent);
            keepAliveTimer.AutoReset = true;
            keepAliveTimer.Interval = 2700000;
            keepAliveTimer.Enabled = true;

        }


        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            LogMaker.MakeLog("Router deactivated - keepAlive timeout");
            isActive = false;
       
        }
    }
}
