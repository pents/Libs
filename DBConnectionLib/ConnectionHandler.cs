using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;

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
    public class ConnectionHandler
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
        /// Определяет, установленно ли подключение к БД
        /// </summary>
        public bool Connected { get; private set; }

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
        /// Отправка запроса к БД, не возвращает результат
        /// </summary>
        public void ExecuteQuery(string Query)
        {
            using (sConnect = new SqlConnection(conStr.ConnectionString))
            {
                sConnect.Open();
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
        }

        /// <summary>
        /// Отправка группы запрсов к БД
        /// </summary>
        public void ExecuteQuery(IEnumerable<string> Query)
        {
            using (sConnect = new SqlConnection(conStr.ConnectionString))
            {
                sConnect.Open();
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

        }
        /// <summary>
        /// Отправка группы запрсов к БД
        /// </summary>
        /// <param name="Query">Список запросов</param>
        /// <param name="ErrorQueries">Запросы вызвавшие исключение</param>
        public void ExecuteQuery(IEnumerable<string> Query, ref List<ErrorQuery> ErrorQueries)
        {
            using (sConnect = new SqlConnection(conStr.ConnectionString))
            {
                sConnect.Open();
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

        }

        /// <summary>
        /// Вставить множество строк в заданную таблицу
        /// </summary>
        public void InsertBulkQuery(DataTable dataTable, string tableName)
        {
            using (sConnect = new SqlConnection(conStr.ConnectionString))
            {
                sConnect.Open();         
                using (var bulkCopy = new SqlBulkCopy(sConnect, SqlBulkCopyOptions.TableLock, null))
                {
                    foreach(DataColumn column in dataTable.Columns)
                    {
                        bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                    }
                    
                    bulkCopy.DestinationTableName = tableName;
                    bulkCopy.BatchSize = dataTable.Rows.Count;
                    bulkCopy.BulkCopyTimeout = 12000;
                    bulkCopy.WriteToServer(dataTable);
                }
            }
        }

        /// <summary>
        /// Обновления множества строк в таблице, если строка не найдена - она добавляется 
        /// </summary>
        /// <param name="dataTable">Таблица данных</param>
        /// <param name="tableName">Имя целевой таблицы в БД</param>
        public void UpdateBulkQuery(DataTable dataTable, string tableName)
        {
            Dictionary<Type, string> typeMap = new Dictionary<Type, string>()
            {
                { typeof(string), "nvarchar(MAX)"},
                { typeof(int), "int"},
                { typeof(bool), "bit"},
                { typeof(float), "real"}
            };

            List<string> columnsProjection = new List<string>();
            // for is faster than foreach
            for(int i = 0; i < dataTable.Columns.Count; ++i)
            {
                columnsProjection.Add(string.Format("{0} {1}", dataTable.Columns[i].ColumnName, typeMap[dataTable.Columns[i].GetType()]));
            }

            string tmpTable = string.Format("CREATE TABLE #TmpDataTable ({0})", string.Join(",", columnsProjection.ToArray()));
            lock(this)
            {
                using (sConnect = new SqlConnection(conStr.ConnectionString))
                {
                    sConnect.Open();

                    SqlCommand cmd = new SqlCommand(tmpTable, sConnect);
                    cmd.ExecuteNonQuery();
                    using (var bulkCopy = new SqlBulkCopy(sConnect, SqlBulkCopyOptions.TableLock, null))
                    {
                        bulkCopy.DestinationTableName = "#TmpDataTable";
                        bulkCopy.WriteToServer(dataTable);

                        List<string> mergeMapColumns = new List<string>();
                        List<string> insertMapColumns = new List<string>();
                        for (int i = 0; i < dataTable.Columns.Count; ++i)
                        {
                            mergeMapColumns.Add(string.Format("Target.{0} = Source.{0}", dataTable.Columns[i].ColumnName));
                        }

                        for (int i = 0; i < dataTable.Columns.Count; ++i)
                        {
                            insertMapColumns.Add(string.Format("Source.{0}", dataTable.Columns[i].ColumnName));
                        }

                        string mergeSQL = string.Format("MERGE INTO {0} as Target USING #TmpDataTable as Source ON Target.{1} = Source.{1}" +
                                                        "WHEN MATCHED THEN UPDATE SET {2} " +
                                                        "WHEN NOT MATCHED THEN INSERT ({3}) VALUES ({4});",
                                                        tableName,
                                                        dataTable.Columns[0].ColumnName,
                                                        string.Join(",", mergeMapColumns.ToArray()),
                                                        string.Join(",", insertMapColumns.ToArray()));
                        cmd.CommandText = mergeSQL;
                        cmd.ExecuteNonQuery();

                        cmd.CommandText = "DROP TABLE #TmpDataTable";
                        cmd.ExecuteNonQuery();
                    }
                }
            }


        }


        /// <summary>
        /// Отправляет запрос к БД и возвращает первую строку первого столбца результата запроса
        /// </summary>
        /// <param name="Query">Запрос</param>
        public string ExecuteOneElemQuery(string Query)
        {
            lock(_lockObj)
            {
                using (sConnect = new SqlConnection(conStr.ConnectionString))
                {
                    sConnect.Open();
                    SqlCommand res = new SqlCommand(Query, sConnect);
                    object result = res.ExecuteScalar();
                    return result == null ? "0" : result.ToString();
                }
            }

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
        /// Проводит запрос и возвращает получившуюся таблицу как List"object"
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

    }
}
