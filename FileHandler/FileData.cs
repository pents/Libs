using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FileHandler
{
    /// <summary>
    /// Класс структуры данных файла
    /// <para>Дополнительные атрибуты добавляются в дочерних классах по мере надобности</para>
    /// </summary>
    [Serializable]
    public abstract class FileData
    {

        public FileData() { }

        /// <summary>
        /// Для копирования данных одного объекта в другой
        /// </summary>
        /// <param name="data">_fileData другого объекта</param>
        public FileData(IEnumerable<string> data)
        {
            _filedata = data.ToList();
            PropertyInfo[] properties = GetType().GetProperties();
            for (int i = 0; i < properties.Count(); ++i)
            {
                if (properties[i].Name != "_filedata")
                {
                    properties[i].SetValue(this, _filedata[i]);
                }
            }
        }

        /// <summary>
        /// Данные из файла в строковом виде
        /// </summary>
        private List<string> _filedata;

        /// <summary>
        /// Сборка массива данных из имеющихся в объекте параметров
        /// </summary>
        private void GeneratefileData()
        {
            PropertyInfo[] properties = GetType().GetProperties();
            foreach(var property in properties)
            {
                if (property.Name != "_filedata")
                {
                    _filedata.Add(property.GetValue(this).ToString());
                }
            }
        }

        /// <summary>
        /// Метод доступа к данным из файла
        /// </summary>
        public List<string> getData()
        {
            if (_filedata == null)
            {
                _filedata = new List<string>();
                GeneratefileData();
            }
            
            return _filedata;
        }
      

    }
}
