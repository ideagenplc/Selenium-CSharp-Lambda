using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using Amazon.Lambda.Core;
using Amazon.S3;
using Amazon.S3.Transfer;
using NUnit.Framework;
using Selenium_CSharp_Lambda.Selenium;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace Selenium_CSharp_Lambda
{
    public class Function
    {
        
        /// <summary>
        /// A function that runs a selenium test using headless chrome
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string FunctionHandler(ILambdaContext context)
        {
            string result;

            LambdaCleanUp();
            TestSetup();

            try
            {
                Console.WriteLine("Started at: " + DateTime.Now.ToString("HH:mm:ss"));

                GoogleTest();

                Console.WriteLine("Finished at: " + DateTime.Now.ToString("HH:mm:ss"));
                result = "Passed";
            }
            catch (Exception ex)
            {

                Console.WriteLine("Failed at: " + DateTime.Now.ToString("HH:mm:ss") + " - Error: " + ex.Message + " - " + ex.InnerException + " - " + ex.StackTrace);
                result = "Failed";
            }

            //Clean up /tmp
            LambdaCleanUp();

            return result;
        }

        //Test Setup
        private void TestSetup()
        {
            //Copy required files from s3
            Console.WriteLine("Copying Headless Selenium Setup to lambda 'tmp'");
            start:
            try
            {
                TransferUtility fileTransferUtility =
                    new TransferUtility(
                        new AmazonS3Client("[Access key ID]", "[Secret access key]", "[RegionEndpoint]"));

                // Note the 'fileName' is the 'key' of the object in S3 (which is usually just the file name)  
                fileTransferUtility.Download("/tmp/HeadlessSeleniumSetup.zip", "[Bucket]", "HeadlessSeleniumSetup.zip");
            }
            catch
            {
                goto start;

            }
            Console.WriteLine("Completed copy!");

            //unzip Headless Selenium setup to tmp
            Console.WriteLine("Unzip HeadlessSeleniumSetup to /tmp");
            using (ZipArchive archive = ZipFile.OpenRead("/tmp/HeadlessSeleniumSetup.zip"))
            {
                archive.ExtractToDirectory("/tmp/");
            }
            Console.WriteLine("HeadlessSeleniumSetup unzipped!");

            //Remove HeadlessSeleniumSetup.zip
            Console.WriteLine("Removing HeadlessSeleniumSetup.zip to free up space");
            File.Delete("/tmp/HeadlessSeleniumSetup.zip");

            //Add correct permissions to chrome & chromedriver
            Console.WriteLine("Adding a+x permissions to chrome & chromedriver");
            Process.Start("chmod", "a+x /tmp/chromedriver");
            Process.Start("chmod", "a+x /tmp/chrome");
        }
        
        //Clean up
        private void LambdaCleanUp()
        {
            KillProcesses();
            clear_tmp();
        }

        //Kill any processes which have been started during test execution
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
                    if (theprocess.ProcessName.Contains("chrome") || theprocess.ProcessName.Contains("Xvfb"))
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

        //Clean up tmp directory
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

        //Selenium test
        [Test]
        public void GoogleTest()
        {
            //Initialise the driver
            BrowserFactory.InitBrowser();

            Console.WriteLine("Check web page title");

            BrowserFactory.LoadApplication("https://www.google.com");
            string title = BrowserFactory.Driver.Title;
            if (title == "Google")
            {
                Console.WriteLine("Driver title is: " + title);
            }
            else
            {
                Console.WriteLine("Failed! Driver title is: " + title);
            }
        }
    }
}
