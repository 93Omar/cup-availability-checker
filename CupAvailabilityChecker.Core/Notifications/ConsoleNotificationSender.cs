using CupAvailabilityChecker.Core.Models;
using Microsoft.Extensions.Logging;

namespace CupAvailabilityChecker.Core.Notifications
{
    /// <summary>
    /// Placeholder <see cref="INotificationSender"/> that logs the found availability to the
    /// console, until a real notification channel is decided.
    /// </summary>
    public sealed class ConsoleNotificationSender : INotificationSender
    {
        private readonly ILogger<ConsoleNotificationSender> _logger;

        public ConsoleNotificationSender(ILogger<ConsoleNotificationSender> logger)
        {
            _logger = logger;
        }

        public Task NotifyAsync(AvailabilitySlot slot, BookingParameters parameters, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Disponibilità trovata: Data={Data}, Ora={Ora}, Sede={Sede}",
                slot.Date, slot.Time, slot.Location);

            return Task.CompletedTask;
        }
    }
}
