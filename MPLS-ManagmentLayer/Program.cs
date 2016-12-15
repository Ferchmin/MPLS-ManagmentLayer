using System;

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
            InteractionClass consoleInterface = new InteractionClass();

            Console.Write("\nEnter commands here: ");

            while(run)
            {
                inputString = Console.ReadLine();

                if (inputString == "exit")
                {
                    run = false;
                    LogMaker.MakeLog("INFO - Management agent is offline");
                }
                else
                {
                    if (inputString == "")
                        Console.Write("Enter commands here: ");
                    else
                        consoleInterface.GetCommand(inputString);
                }
            }
        }
    }
}
