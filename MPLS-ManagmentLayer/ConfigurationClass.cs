using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        private string CloudIPAdd { get; set; }
        private int CloudPortNumber { get; set; }
        private string LocalIPAdd { get; set; }
        private int LocalPortNumber { get; set; }
        public string LogFilePath { get; private set; }


        /*
         * Konstruktor klasy
         * - przypisanie ścieżki do pliku odczytanej z konsoli;
         */
        public ConfigurationClass()
        {
            //tutaj wywołujemy metode ShowPathRequest oraz pobieramy FilePath metoda get publiczną z obiektu
        }

        /*
        * Klasa odpowiadająca za bezpieczne otwarcię pliku
        */
        private void OpenFile()
        {

        }

        /*
         * Klasa odpowiadająca za odczytanie pliku w odpowiedni sposób
         * - odczytanie i przypisanie wszystkich zmiennych
         */
        private void ReadFile()
        {

        }

        /*
        * Klasa odpowiadająca za bezpieczne zamknięcie pliku
        */
        private void CloseFile()
        {

        }
    }
}
