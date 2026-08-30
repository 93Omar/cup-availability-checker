using CupAvailabilityChecker.Core.Models;

namespace CupAvailabilityChecker.Core.Availability
{
    /// <summary>
    /// For <see cref="BookingMode.Existing"/>: a slot is interesting if its date comes before
    /// <see cref="AvailabilityCheckContext.CurrentBookingDate"/>, at parity of the search filters
    /// already applied by the "Le mie Prenotazioni" flow.
    /// </summary>
    public sealed class ExistingBookingAvailabilityMatcher : IAvailabilityMatcher
    {
        public bool IsInteresting(AvailabilitySlot slot, AvailabilityCheckContext context)
        {
            DateOnly? currentBookingDate = context.CurrentBookingDate;
            if (currentBookingDate is null)
                return false;

            return slot.Date < currentBookingDate.Value;
        }
    }
}
