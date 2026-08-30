using CupAvailabilityChecker.Core.Browser;
using CupAvailabilityChecker.Core.Utilities;
using OpenQA.Selenium;

namespace CupAvailabilityChecker.Core.Availability
{
    /// <summary>
    /// Refreshes the "Le mie Prenotazioni" availability results by clicking the filters' update
    /// button again (without touching the applied area/provincia filters), and waits for the
    /// results to be redrawn by the AJAX (JSF/PrimeFaces) update.
    /// </summary>
    public sealed class ExistingBookingAvailabilityRefresher : IAvailabilityRefresher
    {
        private static readonly WebElementLocator RefreshFiltersButton =
            new("_ricettaelettronica_WAR_cupprenotazione_:appuntamentiForm:_t223_button", WebElementLocatorType.Name);

        private readonly SeleniumWaitHelper _waitHelper;
        private readonly SeleniumClickHelper _clickHelper;

        public ExistingBookingAvailabilityRefresher(SeleniumWaitHelper waitHelper, SeleniumClickHelper clickHelper)
        {
            _waitHelper = waitHelper;
            _clickHelper = clickHelper;
        }

        public void Refresh(IWebDriver driver)
        {
            IWebElement refreshFiltersButton = _waitHelper.WaitUntilVisible(driver, RefreshFiltersButton.ToBy());
            _clickHelper.Click(driver, refreshFiltersButton);
            _waitHelper.WaitForPageToSettle(driver);
            _waitHelper.WaitUntilVisible(driver, By.CssSelector($"span[id$='{ExistingBookingAvailabilityReader.DateSpanIdSuffix}']"));
        }
    }
}
