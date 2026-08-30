namespace CupAvailabilityChecker.Core.Models
{
    /// <summary>
    /// Aggregates all the parameters gathered from the CLI, needed to drive the Selenium
    /// navigation and the availability polling loop.
    /// </summary>
    public sealed record BookingParameters(
        string FiscalCode,
        string Nre,
        BookingMode Mode,
        Area Area,
        Province Province,
        IReadOnlyList<string>? Municipalities,
        string? ReferenceMunicipality,
        double? RadiusKm,
        int? Days,
        TimeSpan RefreshInterval,
        BrowserType Browser,
        bool Headless);
}
