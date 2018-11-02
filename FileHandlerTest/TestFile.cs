using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileHandler;

namespace FileHandlerTest
{
    public class TestFile : File
    {
        public new TestFileData ReadFile(string filePath)
        {
            TestFileData testData = new TestFileData();
            string[] data = base.ReadFile(filePath);
                            
            testData.Day = int.Parse(data[0]);
            testData.Month = int.Parse(data[1]);
            testData.Year = int.Parse(data[2]);
            base.data = testData;

            return testData;         
        }

        public void WriteToFile(TestFileData data, string path)
        {
            //CreateFile(path);

            base.WriteToFile(path, data.Day.ToString());
            base.WriteToFile(path, data.Month.ToString());
            base.WriteToFile(path, data.Year.ToString());
        }

        protected override void WrightToDB()
        {
            throw new NotImplementedException();
        }
    }
}
