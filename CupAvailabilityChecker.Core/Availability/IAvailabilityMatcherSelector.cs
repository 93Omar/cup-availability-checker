using CupAvailabilityChecker.Core.Models;

namespace CupAvailabilityChecker.Core.Availability
{
    /// <summary>
    /// Selects the <see cref="IAvailabilityMatcher"/> implementation to use for a given
    /// <see cref="BookingMode"/>.
    /// </summary>
    public interface IAvailabilityMatcherSelector
    {
        IAvailabilityMatcher GetMatcher(BookingMode mode);
    }
}
