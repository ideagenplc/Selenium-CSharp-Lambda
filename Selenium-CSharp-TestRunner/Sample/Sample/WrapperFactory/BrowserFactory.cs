using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Sample.WrapperFactory
{
    public class BrowserFactory
    {
        private static IWebDriver driver;
        public static IWebDriver Driver
        {
            get
            {
                if (driver == null)
                    throw new NullReferenceException("The WebDriver browser instance was not initialised. You should first call the method InitBrowser.");
                return driver;
            }
            private set
            {
                driver = value;
            }
        }

        public static void InitBrowser()
        {
            driver = new ChromeDriver("/tmp", options(), TimeSpan.FromSeconds(300));
        }

        public static void LoadApplication(string url)
        {
            Driver.Url = url;
        }

        public static ChromeOptions options()
        {
            Console.WriteLine("Setting up chrome options");
            ChromeOptions chrome_options = new ChromeOptions();

            if (!System.IO.Directory.Exists("/tmp/data-path"))
            {
                System.IO.Directory.CreateDirectory("/tmp/data-path");
            }

            if (!System.IO.Directory.Exists("/tmp/cache-dir"))
            {
                System.IO.Directory.CreateDirectory("/tmp/cache-dir");
            }

            if (!System.IO.Directory.Exists("/tmp/homedir"))
            {
                System.IO.Directory.CreateDirectory("/tmp/homedir");
            }

            chrome_options.AddArgument("--no-sandbox");
            chrome_options.AddArgument("--disable-gpu");
            chrome_options.AddArgument("--headless");
            chrome_options.AddArgument("--window-size=1920x1080");
            chrome_options.AddArgument("--start-maximized");
            chrome_options.AddAdditionalCapability("useAutomationExtension", false);
            chrome_options.AddArgument("--single-process");
            chrome_options.AddArgument("--data-path=/tmp/data-path");
            chrome_options.AddArgument("--homedir=/tmp/homedir");
            chrome_options.AddArgument("--disk-cache-dir=/tmp/cache-dir");
            chrome_options.AddArgument("--allow-file-access-from-files");
            chrome_options.AddArgument("--disable-web-security");
            chrome_options.AddArgument("--disable-extensions");
            chrome_options.AddArgument("--ignore-certificate-errors");
            chrome_options.AddArgument("--disable-ntp-most-likely-favicons-from-server");
            chrome_options.AddArgument("--disable-ntp-popular-sites");
            chrome_options.AddArgument("--disable-infobars");
            chrome_options.AddArgument("--disable-dev-shm-usage");
            chrome_options.BinaryLocation = "/tmp/chrome";

            //Return options.
            return chrome_options;
        }
    }
}
