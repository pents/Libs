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
        private static SqlConnectionStringBuilder conStrr;

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
            conStrr = new SqlConnectionStringBuilder();
            conStrr.IntegratedSecurity = true; // true for testing purposes
            conStrr.DataSource = "10.255.7.203";
            conStrr.InitialCatalog = "URV";
            ConnectionHandler.conStr = conStrr;
            ConnectionHandler testCon = ConnectionHandler.GetInstance();
            Assert.IsNotNull(testCon);
        }

        [TestMethod]
        public void DBQueryTest()
        {
            conStrr = new SqlConnectionStringBuilder();
            conStrr.DataSource = "10.255.7.203";
            conStrr.InitialCatalog = "SGT_MMC";
            //conStr.IntegratedSecurity = true; // true for testing purposes
            conStrr.UserID = "UsersSGT";
            conStrr.Password = "123QWEasd";
            conStrr.PersistSecurityInfo = true;

            ConnectionHandler.conStr = conStrr;

            ConnectionHandler testCon = ConnectionHandler.GetInstance();

            Assert.AreEqual(7, testCon.ExecuteOneElemQuery("SELECT ID FROM SGT_MMC.dbo.TBDetailID WHERE Detail = 'БА8226305-02'"));
        }
        
    }
}
