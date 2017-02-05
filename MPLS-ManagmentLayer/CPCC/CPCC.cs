using System.Collections.Generic;
using System.Linq;
using DTO.ControlPlane;
using MPLS_ManagmentLayer;

namespace ControlPlane
{
    class CPCC
    {

        #region Variables_from_configuration_file
        public string _configurationFilePath { get; set; }
        public string _localHostName { get; set; }
        public string _localPcIpAddress { get; set; }
        public string _domainNccIpAddress { get; set; }
        public List<int> FreeLabelList { get; set; }
        #endregion

        #region Dictionary with Call Parametres 
        public struct CallParametres
        {
            public string _callingId { get; set; }                    
            public string _calledId { get; set; }              
            public int _labelIN { get; set; }        
            public int _capacity { get; set; }              
        }
        public Dictionary<int, CallParametres> CallParametresDictionary = new Dictionary<int, CallParametres>();
        #endregion

        #region Variables_to_signalling_message
        //Numer porządkowy Calla
        public int _callID { get; set; }
        //Nazwa urządzenia proszącego o zestawienie połączenia
        public string _callingID { get; set; }
        // Nazwa urządzenia z którym chcemy się połączyć
        public string _calledID { get; set; }
        //Pojemność jaką chcemy zestawić w naszym połączeniu
        public int _callingcapacity { get; set; }
        //Potwierdzenie wysyłane przez CPCC jeśli jest zgoda na połączenie
        public bool _confirmation { get; set; }
        //Label z jakim należy wysłać
        public int _labelIN { get; set; }
        public PC _pc;
        public SignalMessage sm;
        public List<int> CallIdList { get; set; }
        public List<int> LabelInList { get; set; }
        public Dictionary<int, string> IdCalledIdDictionary;
        public List<int> LabelPool {
            get; set; }
        public Dictionary<int, string> IdLabelDictionary
        {
            get; set;
        }
        #endregion

        #region Properties
        public PC LocalPC { set { _pc = value; } }
        #endregion
        #region Main_Methodes

        public CPCC(string configurationFilePath)
        {
            InitialiseVariables(configurationFilePath);
            CallIdList = new List<int>();
            LabelInList = new List<int>();
            IdCalledIdDictionary = new Dictionary<int, string>();
            IdLabelDictionary = new Dictionary<int, string>();     
        }


        private void InitialiseVariables(string configurationFilePath)
        {
            _configurationFilePath = configurationFilePath;
            CPCC_XmlSchame tmp = new CPCC_XmlSchame();
            tmp = CPCC_LoadingXmlFile.Deserialization(_configurationFilePath);

            _localHostName = tmp.XML_localHostName;
            _localPcIpAddress = tmp.XML_localPcIpAddress;
            _domainNccIpAddress = tmp.XML_domainNccIpAddress;
            FreeLabelList = tmp.XML_FreeLabelList;
        }
        #endregion

        #region Methodes_From_Standarization
        public void CallRequest(int _callID, string _callingID, string _calledID, int _callingcapacity)
        {
            sm = new SignalMessage();      
            sm.General_SignalMessageType = SignalMessage.SignalType.CallRequest;
            sm.General_DestinationIpAddress = _domainNccIpAddress;
            sm.General_SourceIpAddress = _localPcIpAddress;
            sm.General_SourceModule = "CPCC";
            sm.General_DestinationModule = "NCC";

            sm.CallID = _callID;
            sm.CallingID = _callingID;
            sm.CalledID = _calledID;
            sm.CallingCapacity = _callingcapacity;
            SendMessageToPC(sm);
            CallParametresDictionary.Add(sm.CallID, new CallParametres() {_callingId = sm.CallingID, _calledId = sm.CalledID, _capacity = sm.CallingCapacity});
        }

        public void CallIndication(int _callID, string _callingID, string _calledID, int _callingcapacity)
        {
            sm = new SignalMessage();
            sm.General_SignalMessageType = SignalMessage.SignalType.CallIndication;
            sm.General_DestinationIpAddress = _localPcIpAddress;
            sm.General_SourceIpAddress = _domainNccIpAddress;
            sm.General_SourceModule = "NCC";
            sm.General_DestinationModule = "CPCC";

            sm.CallID = _callID;
            sm.CallingID = _callingID;
            sm.CalledID = _calledID;
            sm.CallingCapacity = _callingcapacity;
        }

        public void CallAccept(int _callID, bool _confirmation, int _labelIN, int _callingcapacity)
        {
            sm = new SignalMessage();
            //sm.LinkConnection_AllocatedSnpAreaNameList.Add("roe");

            sm.General_SignalMessageType = SignalMessage.SignalType.CallAccept;
            sm.General_DestinationIpAddress = _domainNccIpAddress;
            sm.General_SourceIpAddress = _localPcIpAddress;
            sm.General_SourceModule = "CPCC";
            sm.General_DestinationModule = "NCC";

            sm.CallID = _callID;
            sm.Confirmation = _confirmation;
            sm.CallingCapacity = _callingcapacity;
            if (_confirmation == true)
                try
                { sm.LabelIN = FreeLabelList.Last();
                    FreeLabelList.Remove(FreeLabelList.Count);
                }
                catch
                {
                    sm.Confirmation = false;
                  
                }

            SendMessageToPC(sm);
            CallParametresDictionary.Add(sm.CallID, new CallParametres() { _callingId = sm.CallingID, _labelIN = sm.LabelIN });
        }

        public void CallModificationIndication(int modificationID, int _callID, int _labelOUT)
        {

        }

        public void CallModificationAccept(int _modificationID)
        {

        }

        public void CallRelease(int _callID)
        {
            sm = new SignalMessage();
            sm.General_SignalMessageType = SignalMessage.SignalType.CallRelease;
            sm.General_DestinationIpAddress = _domainNccIpAddress;
            sm.General_SourceIpAddress = _localPcIpAddress;
            sm.General_SourceModule = "CPCC";
            sm.General_DestinationModule = "NCC";

            sm.CallID = _callID;
       }


        public void CallAccept_Analize(SignalMessage message)
        {
            if (message.Confirmation)
            {
                LogMaker.MakeConsoleLog("Zestawiono polaczenie, etykieta wejsciowa: " + message.LabelIN);
                LogMaker.MakeLog("Zestawiono polaczenie, etykieta wejsciowa: " + message.LabelIN);
            }
            else
            {
                LogMaker.MakeConsoleLog("Polaczenie odrzucone");
                LogMaker.MakeLog("Polaczenie odrzucone");
            }
        }

        #endregion

        #region Proceeding Methods
        private void CallIndicationProceeding(SignalMessage message)
        {
            var req = "Do u want to accept CallRequest from " + message.CallingID + "?";
            var title = "CallIndication  was received";
            /*var result = MessageBox.Show(
                req,
                title,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            switch (result)
            {
                case DialogResult.Yes:
                    message.Confirmation = true;
                    break;
                case DialogResult.No:
                    message.Confirmation = false;
                    break;
            }*/
           CallAccept(message.CallID, message.Confirmation, message.LabelIN, message.CallingCapacity);
        }

        private void CallReleaseProceeding(SignalMessage message)
        {
            CallParametres cp = new CallParametres();
            cp = CallParametresDictionary[message.CallID];
            FreeLabelList.Add(cp._labelIN);
            CallParametresDictionary.Remove(message.CallID);
        }

        private void ConnectionResponseProceeding(SignalMessage message)
        {
            CallParametres cp = new CallParametres();
            cp = CallParametresDictionary[message.CallID];
            cp._labelIN = message.LabelIN;


            CallParametresDictionary.Remove(message.CallID);
            CallParametresDictionary.Add(message.CallID, cp);

            InteractionClass intClas = new InteractionClass();
            intClas.SendLabelNumber(cp._labelIN, message.CalledID, "127.0.1.101");

        }

        #endregion
        #region PC_Cooperation_Methodes
        private void SendMessageToPC(SignalMessage message)
        {
            _pc.SendSignallingMessage(message);
        }

        public void ReceiveMessageFromPC(SignalMessage message)
        {
            switch (message.General_SignalMessageType)
            {
                case SignalMessage.SignalType.CallRequest:
                    CallRequest(message.CallID, message.CallingID, message.CalledID, message.CallingCapacity);     
                    break;
                case SignalMessage.SignalType.CallIndication:               
                    CallIndicationProceeding(message);             
                    break;                           
                case SignalMessage.SignalType.ConnectionResponse:
                    ConnectionResponseProceeding(message);
                    break;
                case SignalMessage.SignalType.CallRelease:
                    CallReleaseProceeding(message);
                    break;
                case SignalMessage.SignalType.CallAccept:
                    CallAccept_Analize(message);
                    break;
            }
        }


        
        #endregion

    }
}

