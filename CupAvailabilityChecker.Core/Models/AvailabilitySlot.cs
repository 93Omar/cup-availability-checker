namespace CupAvailabilityChecker.Core.Models
{
    /// <summary>
    /// A single availability slot read from the CUP availability page's DOM.
    /// </summary>
    public sealed record AvailabilitySlot(DateOnly Date, TimeOnly? Time, string Location);
}
