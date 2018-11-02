using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace FileHandler
{
    /// <summary>
    /// <para>Абстрактный класс обработки данных, подразумевается что вся 
    /// основная логика реализуется в дочерних класса</para>
    /// </summary>
    public abstract class File
    {
        /// <summary>
        /// Структура данных из файла
        /// </summary>
        public FileData data;

        /// <summary>
        /// Чтение файла
        /// </summary>
        protected virtual string[] ReadFile(string filePath)
        {
           return System.IO.File.ReadAllLines(filePath);
        }
        protected virtual string[] ReadFile(string filePath, Encoding encode)
        {
            return System.IO.File.ReadAllLines(filePath, encode);
        }
        /// <summary>
        /// Запись в БД
        /// </summary>
        protected virtual void WriteToDB(List<FileData> fileD, string tableName) { }

        /// <summary>
        /// Записывает в файл данные данной строки 
        /// </summary>
        /// <param name="path">путь к файлу записи</param>
        /// <param name="line">Строка данных</param>
        public virtual void WriteToFile(string path, string line)
        {
            StreamWriter fstream = System.IO.File.AppendText(path);
            fstream.WriteLine(line);
            fstream.Close();
        }

        /// <summary>
        /// Возвращает данные из файла как объект класса данных
        /// </summary>
        /// <returns></returns>
        public virtual FileData GetData()
        {
            return data;
        }

        /// <summary>
        /// Возвращает данные файла в виде массива строк
        /// </summary>
        public virtual List<string> GetDataStrings()
        {
            return data.getData();
        }

        /// <summary>
        /// Создает пустой файл в указанной директории
        /// </summary>
        /// <param name="filePath">полное имя файла с расширением</param>
        public void CreateFile(string filePath)
        {
            if(!System.IO.File.Exists(filePath))
            {
                FileStream fstream = System.IO.File.Create(filePath);
                fstream.Close();
            }
        }
    }
}
