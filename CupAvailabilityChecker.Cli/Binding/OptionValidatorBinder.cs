using System.CommandLine;
using System.CommandLine.Parsing;
using System.Diagnostics.CodeAnalysis;
using CupAvailabilityChecker.Cli.Parameters;
using CupAvailabilityChecker.Cli.Utilities;
using FluentResults;

namespace CupAvailabilityChecker.Cli.Binding
{
    /// <summary>
    /// Centralizes wiring the parameter validators (single or dependent on another parameter)
    /// to the options of a System.CommandLine <see cref="RootCommand"/>.
    /// </summary>
    public sealed class OptionValidatorBinder
    {
        private readonly RootCommand _rootCommand;

        public OptionValidatorBinder(RootCommand rootCommand)
        {
            _rootCommand = rootCommand;
        }

        /// <summary>
        /// Links an <see cref="IParameterValidator{T}"/> to the value of an option, regardless of its type.
        /// </summary>
        public void AddValidator<T>(Option<T> option, IParameterValidator<T> validator)
        {
            _rootCommand.Validators.Add(commandResult =>
            {
                if (!TryGetValue(commandResult, option, out var value))
                    return;

                Result validationResult = validator.Validate(value);
                AddErrorsIfFailed(commandResult, validationResult);
            });
        }

        /// <summary>
        /// Links an <see cref="IDependentParameterValidator{TValue,TDependency}"/> to two options,
        /// to validate a parameter whose correctness depends on the value of another one.
        /// </summary>
        public void AddDependentValidator<TValue, TDependency>(Option<TValue> valueOption, Option<TDependency> dependencyOption,
            IDependentParameterValidator<TValue, TDependency> validator)
        {
            _rootCommand.Validators.Add(commandResult =>
            {
                if (!TryGetValue(commandResult, valueOption, out var value)
                    || !TryGetValue(commandResult, dependencyOption, out var dependency))
                    return;

                Result validationResult = validator.Validate(value, dependency);
                AddErrorsIfFailed(commandResult, validationResult);
            });
        }

        /// <summary>
        /// Reads the value of an option already resolved by the parser. If the option's parsing has
        /// already failed (e.g. a CustomParser reported an error), avoids throwing an exception
        /// and generating a further, misleading validation error.
        /// </summary>
        private static bool TryGetValue<T>(CommandResult commandResult, Option<T> option, [MaybeNullWhen(false)] out T value)
        {
            try
            {
                value = commandResult.GetValue(option)!;
                return true;
            }
            catch (InvalidOperationException)
            {
                value = default;
                return false;
            }
        }

        /// <summary>
        /// Reports any errors from a FluentResults <see cref="Result"/> onto the System.CommandLine result.
        /// </summary>
        private static void AddErrorsIfFailed(SymbolResult result, Result validation)
        {
            if (validation.IsFailed)
            {
                IEnumerable<string> errorMessages = validation.Errors.Select(e => e.Message);
                string errorMessage = FormatUtils.JoinValues(errorMessages, "; ");
                result.AddError(errorMessage);
            }
        }
    }
}
