using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XMLParser;
using System.IO;

namespace XMLFileTest
{
    [TestClass]
    public class XMLFile_ReadTest
    {
        [TestMethod]
        public void ReadXMLTest()
        {
            
            XMLFile testXML = new XMLFile(string.Format("{0}\\test.xml",Directory.GetCurrentDirectory()));

            string test = testXML.getNode("ChildNode1").getValue();

            Assert.AreEqual(test, "SomeValue1");

        }
    }
}
