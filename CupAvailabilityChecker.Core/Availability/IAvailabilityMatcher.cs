using CupAvailabilityChecker.Core.Models;

namespace CupAvailabilityChecker.Core.Availability
{
    /// <summary>
    /// Step 3 of the flow: decides whether a slot read by an <see cref="IAvailabilityReader"/> is
    /// "interesting" enough to be notified. Two concrete implementations exist ("New recipe" and
    /// "Existing booking"), selected via <see cref="IAvailabilityMatcherSelector"/> according to
    /// <see cref="BookingMode"/>, since the matching criteria differ between the two flows.
    /// </summary>
    public interface IAvailabilityMatcher
    {
        bool IsInteresting(AvailabilitySlot slot, AvailabilityCheckContext context);
    }
}
