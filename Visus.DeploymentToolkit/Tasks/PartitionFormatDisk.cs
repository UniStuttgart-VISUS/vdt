// <copyright file="PartitionFormatDisk.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.DiskManagement;
using Visus.DeploymentToolkit.Extensions;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Services;
using Visus.DeploymentToolkit.SystemInformation;


namespace Visus.DeploymentToolkit.Tasks {

    /// <summary>
    /// This task partitions and formats disks.
    /// </summary>
    /// <remarks>
    /// <para>Once the task sequence completed successfully, it will report the
    /// location where the system disk is mounted as
    /// <see cref="IState.InstallationDirectory"/> and in case of an EFI system
    /// the system partition as <see cref="IState.BootDrive"/>.</para>
    /// </remarks>
    public sealed class PartitionFormatDisk : TaskBase {

        #region Public constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="diskManagement">The disk management abstraction that
        /// allows the task to access and modify partitions.</param>
        /// <param name="systemInformation">The system information service that
        /// allows the task to find out whether we are running UEFI or BIOS.
        /// </param>
        /// <param name="driveInfo">The drive information service allows the
        /// task assign temporary drive letters to the partitions.</param>
        /// <param name="options">The partitioning options which define the
        /// default partitions being created if no custom scheme was specified
        /// in the task.</param>
        /// <param name="logger">A logger to report progress and problems.
        /// </param>
        /// <exception cref="ArgumentNullException">If
        /// <paramref name="diskManagement"/> is <c>null</c>, or if
        /// <paramref name="logger"/> is <c>null</c>.</exception>
        public PartitionFormatDisk(IState state,
                IDiskManagement diskManagement,
                ISystemInformation systemInformation,
                IDriveInfo driveInfo,
                IOptions<PartitioningOptions> options,
                ILogger<PartitionFormatDisk> logger)
                : base(state, logger) {
            this._diskManagement = diskManagement
                ?? throw new ArgumentNullException(nameof(diskManagement));
            this._driveInfo = driveInfo
                ?? throw new ArgumentNullException(nameof(driveInfo));
            this._options = options?.Value
                ?? throw new ArgumentNullException(nameof(options));
            this._systemInformation = systemInformation
                ?? throw new ArgumentNullException(nameof(systemInformation));
            this.Name = Resources.PartitionFormatDisk;
        }
        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets the flags that control how the disk is cleaned.
        /// </summary>
        public CleanFlags CleanFlags {
            get;
            set;
        } = CleanFlags.Force | CleanFlags.ForceOem | CleanFlags.IgnoreErrors;

        /// <summary>
        /// Gets or sets the disk to work with.
        /// </summary>
        [Required]
        [FromState(WellKnownStates.InstallationDisk)]
        public IDisk Disk { set; get; } = null!;

        /// <summary>
        /// Gets or sets whether this disk should be treated as the installation
        /// disk. For the installation disk, the task will report the location
        /// of the partition to the state.
        /// </summary>
        public bool IsInstallationDisk { get; set; } = true;

        /// <summary>
        /// Gets or sets the description of the disk partitions to be created,
        /// which is basically a description of the desired state of the disk.
        /// </summary>
        public DiskPartitioningDefinition? PartitionScheme { get; set; }

        /// <summary>
        /// Gets the desired partition style to be established on the disk.
        /// This information is obtained from the <see cref="PartitionScheme"/>.
        /// </summary>
        public PartitionStyle PartitionStyle
            => this.PartitionScheme?.PartitionStyle
            ?? PartitionStyle.Unknown;
        #endregion

        #region Public methods
        /// <inheritdoc />
        [SupportedOSPlatform("windows")]
        public override async Task ExecuteAsync(
                CancellationToken cancellationToken) {
            this.CopyFrom(this._state);
            this.Validate();

            if (this.Disk is DiskPartitioningDefinition definition) {
                this._logger.LogInformation("The disk to partition was "
                    + "provided as a definition object which we need to search "
                    + "the hardware for ID {DiskID}.", definition.ID);

                var disks = await this._diskManagement
                    .GetDisksAsync(cancellationToken)
                    .ConfigureAwait(false);

                this.Disk = await this._diskManagement.GetDiskAsync(
                    this.Disk.ID, cancellationToken)
                    ?? throw new ArgumentException(
                        string.Format(Errors.DiskNotFound, definition.ID),
                        nameof(this.Disk));
            }

            if (this.PartitionScheme is null) {
                cancellationToken.ThrowIfCancellationRequested();
                this._logger.LogInformation("No partition scheme provided, so "
                    + "create the default partitioning scheme for {Firmware}.",
                    this._systemInformation.Firmware);
                this.PartitionScheme = this._systemInformation.Firmware switch {
                    FirmwareType.Bios => this.CreateBiosScheme(),
                    FirmwareType.Uefi => this.CreateUefiScheme(),
                    _ => throw new NotSupportedException(string.Format(
                        Errors.UnexpectedFirmware,
                        this._systemInformation.Firmware))
                };
            }
            Debug.Assert(this.PartitionScheme is not null);

            this._logger.LogInformation("Preparing {DiskID} ({Name}) for "
                + "deployment by cleaning everything on it.",
                this.Disk.ID, this.Disk.FriendlyName);
            try {
                await this._diskManagement.CleanAsync(this.Disk,
                    this.CleanFlags, cancellationToken);
            } catch (COMException ex) {
                if (this.CleanFlags.HasFlag(CleanFlags.IgnoreErrors)) {
                    this._logger.LogWarning(ex, "Cleaning disk {DiskID} failed "
                        + "with error code {Hresult}. The error will be "
                        + $"ignored because {CleanFlags.IgnoreErrors} is set.",
                        this.Disk.ID, ex.HResult);
                } else {
                    this._logger.LogError(ex, "Cleaning disk {DiskID} failed "
                        + "with error code {Hresult}. Make sure that the "
                        + "selected disk is not a read-only device.",
                        this.Disk.ID, ex.HResult);
                    throw;
                }
            }

            this._logger.LogTrace("Refreshing disk {DiskID} information after "
                + "cleaning.", this.Disk.ID);
            this.Disk = await this._diskManagement.GetDiskAsync(this.Disk.ID,
                cancellationToken)
                ?? throw new InvalidOperationException(Errors.DiskRefreshFail);

            if (this.Disk.PartitionStyle
                    != this.PartitionScheme.PartitionStyle) {
                this._logger.LogInformation("Making sure that disk {DiskID} has "
                    + "the partition style {RequiredPartitionStyle} (is "
                    + "currently {CurrentPartitionStyle}).", this.Disk.ID,
                    this.PartitionScheme.PartitionStyle, this.Disk.PartitionStyle);
                await this._diskManagement.ConvertAsync(this.Disk,
                    this.PartitionScheme.PartitionStyle,
                    cancellationToken);
            } else {
                this._logger.LogInformation("Disk {DiskID} already has the "
                    + "required partition style {PartitionStyle}.",
                    this.Disk.ID, this.PartitionScheme.PartitionStyle);
            }

            this._logger.LogTrace("Refreshing disk {DiskID} information after "
                + "enforcing partition style {PartitionStyle}.", this.Disk.ID,
                this.PartitionScheme.PartitionStyle);
            this.Disk = await this._diskManagement.GetDiskAsync(this.Disk.ID,
                cancellationToken)
                ?? throw new InvalidOperationException(Errors.DiskRefreshFail);

            this._logger.LogInformation("Creating new partitions on disk "
                + "{DiskID}.", this.Disk.ID);
            var partitions = from p in this.PartitionScheme.Partitions
                             orderby p.Offset
                             select p;
            foreach (var p in partitions) {
                cancellationToken.ThrowIfCancellationRequested();
                await this._diskManagement.CreatePartitionAsync(this.Disk,
                    p, cancellationToken);

                // TODO: format!

                foreach (var m in p.Mounts) {
                    var mountPoint = m.TrimEnd(':',
                        Path.DirectorySeparatorChar,
                        Path.AltDirectorySeparatorChar);

                    //if (mountPoint.Length == 1) {
                    //    this._logger.LogDebug("Assigning drive letter "
                    //        + "{MountPoint} to partition {Partition}.",
                    //        mountPoint, p.Name);
                    //    disk.AssignDriveLetter(p.Offset, mountPoint[0]);
                    //}
                }

                if (this.IsInstallationDisk) {
                    this._logger.LogTrace("Propagating mount points of an "
                        + "installation partition to next tasks.");
                    if (p.IsUsedFor(PartitionUsage.Boot)) {
                        this._state.BootDrive = p.Mounts.First();
                    } else if (p.IsUsedFor(PartitionUsage.Installation)) {
                        this._state.InstallationDirectory = p.Mounts.First();
                    }
                }
            }
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Create the default partitioning scheme for BIOS systems.
        /// </summary>
        /// <returns></returns>
        private DiskPartitioningDefinition CreateBiosScheme() {
            var retval = new DiskPartitioningDefinition {
                ID = this.Disk.ID,
                PartitionStyle = PartitionStyle.Mbr
            };

            // If configured, create a sepearate boot partition.
            if (this._options.BiosSystemReservedSize > 0) {
                retval.Partitions.Add(new PartitionDefinition {
                    Offset = 0,
                    Size = this._options.BiosSystemReservedSize,
                    Name = this._options.BiosSystemReservedLabel,
                    Label = this._options.BiosSystemReservedLabel,
                    FileSystem = FileSystem.Ntfs,
                    Type = PartitionType.Ntfs,
                    Usage = PartitionUsage.Boot
                });
            }

            {
                retval.Partitions.Add(new PartitionDefinition {
                    Offset = 0,
                    Size = 0,
                    Name = this._options.SystemLabel,
                    Label = this._options.SystemLabel,
                    FileSystem = FileSystem.Ntfs,
                    Type = PartitionType.Ntfs,
                    Usage = PartitionUsage.Installation
                });
            }
            

            return retval;
        }

        /// <summary>
        /// Create the default partitioning scheme for UEFI systems.
        /// </summary>
        /// <returns></returns>
        private DiskPartitioningDefinition CreateUefiScheme() {
            Debug.Assert(this.Disk is not null);
            var retval = new DiskPartitioningDefinition {
                ID = this.Disk  .ID,
                PartitionStyle = PartitionStyle.Gpt
            };

            var systemDrive = (from d in this._driveInfo.GetFreeDrives()
                               where d.StartsWith("c", true, null)
                               select d).FirstOrDefault()
                              ?? this._driveInfo.GetFreeDrive();
            var bootDrive = this._driveInfo.GetFreeDrive();

            // Create the EFI system partition.
            {
                retval.Partitions.Add(new PartitionDefinition {
                    Offset = 0,
                    Size = this._options.EfiSize,
                    Name = this._options.EfiLabel,
                    Label = this._options.EfiLabel,
                    FileSystem = FileSystem.Fat32,
                    Type = PartitionType.EfiSystem,
                    Mounts = [bootDrive],
                    Usage = PartitionUsage.Boot | PartitionUsage.System
                });
            }

            // Create the Windows recovery partition if configured.
            if (this._options.RecoverySize > 0) {
                retval.Partitions.Add(new PartitionDefinition {
                    Offset = 0,
                    Size = this._options.RecoverySize,
                    Name = this._options.RecoveryLabel,
                    Label = this._options.RecoveryLabel,
                    Type = PartitionType.WindowsRe
                });
            }

            // Create the OS partition.
            {
                retval.Partitions.Add(new PartitionDefinition {
                    Offset = 0,
                    Size = 0,
                    Name = this._options.SystemLabel,
                    Label = this._options.SystemLabel,
                    FileSystem = FileSystem.Ntfs,
                    Type = PartitionType.MicrosoftBasicData,
                    Mounts = [systemDrive],
                    Usage = PartitionUsage.Installation
                });
            }

            return retval;
        }
        #endregion

        #region Private constants
        /// <summary>
        /// The service's cache has become corrupt.
        /// </summary>
        private const int VDS_E_CACHE_CORRUPT = unchecked((int) 0x80042556);

        /// <summary>
        /// The specified disk is not convertible. CDROMs and DVDs  are examples
        /// of disks that are not convertible.
        /// </summary>
        private const int VDS_E_DISK_NOT_CONVERTIBLE
            = unchecked((int) 0x80042559);
        #endregion

        #region Private fields
        private readonly IDiskManagement _diskManagement;
        private readonly IDriveInfo _driveInfo;
        private readonly PartitioningOptions _options;
        private readonly ISystemInformation _systemInformation;
        #endregion
    }
}
