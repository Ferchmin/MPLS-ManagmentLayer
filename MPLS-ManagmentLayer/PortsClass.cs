using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

/*
 * Klasa odpowiedzialna za wszelkie połaczenia sieciowe programu
 * - program powinien posiadać jeden obiekt klasy Socket połączony bezpośrednio z Chmura Kablową;
 * - stworzenie wirtualnego portu nasłuchującego
 * - dynamiczne tworzenie tablicy aktywnych węzłów sieciowych
 * - otrzymywanie i przetwarzanie wiadomości Keep-Alive
 * - udostępnia metodę wysłania i odebrania dowolnej wiadomości
*/
namespace MPLS_ManagmentLayer
{
    class PortsClass
    {

        #region Variables
        Socket mySocket;
        IPEndPoint myIpEndPoint;

        IPEndPoint agentIPEndPoint;
        EndPoint agentEndPoint;

        byte[] buffer;
        int bufferSize;

        IPAddress myIpAddress;
        int myPort;
        #endregion

        #region Accessors
        private ManagementPacket managmentPacket = new ManagementPacket();
        private List<LSRouter> connectedRouters = new List<LSRouter>();

        public List<LSRouter> ConnectedRouters
        {
            get { return connectedRouters; }
        }
        public IPAddress MyIPAddress
        {
            get { return myIpAddress; }
            set { myIpAddress = value; }
        }
        #endregion


        /*
		* Konstruktor - wymaga podania zmiennych pobranych z pliku konfiguracyjnego
		*/
        public PortsClass(ConfigurationClass configurationBase)
        {
            InitializeData(configurationBase.localIP, configurationBase.localPort, configurationBase.cloudIP, configurationBase.cloudPort);
            InitializeSocket();
        }

        /*
		* Metoda odpowiedzialna za przypisanie danych do lokalnych zmiennych.
		*/
        private void InitializeData(IPAddress myIpAddress, int myPort, IPAddress cloudIpAddress, int cloudPort)
        {
            this.myIpAddress = myIpAddress;
            this.myPort = myPort;

            bufferSize = 275;
        }

        /*
		* Metoda odpowiedzialna za inicjalizację nasłuchiwania na przychodzące wiadomośći.
		*/
        private void InitializeSocket()
        {
            //tworzymy gniazdo i przypisujemy mu numer portu i IP zgodne z plikiem konfig
            mySocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            myIpEndPoint = new IPEndPoint(myIpAddress, myPort);
            mySocket.Bind(myIpEndPoint);

            //tworzymy punkt końcowy chmury kablowej
            agentIPEndPoint = new IPEndPoint(IPAddress.Any, 0);
            agentEndPoint = (EndPoint)agentIPEndPoint;

            //tworzymy bufor nasłuchujący
            buffer = new byte[bufferSize];

            //inicjalizacja nasłuchiwania
            mySocket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref agentEndPoint, new AsyncCallback(ReceivedPacket), null);
        }

        /*
		* Metoda odpowiedzialna za ukończenie odbierania pakietu.
		* - tutaj generowany będzie log z wydarzenia;
		* - tutaj przesyłamy otryzmany pakiet do wewnętrznej metody odpowiedzialnej za przetwarzanie
		*/
        private void ReceivedPacket(IAsyncResult res)
        {
            //kończymy odbieranie pakietu - metoda zwraca rozmiar faktycznie otrzymanych danych
            int size = mySocket.EndReceiveFrom(res, ref agentEndPoint);

            //tworzę tablicę bajtów składającą się jedynie z danych otrzymanych (otrzymany pakiet)
            byte[] receivedPacket = new byte[size];
            Array.Copy(buffer, receivedPacket, receivedPacket.Length);

            //tworzę tymczasoyw punkt końcowy zawierający informacje o nadawcy (jego ip oraz nr portu)
            IPEndPoint receivedIPEndPoint = (IPEndPoint)agentEndPoint;

            //generujemy logi
            LogMaker.MakeLog("INFO - Packet received from " + receivedIPEndPoint.Address + " port: "+receivedIPEndPoint.Port);

            //przesyłam pakiet do metody przetwarzającej
            ProcessReceivedPacket(receivedPacket, receivedIPEndPoint);

            //zeruje bufor odbierający
            buffer = new byte[bufferSize];

            //uruchamiam ponowne nasłuchiwanie
            mySocket.BeginReceiveFrom(buffer, 0, buffer.Length, SocketFlags.None, ref agentEndPoint, new AsyncCallback(ReceivedPacket), receivedIPEndPoint);
        }

        /*
		* Metoda odpowiedzialna za ukończenie wysyłania pakietu.
		* - tutaj generowany będzie log z wydarzenia;
		*/
        private void SendPacket(IAsyncResult res)
        {
            //kończymy wysyłanie pakietu - funkcja zwraca rozmiar wysłanego pakietu
            int size = mySocket.EndSendTo(res);

            var endPoint = res.AsyncState as IPEndPoint; 

            //tworzmy log zdarzenia
            //LogMaker.MakeLog("INFO - Packet sent to "+endPoint.Address + "port: "+endPoint.Port);
        }

        /*
		* Metoda odpowiedzialna za przetwarzanie odebranego pakietu.
		*/
        private void ProcessReceivedPacket(byte[] receivedPacketBytes, IPEndPoint receivedIPEndPoint)
        {
            ManagementPacket receivedPacket = new ManagementPacket(receivedPacketBytes);

            switch (receivedPacket.DataIdentifier)
            {
                case 0:
                    AddConectedRouter(receivedPacket, receivedIPEndPoint);
                    break;
                case 1:
                    RestartRouterTimer(receivedPacket, receivedIPEndPoint);
                    break;
                case 2:
                    //commands to management layer do nothing
                    break;
                case 3:
                    GetResponse(receivedPacket);
                    break;
                default:
                    LogMaker.MakeLog("ERROR - Received unknown type of packet " + receivedPacket.DataIdentifier.ToString() + " from: " + receivedIPEndPoint.Address.ToString());
                    break;
            }
        }

        private void GetResponse(ManagementPacket packet)
        {
            string[] table = packet.Data.Split('|');

            switch (table[0])
            {
                case "Accepted":
                    LogMaker.MakeLog("INFO - Received accepted response from: " + packet.IpSource + " : " + packet.Data);
                    break;

                case "Denied":
                    LogMaker.MakeLog("INFO - Received denied response from: " + packet.IpSource + " : " + packet.Data);
                    break;

                default:
                    LogMaker.MakeConsoleLog("INFO - Received table from: " + packet.IpSource);
                    LogMaker.MakeLog("INFO - Received table from: " + packet.IpSource);

                    String column = String.Format("{0,10} {1,12} {2,10} {3,12} {4,10}", "LabelIn:", "InterfaceIn:", "LabelOut:", "InterfaceOut:", "Operation:");
                    Console.WriteLine("\n" + column);

                    foreach (var line in table)
                    {
                        if (line != "")
                        {
                            string[] lineParts = line.Split('&');
                            String row = String.Format("{0,-5} {1,-10} {2,-10} {3,-15} {4,-20}", lineParts[0], lineParts[1], lineParts[2], lineParts[3], lineParts[4]);
                            Console.WriteLine(row);
                            //LogMaker.MakeConsoleLog("LabelIn: " + lineParts[0] + " InterfaceIn: " + lineParts[1] + " LabelOut: " + lineParts[2] + " InterfaceOut: " + lineParts[3] + " Operation: " + lineParts[4]);
                        }
                    }
                    break;
            }
        }

        private void AddConectedRouter(ManagementPacket packet, IPEndPoint receivedIPEndPoint)
        {
            bool flag = false;
            LSRouter lsRouter = new LSRouter(packet.IpSource, receivedIPEndPoint.Port);
            foreach (LSRouter router in ConnectedRouters)
            {
                if (router.IpAddress == lsRouter.IpAddress)
                {
                    flag = true;
                }
            }

            if (flag)
            {
                RestartRouterTimer(packet, receivedIPEndPoint);
            }
            else
            {
                ConnectedRouters.Add(lsRouter);
                LogMaker.MakeLog("INFO - Received IsUp from: " + lsRouter.IpAddress);
                LogMaker.MakeConsoleLog("INFO - Received IsUp from: " + lsRouter.IpAddress);
            }

        }

        private void RestartRouterTimer(ManagementPacket packet, IPEndPoint receivedIPEndPoint)
        {
            bool routerRestarted = false;
            foreach (LSRouter router in ConnectedRouters)
            {
                if (router.IpAddress == packet.IpSource)
                {
                    router.keepAliveTimer.Stop();
                    router.keepAliveTimer.Start();

                    routerRestarted = true;
                    LogMaker.MakeLog("INFO - Received keepAlive from: " + router.IpAddress);
                }
            }
            if (!routerRestarted)
            {
                LSRouter lsRouter = new LSRouter(packet.IpSource, receivedIPEndPoint.Port);
                ConnectedRouters.Add(lsRouter);
                LogMaker.MakeLog("INFO - Received keepAlive from new router - router added");
                LogMaker.MakeConsoleLog("INFO - Received keepAlive from new router - router added");
            }

        }

        /*
		* Metoda odpowiedzialna za inicjalizowanie wysyłania własnego pakietu przez węzeł kliencki.
		* - metoda publiczna, wywoływana przez inne klasy w celu nadania wiadomosćil;
		*/
        public void SendMyPacket(byte[] myPacket, IPEndPoint agentIPEndPoint)
        {
            //inicjuje start wysyłania przetworzonego pakietu do nadawcy
            mySocket.BeginSendTo(myPacket, 0, myPacket.Length, SocketFlags.None, agentIPEndPoint, new AsyncCallback(SendPacket), agentIPEndPoint);
        }
    }
}
