using OpenQA.Selenium;

namespace CupAvailabilityChecker.Core.Utilities
{
    /// <summary>
    /// Removes the cookie consent banner shown by the regional site (a div with id "cookie-bar")
    /// from the page, since it overlaps buttons further down the page and intercepts clicks meant
    /// for them. It is removed outright via JavaScript rather than clicked through, as its own
    /// "accept" control is not part of the booking flow we need to automate.
    /// </summary>
    public sealed class CookieBannerDismisser
    {
        private const string RemoveCookieBannerScript =
            "var el = document.getElementById('cookie-bar'); if (el) { el.parentNode.removeChild(el); }";

        /// <summary>
        /// Should be called right after every page load (initial navigation or full-page
        /// redirect), before clicking any element on the page.
        /// </summary>
        public void Dismiss(IWebDriver driver)
        {
            ((IJavaScriptExecutor)driver).ExecuteScript(RemoveCookieBannerScript);
        }
    }
}
