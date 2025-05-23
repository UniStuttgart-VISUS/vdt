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
using Visus.DeploymentToolkit.Extensions;
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
        /// <param name="state"></param>
        /// <param name="factory"></param>
        /// <param name="logger"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public RunCommand(IState state,
                ICommandBuilderFactory factory,
                ILogger<RunCommand> logger)
                : base(state,logger) {
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
        public string Path { get; set; } = null!;

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
        public override async Task ExecuteAsync(
                CancellationToken cancellationToken) {
            this.Validate();

            var cmd = this._factory.Run(this.Path)
                .WithArguments(this.Arguments)
                .InWorkingDirectory(this.WorkingDirectory)
                .Build();

            this._logger.LogInformation("Running command \"{Command}\".", cmd);

            var exitCode = await cmd.ExecuteAndCheckAsync(
                this.SucccessExitCodes,
                this.FailureExitCodes,
                this._logger);
        }

        #region Private fields
        private readonly ICommandBuilderFactory _factory;
        #endregion
    }
}
