// <copyright file="ClearState.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Services;


namespace Visus.DeploymentToolkit.Tasks {

    /// <summary>
    /// Allows for clearing a state variable.
    /// </summary>
    public sealed class ClearState : TaskBase {

        #region Public constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="state">The current state of the task sequence.</param>
        /// <param name="logger">A logger for progress and error messages.
        /// </param>
        public ClearState(IState state, ILogger<ClearState> logger)
                : base(state, logger) {
            this.Name = Resources.ClearState;
        }
        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets the name of the state variable to clear.
        /// </summary>
        [Required]
        public string Variable { get; set; } = null!;
        #endregion

        #region Public methods
        /// <inheritdoc />
        public override Task ExecuteAsync(CancellationToken cancellationToken) {
            this.Validate();

            cancellationToken.ThrowIfCancellationRequested();
            this._logger.LogInformation("Clearing state {Variable}.",
                this.Variable);
            this._state.Clear(this.Variable);

            return Task.CompletedTask;
        }
        #endregion
    }
}
