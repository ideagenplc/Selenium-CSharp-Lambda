using System;
using NUnit.Framework;
using Sample.WrapperFactory;

namespace Sample.Tests
{
    class SeleniumTests2
    {
        [Test]
        public void AmazonTest()
        {
            //Initialise the driver
            BrowserFactory.InitBrowser();

            Console.WriteLine("Check web page title");

            BrowserFactory.LoadApplication("https://www.amazon.com/");
            string title = BrowserFactory.Driver.Title;
            if (title == "Amazon.com: Online Shopping for Electronics, Apparel, Computers, Books, DVDs & more")
            {
                Console.WriteLine("Driver title is: " + title);
            }
            else
            {
                Console.WriteLine("Failed! Driver title is: " + title);
            }
        }

        [Test]
        public void AWSTest()
        {
            //Initialise the driver
            BrowserFactory.InitBrowser();

            Console.WriteLine("Check web page title");

            BrowserFactory.LoadApplication("https://aws.amazon.com/");
            string title = BrowserFactory.Driver.Title;
            if (title == "Amazon Web Services (AWS) - Cloud Computing Services")
            {
                Console.WriteLine("Driver title is: " + title);
            }
            else
            {
                Console.WriteLine("Failed! Driver title is: " + title);
            }
        }

        [Test]
        public void AzureTest()
        {
            //Initialise the driver
            BrowserFactory.InitBrowser();

            Console.WriteLine("Check web page title");

            BrowserFactory.LoadApplication("https://azure.microsoft.com/en-gb/");
            string title = BrowserFactory.Driver.Title;
            if (title == "Microsoft Azure Cloud Computing Platform & Services")
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
