namespace CupAvailabilityChecker.Core.Models
{
    /// <summary>
    /// Distinguishes whether the CLI is searching for a brand new booking, or checking for
    /// better availability compared to an already existing booking.
    /// </summary>
    public enum BookingMode
    {
        New,
        Existing
    }
}
