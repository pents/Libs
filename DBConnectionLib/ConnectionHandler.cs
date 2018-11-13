using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace DBConnectionLib
{
    /// <summary>
    /// Исключение вызываемое если при обработке запроса произошла ошибка
    /// </summary>
    public class QueryErrorException : Exception
    {
        public QueryErrorException() { }
        public QueryErrorException(string message) : base(message) { }
    }

    /// <summary>
    /// Исключение вызываемое при сбое/ошибке подключения к БД
    /// </summary>
    public class DataBaseConnectionErrorException : Exception
    {
        public DataBaseConnectionErrorException() { }
        public DataBaseConnectionErrorException(string message) : base(message) { }
    }

    /// <summary>
    /// Структура данных запроса с ошибкой
    /// </summary>
    public struct ErrorQuery
    {
        /// <summary>
        /// Запрос выдавший ошибку
        /// </summary>
        public string Query;

        /// <summary>
        /// Сообщение об ошибке
        /// </summary>
        public string Message;

        /// <summary>
        /// Конструктор 
        /// </summary>
        /// <param name="query">запрос</param>
        /// <param name="msg">ошибка</param>
        public ErrorQuery(string query, string msg)
        {
            Query = query;
            Message = msg;
        }
    }




    /// <summary>
    /// Класс, поддерживающий соединение с заданной БД
    /// </summary>
    public class ConnectionHandler : IDisposable
    {
        /// <summary>
        /// Объект подключения к БД
        /// </summary>
        private SqlConnection sConnect;
        /// <summary>
        /// Объект строки настроек подключения
        /// </summary>
        public static SqlConnectionStringBuilder conStr { private get; set; }
        /// <summary>
        /// Объект текущей транзакции данных
        /// </summary>
        private SqlTransaction sTrans;
        /// <summary>
        /// Единая точка доступа к подключению
        /// </summary>
        private static ConnectionHandler Instance;

        /// <summary>
        /// Объект для закрытия доступа нескольким потокам 
        /// </summary>
        private readonly object _lockObj = new object();

        /// <summary>
        /// Вызов единого экземпляра класса ConnectionHandler
        /// </summary>
        public static ConnectionHandler GetInstance()
        {
            if (Instance == null)
            {
                Instance = new ConnectionHandler();           
            }
            return Instance;
        }
        /// <summary>
        /// Конструктор соединения, устанавливает соединение с данный БД по заданным параметрам
        /// </summary>
        private ConnectionHandler()
        {   
            // Не найдены настройки подключения
            if (conStr == null) throw new DataBaseConnectionErrorException("Не обнаружен объект SqlConnectionStringBuilder");

            sConnect = new SqlConnection(conStr.ConnectionString);

            Console.WriteLine("Соединение с БД успешно установлено");
        }

        /// <summary>
        /// Подключение к БД
        /// </summary>
        public void OpenConnection()
        {
            try
            {
                if (sConnect.State == ConnectionState.Open) return;
                sConnect.Open();
            }
            catch (Exception e)
            {
                throw new DataBaseConnectionErrorException(string.Format("Ошибка подключения к базе данных - {0}", e.Message));
            }

        }

        /// <summary>
        /// Отправка запроса к БД, не возвращает результат
        /// </summary>
        public void ExecuteQuery(string Query)
        {
            try
            {

                sTrans = sConnect.BeginTransaction();
                SqlCommand command = new SqlCommand(Query, sConnect, sTrans);


                command.ExecuteNonQuery();
                sTrans.Commit();
            }
            catch (Exception e)
            {
                sTrans.Rollback();
                throw new QueryErrorException(string.Format("Ошибка в запросе {0} \n {1}", Query, e.Message));
            }

        }

        /// <summary>
        /// Отправка группы запрсов к БД
        /// </summary>
        public void ExecuteQuery(IEnumerable<string> Query)
        {
            string currentQuery = "";
            try
            {

                sTrans = sConnect.BeginTransaction();
                SqlCommand command = new SqlCommand();
                command.Connection = sConnect;
                command.Transaction = sTrans;
                foreach (var line in Query)
                {
                    currentQuery = line;
                    command.CommandText = line;
                    command.ExecuteNonQuery();
                }
                sTrans.Commit();
            }
            catch (DataBaseConnectionErrorException e)
            {
                throw new DataBaseConnectionErrorException(e.Message);
            }
            catch (Exception e)
            {
                sTrans.Rollback();
                throw new QueryErrorException(string.Format("Ошибка в запросе {0} \n {1}", currentQuery, e.Message));
            }

        }
        /// <summary>
        /// Отправка группы запрсов к БД
        /// </summary>
        /// <param name="Query">Список запросов</param>
        /// <param name="ErrorQueries">Запросы вызвавшие исключение</param>
        public void ExecuteQuery(IEnumerable<string> Query, ref List<ErrorQuery> ErrorQueries)
        {
            string currentQuery = "";
            try
            {

                sTrans = sConnect.BeginTransaction();
                SqlCommand command = new SqlCommand();
                command.Connection = sConnect;
                command.Transaction = sTrans;
                foreach (var line in Query)
                {
                    try
                    {
                        currentQuery = line;
                        command.CommandText = line;
                        command.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        ErrorQueries.Add(new ErrorQuery(line, e.Message));
                        if (ErrorQueries.Count >= (from q in Query select q).Count())
                        {
                            throw new Exception(e.Message, e);
                        }
                    }

                }
                sTrans.Commit();
            }
            catch (DataBaseConnectionErrorException e)
            {
                throw new DataBaseConnectionErrorException(e.Message);
            }
            catch (Exception e)
            {
                sTrans.Rollback();
                throw new QueryErrorException(string.Format("Ошибка в запросе {0} \n {1}", currentQuery, e.Message));
            }

        }

        /// <summary>
        /// Вставить множество строк в заданную таблицу
        /// </summary>
        public void InsertBulkQuery(DataTable dataTable, string tableName)
        {
            lock(_lockObj)
            {


                var bulkCopy = new SqlBulkCopy(sConnect, SqlBulkCopyOptions.TableLock, null)
                {
                    DestinationTableName = tableName,
                    BatchSize = dataTable.Rows.Count
                };

                bulkCopy.WriteToServer(dataTable);


                bulkCopy.Close();
            }

        }

        /// <summary>
        /// Отправляет запрос к БД и возвращает первую строку первого столбца результата запроса
        /// </summary>
        /// <param name="Query">Запрос</param>
        public string ExecuteOneElemQuery(string Query)
        {

            SqlCommand res = new SqlCommand(Query, sConnect);

            object result = res.ExecuteScalar();

            return result == null ? "0" : result.ToString();
        }

        /// <summary>
        /// Проводит запрос и возвращает данные как объект "System.Data.Set"
        /// </summary>
        public DataSet GetDataTable(string Query)
        {
            DataSet ds = new DataSet("TableDataSet");
            DataTable dt = new DataTable("New_WorkSheet");



            SqlCommand command = new SqlCommand(Query);
            command.Connection = sConnect;
            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = command;
            adapter.Fill(dt);


            ds.Tables.Add(dt);

            return ds;
        }

        /// <summary>
        /// Проводит запрос и возвращает получившуюся таблицу как List типа object
        /// </summary>
        public List<object[]> GetTableData(string Query)
        {
            var result = new List<object[]>();
            try
            {
                var command = new SqlCommand(Query, sConnect);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        List<object> semiRes = new List<object>();
                        for (int i = 0; i < reader.FieldCount; ++i)
                        {
                            semiRes.Add(reader.GetValue(i));
                        }
                        result.Add(semiRes.ToArray());
                    }
                }

            }
            catch (DataBaseConnectionErrorException e)
            {
                throw new DataBaseConnectionErrorException(string.Format("Не удалось обработать запрос {0}\n {1}", Query, e.Message));
            }
            catch (Exception e)
            {
                throw new QueryErrorException(string.Format("Не удалось обработать запрос {0}\n {1}", Query, e.Message));
            }

            return result;

        }

        /// <summary>
        /// Закрытие соединения при ненадобности
        /// </summary>
        public void Dispose()
        {
            sConnect.Close();
        }
    }
}
