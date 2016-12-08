using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
/*
 * Klasa odpowiedzialna za zarządanie protokołem komunikacyjnym oraz tworzeniem logów
 * - zapisujemy tutaj całą metodyke działania protokołu
 * (tutaj sprowadza się to jedynie do stworzenia talicy dostepnych opcji menu
 * a nastepnie sprecyzowania odpowiednich komend, które będą przetwarzane w agencie zarzadzania w urzadzeniu)
 * - jeżeli otrzymamy dane powrotne, to musimy je tutaj przeanalizować, czy są dobre i pozwolić użytkownikowi 
 * na dalsze polecenia, czy są złe i wymusić ponowne wpisanie komendy z poprawnymi parametrami
 * 
 * 
*/
namespace MPLS_ManagmentLayer
{
    class ManagementClass
    {
        /*
         * Zmienne lokalne:
         * - LogFilePath, ścieżka do pliku z logami
         * - logID, identyfikator logów
        */

        public ConfigurationClass configurationBase;
        public PortsClass portsCommunication;

        public LogMaker logMaker;


        public string LogFilePath { get; private set; }
        private int logID;


        /*
         * Konstruktor obiektu
         * - wyzerowanie licznika logów
        */
        public ManagementClass()
        {
            configurationBase = new ConfigurationClass();
            portsCommunication = new PortsClass(configurationBase);
            logMaker = new LogMaker();

            LogMaker.MakeLog("Managment agent is online");

            logID = 0;
        }

        /*
         * Metoda odpowiedzialna za analizowanie poprawności komendy z protokołem
         * - metoda weryfikuje również, czy np nie wywołaliśmy komendy dla nieistniejącego w tablicy aktywnych
         *  elementów sieci węzła;
         * - jeżeli komenda jest prawidłowa to metoda zwraca true i przesyła komendę do wysłania do klasy PortsClass
         * - jezeli komenda jest wadliwa, zwraca false, dzięki czemu w obiekt wywołujący (interactionclass) będzie wiedział,
         *  że musi napisąć worning i poprosić o ponownę wpisanie komendy
        */
        public bool AnalyseCommand(string command)
        {
            return true;
        }

        public void SendAddCommand()
        {
            IPEndPoint agentEndPoint = ChooseTargetRouter();

            if (agentEndPoint == null)
            {
                LogMaker.MakeLog("Failed to send ADD command - wrong router selected or no router availible");
            }
            else
            {

                Console.WriteLine("Set LabelIn: ");
                int tableLine = Int32.Parse(Console.ReadLine());

                Console.WriteLine("State the InterfaceIn: ");
                int inPort = Int32.Parse(Console.ReadLine());

                Console.WriteLine("State the labelOut: ");
                int outPort = Int32.Parse(Console.ReadLine());

                string packetMessage = "Add " + tableLine.ToString() + " " + inPort.ToString() + " " + outPort.ToString() + " swap";

                ManagementPacket commandPacket = new ManagementPacket();
                commandPacket.IpSource = portsCommunication.MyIPAddress.ToString();
                commandPacket.IpDestination = agentEndPoint.Address.ToString();
                commandPacket.DataIdentifier = 2;
                commandPacket.Data = packetMessage;
                commandPacket.MessageLength = (ushort)(Encoding.ASCII.GetBytes(packetMessage).Length);


                portsCommunication.SendMyPacket(commandPacket.CreatePacket(), agentEndPoint);

                LogMaker.MakeLog("Sent ADD command to " + agentEndPoint.Address.ToString());
            }
        }

        public IPEndPoint ChooseTargetRouter()
        {
            int idRange = ShowClientList();

            if(idRange == 0)
            {
                return null;
            }

            IPAddress ipAddress;
            int port;

            Console.WriteLine("Choose router by typing ID: ");
            int destinationRouterId = Int32.Parse(Console.ReadLine());

            if(destinationRouterId <= idRange)
            {
                if (portsCommunication.ConnectedRouters[destinationRouterId].IsActive)
                {
                    ipAddress = IPAddress.Parse(portsCommunication.ConnectedRouters[destinationRouterId].IpAddress);
                    port = portsCommunication.ConnectedRouters[destinationRouterId].Port;
                    return new IPEndPoint(ipAddress, port);

                }
                else
                {
                    Console.WriteLine("Router is no longer active");
                    return null;
                }
            
            }
            Console.WriteLine("Wrong router selected");
            return null;
        }

        public void SendRemoveCommand()
        {
            IPEndPoint agentEndPoint = ChooseTargetRouter();

            if (agentEndPoint == null)
            {
                LogMaker.MakeLog("Failed to send REMOVE command - wrong router selected");
            }
            else
            {

                Console.WriteLine("Set line: ");
                int tableLine = Int32.Parse(Console.ReadLine());

                Console.WriteLine("State the IN port: ");
                int inPort = Int32.Parse(Console.ReadLine());

                Console.WriteLine("State the OUT port: ");
                int outPort = Int32.Parse(Console.ReadLine());

                string packetMessage = "REMOVE " + tableLine.ToString() + " " + inPort.ToString() + " " + outPort.ToString();

                ManagementPacket commandPacket = new ManagementPacket();
                commandPacket.IpSource = portsCommunication.MyIPAddress.ToString();
                commandPacket.IpDestination = agentEndPoint.Address.ToString();
                commandPacket.DataIdentifier = 2;
                commandPacket.Data = packetMessage;
                commandPacket.MessageLength = (ushort)(Encoding.ASCII.GetBytes(packetMessage).Length);


                portsCommunication.SendMyPacket(commandPacket.CreatePacket(), agentEndPoint);

                LogMaker.MakeLog("Sent REMOVE command to " + agentEndPoint.Address.ToString());
            }
        }

        public int ShowClientList()
        {

            if (portsCommunication.ConnectedRouters.Count == 0)
            {
                Console.WriteLine(" - No clients connected");
                return 0;
            }
            else
            {
                Console.WriteLine(" - Number of availible clients: " + portsCommunication.ConnectedRouters.Count);
                int i = 0;
                foreach (LSRouter client in portsCommunication.ConnectedRouters)
                {
                    if (client.IsActive)
                    {
                        Console.WriteLine(i + ". " + "IP: " + client.IpAddress);
                        i++;
                    }
                }
                return i;
            }
            
        }


        /*
         * Metoda odpowiedzialna za sprawdzenie (w jakiś sposób), czy danych host jest dostępny czy może należy
         * wyświetlić powiadomienie (allert), dotyczący tego, że np nie otrzymaliśmy 3 keepalive wiec prawd.
         * tak węzeł is down (jest nieosiągalny)
         * 
         * -trzeba to jakoś rozkminić i wymyślec mechanizm (takie wiaodmości będziemy wysyłać co 30 sekund)
        */
        public void KeepAliveVeryfication(int id)
        {

        }
    }
}
