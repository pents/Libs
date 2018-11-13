using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace ProcessLog
{
    public static class Log
    {
        /// <summary>
        /// Полное имя лог-фала
        /// </summary>
        public static string LogFileName { get; private set; }
        private static bool _logFileCreated = false;

        /// <summary>
        /// Инициализация лог файла
        /// </summary>
        /// <param name="fileName">полное имя будущего файла</param>
        public static void Init(string fileName)
        {
            File.WriteAllText(fileName,string.Format("[LOG FILE CREATED {0}]", getCurrentDate()));

            _logFileCreated = true;

            LogFileName = fileName;
        }

        /// <summary>
        /// Добавление данной строки в файл
        /// <p>При записи в файл строка форматируется по форме [текущ. время]: Данная строка </p>
        /// </summary>
        public static void Add(string line)
        {
            if (!_logFileCreated) throw new FileNotFoundException("Log file is not initialized");
            File.AppendAllText(LogFileName, string.Format("\n[{0}]: {1}", getCurrentTime(), line));
        }

        private static string getCurrentTime()
        {
            return string.Format("{0}:{1}:{2}", DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
        }

        private static string getCurrentDate()
        {
            return string.Format("{0}/{1}/{2}", DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);
        }
    }

   

}
