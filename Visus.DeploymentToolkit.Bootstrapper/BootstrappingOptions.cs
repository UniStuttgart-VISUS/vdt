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
        /// Gets or sets the path to the deployment agent relative to the
        /// location of the <see cref="DeploymentShare"/>.
        /// </summary>
        public string AgentPath {
            get;
            set;
        } = @"bin\Visus.DeploymentToolkit.Agent.exe";

        /// <summary>
        /// Gets or sets the drive where the deployment share should be mapped.
        /// </summary>
        /// <remarks>
        /// This property can be <c>null</c>, in which case the programme will
        /// choose a free drive on its own.
        /// </remarks>
        public string? DeploymentDrive { get; set; }

        /// <summary>
        /// Gets or sets the UNC path of the deployment share.
        /// </summary>
        public string DeploymentShare { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name of the domain the deployment server belongs
        /// to.
        /// </summary>
        public string? Domain { get; set; }

        /// <summary>
        /// Gets the path to the log file.
        /// </summary>
        public string LogFile { get; set; } = "deimosbootstrapper.log";

        /// <summary>
        /// Gets or sets the path to the state file where the bootstrapper
        /// persists the current <see cref="Services.IState"/> before calling
        /// into the agent.
        /// </summary>
        public string StateFile { get; set; } = "deimosstate.json";

        /// <summary>
        /// Gets or sets the name of the user to connect to the deployment
        /// share.
        /// </summary>
        public string? User { get; set; }
    }
}
