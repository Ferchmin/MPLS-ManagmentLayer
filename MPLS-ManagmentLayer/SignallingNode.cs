using ControlPlane;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ControlPlane;

namespace MPLS_ManagmentLayer
{
    class SignallingNode
    {

        #region Variables
        
        public PC _pc
        {
            get; set;
        }       
        private CPCC _cpcc;
        public string _configurationFolderPath;
        static string logRegisterFilepath = "mleko.txt";
        static int logId = 1;
        public struct CallParametres
        {
            public string _callingId
            {
                get; set;
            }
            public string _calledId
            {
                get; set;
            }
            public int _labelIN
            {
                get; set;
            }
            public int _capacity
            {
                get; set;
            }
        }
        #endregion


        #region Configuration_Methodes
        public SignallingNode(string configurationFolderPath)
        {
            _configurationFolderPath = configurationFolderPath;
            InitializeSignallingModules();
        }
        private void InitializeSignallingModules()
        {
            // DeviceClass.MakeLog("|SIGNALLING| NodeDevise is waking up...");
            // DeviceClass.MakeConsoleLog("|SIGNALLING| NodeDevise is waking up...");
            WriteLogs("Signalling node has been initialized");
            // _pc = new PC(_configurationFolderPath + "/PC1_config.xml");
            _cpcc = new CPCC(_configurationFolderPath + "/CPCC1_config.xml");
             _pc = new PC(_configurationFolderPath + "/PC1_config.xml", _cpcc);
            _cpcc.LocalPC = _pc;
           
            StartWorking();
        }
        #endregion

        #region Start_Stop_Methodes
        private void StartWorking()
        {
            //DeviceClass.MakeLog("|SIGNALLING| NodeDevise is working...");
            //DeviceClass.MakeConsoleLog("|SIGNALLING| NodeDevise is working...");
        }
        private void StopWorking(string reason)
        {
           // DeviceClass.MakeLog("|SIGNALLING| NodeDevise is working...");
        }
        #endregion

        #region Signalling_Log_Methodes
        public static void MakeSignallingLog(string moduleName, string logMessage)
        {
            //DeviceClass.MakeLog("|SIGNALLING - " + moduleName + "| " + logMessage);
        }
        public static void MakeSignallingConsoleLog(string moduleName, string logMessage)
        {
            //DeviceClass.MakeConsoleLog("|SIGNALLING - " + moduleName + "| " + logMessage);
        }
        #endregion

        public void InitiateCall(string senderName ,string receiverName, int callID, int calledCapacity)
        {
            _cpcc.CallRequest(callID, senderName, receiverName, calledCapacity);          
        }

        public void CallReleaseInitation(int callId)
        {
            
            _cpcc.CallRelease(callId);
        }

       

       
        //Method, which writes log to a file
        static void WriteLogs(string logDescription)
        {
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(logRegisterFilepath, true))
            {

                file.WriteLine(logId + " " + "|" + " " + DateTime.Now.ToString("hh:mm:ss") + " INFO: " + logDescription);
                ++logId;
            }
        }






    }
}

