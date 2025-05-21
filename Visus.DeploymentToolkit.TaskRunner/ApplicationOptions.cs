// <copyright file="ApplicationOptions.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Visus.DeploymentToolkit.Tasks;


namespace Visus.DeploymentToolkit {

    /// <summary>
    /// Structured representation of the options used for task runner.
    /// </summary>
    public sealed class ApplicationOptions {

        #region Public properties
        /// <summary>
        /// Gets the path to the log file.
        /// </summary>
        public string LogFile { get; set; } = "deimostaskrunner.log";

        /// <summary>
        /// Gets or sets the path to the state file where the application
        /// obtains persists the current <see cref="Services.IState"/>.
        /// </summary>
        public string StateFile { get; set; } = PersistState.DefaultPath;

        /// <summary>
        /// Gets or sets the name of the task to be executed.
        /// </summary>
        public string Task { get; set; } = null!;
        #endregion
    }
}
