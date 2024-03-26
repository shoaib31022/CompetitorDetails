using OpenQA.Selenium;

namespace CompetitorDetails.Data.Wait.FluentW
{
    public class FluentWait
    {
        private readonly IWebDriver driver;
        private TimeSpan timeout;
        private TimeSpan pollingInterval;
        private Func<IWebDriver, bool> condition;

        public FluentWait(IWebDriver driver)
        {
            this.driver = driver;
            this.timeout = TimeSpan.FromSeconds(30); // Default timeout
            this.pollingInterval = TimeSpan.FromSeconds(2); // Default polling interval
        }

        public FluentWait WithTimeout(TimeSpan timeout)
        {
            this.timeout = timeout;
            return this;
        }

        public FluentWait PollingEvery(TimeSpan pollingInterval)
        {
            this.pollingInterval = pollingInterval;
            return this;
        }

        public void Until(Func<IWebDriver, bool> condition)
        {
            this.condition = condition;
            var endTime = DateTime.Now.Add(timeout);
            while (DateTime.Now < endTime)
            {
                try
                {
                    if (condition(driver))
                    {
                        return;
                    }
                }
                catch
                {
                    // Ignore exceptions for now
                }
                Thread.Sleep(pollingInterval);
            }
            throw new TimeoutException("Timed out after " + timeout);
        }
    }
}
