// <copyright file="Configuration.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Visus.DeploymentToolkit.Workflow;


namespace Visus.DeploymentToolkit.Agent {

    /// <summary>
    /// Holds the configuration of the agent, which basically determines what it
    /// should do.
    /// </summary>
    /// <remarks>
    /// The configuration is read from the application settings file or from the
    /// command line and most importantly determines in which phase the process
    /// is. In the application settings file, the <see cref="AgentOptions"/>
    /// class maps to the root of the file.
    /// </remarks>
    internal sealed class AgentOptions {

        /// <summary>
        /// Gets or sets the phase in which the agent is currently running.
        /// </summary>
        public Phase Phase { get; set; } = Phase.Unknown;

        /// <summary>
        /// Gets or sets the path of the deployment share, either the UNC path
        /// or the drive letter.
        /// </summary>
        public string DeploymentShare { get; set; } = string.Empty;

        /// <summary>
        /// Gets the path to the log file.
        /// </summary>
        public string LogFile { get; set; } = "deimosagent.log";

        /// <summary>
        /// Gets or sets the path to the state file where the bootstrapper
        /// has persisted the state that is loaded when starting.
        /// </summary>
        public string StateFile { get; set; } = "deimosstate.json";

        /// <summary>
        /// Gets or sets the name of the task sequence to execute.
        /// </summary>
        /// <remarks>
        /// If this option is not set, the agent will ask for what to do.
        /// </remarks>
        public string? TaskSequence { get; set; }

        /// <summary>
        /// Gets or sets the folder relative to the deployment share where the
        /// task sequences are stored.
        /// </summary>
        public string TaskSequenceFolder { get; set; } = "TaskSequences";
    }
}
