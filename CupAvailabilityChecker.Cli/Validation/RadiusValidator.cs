using CupAvailabilityChecker.Cli.Parameters;
using FluentResults;

namespace CupAvailabilityChecker.Cli.Validation
{
    /// <summary>
    /// Validates the value of <c>--raggio</c>, if provided: must be a positive number of km.
    /// A missing value (<c>null</c>) is considered valid, since the parameter is optional.
    /// </summary>
    public sealed class RadiusValidator : IParameterValidator<double?>
    {
        public Result Validate(double? value)
        {
            if (value is null)
                return Result.Ok();

            if (value <= 0)
            {
                string message = $"Il raggio di ricerca deve essere maggiore di zero (valore fornito: {value}).";
                return Result.Fail(message);
            }

            return Result.Ok();
        }
    }
}
