using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.SqlClient;
using DBConnectionLib;

namespace ConnectionHandlerTest
{

    /// <summary>
    /// Клас для модульных тестов
    /// </summary>
    [TestClass]
    public class DBConnectionTest
    {
        private static SqlConnectionStringBuilder conStr;

        /// <summary>
        /// Модульный тест на проверку нужной ошибки в случае неправильного задания строки подключения
        /// </summary>
        [ExpectedException(typeof(DataBaseConnectionErrorException))]
        [TestMethod]
        public void DBConnectionFAILTest()
        {
            ConnectionHandler testCon = ConnectionHandler.GetInstance();
        }

        /// <summary>
        /// Модульный тест на проверку правильность работы задания строки подключения
        /// </summary>
        [TestMethod]
        public void DBConnectionOKTest()
        {
            conStr = new SqlConnectionStringBuilder();
            conStr.IntegratedSecurity = true; // true for testing purposes
            conStr.DataSource = "10.255.7.203";
            conStr.InitialCatalog = "URV";
            ConnectionHandler.conStr = conStr;
            ConnectionHandler testCon = ConnectionHandler.GetInstance();
            Assert.IsNotNull(testCon);
        }


        
    }
}
