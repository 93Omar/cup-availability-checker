using System.Globalization;
using System.Text.Json;
using CupAvailabilityChecker.Core.Geo;
using CupAvailabilityChecker.Core.Models;

namespace CupAvailabilityChecker.Core.Repositories
{
    /// <summary>
    /// <see cref="IMunicipalityRepository"/> implementation backed by the local JSON dataset
    /// in <c>Data/gi_comuni.json</c>. Loads and parses the file lazily, once, on first use, and
    /// composes a <see cref="HaversineDistanceCalculator"/> for radius-based lookups.
    /// </summary>
    public sealed class JsonMunicipalityRepository : IMunicipalityRepository
    {
        private static readonly string DataFilePath = Path.Combine(AppContext.BaseDirectory, "Data", "gi_comuni.json");

        private readonly HaversineDistanceCalculator _distanceCalculator = new();
        private readonly Lazy<IReadOnlyList<Municipality>> _municipalities = new(LoadFromFile);

        public IReadOnlyCollection<Municipality> GetAll() => _municipalities.Value;

        public bool Exists(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            string normalizedName = name.Trim();
            Municipality? foundMunicipality = FindByName(normalizedName);

            return foundMunicipality is not null;
        }

        public IReadOnlyCollection<Municipality> GetWithinRadius(string referenceMunicipalityName, double radiusKm)
        {
            string normalizedName = referenceMunicipalityName.Trim();
            Municipality? referenceMunicipality = FindByName(normalizedName);

            if (referenceMunicipality is null)
                return [];

            List<Municipality> municipalitiesWithinRadius = _municipalities.Value
                .Where(municipality => IsWithinRadius(referenceMunicipality, municipality, radiusKm))
                .ToList();

            return municipalitiesWithinRadius;
        }

        private Municipality? FindByName(string normalizedName)
            => _municipalities.Value.FirstOrDefault(municipality =>
                string.Equals(municipality.Name, normalizedName, StringComparison.OrdinalIgnoreCase));

        private bool IsWithinRadius(Municipality from, Municipality to, double radiusKm)
        {
            double distanceKm = _distanceCalculator.CalculateDistanceKm(from.Lat, from.Lon, to.Lat, to.Lon);
            return distanceKm <= radiusKm;
        }

        private static IReadOnlyList<Municipality> LoadFromFile()
        {
            string json = File.ReadAllText(DataFilePath);
            List<MunicipalityJsonRecord>? records = JsonSerializer.Deserialize<List<MunicipalityJsonRecord>>(json);

            if (records is null)
                return [];

            List<Municipality> municipalities = records.Select(MapToMunicipality).ToList();
            return municipalities;
        }

        private static Municipality MapToMunicipality(MunicipalityJsonRecord record)
        {
            double lat = double.Parse(record.Lat, NumberStyles.Float, CultureInfo.InvariantCulture);
            double lon = double.Parse(record.Lon, NumberStyles.Float, CultureInfo.InvariantCulture);
            double areaKmq = double.Parse(record.AreaKmq, NumberStyles.Float, CultureInfo.InvariantCulture);
            bool isProvincialCapital = string.Equals(record.ProvincialCapitalFlag, "SI", StringComparison.OrdinalIgnoreCase);

            return new Municipality(
                record.Name,
                record.ProvinceCode,
                record.IstatCode,
                record.BelfioreCode,
                lat,
                lon,
                areaKmq,
                isProvincialCapital);
        }
    }
}
