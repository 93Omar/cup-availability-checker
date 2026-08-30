using CupAvailabilityChecker.Core.Models;

namespace CupAvailabilityChecker.Core.Availability
{
    /// <summary>
    /// Composes the two concrete <see cref="IAvailabilityMatcher"/> implementations and picks the
    /// right one for the requested <see cref="BookingMode"/>.
    /// </summary>
    public sealed class AvailabilityMatcherSelector : IAvailabilityMatcherSelector
    {
        private readonly NewRecipeAvailabilityMatcher _newRecipeMatcher;
        private readonly ExistingBookingAvailabilityMatcher _existingBookingMatcher;

        public AvailabilityMatcherSelector(NewRecipeAvailabilityMatcher newRecipeMatcher, ExistingBookingAvailabilityMatcher existingBookingMatcher)
        {
            _newRecipeMatcher = newRecipeMatcher;
            _existingBookingMatcher = existingBookingMatcher;
        }

        public IAvailabilityMatcher GetMatcher(BookingMode mode)
        {
            return mode switch
            {
                BookingMode.New => _newRecipeMatcher,
                BookingMode.Existing => _existingBookingMatcher,
                _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, "Modalità di prenotazione non supportata."),
            };
        }
    }
}
