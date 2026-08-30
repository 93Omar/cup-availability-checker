using CupAvailabilityChecker.Core.Models;

namespace CupAvailabilityChecker.Cli.Mapping
{
    /// <summary>
    /// Maps the Italian values entered by the user on the command line to the <see cref="Area"/> enum.
    /// </summary>
    public sealed class AreaItalianMapper : IItalianEnumMapper<Area>
    {
        private readonly ItalianEnumMapper<Area> _mapper = new(
            new Dictionary<string, Area>(StringComparer.OrdinalIgnoreCase)
            {
                ["Tutte"] = Area.All,
                ["Nord"] = Area.North,
                ["Centro"] = Area.Center,
                ["Sud"] = Area.South,
            },
            "l'area");

        public IReadOnlyCollection<string> AllowedValues => _mapper.AllowedValues;

        public bool TryParse(string? input, out Area value) => _mapper.TryParse(input, out value);

        public Area Parse(string? input) => _mapper.Parse(input);
    }
}
