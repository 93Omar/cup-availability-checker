namespace CupAvailabilityChecker.Cli.Mapping
{
    /// <summary>
    /// Maps the Italian values entered by the user on the command line to an enum.
    /// </summary>
    /// <typeparam name="TEnum">Type of the destination enum.</typeparam>
    public interface IItalianEnumMapper<TEnum> where TEnum : struct, Enum
    {
        IReadOnlyCollection<string> AllowedValues { get; }

        bool TryParse(string? input, out TEnum value);

        TEnum Parse(string? input);
    }
}
