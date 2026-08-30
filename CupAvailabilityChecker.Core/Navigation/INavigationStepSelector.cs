using CupAvailabilityChecker.Core.Models;

namespace CupAvailabilityChecker.Core.Navigation
{
    /// <summary>
    /// Selects the <see cref="INavigationStep"/> implementation to use for a given
    /// <see cref="BookingMode"/>.
    /// </summary>
    public interface INavigationStepSelector
    {
        INavigationStep GetStep(BookingMode mode);
    }
}
