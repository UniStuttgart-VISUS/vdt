// <copyright file="Options.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Visus.DeploymentToolkit.Application;
using Visus.DeploymentToolkit.Workflow;


namespace Visus.DeploymentToolkit.ImageBuilder {

    /// <summary>
    /// Holds the parameters for the application.
    /// </summary>
    internal sealed class Options : OptionsBase {

        /// <summary>
        /// Gets or sets the configuration of the task sequence store.
        /// </summary>
        public TaskSequenceStoreOptions TaskSequenceStore { get; set; } = new();
    }
}
