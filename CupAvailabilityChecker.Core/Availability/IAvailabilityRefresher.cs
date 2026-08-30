using OpenQA.Selenium;

namespace CupAvailabilityChecker.Core.Availability
{
    /// <summary>
    /// Step 2's "refresh" action: triggers a new read of the availability results, without
    /// re-navigating or losing the filters already applied. Two concrete implementations exist
    /// ("New recipe" and "Existing booking"), selected via <see cref="IAvailabilityRefresherSelector"/>
    /// according to <see cref="Models.BookingMode"/>, since the refresh mechanism differs between
    /// the two flows.
    /// </summary>
    public interface IAvailabilityRefresher
    {
        void Refresh(IWebDriver driver);
    }
}
