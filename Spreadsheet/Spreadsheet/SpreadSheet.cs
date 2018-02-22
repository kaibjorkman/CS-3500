using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Dependencies;
using Formulas;

namespace SS
{
   
    public class Spreadsheet : AbstractSpreadsheet
    {
        // dictionary maps cell name (row, col) to the cell itself
        Dictionary<string, Cell> cells;

        // dependency graph for cells
        DependencyGraph graph;

        /// <summary>
        /// Constructor creates a new spreadsheet and initialized variables
        /// </summary>
        public Spreadsheet()
        {
            // initialize spreadsheet variables;
            cells = new Dictionary<string, Cell>();
            graph = new DependencyGraph();
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
        public override ISet<string> SetCellContents(string name, double number)
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
        public override ISet<string> SetCellContents(string name, string text)
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
        public override ISet<string> SetCellContents(string name, Formula formula)
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
            if (Regex.IsMatch(name, @"^[a-zA-Z_](?: [a-zA-Z_]|\d)*$", RegexOptions.Singleline))
                return true;
            else return false;
        }

        /// <summary>
        /// This class creates a cell object 
        /// </summary>
        private class Cell
        {
            // only one of these will be initialized
            public Object contents { get; private set; }
            public Object value { get; private set; }

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
                //value = name.Evaluate();
            }

            /// <summary>
            /// Checks if the content n the cell is an empty string
            /// </summary>
            /// <param name="name"></param>
            /// <returns></returns>
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
