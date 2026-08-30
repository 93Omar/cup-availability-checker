using System.CommandLine.Parsing;

namespace CupAvailabilityChecker.Cli.Mapping
{
    /// <summary>
    /// Adapts an <see cref="IItalianEnumMapper{TEnum}"/> to System.CommandLine's
    /// <c>CustomParser</c> mechanism, converting a mapping error into an option parsing error.
    /// </summary>
    public sealed class ItalianEnumOptionParser<TEnum>
        where TEnum : struct, Enum
    {
        private readonly IItalianEnumMapper<TEnum> _mapper;

        public ItalianEnumOptionParser(IItalianEnumMapper<TEnum> mapper)
        {
            _mapper = mapper;
        }

        public TEnum Parse(ArgumentResult result)
        {
            string? raw = result.Tokens.Count > 0 ? result.Tokens[0].Value : null;
            try
            {
                return _mapper.Parse(raw);
            }
            catch (FormatException ex)
            {
                result.AddError(ex.Message);
                return default;
            }
        }
    }
}
