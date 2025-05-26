// <copyright file="XmlValueCustomisation.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Visus.DeploymentToolkit.Extensions;


namespace Visus.DeploymentToolkit.Unattend {

    /// <summary>
    /// Customises the &quot;Microsoft-Windows-International-Core&quot; and
    /// &quot;Microsoft-Windows-International-Core-WinPE&quot; components.
    /// </summary>
    /// <param name="logger"></param>
    public sealed class LocalisationCustomisation(
            ILogger<LocalisationCustomisation> logger)
            : CustomisationBase(logger) {

        #region Public properties
        /// <summary>
        /// Gets or sets the name of the components to be searched for
        /// localisation stuff. This defaults to international core and
        /// international cor for WinPE.
        /// </summary>
        [Required]
        public IEnumerable<string> Components {
            get;
            set;
        } = [
            "Microsoft-Windows-International-Core",
            "Microsoft-Windows-International-Core-WinPE"
        ];

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

        /// <inheritdoc />
        public override void Apply(XDocument unattend) {
            ArgumentNullException.ThrowIfNull(this.Components);
            ArgumentNullException.ThrowIfNull(unattend);
            var resolver = GetResolver(unattend);

            foreach (var c in this.Components) {
                var elements = unattend.XPathSelectElements(
                    $"//u:component[@name='{c}']",
                    resolver);

                foreach (var e in elements) {
                    this._logger.LogTrace("Setting locales for {Element}",
                        e.GetXPath(resolver));
                    this.SetLocales(e, resolver);
                }
            }
        }

        private void SetLocales(XElement element,
                IXmlNamespaceResolver resolver) {
            Debug.Assert(element != null);
            Debug.Assert(resolver != null);

            var input = element.Descendant("InputLocale");
            var setup = element.XPathSelectElement(
                "/u:SetupUILanguage/u:UILanguage",
                resolver);
            var system = element.Descendant("SystemLocale");
            var ui = element.Descendant("UILanguage");
            var user = element.Descendant("UserLocale");

            {
                var locale = this.InputProfile
                    ?? this.InputLocale?.IetfLanguageTag;
                if ((locale is not null) && (input is not null)) {
                    this._logger.LogTrace("Setting {Element} to \"{Locale}\"",
                        input.Name.LocalName, locale);
                    input.SetValue(locale);
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
    }
}
