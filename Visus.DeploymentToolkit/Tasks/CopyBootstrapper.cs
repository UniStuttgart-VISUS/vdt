// <copyright file="CopyBootstrapper.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.DeploymentShare;
using Visus.DeploymentToolkit.Extensions;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Services;


namespace Visus.DeploymentToolkit.Tasks {

    /// <summary>
    /// This task copies the bootstrapper files to the installation directory
    /// such that is can be scheduled to start after the reboot.
    /// </summary>
    public sealed class CopyBootstrapper : TaskBase {

        #region Public constants
        /// <summary>
        /// Gets the name of the directory that is created on the installation
        /// disk when the destination has not been specified.
        /// </summary>
        public const string DefaultDestination = "__DEIMOS";
        #endregion

        #region Public constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="copy"></param>
        /// <param name="logger"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public CopyBootstrapper(IState state,
                ICopy copy,
                IDirectory directory,
                ILogger<CopyBootstrapper> logger)
                : base(state, logger) {
            this._copy = copy
                ?? throw new ArgumentNullException(nameof(copy));
            this._directory = directory
                ?? throw new ArgumentNullException(nameof(directory));
            this.Name = Resources.CopyBootstrapper;
            this.IsCritical = true;
        }
        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets the destination where the copy is placed.
        /// </summary>
        /// <remarks>
        /// If the destination is not specified, the task will create a
        /// directory on the disk where the
        /// <see cref="IState.InstallationDirectory"/> is located.
        /// </remarks>
        public string? Destination { get; set; }

        /// <summary>
        /// Gets or sets the folder from where the bootstrapper is copied.
        /// </summary>
        /// <remarks>
        /// If the source is not specified, the task will copy it from the
        /// <see cref="IState.DeploymentShare"/>.
        /// </remarks>
        public string? Source { get; set; }
        #endregion

        #region Public methods
        /// <inheritdoc />
        public override async Task ExecuteAsync(
                CancellationToken cancellationToken) {
            this.CopyFrom(this._state);
            this.Validate();

            if (string.IsNullOrWhiteSpace(this.Destination)) {
                if (this._directory.Exists(this._state.InstallationDirectory)) {
                    this._logger.LogTrace("Using installation directory "
                        + "{InstallationDirectory} to derive the destination of "
                        + "the bootstrapper.",
                        this._state.InstallationDirectory);
                    this.Destination = Path.GetPathRoot(
                        this._state.InstallationDirectory)
                        ?? throw new InvalidOperationException(
                            Errors.InstallationDirectoryNotRooted);

                    if (this._state.TryGetNonEmptyString(
                            WellKnownStates.WorkingDirectory,
                            out var d)) {
                        var r = Path.GetPathRoot(d);
                        if (!string.IsNullOrEmpty(r)) {
                            d = d.Remove(0, r.Length);
                        }

                    } else {
                        d = DefaultDestination;
                    }

                    this.Destination = Path.Combine(this.Destination, d);
                    this._logger.LogInformation("Using implicit destination "
                        + "{Destination} for the bootstrapper on the"
                        + " installation disk.", this.Destination);
                } else {
                    throw new InvalidOperationException(
                        Errors.NoInstallationDirectory);
                }
            }

            if (string.IsNullOrWhiteSpace(this.Source)
                    || this._directory.Exists(this.Source)) {
                var share = (Directory.Exists(this._state.DeploymentDirectory)
                    ? this._state.DeploymentDirectory
                    : this._state.DeploymentShare)
                    ?? throw new InvalidOperationException(
                        Errors.InvalidDeploymentShare);
                this.Source = Path.Combine(share, Layout.BootstrapperPath);
                this._logger.LogInformation("Using bootstrapper {Source} from "
                    + "the deployment share {DeploymentShare}", this.Source,
                    share);
            }

            this._logger.LogInformation("Copying bootstrapper from {Source} to "
                + "{Destination}.", this.Source, this.Destination);
            await this._copy.CopyAsync(this.Source,
                this.Destination,
                CopyFlags.Recursive | CopyFlags.Required);
            this._logger.LogInformation("The unattend file was copied to "
                + "{Destination}.", this.Destination);
        }
        #endregion

        #region Private fields
        private readonly ICopy _copy;
        private readonly IDirectory _directory;
        #endregion
    }
}
