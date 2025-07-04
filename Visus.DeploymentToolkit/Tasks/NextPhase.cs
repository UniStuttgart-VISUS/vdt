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
    /// <remarks>
    /// <para>The task might do nothing if there is no obvious next phase to
    /// <see cref="IState.Phase"/>.</para>
    /// <para>The task also prepares certain changes to the state for
    /// transitions that require a reboot. For instance, when transitioning from
    /// <see cref="Phase.Installation"/> to
    /// <see cref="Phase.PostInstallation"/>, the task makes sure that the
    /// bootstrapper is copied to the installation drive and can be executed
    /// from there in order to continue the active task sequence after a reboot.
    /// </para>
    /// </remarks>
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
            this._state.Phase = Phase.PostInstallation;

            // We need to know the bootstrapper executable after reboot such
            // that we can continue the task sequence in the deployed system. As
            // the entry assembly is the DLL and not the executable, we make
            // sure that we register the executable here as we want to call it
            // directly after the reboot.
            var bootstrapper = this._state.BootstrapperPath;
            bootstrapper = Path.GetFileName(bootstrapper);
            bootstrapper = Path.ChangeExtension(bootstrapper, ".exe");
            this._logger.LogTrace("The bootstrapper executable is {Path}.",
                bootstrapper);

            // Find out where the bootstrapper is located in WinPE and copy the
            // whole directory to the installation disk. The directory we create
            // there will be the new working directory after the reboot.
            var src = this._state.BootstrapperPath;
            if ((src = Path.GetDirectoryName(src)) is null) {
                throw new InvalidOperationException(Errors.NoBootstrapperPath);
            }
            this._logger.LogTrace("The bootstrapper is located at {Path}.",
                src);

            var dst = this._state.InstallationDrive;
            if (dst is null) {
                throw new InvalidOperationException(Errors.NoInstallationDrive);
            }
            this._logger.LogTrace("The installation drive is {Path}.", dst);

            var workDir = this._state.WorkingDirectory
                ?? Environment.CurrentDirectory;
            if ((workDir = Path.GetFileName(workDir)) is null) {
                throw new InvalidOperationException(Errors.NoWorkingDirectory);
            }

            // Construct the new working directory on the installation disk and
            // perform the copy operation.
            dst = Path.Combine(dst, workDir);

            this._logger.LogInformation("Copying bootstrapper from {Source} to "
                + "{Destination} on the installation disk.", src, dst);
            await this._copy.CopyAsync(src, dst, CopyFlags.Recursive);

            this._logger.LogInformation("Copying agent files from working "
                + "directory {Source} to {Destination} on the installation"
                + " disk.", Environment.CurrentDirectory, dst);
            await this._copy.CopyAsync(Environment.CurrentDirectory, dst,
                CopyFlags.Recursive);

            // After the reboot, the bootstrapper needs to restore the state
            // from the state file, i.e. we need to make sure that the latest
            // state is saved to the installation disk. We do that by switching
            // the state file to a relative path and changing the working
            // directory to the copy on the installation disk, which means that
            // the state will be saved there from now on.
            var state = Path.GetFileName(this._state.StateFile)
                ?? PersistState.DefaultPath;
            state = Path.Combine(dst, state);
            this._logger.LogInformation("Move state to installation drive "
                + "{Destination}.", state);

            this._logger.LogTrace("Changing to working directory on "
                + "installation disk {Path}.", dst);
            Environment.CurrentDirectory = dst;

            // We expect the drive letter of the installation disk to change on
            // reboot, so we remove it from the path and only provide a path
            // relative to the root of the installation disk.
            dst = dst.Substring(Path.GetPathRoot(dst)?.Length ?? 0);
            dst = Path.Combine($"{Path.DirectorySeparatorChar}", dst);

            this._logger.LogInformation("Registering bootstrapper with its new "
                + "location after reboot.");
            this._state.BootstrapperPath = Path.Combine(dst, bootstrapper!);

            // As for the state file, make the location of the installation
            // location relative to the installation disk.
            this._state.InstallationDirectory = $"{Path.DirectorySeparatorChar}";

            //if (this._state.TaskSequence is ITaskSequence ts) {
            //    this._logger.LogTrace("Reverting the task sequence to its ID "
            //        + "{ID}.", ts.ID);
            //    this._state.TaskSequence = ts.ID;
            //}

            await this._state.SaveAsync(this._state.StateFile);
        }
        #endregion

        #region Private fields
        private readonly ICopy _copy;
        private readonly IDirectory _directory;
        #endregion
    }
}
