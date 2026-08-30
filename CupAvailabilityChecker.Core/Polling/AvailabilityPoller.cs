using CupAvailabilityChecker.Core.Availability;
using CupAvailabilityChecker.Core.Models;
using CupAvailabilityChecker.Core.Notifications;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;

namespace CupAvailabilityChecker.Core.Polling
{
    /// <summary>
    /// Steps 2-3-4 of the flow: periodically refreshes the availability page (at
    /// <see cref="BookingParameters.RefreshInterval"/>), reads the current availability, checks it
    /// against the matching criteria and, if an interesting slot is found, sends a notification
    /// and returns. If the session is detected as expired, throws <see cref="SessionExpiredException"/>
    /// so that the orchestrator can restart the flow from step 1.
    /// </summary>
    public sealed class AvailabilityPoller
    {
        private readonly IAvailabilityReaderSelector _readerSelector;
        private readonly IAvailabilityMatcherSelector _matcherSelector;
        private readonly IAvailabilityRefresherSelector _refresherSelector;
        private readonly ISessionExpiryDetector _sessionExpiryDetector;
        private readonly INotificationSender _notificationSender;
        private readonly ILogger<AvailabilityPoller> _logger;

        public AvailabilityPoller(
            IAvailabilityReaderSelector readerSelector,
            IAvailabilityMatcherSelector matcherSelector,
            IAvailabilityRefresherSelector refresherSelector,
            ISessionExpiryDetector sessionExpiryDetector,
            INotificationSender notificationSender,
            ILogger<AvailabilityPoller> logger)
        {
            _readerSelector = readerSelector;
            _matcherSelector = matcherSelector;
            _refresherSelector = refresherSelector;
            _sessionExpiryDetector = sessionExpiryDetector;
            _notificationSender = notificationSender;
            _logger = logger;
        }

        public async Task PollUntilMatchAsync(IWebDriver driver, AvailabilityCheckContext context, CancellationToken cancellationToken)
        {
            IAvailabilityReader reader = _readerSelector.GetReader(context.Parameters.Mode);
            IAvailabilityMatcher matcher = _matcherSelector.GetMatcher(context.Parameters.Mode);
            IAvailabilityRefresher refresher = _refresherSelector.GetRefresher(context.Parameters.Mode);

            while (!cancellationToken.IsCancellationRequested)
            {
                if (_sessionExpiryDetector.IsSessionExpired(driver))
                    throw new SessionExpiredException();

                IReadOnlyList<AvailabilitySlot> slots = reader.ReadCurrentAvailability(driver);
                AvailabilitySlot? match = slots.FirstOrDefault(slot => matcher.IsInteresting(slot, context));

                if (match is not null)
                {
                    _logger.LogInformation("Trovata disponibilità interessante: {Slot}", match);
                    await _notificationSender.NotifyAsync(match, context.Parameters, cancellationToken);
                    return;
                }

                await Task.Delay(context.Parameters.RefreshInterval, cancellationToken);
                refresher.Refresh(driver);
            }
        }
    }
}
