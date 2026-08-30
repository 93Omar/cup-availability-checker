using CupAvailabilityChecker.Cli.Commands;
using CupAvailabilityChecker.Cli.Mapping;
using CupAvailabilityChecker.Cli.Validation;
using CupAvailabilityChecker.Core.Models;
using CupAvailabilityChecker.Core.Services;
using Microsoft.Extensions.DependencyInjection;

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
            services.AddScoped<ProvinceFilterRetriever>();
            services.AddScoped<IItalianEnumMapper<Area>, AreaItalianMapper>();
            services.AddScoped<IItalianEnumMapper<Province>, ProvinceItalianMapper>();
            services.AddScoped<ItalianEnumOptionParser<Area>>();
            services.AddScoped<ItalianEnumOptionParser<Province>>();
            services.AddScoped<CodiceFiscaleValidator>();
            services.AddScoped<ProvinciaAreaValidator>();
            services.AddScoped<RootCommandBuilder>();

            return services;
        }
    }
}
