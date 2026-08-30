using OpenQA.Selenium;

namespace CupAvailabilityChecker.Core.Polling
{
    /// <summary>
    /// Detects whether the browser session has expired (e.g. the site redirected back to a login
    /// page) while polling the availability page.
    /// </summary>
    public interface ISessionExpiryDetector
    {
        bool IsSessionExpired(IWebDriver driver);
    }
}
