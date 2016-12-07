using System;
using System.Collections.Generic;
using System.Linq;
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

        private List<LSRouter> connectedRouters = new List<LSRouter>();

        public List<LSRouter> ConnectedRouters
        {
            get { return connectedRouters; }
        }

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
            Console.WriteLine("Type destination IP: ");
            string destinationIP = Console.ReadLine();

            Console.WriteLine("Set line: ");
            int tableLine = Int32.Parse(Console.ReadLine());

            Console.WriteLine("State the IN port: ");
            int inPort = Int32.Parse(Console.ReadLine());

            Console.WriteLine("State the OUT port: ");
            int outPort = Int32.Parse(Console.ReadLine());

            string packetMessage = "ADD " + tableLine.ToString() + " " + inPort.ToString() + " " + outPort.ToString();

            ManagementPacket commandPacket = new ManagementPacket();
            commandPacket.IpSource = portsCommunication.MyIPAddress.ToString();
            commandPacket.IpDestination = destinationIP;
            commandPacket.DataIdentifier = 2;
            commandPacket.Data = packetMessage;
            commandPacket.MessageLength = (ushort)(Encoding.ASCII.GetBytes(packetMessage).Length);

            portsCommunication.SendMyPacket(commandPacket.CreatePacket());
        }

        public void SendRemoveCommand()
        {
            Console.WriteLine("Type destination IP: ");
            string destinationIP = Console.ReadLine();

            Console.WriteLine("Set line: ");
            int tableLine = Int32.Parse(Console.ReadLine());

            Console.WriteLine("State the IN port: ");
            int inPort = Int32.Parse(Console.ReadLine());

            Console.WriteLine("State the OUT port: ");
            int outPort = Int32.Parse(Console.ReadLine());

            string packetMessage = "REMOVE " + tableLine.ToString() + " " + inPort.ToString() + " " + outPort.ToString();

            ManagementPacket commandPacket = new ManagementPacket();
            commandPacket.IpSource = portsCommunication.MyIPAddress.ToString();
            commandPacket.IpDestination = destinationIP;
            commandPacket.DataIdentifier = 2;
            commandPacket.Data = packetMessage;
            commandPacket.MessageLength = (ushort)(Encoding.ASCII.GetBytes(packetMessage).Length);

            portsCommunication.SendMyPacket(commandPacket.CreatePacket());
        }

        public void RestartRouterTimer(ManagementPacket packet)
        {
            foreach (LSRouter router in ConnectedRouters)
            {
                if (router.IpAddress == packet.IpSource)
                {
                    router.keepAliveTimer.Stop();
                    //Nie musze uruchamiac stopera poniewaz parametr AutoReset jest ustawiony na true
                    //router.keepAliveTimer.Start();
                }
            }

        }

        public void AddConectedRouter(ManagementPacket packet)
        {
            bool flag = false;
            LSRouter lsRouter = new LSRouter(packet.IpSource);
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
            }

        }

        public void GetResponse(ManagementPacket packet)
        {
            //Make a log
            Console.WriteLine("Response from: " + packet.IpSource + " : " + packet.Data);
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


        /*
         * Metoda generująca zdarzenie
         * -trzeba się zastanowić w jakiej formie to tworzyć i kiedy ma być wykonywana ta komenda
         * - log powinien się składać na pewno z czasu zajścia i jakiś charakterystycznych parametrów
         * albo wczesniej trzeba zdefiniować listę stringów zawierających wszystkie możliwe logi
         * (np: zgłoszenie węzła o id takim, węzeł o id takim wyslał keep-alive, wysłanie do agenta o id takim wiadomości takiej,
         * odebranie od agenta wiadomości takiej a takiej, blad w dostarczeniu komendy (w komunikacji) itp.)
         * 
         * -moim zdaniem powinniśmy wysyłać logType i listę parametrów w stringu (może być nullem jak nie mamy nic do dodania,
         * jednak w wiekszosci logów bedzie trzeba coś wrzucic np id wezła, albo treść wiadomości)
         * 
         * - komenda musi sie kończyć zaktualizowaniem pliku, czyli wywoałniem metody UpdateLogFile
        */
        public void MakeLog(int logType, string[] parameters)
        {

            string logDescription = "";

            //wyrażenie switch(logType) i w każdym definiujemy specjalne logi
            switch (logType)
            {
                case 0:
                    logDescription = "log typu 0";
                    break;
                default:
                    break;

            }

            UpdateLogFile(logDescription);
            UpdateLogConsole(logDescription);
        }


        /*
         * Metoda odpowiedzialna za aktualizację pliku ze zdarzeniami
         * - jeżeli pojawia się w programie nowe zdarzenie, trzeba dopisać je do pliku
         * - do każdego nowego zdarzenia dodajemy id (globalny licznik większany przy każdym wywołaniu metody)
         * - jeżeli wystąpi błąd przy otwarciu pliku, powinniśmy to wyświetlić na konsoli głównej metodą ShowLogError 
         * 
         * - jeżeli wystąpi bład w otwarciu, to może będziemy robić zapasowy plik awaryjny? do ustalenia
        */
        public void UpdateLogFile(string logDescription)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter("ManagementLayerLogs.txt", true))
                file.WriteLine(logID + " " + DateTime.Now.ToString("hh:mm:ss") + " " + logDescription);
            logID++;
        }

        public void UpdateLogConsole(string logDescription)
        {
            Console.WriteLine(logID + " " + DateTime.Now.ToString("hh:mm:ss") + " " + logDescription);
        }
    }
}
