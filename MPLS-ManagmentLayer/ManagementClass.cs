using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

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

        public ConfigurationClass configurationBase;
        public PortsClass portsCommunication;

        public LogMaker logMaker;

        private List<LSRouter> connectedRouters = new List<LSRouter>();
        public List<LSRouter> ConnectedRouters
        {
            get { return connectedRouters; }
        }


        /*
         * Konstruktor obiektu
         * - wyzerowanie licznika logów
        */
        public ManagementClass()
        {
            configurationBase = new ConfigurationClass();
            portsCommunication = new PortsClass(configurationBase);
            logMaker = new LogMaker();

            LogMaker.MakeLog("INFO - Managment agent is online");
        }


        public void SendAddCommand()
        {
            IPEndPoint agentEndPoint = ChooseTargetRouter();

            if (agentEndPoint == null)
            {
                LogMaker.MakeConsoleLog("ERROR - Failed to send ADD command - wrong router selected or no router availible");
            }
            else
            {
                Console.Write("Set LabelIn: ");
                int labelIn = Int32.Parse(Console.ReadLine());

                Console.Write("State the InterfaceIn: ");
                int intIn = Int32.Parse(Console.ReadLine());

                Console.Write("State the labelOut: ");
                int labelOut = Int32.Parse(Console.ReadLine());

                Console.Write("State the InterfaceOut: ");
                int intOut = Int32.Parse(Console.ReadLine());

                Console.Write("Choose which operation to perform (pop, push or swap): ");
                string operation = Console.ReadLine();

                operation = operation.ToLower();

                string packetMessage = "Add " + labelIn.ToString() + " " + intIn.ToString() + " " + labelOut.ToString() + " " + intOut.ToString() + " " + operation;

                ManagementPacket commandPacket = new ManagementPacket();
                commandPacket.IpSource = portsCommunication.MyIPAddress.ToString();
                commandPacket.IpDestination = agentEndPoint.Address.ToString();
                commandPacket.DataIdentifier = 2;
                commandPacket.Data = packetMessage;
                commandPacket.MessageLength = (ushort)(Encoding.ASCII.GetBytes(packetMessage).Length);

                portsCommunication.SendMyPacket(commandPacket.CreatePacket(), agentEndPoint);

                LogMaker.MakeLog("INFO - Sent ADD command to " + agentEndPoint.Address.ToString());
                LogMaker.MakeConsoleLog("INFO - Sent ADD command to " + agentEndPoint.Address.ToString());
            }
        }

        public IPEndPoint ChooseTargetRouter()
        {
            int idRange = ShowClientList();

            if (idRange == 0)
            {
                return null;
            }

            IPAddress ipAddress;
            int port;

            Console.WriteLine("Choose router by typing ID: ");

            try
            {
                int destinationRouterId = Int32.Parse(Console.ReadLine());
                if (destinationRouterId < idRange)
                {
                    if (portsCommunication.ConnectedRouters[destinationRouterId].IsActive)
                    {
                        ipAddress = IPAddress.Parse(portsCommunication.ConnectedRouters[destinationRouterId].IpAddress);
                        port = portsCommunication.ConnectedRouters[destinationRouterId].Port;
                        return new IPEndPoint(ipAddress, port);
                    }
                    else
                    {
                        Console.WriteLine("ERROR - Router is no longer active");
                        return null;
                    }
                }
                else
                {
                    Console.WriteLine("ERROR - Wrong router selected");
                    return null;
                }
            }
            catch
            {
                Console.WriteLine("Please, choose the number: ");
                return ChooseTargetRouter();
            } 
        }

        public void SendRemoveCommand()
        {
            IPEndPoint agentEndPoint = ChooseTargetRouter();

            if (agentEndPoint == null)
            {
                LogMaker.MakeConsoleLog("ERROR - Failed to send REMOVE command - wrong router selected");
            }
            else
            {
                Console.Write("Set LabelIn: ");
                int labelIn = Int32.Parse(Console.ReadLine());

                Console.Write("State the InterfaceIn: ");
                int intIn = Int32.Parse(Console.ReadLine());

                string packetMessage = "Remove " + labelIn.ToString() + " " + intIn.ToString();

                ManagementPacket commandPacket = new ManagementPacket();
                commandPacket.IpSource = portsCommunication.MyIPAddress.ToString();
                commandPacket.IpDestination = agentEndPoint.Address.ToString();
                commandPacket.DataIdentifier = 2;
                commandPacket.Data = packetMessage;
                commandPacket.MessageLength = (ushort)(Encoding.ASCII.GetBytes(packetMessage).Length);


                portsCommunication.SendMyPacket(commandPacket.CreatePacket(), agentEndPoint);

                LogMaker.MakeLog("INFO - Sent REMOVE command to " + agentEndPoint.Address.ToString());
                LogMaker.MakeConsoleLog("INFO - Sent REMOVE command to " + agentEndPoint.Address.ToString());
            }
        }

        public int ShowClientList()
        {
            for (int i = 0; i < portsCommunication.ConnectedRouters.Count; i++)
            {
                if (!portsCommunication.ConnectedRouters[i].IsActive)
                {
                    portsCommunication.ConnectedRouters.RemoveAt(i);
                }
            }

            if (portsCommunication.ConnectedRouters.Count == 0)
            {
                Console.WriteLine("INFO - No clients connected");
                return 0;
            }
            else
            {
                //Console.WriteLine("Number of availible clients: " + portsCommunication.ConnectedRouters.Count);
                Console.WriteLine("Availible routers: ");
                int i = 0;
                foreach (LSRouter client in portsCommunication.ConnectedRouters)
                {
                    Console.WriteLine(portsCommunication.ConnectedRouters.IndexOf(client) + ". " + "IP: " + client.IpAddress);
                    i++;
                }
                return i;
            }
        }

       public void SendGetTableCommand()
        {
            IPEndPoint agentEndPoint = ChooseTargetRouter();

            if (agentEndPoint == null)
            {
                LogMaker.MakeConsoleLog("ERROR - Failed to send GETTABLE command - wrong router selected.");
            }
            else
            {
                string packetMessage = "GetTable";

                ManagementPacket commandPacket = new ManagementPacket();
                commandPacket.IpSource = portsCommunication.MyIPAddress.ToString();
                commandPacket.IpDestination = agentEndPoint.Address.ToString();
                commandPacket.DataIdentifier = 2;
                commandPacket.Data = packetMessage;
                commandPacket.MessageLength = (ushort)(Encoding.ASCII.GetBytes(packetMessage).Length);


                portsCommunication.SendMyPacket(commandPacket.CreatePacket(), agentEndPoint);

                LogMaker.MakeLog("INFO - Sent GetTable command to " + agentEndPoint.Address.ToString());
                LogMaker.MakeConsoleLog("INFO - Sent GetTable command to " + agentEndPoint.Address.ToString());
            }
        }



        #region SHOW_METHODES
        public void SendAutomatedAdd(IPEndPoint agentEndPoint, int labelIn, int intIn, int labelOut, int intOut, string operation)
        {
            string packetMessage = "Add " + labelIn.ToString() + " " + intIn.ToString() + " " + labelOut.ToString() + " " + intOut.ToString() + " " + operation;

            ManagementPacket commandPacket = new ManagementPacket();
            commandPacket.IpSource = portsCommunication.MyIPAddress.ToString();
            commandPacket.IpDestination = agentEndPoint.Address.ToString();
            commandPacket.DataIdentifier = 2;
            commandPacket.Data = packetMessage;
            commandPacket.MessageLength = (ushort)(Encoding.ASCII.GetBytes(packetMessage).Length);


            portsCommunication.SendMyPacket(commandPacket.CreatePacket(), agentEndPoint);
            LogMaker.MakeLog("INFO - Sent ADD command to " + agentEndPoint.Address.ToString());
            LogMaker.MakeConsoleLog("INFO - Sent ADD command to " + agentEndPoint.Address.ToString());
        }

        public void SendAutomatedRemove(IPEndPoint agentEndPoint, int labelIn, int intIn)
        {
            string packetMessage = "Remove " + labelIn.ToString() + " " + intIn.ToString();

            ManagementPacket commandPacket = new ManagementPacket();
            commandPacket.IpSource = portsCommunication.MyIPAddress.ToString();
            commandPacket.IpDestination = agentEndPoint.Address.ToString();
            commandPacket.DataIdentifier = 2;
            commandPacket.Data = packetMessage;
            commandPacket.MessageLength = (ushort)(Encoding.ASCII.GetBytes(packetMessage).Length);


            portsCommunication.SendMyPacket(commandPacket.CreatePacket(), agentEndPoint);

            LogMaker.MakeLog("INFO - Sent REMOVE command to " + agentEndPoint.Address.ToString());
            LogMaker.MakeConsoleLog("INFO - Sent REMOVE command to " + agentEndPoint.Address.ToString());
        }

        public void FixBrokenLink()
        {
            IPEndPoint LSR3 = new IPEndPoint(IPAddress.Parse("127.0.3.0"), 7030);

            //SendAutomatedRemove(LSR3, 12, 1);
            SendAutomatedRemove(LSR3, 22, 1);
            SendAutomatedRemove(LSR3, 56, 2);
            SendAutomatedRemove(LSR3, 57, 2);

            //SendAutomatedAdd(LSR3, 12, 1, 1, 4, "push");
            SendAutomatedAdd(LSR3, 56, 2, 1, 4, "push");

            IPEndPoint LSR4 = new IPEndPoint(IPAddress.Parse("127.0.4.0"), 7040);
            SendAutomatedRemove(LSR4, 1, 1);
            SendAutomatedRemove(LSR4, 22, 1);
            SendAutomatedRemove(LSR4, 57, 1);

            SendAutomatedAdd(LSR4, 2, 2, 0, 0, "pop");
            SendAutomatedAdd(LSR4, 12, 2, 44, 3, "swap");
            SendAutomatedAdd(LSR4, 56, 2, 58, 4, "swap");
        }

        public void FixedBrokenLSR()
        {
            IPEndPoint LSR1 = new IPEndPoint(IPAddress.Parse("127.0.1.0"), 7010);
            SendAutomatedRemove(LSR1, 11, 1);
            SendAutomatedAdd(LSR1, 11, 1, 12, 3, "swap");

            IPEndPoint LSR5 = new IPEndPoint(IPAddress.Parse("127.0.5.0"), 7050);
            SendAutomatedAdd(LSR5, 12, 1, 0, 3, "pop");

            IPEndPoint LSR2 = new IPEndPoint(IPAddress.Parse("127.0.2.0"), 7020);
            SendAutomatedRemove(LSR2, 55, 1);
            SendAutomatedAdd(LSR2, 55, 1, 56, 4, "swap");

            IPEndPoint LSR6 = new IPEndPoint(IPAddress.Parse("127.0.6.0"), 7060);
            SendAutomatedAdd(LSR6, 56, 2, 0, 3, "pop");
        }
        #endregion
    }
}
