using OpenQA.Selenium;

namespace CupAvailabilityChecker.Core.Availability
{
    /// <summary>
    /// Placeholder refresher for <see cref="Models.BookingMode.New"/>, falling back to a full
    /// page reload.
    /// </summary>
    /// <remarks>
    /// TODO: replace with the actual "refresh results" button once the "New recipe" availability
    /// page's DOM has been inspected with the browser's dev tools.
    /// </remarks>
    public sealed class NewRecipeAvailabilityRefresher : IAvailabilityRefresher
    {
        public void Refresh(IWebDriver driver) => driver.Navigate().Refresh();
    }
}
