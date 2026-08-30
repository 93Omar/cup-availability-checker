using System.Text.RegularExpressions;
using CupAvailabilityChecker.Cli.Parameters;
using FluentResults;

namespace CupAvailabilityChecker.Cli.Validation
{
    /// <summary>
    /// Validates that a string is a formally correct Italian codice fiscale
    /// (16 alphanumeric characters in the standard format: 6 letters, 2 digits, 1 letter,
    /// 2 digits, 1 letter, 3 digits, 1 letter). The check digit is not validated.
    /// </summary>
    public sealed class CodiceFiscaleValidator : IParameterValidator<string>
    {
        private static readonly Regex Pattern = new(
            "^[A-Za-z]{6}[0-9]{2}[A-Za-z][0-9]{2}[A-Za-z][0-9]{3}[A-Za-z]$",
            RegexOptions.Compiled);

        public Result Validate(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return Result.Fail("Il codice fiscale non può essere vuoto.");

            string normalized = value.Trim();

            if (normalized.Length != 16)
            {
                string message = $"Il codice fiscale deve essere composto da 16 caratteri (trovati {normalized.Length}).";
                return Result.Fail(message);
            }

            if (!Pattern.IsMatch(normalized))
            {
                string message = $"'{value}' non è un codice fiscale valido.";
                return Result.Fail(message);
            }

            return Result.Ok();
        }
    }
}
