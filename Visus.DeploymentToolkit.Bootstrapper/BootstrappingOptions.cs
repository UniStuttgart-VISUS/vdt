// <copyright file="BootstrappingOptions.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 - 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.IO;
using Visus.DeploymentToolkit.Extensions;
using Visus.DeploymentToolkit.Services;
using Visus.DeploymentToolkit.Tasks;


namespace Visus.DeploymentToolkit {

    /// <summary>
    /// Structured representation of the options used for bootstrapping the
    /// installation.
    /// </summary>
    public sealed class BootstrappingOptions {

        #region Public properties
        /// <summary>
        /// Gets or sets the path to the agent.
        /// </summary>
        /// <remarks>
        /// See <see cref="IState.AgentPath"/> for further details.
        /// </remarks>
        [State]
        public string AgentPath {
            get;
            set;
        } = @"bin\Visus.DeploymentToolkit.Agent.exe";

        /// <summary>
        /// Gets or sets the path where the binaries are located relative to
        /// the location of the <see cref="DeploymentShare"/>, which is required
        /// to copy the contents of the share to the local disk for later
        /// installation phases.
        /// </summary>
        public string BinaryPath { get; set; } = "bin";

        /// <summary>
        /// Gets or sets the drive where the deployment share should be mapped.
        /// </summary>
        /// <remarks>
        /// This property can be <c>null</c>, in which case the programme will
        /// choose a free drive on its own.
        /// </remarks>
        [State(WellKnownStates.DeploymentDirectory)]
        public string? DeploymentDrive { get; set; }

        /// <summary>
        /// Gets or sets the UNC path of the deployment share.
        /// </summary>
        /// <remarks>
        /// If this property is empty and the <see cref="MountDeploymentShare"/>
        /// task has been configured to be interactive, which is the default, the
        /// task will prompt the user for the share. This property can be used
        /// to preconfigure the input such that users do not have to type it
        /// over and over.
        /// </remarks>
        [State(WellKnownStates.DeploymentShare)]
        public string DeploymentShare { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name of the domain the deployment server belongs
        /// to.
        /// </summary>
        [State(WellKnownStates.DeploymentShareDomain)]
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
        public string StateFile { get; set; } = PersistState.DefaultPath;

        /// <summary>
        /// Gets the path to the <see cref="StateFile"/> in the
        /// <see cref="WorkingDirectory"/>.
        /// </summary>
        [State(WellKnownStates.StateFile)]
        public string StatePath {
            get => Path.Combine(this.WorkingDirectory, this.StateFile);
        }

        /// <summary>
        /// Gets or sets the name of the user to connect to the deployment
        /// share.
        /// </summary>
        [State(WellKnownStates.DeploymentShareUser)]
        public string? User { get; set; }

        /// <summary>
        /// Gets or sets the path to the local working directory where the agent
        /// and the task sequence are copied to.
        /// </summary>
        [State(WellKnownStates.WorkingDirectory)]
        public string WorkingDirectory { get; set; } = @"\DEIMOS";
        #endregion
    }
}
