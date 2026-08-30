using CupAvailabilityChecker.Core.Models;
using OpenQA.Selenium;

namespace CupAvailabilityChecker.Core.Navigation
{
    /// <summary>
    /// Step 1 of the flow: navigates from the CUP starting page to the availability page, using
    /// the filters carried by <see cref="AvailabilityCheckContext"/>. Two concrete
    /// implementations exist ("New recipe" and "Existing booking"), selected via
    /// <see cref="INavigationStepSelector"/> according to <see cref="BookingMode"/>, since the
    /// starting page, the login and the filters form differ between the two flows.
    /// </summary>
    public interface INavigationStep
    {
        void NavigateToAvailabilityPage(IWebDriver driver, AvailabilityCheckContext context);
    }
}
