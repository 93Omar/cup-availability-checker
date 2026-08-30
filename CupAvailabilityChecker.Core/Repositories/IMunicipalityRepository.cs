using CupAvailabilityChecker.Core.Models;

namespace CupAvailabilityChecker.Core.Repositories
{
    /// <summary>
    /// Abstraction for retrieving Italian municipalities. Decouples consumers from the actual
    /// data source (e.g. a local JSON file today, an HTTP call or a database in the future).
    /// </summary>
    public interface IMunicipalityRepository
    {
        IReadOnlyCollection<Municipality> GetAll();

        /// <summary>
        /// Checks (case-insensitively) whether a municipality with the given name exists.
        /// </summary>
        bool Exists(string name);

        /// <summary>
        /// Returns the municipalities within <paramref name="radiusKm"/> km from
        /// <paramref name="referenceMunicipalityName"/>. Returns an empty collection if the
        /// reference municipality is not found.
        /// </summary>
        IReadOnlyCollection<Municipality> GetWithinRadius(string referenceMunicipalityName, double radiusKm);
    }
}
