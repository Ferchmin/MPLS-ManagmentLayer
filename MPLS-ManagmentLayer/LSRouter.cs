using System.Timers;
using ControlPlane;

namespace MPLS_ManagmentLayer
{
    class LSRouter
    {
        private string ipAdress;
        private int port;
        private bool isActive;
        public System.Timers.Timer keepAliveTimer;

        public int Port
        {
            get { return port; }
            set { port = value; }
        }
        public string IpAddress
        {
            get { return ipAdress; }
            set { ipAdress = value; }
        }
        public bool IsActive
        {
            get { return isActive; }
        }

        public LSRouter(string ipAdress, int port)
        {
            this.ipAdress = ipAdress;
            this.port = port;
            isActive = true;

            keepAliveTimer = new System.Timers.Timer();
            keepAliveTimer.Elapsed += new System.Timers.ElapsedEventHandler(OnTimedEvent);
            keepAliveTimer.AutoReset = false;
            keepAliveTimer.Interval = 40000;
            keepAliveTimer.Enabled = true;
        }


        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            LogMaker.MakeLog("ERROR - Router: " + ipAdress + " deactivated - keepAlive timeout");
            LogMaker.MakeConsoleLog("ERROR - Router: " + ipAdress + " deactivated - keepAlive timeout");
            isActive = false;
            keepAliveTimer.Stop();
            keepAliveTimer.Close();
        }
    }
}
