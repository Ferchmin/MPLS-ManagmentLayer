using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

/*
 * Klasa odpowiedzialna za wszelkie połaczenia sieciowe programu
 * - program powinien posiadać jeden obiekt klasy Socket połączony bezpośrednio z Chmura Kablową;
 * - stworzenie wirtualnego portu nasłuchującego
 * - dynamiczne tworzenie tablicy aktywnych węzłów sieciowych
 * - otrzymywanie i przetwarzanie wiadomości Keep-Alive
 * - udostępnia metodę wysłania i odebrania dowolnej wiadomości
*/
namespace MPLS_ManagmentLayer
{
    class PortsClass
    {
        /*
         * Lokalne zmienne
         * - CloudSocket, socket odpowiedzialny za cały ruch we/wy
         * - LocalEndPoint, punkt końcowy (numer IP centrum zarządzania oraz numer portu, na którym będzie komunikacja)
         * - configurationBase, wskaźnik na obiekt przechowujący dane pobrane z pliku konfig.
         * - AcctiveNetworkElements, lista aktywnych elementów sieciowych (tutaj węzłów sieciowych zgłaszających swoja obecność)
         */
        private Socket CloudSocket { get; set; }
        private IPEndPoint LocalEndPoint { get; set; }
        private ConfigurationClass configurationBase;
        public List<int> AcctiveNetworkElements { get; set; }


        /*
         * Konstruktor klasy
         * - tworzy nasłuchujący wątek
         * - próba nawiązania połaczenia?
        */
        public PortsClass(ConfigurationClass configurationBase)
        {
            this.configurationBase = configurationBase;
        }


        /*
         * Metoda odpowiedzilna za stworzenie oddzielnego wątku, zajmującego się nasłuchiwaniem
         * 
        */
        private void MakeListeningThread()
        {

        }

        /*
         * Metoda odpowiedzilna za dynamiczne tworzenie tablicy aktywnych węzłów sieci
         * - jeżeli węzeł wyśle wiadomość o treści "UP", wtedy powinniśmy go dołączyć do listy aktywnych węzłów sieci
         * - jeżeli otrzymamy inną wiadomośc, powinien wygenerować się log, z treścią otrzymanej wiadomości
         * - na podstawie otrzymanego zgłoszenia dodaje ID węzła do listy
         * 
         * - metoda będzie wywoływana z poziomu metody wątku nasłuchującego (prawdopodobnie musi być to metoda statyczna czyli funckja)
        */
        private void AddNetworkElement()
        {
            
        }

        /*
         * Metoda odpowiedzialna za przetwarzanie wiadomości typu Keep Alive
         * - trzeba przemyśleć sprawę jak tym zarządzać
         * - może trzeba oddzielną listę zrobic czy coś,
         * - trzeba w jakiś sposób sprawić, że jeżeli nie otrzymamy np 2 czy 3 keep alive to powinnien pojawić się alert
         * węzeł może być niedostepny - wtedy to od nas zależy, czy zajmiemy się tym alertem czy zignorujemy go
         * węzeł nadal będzie w liście dostepnych (może powinna istnieć możliwośc, ręcznego usunięcia go wtedy?)
         * 
         * -powinniśmy wysyłać jakąs informacje, ze dostalismy taki a taki keepalive do managementClas, który powinien to jakoś analizować
         * 
        */
        private void KeepAliveProcess()
        {

        }

        /*
         * Metoda odpowiedzialna za możliwośc wysyłania dowolnego zapytania do węzła aktywnego (konkretnie do jego agenta zarzadzania)
         * - 
        */
        public void SendRequest(string request)
        {

        }

        /*
         * Metoda odpowiedzialna za odbieranie wiadomości (odpowiedzi od agentów zarządzania)
         * -metoda powinna być wykonywana wtedy, gdy wątek nasłuchujący otrzyma wiadomośc pochodzacą od węzła,
         * którego id jest zapisane w tablicy aktywnych elementów sieci (wtedy domyśla się, że jest to odpowiedź)
         * jak odróżnić odpowiedź od keepalive? może tutaj będzie dopiero cały pakiet przerabiany na stringa
         * i jeżeli tresc pakietu to keepalive to przekieruj do metody odpowiedniej z przetwarzaniem tego typu wiadomosci
         * 
         * - metoda za parametr przyjmuję tablicę bajtów, w których zapisana jest treść pochodząca od agenta
         * (trzeba przerobić to na stringa i w zalezności od treści uruchomić odpowiedni fragment kodu, czy to w tej klasie,
         * czy to w obiekcie klasy management)
         */
        public void GetRespond(byte[] respond)
        {

        }


    }
}
