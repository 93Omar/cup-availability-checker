using CupAvailabilityChecker.Core.Availability;
using CupAvailabilityChecker.Core.Browser;
using CupAvailabilityChecker.Core.Models;
using CupAvailabilityChecker.Core.Utilities;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace CupAvailabilityChecker.Core.Navigation
{
    /// <summary>
    /// Step 1 for <see cref="BookingMode.Existing"/>: identifies the existing booking on the
    /// "Le mie Prenotazioni" page by codice fiscale + NRE (this flow has no separate login form),
    /// reads its current date/time into <see cref="AvailabilityCheckContext"/>, moves to the
    /// "check for a better date" page, and applies the area/provincia filters to reach the
    /// availability results.
    /// </summary>
    public sealed class ExistingBookingNavigationStep : INavigationStep
    {
        private const string StartUrl = "https://cupweb.sardegnasalute.it/web/guest/le-mie-prenotazioni";

        private static readonly WebElementLocator FiscalCodeInput =
            new("_listaprenotazioni_WAR_cupprenotazione_:prescrizioniForm:CFInput", WebElementLocatorType.Id);

        private static readonly WebElementLocator SearchTypeSelect =
            new("_listaprenotazioni_WAR_cupprenotazione_:prescrizioniForm:IDSearchTypeInput", WebElementLocatorType.Id);

        private const string SearchTypeNreOptionValue = "nre-label";

        private static readonly WebElementLocator SearchValueInput =
            new("_listaprenotazioni_WAR_cupprenotazione_:prescrizioniForm:IDSearchValueInput", WebElementLocatorType.Id);

        private static readonly WebElementLocator FilterButton =
            new("_listaprenotazioni_WAR_cupprenotazione_:prescrizioniForm:filterPrescr_button", WebElementLocatorType.Name);

        private static readonly WebElementLocator CurrentBookingDateSpan =
            new("_listaprenotazioni_WAR_cupprenotazione_:prescrizioniForm:j_idt72:0:j_idt107:0:_t133", WebElementLocatorType.Id);

        private static readonly WebElementLocator CurrentBookingTimeSpan =
            new("_listaprenotazioni_WAR_cupprenotazione_:prescrizioniForm:j_idt72:0:j_idt107:0:_t135", WebElementLocatorType.Id);

        private static readonly WebElementLocator MoveButton =
            new("_listaprenotazioni_WAR_cupprenotazione_:prescrizioniForm:j_idt72:0:spostaButton_button", WebElementLocatorType.Name);

        private static readonly WebElementLocator MacrozonaSelect =
            new("_ricettaelettronica_WAR_cupprenotazione_:appuntamentiForm:macrozonaDropDown_input", WebElementLocatorType.Id);

        private static readonly WebElementLocator ZonaSelect =
            new("_ricettaelettronica_WAR_cupprenotazione_:appuntamentiForm:zonaDropDown_input", WebElementLocatorType.Name);

        private readonly AreaSelectValueMapper _areaSelectValueMapper;
        private readonly ProvinceSelectValueMapper _provinceSelectValueMapper;
        private readonly ExistingBookingAvailabilityRefresher _availabilityRefresher;
        private readonly SeleniumWaitHelper _waitHelper;
        private readonly SeleniumClickHelper _clickHelper;
        private readonly CookieBannerDismisser _cookieBannerDismisser;

        public ExistingBookingNavigationStep(
            AreaSelectValueMapper areaSelectValueMapper,
            ProvinceSelectValueMapper provinceSelectValueMapper,
            ExistingBookingAvailabilityRefresher availabilityRefresher,
            SeleniumWaitHelper waitHelper,
            SeleniumClickHelper clickHelper,
            CookieBannerDismisser cookieBannerDismisser)
        {
            _areaSelectValueMapper = areaSelectValueMapper;
            _provinceSelectValueMapper = provinceSelectValueMapper;
            _availabilityRefresher = availabilityRefresher;
            _waitHelper = waitHelper;
            _clickHelper = clickHelper;
            _cookieBannerDismisser = cookieBannerDismisser;
        }

        public void NavigateToAvailabilityPage(IWebDriver driver, AvailabilityCheckContext context)
        {
            BookingParameters parameters = context.Parameters;

            driver.Navigate().GoToUrl(StartUrl);
            _cookieBannerDismisser.Dismiss(driver);

            IWebElement fiscalCodeInput = _waitHelper.WaitUntilVisible(driver, FiscalCodeInput.ToBy());
            fiscalCodeInput.SendKeys(parameters.FiscalCode);

            SelectElement searchTypeSelect = new SelectElement(driver.FindElement(SearchTypeSelect.ToBy()));
            searchTypeSelect.SelectByValue(SearchTypeNreOptionValue);

            IWebElement searchValueInput = _waitHelper.WaitUntilVisible(driver, SearchValueInput.ToBy());
            searchValueInput.SendKeys(parameters.Nre);

            IWebElement filterButton = _waitHelper.WaitUntilVisible(driver, FilterButton.ToBy());
            _waitHelper.WaitForFullPageNavigation(driver, () => _clickHelper.Click(driver, filterButton));
            _cookieBannerDismisser.Dismiss(driver);

            IWebElement currentDateSpan = _waitHelper.WaitUntilVisible(driver, CurrentBookingDateSpan.ToBy());
            IWebElement currentTimeSpan = driver.FindElement(CurrentBookingTimeSpan.ToBy());
            context.CurrentBookingDate = ItalianDateTimeParser.ParseDate(currentDateSpan.Text);
            context.CurrentBookingTime = ItalianDateTimeParser.ParseTime(currentTimeSpan.Text);

            IWebElement moveButton = _waitHelper.WaitUntilVisible(driver, MoveButton.ToBy());
            _waitHelper.WaitForFullPageNavigation(driver, () => _clickHelper.Click(driver, moveButton));
            _cookieBannerDismisser.Dismiss(driver);

            ApplyAvailabilityFilters(driver, parameters);
        }

        private void ApplyAvailabilityFilters(IWebDriver driver, BookingParameters parameters)
        {
            IWebElement macrozonaElement = _waitHelper.WaitUntilVisible(driver, MacrozonaSelect.ToBy());
            SelectElement macrozonaSelect = new SelectElement(macrozonaElement);
            string? macrozonaValue = _areaSelectValueMapper.GetSelectValue(parameters.Area);
            SelectByValueOrFirstOption(macrozonaSelect, macrozonaValue);

            // Changing the macrozona select repopulates the zona select's options (possibly via a
            // full page reload). Rather than guessing when that update has finished, wait directly
            // for the option we actually need to appear in the zona select.
            string? zonaValue = _provinceSelectValueMapper.GetSelectValue(parameters.Province);
            IWebElement zonaElement = _waitHelper.WaitUntilSelectHasOption(driver, ZonaSelect.ToBy(), zonaValue);
            _cookieBannerDismisser.Dismiss(driver);

            SelectElement zonaSelect = new SelectElement(zonaElement);
            SelectByValueOrFirstOption(zonaSelect, zonaValue);

            _availabilityRefresher.Refresh(driver);
        }

        // A null select value means the "all" option, which has no value attribute and is always
        // the first option in the select.
        private static void SelectByValueOrFirstOption(SelectElement select, string? value)
        {
            if (value is null)
                select.SelectByIndex(0);
            else
                select.SelectByValue(value);
        }
    }
}
