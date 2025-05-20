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
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Services;


namespace Visus.DeploymentToolkit.Tasks {

    /// <summary>
    /// This task copies the Windows PE files to the specified location for
    /// building a new boot image.
    /// </summary>
    [SupportsPhase(Workflow.Phase.PreinstalledEnvironment)]
    public sealed class CopyWindowsPe : TaskBase {

        public CopyWindowsPe(IState state,
                ICopy copy,
                ILogger<CopyWindowsPe> logger)
                : base(state, logger) {
            this._copy = copy ?? throw new ArgumentNullException(nameof(copy));
            this.Name = Resources.CopyWindowsPe;
        }

        #region Public properties
        /// <summary>
        /// Gets or sets the architecture to use for the PE image.
        /// </summary>
        public Architecture Architecture {
            get;
            set;
        } = RuntimeInformation.ProcessArchitecture;

        /// <summary>
        /// Gets or sets the root directory for the deployment tools, most
        /// importantly including the firmware files and the oscdimg tool for
        /// creating ISOs.
        /// </summary>
        public string DeploymentToolsRootDirectory {
            get;
            set;
        } = Path.Combine(ProgrammeFiles,
            "Windows Kits",
            "10",
            "Assessment and Deployment Kit",
            "Deployment Tools");

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
        /// Gets or sets the working directory where the image will be staged.
        /// </summary>
        /// <remarks>
        /// If this property is not set, a temporary directory will be created,
        /// which can be retrieved from this property.
        /// </remarks>
        public string? WorkingDirectory { get; set; }

        /// <summary>
        /// Gets or sets the source location of the WinPE image.
        /// </summary>
        /// <remarks>
        /// If this path is not given, it will be derived from
        /// <see cref="WinPeSourceDirectory"/>.
        /// </remarks>
        public string? WimSourcePath { get; set; }

        /// <summary>
        /// Gets or sets the root directory where the WinPE images are stored in
        /// the Windows Assessment and Deployment Kit (ADK).
        /// </summary>
        public string WinPeSourceDirectory {
            get;
            set;
        } = Path.Combine(ProgrammeFiles,
            "Windows Kits",
            "10",
            "Assessment and Deployment Kit",
            "Windows Preinstallation Environment");

        /// <summary>
        /// Gets the architecture string used in the WinPE paths, which is
        /// derived from <see cref="Architecture"/>.
        /// </summary>
        public string WinPeArchitecture => this.Architecture switch {
            Architecture.X64 => "amd64",
            Architecture.X86 => "x86",
            Architecture.Arm => "arm",
            Architecture.Arm64 => "arm64",
            _ => throw new NotSupportedException(string.Format(
                Errors.UnsupportedArchitecture, this.Architecture))
        };
        #endregion

        #region Public methods
        /// <inheritdoc />
        public override async Task ExecuteAsync(
                CancellationToken cancellationToken) {
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

            this._logger.LogInformation("Creating Windows PE working directory "
                + "\"{WorkingDirectory}\".", this.WorkingDirectory);
            Directory.CreateDirectory(this.WorkingDirectory);

            this._logger.LogTrace("Preparing directory structure in "
                + "\"{WorkingDirectory}\".", this.WorkingDirectory);
            Directory.CreateDirectory(this.FirmwareDirectory);
            Directory.CreateDirectory(this.MediaDirectory);
            Directory.CreateDirectory(this.MountDirectory);

            // Copy the boot files.
            this._logger.LogTrace("Copying boot files ...");
            await this._copy.CopyAsync(this.MediaSourceDirectory,
                this.MediaDirectory,
                CopyFlags.Recursive | CopyFlags.Required)
                .ConfigureAwait(false);

            // Copy the image.
            this._logger.LogTrace("Copying WIM file ...");
            await this._copy.CopyAsync(this.WimSourcePath,
                this.WimPath,
                CopyFlags.Required)
                .ConfigureAwait(false);

            // Copy the firmware files.
            this._logger.LogTrace("Copying firmware files ...");
            await this._copy.CopyAsync(this.EfiSourcePath,
                this.FirmwareDirectory,
                CopyFlags.Required)
                .ConfigureAwait(false);

            await this._copy.CopyAsync(this.BiosSourcePath,
                this.FirmwareDirectory,
                CopyFlags.None)
                .ConfigureAwait(false);
        }
        #endregion

        #region Private properties
        /// <summary>
        /// The folder where we expect the WAIK to be installed.
        /// </summary>
        private static string ProgrammeFiles => Environment.GetFolderPath(
            Environment.SpecialFolder.ProgramFilesX86);

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

        /// <summary>
        /// Gets the directory where the firmware files are copied to.
        /// </summary>
        private string FirmwareDirectory => Path.Combine(
            this.WorkingDirectory!, "fwfiles");

        /// <summary>
        /// Gets the directory where the media files are copied to.
        /// </summary>
        private string MediaDirectory => Path.Combine(
            this.WorkingDirectory!, "media");

        /// <summary>
        /// Gets the directory where the WinPE image will be mounted.
        /// </summary>
        private string MountDirectory => Path.Combine(
            this.WorkingDirectory!, "mount");

        /// <summary>
        /// Gets the path where the WinPE image file will be copied to.
        /// </summary>
        private string WimPath => Path.Combine(
            this.MediaDirectory, "sources", "boot.wim");
        #endregion

        #region Private fields
        private readonly ICopy _copy;
        #endregion
    }
}
