// <copyright file="ApplicationOptions.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Runtime.InteropServices;
using Visus.DeploymentToolkit.ImageBuilder.Properties;
using Visus.DeploymentToolkit.Workflow;


namespace Visus.DeploymentToolkit.ImageBuilder {

    /// <summary>
    /// Holds the parameters for the application.
    /// </summary>
    internal sealed class ApplicationOptions {

        /// <summary>
        /// Gets or sets the configuration of the task sequence store.
        /// </summary>
        public TaskSequenceStoreOptions TaskSequenceStore { get; set; } = new();
    }
}
