using CupAvailabilityChecker.Core.Models;
using OpenQA.Selenium;

namespace CupAvailabilityChecker.Core.Availability
{
    /// <summary>
    /// Step 2 of the flow: reads the availability slots currently shown on the availability
    /// page's DOM. Two concrete implementations exist ("New recipe" and "Existing booking"),
    /// selected via <see cref="IAvailabilityReaderSelector"/> according to
    /// <see cref="BookingMode"/>, since the results table markup differs between the two flows.
    /// </summary>
    public interface IAvailabilityReader
    {
        IReadOnlyList<AvailabilitySlot> ReadCurrentAvailability(IWebDriver driver);
    }
}
