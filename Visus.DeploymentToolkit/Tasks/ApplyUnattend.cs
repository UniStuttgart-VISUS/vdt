// <copyright file="ApplyUnattend.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 - 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Services;
using Visus.DeploymentToolkit.Validation;


namespace Visus.DeploymentToolkit.Tasks {

    /// <summary>
    /// Applies an unattend file to a mounted Windows installation.
    /// </summary>
    public sealed class ApplyUnattend : ImageServicingTaskBase {

        #region Public constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="state">The current state of the task sequence.</param>
        /// <param name="imageServicing">A new image servicing implementation
        /// to perform the work.</param>
        /// <param name="logger">The logger used to report results of the
        /// operation.</param>
        public ApplyUnattend(IState state,
                IImageServicing imageServicing,
                ILogger<ApplyUnattend> logger)
                : base(state,imageServicing, logger) {
            this.Name = Resources.ApplyUnattend;
            this.IsCritical = true;
        }
        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets the path to the unattend file to apply.
        /// </summary>
        [FileExists]
        public string UnattendFile { get; set; } = null!;
        #endregion

        #region Public methods
        /// <inheritdoc />
        public override Task ExecuteAsync(CancellationToken cancellationToken) {
            this.Validate();

            return Task.Run(() => {
                try {
                    this._logger.LogInformation("Applying unattend file {Path} "
                        + "to {Image}.", this.UnattendFile,
                        this.EffectiveInstallationPath);
                    this.Open().ApplyUnattend(this.UnattendFile);
                    this._logger.LogInformation("Unattend file {Path} "
                        + "applied successfully to image {Image}.",
                        this.UnattendFile, this.EffectiveInstallationPath);
                } finally {
                    this.Close();
                }
            }, cancellationToken);
        }
        #endregion
    }
}
