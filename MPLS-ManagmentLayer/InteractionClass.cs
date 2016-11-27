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
        public string FilePath { get; private set; }
        public string InputString {
            set {   GetCommand(value); }   }

        /*
         * Konstruktor klasy 
        */
        public InteractionClass()
        { 
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
        public void ShowDetailedHelp()
        {

        }

        /*
        * Metoda wyświetla błąd powstały przy odczycie/aktualizacji pliku z logami.
        */
        public void ShowLogError()
        {

        }

    }
}
