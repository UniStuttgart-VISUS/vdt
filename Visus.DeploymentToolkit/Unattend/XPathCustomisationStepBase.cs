// <copyright file="XPathCustomisationStep.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using System.Xml.XPath;
using Visus.DeploymentToolkit.Properties;


namespace Visus.DeploymentToolkit.Unattend {

    /// <summary>
    /// The base class for customisation steps that modify an unattend.xml file
    /// using XPath expressions.
    /// </summary>
    /// <param name="logger">The logger used by the step to record errors and
    /// progress messages.</param>
    public abstract class XPathCustomisationStepBase(ILogger logger)
            : ICustomisationStep {

        #region Public properties
        /// <summary>
        /// Gets or sets whether the element selected by <see cref="Path"/> must
        /// exist or whether the operation should be skipped if it does not.
        /// </summary>
        public bool IsRequired { get; set; } = false;

        /// <summary>
        /// Gets or sets the XPath expression that selects the element to be
        /// modified.
        /// </summary>
        [Required]
        public string Path { get; set; } = null!;
        #endregion

        #region Public methods
        /// <inheritdoc />
        public void Apply(XDocument unattend) {
            ArgumentNullException.ThrowIfNull(unattend);

            this._logger.LogTrace("Selecting element to modify via XPath "
                + "expression \"{Path}\".", this.Path);
            var element = unattend.XPathSelectElement(this.Path);

            if (element is null) {
                if (this.IsRequired) {
                    this._logger.LogError("The XML element at \"{Path}\" is "
                        + "expected to exist but does not.", this.Path);
                    throw new InvalidOperationException(string.Format(
                        Errors.InexistentXPath, this.Path));
                } else {
                    this._logger.LogWarning("The XML element at \"{Path}\" "
                        + "but does not exist.", this.Path);
                }

            } else {
                this.Apply(element);
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

        #region Protected fields
        /// <summary>
        /// The logger used by the step to record errors and progress messages.
        /// </summary>
        protected readonly ILogger _logger = logger
            ?? throw new ArgumentNullException(nameof(logger));
        #endregion
    }
}
