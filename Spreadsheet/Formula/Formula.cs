﻿// Skeleton written by Joe Zachary for CS 3500, January 2017

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Formulas
{
    /// <summary>
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules.  Provides a means to evaluate Formulas.  Formulas can be composed of
    /// non-negative floating-point numbers, variables, left and right parentheses, and
    /// the four binary operator symbols +, -, *, and /.  (The unary operators + and -
    /// are not allowed.)
    /// </summary>
    public struct Formula
    {
        //instance variables
        private string[] stringFormula;


        // variable to keep track of normalized variables
        private HashSet<string> normalized_vars;

        /// <summary>
        /// Creates a Formula from a string that consists of an infix expression written as
        /// described in the class comment.  If the expression is syntactically invalid,
        /// throws a FormulaFormatException with an explanatory Message.
        /// 
        /// An Argument Null Exception will be thrown if string parameter is null
        /// 
        /// The associated normalizer is the identity function, and the associated validator
        /// maps every string to true.  
        /// </summary>       
        public Formula(String formula) : this(formula, s => s, s => true)
        {
            if (formula == null)
            {
                throw new ArgumentNullException("Parameter is Null");
            }
        }

        /// <summary>
        /// Creates a Formula from a string that consists of a standard infix expression composed
        /// from non-negative floating-point numbers (using C#-like syntax for double/int literals), 
        /// variable symbols (a letter followed by zero or more letters and/or digits), left and right
        /// parentheses, and the four binary operator symbols +, -, *, and /.  White space is
        /// permitted between tokens, but is not required.
        /// 
        /// Examples of a valid parameter to this constructor are:
        ///     "2.5e9 + x5 / 17"
        ///     "(5 * 2) + 8"
        ///     "x*y-2+35/9"
        ///     
        /// Examples of invalid parameters are:
        ///     "_"
        ///     "-5.3"
        ///     "2 5 + 3"
        /// 
        /// If the formula is syntacticaly invalid, throws a FormulaFormatException with an 
        /// explanatory Message.
        /// 
        /// An ArgumentNullException is thrown if string is null.
        /// </summary>
        public Formula(String formula, Normalizer n, Validator v)
        {
            if (formula == null)
            {
                throw new ArgumentNullException("Parameter is Null");
            }

            //get tokens
            IEnumerable<string> tokens = Formula.GetTokens(formula);

            //valid token patterns
            String opPattern = @"^[\+\-*/]$";
            Regex operation = new Regex(opPattern);
            String varPattern = @"^[a-zA-Z][0-9a-zA-Z]*$";
            Regex variable = new Regex(varPattern);
            String spacePattern = @"^\s+$";
            Regex space = new Regex(spacePattern);

            //counters
            int opening = 0;
            int closing = 0;
            int elementCounter = 0;
            String previous = null;

            //keep track of normalized variables

            normalized_vars = new HashSet<string>();

            //check if the there are any tokens to check
            if (tokens.Count<string>() == 0)
            {
                throw new FormulaFormatException("No tokens");
            }

            //check if the formua meets the syntactic standards
            foreach (string element in tokens)
            {
                elementCounter++;

                //check if there is at least one token
                if (element == null)
                {
                    throw new FormulaFormatException("No tokens");
                }


                double number;
                //check for all valid tokens
                if (!(element.Equals("(") || element.Equals(")") || operation.IsMatch(element) || variable.IsMatch(element) || Double.TryParse(element, out number) || space.IsMatch(element)))
                {
                    throw new FormulaFormatException("Invalid Tokens");
                }

                //check if the number of closing parathensis is ever greater than the number of openeing parathensis
                if (element.Equals("("))
                {
                    opening++;
                }

                if (element.Equals(")"))
                {
                    closing++;
                }

                if (closing > opening)
                {
                    throw new FormulaFormatException("More closing than opening");
                }

                if (elementCounter == tokens.Count<string>())
                {
                    //check if The total number of opening parentheses must equal the total number of closing parentheses.
                    if (opening != closing)
                    {
                        throw new FormulaFormatException("closing and opening do not equal");

                    }
                }

                //check if The first token of a formula must be a number, a variable, or an opening parenthesis.

                if (!(tokens.First().Equals("(") || variable.IsMatch(tokens.First()) || Double.TryParse(tokens.First(), out number)))
                {
                    throw new FormulaFormatException("First token was not a number, variable, or opening parenthesis");
                }

                //check if Any token that immediately follows an opening parenthesis or an operator must be either a number,
                //a variable, or an opening parenthesis.
                if (previous != null)
                {
                    if (previous.Equals("(") || operation.IsMatch(previous))
                    {
                        if (!(variable.IsMatch(element) || Double.TryParse(element, out number) || element.Equals("(")))
                        {
                            throw new FormulaFormatException("token immediatly following an open parethesis or operater was not a number, variable, or open parenthesis");
                        }


                    }

                    if (element.Equals("(") || operation.IsMatch(element))
                    {
                        if (element == tokens.Last<string>())
                        {
                            throw new FormulaFormatException("The last element can not be an open parathesis or an operator");
                        }
                    }

                    //check if Any token that immediately follows a number, a variable, or a closing parenthesis must be either 
                    //an operator or a closing parenthesis.

                    if (Double.TryParse(previous, out number) || variable.IsMatch(previous) || previous.Equals(")"))
                    {
                        if (!(operation.IsMatch(element) || element.Equals(")")))
                        {
                            throw new FormulaFormatException("The token immediatlely folloeing a number, varibale, or colsing parenthesis was not either an operator or closing parethesis");
                        }
                    }
                }
                //change the previous reference
                previous = element;
            }


            //if all syntactical arrors pass then put the tokens in a string array to be accessed by other methods
            int stringCounter = 0;
            stringFormula = new String[tokens.Count()];
            foreach (string element in tokens)
            {
                //check if the element is a variable
                if (variable.IsMatch(element))
                {
                    if (!(variable.IsMatch(n.Invoke(element))))
                    {
                        throw new FormulaFormatException("normalized variable could not be recognized as a variable");
                    }

                    if (!(v.Invoke(n.Invoke(element))))
                    {
                        throw new FormulaFormatException("the normalized varible was not valid");
                    }

                    //add the normalized variable to the array

                    stringFormula[stringCounter] = n.Invoke(element);

                    //keep track of the normalized variables
                    normalized_vars.Add(n.Invoke(element));
                }
                stringFormula[stringCounter] = element;
                stringCounter++;
            }






        }
        /// <summary>
        /// Evaluates this Formula, using the Lookup delegate to determine the values of variables.  (The
        /// delegate takes a variable name as a parameter and returns its value (if it has one) or throws
        /// an UndefinedVariableException (otherwise).  Uses the standard precedence rules when doing the evaluation.
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, its value is returned.  Otherwise, throws a FormulaEvaluationException  
        /// with an explanatory Message.
        /// 
        /// if the lookup object is null an ArgumentNUllException will be thrown
        /// </summary>
        public double Evaluate(Lookup lookup)
        {
            if (lookup == null)
            {
                throw new ArgumentNullException("Parameter is Null");
            }

            //two empty stacks
            Stack<double> values = new Stack<double>();
            Stack<String> operators = new Stack<String>();



            for (int i = 0; i < stringFormula.Length; i++)
            {
                String t = stringFormula[i];
                double number;

                //check if t is a double
                if (Double.TryParse(t, out number)) // if it is parsable, will return true
                {
                    double token = number; // number will hold the token value as a double

                    String top;
                    if (operators.Count > 0 && values.Count > 0)
                    {
                        top = operators.Peek();

                        if (top.Equals("*") || top.Equals("/"))
                        {
                            double first_val = values.Pop();
                            String operation = operators.Pop();
                            double result;
                            if (operation.Equals("*"))
                                result = first_val * token;
                            else
                            {
                                if (token == 0)
                                    throw new FormulaEvaluationException("You cannot divide by 0");
                                result = first_val / token;
                            }
                            values.Push(result);
                        }

                        else
                        {
                            values.Push(token);
                        }
                    }
                    else
                    {
                        values.Push(token);
                    }
                }

                //check if t is a + or -
                else if (t.Equals("+") || t.Equals("-"))
                {
                    if (operators.Count > 0 && values.Count > 0 && (operators.Peek().Equals("-") || operators.Peek().Equals("+")))
                    {



                        double value1 = values.Pop();
                        double value2 = values.Pop();

                        String topOp = operators.Pop();

                        double result;
                        if (topOp.Equals("+"))
                        {
                            result = value1 + value2;
                            values.Push(result);
                            operators.Push(t);
                        }



                        else
                        {
                            result = value2 - value1;
                            values.Push(result);
                            operators.Push(t);
                        }



                    }

                    else
                    {
                        operators.Push(t);
                    }
                }

                //if t is * or /
                else if (t.Equals("/") || t.Equals("*"))
                {
                    operators.Push(t);
                }

                //if t is (
                else if (t.Equals("("))
                {
                    operators.Push(t);
                }

                //if t is )
                else if (t.Equals(")"))
                {
                    if (operators.Count > 0 && values.Count > 0)
                    {
                        if (operators.Peek().Equals("+") || operators.Peek().Equals("-"))
                        {
                            double value1 = values.Pop();
                            double value2 = values.Pop();
                            string topOp = operators.Pop();

                            double result;
                            if (topOp.Equals("+"))
                            {
                                result = value1 + value2;
                                values.Push(result);
                            }



                            else
                            {
                                result = value2 - value1;
                                values.Push(result);
                            }

                        }
                    }

                    String para = operators.Pop();
                    if (operators.Count() != 0)
                    {
                        if (operators.Peek().Equals("*") || operators.Peek().Equals("/"))
                        {
                            double value1 = values.Pop();
                            double value2 = values.Pop();
                            string topOp = operators.Pop();

                            double result;


                            if (topOp.Equals("*"))
                            {
                                result = value1 * value2;
                                values.Push(result);
                            }

                            else
                            {
                                if (value1 == 0)
                                {
                                    throw new FormulaEvaluationException("You cannot divide by 0");
                                }
                                result = value2 / value1;
                                values.Push(result);
                            }



                        }
                    }
                }

                //if t is a variable
                else
                {
                    double token;
                    try
                    {
                        token = lookup(t); // number will hold the token value as a double

                    }

                    catch (UndefinedVariableException)
                    {
                        throw new FormulaEvaluationException("Could not define Variable");
                    }


                    String top;
                    if (operators.Count > 0 && values.Count > 0)
                    {
                        top = operators.Peek();

                        if (top.Equals("*") || top.Equals("/"))
                        {
                            double first_val = values.Pop();
                            String operation = operators.Pop();
                            double result;
                            if (operation.Equals("*"))
                                result = first_val * token;
                            else
                            {
                                if (token == 0)
                                    throw new FormatException();
                                result = first_val / token;
                            }
                            values.Push(result);
                        }
                        else
                        {
                            values.Push(token);
                        }
                    }
                    else
                        values.Push(token);
                }
            }

            //return a value
            if (operators.Count() == 0)
            {
                return values.Pop();
            }
            else
            {
                double value1 = values.Pop();
                double value2 = values.Pop();

                if (operators.Pop() == "+")
                {
                    return value1 + value2;
                }

                else
                {
                    return value2 - value1;
                }
            }

        }

        /// <summary>
        /// Returns an ISet<string> that contains each distinct variable (in normalized form) that appears in the Formula.
        /// </summary>
        /// <returns></returns>
        public ISet<string> GetVariables()
        {
            HashSet<string> normalSet = new HashSet<string>(normalized_vars);
            return normalSet;

        }

        /// <summary>
        /// Returns a string containing no spaces which, if passed to the Formula
        /// constructor, will produce a Formula f such that this.Equals(f).  All of the
        /// variables in the string should be normalized.
        /// 
        /// For example, if N is a method that converts all the letters in a string to upper case:
        /// 
        /// new Formula("x + y", N, s => true).ToString() should return "X+Y"
        /// new Formula("x + Y").ToString() should return "x+Y"
        /// </summary>
        public override string ToString()
        {
            string formula = "";
            for (int i = 0; i < stringFormula.Count(); i++)
            {
                formula += stringFormula[i];
            }
            return formula;
        }


        /// <summary>
        /// Given a formula, enumerates the tokens that compose it.  Tokens are left paren,
        /// right paren, one of the four operator symbols, a string consisting of a letter followed by
        /// zero or more digits and/or letters, a double literal, and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(String formula)
        {
            // Patterns for individual tokens.
            // NOTE:  These patterns are designed to be used to create a pattern to split a string into tokens.
            // For example, the opPattern will match any string that contains an operator symbol, such as
            // "abc+def".  If you want to use one of these patterns to match an entire string (e.g., make it so
            // the opPattern will match "+" but not "abc+def", you need to add ^ to the beginning of the pattern
            // and $ to the end (e.g., opPattern would need to be @"^[\+\-*/]$".)
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z][0-9a-zA-Z]*";

            // PLEASE NOTE:  I have added white space to this regex to make it more readable.
            // When the regex is used, it is necessary to include a parameter that says
            // embedded white space should be ignored.  See below for an example of this.
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: e[\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall pattern.  It contains embedded white space that must be ignored when
            // it is used.  See below for an example of this.  This pattern is useful for 
            // splitting a string into tokens.
            String splittingPattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            // PLEASE NOTE:  Notice the second parameter to Split, which says to ignore embedded white space
            /// in the pattern.
            foreach (String s in Regex.Split(formula, splittingPattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }
        }
    }

    /// <summary>
    /// A Lookup method is one that maps some strings to double values.  Given a string,
    /// such a function can either return a double (meaning that the string maps to the
    /// double) or throw an UndefinedVariableException (meaning that the string is unmapped 
    /// to a value. Exactly how a Lookup method decides which strings map to doubles and which
    /// don't is up to the implementation of the method.
    /// </summary>
    public delegate double Lookup(string var);
    public delegate string Normalizer(string s);
    public delegate bool Validator(string s);

    /// <summary>
    /// Used to report that a Lookup delegate is unable to determine the value
    /// of a variable.
    /// </summary>
    [Serializable]
    public class UndefinedVariableException : Exception
    {
        /// <summary>
        /// Constructs an UndefinedVariableException containing whose message is the
        /// undefined variable.
        /// </summary>
        /// <param name="variable"></param>
        public UndefinedVariableException(String variable)
            : base(variable)
        {
        }
    }

    /// <summary>
    /// Used to report syntactic errors in the parameter to the Formula constructor.
    /// </summary>
    [Serializable]
    public class FormulaFormatException : Exception
    {
        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(String message) : base(message)
        {
        }
    }

    /// <summary>
    /// Used to report errors that occur when evaluating a Formula.
    /// </summary>
    [Serializable]
    public class FormulaEvaluationException : Exception
    {
        /// <summary>
        /// Constructs a FormulaEvaluationException containing the explanatory message.
        /// </summary>
        public FormulaEvaluationException(String message) : base(message)
        {
        }
    }
}

