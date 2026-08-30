using CupAvailabilityChecker.Core.Browser;
using OpenQA.Selenium;

namespace CupAvailabilityChecker.Core.Polling
{
    /// <summary>
    /// Detects session expiry by checking for the presence of the login form on the current
    /// page, which CupWeb shows again once the session times out.
    /// </summary>
    /// <remarks>
    /// TODO: the selector below is a placeholder. Replace it once the authenticated CupWeb DOM
    /// (and its expired-session redirect) has been inspected with the browser's dev tools.
    /// </remarks>
    public sealed class SessionExpiryDetector : ISessionExpiryDetector
    {
        private static readonly WebElementLocator LoginButton = new("loginButton", WebElementLocatorType.Id);

        public bool IsSessionExpired(IWebDriver driver)
            => driver.FindElements(LoginButton.ToBy()).Count > 0;
    }
}
