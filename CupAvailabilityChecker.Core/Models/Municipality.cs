namespace CupAvailabilityChecker.Core.Models
{
    /// <summary>
    /// Represents an Italian municipality (comune), with the demographic and geographic data
    /// needed to validate CLI parameters and to perform radius-based searches.
    /// </summary>
    public sealed record Municipality(
        string Name,
        string ProvinceCode,
        string IstatCode,
        string BelfioreCode,
        double Lat,
        double Lon,
        double AreaKmq,
        bool IsProvincialCapital);
}
