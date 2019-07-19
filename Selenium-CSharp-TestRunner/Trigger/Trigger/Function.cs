using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.Lambda;
using Amazon.Lambda.Core;
using Amazon.Lambda.Model;
using Amazon.S3;
using Amazon.S3.Transfer;
using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace Trigger
{
    public class Function
    {
        string _results;
        /// <summary>
        /// A function that sends test requests to selenium test runner
        /// </summary>
        /// <param name="suite"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<string> FunctionHandler(string suite, ILambdaContext context)
        {
            if (!Directory.Exists("/tmp/TestSuite"))
            {
                TestSetup(suite);
            }
            else
            {
                LambdaCleanUp();
                TestSetup(suite);
            }
            try
            {
                List<TestRequest> tests = TestSuite.GetTests("/tmp/TestSuite/" + suite + ".dll"); 

                var tasks = tests.Select(async test =>
                {
                    using (AmazonLambdaClient client = new AmazonLambdaClient("[RegionEndpoint]"))
                    {
                        string input = JsonConvert.SerializeObject(test).Replace("\"", "\\\""); 
                        
                        Console.WriteLine(input);
                        var request = new InvokeRequest
                        {
                            FunctionName = "runner",
                            Payload = "\"" + input + "\""
                        };
                        
                        var response = await client.InvokeAsync(request);

                        using (var sr = new StreamReader(response.Payload))
                        {
                            _results += await sr.ReadToEndAsync() + System.Environment.NewLine;

                        }
                    }

                });
                await Task.WhenAll(tasks);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return _results;
        }

        private void TestSetup(string TestSuite)
        {
            //Copy required files from s3
            Console.WriteLine("Copying testsuite to lambda 'tmp'");
            start:
            try
            {
                TransferUtility fileTransferUtility =
                    new TransferUtility(
                        new AmazonS3Client("[Access key ID]", "[Secret access key]", "[RegionEndpoint]"));

                // Note the 'fileName' is the 'key' of the object in S3 (which is usually just the file name)
                fileTransferUtility.Download("/tmp/" + TestSuite + ".zip", "[Bucket]", TestSuite + ".zip");
            }
            catch
            {
                goto start;

            }

            Console.WriteLine("Completed copy!");

            //Unzip testsuite
            Console.WriteLine("Create new TestSuite directory and unzip " + TestSuite + " to it");
            Process.Start("mkdir", "/tmp/TestSuite");
            using (ZipArchive archive = ZipFile.OpenRead("/tmp/" + TestSuite + ".zip"))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    retry:
                    try
                    {
                        if (!File.Exists("/tmp/TestSuite/ " + entry.FullName))
                        {
                            entry.ExtractToFile(Path.Combine("/tmp/TestSuite/", entry.FullName));
                            Console.WriteLine(entry.FullName + " unzipped to TestSuite directory");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        goto retry;
                    }
                }
            }

            Console.WriteLine("Testsuite unzipped!");

            //Remove testsuite.zip
            Console.WriteLine("Removing " + TestSuite + ".zip to free up space");
            File.Delete("/tmp/" + TestSuite + ".zip");
        }

        private void LambdaCleanUp()
        {
            //Delete sub directories
            Console.WriteLine("Deleting sub directories from '/tmp'");
            foreach (var subDir in new DirectoryInfo("/tmp/").GetDirectories())
            {
                subDir.Delete(true);
                Console.WriteLine(subDir + " deleted!");
            }

            Console.WriteLine("Deleting files from '/tmp'");
            foreach (string fileName in Directory.GetFiles("/tmp/"))
            {
                Console.WriteLine("Deleting: " + fileName);
                File.Delete(fileName);
            }
        }
    }
}
