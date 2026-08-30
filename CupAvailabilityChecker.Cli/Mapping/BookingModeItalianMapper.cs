using CupAvailabilityChecker.Core.Models;

namespace CupAvailabilityChecker.Cli.Mapping
{
    /// <summary>
    /// Maps the Italian values entered by the user on the command line to the
    /// <see cref="BookingMode"/> enum.
    /// </summary>
    public sealed class BookingModeItalianMapper : IItalianEnumMapper<BookingMode>
    {
        private readonly ItalianEnumMapper<BookingMode> _mapper = new(
            new Dictionary<string, BookingMode>(StringComparer.OrdinalIgnoreCase)
            {
                ["Nuova"] = BookingMode.New,
                ["Esistente"] = BookingMode.Existing,
            },
            "la modalità");

        public IReadOnlyCollection<string> AllowedValues => _mapper.AllowedValues;

        public bool TryParse(string? input, out BookingMode value) => _mapper.TryParse(input, out value);

        public BookingMode Parse(string? input) => _mapper.Parse(input);
    }
}
