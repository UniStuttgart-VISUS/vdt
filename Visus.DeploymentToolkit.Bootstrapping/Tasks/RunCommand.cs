// <copyright file="RunCommand.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Services;


namespace Visus.DeploymentToolkit.Tasks {

    /// <summary>
    /// A task that runs an external programme or command.
    /// </summary>
    public class RunCommand : TaskBase {

        #region Public constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="logger"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public RunCommand(ICommandBuilderFactory factory,
                ILogger<RunCommand> logger) : base(logger) {
            this._factory = factory
                ?? throw new ArgumentNullException(nameof(factory));
            this.Name = Resources.RunCommand;
        }
        #endregion

        /// <summary>
        /// Gets or sets the argument list for the command.
        /// </summary>
        public string? Arguments { get; set; }

        /// <summary>
        /// Gets or sets the exit codes that indicate a failure.
        /// </summary>
        /// <remarks>
        /// <see cref="SucccessExitCodes"/> and <see cref="FailureExitCodes"/>
        /// work together in the way that if only
        /// <see cref="SucccessExitCodes"/> are set, all other exit codes are
        /// considered a failure, and if only <see cref="FailureExitCodes"/> are
        /// set, all other exit codes are considered a success. If both
        /// properties are set, the behaviour is as if only
        /// <see cref="SucccessExitCodes"/> had been set.
        /// </remarks>
        public int[]? FailureExitCodes { get; set; }

        /// <summary>
        /// Gets or sets the path to the executable to be run.
        /// </summary>
        [Required]
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the working directory for the command.
        /// </summary>
        public string WorkingDirectory { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the exit codes that are considered a success.
        /// </summary>
        /// <remarks>
        /// <see cref="SucccessExitCodes"/> and <see cref="FailureExitCodes"/>
        /// work together in the way that if only
        /// <see cref="SucccessExitCodes"/> are set, all other exit codes are
        /// considered a failure, and if only <see cref="FailureExitCodes"/> are
        /// set, all other exit codes are considered a success. If both
        /// properties are set, the behaviour is as if only
        /// <see cref="SucccessExitCodes"/> had been set.
        /// </remarks>
        public int[]? SucccessExitCodes { get; set; }

        /// <inheritdoc />
        public override async Task ExecuteAsync(IState state,
                CancellationToken cancellationToken) {
            _ = state ?? throw new ArgumentNullException(nameof(state));
            var cmd = this._factory.Run(this.Path)
                .WithArguments(this.Arguments)
                .InWorkingDirectory(this.WorkingDirectory)
                .Build();

            this._logger.LogTrace(Resources.CommandExecuting, cmd);
            var exitCode = await cmd.ExecuteAsync();
            this._logger.LogTrace(Resources.CommandExecuted, cmd, exitCode);

            if (exitCode == null) {
                // For some reasons, the process did not start at all.
                throw new InvalidOperationException(
                    string.Format(Errors.CommandNotRun, cmd.ToString()));

            } else if (this.SucccessExitCodes?.Length > 0) {
                // If the user indicated which error codes are successful, this
                // takes precedence and we fail if any other exit code was
                // returned.
                if (!this.SucccessExitCodes.Contains(exitCode.Value)) {
                    throw new CommandFailedException(cmd.ToString()!,
                        exitCode.Value);
                }

            } else if (this.FailureExitCodes?.Length > 0) {
                // If the user indicated which error codes are a failure, we
                // fail if we encounter any of these exit codes.
                if (this.FailureExitCodes.Contains(exitCode.Value)) {
                    throw new CommandFailedException(cmd.ToString()!,
                        exitCode.Value);
                }
            }
        }

        #region Private fields
        private readonly ICommandBuilderFactory _factory;
        #endregion
    }
}
