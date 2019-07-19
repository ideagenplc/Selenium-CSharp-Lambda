using System;
using System.Reflection;
using System.Threading;

namespace Runner
{
    internal class TestRunner
    {
        public static void Runner(TestRequest testRequest) 
        {
            //Run test
            RunTest(testRequest);
        }

        private static Type Class(TestRequest testRequest)
        {
            Console.WriteLine("Creating type!");
            Type t = null;
            var assembly = Assembly.LoadFrom("/tmp/TestSuite/" + testRequest.GetTestSuite() + ".dll");
            int i = 0;
            Retry:
            t = assembly.GetType(testRequest.GetTestClass(), true, true);
            if (t == null && i < 3)
            {
                Console.WriteLine("Type is null - retrying");
                Thread.Sleep(1000);
                i++;
                goto Retry;
            }
            return t;
        }

        public static void RunTest(TestRequest testRequest)
        {
            Console.WriteLine("Running test");
            int i = 0;
            Retry:
            Type type = Class(testRequest);
            if (type == null && i < 3)
            {
                Console.WriteLine("type is null - retrying");
                Thread.Sleep(1000);
                goto Retry;
            }

            // Create an instance of that type
            object obj = Activator.CreateInstance(type);

            // Retrieve the method you are looking for
            MethodInfo methodInfo = type.GetMethod(testRequest.GetTestName());
            // Invoke the method on the instance we created above
            methodInfo.Invoke(obj, null);
        }
    }
}
