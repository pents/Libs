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
        public static string LogFileName;
        

        /// <summary>
        /// Инициализация лог файла
        /// </summary>
        /// <param name="fileName">полное имя будущего файла</param>
        public static void Init(string fileName)
        {
            LogFileName = fileName;

            File.WriteAllText(fileName,string.Format("[LOG FILE CREATED {0}]", getCurrentDate()));
        }

        /// <summary>
        /// Добавление данной строки в файл
        /// <p>При записи в файл строка форматируется по форме [текущ. время]: Данная строка </p>
        /// </summary>
        public static void Add(string line)
        {
            File.AppendAllText(LogFileName, string.Format("[{0}]: ", getCurrentTime()));
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
