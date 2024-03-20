// <copyright file="ICommand.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Collections.Generic;
using System.Threading.Tasks;


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// The interface for a command or process that can be executed to run
    /// externally.
    /// </summary>
    public interface ICommand {

        #region Public properties
        /// <summary>
        /// Gets the arguments passed to the command.
        /// </summary>
        IEnumerable<string> Arguments { get; }

        /// <summary>
        /// Gets the path to the file to be executed.
        /// </summary>
        string Path { get; }

        /// <summary>
        /// Gets the working directory to run the command in.
        /// </summary>
        string WorkingDirectory { get; }
        #endregion

        #region Public methods
        /// <summary>
        /// Execute the command and answer its exit code.
        /// </summary>
        /// <returns>A task for waiting for the exit code. The exit code
        /// might be <c>null</c> if the process could not be started.</returns>
        Task<int?> ExecuteAsync();
        #endregion
    }
}
