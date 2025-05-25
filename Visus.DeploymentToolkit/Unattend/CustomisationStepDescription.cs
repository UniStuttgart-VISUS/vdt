// <copyright file="CustomisationStepDescription.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace Visus.DeploymentToolkit.Unattend {

    /// <summary>
    /// Describes a customisation step for an unattend.xml file.
    /// </summary>
    public sealed class CustomisationStepDescription {

        #region Public properties
        /// <summary>
        /// Gets or sets the properties to be set in the step.
        /// </summary>
        public IDictionary<string, object?> Parameters { get; init; } = null!;

        /// <summary>
        /// Gets or sets the fully qualified type name of the step.
        /// </summary>
        [Required]
        public string Step { get; set; } = null!;
        #endregion

        #region Public methods
        /// <inheritdoc />
        public override string ToString() => this.Step ?? base.ToString()!;
        #endregion
    }
}
