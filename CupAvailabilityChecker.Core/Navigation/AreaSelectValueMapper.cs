using CupAvailabilityChecker.Core.Models;

namespace CupAvailabilityChecker.Core.Navigation
{
    /// <summary>
    /// Maps an <see cref="Area"/> to the <c>value</c> attribute of the corresponding
    /// <c>&lt;option&gt;</c> in the availability page's "macrozona" filter select. A <c>null</c>
    /// result means the "all" option, which has no <c>value</c> attribute (it is always the first
    /// option in the select).
    /// </summary>
    public sealed class AreaSelectValueMapper
    {
        public string? GetSelectValue(Area area)
        {
            return area switch
            {
                Area.All => null,
                Area.North => "MZ_NORD",
                Area.Center => "MZ_CENT",
                Area.South => "MZ_SUD",
                _ => throw new ArgumentOutOfRangeException(nameof(area), area, "Area non supportata."),
            };
        }
    }
}
