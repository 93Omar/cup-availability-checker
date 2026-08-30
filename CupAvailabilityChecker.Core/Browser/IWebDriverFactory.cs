using CupAvailabilityChecker.Core.Models;
using OpenQA.Selenium;

namespace CupAvailabilityChecker.Core.Browser
{
    /// <summary>
    /// Creates the <see cref="IWebDriver"/> instance used to drive the CUP navigation, for the
    /// browser engine and headless mode requested via the CLI.
    /// </summary>
    public interface IWebDriverFactory
    {
        IWebDriver Create(BrowserType browserType, bool headless);
    }
}
