using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;


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

        /*
         * Lokalne zmienne
         * - CloudIPAdd - adres IP chmury kablowej
         * - CloudPortNumber -numer portu w chmurze przypisany do komunikacji z tym urządzeniem
         * - LocalIPAdd -numer IP przypisany do danego urządzenia
         * - LocalPortNumber - numer portu przypisany do danego urządzenia
         * - LogFilePath - ścieżka do pliku, w którym zapisywane bedą wszystkie zdarzenia (logi)
         */

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
            //tutaj wywołujemy metode ShowPathRequest oraz pobieramy FilePath metoda get publiczną z obiektu
            GetConfigPath();
            XmlDocument config = OpenFile();
            PerformConfiguration(config);

        }

        private void GetConfigPath()
        {
            Console.Write("Please type the path for config file (leave empty for default): ");
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
        private XmlDocument OpenFile()
        {
            XmlTextReader configFile = new XmlTextReader(configFilePath);
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
            this.cloudIP = IPAddress.Parse(configFile.SelectSingleNode("ManagmentLayer/localIP").InnerText);
            this.cloudPort = int.Parse(configFile.SelectSingleNode("ManagmentLayer/localPort").InnerText);
            this.localIP = IPAddress.Parse(configFile.SelectSingleNode("ManagmentLayer/cloudIP").InnerText);
            this.localPort = int.Parse(configFile.SelectSingleNode("ManagmentLayer/cloudPort").InnerText);
        }

    }
}
