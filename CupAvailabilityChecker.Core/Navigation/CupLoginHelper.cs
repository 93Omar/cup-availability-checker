using CupAvailabilityChecker.Core.Browser;
using CupAvailabilityChecker.Core.Models;
using OpenQA.Selenium;

namespace CupAvailabilityChecker.Core.Navigation
{
    /// <summary>
    /// Encapsulates the login step shared by both the "New recipe" and "Existing booking" flows.
    /// Composed by the concrete <see cref="INavigationStep"/> implementations instead of being
    /// inherited from a common base class, per this codebase's "composition over inheritance"
    /// convention.
    /// </summary>
    /// <remarks>
    /// TODO: the selectors below are placeholders. Replace them once the authenticated CupWeb
    /// DOM has been inspected with the browser's dev tools.
    /// </remarks>
    public sealed class CupLoginHelper
    {
        private static readonly WebElementLocator FiscalCodeInput = new("fiscalCode", WebElementLocatorType.Id);
        private static readonly WebElementLocator NreInput = new("nre", WebElementLocatorType.Id);
        private static readonly WebElementLocator LoginButton = new("loginButton", WebElementLocatorType.Id);

        public void Login(IWebDriver driver, BookingParameters parameters)
        {
            driver.FindElement(FiscalCodeInput.ToBy()).SendKeys(parameters.FiscalCode);
            driver.FindElement(NreInput.ToBy()).SendKeys(parameters.Nre);
            driver.FindElement(LoginButton.ToBy()).Click();
        }
    }
}
