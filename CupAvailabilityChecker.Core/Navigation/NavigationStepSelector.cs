using CupAvailabilityChecker.Core.Models;

namespace CupAvailabilityChecker.Core.Navigation
{
    /// <summary>
    /// Composes the two concrete <see cref="INavigationStep"/> implementations and picks the
    /// right one for the requested <see cref="BookingMode"/>.
    /// </summary>
    public sealed class NavigationStepSelector : INavigationStepSelector
    {
        private readonly NewRecipeNavigationStep _newRecipeStep;
        private readonly ExistingBookingNavigationStep _existingBookingStep;

        public NavigationStepSelector(NewRecipeNavigationStep newRecipeStep, ExistingBookingNavigationStep existingBookingStep)
        {
            _newRecipeStep = newRecipeStep;
            _existingBookingStep = existingBookingStep;
        }

        public INavigationStep GetStep(BookingMode mode)
        {
            return mode switch
            {
                BookingMode.New => _newRecipeStep,
                BookingMode.Existing => _existingBookingStep,
                _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, "Modalità di prenotazione non supportata."),
            };
        }
    }
}
