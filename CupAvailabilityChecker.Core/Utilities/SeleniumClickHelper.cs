using Microsoft.Extensions.Logging;
using OpenQA.Selenium;

namespace CupAvailabilityChecker.Core.Utilities
{
    /// <summary>
    /// Clicks Selenium elements reliably even when another element (e.g. an adjacent button, or
    /// one temporarily overlapping it) would otherwise intercept the click at its on-screen point.
    /// </summary>
    public sealed class SeleniumClickHelper
    {
        private readonly ILogger<SeleniumClickHelper> _logger;

        public SeleniumClickHelper(ILogger<SeleniumClickHelper> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Scrolls the element into view first, then falls back to a JavaScript-dispatched click
        /// (which does not depend on screen coordinates) if a normal click is intercepted.
        /// </summary>
        public void Click(IWebDriver driver, IWebElement element)
        {
            IJavaScriptExecutor javaScriptExecutor = (IJavaScriptExecutor)driver;
            javaScriptExecutor.ExecuteScript("arguments[0].scrollIntoView({block: 'center'});", element);

            try
            {
                element.Click();
            }
            catch (ElementClickInterceptedException ex)
            {
                _logger.LogWarning(ex, "Click intercettato da un altro elemento, eseguo il click via JavaScript.");
                javaScriptExecutor.ExecuteScript("arguments[0].click();", element);
            }
        }
    }
}
