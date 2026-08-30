using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace CupAvailabilityChecker.Core.Utilities
{
    /// <summary>
    /// Shared Selenium wait utility: CupWeb pages update parts of the DOM via AJAX (JSF/PrimeFaces)
    /// without a full page reload, so elements must be waited for explicitly instead of assuming
    /// they are already present right after triggering an action such as a button click.
    /// </summary>
    public sealed class SeleniumWaitHelper
    {
        private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(30);

        // CupWeb shows a full-page loading overlay (a div with id "blocking") while an AJAX
        // request is in flight, then sets it to display:none once the partial page update has
        // been applied. Waiting for it to disappear before waiting for the updated content avoids
        // racing the visibility check against the AJAX re-render.
        private static readonly By LoadingOverlayLocator = By.Id("blocking");

        private readonly ILogger<SeleniumWaitHelper> _logger;

        public SeleniumWaitHelper(ILogger<SeleniumWaitHelper> logger)
        {
            _logger = logger;
        }

        public IWebElement WaitUntilVisible(IWebDriver driver, By locator, TimeSpan? timeout = null)
        {
            WebDriverWait wait = new WebDriverWait(driver, timeout ?? DefaultTimeout);

            try
            {
                return wait.Until(currentDriver => currentDriver
                    .FindElements(locator)
                    .FirstOrDefault(element => element.Displayed));
            }
            catch (WebDriverTimeoutException ex)
            {
                _logger.LogError(ex, "Elemento non trovato nel DOM entro il timeout: {Locator}", locator);
                throw;
            }
        }

        /// <summary>
        /// Waits for the page to settle after triggering an action that either starts an AJAX
        /// request or a full page navigation (some CupWeb buttons use "fullSubmit", causing an
        /// actual browser page load instead of a pure AJAX partial update - visible as the tab's
        /// loading spinner in Chrome). Checks both <c>document.readyState</c> (covers full page
        /// reloads) and the AJAX loading overlay (covers in-place partial updates), and treats
        /// transient errors thrown while the old document is unloading as "not settled yet"
        /// instead of failing outright. Should be called right after the triggering action, before
        /// waiting for the resulting DOM update, so that visibility checks do not race against the
        /// page still loading or being re-rendered.
        /// </summary>
        public void WaitForPageToSettle(IWebDriver driver, TimeSpan? timeout = null)
        {
            WebDriverWait wait = new WebDriverWait(driver, timeout ?? DefaultTimeout);

            try
            {
                wait.Until(currentDriver =>
                {
                    try
                    {
                        string? readyState = ((IJavaScriptExecutor)currentDriver)
                            .ExecuteScript("return document.readyState;") as string;

                        if (readyState != "complete")
                            return false;

                        IWebElement? overlay = currentDriver.FindElements(LoadingOverlayLocator).FirstOrDefault();
                        return overlay is null || !overlay.Displayed;
                    }
                    catch (WebDriverException)
                    {
                        // The old document may still be unloading while a full page navigation is
                        // starting; treat this as "not settled yet" rather than failing the wait.
                        return false;
                    }
                });
            }
            catch (WebDriverTimeoutException ex)
            {
                _logger.LogError(ex, "La pagina non si è stabilizzata (caricamento/AJAX) entro il timeout.");
                throw;
            }
        }

        /// <summary>
        /// Waits for a select element to contain an option with the given value (or, if
        /// <paramref name="optionValue"/> is null, simply waits for the select to be visible - a
        /// null value means the "all" option, which is always the first option and thus always
        /// present). Useful when a preceding action (e.g. selecting another, related dropdown) may
        /// asynchronously repopulate this select's options, since it directly waits for the actual
        /// condition we care about instead of guessing when the page/AJAX update has "settled".
        /// </summary>
        public IWebElement WaitUntilSelectHasOption(IWebDriver driver, By selectLocator, string? optionValue, TimeSpan? timeout = null)
        {
            WebDriverWait wait = new WebDriverWait(driver, timeout ?? DefaultTimeout);

            try
            {
                return wait.Until(currentDriver =>
                {
                    try
                    {
                        IWebElement? select = currentDriver.FindElements(selectLocator).FirstOrDefault(element => element.Displayed);
                        if (select is null)
                            return null;

                        if (optionValue is null)
                            return select;

                        bool hasOption = select.FindElements(By.CssSelector($"option[value='{optionValue}']")).Count > 0;
                        return hasOption ? select : null;
                    }
                    catch (StaleElementReferenceException)
                    {
                        // The select may have been replaced by an AJAX/page update while we were
                        // reading its options; treat this as "not ready yet" and retry.
                        return null;
                    }
                });
            }
            catch (WebDriverTimeoutException ex)
            {
                _logger.LogError(ex, "La select non contiene l'opzione richiesta entro il timeout: {Locator}, {OptionValue}", selectLocator, optionValue);
                throw;
            }
        }

        /// <summary>
        /// Waits for a full browser page navigation to complete after triggering an action known
        /// to cause one (e.g. a button whose onclick performs a real form submit rather than a
        /// PrimeFaces/JSF AJAX update - CupWeb shows the browser tab's own loading spinner in these
        /// cases). Captures the current page's root <c>&lt;html&gt;</c> element before invoking
        /// <paramref name="triggerNavigation"/>, then waits for it to become stale - which is
        /// guaranteed to happen once the old document is torn down by a real navigation, unlike
        /// waiting for a specific control to go stale (some AJAX partial updates preserve the very
        /// element that triggered them) - before waiting for the new page to settle.
        /// </summary>
        public void WaitForFullPageNavigation(IWebDriver driver, Action triggerNavigation, TimeSpan? timeout = null)
        {
            IWebElement previousPageRoot = driver.FindElement(By.TagName("html"));

            triggerNavigation();

            WebDriverWait wait = new WebDriverWait(driver, timeout ?? DefaultTimeout);

            try
            {
                wait.Until(currentDriver =>
                {
                    try
                    {
                        _ = previousPageRoot.Enabled;
                        return false;
                    }
                    catch (StaleElementReferenceException)
                    {
                        return true;
                    }
                });
            }
            catch (WebDriverTimeoutException ex)
            {
                _logger.LogError(ex, "La pagina non sembra essersi ricaricata (nessuna navigazione rilevata) entro il timeout.");
                throw;
            }

            WaitForPageToSettle(driver, timeout);
        }
    }
}