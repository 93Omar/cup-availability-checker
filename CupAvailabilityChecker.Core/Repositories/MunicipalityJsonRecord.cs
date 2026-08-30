using System.Text.Json.Serialization;

namespace CupAvailabilityChecker.Core.Repositories
{
    /// <summary>
    /// Raw shape of a record in <c>Data/gi_comuni.json</c>: an implementation detail of
    /// <see cref="JsonMunicipalityRepository"/>, kept separate so that the public
    /// <see cref="Models.Municipality"/> model stays independent of the JSON source.
    /// </summary>
    internal sealed class MunicipalityJsonRecord
    {
        [JsonPropertyName("sigla_provincia")]
        public string ProvinceCode { get; set; } = null!;

        [JsonPropertyName("codice_istat")]
        public string IstatCode { get; set; } = null!;

        [JsonPropertyName("denominazione_ita")]
        public string Name { get; set; } = null!;

        [JsonPropertyName("flag_capoluogo")]
        public string ProvincialCapitalFlag { get; set; } = null!;

        [JsonPropertyName("codice_belfiore")]
        public string BelfioreCode { get; set; } = null!;

        [JsonPropertyName("lat")]
        public string Lat { get; set; } = null!;

        [JsonPropertyName("lon")]
        public string Lon { get; set; } = null!;

        [JsonPropertyName("superficie_kmq")]
        public string AreaKmq { get; set; } = null!;
    }
}
