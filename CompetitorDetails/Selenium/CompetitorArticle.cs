using NUnit.Framework;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using System;
using CompetitorDetails.Data.Wait.FluentW;
using CompetitorDetails.Models;
using OpenQA.Selenium.Remote;
using Microsoft.Extensions.Options;

namespace CompetitorDetails.Selenium
{
    public class CompetitorArticle
    {
        public List<ArticleDetail> TestGoogleSearch1(string url)
        {
            // Create a temporary directory for the user data directory
            string tempUserDataDir = Path.Combine("/tmp", Guid.NewGuid().ToString());

           // url = "https://www.google.com/";
            Console.WriteLine(url);
            List<ArticleDetail> articleDetails = new List<ArticleDetail>();
            // Initialize ChromeDriver
            var browserOptions = new ChromeOptions();

            

            var cloudOptions = new Dictionary<string, object>();
            browserOptions.AddAdditionalOption("cloud:options", cloudOptions);
            //browserOptions.AddArgument("--headless");
            //browserOptions.AddArgument($"--user-data-dir={tempUserDataDir}");
            // Disable browser notifications
            browserOptions.AddArgument("--disable-notifications");

            // Disable browser extensions
            browserOptions.AddArgument("--disable-extensions");

            // Disable image loading to speed up browsing
            browserOptions.AddArgument("--blink-settings=imagesEnabled=false");

            // Specify the hub URL
            var hubUrl = "http://localhost:4444/ui/wd/hub";
            //var hubUrl = "http://selenium-hub:4444/wd/hub";


            // Initialize a Chrome WebDriver
            using (var driver = new RemoteWebDriver(new Uri(hubUrl), browserOptions))
            {
                try
                {
                    Console.WriteLine("starting browser");
                    // Navigate to the specified URL
                    driver.Navigate().GoToUrl(url);
                    System.Threading.Thread.Sleep(10000);
                    var article = new ArticleDetail()
                    {
                        ArticleTime = "2h",
                        ArticleTitle = driver.Title,
                    };
                    articleDetails.Add(article);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw;
                }
                finally 
                {
                    if (driver != null)
                    {
                        driver.Quit();
                        // Clean up temporary directory
                        if (File.Exists(tempUserDataDir))
                        {
                            Directory.Delete(tempUserDataDir, true);
                        }
                    }
                }
            }
            return articleDetails;
        }
        public List<ArticleDetail> TestGoogleSearch(string url)
        {
            // Specify the hub URL
            var hubUrl = "http://selenium-hub:4444/wd/hub";
            Console.WriteLine(url);
            List<ArticleDetail> articleDetails = new List<ArticleDetail>();
            // Create a temporary directory for the user data directory
            string tempUserDataDir = Path.Combine("/tmp", Guid.NewGuid().ToString());
            //string tempUserDataDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            // Initialize ChromeDriver
            var options = new ChromeOptions();

            var cloudOptions = new Dictionary<string, object>();
            options.AddAdditionalOption("cloud:options", cloudOptions);

            //options.AddArgument("--headless"); // Optional: Run Chrome in headless mode
                                               // Set user data directory to a temporary directory
            options.AddArgument($"--user-data-dir={tempUserDataDir}");

            // Enable cookies
            options.AddArgument("--enable-cookies");

            // Enable JavaScript execution
            options.AddArgument("--enable-javascript");

            // Disable browser notifications
            options.AddArgument("--disable-notifications");

            // Disable browser extensions
            options.AddArgument("--disable-extensions");

            // Disable image loading to speed up browsing
            options.AddArgument("--blink-settings=imagesEnabled=false");

            // Set window size to emulate common screen resolutions
            options.AddArgument("--window-size=1366,768");

            // Disable infobars
            options.AddArgument("--disable-infobars");

            // Disable the "Chrome is being controlled by automated test software" infobar
            options.AddExcludedArgument("enable-automation");

            // Disable devtools opening by default
            options.AddExcludedArgument("enable-devtools-experiments");

            // Disable saving passwords
            options.AddUserProfilePreference("credentials_enable_service", false);
            options.AddUserProfilePreference("profile.password_manager_enabled", false);

            // Set a specific language for the browser (e.g., English)
            options.AddArgument("--lang=en-US");

            // Initialize a Chrome WebDriver
            using (var driver = new RemoteWebDriver(new Uri(hubUrl), options))
            {
                // Navigate to the specified URL
                driver.Navigate().GoToUrl(url);
                Console.WriteLine("starting browser");
                // Create a FluentWait instance
                FluentWait wait = new FluentWait(driver)
                .WithTimeout(TimeSpan.FromSeconds(30)) // Set timeout
                .PollingEvery(TimeSpan.FromSeconds(2)); // Set polling interval

                // Wait until the specified condition is met
                wait.Until(d => {
                    try
                    {
                        // Pause for a short while to allow content to load
                        System.Threading.Thread.Sleep(5000); // Adjust as needed
                                                             // Attempt to find the desired element
                        IWebElement element = d.FindElement(By.XPath("/html/body/fluent-design-system-provider/channel-page"));

                        // If the element is found, return true
                        return element != null;
                    }
                    catch (NoSuchElementException)
                    {
                        // If element is not found, return false
                        return false;
                    }
                });
                Console.WriteLine("after wait");
                System.Threading.Thread.Sleep(5000); // Adjust as needed
                                                     // Initialize JavaScript executor
                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                //js.ExecuteScript("window.scrollBy(0,500)");
                // Retrieve body height using clientHeight
                long bodyHeight = (long)js.ExecuteScript("return document.body.scrollHeight;");

                try
                {
                    // Get the initial page height
                    long initialHeight = GetPageHeight(driver);

                    // Set the scroll increment
                    int scrollIncrement = 2000; // Adjust as needed

                    int counte = 0;

                    // Loop until we reach the end of the page
                    while (true)
                    {
                        // Scroll down by the specified increment
                        ScrollPage(driver, 0, scrollIncrement);

                        // Wait for a short while to allow content to load
                        System.Threading.Thread.Sleep(2000);

                        // Get the current page height
                        //long currentHeight = GetPageHeight(driver);

                        bool element = CheckElement(driver);
                        // Check if we have reached the end of the page
                        if (element == true || counte == 20)
                        {
                            break;
                        }
                        counte++;
                        scrollIncrement = +scrollIncrement;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: " + ex.Message);
                }

                try
                {
                    IWebElement articlesContainer = driver.FindElement(By.XPath("/html/body/fluent-design-system-provider/channel-page")).GetShadowRoot()
                                                   .FindElement(By.CssSelector("div:nth-child(2) > msn-channel-linear-feed")).GetShadowRoot()
                                                   .FindElement(By.Id("articles-container"));

                    // Find all clf-cs-card elements within the container using XPath
                    IList<IWebElement> clfCsCards = articlesContainer.FindElements(By.XPath(".//clf-cs-card"));

                    foreach (IWebElement clfCsCard in clfCsCards)
                    {
                        IWebElement clfCs = clfCsCard.GetShadowRoot().FindElement(By.CssSelector("cs-card > cs-content-card > div.provider > span.date"));
                        IWebElement TitleclfCs = clfCsCard.GetShadowRoot().FindElement(By.CssSelector("cs-card > cs-content-card > div.title"));

                        // Get the title text
                        string title = TitleclfCs.GetAttribute("title");
                        // Do something with each clf-cs-card element
                        // For example, you can retrieve text or attributes
                        //Console.WriteLine(clfCs.Text);
                        var article = new ArticleDetail()
                        {
                            ArticleTime = clfCs.Text,
                            ArticleTitle = title,
                        };
                        articleDetails.Add(article);
                    }
                    return articleDetails;

                }
                catch (NoSuchElementException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    if (driver != null)
                    {
                        driver.Quit();
                        // Clean up temporary directory
                        if (File.Exists(tempUserDataDir))
                        {
                            Directory.Delete(tempUserDataDir, true);
                        }
                    }
                }
            }
            return articleDetails;
        }

        static long GetPageHeight(IWebDriver driver)
        {
            return (long)((IJavaScriptExecutor)driver).ExecuteScript("return document.body.scrollHeight");
        }

        static void ScrollPage(IWebDriver driver, int xOffset, int yOffset)
        {
            ((IJavaScriptExecutor)driver).ExecuteScript($"window.scrollBy({xOffset}, {yOffset});");
        }
        static bool CheckElement(IWebDriver driver)
        {
            try
            {
                // Find the shadow host element
                IWebElement shadowHost = driver.FindElement(By.XPath("/html/body/fluent-design-system-provider/channel-page")).GetShadowRoot()
                                               .FindElement(By.CssSelector("div:nth-child(2) > msn-channel-linear-feed")).GetShadowRoot()
                                               .FindElement(By.CssSelector("#articles-container > li:nth-child(14) > msn-channel-publisher-link"));
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }
    }
}
