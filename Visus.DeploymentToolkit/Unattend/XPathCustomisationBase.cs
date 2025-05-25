// <copyright file="XPathCustomisationStep.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Visus.DeploymentToolkit.Properties;


namespace Visus.DeploymentToolkit.Unattend {

    /// <summary>
    /// The base class for customisation steps that modify an unattend.xml file
    /// using XPath expressions specified by the caller.
    /// </summary>
    /// <param name="logger">The logger used by the step to record errors and
    /// progress messages.</param>
    public abstract class XPathCustomisationBase(ILogger logger)
            : CustomisationBase(logger) {

        #region Public properties
        /// <summary>
        /// Gets or sets whether the element selected by <see cref="Path"/> must
        /// exist or whether the operation should be skipped if it does not.
        /// </summary>
        public bool IsRequired { get; set; } = false;

        /// <summary>
        /// Gets or sets whether the element selected by <see cref="Path"/> must
        /// be unique.
        /// </summary>
        public bool IsUnique { get; set; } = false;

        /// <summary>
        /// Gets or sets the XPath expression that selects the element to be
        /// modified.
        /// </summary>
        /// <remarks>
        /// As XPath 1.0 requires a namespace, the default namespace will be set
        /// to be &quot;unattend&quot;. All expressions must use this prefix.
        /// </remarks>
        [Required]
        public string Path { get; set; } = null!;
        #endregion

        #region Public methods
        /// <inheritdoc />
        public override void Apply(XDocument unattend) {
            ArgumentNullException.ThrowIfNull(this.Path);

            var elements = (this.IsRequired || this.IsUnique)
                ? this.GetRequiredElements(unattend, this.Path)
                : this.GetElements(unattend, this.Path);
            var cnt = elements.Count();

            if (this.IsUnique && (cnt != 1)) {
                Debug.Assert(cnt > 1);
                this._logger.LogError("The XML element at \"{Path}\" is "
                    + "expected to be unique, but the expression matched "
                    + "{Count} elements.", this.Path, cnt);
                throw new InvalidOperationException(string.Format(
                    Errors.NonUniqueXPath, this.Path));
            }

            foreach (var e in elements) {
                this.Apply(e);
            }
        }
        #endregion

        #region Protected methods
        /// <summary>
        /// Applies the customisation step to the element selected by
        /// <see cref="Path"/>.
        /// </summary>
        /// <param name="element">The XML element selected by the XPath
        /// expression, which is guaranteed to exist if the method is called.
        /// </param>
        protected abstract void Apply(XElement element);
        #endregion
    }
}
