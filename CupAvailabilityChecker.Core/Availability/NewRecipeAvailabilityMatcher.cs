using CupAvailabilityChecker.Core.Models;

namespace CupAvailabilityChecker.Core.Availability
{
    /// <summary>
    /// For <see cref="BookingMode.New"/>: a slot is interesting if it falls within
    /// <see cref="BookingParameters.Days"/> days from today. A missing "giorni" parameter means no
    /// upper bound, so every slot is considered interesting.
    /// </summary>
    public sealed class NewRecipeAvailabilityMatcher : IAvailabilityMatcher
    {
        public bool IsInteresting(AvailabilitySlot slot, AvailabilityCheckContext context)
        {
            int? days = context.Parameters.Days;
            if (days is null)
                return true;

            DateOnly threshold = DateOnly.FromDateTime(DateTime.Today).AddDays(days.Value);
            return slot.Date <= threshold;
        }
    }
}
