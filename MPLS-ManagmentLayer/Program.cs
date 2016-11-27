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

            string inputString;
            bool run = true;

            //tworzymy wszystkie główne obiekty
            //ConfigurationClass configurationBase = new ConfigurationClass();
            //PortsClass portsCommunication = new PortsClass(configurationBase);
            InteractionClass consoleInterface = new InteractionClass();
            //ManagementClass manager = new ManagementClass();

            do
            {
                Console.Write("Enter commands here: ");
                inputString = Console.ReadLine();

                if (inputString == "exit")
                {
                    run = false;
                }
                else
                {
                    consoleInterface.GetCommand(inputString);
                }

            } while (run);


        }
    }
}
