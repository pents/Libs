using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CSharp;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;

namespace ExcelLib
{
    /// <summary>
    /// Лист Excel
    /// </summary>
    public class Worksheet
    {
        /// <summary>
        /// Лист Excel, используется полько внутренне
        /// </summary>
        private Microsoft.Office.Interop.Excel.Worksheet _ws;
        private Workbook _wb;
        /// <summary>
        /// Создание листа с назначением имени
        /// </summary>
        /// <param name="name">имя листа</param>
        public Worksheet(string name, Workbook wb)
        {
            _ws = wb.getCurrentBook().Worksheets.get_Item(wb.GetWBListCount()+1);
            _ws.Name = name;
            _wb = wb;
        }

        /// <summary>
        /// Внутренний конструктор
        /// </summary>
        /// <param name="workSheet">Объект Microsoft.Office.Interop.Excel передаваемый классу</param>
        public Worksheet(Microsoft.Office.Interop.Excel.Worksheet workSheet, Workbook wb)
        {
            _ws = workSheet;
            _wb = wb;
        }

        /// <summary>
        /// Создание листа с именем по умолчанию
        /// </summary>
        public Worksheet(Workbook wb)
        {
            _ws = wb.getCurrentBook().Worksheets.get_Item(wb.GetWBListCount()+1);
        }

        /// <summary>
        /// Принимает строку типа "A1","A20"
        /// Возвращает диапазон ячеек
        /// </summary>
        public Array getCellValue(string rngStart, string rngEnd)
        {
            return (Array)_ws.get_Range(rngStart, rngEnd).Cells.Value2;
        }

        /// <summary>
        /// Принимает строку типа "A1"
        /// Возвращает значение ячейки 
        /// </summary>
        public object getCellValue(string cell)
        {
            return _ws.get_Range(cell).Cells.Value;
        }

        /// <summary>
        /// Устанавливает значение ячейки равным данному \n
        /// <para>ЗНАЧЕНИЯ row И col НЕ МОГУТ БЫТЬ МЕНЬШЕ 1</para>
        /// </summary>
        public void setCellValue(int row, int col, dynamic Value)
        {
            _ws.Cells[row, col] = Value;
            //Save();
        }
        /// <summary>
        /// Удаление листа
        /// </summary>
        internal void Release()
        {
            Marshal.ReleaseComObject(_ws);
        }

        private void Save()
        {
            _wb.Save();
        }
    }
}
