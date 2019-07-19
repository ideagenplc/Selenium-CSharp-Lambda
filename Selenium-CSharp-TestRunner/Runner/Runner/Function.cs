using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using Amazon.Lambda.Core;
using Amazon.S3;
using Amazon.S3.Transfer;
using Newtonsoft.Json;


// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace Runner
{
    public class Function
    {
        string _result;
        /// <summary>
        /// A function that runs selenium tests using headless chrome
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public string FunctionHandler(string request, ILambdaContext context)
        {
            TestRequest testRequest = JsonConvert.DeserializeObject<TestRequest>(request);

            LambdaCleanUp();
            TestSetup(testRequest.GetTestSuite());

            try
            {
                Console.WriteLine(testRequest.GetTestName() + " started at: " + DateTime.Now.ToString("HH:mm:ss"));
                TestRunner.Runner(testRequest);
                Console.WriteLine(testRequest.GetTestName() + "  finished at: " + DateTime.Now.ToString("HH:mm:ss"));
                _result = "Passed";
            }
            catch (Exception ex)
            {
                Console.WriteLine(testRequest.GetTestName() + " failed at: " + DateTime.Now.ToString("HH:mm:ss") + " - Error: " + ex.Message + " - " + ex.InnerException + " - " + ex.StackTrace);
                _result = "Failed";
            }
            //Clean up /tmp
            LambdaCleanUp();

            return testRequest.GetTestName() + ": " + _result;
        }

        private void TestSetup(string testSuite)
        {
            //Copy required files from s3
            Console.WriteLine("Copying Selenium Setup to lambda 'tmp'");
            start:
            try
            {
                TransferUtility fileTransferUtility =
                    new TransferUtility(
                        new AmazonS3Client("[Access key ID]", "[Secret access key]", "[RegionEndpoint]"));

                // Note the 'fileName' is the 'key' of the object in S3 (which is usually just the file name)  
                fileTransferUtility.Download("/tmp/" + testSuite + ".zip", "[Bucket]", testSuite + ".zip"); ;
                fileTransferUtility.Download("/tmp/HeadlessSeleniumSetup.zip", "[Bucket]", "HeadlessSeleniumSetup.zip");
            }
            catch
            {
                goto start;

            }
            Console.WriteLine("Completed copy!");

            //Unzip HeadlessSeleniumSetup
            Console.WriteLine("Unzip HeadlessSeleniumSetup to /tmp");
            using (ZipArchive archive = ZipFile.OpenRead("/tmp/HeadlessSeleniumSetup.zip"))
            {
                archive.ExtractToDirectory("/tmp/");
            }
            Console.WriteLine("HeadlessSeleniumSetup unzipped!");

            //Remove testsuite.zip
            Console.WriteLine("Removing HeadlessSeleniumSetup.zip to free up space");
            File.Delete("/tmp/HeadlessSeleniumSetup.zip");

            //Unzip test suite
            Console.WriteLine("Unzipping TestSuite");
            using (ZipArchive archive = ZipFile.OpenRead("/tmp/" + testSuite + ".zip"))
            {
                archive.ExtractToDirectory("/tmp/TestSuite/");
            }
            Console.WriteLine("TestSuite unzipped!");

            //Remove testsuite.zip
            Console.WriteLine("Removing " + testSuite + ".zip to free up space");
            File.Delete("/tmp/" + testSuite + ".zip");


            //Add correct permissions to chrome & chromedriver
            Console.WriteLine("Adding a+x permissions to chrome & chromedriver");
            Process.Start("chmod", "a+x /tmp/chromedriver");
            Process.Start("chmod", "a+x /tmp/chrome");
        }
        
        private void LambdaCleanUp()
        {
            KillProcesses();
            clear_tmp();
        }

        private void KillProcesses()
        {
            Process[] processlist = Process.GetProcesses();

            foreach (Process theprocess in processlist)
            {
                string cmd = "ps -p " + theprocess.Id;
                if (!cmd.Bash().Contains("defunct"))
                {
                    Console.WriteLine("Process: {0} ID: {1}", theprocess.ProcessName, theprocess.Id);
                }

            }
            foreach (Process theprocess in processlist)
            {
                string cmd = "ps -p " + theprocess.Id;
                if (!cmd.Bash().Contains("defunct"))
                {
                    if (theprocess.ProcessName.Contains("chrome"))
                    {
                        Console.WriteLine("Killing process PID: {0}", theprocess.Id);
                        try
                        {
                            theprocess.Kill();
                        }
                        catch { }
                    }
                }
            }

        }

        private void clear_tmp()
        {
            Console.WriteLine("Cleaning /tmp directory:");
            foreach (var subDir in new DirectoryInfo("/tmp/").GetDirectories())
            {
                subDir.Delete(true);
                Console.WriteLine(subDir + " deleted!");
            }

            foreach (string fileName in Directory.GetFiles("/tmp/"))
            {
                File.Delete(fileName);
                Console.WriteLine(fileName + " deleted!");
            }
        }
    }
}
