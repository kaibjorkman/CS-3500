using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using Dependencies;
using Formulas;

namespace SS
{
   /// <summary>
   /// class to reperesent a spreadsheet
   /// </summary>
    public class Spreadsheet : AbstractSpreadsheet
    {
        // dictionary maps cell name (row, col) to the cell itself
        Dictionary<string, Cell> cells;

        // dependency graph for cells
        DependencyGraph graph;

        //Statrt a regex for validity of cell names
        Regex isValid;

        // track if the spreadsheet was modified
        private bool changed;

        

        /// <summary>
        /// Constructor creates a new spreadsheet and initialized variables with isValid that excepts everything
        /// </summary>
        public Spreadsheet()
        {
            //set the isValid to a regex expression that excepts everything
            isValid = new Regex("^.*$");

            // initialize spreadsheet variables;
            cells = new Dictionary<string, Cell>();
            graph = new DependencyGraph();

            Changed = false;
        }
        /// <summary>
        ///  Creates an empty Spreadsheet whose IsValid regular expression is provided as the parameter
        /// </summary>
        public Spreadsheet(Regex isValid)
        {
            //set the isValid to a regex expression that excepts everything
            this.isValid = isValid;

            // initialize spreadsheet variables;
            cells = new Dictionary<string, Cell>();
            graph = new DependencyGraph();

            Changed = false;
        }

        /// Creates a Spreadsheet that is a duplicate of the spreadsheet saved in source.
        ///
        /// See the AbstractSpreadsheet.Save method and Spreadsheet.xsd for the file format 
        /// specification.  
        ///
        /// If there's a problem reading source, throws an IOException.
        ///
        /// Else if the contents of source are not consistent with the schema in Spreadsheet.xsd, 
        /// throws a SpreadsheetReadException.  
        ///
        /// Else if the IsValid string contained in source is not a valid C# regular expression, throws
        /// a SpreadsheetReadException.  (If the exception is not thrown, this regex is referred to
        /// below as oldIsValid.)
        ///
        /// Else if there is a duplicate cell name in the source, throws a SpreadsheetReadException.
        /// (Two cell names are duplicates if they are identical after being converted to upper case.)
        ///
        /// Else if there is an invalid cell name or an invalid formula in the source, throws a 
        /// SpreadsheetReadException.  (Use oldIsValid in place of IsValid in the definition of 
        /// cell name validity.)
        ///
        /// Else if there is an invalid cell name or an invalid formula in the source, throws a
        /// SpreadsheetVersionException.  (Use newIsValid in place of IsValid in the definition of
        /// cell name validity.)
        ///
        /// Else if there's a formula that causes a circular dependency, throws a SpreadsheetReadException. 
        ///
        /// Else, create a Spreadsheet that is a duplicate of the one encoded in source except that
        /// the new Spreadsheet's IsValid regular expression should be newIsValid.
        public Spreadsheet(TextReader source, Regex newIsValid)
        {
            // initialize spreadsheet variables;
            cells = new Dictionary<string, Cell>();
            graph = new DependencyGraph();

            // Create an XmlSchemaSet object.
            XmlSchemaSet sc = new XmlSchemaSet();

            //Add the schema rules
            sc.Add(null, "Spreadsheet.xsd");

            // Configure validation.
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.Schema;
            settings.Schemas = sc;
            settings.ValidationEventHandler += ValidationCallback;

            //set up oldIsValid Regex
            Regex oldIsValid = new Regex("");

            using (XmlReader reader = XmlReader.Create(source, settings))
            {

                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        switch (reader.Name)
                        {
                            case "cell":
                                string key = reader["name"];
                                //check fr duplicate cell names
                                if (cells.Count() > 0)
                                {
                                    if (cells.ContainsKey(key.ToUpper()))
                                    {
                                        throw new SpreadsheetReadException("Duplicate name of Cell");
                                    }
                                }
                                //check for old validaty
                                isValid = oldIsValid;
                                try
                                {
                                    this.SetContentsOfCell(reader["name"], reader["contents"]);
                                }
                                catch(ArgumentException)
                                {
                                    throw new SpreadsheetReadException("Name or Formula was invalid against oldisValid");
                                }

                                //check for old validaty
                                isValid = newIsValid;
                                
                                if(!(this.IsValidName(reader["name"])))
                                {
                                    throw new SpreadsheetVersionException("Name or Formula was invalid against newisValid");
                                }
 

                                break;

                            case "spreadsheet":
                                try
                                {
                                    oldIsValid = new Regex(reader["IsValid"]); //set the old is valid statement for checking later
                                }
                                catch(ArgumentException)
                                {
                                    throw new SpreadsheetReadException("Not a valid Regex Expression");
                                }
                                break;
                        }
                    }
                }
            }
        }

        private void ValidationCallback(object sender, ValidationEventArgs e)
        {
            throw new SpreadsheetReadException(e.Message);
        }

        // ADDED FOR PS6
        /// <summary>
        /// True if this spreadsheet has been modified since it was created or saved
        /// (whichever happened most recently); false otherwise.
        /// </summary>
        public override bool Changed
        {
            get
            {
                return changed;
            }
            protected set
            {
                changed = value;
            }

        }

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, returns the contents (as opposed to the value) of the named cell.  The return
        /// value should be either a string, a double, or a Formula.
        /// </summary>
        public override object GetCellContents(string name)
        {
            if(name == null || !(IsValidName(name)))
            {
                throw new InvalidNameException();
            }

            Cell cellValue; // value of name

            if (cells.TryGetValue(name, out cellValue))
            {
                return cellValue.contents;
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// Enumerates the names of all the non-empty cells in the spreadsheet.
        /// </summary>
        public override IEnumerable<string> GetNamesOfAllNonemptyCells()
        {
            HashSet<string> nonEmpties = new HashSet<string>();

            foreach(KeyValuePair<string, Cell> pair in cells)
            {
                if(pair.Value.CheckCellContents())
                {
                    nonEmpties.Add(pair.Key);
                }
            }
            
            return nonEmpties;
        }
        

        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes number.  The method returns a
        /// set consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        protected override ISet<string> SetCellContents(string name, double number)
        {
            if (name == null || !(IsValidName(name)))
            {
                throw new InvalidNameException();
            }

            // new cell
            Cell cell = new Cell(number);

            if (cells.ContainsKey(name))    // if it already contains that key
            {
                cells[name] = cell;
            }                           // replace the key with the new value
            else
            {
                cells.Add(name, cell);      // otherwise add a new key for that value
            }
            // replace the dependents of 'name' in the dependency graph with an empty hash set
            graph.ReplaceDependees(name, new HashSet<String>());

            // recalculate at end
            HashSet<String> all_dependees = new HashSet<String>(GetCellsToRecalculate(name));

            return all_dependees;


        }

        /// <summary>
        /// If text is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, the contents of the named cell becomes text.  The method returns a
        /// set consisting of name plus the names of all other cells whose value depends, 
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        protected override ISet<string> SetCellContents(string name, string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException();
            }

            if (name == null || !(IsValidName(name)))
            {
                throw new InvalidNameException();
            }

            //new cell
            Cell cell = new Cell(text);
            if (cells.ContainsKey(name))    // if it already contains that key
            {
                cells[name] = cell;         // replace the key with the new value
            }
            else
            {
                cells.Add(name, cell);      // otherwise add a new key for that value
            }
            
            // replace the dependents of 'name' in the dependency graph with an empty hash set to make room for recalculation
            graph.ReplaceDependees(name, new HashSet<String>());

            // recalculate cells
            HashSet<String> all_dependees = new HashSet<String>(GetCellsToRecalculate(name));
            return all_dependees;
        }

        /// <summary>
        /// Requires that all of the variables in formula are valid cell names.
        /// 
        /// If name is null or invalid, throws an InvalidNameException.
        /// 
        /// Otherwise, if changing the contents of the named cell to be the formula would cause a 
        /// circular dependency, throws a CircularException.
        /// 
        /// Otherwise, the contents of the named cell becomes formula.  The method returns a
        /// Set consisting of name plus the names of all other cells whose value depends,
        /// directly or indirectly, on the named cell.
        /// 
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        protected override ISet<string> SetCellContents(string name, Formula formula)
        {
            if (ReferenceEquals(formula, null))
            {
                throw new ArgumentNullException();
            }

            if (name == null || !(IsValidName(name)))
            {
                throw new InvalidNameException();
            }

            //save the old dependees if it ends up being a circular exception
            IEnumerable<String> old_dependees = graph.GetDependees(name);

            // replace the dependents of 'name' in the dependency graph with the variables in formula
            graph.ReplaceDependees(name, formula.GetVariables());

            try // check if the new depdendency graph creates a circular reference
            {
                
                HashSet<String> all_dependees = new HashSet<String>(GetCellsToRecalculate(name));

                // new cell
                Cell cell = new Cell(formula);
                cell.value = formula.Evaluate(Lookup);

                if (cells.ContainsKey(name))    // if it already contains that key
                {
                    cells[name] = cell;         // replace the key with the new value
                }
                else
                {
                    cells.Add(name, cell);      // otherwise add a new key for that value
                }

                return all_dependees;
            }
            catch (CircularException e) // if an exception is caught, we want to keep the old dependents and not change the cell
            {
                graph.ReplaceDependees(name, old_dependees);

                throw new CircularException();
            }
        }

        /// <summary>
        /// If name is null, throws an ArgumentNullException.
        /// 
        /// Otherwise, if name isn't a valid cell name, throws an InvalidNameException.
        /// 
        /// Otherwise, returns an enumeration, without duplicates, of the names of all cells whose
        /// values depend directly on the value of the named cell.  In other words, returns
        /// an enumeration, without duplicates, of the names of all cells that contain
        /// formulas containing name.
        /// 
        /// For example, suppose that
        /// A1 contains 3
        /// B1 contains the formula A1 * A1
        /// C1 contains the formula B1 + A1
        /// D1 contains the formula B1 - C1
        /// The direct dependents of A1 are B1 and C1
        /// </summary>
        protected override IEnumerable<string> GetDirectDependents(string name)
        {
            if(name == null)
            {
                throw new ArgumentNullException();
            }

            if(!(IsValidName(name)))
            {
                throw new InvalidNameException();
            }

                return graph.GetDependents(name); //get the dependents from corrisponding name
            
                
        }

        /// <summary>
        /// Private helper method to check if the name of a cell is valid or not
        /// </summary>
        private Boolean IsValidName(String name)
        {
            // if it is a valid cell name return true, else return false
            if (Regex.IsMatch(name, "^[a-zA-Z]*[1-9][0-9]*$"))
            {
                if (isValid.IsMatch(name.ToUpper()))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// ADDED FOR PS6
        /// <summary>
        /// Writes the contents of this spreadsheet to dest using an XML format.
        /// The XML elements should be structured as follows:
        ///
        /// <spreadsheet IsValid="IsValid regex goes here">
        ///   <cell name="cell name goes here" contents="cell contents go here"></cell>
        ///   <cell name="cell name goes here" contents="cell contents go here"></cell>
        ///   <cell name="cell name goes here" contents="cell contents go here"></cell>
        /// </spreadsheet>
        ///
        /// The value of the IsValid attribute should be IsValid.ToString()
        /// 
        /// There should be one cell element for each non-empty cell in the spreadsheet.
        /// If the cell contains a string, the string (without surrounding double quotes) should be written as the contents.
        /// If the cell contains a double d, d.ToString() should be written as the contents.
        /// If the cell contains a Formula f, f.ToString() with "=" prepended should be written as the contents.
        ///
        /// If there are any problems writing to dest, the method should throw an IOException.
        /// </summary>
        public override void Save(TextWriter dest)
        {
            using (XmlWriter writer = XmlWriter.Create(dest))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("", "spreadsheet", "Spreadsheet.xsd");
                writer.WriteAttributeString("isValid", isValid.ToString());//set the isvalid tag to the isvalid regex string

                foreach (String element in cells.Keys)
                {
                    writer.WriteStartElement("cell");
                    writer.WriteAttributeString("name", element);
                    writer.WriteAttributeString("contents", cells[element].contents.ToString());
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
            }
        }

        // ADDED FOR PS6
        /// <summary>
        /// If name is null or invalid, throws an InvalidNameException.
        ///
        /// Otherwise, returns the value (as opposed to the contents) of the named cell.  The return
        /// value should be either a string, a double, or a FormulaError.
        /// </summary>
        public override object GetCellValue(string name)
        {
            // if name is null or invalid, throw exception
            if (ReferenceEquals(name, null) || !(IsValidName(name)))
            {
                throw new InvalidNameException();
            }

            Cell cell; // value of name

            // Otherwise return the value of the named cell
            if (cells.TryGetValue(name, out cell))
            {
                return cell.value;
            }
            else
            {
                return "";
            }
        }

        // ADDED FOR PS6
        /// <summary>
        /// If content is null, throws an ArgumentNullException.
        ///
        /// Otherwise, if name is null or invalid, throws an InvalidNameException.
        ///
        /// Otherwise, if content parses as a double, the contents of the named
        /// cell becomes that double.
        ///
        /// Otherwise, if content begins with the character '=', an attempt is made
        /// to parse the remainder of content into a Formula f using the Formula
        /// constructor with s => s.ToUpper() as the normalizer and a validator that
        /// checks that s is a valid cell name as defined in the AbstractSpreadsheet
        /// class comment.  There are then three possibilities:
        ///
        ///   (1) If the remainder of content cannot be parsed into a Formula, a
        ///       Formulas.FormulaFormatException is thrown.
        ///
        ///   (2) Otherwise, if changing the contents of the named cell to be f
        ///       would cause a circular dependency, a CircularException is thrown.
        ///
        ///   (3) Otherwise, the contents of the named cell becomes f.
        ///
        /// Otherwise, the contents of the named cell becomes content.
        ///
        /// If an exception is not thrown, the method returns a set consisting of
        /// name plus the names of all other cells whose value depends, directly
        /// or indirectly, on the named cell.
        ///
        /// For example, if name is A1, B1 contains A1*2, and C1 contains B1+A1, the
        /// set {A1, B1, C1} is returned.
        /// </summary>
        public override ISet<string> SetContentsOfCell(string name, string content)
        {
            // the content we want to set in a cell can't be null
            if (ReferenceEquals(content, null) || content == "")
            {
                throw new ArgumentNullException();
            }

            // the name of the cell we want to set can't be null, and must be a valid name
            if (ReferenceEquals(name, null) || !(IsValidName(name)))
            {
                throw new InvalidNameException();
            }

            // holds the list of dependees to be returned from the correct SetCellContents method
            HashSet<String> all_dependents;

            double result;  // will hold content if it is a double, otherwise remains unused

            // if content is empty, just add it to the cell
            if (content.Equals(""))
            {
                all_dependents = new HashSet<String>(SetCellContents(name, content));
            }
            // if we can parse content as a double, set the named cell contents to result
            else if (Double.TryParse(content, out result))
            {
                all_dependents = new HashSet<String>(SetCellContents(name, result));
            }
            // otherwise, if content begins with '=' then try to parse it as a formula
            else if (content.Substring(0, 1).Equals("="))
            {
                // try to create a formula from the remaining content (minus first '=' character)
                string formula_as_string = content.Substring(1, content.Length - 1);

                // 1. If content can't be parsed as a Formula this will throw a FormulaFormatException
                Formula f = new Formula(formula_as_string);

                // 2. If this creates a circular dependency, will throw a CircularException

                // 3. Otherwise, the contents of 'name' become f
                all_dependents = new HashSet<String>(SetCellContents(name, f));
            }
            else // otherwise, the content is a string so just set the cell to that string
            {
                all_dependents = new HashSet<String>(SetCellContents(name, content));
            }


            // after changing cell content, set changed to true
            Changed = true;

            return all_dependents;   // return list of all the dependees of the cell       
        }

        private double Lookup(string variable)
        {
            Cell cell;
            Double result;
            if(cells.TryGetValue(variable, out cell))
            {
                Double.TryParse(cell.value.ToString(), out result);
                return result;
;           }

            else
            {
                throw VariableNotFoundException();
            }
        }

        /// <summary>
        /// thrown if a variable is 
        /// </summary>
        /// <returns></returns>
        public static Exception VariableNotFoundException()
        {
            return VariableNotFoundException();
        }

        /// <summary>
        /// This class creates a cell object 
        /// </summary>
        private class Cell
        {
            // only one of these will be initialized
            public Object contents { get; set; }
            public Object value { get; set; }

            /// <summary>
            /// Constructor for strings
            /// </summary>
            public Cell(string name)
            {
                contents = name;
                value = name;
            }

            /// <summary>
            /// Constructor for doubles
            /// </summary>
            public Cell(double name)
            {
                contents = name;
                value = name;
            }

            /// <summary>
            /// Constructor for Formulas
            /// </summary>
            public Cell(Formula name)
            {
                contents = name;
                
            }

            /// <summary>
            /// Checks if the content n the cell is an empty string
            /// </summary>
            public bool CheckCellContents()
            {
                if (contents.Equals(""))
                {
                    return false;
                }

                else
                {
                    return true;
                }
            }

        } // Cell class
    }
}
