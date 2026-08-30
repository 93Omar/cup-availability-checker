using System.CommandLine;
using CupAvailabilityChecker.Cli.Binding;
using CupAvailabilityChecker.Cli.Mapping;
using CupAvailabilityChecker.Cli.Utilities;
using CupAvailabilityChecker.Cli.Validation;
using CupAvailabilityChecker.Core;
using CupAvailabilityChecker.Core.Models;
using Microsoft.Extensions.Logging;

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
        private readonly IItalianEnumMapper<BookingMode> _bookingModeMapper;
        private readonly ItalianEnumOptionParser<Area> _areaParser;
        private readonly ItalianEnumOptionParser<Province> _provinciaParser;
        private readonly ItalianEnumOptionParser<BookingMode> _modalitaParser;
        private readonly RadiusOptionParser _radiusParser;
        private readonly FiscalCodeValidator _fiscalCodeValidator;
        private readonly ProvinceAreaValidator _provinceAreaValidator;
        private readonly MunicipalityValidator _municipalityValidator;
        private readonly MunicipalitiesValidator _municipalitiesValidator;
        private readonly RadiusValidator _radiusValidator;
        private readonly BookingCheckOrchestrator _orchestrator;
        private readonly ILogger<RootCommandBuilder> _logger;

        public RootCommandBuilder(
            IItalianEnumMapper<Area> areaMapper,
            IItalianEnumMapper<Province> provinceMapper,
            IItalianEnumMapper<BookingMode> bookingModeMapper,
            ItalianEnumOptionParser<Area> areaParser,
            ItalianEnumOptionParser<Province> provinciaParser,
            ItalianEnumOptionParser<BookingMode> modalitaParser,
            RadiusOptionParser radiusParser,
            FiscalCodeValidator fiscalCodeValidator,
            ProvinceAreaValidator provinceAreaValidator,
            MunicipalityValidator municipalityValidator,
            MunicipalitiesValidator municipalitiesValidator,
            RadiusValidator radiusValidator,
            BookingCheckOrchestrator orchestrator,
            ILogger<RootCommandBuilder> logger)
        {
            _areaMapper = areaMapper;
            _provinceMapper = provinceMapper;
            _bookingModeMapper = bookingModeMapper;
            _areaParser = areaParser;
            _provinciaParser = provinciaParser;
            _modalitaParser = modalitaParser;
            _radiusParser = radiusParser;
            _fiscalCodeValidator = fiscalCodeValidator;
            _provinceAreaValidator = provinceAreaValidator;
            _municipalityValidator = municipalityValidator;
            _municipalitiesValidator = municipalitiesValidator;
            _radiusValidator = radiusValidator;
            _orchestrator = orchestrator;
            _logger = logger;
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

            string modalitaAllowedValuesText = FormatUtils.JoinValues(_bookingModeMapper.AllowedValues);
            var modalitaOption = new Option<BookingMode>("--modalita", "-m")
            {
                Description = $"Modalità di ricerca. Valori ammessi: {modalitaAllowedValuesText}",
                Required = true,
                CustomParser = _modalitaParser.Parse,
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

            // Alternative filtering modes (mutually exclusive; the actual filtering/booking logic
            // will be implemented later): an explicit list of comuni, or a reference comune with a
            // search radius in km.
            var comuniOption = new Option<string[]>("--comuni")
            {
                Description = "Elenco dei comuni su cui filtrare la ricerca. Alternativo a --comune/--raggio.",
                Required = false,
                AllowMultipleArgumentsPerToken = true,
            };

            var comuneOption = new Option<string>("--comune")
            {
                Description = "Comune di riferimento per la ricerca per raggio. Da usare insieme a --raggio.",
                Required = false,
            };

            // Invariant culture for the decimal separator: prevents "15.5" from being interpreted
            // as "155" under an it-IT culture (dot as thousands separator).
            var raggioOption = new Option<double?>("--raggio")
            {
                Description = "Raggio di ricerca in km dal comune indicato con --comune.",
                Required = false,
                CustomParser = _radiusParser.Parse,
            };

            var giorniOption = new Option<int?>("--giorni", "-g")
            {
                Description = "Numero massimo di giorni da oggi entro cui una disponibilità è considerata interessante (solo --modalita nuova).",
                Required = false,
            };

            var intervalloOption = new Option<int>("--intervallo", "-i")
            {
                Description = "Intervallo in secondi tra un controllo di disponibilità e il successivo.",
                Required = false,
                DefaultValueFactory = _ => 30,
            };

            var browserOption = new Option<BrowserType>("--browser", "-b")
            {
                Description = $"Browser da usare per la navigazione. Valori ammessi: {FormatUtils.JoinValues(Enum.GetValues<BrowserType>())}",
                Required = false,
                DefaultValueFactory = _ => BrowserType.Chrome,
            };

            var headlessOption = new Option<bool>("--headless")
            {
                Description = "Esegue il browser in modalità headless (senza finestra visibile).",
                Required = false,
            };

            var rootCommand = new RootCommand("Cup Availability Checker")
            {
                codiceFiscaleOption,
                nreOption,
                modalitaOption,
                areaOption,
                provinciaOption,
                comuniOption,
                comuneOption,
                raggioOption,
                giorniOption,
                intervalloOption,
                browserOption,
                headlessOption,
            };

            var validatorBinder = new OptionValidatorBinder(rootCommand);
            validatorBinder.AddValidator(codiceFiscaleOption, _fiscalCodeValidator);
            validatorBinder.AddDependentValidator(provinciaOption, areaOption, _provinceAreaValidator);
            validatorBinder.AddValidator(comuneOption, _municipalityValidator);
            validatorBinder.AddValidator(comuniOption, _municipalitiesValidator);
            validatorBinder.AddValidator(raggioOption, _radiusValidator);

            rootCommand.SetAction(async (parseResult, cancellationToken) =>
            {
                string codiceFiscale = parseResult.GetValue(codiceFiscaleOption)!;
                string nre = parseResult.GetValue(nreOption)!;
                BookingMode modalita = parseResult.GetValue(modalitaOption);
                Area area = parseResult.GetValue(areaOption);
                Province provincia = parseResult.GetValue(provinciaOption);
                string[]? comuni = parseResult.GetValue(comuniOption);
                string? comune = parseResult.GetValue(comuneOption);
                double? raggio = parseResult.GetValue(raggioOption);
                int? giorni = parseResult.GetValue(giorniOption);
                int intervallo = parseResult.GetValue(intervalloOption);
                BrowserType browser = parseResult.GetValue(browserOption);
                bool headless = parseResult.GetValue(headlessOption);

                string comuniText = comuni is null ? "-" : FormatUtils.JoinValues(comuni);

                _logger.LogInformation(
                    "Parametri ricevuti: CodiceFiscale={CodiceFiscale}, Nre={Nre}, Modalita={Modalita}, Area={Area}, Provincia={Provincia}, Comuni={Comuni}, Comune={Comune}, Raggio={Raggio}, Giorni={Giorni}, Intervallo={Intervallo}s, Browser={Browser}, Headless={Headless}",
                    codiceFiscale, nre, modalita, area, provincia, comuniText, comune, raggio, giorni, intervallo, browser, headless);

                BookingParameters parameters = new BookingParameters(
                    codiceFiscale,
                    nre,
                    modalita,
                    area,
                    provincia,
                    comuni,
                    comune,
                    raggio,
                    giorni,
                    TimeSpan.FromSeconds(intervallo),
                    browser,
                    headless);

                await _orchestrator.RunAsync(parameters, cancellationToken);
            });

            return rootCommand;
        }
    }
}
