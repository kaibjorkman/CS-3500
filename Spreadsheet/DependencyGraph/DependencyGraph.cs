// Skeleton implementation written by Joe Zachary for CS 3500, January 2018.

using System;
using System.Collections.Generic;

namespace Dependencies
{
    /// <summary>
    /// A DependencyGraph can be modeled as a set of dependencies, where a dependency is an ordered 
    /// pair of strings.  Two dependencies (s1,t1) and (s2,t2) are considered equal if and only if 
    /// s1 equals s2 and t1 equals t2.
    /// 
    /// Given a DependencyGraph DG:
    /// 
    ///    (1) If s is a string, the set of all strings t such that the dependency (s,t) is in DG 
    ///    is called the dependents of s, which we will denote as dependents(s).
    ///        
    ///    (2) If t is a string, the set of all strings s such that the dependency (s,t) is in DG 
    ///    is called the dependees of t, which we will denote as dependees(t).
    ///    
    /// The notations dependents(s) and dependees(s) are used in the specification of the methods of this class.
    ///
    /// For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
    ///     dependents("a") = {"b", "c"}
    ///     dependents("b") = {"d"}
    ///     dependents("c") = {}
    ///     dependents("d") = {"d"}
    ///     dependees("a") = {}
    ///     dependees("b") = {"a"}
    ///     dependees("c") = {"a"}
    ///     dependees("d") = {"b", "d"}
    ///     
    /// All of the methods below require their string parameters to be non-null.  This means that 
    /// the behavior of the method is undefined when a string parameter is null.  
    ///
    /// IMPORTANT IMPLEMENTATION NOTE
    /// 
    /// The simplest way to describe a DependencyGraph and its methods is as a set of dependencies, 
    /// as discussed above.
    /// 
    /// However, physically representing a DependencyGraph as, say, a set of ordered pairs will not
    /// yield an acceptably efficient representation.  DO NOT USE SUCH A REPRESENTATION.
    /// 
    /// You'll need to be more clever than that.  Design a representation that is both easy to work
    /// with as well acceptably efficient according to the guidelines in the PS3 writeup. Some of
    /// the test cases with which you will be graded will create massive DependencyGraphs.  If you
    /// build an inefficient DependencyGraph this week, you will be regretting it for the next month.
    /// </summary>
    public class DependencyGraph
    {
        //Instance Variables

        //Map a dependent to all its dependees
        private Dictionary<string, HashSet<string>> dependentsMap;

        //Map a dependee to all of its dependents
        private Dictionary<string, HashSet<string>> dependeesMap;

        //size counter;
        int graphSize;


        /// <summary>
        /// Creates a DependencyGraph containing no dependencies.
        /// 
        /// 
        /// </summary>
        public DependencyGraph()
        {
            dependentsMap = new Dictionary<string, HashSet<string>>();
            dependeesMap = new Dictionary<string, HashSet<string>>();

            graphSize = 0;

        }

        /// <summary>
        /// Takes a dependency graph as a parameter and copies all the dependencies into its own maps.
        /// </summary>
        /// <param name="graph"></param>
        public DependencyGraph(DependencyGraph graph)
        {
            if(graph == null)
            {
                throw new ArgumentNullException("Parameter is Null");
            }
            //make a copy of the dictionaries
            dependentsMap = new Dictionary<string, HashSet<string>>(graph.dependentsMap);
            dependeesMap = new Dictionary<string, HashSet<string>>(graph.dependeesMap);

            int size = graph.Size;
            

        }

        /// <summary>
        /// The number of dependencies in the DependencyGraph.
        /// </summary>
        public int Size
        {
            get { return graphSize; }
        }

        /// <summary>
        /// Reports whether dependents(s) is non-empty.  Requires s != null.
        /// 
        /// ArgumentNull exception will be thrown if string s is null
        /// </summary>
        public bool HasDependees(string s)
        {
            if(s == null)
            {
                throw new ArgumentNullException("Parameter is Null");
            }

            //if the dependents has the key "s"
            if (dependeesMap.ContainsKey(s))
            {
                if (dependeesMap[s].Count != 0)
                {
                    return true;
                }

                return false;

            }
            else
                return false;
        }

        /// <summary>
        /// Reports whether dependees(s) is non-empty.  Requires s != null.
        /// 
        /// ArgumentNullException will be thrown if parameter is null
        /// </summary>
        public bool HasDependents(string s)
        {
            if(s == null)
            {
                throw new ArgumentNullException("Parameter is Null");
            }

            //if the dependees has the key "s"
            if (dependentsMap.ContainsKey(s))
            {
                if (dependentsMap[s].Count != 0)
                {
                    return true;
                }

                return false;

            }
            else
                return false;
        }

        /// <summary>
        /// Enumerates dependents(s).  Requires s != null.
        /// 
        /// ArgumentNullException will be thrown if string s is null
        /// </summary>
        public IEnumerable<string> GetDependents(string s)
        {
            if(s == null)
            {
                throw new ArgumentNullException("Parameter is Null");
            }

            //if the dependees has the key "s" return the set of dependents
            if (dependentsMap.ContainsKey(s))
            {
                return new HashSet<string>(dependentsMap[s]);
            }

            else
                //if not return an empty string
                return new HashSet<string>();
        }

        /// <summary>
        /// Enumerates dependees(s).  Requires s != null.
        /// 
        /// ArgumentNullException will be thrown if string s is null
        /// </summary>
        public IEnumerable<string> GetDependees(string s)
        {
            if (s == null)
            {
                throw new ArgumentNullException("Parameter is Null");
            }

            //if the dependents has the key "s" return the set of dependees
            if (dependeesMap.ContainsKey(s))
            {
                return new HashSet<string>(dependeesMap[s]);
            }

            else
                //if not retrun an empty string
                return new HashSet<string>();
        }

        /// <summary>
        /// Adds the dependency (s,t) to this DependencyGraph.
        /// This has no effect if (s,t) already belongs to this DependencyGraph.
        /// Requires s != null and t != null.
        /// 
        /// ArgumentNullExcption will be thrown if string s or string t is null
        /// </summary>
        public void AddDependency(string s, string t)
        {
            if(s == null || t == null)
            {
                throw new ArgumentNullException("Parameter is Null");
            }

            //check if the dependency pair exists
            if(!(dependentsMap.ContainsKey(s) && dependeesMap.ContainsKey(t)))
            {
                graphSize++;
            }

            //check if "s" is in the dependents already

            if(dependentsMap.ContainsKey(s))
            {
                //add t to its dependees set
                dependentsMap[s].Add(t);
            }

            else //else add s to dependents and t as a dependee of s
            {
                //create a new HashSet of dependees for s
                HashSet<string> dependees = new HashSet<string>();

                dependees.Add(t);
                dependentsMap.Add(s, dependees);
            }

            //check if "t" is in the dependees already

            if (dependeesMap.ContainsKey(t))
            {
                //add s to its dependents set
                dependeesMap[t].Add(s);
            }

            else //else add s to dependents and t as a dependee of s
            {
                //create a new HashSet of dependents for t
                HashSet<string> dependents = new HashSet<string>();

                dependents.Add(s);
                dependeesMap.Add(t, dependents);
            }




        }

        /// <summary>
        /// Removes the dependency (s,t) from this DependencyGraph.
        /// Does nothing if (s,t) doesn't belong to this DependencyGraph.
        /// Requires s != null and t != null.
        /// 
        /// ArgumentNullExcption will be thrown if string s or string t is null
        /// </summary>
        public void RemoveDependency(string s, string t)
        {
            if (s == null || t == null)
            {
                throw new ArgumentNullException("Parameter is Null");
            }

            //check if this pair exists yet
            if (dependentsMap.ContainsKey(s) && dependeesMap.ContainsKey(t))
            {
                graphSize--;
            }

            //check if s is contained in dependents
            if(dependentsMap.ContainsKey(s))
            {
                dependentsMap[s].Remove(t);

                if(dependentsMap[s].Count == 0)
                {
                    dependentsMap.Remove(s);
                }

            }

            //check if t is contained in dependees
            if (dependeesMap.ContainsKey(t))
            {
                dependeesMap[t].Remove(s);

               if(dependeesMap[t].Count == 0)
                {
                    dependeesMap.Remove(t);
                }
            }
        }

        /// <summary>
        /// Removes all existing dependencies of the form (s,r).  Then, for each
        /// t in newDependents, adds the dependency (s,t).
        /// Requires s != null and t != null.
        /// 
        /// ArgumentNullException will be thrown if s is null of a string in the IEnumerable is null.
        /// </summary>
        public void ReplaceDependents(string s, IEnumerable<string> newDependents)
        {
            if(s == null)
            {
                throw new ArgumentNullException("Parameter is Null");
            }

            foreach(string dependent in newDependents)
            {
                if(dependent == null)
                {
                    throw new ArgumentNullException("Parameter is Null");
                }
            }
            // new IEnumerable object to hold the old dependents of 's'  
            IEnumerable<string> oldDependents = GetDependents(s);

            // remove each dependent 'r' associated with 's' in the set of oldDependents
            foreach (string r in oldDependents)
                RemoveDependency(s, r); // remove the ordered pair           

            // add each dependent 't' associated with 's' in the set of NewDependents
            foreach (string t in newDependents)
                AddDependency(s, t);
        }

        /// <summary>
        /// Removes all existing dependencies of the form (r,t).  Then, for each 
        /// s in newDependees, adds the dependency (s,t).
        /// Requires s != null and t != null.
        /// 
        ///  ArgumentNullException will be thrown if s is null of a string in the IEnumerable is null.
        /// </summary>
        public void ReplaceDependees(string t, IEnumerable<string> newDependees)
        {
            if (t == null)
            {
                throw new ArgumentNullException("Parameter is Null");
            }

            foreach (string dependee in newDependees)
            {
                if (dependee == null)
                {
                    throw new ArgumentNullException("Parameter is Null");
                }
            }
            // new IEnumerable object to hold the old dependees of 's'  
            IEnumerable<string> oldDependees = GetDependees(t);

            // remove each dependee 'r' associated with 's' in the set of oldDependees
            foreach (string r in oldDependees)
                RemoveDependency(r, t); // remove the ordered pair           

            // add each dependee 't' associated with 's' in the set of NewDependees
            foreach (string s in newDependees)
                AddDependency(s, t);
        }
    }
}
