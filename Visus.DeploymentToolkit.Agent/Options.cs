// <copyright file="Options.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Visus.DeploymentToolkit.Application;
using Visus.DeploymentToolkit.Extensions;
using Visus.DeploymentToolkit.Services;
using Visus.DeploymentToolkit.Workflow;


namespace Visus.DeploymentToolkit.Agent {

    /// <summary>
    /// Holds the configuration of the agent, which basically determines what it
    /// should do.
    /// </summary>
    /// <remarks>
    /// The configuration is read from the application settings file or from the
    /// command line and most importantly determines in which phase the process
    /// is. In the application settings file, the <see cref="Options"/>
    /// class maps to the root of the file.
    /// </remarks>
    internal sealed class Options : OptionsBase {

        #region Public properties
        /// <summary>
        /// Gets or sets the phase in which the agent is currently running.
        /// </summary>
        [State(WellKnownStates.Phase)]
        public Phase Phase { get; set; } = Phase.Unknown;

        /// <summary>
        /// Gets or sets the progress into the task sequence where the agent
        /// should continue its work, which is something we need to know when
        /// a task sequence requires a reboot.
        /// </summary>
        [State(WellKnownStates.Progress)]
        public int Progress { get; set; } = 0;

        /// <summary>
        /// Gets or sets the name of the task sequence to execute.
        /// </summary>
        /// <remarks>
        /// If this option is not set, the agent will ask for what to do.
        /// </remarks>
        public string? TaskSequence { get; set; }

        /// <summary>
        /// Gets or sets the configuration of the task sequence store.
        /// </summary>
        public TaskSequenceStoreOptions TaskSequenceStore { get; set; } = new();
        #endregion
    }
}
