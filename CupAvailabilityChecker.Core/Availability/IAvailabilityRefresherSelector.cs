using CupAvailabilityChecker.Core.Models;

namespace CupAvailabilityChecker.Core.Availability
{
    /// <summary>
    /// Selects the <see cref="IAvailabilityRefresher"/> implementation to use for a given
    /// <see cref="BookingMode"/>.
    /// </summary>
    public interface IAvailabilityRefresherSelector
    {
        IAvailabilityRefresher GetRefresher(BookingMode mode);
    }
}
