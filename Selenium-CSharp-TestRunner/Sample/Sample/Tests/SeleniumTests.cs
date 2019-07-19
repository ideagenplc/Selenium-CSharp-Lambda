using System;
using NUnit.Framework;
using Sample.WrapperFactory;

namespace Sample.Tests
{
    class SeleniumTests
    {
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

        [Test]
        public void BBCTest()
        {
            //Initialise the driver
            BrowserFactory.InitBrowser();

            Console.WriteLine("Check web page title");

            BrowserFactory.LoadApplication("https://www.bbc.co.uk/");
            string title = BrowserFactory.Driver.Title;
            if (title == "BBC - Home")
            {
                Console.WriteLine("Driver title is: " + title);
            }
            else
            {
                Console.WriteLine("Failed! Driver title is: " + title);
            }
        }

        [Test]
        public void BingTest()
        {
            //Initialise the driver
            BrowserFactory.InitBrowser();

            Console.WriteLine("Check web page title");

            BrowserFactory.LoadApplication("https://www.bing.com/");
            string title = BrowserFactory.Driver.Title;
            if (title == "Bing")
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
