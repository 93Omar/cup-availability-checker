using System.CommandLine.Parsing;
using System.Globalization;

namespace CupAvailabilityChecker.Cli.Mapping
{
    /// <summary>
    /// Parses the raw token of the <c>--raggio</c> option into a <see cref="double"/>, using
    /// invariant culture for the decimal separator: this prevents "15.5" from being interpreted
    /// as "155" under an it-IT culture (dot as thousands separator). A missing token results in
    /// <c>null</c>, since the option is optional.
    /// </summary>
    public sealed class RadiusOptionParser
    {
        public double? Parse(ArgumentResult result)
        {
            if (result.Tokens.Count == 0)
                return null;

            string rawValue = result.Tokens[0].Value;
            bool isValid = double.TryParse(rawValue, NumberStyles.Float, CultureInfo.InvariantCulture, out double parsedValue);

            if (!isValid)
            {
                result.AddError($"'{rawValue}' non è un valore numerico valido per --raggio.");
                return null;
            }

            return parsedValue;
        }
    }
}
