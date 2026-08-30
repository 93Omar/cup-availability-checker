using CupAvailabilityChecker.Core.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;

namespace CupAvailabilityChecker.Core.Browser
{
    /// <summary>
    /// Creates a Selenium <see cref="IWebDriver"/> for the requested <see cref="BrowserType"/>.
    /// Relies on Selenium Manager (bundled with Selenium.WebDriver 4.6+) to automatically
    /// download/locate the matching driver executable, so no separate driver package is needed.
    /// </summary>
    public sealed class SeleniumWebDriverFactory : IWebDriverFactory
    {
        public IWebDriver Create(BrowserType browserType, bool headless)
        {
            return browserType switch
            {
                BrowserType.Chrome => CreateChromeDriver(headless),
                BrowserType.Edge => CreateEdgeDriver(headless),
                _ => throw new ArgumentOutOfRangeException(nameof(browserType), browserType, "Browser non supportato."),
            };
        }

        private static IWebDriver CreateChromeDriver(bool headless)
        {
            ChromeOptions options = new ChromeOptions();
            if (headless)
                options.AddArgument("--headless=new");

            return new ChromeDriver(options);
        }

        private static IWebDriver CreateEdgeDriver(bool headless)
        {
            EdgeOptions options = new EdgeOptions();
            if (headless)
                options.AddArgument("--headless=new");

            return new EdgeDriver(options);
        }
    }
}
