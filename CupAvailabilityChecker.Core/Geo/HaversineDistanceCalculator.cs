namespace CupAvailabilityChecker.Core.Geo
{
    /// <summary>
    /// Computes the great-circle distance between two geographic points using the
    /// Haversine formula. Kept as a standalone, composable class (not inherited) so that any
    /// repository or service needing distance calculations can reuse it without depending on
    /// coordinate-parsing or data-source concerns.
    /// </summary>
    public sealed class HaversineDistanceCalculator
    {
        // Mean radius of the Earth in kilometers, as used by the Haversine formula.
        private const double EarthRadiusKm = 6371.0;

        /// <summary>
        /// Calculates the distance in kilometers between two points identified by their
        /// latitude/longitude, expressed in decimal degrees.
        /// </summary>
        public double CalculateDistanceKm(double fromLat, double fromLon, double toLat, double toLon)
        {
            // The Haversine formula computes the great-circle distance between two points on a
            // sphere from their latitude/longitude: it first converts the angular difference
            // between the two points into radians, then derives the central angle between them
            // (via the haversine of that angle), and finally scales it by the Earth's radius.
            double fromLatRad = DegreesToRadians(fromLat);
            double toLatRad = DegreesToRadians(toLat);
            double deltaLatRad = DegreesToRadians(toLat - fromLat);
            double deltaLonRad = DegreesToRadians(toLon - fromLon);

            // Haversine of the central angle: combines the latitude difference with the
            // longitude difference (the latter weighted by the cosine of both latitudes, to
            // account for the convergence of meridians near the poles).
            double haversineOfCentralAngle = (Math.Sin(deltaLatRad / 2) * Math.Sin(deltaLatRad / 2))
                + (Math.Cos(fromLatRad) * Math.Cos(toLatRad) * Math.Sin(deltaLonRad / 2) * Math.Sin(deltaLonRad / 2));

            // Converts the haversine value back into the actual central angle (in radians),
            // using atan2 for numerical stability across the full range of inputs.
            double centralAngleRad = 2 * Math.Atan2(Math.Sqrt(haversineOfCentralAngle), Math.Sqrt(1 - haversineOfCentralAngle));

            return EarthRadiusKm * centralAngleRad;
        }

        private static double DegreesToRadians(double degrees) => degrees * Math.PI / 180.0;
    }
}
