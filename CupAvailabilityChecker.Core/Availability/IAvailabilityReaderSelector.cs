using CupAvailabilityChecker.Core.Models;

namespace CupAvailabilityChecker.Core.Availability
{
    /// <summary>
    /// Selects the <see cref="IAvailabilityReader"/> implementation to use for a given
    /// <see cref="BookingMode"/>.
    /// </summary>
    public interface IAvailabilityReaderSelector
    {
        IAvailabilityReader GetReader(BookingMode mode);
    }
}
