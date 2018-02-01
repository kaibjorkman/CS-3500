using System;
using System.Collections.Generic;
using System.Linq;
using Dependencies;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DependencyGraphTestCases
{
    [TestClass]
    public class UnitTests
    {
        /// <summary>
        /// this test builds a dendency gragh and then pulls all the corisponding dependees and dependents
        /// to see if they are still where they are supposed to be
        /// </summary>
        [TestMethod]
        public void TestHas()
        {
            DependencyGraph test = new DependencyGraph();

            test.AddDependency("kai", "cool");
            Assert.IsTrue(test.HasDependees("kai"));
            Assert.IsTrue(test.HasDependents("cool"));

        }
        /// <summary>
        /// this test makes a dependency nd then sees if you can
        /// pull out the correct dependee and dependent
        /// 
        /// </summary>
        /// 
        [TestMethod]
        public void TestGet()
        {
            DependencyGraph test = new DependencyGraph();

            test.AddDependency("kai", "cool");
            IEnumerable<string> dependees = test.GetDependees("kai");
            IEnumerable<string> dependents = test.GetDependents("cool");

            string element = null;
            foreach (string dependee in dependees)
            {
                element = dependee;
            }

            Assert.AreEqual("cool", element);

            string thing = null; ;
            foreach (string dependent in dependents)
            {
                thing = dependent;
            }

            Assert.AreEqual("kai", thing);

        }

        /// <summary>
        /// this test adds a few dependencies and then takes a few away and then sees
        /// if they are really removed.
        /// </summary>
        [TestMethod]
        public void TestAddRemove()
        {
            DependencyGraph test = new DependencyGraph();

            test.AddDependency("kai", "cool");
            test.AddDependency("shane", "stupid");
            test.AddDependency("hey", "hi");

            test.RemoveDependency("hey", "hi");
            test.RemoveDependency("shane", "stupid");

            Assert.IsFalse(test.HasDependees("shane"));
            Assert.IsFalse(test.HasDependents("hi"));
            Assert.IsTrue(test.HasDependents("cool"));
        }
        /// <summary>
        /// This test adds a few dependencies and then replaces one with a list of new
        /// ones and then checks that the size of the graph is still consistent.
        /// </summary>

        [TestMethod]
        public void TestReplace()
        {
            DependencyGraph test = new DependencyGraph();

            test.AddDependency("kai", "cool");
            test.AddDependency("shane", "stupid");
            test.AddDependency("hey", "hi");

            HashSet<string> dependees = new HashSet<string>();
            dependees.Add("who");
            dependees.Add("what");
            dependees.Add("when");
            dependees.Add("where");
            dependees.Add("why");
            dependees.Add("How");

            HashSet<string> dependents = new HashSet<string>();
            dependents.Add("hey");
            dependents.Add("hi");
            dependents.Add("hello");
            dependents.Add("ha");
            dependents.Add("ho");
            dependents.Add("hee");

            test.ReplaceDependees("kai", dependees);
            test.ReplaceDependents("stupid", dependents);

            IEnumerable<string> newDependees = test.GetDependees("kai");

            Assert.AreEqual(13, test.Size);
        }
        /// <summary>
        /// This test adds a bunch of strings and then removes a bunch of strings
        /// han it checks if it has the correct size.
        /// </summary>
        [TestMethod]
        public void TestAddMany()
        {
            DependencyGraph test = new DependencyGraph();

            for(int i = 0; i < 1_000; i++)
            {
                string first = RandomString(10);
                string second = RandomString(10);

                test.AddDependency(first, second);
            }

            Assert.AreEqual(1000, test.Size);
        }

        [TestMethod]
        public void TestHasNone()
        {
            DependencyGraph test = new DependencyGraph();

            test.RemoveDependency("kai", "stupid");

            

            IEnumerable<string> none = test.GetDependees("kai");
            IEnumerable<string> nothing = test.GetDependents("kai");

            int counter = 0;
            foreach(string element in none)
            {
                counter++;
            }

            foreach (string element in nothing)
            {
                counter++;
            }

            Assert.AreEqual(0, counter);
            Assert.IsFalse(test.HasDependees("kai"));
            Assert.IsFalse(test.HasDependents("kai"));



        }
        /// <summary>
        /// This test replaces dependents and sees if the count is consistant
        /// </summary>
        [TestMethod]
        public void TestReplaceDependents()
        {
            DependencyGraph test = new DependencyGraph();

            test.AddDependency("kai", "cool");
           

            HashSet<string> dependees = new HashSet<string>();
            dependees.Add("who");
            dependees.Add("what");
            dependees.Add("when");
            dependees.Add("where");
            dependees.Add("why");
            dependees.Add("How");


            test.ReplaceDependees("kai", dependees);
            

        

            Assert.AreEqual(6, test.Size);
        }

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }

    

}
