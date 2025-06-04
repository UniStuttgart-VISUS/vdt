// <copyright file="SetEnvironmentVariable.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Services;


namespace Visus.DeploymentToolkit.Tasks {

    /// <summary>
    /// Sets an environment variable.
    /// </summary>
    public class SetEnvironmentVariable : TaskBase {

        #region Public constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="state">The current state of the task sequence.</param>
        /// <param name="environment">The environment service which allows the
        /// task to access environment variables.</param>
        /// <param name="logger">A logger for progress and error messages.
        /// </param>
        /// <exception cref="ArgumentNullException">If any of the arguments is
        /// <see langword="null"/>.</exception>
        public SetEnvironmentVariable(IState state,
                IEnvironment environment,
                ILogger<SetEnvironmentVariable> logger)
                : base(state,logger) {
            this._environment = environment
                ?? throw new ArgumentNullException(nameof(environment));
            this.Name = Resources.SetEnvironmentVariable;
        }
        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets whether the variable should only be set if it does not
        /// exist yet.
        /// </summary>
        public bool IsNoOverwrite { get; set; } = false;

        /// <summary>
        /// Gets or set the value to be set.
        /// </summary>
        /// <remarks>
        /// If this is <see langword="null"/>, the environment variable will be
        /// deleted. Make sure to se <see cref="IsNoOverwrite"/> to
        /// <see langword="false"/> if you want to delete an existing variable.
        /// </remarks>
        public string? Value { get; set; }

        /// <summary>
        /// Gets or sets the name of the environment variable to modify.
        /// </summary>
        [Required]
        public string Variable { get; set; } = null!;
        #endregion

        #region Public methods
        /// <inheritdoc />
        public override Task ExecuteAsync(
                CancellationToken cancellationToken) {
            this.Validate();

            var exists = (this._environment[this.Variable] is not null);
            this._logger.LogTrace("Modifying the {Existence} environment "
                + "variable {Variable}.",
                exists ? "existing" : "non-existing",
                this.Variable);

            if (this.IsNoOverwrite && exists) {
                this._logger.LogInformation("Leaving the environment variable "
                    + "{Variable} untouched, because it already exists.",
                    this.Variable);
            } else {
                this._logger.LogInformation("Setting the environment variable "
                    + "{Variable} to {Value}.", this.Variable,
                    this.Value);
                this._environment[this.Variable] = this.Value;
            }

            return Task.CompletedTask;
        }
        #endregion

        #region Private fields
        private readonly IEnvironment _environment;
        #endregion
    }
}
