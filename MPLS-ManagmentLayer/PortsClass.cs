using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

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
        
        Socket mySocket;
        IPEndPoint myIpEndPoint;

        IPEndPoint agentIPEndPoint;
        EndPoint agentEndPoint;

        byte[] buffer;
        byte[] packet;

        IPAddress myIpAddress;
        int myPort;
        
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


        /*
		* Konstruktor - wymaga podania zmiennych pobranych z pliku konfiguracyjnego
		*/
        public PortsClass(ConfigurationClass configurationBase)
        {
            InitializeData(configurationBase.localIP, configurationBase.localPort, configurationBase.cloudIP, configurationBase.cloudPort);
            InitializeSocket();
            //Console.WriteLine("Config Loaded - local IP: " + myIpAddress + " local Port: " + myPort + " cloud IP: "+routerIpAddress +" cloud Port: " + cloudPort);
        }

        /*
		* Metoda odpowiedzialna za przypisanie danych do lokalnych zmiennych.
		*/
        private void InitializeData(IPAddress myIpAddress, int myPort, IPAddress cloudIpAddress, int cloudPort)
        {
            this.myIpAddress = myIpAddress;
            this.myPort = myPort;        }

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
            buffer = new byte[1024];

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
            //tutaj niby zawsze będzie to z chmury kablowej więc cloudIPEndPoint powinien być tym samym co receivedIPEndPoint
            //tutaj można będzie zrobić sprawdzenie bo cloud to teoria a received to praktyka skąd przyszły dane
            IPEndPoint receivedIPEndPoint = (IPEndPoint)agentEndPoint;

            //generujemy logi
            LogMaker.MakeLog("Packet received from" + receivedIPEndPoint.Address + " port: "+receivedIPEndPoint.Port);

            //przesyłam pakiet do metody przetwarzającej
            ProcessReceivedPacket(receivedPacket, receivedIPEndPoint);

            //zeruje bufor odbierający
            buffer = new byte[1024];

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
            LogMaker.MakeLog("Packet sent to "+endPoint.Address + "port: "+endPoint.Port);

        }

        /*
		* Metoda odpowiedzialna za przetwarzanie odebranego pakietu.
		*/
        private void ProcessReceivedPacket(byte[] receivedPacketBytes, IPEndPoint receivedIPEndPoint)
        {
            //w celach testowych przypisuje ten sam pakiet co przyszedł do wysłania
            packet = receivedPacketBytes;

            ManagementPacket receivedPacket = new ManagementPacket(receivedPacketBytes);

            switch (receivedPacket.DataIdentifier)
            {
                case 0:
                    AddConectedRouter(receivedPacket, receivedIPEndPoint);
                    break;
                case 1:
                    RestartRouterTimer(receivedPacket);
                    break;
                case 2:
                    break;
                case 3:
                    GetResponse(receivedPacket);
                    break;
                default:
                    break;
            }
        }

        private void GetResponse(ManagementPacket packet)
        {
            //Make a log

            LogMaker.MakeLog("Received response from: "+packet.IpSource + " : "+packet.Data);

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
                RestartRouterTimer(packet);
            }
            else
            {
                ConnectedRouters.Add(lsRouter);
                LogMaker.MakeLog("Received IsUp from: " + lsRouter.IpAddress);
            }

        }


        private void RestartRouterTimer(ManagementPacket packet)
        {
            foreach (LSRouter router in ConnectedRouters)
            {
                if (router.IpAddress == packet.IpSource)
                {
                    router.keepAliveTimer.Stop();

                    LogMaker.MakeLog("Received keepAlive from: " + router.IpAddress);
                    //Nie musze uruchamiac stopera poniewaz parametr AutoReset jest ustawiony na true
                    //router.keepAliveTimer.Start();
                }else
                {
                    LogMaker.MakeLog("Received IsUp from unknown router");
                }
            }

        }


        /*
		* Metoda odpowiedzialna za inicjalizowanie wysyłania własnego pakietu przez węzeł kliencki.
		* - metoda publiczna, wywoływana przez inne klasy w celu nadania wiadomosćil;
		*/
        public void SendMyPacket(byte[] myPacket, IPEndPoint agentIPEndPoint)
        {
            //przypisujemy pakiet do zmiennej lokalnej
            packet = myPacket;

            //inicjuje start wysyłania przetworzonego pakietu do nadawcy
            mySocket.BeginSendTo(packet, 0, packet.Length, SocketFlags.None, agentIPEndPoint, new AsyncCallback(SendPacket), null);
        }





    }
}
