// <copyright file="Delay.cs" company="Visualisierungsinstitut der Universität Stuttgart">
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
    /// A task that delays execution for a specified time.
    /// </summary>
    public sealed class Delay : TaskBase {

        #region Public constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="state">The current state of the task sequence.</param>
        /// <param name="logger">A logger for progress and error messages.
        /// </param>
        public Delay(IState state,
                ILogger<CopyFiles> logger)
                : base(state, logger) {
            this.Name = Resources.Delay;
        }
        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets the duration of the delay.
        /// </summary>
        [Required]
        public TimeSpan Duration { get; set; }

        /// <summary>
        /// Gers or sets the optional reason for the delay.
        /// </summary>
        public string? Reason { get; set; }
        #endregion

        #region Public methods
        /// <inheritdoc />
        public override Task ExecuteAsync(CancellationToken cancellationToken) {
            if (string.IsNullOrWhiteSpace(this.Reason)) {
                this._logger.LogInformation("Delaying for {Duration}.",
                    this.Duration);
            } else {
                this._logger.LogInformation("Delaying for {Duration} because "
                    + "\"{Reason}\".", this.Duration, this.Reason);
            }

            return Task.Delay(this.Duration, cancellationToken);
        }
        #endregion
    }
}
