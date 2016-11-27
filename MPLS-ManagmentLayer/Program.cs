using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * Aplikacja CENTRUM ZARZĄDZANIA
 * - aplikacja zarządza wszystkimi elementami sieci, które wyposażone są w agentów zarządzania
 * 
 * - osoba odpowiedzialna: Paweł Zgoda-Ferchmin
*/

namespace MPLS_ManagmentLayer
{
    class Program
    {
        static void Main(string[] args)
        {

            //tworzymy wszystkie główne obiekty
            ConfigurationClass configurationBase = new ConfigurationClass();
            PortsClass portsCommunication = new PortsClass(configurationBase);
            InteractionClass consoleInterface = new InteractionClass();
            ManagementClass manager = new ManagementClass();

            consoleInterface.ShowPathRequest();

            /*
             * Zmienna kończy pętle programu
             * Domyślnie, konsola cały czas czeka na wpisanie przez użytkownika komendy
             * Jeżeli użytkownik wpisze exit, wtedy nastąpi wyłączenie programu
            */
            bool exitCommand = false;
            do
            {
                consoleInterface.InputString = Console.ReadLine();

            } while (exitCommand);

        }
    }
}
