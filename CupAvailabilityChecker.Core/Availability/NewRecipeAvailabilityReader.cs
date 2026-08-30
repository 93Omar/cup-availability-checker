using CupAvailabilityChecker.Core.Models;
using OpenQA.Selenium;

namespace CupAvailabilityChecker.Core.Availability
{
    /// <summary>
    /// Reads the availability slots from the "Ricetta Elettronica" results table.
    /// </summary>
    /// <remarks>
    /// TODO: the selectors below are placeholders. Replace them once the authenticated CupWeb
    /// DOM has been inspected with the browser's dev tools.
    /// </remarks>
    public sealed class NewRecipeAvailabilityReader : IAvailabilityReader
    {
        private const string ResultRowSelector = ".risultato-disponibilita-row";
        private const string DateCellSelector = ".data";
        private const string TimeCellSelector = ".ora";
        private const string LocationCellSelector = ".sede";

        public IReadOnlyList<AvailabilitySlot> ReadCurrentAvailability(IWebDriver driver)
        {
            IReadOnlyList<IWebElement> rows = driver.FindElements(By.CssSelector(ResultRowSelector));
            List<AvailabilitySlot> slots = new List<AvailabilitySlot>(rows.Count);

            foreach (IWebElement row in rows)
            {
                DateOnly date = DateOnly.Parse(row.FindElement(By.CssSelector(DateCellSelector)).Text);
                TimeOnly time = TimeOnly.Parse(row.FindElement(By.CssSelector(TimeCellSelector)).Text);
                string location = row.FindElement(By.CssSelector(LocationCellSelector)).Text;

                slots.Add(new AvailabilitySlot(date, time, location));
            }

            return slots;
        }
    }
}
