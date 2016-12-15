using System;

/*
 * Klasa odpowiedzialna za interakcje z użytkownikiem 
 * - interfejs konsolowy
*/

namespace MPLS_ManagmentLayer
{
    class InteractionClass
    {
        ManagementClass managementClass;

        public string filePath { get; private set; }
        public string InputString
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
        * Metoda odpowiedzialna za zczytywanie komend wpisanych przez uzytkownika.
        * - komenda w postaci obiektu typu string jest przesyłana do obiektu klasy management, która to klasa zajmuje sie jedynie przetwarzaniem danej komendy
        * - metoda zabezpiecza przed przyjęciem błednej komendy (trzeba jakoś weryfikować, w razie błędu napisać w konsoli, że błedna składnia)
        * 
        * - parametrem command jest tekst wpisany przez użytkownika do konsoli
        */
        public void ShowHelp()
        {
            Console.Write("\nAvailible commands: \n 1.Add \n 2.Remove \n 3.GetTable \n 4.LSR list \n 5.Help \n");
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
            command = command.ToUpper();
            switch (command)
            {
                case "ADD":
                    Console.WriteLine("Executing ADD");
                    managementClass.SendAddCommand();
                    break;
                case "1":
                    Console.WriteLine("Executing ADD");
                    managementClass.SendAddCommand();
                    break;

                case "REMOVE":
                    Console.WriteLine("Executing REMOVE command");
                    managementClass.SendRemoveCommand();
                    break;
                case "2":
                    Console.WriteLine("Executing REMOVE command");
                    managementClass.SendRemoveCommand();
                    break;

                case "GET TABLE":
                    managementClass.SendGetTableCommand();
                    break;
                case "3":
                    managementClass.SendGetTableCommand();
                    break;

                case "LSR LIST":
                    ShowClientList();
                    break;
                case "4":
                    ShowClientList();
                    break;

                case "HELP":
                    ShowHelp();
                    break;
                case "5":
                    ShowHelp();
                    break;

                case "6":
                    managementClass.FixBrokenLink();
                    break;

                case "7":
                    managementClass.FixedBrokenLSR();
                    break;

                default:
                    Console.WriteLine("ERROR - Invalid command, try again.");
                    break;
            }
        }

        /* 
         * Metoda wyswietlajaca liste aktualnie podlaczonych klientow
         */
        public void ShowClientList()
        {
            for (int i= 0;i < managementClass.portsCommunication.ConnectedRouters.Count; i++)
                {
                    if (!managementClass.portsCommunication.ConnectedRouters[i].IsActive)
                    {
                        managementClass.portsCommunication.ConnectedRouters.RemoveAt(i);
                    }
                }

            if(managementClass.portsCommunication.ConnectedRouters.Count == 0)
                {
                    Console.WriteLine("ALERT - No clients connected");
                }else
                {
                 foreach (LSRouter client in managementClass.portsCommunication.ConnectedRouters)
                 {
                    Console.WriteLine(managementClass.portsCommunication.ConnectedRouters.IndexOf(client) + ". " + "IP: " + client.IpAddress);
                 }
            }
               
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
                    Console.WriteLine("No help availible. Try asking the authors.");
                    break;
            }
        }
    }
}
