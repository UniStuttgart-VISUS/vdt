// <copyright file="OptionsBase.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Visus.DeploymentToolkit.Extensions;
using Visus.DeploymentToolkit.Services;
using Visus.DeploymentToolkit.Tasks;


namespace Visus.DeploymentToolkit.Application {

    /// <summary>
    /// The base class for application options collecting the information
    /// typically required for a Project Deimos application.
    /// </summary>
    public abstract class OptionsBase {

        /// <summary>
        /// Gets or sets the location of the deployment share where the stuff to
        /// be added to the boot image is located.
        /// </summary>
        /// <remarks>
        /// Depending on the type of the application, this might be a local path
        /// to the folder exported as the deployment share or and UNC path to
        /// the share exported by the deployment server.
        /// </remarks>
        [State(WellKnownStates.DeploymentShare)]
        public string? DeploymentShare { get; set; }

        /// <summary>
        /// Gets or sets the path to the state file where the bootstrapper
        /// persists the current <see cref="Services.IState"/> before calling
        /// into the agent.
        /// </summary>
        public string StateFile { get; set; } = PersistState.DefaultPath;
    }
}
