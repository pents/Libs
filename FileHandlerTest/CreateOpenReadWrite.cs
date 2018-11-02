using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FileHandlerTest
{
    [TestClass]
    public class CreateOpenReadWrite
    {
        [TestMethod]
        public void CreateTest()
        {
            TestFile testFile = new TestFile();
            TestFileData filedata = new TestFileData();

            testFile.CreateFile(@"\\192.168.16.50\bazaotd\Давыдов Д.В\testFile.tst");
        }

        [TestMethod]
        public void WriteFile()
        {
            TestFile testFile = new TestFile();
            TestFileData filedata = new TestFileData();
            filedata.Day = 1;
            filedata.Month = 8;
            filedata.Year = 2018;
            testFile.WriteToFile(filedata, @"\\192.168.16.50\bazaotd\Давыдов Д.В\testFile.tst");
        }

        [TestMethod]
        public void ReadFile()
        {
            TestFile testFile = new TestFile();
            TestFileData filedata = testFile.ReadFile(@"\\192.168.16.50\bazaotd\Давыдов Д.В\testFile.tst");
            Assert.AreEqual(filedata.Day, 1);
            Assert.AreEqual(filedata.Month, 8);
            Assert.AreEqual(filedata.Year, 2018);
        }
    }
}
