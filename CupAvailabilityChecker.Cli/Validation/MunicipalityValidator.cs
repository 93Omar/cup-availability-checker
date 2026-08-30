using CupAvailabilityChecker.Cli.Parameters;
using CupAvailabilityChecker.Core.Repositories;
using FluentResults;

namespace CupAvailabilityChecker.Cli.Validation
{
    /// <summary>
    /// Validates that the value of <c>--comune</c>, if provided, refers to an existing
    /// municipality (case-insensitive check via <see cref="IMunicipalityRepository"/>). An empty
    /// value is considered valid, since the parameter is optional.
    /// </summary>
    public sealed class MunicipalityValidator : IParameterValidator<string>
    {
        private readonly IMunicipalityRepository _municipalityRepository;

        public MunicipalityValidator(IMunicipalityRepository municipalityRepository)
        {
            _municipalityRepository = municipalityRepository;
        }

        public Result Validate(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return Result.Ok();

            string normalized = value.Trim();
            bool exists = _municipalityRepository.Exists(normalized);

            if (!exists)
            {
                string message = $"'{value}' non è un comune valido.";
                return Result.Fail(message);
            }

            return Result.Ok();
        }
    }
}
