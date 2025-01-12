// <copyright file="BootstrappingOptions.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 - 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>


namespace Visus.DeploymentToolkit {

    /// <summary>
    /// Structured representation of the options used for bootstrapping the
    /// installation.
    /// </summary>
    public sealed class BootstrappingOptions {

        /// <summary>
        /// Gets or sets the path to the agent in the
        /// <see cref="WorkingDirectory"/>.
        /// </summary>
        /// <remarks>
        /// Note that the agent binary is copied to the working directory from
        /// the <see cref="DeploymentShare"/> as part of the bootstrapping.
        /// </remarks>
        public string AgentPath {
            get;
            set;
        } = "Visus.DeploymentToolkit.Agent.exe";

        /// <summary>
        /// Gets or sets the path where the binaries are located relative to
        /// the lcoation of the <see cref="DeploymentShare"/>.
        /// </summary>
        public string BinaryPath { get; set; } = "bin";

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

        /// <summary>
        /// Gets or sets the path to the local working directory where the agent
        /// and the task sequence are copied to.
        /// </summary>
        public string WorkingDirectory { get; set; } = @"\DEIMOS";
    }
}
