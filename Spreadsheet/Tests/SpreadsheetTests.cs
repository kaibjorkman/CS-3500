using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SS;
using System.Collections.Generic;
using Formulas;


namespace SS
{
    [TestClass]
    public class SpreadsheetTests
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
            sheet.SetCellContents("A1", empty);
        }

        /// <summary>
        /// Tests null exception
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestMethod2()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents(null, "string");
        }

        /// <summary>
        /// Tests invalid name exception  for SetCellContents method
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestMethod3()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("8", "string");

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
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.GetCellContents("878");

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
            sheet.SetCellContents(null, test);
        }

        /// <summary>
        /// Tests invalid name exception for SetCellContents method \
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestMethod8()
        {
            Formula test = new Formula("8 - 1");
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("48896", test);
        }

        /// <summary>
        /// Tests null name exception for etCellContents method 
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestMethod9()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents(null, 40);
        }

        /// <summary>
        /// Tests the GetNamesOfAllNoneEmptyCells
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(InvalidNameException))]
        public void TestMethod10()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("kai", 50);
           
            
            


        }

        /// <summary>
        /// Tests the GetNamesOfAllNoneemptyCells
        /// </summary>
        [TestMethod]
        public void TestMethod11()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("A1", 50);
            IEnumerable<string> set = sheet.GetNamesOfAllNonemptyCells();
            foreach(string element in set)
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
            Formula first = new Formula();
            sheet.SetCellContents("A1", first);
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
            sheet.SetCellContents("A1", first);
            sheet.SetCellContents("A1", second);
            Assert.AreEqual(sheet.GetCellContents("A1"), second);



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
            sheet.SetCellContents("A1", f1);
        }

        /// <summary>
        /// A more complex test for CircularException
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(CircularException))]
        public void TestMethod15()
        {
            Formula f1 = new Formula("A1+B1");
            Formula f2 = new Formula("A3*B4");
            Formula f3 = new Formula("E1+C1");
            Formula f4 = new Formula("C1-A3");
            Formula f5 = new Formula("A1");
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("D1", f1);
            sheet.SetCellContents("A1", f2);
            sheet.SetCellContents("B1", f2);
            sheet.SetCellContents("A3", f3);
            sheet.SetCellContents("B4", f4);
            sheet.SetCellContents("E1", 2);
            sheet.SetCellContents("C1", 6);
            sheet.SetCellContents("A3", f5);

        }

        // --------- Non-Exception Tests ------------ //

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
        public void TestMethod17()
        {
            Formula f1 = new Formula("C1 + B1");
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("A1", f1);
            object value = sheet.GetCellContents("A1");
            Assert.AreEqual(f1, value);
        }

        /// <summary>
        /// Tests for SetCellContents 
        /// </summary>
        [TestMethod]
        public void TestMethod18()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("A1", 10.0);
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
            sheet.SetCellContents("A1", "Hello World");
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
            sheet.SetCellContents("A1", "Hello World");
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
            sheet.SetCellContents("A1", f1);
            object value = sheet.GetCellContents("A1");
            Assert.AreEqual(f1 , value);
        }

        /// <summary>
        /// Tests for SetCellContents (number)
        /// </summary>
        [TestMethod]
        public void TestMethod22()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("A1", 10.0);
            sheet.SetCellContents("A1", 11.0);
            Assert.AreEqual(sheet.GetCellContents("A1"), 11.0);
        }

        /// <summary>
        /// Tests for SetCellContents (string)
        /// </summary>
        [TestMethod]
        public void TestMethod23()
        {
            AbstractSpreadsheet sheet = new Spreadsheet();
            sheet.SetCellContents("A1", "kai");
            sheet.SetCellContents("A1", "new");
            Assert.AreEqual(sheet.GetCellContents("A1"), "new");
        }


    }
}