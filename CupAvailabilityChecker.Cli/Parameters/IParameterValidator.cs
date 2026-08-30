using FluentResults;

namespace CupAvailabilityChecker.Cli.Parameters
{
    /// <summary>
    /// Abstraction for validating a parameter's value, independent of other parameters.
    /// </summary>
    /// <typeparam name="T">Type of the parameter's value to validate.</typeparam>
    public interface IParameterValidator<in T>
    {
        Result Validate(T value);
    }
}
