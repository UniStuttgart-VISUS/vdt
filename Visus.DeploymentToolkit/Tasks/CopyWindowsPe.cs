// <copyright file="CopyWindowsPe.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Extensions;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Services;
using Visus.DeploymentToolkit.Validation;


namespace Visus.DeploymentToolkit.Tasks {

    /// <summary>
    /// This task copies the Windows PE files to the specified location for
    /// building a new boot image.
    /// </summary>
    [SupportsPhase(Workflow.Phase.PreinstalledEnvironment)]
    public sealed class CopyWindowsPe : WindowsPeTaskBase {

        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="copy"></param>
        /// <param name="logger"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public CopyWindowsPe(IState state,
                ICopy copy,
                IDirectory directory,
                ILogger<CopyWindowsPe> logger)
                : base(state, logger) {
            this._copy = copy
                ?? throw new ArgumentNullException(nameof(copy));
            this._directory = directory
                ?? throw new ArgumentNullException(nameof(directory));
            this.IsCritical = true;
            this.Name = Resources.CopyWindowsPe;
        }

        #region Public properties
        /// <summary>
        /// Gets or sets the directory where the firmware source files are
        /// stored.
        /// </summary>
        /// <remarks>
        /// If this path is not given, it will be derived from
        /// <see cref="DeploymentToolsRootDirectory"/>.
        /// </remarks>
        public string? FirmwareSourceDirectory { get; set; }

        /// <summary>
        /// Gets or sets the directory where the boot media are located.
        /// </summary>
        /// <remarks>
        /// If this path is not given, it will be derived from
        /// <see cref="WinPeSourceDirectory"/>.
        /// </remarks>
        public string? MediaSourceDirectory { get; set; }

        /// <summary>
        /// Gets or sets the source location of the WinPE image.
        /// </summary>
        /// <remarks>
        /// If this path is not given, it will be derived from
        /// <see cref="WinPeSourceDirectory"/>.
        /// </remarks>
        [FileExists]
        public string? WimSourcePath { get; set; }
        #endregion

        #region Public methods
        /// <inheritdoc />
        public override async Task ExecuteAsync(
                CancellationToken cancellationToken) {
            this.CopyFrom(this._state);

            if (string.IsNullOrEmpty(this.WorkingDirectory)) {
                this.WorkingDirectory = Path.Combine(Path.GetTempPath(),
                    Path.GetRandomFileName());
            }

            if (string.IsNullOrEmpty(this.FirmwareSourceDirectory)) {
                this.FirmwareSourceDirectory = Path.Combine(
                    this.DeploymentToolsRootDirectory,
                    this.WinPeArchitecture,
                    "Oscdimg");
            }

            if (string.IsNullOrEmpty(this.MediaSourceDirectory)) {
                this.MediaSourceDirectory = Path.Combine(
                    this.WinPeSourceDirectory,
                    this.WinPeArchitecture,
                    "Media");
            }

            if (string.IsNullOrEmpty(this.WimSourcePath)) {
                this.WimSourcePath = Path.Combine(
                    this.WinPeSourceDirectory,
                    this.WinPeArchitecture,
                    "en-us",
                    "winpe.wim");
            }

            this.Validate();

            this._logger.LogInformation("Creating Windows PE working directory "
                + "{WorkingDirectory}.", this.WorkingDirectory);
            await this._directory.CreateAsync(this.WorkingDirectory)
                .ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();

            this._logger.LogTrace("Preparing directory structure in "
                + "{WorkingDirectory}.", this.WorkingDirectory);
            await this._directory.CreateAsync(this.FirmwareDirectory)
                .ConfigureAwait(false);
            await this._directory.CreateAsync(this.MediaDirectory)
                .ConfigureAwait(false);
            await this._directory.CreateAsync(this.MountDirectory)
                .ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();

            // Copy the boot files.
            this._logger.LogTrace("Copying boot files ...");
            await this._copy.CopyAsync(this.MediaSourceDirectory,
                this.MediaDirectory,
                CopyFlags.Recursive | CopyFlags.Required)
                .ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();

            // Copy the image.
            this._logger.LogTrace("Copying WIM file ...");
            await this._copy.CopyAsync(this.WimSourcePath,
                this.WimPath,
                CopyFlags.Required)
                .ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();

            // Copy the firmware files.
            this._logger.LogTrace("Copying firmware files ...");
            await this._copy.CopyAsync(this.EfiSourcePath,
                this.FirmwareDirectory,
                CopyFlags.Required)
                .ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();

            await this._copy.CopyAsync(this.BiosSourcePath,
                this.FirmwareDirectory,
                CopyFlags.None)
                .ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();

            // Tell subsequent tasks which kind of architecture we have copied.
            this._state.Architecture = this.Architecture;

            // Persist the working directory for subsequent tasks.
            this._state.WorkingDirectory = this.WorkingDirectory;

            // Persist the information required for mounting the image.
            this._state[nameof(MountWim.ImagePath)] = this.WimPath;
            this._state[nameof(MountWim.MountPoint)] = this.MountDirectory;
        }
        #endregion

        #region Private properties
        /// <summary>
        /// Gets the location where the BIOS firmware is located.
        /// </summary>
        private string BiosSourcePath => Path.Combine(
            this.FirmwareSourceDirectory!, "etfsboot.com");

        /// <summary>
        /// Gets the location where the EFI firmware is located.
        /// </summary>
        private string EfiSourcePath => Path.Combine(
            this.FirmwareSourceDirectory!, "efisys.bin");
        #endregion

        #region Private fields
        private readonly ICopy _copy;
        private readonly IDirectory _directory;
        #endregion
    }
}
