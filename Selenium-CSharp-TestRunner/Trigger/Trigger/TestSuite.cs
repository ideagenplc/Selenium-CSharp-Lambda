using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace Trigger
{
    public class TestSuite
    {
        public static List<TestRequest> GetTests(string path)
        {
            List<TestRequest> tests = new List<TestRequest>();
            var assembly = Assembly.LoadFrom(path);

            var types = (from t in assembly.GetTypes()
                where t.Namespace.Contains("Tests")
                select t).ToList();
            
            foreach (var type in types)
            {
                foreach (var method in WithAttribute<TestAttribute>(type))
                {
                    TestRequest request = new TestRequest();
                    request.SetTestName(method.Name);
                    request.SetTestClass(method.DeclaringType.FullName);
                    request.SetTestSuite(type.Namespace.Remove(type.Namespace.IndexOf("."))); 
                    tests.Add(request);
                }
            }
            return tests;
        }

        //Gets all methods with an NUnit [Test] attribute
        private static IEnumerable<MethodInfo> WithAttribute<TAttribute>(Type type)
        {
            return type.GetMethods().Where(method => method.GetCustomAttributes(typeof(TAttribute), true).Any());
        }
    }
}
