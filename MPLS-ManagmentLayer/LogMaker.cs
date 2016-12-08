using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MPLS_ManagmentLayer
{
    class LogMaker
    {

        private static string _fileLogPath;
        private static int _logID;


        public LogMaker()
        {
            //inicjalizacja ustalonej ściezki pliku zawierającego historię zdarzeń
            _fileLogPath = "LogDatabase.txt";

            //odczytujemy wartość ID ostatniego zdarzenia zapisanego w pliku
            InitializeLogLastIdNumber();

        }



        public static void MakeLog(string logDescription)
        {
            string log;

            using (StreamWriter file = new StreamWriter(_fileLogPath, true))
            {
                log = _logID + " | " + DateTime.Now.ToString("hh:mm:ss") + " " + logDescription;
                file.WriteLine(log);
                _logID++;
            }

            Console.WriteLine("\n" + log + "\n" );
        }


        private void InitializeLogLastIdNumber()
        {
            if (File.Exists(_fileLogPath))
            {
                string last = File.ReadLines(_fileLogPath).Last();
                string[] tmp = last.Split('|');
                _logID = Int32.Parse(tmp[0]);
                _logID++;
            }
            else
                _logID = 1;
        }
    }
}
