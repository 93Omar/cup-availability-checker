namespace CupAvailabilityChecker.Cli.Utilities
{
    /// <summary>
    /// Shared formatting utilities, to avoid duplicating rendering logic
    /// (e.g. lists of allowed values, error messages) across the CLI.
    /// </summary>
    public static class FormatUtils
    {
        public static string JoinValues<T>(IEnumerable<T> values, string separator = ", ")
            => string.Join(separator, values);
    }
}
