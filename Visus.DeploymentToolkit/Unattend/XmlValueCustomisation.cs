// <copyright file="XmlValueCustomisation.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Xml.Linq;


namespace Visus.DeploymentToolkit.Unattend {

    /// <summary>
    /// Sets attributes on elements selected by an XPath expression.
    /// </summary>
    /// <param name="logger"></param>
    public sealed class XmlValueCustomisation(
            ILogger<XmlValueCustomisation> logger)
            : XPathCustomisationBase(logger) {

        #region Public properties
        /// <summary>
        /// Gets or sets the value of the value to be set as content of the
        /// selected element.
        /// </summary>
        [Required]
        public object Value { get; set; } = null!;
        #endregion

        #region Protected methods
        /// <inheritdoc />
        protected override void Apply(XElement element) {
            Debug.Assert(element != null);
            ArgumentNullException.ThrowIfNull(this.Value);
            element.SetValue(this.Value);
        }
        #endregion
    }
}
