using OpenQA.Selenium;

namespace CupAvailabilityChecker.Core.Browser
{
    /// <summary>
    /// Pairs a DOM element's raw id/name with the Selenium locator strategy to use for it, so
    /// that each element is declared once (value + strategy) instead of scattering
    /// <c>By.Id(...)</c>/<c>By.Name(...)</c> calls across the navigation/availability classes.
    /// </summary>
    public sealed record WebElementLocator(string Value, WebElementLocatorType Type)
    {
        public By ToBy()
        {
            return Type switch
            {
                WebElementLocatorType.Id => By.Id(Value),
                WebElementLocatorType.Name => By.Name(Value),
                _ => throw new ArgumentOutOfRangeException(nameof(Type), Type, "Tipo di selettore non supportato."),
            };
        }
    }
}
