// <copyright file="ApplicationOptions.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Visus.DeploymentToolkit.Extensions;
using Visus.DeploymentToolkit.Services;
using Visus.DeploymentToolkit.Workflow;


namespace Visus.DeploymentToolkit.ImageBuilder {

    /// <summary>
    /// Holds the parameters for the application.
    /// </summary>
    internal sealed class ApplicationOptions {

        /// <summary>
        /// Gets or sets the location of the deployment share where the stuff to
        /// be added to the boot image is located.
        /// </summary>
        [State(WellKnownStates.DeploymentShare)]
        public string DeploymentShare { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the configuration of the task sequence store.
        /// </summary>
        public TaskSequenceStoreOptions TaskSequenceStore { get; set; } = new();
    }
}
