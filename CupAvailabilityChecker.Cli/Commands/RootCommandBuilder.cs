using System.CommandLine;
using CupAvailabilityChecker.Cli.Binding;
using CupAvailabilityChecker.Cli.Mapping;
using CupAvailabilityChecker.Cli.Utilities;
using CupAvailabilityChecker.Cli.Validation;
using CupAvailabilityChecker.Core.Models;

namespace CupAvailabilityChecker.Cli.Commands
{
    /// <summary>
    /// Assembles the CLI's <see cref="RootCommand"/> (options, parsers and validators), receiving
    /// all its dependencies already resolved via constructor injection, so that the caller
    /// (<c>Program.cs</c>) only needs to resolve a single service from the container.
    /// </summary>
    public sealed class RootCommandBuilder
    {
        private readonly IItalianEnumMapper<Area> _areaMapper;
        private readonly IItalianEnumMapper<Province> _provinceMapper;
        private readonly ItalianEnumOptionParser<Area> _areaParser;
        private readonly ItalianEnumOptionParser<Province> _provinciaParser;
        private readonly CodiceFiscaleValidator _codiceFiscaleValidator;
        private readonly ProvinciaAreaValidator _provinciaAreaValidator;

        public RootCommandBuilder(
            IItalianEnumMapper<Area> areaMapper,
            IItalianEnumMapper<Province> provinceMapper,
            ItalianEnumOptionParser<Area> areaParser,
            ItalianEnumOptionParser<Province> provinciaParser,
            CodiceFiscaleValidator codiceFiscaleValidator,
            ProvinciaAreaValidator provinciaAreaValidator)
        {
            _areaMapper = areaMapper;
            _provinceMapper = provinceMapper;
            _areaParser = areaParser;
            _provinciaParser = provinciaParser;
            _codiceFiscaleValidator = codiceFiscaleValidator;
            _provinciaAreaValidator = provinciaAreaValidator;
        }

        public RootCommand Build()
        {
            var codiceFiscaleOption = new Option<string>("--codice-fiscale", "-cf")
            {
                Description = "Codice fiscale dell'intestatario della ricetta elettronica.",
                Required = true,
            };

            var nreOption = new Option<string>("--nre", "-n")
            {
                Description = "Numero di ricetta elettronica.",
                Required = true,
            };

            string areaAllowedValuesText = FormatUtils.JoinValues(_areaMapper.AllowedValues);
            var areaOption = new Option<Area>("--area", "-a")
            {
                Description = $"Area geografica di ricerca. Valori ammessi: {areaAllowedValuesText}",
                Required = true,
                CustomParser = _areaParser.Parse,
            };

            string provinciaAllowedValuesText = FormatUtils.JoinValues(_provinceMapper.AllowedValues);
            var provinciaOption = new Option<Province>("--provincia", "-p")
            {
                Description = $"Provincia di ricerca. Valori ammessi: {provinciaAllowedValuesText}",
                Required = true,
                CustomParser = _provinciaParser.Parse,
            };

            var rootCommand = new RootCommand("Cup Availability Checker")
            {
                codiceFiscaleOption,
                nreOption,
                areaOption,
                provinciaOption,
            };

            var validatorBinder = new OptionValidatorBinder(rootCommand);
            validatorBinder.AddValidator(codiceFiscaleOption, _codiceFiscaleValidator);
            validatorBinder.AddDependentValidator(provinciaOption, areaOption, _provinciaAreaValidator);

            rootCommand.SetAction(parseResult =>
            {
                string codiceFiscale = parseResult.GetValue(codiceFiscaleOption)!;
                string nre = parseResult.GetValue(nreOption)!;
                Area area = parseResult.GetValue(areaOption);
                Province provincia = parseResult.GetValue(provinciaOption);

                Console.WriteLine($"Codice fiscale: {codiceFiscale}");
                Console.WriteLine($"NRE: {nre}");
                Console.WriteLine($"Area: {area}");
                Console.WriteLine($"Provincia: {provincia}");
            });

            return rootCommand;
        }
    }
}
