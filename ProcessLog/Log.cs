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
        
        public static string LogFileName;
        


        public static void Init(string fileName)
        {
            LogFileName = fileName;

            File.WriteAllText(fileName,);
        }

        public static void Add(string line)
        {
            
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
