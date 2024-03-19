// <copyright file="CommandFailedException.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using Visus.DeploymentToolkit.Properties;


namespace Visus.DeploymentToolkit.Infrastructure {

    /// <summary>
    /// An exception that represents a <see cref="Command"/> being executed, but
    /// indicating failure via its exit code.
    /// </summary>
    internal class CommandFailedException : Exception {

        /// <summary>
        /// Initialsies a new instance.
        /// </summary>
        /// <param name="command">The command that was run.</param>
        /// <param name="exitCode">The exit code of the command.</param>
        public CommandFailedException(string command, int exitCode)
            : base(string.Format(Errors.CommandFailed, command, exitCode)) {
            this.ExitCode = exitCode;
        }

        /// <summary>
        /// Gets the exit code of the command.
        /// </summary>
        public int ExitCode { get; }
    }
}
