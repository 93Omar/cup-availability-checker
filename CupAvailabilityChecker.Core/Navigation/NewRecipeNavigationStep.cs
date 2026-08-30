using CupAvailabilityChecker.Core.Browser;
using CupAvailabilityChecker.Core.Models;
using OpenQA.Selenium;

namespace CupAvailabilityChecker.Core.Navigation
{
    /// <summary>
    /// Step 1 for <see cref="BookingMode.New"/>: logs in from the "Ricetta Elettronica" page and
    /// applies the area/provincia/comuni/raggio filters to reach the availability page.
    /// </summary>
    /// <remarks>
    /// TODO: the filter selectors below are placeholders. Replace them once the authenticated
    /// CupWeb DOM has been inspected with the browser's dev tools.
    /// </remarks>
    public sealed class NewRecipeNavigationStep : INavigationStep
    {
        private const string StartUrl = "https://cupweb.sardegnasalute.it/web/guest/ricetta-elettronica";

        private static readonly WebElementLocator AreaSelect = new("area", WebElementLocatorType.Id);
        private static readonly WebElementLocator ProvinceSelect = new("provincia", WebElementLocatorType.Id);
        private static readonly WebElementLocator SearchButton = new("cercaButton", WebElementLocatorType.Id);

        private readonly CupLoginHelper _loginHelper;

        public NewRecipeNavigationStep(CupLoginHelper loginHelper)
        {
            _loginHelper = loginHelper;
        }

        public void NavigateToAvailabilityPage(IWebDriver driver, AvailabilityCheckContext context)
        {
            driver.Navigate().GoToUrl(StartUrl);
            _loginHelper.Login(driver, context.Parameters);

            // TODO: select area/provincia/comuni/raggio filters from context.Parameters, then
            // submit the search.
            driver.FindElement(SearchButton.ToBy()).Click();
        }
    }
}
