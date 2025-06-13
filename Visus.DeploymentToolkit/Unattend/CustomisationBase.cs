// <copyright file="CustomisationBase.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Visus.DeploymentToolkit.Extensions;
using Visus.DeploymentToolkit.Services;


namespace Visus.DeploymentToolkit.Unattend {

    /// <summary>
    /// The base class for customisation steps, which provides several methods
    /// facilitating the modification of an unattend.xml file.
    /// </summary>
    /// <param name="logger">The logger used by the step to record errors and
    /// progress messages.</param>
    public abstract class CustomisationBase(ILogger logger) : ICustomisation {

        #region Public properties
        /// <inheritdoc />
        public IEnumerable<string> Passes { get; set; } = [];
        #endregion

        #region Public methods
        /// <inheritdoc />
        public abstract void Apply(XDocument unattend);
        #endregion

        #region Protected properties
        /// <summary>
        /// Gets an XPath filter expression that matches the settings in the
        /// specified <see cref="Passes"/>.
        /// </summary>
        /// <remarks>
        /// <para>Callers must use the namespace resolver provided by
        /// <see cref="GetResolver(XDocument)"/> for this method to work.</para>
        /// <para>If no <see cref="Passes"/> are specified, all settings sections
        /// are selected by the filter expression returned.</para>
        /// </remarks>
        public string SettingsFilter
            => ((this.Passes is null) || !this.Passes.Any())
            ? "//u:settings"
            : $"//u:settings[@pass='{string.Join("' or @pass='", this.Passes)}']";
        #endregion

        #region Protected methods
        /// <summary>
        /// Finds the architecture of any component in the given unattend.xml.
        /// </summary>
        /// <remarks>
        /// A valid unattend.xml should have the same processor architecture for
        /// all components, so it should be safe to select any of them as
        /// reference.
        /// </remarks>
        /// <param name="unattend"></param>
        /// <param name="resolver"></param>
        /// <returns></returns>
        protected static string GetArchitecture(XDocument unattend,
                IXmlNamespaceResolver? resolver = null)
            => unattend.XPathSelectElement("//*[@processorArchitecture]",
                resolver ?? GetResolver(unattend))
                ?.Attribute("processorArchitecture")?.Value
                ?? RuntimeInformation.ProcessArchitecture.GetFolder();

        /// <summary>
        /// Gets a filter expression that matches the component with the
        /// specified name.
        /// </summary>
        /// <param name="componentName"></param>
        /// <returns></returns>
        protected static string GetComponentFilter(string componentName)
            => $"//u:component[@name='{componentName}']";

        /// <summary>
        /// Gets a filter expression that matches any of the componets with the
        /// specified names.
        /// </summary>
        /// <param name="componentNames"></param>
        /// <returns></returns>
        protected static string GetComponentFilter(params string[] componentNames)
            => $"//u:component[@name='{string.Join("' or @name='", componentNames)}']";

        /// <summary>
        /// Gets a namespace resolver for the default namespace of the given
        /// XML documment, which is mapped to the prefix &quot;unattend&quot;
        /// and &quot;u&quot;.
        /// </summary>
        /// <param name="unattend"></param>
        /// <returns></returns>
        protected static IXmlNamespaceResolver GetResolver(XDocument unattend)
            => unattend.GetResolver("unattend", "u");

        ///// <summary>
        ///// Extracts all valid <see cref="Phases"/> from
        ///// <paramref name="unattend"/>.
        ///// </summary>
        ///// <param name="unattend"></param>
        ///// <returns></returns>
        //protected IEnumerable<XElement> GetPhases(XDocument unattend) {
        //    if ((this.Phases is null) || !this.Phases.Any()) {

        //    }
        //}

        /// <summary>
        /// Gets the component with the specified name from the given
        /// <paramref name="element"/>, or if a <paramref name="builder"/> is
        /// given, adds if it does not exist yet.
        /// </summary>
        /// <param name="element">The element where the component should be
        /// located.</param>
        /// <param name="component">The name of the component. A filter for this
        /// component will be created using
        /// <see cref="GetComponentFilter(string)"/>.</param>
        /// <param name="resolver">A namespace resolver for the XPath
        /// expression. Albeit being an optional parameter, the unattend schema
        /// does not work without it.</param>
        /// <param name="builder">An optional <see cref="IUnattendBuilder"/>
        /// that is used to add the component if it does not exist. If this
        /// parameter is <see langword="null"/>, missing components will not be
        /// added.</param>
        /// <param name="architecture">The architecture to be used when creating
        /// a new component. If <see langword="null"/>, the architecture of the
        /// calling process will be used.</param>
        /// <returns></returns>
        [return: NotNullIfNotNull(nameof(builder))]
        protected XElement? GetComponent(XElement element,
                string component,
                IXmlNamespaceResolver? resolver,
                IUnattendBuilder? builder = null,
                string? architecture = null) {
            ArgumentNullException.ThrowIfNull(element);
            ArgumentException.ThrowIfNullOrEmpty(component);

            var filter = GetComponentFilter(component);
            this._logger.LogTrace("Searching component {Component} using "
                + "the XPath expression {Filter}.", component, filter);
            var retval = element.XPathSelectElement(filter, resolver);

            if ((retval is null) && (builder is not null)) {
                this._logger.LogTrace("Creating the missing component "
                    + "{Component}.", component);
                retval = builder.MakeComponent(component, architecture);
                element.Add(retval);
            }

            return retval;
        }
        #endregion

        #region Protected fields
        /// <summary>
        /// The logger used by the step to record errors and progress messages.
        /// </summary>
        protected readonly ILogger _logger = logger
            ?? throw new ArgumentNullException(nameof(logger));
        #endregion
    }
}
