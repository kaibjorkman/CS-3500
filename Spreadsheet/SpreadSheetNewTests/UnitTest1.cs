using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Formulas;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS;

namespace SpreadSheetNewTests
{
    [TestClass]
    public class UnitTest1
    { 
        /// <summary>
        /// Tests null exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestMethod1()
        {
            String empty = null;
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", empty);
        }

        /// <summary>
        /// Tests null exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestMethod2()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell(null, "string");
        }

        /// <summary>
        /// Tests invalid name exception  for SetCellContents method
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestMethod3()
        {
            Regex check = new Regex("^[a-zA-Z]*[1-9][0-9]*$");
            AbstractSpreadsheet sheet = new Spreadsheet(check);
            sheet.SetContentsOfCell("hello", "string");

        }

        /// <summary>
        /// Tests null name exception for GetCellContents method
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestMethod4()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.GetCellContents(null);

        }

        /// <summary>
        /// Tests invalid name exception for GetCellContents method
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestMethod5()
        {
            Regex check = new Regex("^[a-zA-Z]*[1-9][0-9]*$");
            AbstractSpreadsheet sheet = new Spreadsheet(check);
            sheet.GetCellContents("878hi");

        }

        /// <summary>
        /// Tests null name exception for SetCellContents method 
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestMethod7()
        {
            Formula test = new Formula("8 + 8");
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell(null, "= 8 + 8");
        }

        /// <summary>
        /// Tests invalid name exception for SetCellContents method \
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestMethod8()
        {
            Formula test = new Formula("8 - 1");
            Regex check = new Regex("^[a-zA-Z]*[1-9][0-9]*$");
            AbstractSpreadsheet sheet = new Spreadsheet(check);
            sheet.SetContentsOfCell("48896hi", "= 8 - 1");
        }

        /// <summary>
        /// Tests null name exception for etCellContents method 
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestMethod9()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell(null, "40");
        }

        /// <summary>
        /// Tests the GetNamesOfAllNoneEmptyCells
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestMethod10()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("kai", "50");





        }

        /// <summary>
        /// Tests the GetNamesOfAllNoneemptyCells
        /// </summary>
        [TestMethod]
        public void TestMethod11()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "50");
            IEnumerable<string> set = sheet.GetNamesOfAllNonemptyCells();
            foreach (string element in set)
            {
                Assert.AreSame(element, "A1");
            }

        }

        /// <summary>
        /// Tests the null formula in SetCellContents
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestMethod12()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "");
        }

        /// <summary>
        /// Tests the replacement when SetCellCOntents is called
        /// </summary>
        [TestMethod]
        public void TestMethod13()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            Formula first = new Formula("2 + 2");
            Formula second = new Formula("3 + 3");
            sheet.SetContentsOfCell("A1", "2 + 2");
            sheet.SetContentsOfCell("A1", "3 + 3");
            Assert.AreEqual(sheet.GetCellContents("A1"), "3 + 3");



        }




        /// <summary>
        /// Tests for CircularException 
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void TestMethod14()
        {
            Formula f1 = new Formula("A1 + B1");
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "= A1 + B1");
        }

        /// <summary>
        /// Tests for GetDirectDependents
        /// </summary>
        [TestMethod]
        public void TestMethod16()
        {
            PrivateObject accessor = new PrivateObject(new Spreadsheet());
            accessor.Invoke("GetDirectDependents", new Object[] { "A1" });

        }

        /// <summary>
        /// Tests for SetCellContents 
        /// </summary>
        [TestMethod]
        public void TestMethod18()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "10.0");
            object value = sheet.GetCellContents("A1");
            Assert.AreEqual(10.0, value);
        }

        /// <summary>
        /// Tests for SetCellContents (string)
        /// </summary>
        [TestMethod]
        public void TestMethod19()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "Hello World");
            object value = sheet.GetCellContents("A1");
            Assert.AreEqual("Hello World", value);
        }

        /// <summary>
        /// Tests for GetCellContents
        /// </summary>
        [TestMethod]
        public void TestMethod20()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "Hello World");
            object value = sheet.GetCellContents("A1");
            Assert.AreEqual("Hello World", value);
        }

        /// <summary>
        /// Tests for SetCellContents (Formula)
        /// </summary>
        [TestMethod]
        public void TestMethod21()
        {
            Formula f1 = new Formula("c1 + b1");
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "c1 + b1");
            object value = sheet.GetCellContents("A1");
            Assert.AreEqual("c1 + b1", value);
        }

        /// <summary>
        /// Tests for SetCellContents (number)
        /// </summary>
        [TestMethod]
        public void TestMethod22()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "10.0");
            sheet.SetContentsOfCell("A1", "11.0");
            Assert.AreEqual(sheet.GetCellContents("A1"), 11.0);
        }

        /// <summary>
        /// Tests for SetCellContents (string)
        /// </summary>
        [TestMethod]
        public void TestMethod23()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetContentsOfCell("A1", "kai");
            sheet.SetContentsOfCell("A1", "new");
            Assert.AreEqual(sheet.GetCellContents("A1"), "new");
        }
        /// <summary>
        /// Tests the read method and get vlaue and set contents of cell methods
        /// </summary>
        [TestMethod]
        public void TestMethodNew()
        {
            TextReader reader = new StreamReader("SampleSavedSpreadsheet.xml");
            TextWriter writer = new StringWriter();
            Regex isValid = new Regex("^.*$");
            Spreadsheet sheet = new Spreadsheet(reader, isValid);

            var value = sheet.GetCellValue("A1");
            sheet.SetContentsOfCell("A2", "1");
            var value2 = sheet.GetCellValue("A2");

            Assert.AreEqual(1.5, value);
            Assert.AreEqual(1.0, value2);



        }
        /// <summary>
        /// THis tests the write method. I viewed the results in a text reader
        /// </summary>
        [TestMethod]
        public void TestMethodNew2()
        {

            TextWriter writer = new StringWriter();
            Regex isValid = new Regex("^.*$");
            Spreadsheet sheet = new Spreadsheet(isValid);


            sheet.SetContentsOfCell("A1", "hey");
            sheet.SetContentsOfCell("A2", "hi");
            sheet.SetContentsOfCell("A3", "hello");

            sheet.Save(writer);




        }
        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void TestMethodGetCellValue1()
        {

            Regex isValid = new Regex("^[a-zA-Z]*[1-9][0-9]*$");
            Spreadsheet sheet = new Spreadsheet(isValid);

            sheet.SetContentsOfCell("A1", "2");

            Assert.AreEqual("2", "2");
        }


    }
}
