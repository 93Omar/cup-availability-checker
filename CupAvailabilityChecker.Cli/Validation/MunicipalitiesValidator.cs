using CupAvailabilityChecker.Cli.Parameters;
using CupAvailabilityChecker.Cli.Utilities;
using CupAvailabilityChecker.Core.Repositories;
using FluentResults;

namespace CupAvailabilityChecker.Cli.Validation
{
    /// <summary>
    /// Validates that every value of <c>--comuni</c>, if provided, refers to an existing
    /// municipality (case-insensitive check via <see cref="IMunicipalityRepository"/>). An
    /// empty/missing list is considered valid, since the parameter is optional.
    /// </summary>
    public sealed class MunicipalitiesValidator : IParameterValidator<string[]>
    {
        private readonly IMunicipalityRepository _municipalityRepository;

        public MunicipalitiesValidator(IMunicipalityRepository municipalityRepository)
        {
            _municipalityRepository = municipalityRepository;
        }

        public Result Validate(string[] value)
        {
            if (value is null || value.Length == 0)
                return Result.Ok();

            IEnumerable<string> invalidMunicipalities = value.Where(municipality => !_municipalityRepository.Exists(municipality));
            List<string> invalidMunicipalitiesList = invalidMunicipalities.ToList();

            if (invalidMunicipalitiesList.Count > 0)
            {
                string invalidMunicipalitiesText = FormatUtils.JoinValues(invalidMunicipalitiesList);
                string message = $"I seguenti comuni non sono validi: {invalidMunicipalitiesText}.";
                return Result.Fail(message);
            }

            return Result.Ok();
        }
    }
}
