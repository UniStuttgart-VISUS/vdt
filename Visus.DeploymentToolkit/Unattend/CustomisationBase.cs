// <copyright file="CustomisationBase.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Visus.DeploymentToolkit.Extensions;


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
            : $"//u:settings[@pass='{string.Join(" or @pass='", this.Passes)}']";
        #endregion

        #region Protected methods
        /// <summary>
        /// Gets a filter expression that matches the component with the
        /// specified name.
        /// </summary>
        /// <param name="componentName"></param>
        /// <returns></returns>
        protected static string GetComponentFilter(string componentName)
            => $"//u:component[@name='{componentName}']";

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
