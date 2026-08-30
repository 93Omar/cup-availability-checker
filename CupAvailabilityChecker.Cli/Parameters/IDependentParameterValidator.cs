using FluentResults;

namespace CupAvailabilityChecker.Cli.Parameters
{
    /// <summary>
    /// Abstraction for validating a parameter's value whose correctness depends
    /// on the value of another parameter (e.g. "provincia" depends on "area").
    /// </summary>
    /// <typeparam name="TValue">Type of the parameter's value to validate.</typeparam>
    /// <typeparam name="TDependency">Type of the parameter's value the validation depends on.</typeparam>
    public interface IDependentParameterValidator<in TValue, in TDependency>
    {
        Result Validate(TValue value, TDependency dependency);
    }
}
