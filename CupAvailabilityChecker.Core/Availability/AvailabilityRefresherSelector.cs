using CupAvailabilityChecker.Core.Models;

namespace CupAvailabilityChecker.Core.Availability
{
    /// <summary>
    /// Composes the two concrete <see cref="IAvailabilityRefresher"/> implementations and picks
    /// the right one for the requested <see cref="BookingMode"/>.
    /// </summary>
    public sealed class AvailabilityRefresherSelector : IAvailabilityRefresherSelector
    {
        private readonly NewRecipeAvailabilityRefresher _newRecipeRefresher;
        private readonly ExistingBookingAvailabilityRefresher _existingBookingRefresher;

        public AvailabilityRefresherSelector(NewRecipeAvailabilityRefresher newRecipeRefresher, ExistingBookingAvailabilityRefresher existingBookingRefresher)
        {
            _newRecipeRefresher = newRecipeRefresher;
            _existingBookingRefresher = existingBookingRefresher;
        }

        public IAvailabilityRefresher GetRefresher(BookingMode mode)
        {
            return mode switch
            {
                BookingMode.New => _newRecipeRefresher,
                BookingMode.Existing => _existingBookingRefresher,
                _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, "Modalità di prenotazione non supportata."),
            };
        }
    }
}
