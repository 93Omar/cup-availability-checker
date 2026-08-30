namespace CupAvailabilityChecker.Core.Models
{
    /// <summary>
    /// Mutable context threaded through the navigation/polling steps for a single run: besides
    /// the immutable <see cref="BookingParameters"/>, it carries state discovered at runtime,
    /// such as the date of the currently existing booking (only meaningful for
    /// <see cref="BookingMode.Existing"/>), needed to decide whether a newly read availability is
    /// an improvement.
    /// </summary>
    public sealed class AvailabilityCheckContext
    {
        public BookingParameters Parameters { get; }

        public DateOnly? CurrentBookingDate { get; set; }

        public TimeOnly? CurrentBookingTime { get; set; }

        public AvailabilityCheckContext(BookingParameters parameters)
        {
            Parameters = parameters;
        }
    }
}
