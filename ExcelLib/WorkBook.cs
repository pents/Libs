using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CSharp;
using Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using System.Reflection;
using System.IO;

namespace ExcelLib
{
    public class Workbook
    {
        /// <summary>
        /// Объект Excel приложения
        /// </summary>
        private Application _app;
        private List<Worksheet> _wsList;

        /// <summary>
        /// Возвращает количество листов в текущей книге
        /// </summary>
        /// <returns></returns>
        public int GetWBListCount()
        {
            return _wsList.Count();
        }
        /// <summary>
        /// Текущая рабочая книга, используется только внутренне
        /// </summary>
        private Microsoft.Office.Interop.Excel.Workbook _CurrentWorkbook;
        internal Microsoft.Office.Interop.Excel.Workbook getCurrentBook()
        {
            return _CurrentWorkbook;
        }

        /// <summary>
        /// Открывает книгу Excel по данному пути
        /// </summary>
        /// <param name="name"></param>
        public Workbook(string name)
        {
            _wsList = new List<Worksheet>();
            OpenWorkbook(name);
        }

        /// <summary>
        /// Возвращает лист относительно заданного индекса
        /// </summary>
        public Worksheet getWorksheet(int index)
        {
            return _wsList[index];
        }

        /// <summary>
        /// Инициализирует объект рабочей киниги
        /// </summary>
        public Workbook()
        {
            _wsList = new List<Worksheet>();
        }

        /// <summary>
        /// Создает пустую рабочую книгу
        /// </summary>
        public void CreateWorkbook()
        {
            _app = new Application();
            _app.Visible = false;
            _app.UserControl = false;
            _app.DisplayAlerts = false;
            _app.ScreenUpdating = true;
            _CurrentWorkbook = _app.Workbooks.Add(Missing.Value);
        }

        /// <summary>
        /// Открытие Excel книги
        /// </summary>
        /// <param name="name">Полное имя файла книги</param>
        /// <returns>объект данной книги</returns>
        private void OpenWorkbook(string name)
        {
            _app = new Application();
            _app.Visible = false;
            _app.UserControl = false;
            _app.DisplayAlerts = false;
            _app.ScreenUpdating = true;
            _CurrentWorkbook = _app.Workbooks.Open(name, 0, true);
            foreach(var sheet in _CurrentWorkbook.Worksheets)
            {
                _wsList.Add(new Worksheet((Microsoft.Office.Interop.Excel.Worksheet)sheet, this));
            }
        }

        /// <summary>
        /// Сохраняет текущее состояние книги
        /// </summary>
        public void Save()
        {
            _CurrentWorkbook.Save();
        }

        /// <summary>
        /// Сохраняет книгу по заданному пути
        /// </summary>
        /// <param name="FullFilePath">путь к новому файлу</param>
        public void SaveAs(string FullFilePath)
        {
            if (File.Exists(FullFilePath)) throw new FileExistsException(string.Format("Файл {0}\n уже существует", FullFilePath));
            _CurrentWorkbook.SaveCopyAs(FullFilePath);
        }

        /// <summary>
        /// Закрытие приложения - освобождение всей занимаемой Excel COM объектами памяти
        /// </summary>
        public void Quit()
        {
            _CurrentWorkbook.Close(true, _CurrentWorkbook.FullName);
            _app.Quit();
            _app.ScreenUpdating = true;
            foreach (var worksheet in _wsList)
            {
                worksheet.Release();
            }
            Marshal.ReleaseComObject(_CurrentWorkbook);
            Marshal.ReleaseComObject(_app);
        }

        /// <summary>
        /// Создает новый лист, приписывает его книге и возвращает
        /// </summary>
        /// <param name="name">Название листа</param>
        public Worksheet CreateWorksheet(string name)
        {
            
            Worksheet newWs = new Worksheet(name, this);
            _wsList.Add(newWs);
            return newWs;
        }
        /// <summary>
        /// Создает новый лист, приписывает его книге и возвращает
        /// </summary>
        public Worksheet CreateWorksheet()
        {
            Worksheet newWs = new Worksheet(this);
            _wsList.Add(newWs);
            return newWs;
        }
    }

    public class FileExistsException : Exception
    {
        public FileExistsException() { }
        public FileExistsException(string msg) : base(msg) { }
    }
}
