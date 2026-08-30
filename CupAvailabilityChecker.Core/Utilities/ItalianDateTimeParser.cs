using System.Globalization;

namespace CupAvailabilityChecker.Core.Utilities
{
    /// <summary>
    /// Parses the Italian date/time strings shown by CupWeb (e.g. "Giovedì 3 Settembre 2026",
    /// "08:00"), shared between the navigation steps (reading the current booking's date/time)
    /// and the availability readers (reading each candidate slot's date/time), to avoid
    /// duplicating the parsing logic. These strings represent Italian local time, not UTC.
    /// </summary>
    public static class ItalianDateTimeParser
    {
        private static readonly CultureInfo ItalianCulture = CultureInfo.GetCultureInfo("it-IT");

        public static DateOnly ParseDate(string text)
        {
            DateTime parsed = DateTime.ParseExact(text.Trim(), "dddd d MMMM yyyy", ItalianCulture);
            return DateOnly.FromDateTime(parsed);
        }

        public static TimeOnly ParseTime(string text)
        {
            DateTime parsed = DateTime.ParseExact(text.Trim(), "HH:mm", ItalianCulture);
            return TimeOnly.FromDateTime(parsed);
        }
    }
}
