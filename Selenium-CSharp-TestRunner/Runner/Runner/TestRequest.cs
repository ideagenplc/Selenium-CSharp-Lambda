namespace Runner
{
    public class TestRequest
    {
        public string TestName;
        public string TestClass;
        public string TestSuite;

        public string GetTestName()
        {
            return TestName;
        }
        public void SetTestName(string testName)
        {
            TestName = testName;
        }
        public string GetTestClass()
        {
            return TestClass;
        }
        public void SetTestClass(string testClass)
        {
            TestClass = testClass;
        }
        public string GetTestSuite()
        {
            return TestSuite;
        }
        public void SetTestSuite(string testSuite)
        {
            TestSuite = testSuite;
        }
    }
}
