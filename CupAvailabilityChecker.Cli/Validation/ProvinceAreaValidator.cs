using CupAvailabilityChecker.Cli.Parameters;
using CupAvailabilityChecker.Cli.Utilities;
using CupAvailabilityChecker.Core.Models;
using CupAvailabilityChecker.Core.Services;
using FluentResults;

namespace CupAvailabilityChecker.Cli.Validation
{
    /// <summary>
    /// Validates that a province is allowed for the selected geographic area,
    /// reusing the rules already defined in <see cref="ProvinceFilterRetriever"/>.
    /// </summary>
    public sealed class ProvinceAreaValidator : IDependentParameterValidator<Province, Area>
    {
        private readonly ProvinceFilterRetriever _provinceFilterRetriever;

        public ProvinceAreaValidator(ProvinceFilterRetriever provinceFilterRetriever)
        {
            _provinceFilterRetriever = provinceFilterRetriever;
        }

        public Result Validate(Province value, Area dependency)
        {
            IList<Province> allowedProvinces = _provinceFilterRetriever.GetProvincesByArea(dependency);

            if (!allowedProvinces.Contains(value))
            {
                string allowedProvincesText = FormatUtils.JoinValues(allowedProvinces);
                string message = $"La provincia '{value}' non è ammessa per l'area '{dependency}'. Valori ammessi: {allowedProvincesText}.";

                return Result.Fail(message);
            }

            return Result.Ok();
        }
    }
}
