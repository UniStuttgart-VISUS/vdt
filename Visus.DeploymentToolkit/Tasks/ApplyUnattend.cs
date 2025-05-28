// <copyright file="ApplyUnattend.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 - 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Services;
using Visus.DeploymentToolkit.Validation;


namespace Visus.DeploymentToolkit.Tasks {

    /// <summary>
    /// Applies an unattend file to a mounted Windows installation.
    /// </summary>
    internal sealed class ApplyUnattend : TaskBase {

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
                : base(state,logger) {
            this._imageServicing = imageServicing
                ?? throw new ArgumentNullException(nameof(imageServicing));
        }
        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets the path to the Windows installation being modified.
        /// </summary>
        /// <remarks>
        /// If this path is <c>null</c>, an online servicing session for the
        /// current Windows installation is opened.
        /// </remarks>
        public string? InstallationPath { get; set; }

        /// <summary>
        /// Gets or sets the path to the unattend file to apply.
        /// </summary>
        [FileExists]
        public string UnattendFile { get; set; } = null!;
        #endregion

        #region Public methods
        /// <inheritdoc />
        public override Task ExecuteAsync(CancellationToken cancellationToken) {
            if (this.InstallationPath is null) {
                this._logger.LogTrace("No installation path was set, so we "
                    + "consider any mounted WIM image a candidate.");
                this.InstallationPath = this._state.WimMount?.MountPoint;
            }

            this._logger.LogInformation("Opening a DISM servicing session "
                + "for \"{Image}\" to apply unattend settings.",
                this.InstallationPath);
            this._imageServicing.Open(this.InstallationPath);

            this._logger.LogInformation("Applying unattend file "
                + "\"{UnattendFile}\" to \"{Image}\".",
                this.UnattendFile, this._imageServicing.Name);
            this._imageServicing.ApplyUnattend(this.UnattendFile);

            this._logger.LogTrace("Committing changes to \"{Image}\".",
                this._imageServicing.Name);
            this._imageServicing.Commit();

            return Task.CompletedTask;
        }
        #endregion

        #region Private fields
        private readonly IImageServicing _imageServicing;
        #endregion
    }
}
