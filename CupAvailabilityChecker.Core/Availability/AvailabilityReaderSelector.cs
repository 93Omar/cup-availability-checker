using CupAvailabilityChecker.Core.Models;

namespace CupAvailabilityChecker.Core.Availability
{
    /// <summary>
    /// Composes the two concrete <see cref="IAvailabilityReader"/> implementations and picks the
    /// right one for the requested <see cref="BookingMode"/>.
    /// </summary>
    public sealed class AvailabilityReaderSelector : IAvailabilityReaderSelector
    {
        private readonly NewRecipeAvailabilityReader _newRecipeReader;
        private readonly ExistingBookingAvailabilityReader _existingBookingReader;

        public AvailabilityReaderSelector(NewRecipeAvailabilityReader newRecipeReader, ExistingBookingAvailabilityReader existingBookingReader)
        {
            _newRecipeReader = newRecipeReader;
            _existingBookingReader = existingBookingReader;
        }

        public IAvailabilityReader GetReader(BookingMode mode)
        {
            return mode switch
            {
                BookingMode.New => _newRecipeReader,
                BookingMode.Existing => _existingBookingReader,
                _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, "Modalità di prenotazione non supportata."),
            };
        }
    }
}
