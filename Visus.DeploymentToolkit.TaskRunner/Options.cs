// <copyright file="Options.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Visus.DeploymentToolkit.Application;


namespace Visus.DeploymentToolkit {

    /// <summary>
    /// Structured representation of the options used for task runner.
    /// </summary>
    public sealed class Options : OptionsBase {

        #region Public properties
        /// <summary>
        /// Gets or sets the name of the task to be executed.
        /// </summary>
        public string Task { get; set; } = null!;
        #endregion
    }
}
