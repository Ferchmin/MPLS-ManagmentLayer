using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/*
 * Klasa odpowiedzialna za interakcje z użytkownikiem 
 * - interfejs konsolowy
 * 
*/
namespace MPLS_ManagmentLayer
{
    class InteractionClass
    {
        /*
         * Zmienne lokalne:
         * - FilePath, ścieżka do pliku konfiguracyjnego
         * - InputString, to co użytkownik wpisze do konsoli - wywołanie metody GetCommand;
        */
        ManagementClass managementClass;


        public string filePath { get; private set; }
        public string inputString
        {
            set { GetCommand(value); }
        }

        /*
         * Konstruktor klasy 
        */
        public InteractionClass()
        {

            managementClass = new ManagementClass();
            ShowHelp();
      
        }


        /*
        * Metoda odpowiedzialna za otrzymanie od użytkownika ścieżki do pliku konfiguracyjnego i przekazania go do odpowiedniej metody.
        * - metoda przypisuje wartośc do lokalnej zmiennej
        * -metoda sprawdza, czy dany plik jest osiagalny, jak nie to wyswietla komunikat o ponowne podanie sciezki
       */
        public void ShowPathRequest()
        {

        }

        /*
         * Metoda odpowiedzialna za generowanie ostrzeżenia, związanego z brakiem otrzymywania sygnałów keepAlive 
        */
        public void ShowAllert(int networkElementId)
        {
            //np "Węzeł o ID: prawdopodobnie uległ awarii"
        }

        /*
        * Metoda odpowiedzialna za wyświetlanie dostepnych opcji dla użytkownika
        * - związane z protokołem, jakiej komendy użyć aby coś zrobić
        * (komendy moga byc zapisane tutaj, albo w klasie management, czyli zarzadcy)
        * -w jakis sposób trzeba zapisac tutaj albo tam te wszystkie komendy możliwe dla uzytkownika
        */
        public void ShowHelp()
        {
            Console.Write("Availible commands: \n 1.Add \n 2.Remove \n 3.lsr list \n 4.help \n");
        }

        /*
        * Metoda odpowiedzialna za zczytywanie komend wpisanych przez uzytkownika.
        * - komenda w postaci obiektu typu string jest przesyłana do obiektu klasy management, która to klasa zajmuje sie jedynie przetwarzaniem danej komendy
        * - metoda zabezpiecza przed przyjęciem błednej komendy (trzeba jakoś weryfikować, w razie błędu napisać w konsoli, że błedna składnia)
        * 
        * - parametrem command jest tekst wpisany przez użytkownika do konsoli
        */
        public void GetCommand(string command)
        {
            command =  command.ToUpper();
            switch (command)
            {
                case "ADD":
                    Console.WriteLine(" Executing ADD");
                    managementClass.SendAddCommand();
                    break;
                case "1":
                    Console.WriteLine(" Executing ADD");
                    managementClass.SendAddCommand();
                    break;
                case "REMOVE":
                    Console.WriteLine(" Executing REMOVE command");
                    managementClass.SendRemoveCommand();
                    break;
                case "2":
                    Console.WriteLine(" Executing REMOVE command");
                    managementClass.SendRemoveCommand();
                    break;
                //case "SWAP":
                //  Console.WriteLine("Executing swap command");
                //  managementClass.AnalyseCommand(command);
                //  break;
                case "LSR LIST":
                    ShowClientList();
                    break;
                case "LIST":
                    ShowClientList();
                    break;
                case "3":
                    ShowClientList();
                    break;
                case "HELP":
                    ShowHelp();
                    break;
                case "4":
                    ShowHelp();
                    break;
                default:
                    Console.WriteLine(" Invalid command, try again");
                    LogMaker.MakeLog("Invalid command");
                    break;
            }
        }

        

        /* 
         * Metoda wyswietlajaca liste aktualnie podlaczonych klientow
         */
        public void ShowClientList()
        {

            if (managementClass.portsCommunication.ConnectedRouters.Count == 0)
            {
                Console.WriteLine(" - No clients connected");
            }
            else
            {
                Console.WriteLine(" - Number of availible clients: " + managementClass.portsCommunication.ConnectedRouters.Count);
                int i = 0;
                foreach (LSRouter client in managementClass.portsCommunication.ConnectedRouters)
                {
                    if (client.IsActive)
                    {
                        i++;
                        Console.WriteLine(i + ". " + "IP: " + client.IpAddress);
                    }
                }
            }

        }

        /*
        * Metoda odpowiedzialna za wyświetlanie:
        * - odebranej od agenta zarzadzania odpowiedzi (np wysyłamy set cośtam, odbieramy akceptacje, zalezy jak to działa w mpls
        * ja bym dodał cos  tym stylu, że agent odebrał i wykonał)
        */
        public void ShowAnswer(int networkElementId)
        {

        }

        /*
        * Metoda odpowiedzialna za wyświetlanie dostepnych opcji dla konkretnej komendy
        * - związane z protokołem, np: jak wpiszemy set? wcisniemy enter to dostaniemy z czego składa sie składnia danej komendy
        * plus przykład)
        */
        public void ShowDetailedHelp(string command)
        {
            switch (command)
            {
                case "add":
                    Console.WriteLine("Parameters for add are: LSR ID, FIB position, new FIB value.\n For list of LSR's connected type LSR list");
                    break;
                case "delete":
                    Console.WriteLine("Parameters for add are: LSR ID, FIB position, new FIB value.\n For list of LSR's connected type LSR list");
                    break;
                case "change":
                    Console.WriteLine("Parameters for add are: LSR ID, FIB position, new FIB value.\n For list of LSR's connected type LSR list");
                    break;
                default:
                    Console.WriteLine("No help availible. Try asking the authors");
                    break;
            }
        }

        /*
        * Metoda wyświetla błąd powstały przy odczycie/aktualizacji pliku z logami.
        */
        public void ShowLogError()
        {

        }

    }
}
