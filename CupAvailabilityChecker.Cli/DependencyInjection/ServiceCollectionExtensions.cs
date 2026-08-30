using CupAvailabilityChecker.Cli.Commands;
using CupAvailabilityChecker.Cli.Mapping;
using CupAvailabilityChecker.Cli.Validation;
using CupAvailabilityChecker.Core;
using CupAvailabilityChecker.Core.Availability;
using CupAvailabilityChecker.Core.Browser;
using CupAvailabilityChecker.Core.Models;
using CupAvailabilityChecker.Core.Navigation;
using CupAvailabilityChecker.Core.Notifications;
using CupAvailabilityChecker.Core.Polling;
using CupAvailabilityChecker.Core.Repositories;
using CupAvailabilityChecker.Core.Services;
using CupAvailabilityChecker.Core.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CupAvailabilityChecker.Cli.DependencyInjection
{
    /// <summary>
    /// CLI composition root: registers the services (retriever, mappers, validators, parsers,
    /// command builder) so that each class declares its dependencies in its constructor
    /// instead of constructing them directly with <c>new</c>.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCliServices(this IServiceCollection services)
        {
            services.AddLogging(builder => builder.AddConsole());
            services.AddScoped<ProvinceFilterRetriever>();
            services.AddScoped<IMunicipalityRepository, JsonMunicipalityRepository>();
            services.AddScoped<IItalianEnumMapper<Area>, AreaItalianMapper>();
            services.AddScoped<IItalianEnumMapper<Province>, ProvinceItalianMapper>();
            services.AddScoped<IItalianEnumMapper<BookingMode>, BookingModeItalianMapper>();
            services.AddScoped<ItalianEnumOptionParser<Area>>();
            services.AddScoped<ItalianEnumOptionParser<Province>>();
            services.AddScoped<ItalianEnumOptionParser<BookingMode>>();
            services.AddScoped<RadiusOptionParser>();
            services.AddScoped<FiscalCodeValidator>();
            services.AddScoped<ProvinceAreaValidator>();
            services.AddScoped<MunicipalityValidator>();
            services.AddScoped<MunicipalitiesValidator>();
            services.AddScoped<RadiusValidator>();
            services.AddScoped<RootCommandBuilder>();

            services.AddScoped<IWebDriverFactory, SeleniumWebDriverFactory>();
            services.AddScoped<SeleniumWaitHelper>();
            services.AddScoped<SeleniumClickHelper>();
            services.AddScoped<CookieBannerDismisser>();
            services.AddScoped<CupLoginHelper>();
            services.AddScoped<AreaSelectValueMapper>();
            services.AddScoped<ProvinceSelectValueMapper>();
            services.AddScoped<NewRecipeNavigationStep>();
            services.AddScoped<ExistingBookingNavigationStep>();
            services.AddScoped<INavigationStepSelector, NavigationStepSelector>();
            services.AddScoped<NewRecipeAvailabilityReader>();
            services.AddScoped<ExistingBookingAvailabilityReader>();
            services.AddScoped<IAvailabilityReaderSelector, AvailabilityReaderSelector>();
            services.AddScoped<NewRecipeAvailabilityMatcher>();
            services.AddScoped<ExistingBookingAvailabilityMatcher>();
            services.AddScoped<IAvailabilityMatcherSelector, AvailabilityMatcherSelector>();
            services.AddScoped<NewRecipeAvailabilityRefresher>();
            services.AddScoped<ExistingBookingAvailabilityRefresher>();
            services.AddScoped<IAvailabilityRefresherSelector, AvailabilityRefresherSelector>();
            services.AddScoped<ISessionExpiryDetector, SessionExpiryDetector>();
            services.AddScoped<INotificationSender, ConsoleNotificationSender>();
            services.AddScoped<AvailabilityPoller>();
            services.AddScoped<BookingCheckOrchestrator>();

            return services;
        }
    }
}
