// <copyright file="RestoreBootConfiguration.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.DiskManagement;
using Visus.DeploymentToolkit.Extensions;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Services;
using Visus.DeploymentToolkit.SystemInformation;
using Visus.DeploymentToolkit.Validation;


namespace Visus.DeploymentToolkit.Tasks {

    /// <summary>
    /// (Re-) Creates the boot configuration of the system.
    /// </summary>
    public sealed class RestoreBootConfiguration : TaskBase {

        #region Public constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="state">The current state of the task sequence.</param>
        /// <param name="bootService">The service for manipulating the boot
        /// sector, the EFI boot partition and the BCD store.</param>
        /// <param name="systemInformation">The system information service that
        /// allows the task to reason about what system it is running on.
        /// </param>
        /// <param name="diskManagement">The disk management service, which
        /// might be required to find the boot drive if it is not specified by
        /// the user.</param>
        /// <param name="driveInfo">A service that allows the task to assign a
        /// free letter to the boot partition.</param>
        /// <param name="logger">A logger for progress and error messages.
        /// </param>
        public RestoreBootConfiguration(IState state,
                IBootService bootService,
                ISystemInformation systemInformation,
                IDiskManagement diskManagement,
                IDriveInfo driveInfo,
                ILogger<RestoreBootConfiguration> logger)
                : base(state, logger) {
            this._bootService = bootService
                ?? throw new ArgumentNullException(nameof(bootService));
            this._diskManagement = diskManagement
                ?? throw new ArgumentNullException(nameof(diskManagement));
            this._driveInfo = driveInfo
                ?? throw new ArgumentNullException(nameof(driveInfo));
            this._systemInformation = systemInformation
                ?? throw new ArgumentNullException(nameof(systemInformation));
            this.Name = Resources.RestoreBootConfiguration;
            this.IsCritical = true;
        }
        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets the EFI system drive if any.
        /// </summary>
        [FromState(WellKnownStates.BootDrive)]
        [FromEnvironment("DEIMOS_BOOT_DRIVE")]
        public string? BootDrive { get; set; }

        /// <summary>
        /// Gets or sets the path where Windows has been installed.
        /// </summary>
        [Required]
        [DirectoryExists]
        [FromState(WellKnownStates.InstallationDirectory)]
        [FromEnvironment("DEIMOS_INSTALLATION_DIRECTORY")]
        public string InstallationDirectory { get; set; } = null!;

        /// <summary>
        /// Gets or sets the version of the NT loader to be installed.
        /// </summary>
        [Required]
        public string Version { get; set; } = "nt60";
        #endregion

        #region Public methods
        /// <inheritdoc />
        public override async Task ExecuteAsync(
                CancellationToken cancellationToken) {
            this.CopyFrom(this._state);
            this.CopyFromEnvironment();
            this.Validate();

            var bootDrive = await this.GetBootDrive(cancellationToken);
            var firmware = this._systemInformation.Firmware;
            var sysDrive = this.GetSystemDrive();

            if (firmware != FirmwareType.Uefi) {
                cancellationToken.ThrowIfCancellationRequested();
                this._logger.LogInformation("Installing a boot sector to"
                    + " {SystemDrive}.", sysDrive);
                await this._bootService.CreateBootsectorAsync(sysDrive,
                    this.Version,
                    firmware);
            }

            this._logger.LogInformation("Cleaning previous boot data.");
            cancellationToken.ThrowIfCancellationRequested();
            await this._bootService.CleanAsync(sysDrive,
                this.Version,
                firmware);

            if (!"nt52".EqualsIgnoreCase(this.Version)) {
                this._logger.LogInformation("Restoring the BCD store on "
                    + " {SystemDrive} for the Windows installation "
                    + "{InstallationDirectory}.", sysDrive,
                    this.InstallationDirectory);
                await this._bootService.CreateBcdStoreAsync(
                    this.InstallationDirectory,
                    sysDrive,
                    firmware);
            }
        }
        #endregion

        #region Private properties.
        /// <summary>
        /// Gets the boot drive of the system
        /// </summary>
        /// <param name="cancellationToken">Allows for cancelling long-runnning
        /// operations.</param>
        private async Task<string> GetBootDrive(
                CancellationToken cancellationToken) {
            if (Directory.Exists(this.BootDrive)) {
                this._logger.LogInformation("Using user-provided boot drive "
                    + "{BootDrive}.", this.BootDrive);
                return this.BootDrive;
            }

            if (this._systemInformation.Firmware == FirmwareType.Bios) {
                var retval = this.GetSystemDrive();
                this._logger.LogTrace("Using system drive {SystemDrive} as "
                    + "boot drive of a BIOS-based system.", retval);
                return retval;

            } else {
                var disks = await this._diskManagement
                    .GetDisksAsync(PartitionType.EfiSystem, cancellationToken)
                    .ConfigureAwait(false);
                this._logger.LogTrace("Found {DiskCount} disks with an EFI "
                    + "system partition.", disks.Count());
                var disk = disks.FirstOrDefault();
                Debug.Assert(disks.Any());

                if (disk is null) {
                    var sysDrive = this.GetSystemDrive();
                    this._logger.LogTrace("There are multiple disks with an "
                        + "EFI system partition in the machine. Search the"
                        + "one that has {InstallationDirectory} (volume "
                        + "{SystemDrive}) on it.",
                        this.InstallationDirectory, sysDrive);
                    disk = (from d in disks
                            where d.Volumes.Any(v => v.Mounts.Any(m => m.EqualsIgnoreCase(sysDrive)))
                            select d).SingleOrDefault();
                }

                if (disk is null) {
                    this._logger.LogTrace("The best candidate for the boot "
                        + "drive could still not be identified. Search a drive "
                        + "that has any Microsoft partition on it.");
                    disk = (from d in disks
                            where d.Partitions.Any(p => p.IsType(PartitionType.AllMicrosoft))
                            select d).FirstOrDefault();
                }

                if (disk is null) {
                    this._logger.LogTrace("The best candidate for the boot "
                        + "drive could still not be identified. Using the "
                        + "first one with an EFI system partition.");
                    disk = disks.First();
                }
                Debug.Assert(disk is not null);

                var retval = (string) null!;
                var (volume, partition) = disk.VolumePartitions.FirstOrDefault(
                    v => v.Item2.IsType(PartitionType.EfiSystem))
                    ?? new(null, null);
                if (volume is null) {
                    this._logger.LogTrace("The boot volume could not be "
                        + "identified via the built-in association.");
                    partition = disk.Partitions.FirstOrDefault(
                        p => p.IsType(PartitionType.EfiSystem));
                    Debug.Assert(partition is not null);

                    //var letter = advDisk.GetDriveLetter(partition.Offset);
                    //if (letter is not null) {
                    //    retval = letter.ToString();
                    //}

                } else {
                    this._logger.LogTrace("Checking whether the boot volume "
                        + "{BootVolume} has a mount point.", volume.Name);
                    retval = volume.Mounts.FirstOrDefault();
                }
                Debug.Assert(partition is not null);

                if (retval is null) {
                    this._logger.LogTrace("No mount point was found so far, so "
                        + "we assign one to the partition {BootPartition}.",
                        partition.Name);
                    retval = this._driveInfo.GetFreeDrive();
                    Debug.Assert(retval?.Any() == true);
                    //advDisk.AssignDriveLetter(partition.Offset, retval[0]);
                }

                this._logger.LogInformation("The boot drive was identified "
                    + "as {BootDrive} on partition {BootPartition} of "
                    + "disk {BootDisk} ({BootDiskName}).", retval,
                    partition.Name, disk.ID, disk.FriendlyName);
                return retval;
            }

            throw new ArgumentException(Errors.NoBootDrive,
                nameof(this.BootDrive));
        }

        /// <summary>
        /// Gets the drive where the system is installed.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private string GetSystemDrive()
            => Path.GetPathRoot(this.InstallationDirectory)
            ?? throw new ArgumentException(Errors.NoSystemDrive,
                nameof(this.InstallationDirectory));
        #endregion

        #region Private fields
        private readonly IBootService _bootService;
        private readonly IDiskManagement _diskManagement;
        private readonly IDriveInfo _driveInfo;
        private readonly ISystemInformation _systemInformation;
        #endregion

    }
}

