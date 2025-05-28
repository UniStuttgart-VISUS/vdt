// <copyright file="XmlValueCustomisation.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Visus.DeploymentToolkit.Extensions;
using Visus.DeploymentToolkit.Services;
using Visus.DeploymentToolkit.SystemInformation;


namespace Visus.DeploymentToolkit.Unattend {

    /// <summary>
    /// Customises the &quot;Microsoft-Windows-International-Core&quot; and
    /// &quot;Microsoft-Windows-International-Core-WinPE&quot; components.
    /// </summary>
    /// <param name="logger"></param>
    public sealed class LocalisationCustomisation(
            IUnattendBuilder builder,
            ILogger<LocalisationCustomisation> logger)
            : CustomisationBase(logger) {

        #region Public properties
        /// <summary>
        /// Gets or sets the input locale to be configured.
        /// </summary>
        /// <remarks>
        /// If both, <see cref="InputLocale"/> and <see cref="InputProfile"/>,
        /// are set, the input profile takes precedence.
        /// </remarks>
        public CultureInfo? InputLocale { get; set; }

        /// <summary>
        /// Gets or sets the input profile to be configured.
        /// </summary>
        /// <remarks>
        /// If both, <see cref="InputLocale"/> and <see cref="InputProfile"/>,
        /// are set, the input profile takes precedence.
        /// </remarks>
        public string? InputProfile { get; set; }

        /// <summary>
        /// Gets or sets the setup locale to be configured for WinPE.
        /// </summary>
        public CultureInfo? SetupLocale { get; set; }

        /// <summary>
        /// Gets or sets the system locale to be configured.
        /// </summary>
        public CultureInfo? SystemLocale { get; set; }

        /// <summary>
        /// Gets or sets the user interface language to be configured.
        /// </summary>
        public CultureInfo? UserInterfaceLanguage { get; set; }

        /// <summary>
        /// Gets or sets the user locale to be configured.
        /// </summary>
        public CultureInfo? UserLocale { get; set; }
        #endregion

        #region Public methods
        /// <inheritdoc />
        public override void Apply(XDocument unattend) {
            ArgumentNullException.ThrowIfNull(unattend);
            var resolver = GetResolver(unattend);
            var settings = unattend.XPathSelectElements(this.SettingsFilter,
                resolver);

            foreach (var s in settings) {
                var elements = s.XPathSelectElements(GetComponentFilter(
                    Components.InternationalCore,
                    Components.InternationalCoreWinPe), resolver);

                foreach (var e in elements) {
                    this._logger.LogTrace("Setting locales for {Element}",
                        e.GetXPath(resolver));
                    this.SetLocales(e, resolver);
                }

                if (!elements.Any()) {
                    var pass = s.Attribute("pass")?.Value;
                    var arch = GetArchitecture(unattend, resolver);

                    if (pass == Unattend.Passes.WindowsPE) {
                        this._logger.LogTrace("There was no existing "
                            + "localisation block, so we add \"{Component}\".",
                            Components.InternationalCoreWinPe);
                        s.Add(this.MakeInternationalCoreWinPE(arch));

                    } else {
                        this._logger.LogTrace("There was no existing "
                            + "localisation block, so we add \"{Component}\".",
                            Components.InternationalCore);
                        s.Add(this.MakeInternationalCore(arch));
                    }
                }
            }
        }
        #endregion

        #region Private properties
        /// <summary>
        /// Gets the actual value for the input profile based on the
        /// <see cref="InputProfile"/> or on the <see cref="InputLocale"/>.
        /// </summary>
        private string? ConsolidatedInputProfile
            => this.InputProfile
                ?? InputProfiles.ForCulture(this.InputLocale)
                ?? this.InputLocale?.IetfLanguageTag;
        #endregion

        #region Private methods
        /// <summary>
        /// Makes an international core component element with the properties of
        /// this object set as initial attribute values.
        /// </summary>
        /// <param name="architecture"></param>
        /// <returns></returns>
        private XElement MakeInternationalCore(string? architecture)
            => this.AddLocales(this._builder.MakeComponent(
                Components.InternationalCore, architecture));

        /// <summary>
        /// Makes an international core for WinPE component element with the
        /// properties of this object set as initial attribute values.
        /// </summary>
        /// <param name="architecture"></param>
        /// <returns></returns>
        private XElement MakeInternationalCoreWinPE(string? architecture) {
            var retval = this._builder.MakeComponent(
                Components.InternationalCoreWinPe, architecture);

            var uiLang = new XElement(this._builder.MakeName(
                UILanguageElement));
            var lang = this.SetupLocale ?? CultureInfo.CurrentCulture;
            uiLang.SetValue(lang.IetfLanguageTag);

            var setupUI = new XElement(this._builder.MakeName(
                SetupUILanguageElement));
            setupUI.Add(uiLang);

            retval.Add(setupUI);

            return this.AddLocales(retval);
        }

        /// <summary>
        /// Adds teh locales shared between international core and WinPE
        /// as children of the given element.
        /// </summary>
        /// <param name="retval">The parent element to which the locales
        /// should be added.</param>
        /// <returns><paramref name="retval"/>.</returns>
        private XElement AddLocales(XElement retval) {
            Debug.Assert(retval is not null);

            {
                var element = new XElement(this._builder.MakeName(
                    InputLocaleElement));
                element.SetValue(this.ConsolidatedInputProfile
                    ?? InputProfiles.ForCulture(CultureInfo.CurrentCulture)
                    ?? InputProfiles.German);
                retval.Add(element);
            }

            {
                var element = new XElement(this._builder.MakeName(
                    SystemLocaleElement));
                var lang = this.SystemLocale ?? CultureInfo.CurrentCulture;
                element.SetValue(lang.IetfLanguageTag);
                retval.Add(element);
            }

            {
                var element = new XElement(this._builder.MakeName(
                    UILanguageElement));
                var lang = this.UserInterfaceLanguage
                    ?? CultureInfo.CurrentCulture;
                element.SetValue(lang.IetfLanguageTag);
                retval.Add(element);
            }

            {
                var element = new XElement(this._builder.MakeName(
                    UserLocaleElement));
                var lang = this.UserLocale ?? CultureInfo.CurrentCulture;
                element.SetValue(lang.IetfLanguageTag);
                retval.Add(element);
            }

            return retval;
        }

        /// <summary>
        /// Sets all configured locale information on the given element.
        /// </summary>
        /// <param name="element"></param>
        /// <param name="resolver"></param>
        private void SetLocales(XElement element,
                IXmlNamespaceResolver resolver) {
            Debug.Assert(element != null);
            Debug.Assert(resolver != null);

            var input = element.Descendant(InputLocaleElement);
            var setup = element.XPathSelectElement(
                $"/u:{SetupUILanguageElement}/u:{UILanguageElement}",
                resolver);
            var system = element.Descendant(SystemLocaleElement);
            var ui = element.Descendant(UILanguageElement);
            var user = element.Descendant(UserLocaleElement);

            {
                var profile = ConsolidatedInputProfile;
                if ((profile is not null) && (input is not null)) {
                    this._logger.LogTrace("Setting {Element} to \"{Locale}\"",
                        input.Name.LocalName, profile);
                    input.SetValue(profile);
                }
            }

            if ((this.SetupLocale is not null) && (setup is not null)) {
                this._logger.LogTrace("Setting {Element} to \"{Locale}\"",
                    setup.Name.LocalName, this.SetupLocale.IetfLanguageTag);
                setup.SetValue(this.SetupLocale.IetfLanguageTag);
            }

            if ((this.SystemLocale is not null) && (system is not null)) {
                this._logger.LogTrace("Setting {Element} to \"{Locale}\"",
                    system.Name.LocalName, this.SystemLocale.IetfLanguageTag);
                system.SetValue(this.SystemLocale.IetfLanguageTag);
            }

            if ((this.UserInterfaceLanguage is not null) && (ui is not null)) {
                this._logger.LogTrace("Setting {Element} to \"{Locale}\"",
                    ui.Name.LocalName,
                    this.UserInterfaceLanguage.IetfLanguageTag);
                ui.SetValue(this.UserInterfaceLanguage.IetfLanguageTag);
            }

            if ((this.UserLocale is not null) && (user is not null)) {
                this._logger.LogTrace("Setting {Element} to \"{Locale}\"",
                    user.Name.LocalName, this.UserLocale.IetfLanguageTag);
                user.SetValue(this.UserLocale.IetfLanguageTag);
            }
        }
        #endregion

        #region Private fields
        private const string InputLocaleElement = "InputLocale";
        private const string SetupUILanguageElement = "SetupUILanguage";
        private const string SystemLocaleElement = "SystemLocale";
        private const string UserLocaleElement = "UserLocale";
        private const string UILanguageElement = "UILanguage";
        private readonly IUnattendBuilder _builder = builder
            ?? throw new ArgumentNullException(nameof(builder));
        #endregion
    }
}
