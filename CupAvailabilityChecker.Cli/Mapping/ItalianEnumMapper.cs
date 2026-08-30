using CupAvailabilityChecker.Cli.Utilities;

namespace CupAvailabilityChecker.Cli.Mapping
{
    /// <summary>
    /// Reusable implementation of <see cref="IItalianEnumMapper{TEnum}"/>, based on a lookup table
    /// from "Italian value" to "enum value". Designed to be composed (not inherited) by the
    /// classes representing the individual mappers.
    /// </summary>
    /// <typeparam name="TEnum">Type of the destination enum.</typeparam>
    public sealed class ItalianEnumMapper<TEnum> : IItalianEnumMapper<TEnum> where TEnum : struct, Enum
    {
        private readonly IReadOnlyDictionary<string, TEnum> _italianToValue;
        private readonly string _parameterName;

        public ItalianEnumMapper(IReadOnlyDictionary<string, TEnum> italianToValue, string parameterName)
        {
            _italianToValue = new Dictionary<string, TEnum>(italianToValue, StringComparer.OrdinalIgnoreCase);
            _parameterName = parameterName;
        }

        public IReadOnlyCollection<string> AllowedValues => _italianToValue.Keys.ToList();

        public bool TryParse(string? input, out TEnum value)
        {
            if (input is not null && _italianToValue.TryGetValue(input.Trim(), out value))
                return true;

            value = default;
            return false;
        }

        public TEnum Parse(string? input)
        {
            if (TryParse(input, out TEnum value))
                return value;

            string allowedValuesText = FormatUtils.JoinValues(AllowedValues);
            string message = $"'{input}' non è un valore valido per {_parameterName}. Valori ammessi: {allowedValuesText}.";
            throw new FormatException(message);
        }
    }
}
