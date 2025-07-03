// <copyright file="NextPhase.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Extensions;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Services;
using Visus.DeploymentToolkit.Workflow;


namespace Visus.DeploymentToolkit.Tasks {

    /// <summary>
    /// Transitions the task sequence to the next phase.
    /// </summary>
    public sealed class NextPhase : TaskBase {

        #region Public constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="state">The current state of the task sequence.</param>
        /// <param name="logger">A logger for the task.</param>
        public NextPhase(IState state,
                ICopy copy,
                IDirectory directory,
                ILogger<NextPhase> logger)
                : base(state, logger) {
            this._copy = copy
                ?? throw new ArgumentNullException(nameof(copy));
            this._directory = directory
                ?? throw new ArgumentNullException(nameof(directory));
            this.Name = Resources.NextPhase;
        }
        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets the location where the bootstrapper is located, which
        /// is required when transitioning from <see cref="Phase.Installation"/>
        /// in WinPE to <see cref="Phase.PostInstallation"/> in the deployed
        /// system.
        /// </summary>
        [FromState(WellKnownStates.BootstrapperPath)]
        public string BootstrapperPath { get; set; } = null!;

        /// <summary>
        /// Gets or sets the location where the image has been applied, which is
        /// required to copy the boostrapper in <see cref="Phase.Installation"/>
        /// such that it can be restarted after a reboot in
        /// <see cref="Phase.PostInstallation"/>.
        /// </summary>
        [FromState(WellKnownStates.InstallationDirectory)]
        public string InstallationLocation { get; set; } = null!;
        #endregion

        #region Public methods
        /// <inheritdoc />
        public override Task ExecuteAsync(CancellationToken cancellationToken) {
            this.CopyFrom(this._state);
            this.Validate();
            cancellationToken.ThrowIfCancellationRequested();

            switch (this._state.Phase) {
                case Phase.Bootstrapping:
                    this._state.Phase = Phase.Installation;
                    break;

                case Phase.Installation:
                    this._state.Phase = Phase.PostInstallation;
                    return this.TransitionToPostInstallation();

                case Phase.PrepareImage:
                    this._state.Phase = Phase.CaptureImage;
                    break;

                default:
                    this._logger.LogWarning("There is no obivous transition "
                        + "from {Phase} to another one.", this._state.Phase);
                    break;
            }

            return Task.CompletedTask;
        }
        #endregion

        #region Protected methods
        /// <inheritdoc />
        protected override void Validate() {
            base.Validate();

            if (this._state.Phase == Phase.Installation) {
                // In the installation phase, we need to be able to copy the
                // boostrapper to the target disk.
                if (!File.Exists(this.BootstrapperPath)) {
                    throw new ArgumentException(Errors.NoBootstrapperPath);
                }

                if (!Directory.Exists(this.InstallationLocation)) {
                    throw new ArgumentException(Errors.NoInstallationDirectory);
                }
            }
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Performs additional work that is required after the installation
        /// phase before we can boot into the deployed system.
        /// </summary>
        /// <returns></returns>
        private async Task TransitionToPostInstallation() {
            var src = this._state.BootstrapperPath;
            if ((src = Path.GetDirectoryName(src)) is null) {
                throw new InvalidOperationException(Errors.NoBootstrapperPath);
            }
            this._logger.LogTrace("The bootstrapper is located at {Path}.",
                src);

            var dst = this._state.InstallationDirectory;
            if ((dst = Path.GetPathRoot(dst)) is null) {
                throw new InvalidOperationException(Errors.NoInstallationDrive);
            }
            this._logger.LogTrace("The installation drive is {Path}.", dst);

            dst = Path.Combine(dst, Path.GetFileName(src));

            this._logger.LogInformation("Copying bootstrapper from {Source} to "
                + "{Destination} on the installation disk.", src, dst);
            await this._copy.CopyAsync(src, dst, CopyFlags.Recursive);
        }
        #endregion

        #region Private fields
        private readonly ICopy _copy;
        private readonly IDirectory _directory;
        #endregion
    }
}
