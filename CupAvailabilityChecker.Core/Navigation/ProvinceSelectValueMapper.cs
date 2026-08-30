using CupAvailabilityChecker.Core.Models;

namespace CupAvailabilityChecker.Core.Navigation
{
    /// <summary>
    /// Maps a <see cref="Province"/> to the <c>value</c> attribute of the corresponding
    /// <c>&lt;option&gt;</c> in the availability page's "zona" filter select. A <c>null</c>
    /// result means the "all" option for the currently selected macrozona (e.g. "Tutta la Regione
    /// Sardegna", "Tutto il Nord Sardegna", ...), which has no <c>value</c> attribute (it is
    /// always the first option in the select).
    /// </summary>
    public sealed class ProvinceSelectValueMapper
    {
        public string? GetSelectValue(Province province)
        {
            return province switch
            {
                Province.All or Province.AllNorth or Province.AllCenter or Province.AllSouth => null,
                Province.SS => "ZN_SS",
                Province.OT => "ZN_OT",
                Province.NU => "ZN_NU",
                Province.OG => "ZN_OG",
                Province.OR => "ZN_OR",
                Province.VS => "ZN_VS",
                Province.CI => "ZN_CI",
                Province.CA => "ZN_CA",
                _ => throw new ArgumentOutOfRangeException(nameof(province), province, "Provincia non supportata."),
            };
        }
    }
}
