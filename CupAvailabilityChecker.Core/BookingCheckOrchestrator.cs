using CupAvailabilityChecker.Core.Browser;
using CupAvailabilityChecker.Core.Models;
using CupAvailabilityChecker.Core.Navigation;
using CupAvailabilityChecker.Core.Polling;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;

namespace CupAvailabilityChecker.Core
{
    /// <summary>
    /// Orchestrates the full flow for a single run: creates the browser, navigates to the
    /// availability page (step 1), then polls it until an interesting slot is found (steps 2-3-4).
    /// If the session expires during polling, restarts from step 1 (re-navigation/login), until
    /// either a match is found or <paramref name="cancellationToken"/> is cancelled.
    /// </summary>
    public sealed class BookingCheckOrchestrator
    {
        private readonly IWebDriverFactory _webDriverFactory;
        private readonly INavigationStepSelector _navigationStepSelector;
        private readonly AvailabilityPoller _poller;
        private readonly ILogger<BookingCheckOrchestrator> _logger;

        public BookingCheckOrchestrator(
            IWebDriverFactory webDriverFactory,
            INavigationStepSelector navigationStepSelector,
            AvailabilityPoller poller,
            ILogger<BookingCheckOrchestrator> logger)
        {
            _webDriverFactory = webDriverFactory;
            _navigationStepSelector = navigationStepSelector;
            _poller = poller;
            _logger = logger;
        }

        public async Task RunAsync(BookingParameters parameters, CancellationToken cancellationToken)
        {
            using IWebDriver driver = _webDriverFactory.Create(parameters.Browser, parameters.Headless);
            AvailabilityCheckContext context = new AvailabilityCheckContext(parameters);
            INavigationStep navigationStep = _navigationStepSelector.GetStep(parameters.Mode);

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    navigationStep.NavigateToAvailabilityPage(driver, context);
                    await _poller.PollUntilMatchAsync(driver, context, cancellationToken);
                    return;
                }
                catch (SessionExpiredException)
                {
                    _logger.LogWarning("Sessione scaduta: ripeto la navigazione dal punto 1.");
                }
            }
        }
    }
}
