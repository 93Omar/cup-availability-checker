using CupAvailabilityChecker.Core.Models;
using CupAvailabilityChecker.Core.Utilities;
using OpenQA.Selenium;

namespace CupAvailabilityChecker.Core.Availability
{
    /// <summary>
    /// Reads the availability slots from the "Le mie Prenotazioni" better-date search results.
    /// Each result row renders its date, time and location spans with a stable id suffix (the
    /// row index varies, the trailing JSF component id doesn't), so all date/time/location spans
    /// on the page are matched by id suffix and zipped together in DOM order.
    /// </summary>
    public sealed class ExistingBookingAvailabilityReader : IAvailabilityReader
    {
        public const string DateSpanIdSuffix = "_t267";
        public const string TimeSpanIdSuffix = "_t269";
        public const string LocationSpanIdSuffix = "_t292";

        public IReadOnlyList<AvailabilitySlot> ReadCurrentAvailability(IWebDriver driver)
        {
            IReadOnlyList<IWebElement> dateSpans = driver.FindElements(By.CssSelector($"span[id$='{DateSpanIdSuffix}']"));
            IReadOnlyList<IWebElement> timeSpans = driver.FindElements(By.CssSelector($"span[id$='{TimeSpanIdSuffix}']"));
            IReadOnlyList<IWebElement> locationSpans = driver.FindElements(By.CssSelector($"span[id$='{LocationSpanIdSuffix}']"));

            int slotCount = Math.Min(dateSpans.Count, Math.Min(timeSpans.Count, locationSpans.Count));
            List<AvailabilitySlot> slots = new List<AvailabilitySlot>(slotCount);

            for (int i = 0; i < slotCount; i++)
            {
                DateOnly date = ItalianDateTimeParser.ParseDate(dateSpans[i].Text);
                TimeOnly time = ItalianDateTimeParser.ParseTime(timeSpans[i].Text);
                string location = CleanLocation(locationSpans[i].Text);

                slots.Add(new AvailabilitySlot(date, time, location));
            }

            return slots;
        }

        // The location span's text starts with a " - " separator (e.g. " - NUORO (NU)"), which is
        // trimmed for a clean location name.
        private static string CleanLocation(string rawText)
        {
            string trimmed = rawText.Trim();
            return trimmed.StartsWith("- ", StringComparison.Ordinal) ? trimmed[2..] : trimmed;
        }
    }
}
