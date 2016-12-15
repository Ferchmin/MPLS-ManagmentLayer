using System;
using System.Net;
using System.Xml;


/*
 * Klasa odpowiedzialna za wczytywanie pliku konfiguracyjnego
 * - ściezka do pliku jest wpisywana przez użytkownika za pomocą konsoli
 * - pliki zapisane są w postaci .xml 
 * 
 * LOKALNE ZMIANY W KLASIE
 * - odczytywanie adresu IP i numeru porty chmury
 * - odczytywanie adresu IP i numeru portu swojego (ten dwie dane chmura ma w swoim pliku konf.
 * 
*/
namespace MPLS_ManagmentLayer
{
    class ConfigurationClass
    {

        string DEFAULCONFIGPATH = "defaultConfig.xml";

        public IPAddress cloudIP { get; set; }
        public int cloudPort { get; set; }
        public IPAddress localIP { get; set; }
        public int localPort { get; set; }
        public string logFilePath { get; private set; }
        public string configFilePath { get; private set; }


        /*
         * Konstruktor klasy
         * - przypisanie ścieżki do pliku odczytanej z konsoli;
         */
        public ConfigurationClass()
        {
            GetConfigPath();

            XmlDocument config = OpenXMLFile(configFilePath);
            PerformConfiguration(config);
        }

        public void GetConfigPath()
        {
            Console.WriteLine("Please type the path for config file: ");
            configFilePath = Console.ReadLine();

            if (configFilePath == "")
            {
                Console.WriteLine("Opening default config file");
                configFilePath = DEFAULCONFIGPATH;
            }
        }

        /*
        * Klasa odpowiadająca za bezpieczne otwarcię pliku
        */
        private XmlDocument OpenXMLFile(string path)
        {
            XmlTextReader configFile = new XmlTextReader(path);
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(configFile);

            return xDoc;
        }

        /*
         * Klasa odpowiadająca za odczytanie pliku w odpowiedni sposób
         * - odczytanie i przypisanie wszystkich zmiennych
         */
        private void PerformConfiguration(XmlDocument configFile)
        {
            this.localIP = IPAddress.Parse(configFile.SelectSingleNode("ManagmentLayer/localIP").InnerText);
            this.localPort = int.Parse(configFile.SelectSingleNode("ManagmentLayer/localPort").InnerText);
        }
    }
}
