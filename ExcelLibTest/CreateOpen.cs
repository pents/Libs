using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ExcelLib;

namespace ExcelLibTest
{
    [TestClass]
    public class CreateOpen
    {

        [TestMethod]
        public void CreateFileTest()
        {
            Workbook wb = new Workbook();
            try
            {
                wb.CreateWorkbook();
                Worksheet ws = wb.CreateWorksheet("SomeSheet");
                wb.SaveAs(@"\\192.168.16.50\bazaotd\Давыдов Д.В\CreateTest.xlsx");
            }
            catch(FileExistsException e)
            {

            }
            finally
            {
                wb.Quit();
            }  
        }

        [TestMethod]
        [ExpectedException(typeof(FileExistsException))]
        public void CreateExistedFileFail()
        {
            Workbook wb = new Workbook();
            wb.CreateWorkbook();
            Worksheet ws = wb.CreateWorksheet("SomeSheet");
            wb.SaveAs(@"\\192.168.16.50\bazaotd\Давыдов Д.В\CreateTest.xlsx");
            wb.Quit();
            
        }

        [TestMethod]
        public void OpenFileTest()
        {
            Workbook wb = new Workbook(@"\\192.168.16.50\bazaotd\Давыдов Д.В\CreateTest.xlsx");
            try
            {
                Worksheet ws = wb.getWorksheet(0);
                for (int i = 1; i < 20; ++i)
                {
                    ws.setCellValue(1, i, 42);
                }
                
                string testStr = ws.getCellValue("A1").ToString();
                Assert.AreEqual(testStr, "42");
                //wb.SaveAs(@"\\192.168.16.50\bazaotd\Давыдов Д.В\CreateTest.xlsx");
            }
            finally
            {
                wb.Quit();
            }
            
        }

    }
}
