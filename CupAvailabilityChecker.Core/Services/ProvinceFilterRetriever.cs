using CupAvailabilityChecker.Core.Models;

namespace CupAvailabilityChecker.Core.Services
{
    public class ProvinceFilterRetriever
    {
        public IList<Province> GetProvincesByArea(Area area)
        {
            switch (area)
            {
                case Area.All:
                    return [Province.All];
                case Area.North:
                    return
                    [
                        Province.AllNorth,
                        Province.SS,
                        Province.OT
                    ];
                case Area.Center:
                    return
                    [
                        Province.AllCenter,
                        Province.NU,
                        Province.OG,
                        Province.OR
                    ];
                case Area.South:
                    return
                    [
                        Province.AllSouth,
                        Province.VS,
                        Province.CI,
                        Province.CA
                    ];
                default:
                    return [];
            }
        }
    }
}
