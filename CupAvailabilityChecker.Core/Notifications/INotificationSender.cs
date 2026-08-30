using CupAvailabilityChecker.Core.Models;

namespace CupAvailabilityChecker.Core.Notifications
{
    /// <summary>
    /// Step 3's output: notifies the user that an interesting availability slot has been found.
    /// The concrete notification channel (console, email, push, ...) is still to be defined; this
    /// interface keeps the polling loop decoupled from it.
    /// </summary>
    public interface INotificationSender
    {
        Task NotifyAsync(AvailabilitySlot slot, BookingParameters parameters, CancellationToken cancellationToken);
    }
}
