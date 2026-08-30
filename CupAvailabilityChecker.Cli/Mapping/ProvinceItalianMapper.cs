using CupAvailabilityChecker.Core.Models;

namespace CupAvailabilityChecker.Cli.Mapping
{
    /// <summary>
    /// Maps the values entered by the user on the command line to the <see cref="Province"/> enum.
    /// Only the "cumulative" values (all provinces / per macro-area) are translated into Italian;
    /// the codes of individual provinces (e.g. "CA", "SS") remain unchanged.
    /// </summary>
    public sealed class ProvinceItalianMapper : IItalianEnumMapper<Province>
    {
        private readonly ItalianEnumMapper<Province> _mapper = new(
            new Dictionary<string, Province>(StringComparer.OrdinalIgnoreCase)
            {
                ["Tutte"] = Province.All,
                ["TutteNord"] = Province.AllNorth,
                ["TutteCentro"] = Province.AllCenter,
                ["TutteSud"] = Province.AllSouth,
                ["SS"] = Province.SS,
                ["OT"] = Province.OT,
                ["NU"] = Province.NU,
                ["OG"] = Province.OG,
                ["OR"] = Province.OR,
                ["VS"] = Province.VS,
                ["CI"] = Province.CI,
                ["CA"] = Province.CA,
            },
            "la provincia");

        public IReadOnlyCollection<string> AllowedValues => _mapper.AllowedValues;

        public bool TryParse(string? input, out Province value) => _mapper.TryParse(input, out value);

        public Province Parse(string? input) => _mapper.Parse(input);
    }
}
