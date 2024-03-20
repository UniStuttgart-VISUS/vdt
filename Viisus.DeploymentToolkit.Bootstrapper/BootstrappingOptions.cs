// <copyright file="BootstrappingOptions.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>


namespace Visus.DeploymentToolkit.Bootstrapper {

    /// <summary>
    /// Structured representation of the bootstrapping options.
    /// </summary>
    internal sealed class BootstrappingOptions {

        /// <summary>
        /// Gets or sets the name of the domain the deployment server belongs
        /// to.
        /// </summary>
        public string? Domain { get; set; }

        /// <summary>
        /// Gets or sets the UNC path of the deployment share.
        /// </summary>
        public string DeploymentShare { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name of the user to connect to the deployment
        /// share.
        /// </summary>
        public string? User { get; set; }

        /// <summary>
        /// Gets or sets the path to the state file where the bootstrapper
        /// persists the current <see cref="Services.IState"/> before calling
        /// into the agent.
        /// </summary>
        public string StateFile { get; set; } = "deimosstate.json";
    }
}
