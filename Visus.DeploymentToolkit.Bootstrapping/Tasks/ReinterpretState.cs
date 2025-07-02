// <copyright file="ReinterpretState.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Extensions;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Services;


namespace Visus.DeploymentToolkit.Tasks {

    /// <summary>
    /// Allows for coyping data from one state variable to another.
    /// </summary>
    public sealed class ReinterpretState : TaskBase {

        #region Public constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="state">The current state of the task sequence.</param>
        /// <param name="logger">A logger for progress and error messages.
        /// </param>
        public ReinterpretState(IState state,
                ILogger<ReinterpretState> logger)
                : base(state, logger) {
            this.Name = Resources.ReinterpretState;
        }
        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets whether <see cref="Destination"/> can be cleared if
        /// <see cref="Source"/> is not set. Otherwise, the destination will
        /// remain untouched in such a case.
        /// </summary>
        public bool AllowClear { get; set; } = false;

        /// <summary>
        /// Gets or sets the name of the destintation variable that will receive
        /// the state from <see cref="Source"/>.
        /// </summary>
        [Required]
        public string Destination { get; set; } = null!;

        /// <summary>
        /// Gets or  sets the name of the source variable.
        /// </summary>
        [Required]
        public string Source { get; set; } = null!;

        /// <summary>
        /// Gets or sets whether <see cref="Source"/> not existing in the state
        /// is an error.
        /// </summary>
        public bool SourceMustExist { get; set; } = true;
        #endregion

        #region Public methods
        /// <inheritdoc />
        public override Task ExecuteAsync(CancellationToken cancellationToken) {
            this.Validate();

            if (this._state.TryGetValue(this.Source, out object? source)) {
                cancellationToken.ThrowIfCancellationRequested();
                this._logger.LogInformation("Copying state from {Source} to "
                    + "{Destination}.", this.Source, this.Destination);
                this._state[this.Destination] = source;

            } else {
                if (this.SourceMustExist) {
                    throw new InvalidOperationException(string.Format(
                        Errors.StateNotFound, this.Source));
                }

                if (this.AllowClear) {
                    cancellationToken.ThrowIfCancellationRequested();
                    this._logger.LogInformation("Clearing state {Destination}.",
                        this.Destination);
                    this._state[this.Destination] = null;
                }
            }

            return Task.CompletedTask;
        }
        #endregion
    }
}
