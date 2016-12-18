using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace MPLS_ManagmentLayer
{
    class LogMaker
    {

        private static string _fileLogPath;
        private static int _logID;

        private static ReaderWriterLockSlim _writeLock = new ReaderWriterLockSlim();

        public LogMaker()
        {
            //inicjalizacja ustalonej ściezki pliku zawierającego historię zdarzeń
            _fileLogPath = "LogDatabase_Management.txt";

            //odczytujemy wartość ID ostatniego zdarzenia zapisanego w pliku
            InitializeLogLastIdNumber();
        }

        public static void MakeConsoleLog(string logDescription)
        {
            string log;
            log = _logID + " | " + DateTime.Now.ToString("hh:mm:ss") + " " + logDescription;
            Console.Write("\n" + log);
        }

        public static void MakeLog(string logDescription)
        {
            string log = "#" + _logID + " | " + DateTime.Now.ToString("hh:mm:ss") + " " + logDescription;
            _logID++;

            WriteToFileThreadSafe(log, _fileLogPath);
        }
        public static void WriteToFileThreadSafe(string text, string path)
        {
            // Set Status to Locked
            _writeLock.EnterWriteLock();
            try
            {
                // Append text to the file
                using (StreamWriter sw = File.AppendText(path))
                {
                    sw.WriteLine(text);
                    sw.Close();
                }
            }
            finally
            {
                // Release lock
                _writeLock.ExitWriteLock();
            }
        }

        private void InitializeLogLastIdNumber()
        {
            if (File.Exists(_fileLogPath))
            {
                string last = File.ReadLines(_fileLogPath).Last();
                string[] tmp = last.Split('|');

                string tmp2 = tmp[0].Substring(1);

                _logID = Int32.Parse(tmp2);
                _logID++;
            }
            else
                _logID = 1;
        }
    }
}
