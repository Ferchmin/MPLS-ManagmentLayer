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
        public string LogFilePath { get; private set; }
        private int logID;


        /*
         * Konstruktor obiektu
         * - wyzerowanie licznika logów
        */
        public ManagementClass()
        {
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
            string logString = null;
            //dodajemy czas wystapienia

            //wyrażenie switch(logType) i w każdym definiujemy specjalne logi

            UpdateLogFile(logString);
        }


        /*
         * Metoda odpowiedzialna za aktualizację pliku ze zdarzeniami
         * - jeżeli pojawia się w programie nowe zdarzenie, trzeba dopisać je do pliku
         * - do każdego nowego zdarzenia dodajemy id (globalny licznik większany przy każdym wywołaniu metody)
         * - jeżeli wystąpi błąd przy otwarciu pliku, powinniśmy to wyświetlić na konsoli głównej metodą ShowLogError 
         * 
         * - jeżeli wystąpi bład w otwarciu, to może będziemy robić zapasowy plik awaryjny? do ustalenia
        */
        public void UpdateLogFile(string logString)
        {
            logID++;
        }
    }
}
