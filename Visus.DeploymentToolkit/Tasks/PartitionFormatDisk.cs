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
using System.Threading;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.DiskManagement;
using Visus.DeploymentToolkit.Extensions;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Services;
using Visus.DeploymentToolkit.SystemInformation;
using Visus.DeploymentToolkit.Vds;
using CreateFunc = System.Func<
    Visus.DeploymentToolkit.DiskManagement.IAdvancedDisk,
    Visus.DeploymentToolkit.DiskManagement.PartitionDefinition,
    System.Threading.CancellationToken,
    System.Threading.Tasks.Task>;


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
        #endregion

        #region Public methods
        /// <inheritdoc />
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

            if (!(this.Disk is IAdvancedDisk disk)) {
                throw new ArgumentException(Errors.NoAdvancedDisk);
            }

            if (this.PartitionScheme is null) {
                cancellationToken.ThrowIfCancellationRequested();
                this._logger.LogInformation("No partition scheme provided, so "
                    + "create the default partitioning scheme.");
                this.PartitionScheme = this._systemInformation.Firmware switch {
                    FirmwareType.Bios => this.CreateBiosScheme(disk),
                    FirmwareType.Uefi => this.CreateUefiScheme(disk),
                    _ => throw new NotSupportedException(string.Format(
                        Errors.UnexpectedFirmware,
                        this._systemInformation.Firmware))
                };
            }

            cancellationToken.ThrowIfCancellationRequested();
            this._logger.LogInformation("Cleaning disk {DiskID}.",
                this.Disk.ID);
            try {
                await disk.CleanAsync(this.CleanFlags, cancellationToken);
            } catch (COMException ex) {
                if (this.CleanFlags.HasFlag(CleanFlags.IgnoreErrors)) {
                    this._logger.LogWarning(ex, "Cleaning disk {DiskID} failed "
                        + "with error code {Hresult}. The error will be "
                        + $"ignored bacause {CleanFlags.IgnoreErrors} is set.",
                        this.Disk.ID, ex.HResult);
                } else {
                    this._logger.LogError(ex, "Cleaning disk {DiskID} failed "
                        + "with error code {Hresult}.", this.Disk.ID,
                        ex.HResult);
                    throw;
                }
            }

            cancellationToken.ThrowIfCancellationRequested();
            Debug.Assert(this.PartitionScheme is not null);
            this._logger.LogInformation("Creating new partitions on disk "
                + "{DiskID}.", this.Disk.ID);
            var partitions = from p in this.PartitionScheme.Partitions
                             orderby p.Offset
                             select p;
            CreateFunc create = this.PartitionScheme.PartitionStyle switch {
                    PartitionStyle.Mbr => this.CreateMbrPartition,
                    PartitionStyle.Gpt => this.CreateGptPartition,
                    _ => throw new NotSupportedException(string.Format(
                        Errors.UnsupportedPartitionStyle,
                        this.PartitionScheme.PartitionStyle))
            };

            foreach (var p in partitions) {
                await create(disk, p, cancellationToken).ConfigureAwait(false);

                // TODO: format!

                foreach (var m in p.Mounts) {
                    var mountPoint = m.TrimEnd(':',
                        Path.DirectorySeparatorChar,
                        Path.AltDirectorySeparatorChar);

                    if (mountPoint.Length == 1) {
                        this._logger.LogDebug("Assigning drive letter "
                            + "{MountPoint} to partition {Partition}.",
                            mountPoint, p.Name);
                        disk.AssignDriveLetter(p.Offset, mountPoint[0]);
                    }
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
        /// Adjusts the size of a partition to be a multiple of the sector
        /// size of the gvien <paramref name="disk"/>.
        /// </summary>
        /// <param name="size"></param>
        /// <param name="disk"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private ulong AdjustSize(ulong size, IAdvancedDisk disk) {
            Debug.Assert(disk is not null);
            var remainder = size % disk.SectorSize;
            if (remainder != 0) {
                this._logger.LogTrace("Adjusting partition size {Size} to "
                    + "match the sector size {SectorSize}.",
                    size, disk.SectorSize);
                size += disk.SectorSize - remainder;
            }
            return size;
        }

        /// <summary>
        /// Create the default partitioning scheme for BIOS systems.
        /// </summary>
        /// <param name="disk"></param>
        /// <returns></returns>
        private DiskPartitioningDefinition CreateBiosScheme(
                IAdvancedDisk disk) {
            var retval = new DiskPartitioningDefinition {
                ID = disk.ID,
                PartitionStyle = PartitionStyle.Mbr
            };

            var offset = 0UL;

            // If configured, create a sepearate boot partition.
            if (this._options.BiosSystemReservedSize > 0) {
                var size = this._options.BiosSystemReservedSize;
                size = this.AdjustSize(size, disk);
                retval.Partitions.Add(new PartitionDefinition {
                    Offset = offset,
                    Size = size,
                    Name = this._options.BiosSystemReservedLabel,
                    Label = this._options.BiosSystemReservedLabel,
                    FileSystem = FileSystem.Ntfs,
                    Type = PartitionType.Ntfs,
                    Usage = PartitionUsage.Boot
                });
                offset += retval.Partitions.Last().Size;
            }

            {
                var size = disk.Size - this._options.BiosSystemReservedSize;
                size = this.AdjustSize(size ,disk);
                retval.Partitions.Add(new PartitionDefinition {
                    Offset = offset,
                    Size = size,
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
        /// Creates the specified GPT partition.
        /// </summary>
        /// <param name="partition"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task CreateGptPartition(IAdvancedDisk disk,
                PartitionDefinition partition,
                CancellationToken cancellationToken) {
            Debug.Assert(disk is not null);
            Debug.Assert(partition is not null);

            try {
                var info = new VDS_PARTITION_INFO_GPT {
                    PartitionType = (Guid) partition.Type.Gpt!,
                    PartitionId = Guid.NewGuid(),
                    Name = partition.Name
                };

                this._logger.LogInformation("Creating GPT partition "
                    + "{Partition} of type {Type} with ID {ID} and "
                    + "attributes {Attributes} at offset {Offset} with length "
                    + "{Size}.", info.Name, info.PartitionType.ToString("B"),
                    info.PartitionId.ToString("B"), info.Attributes,
                    partition.Offset, partition.Size);
                await disk.CreatePartitionAsync(partition.Offset,
                    partition.Size,
                    info,
                    cancellationToken).ConfigureAwait(false);
            } catch (COMException ex) {
                this._logger.LogError(ex, "Creating GPT partition {Partition} "
                    + "failed with error code {Hresult}.", partition.Name,
                    ex.HResult);
                throw;
            }
        }

        /// <summary>
        /// Creates the specified MBR partition.
        /// </summary>
        /// <param name="partition"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task CreateMbrPartition(IAdvancedDisk disk,
                PartitionDefinition partition,
                CancellationToken cancellationToken) {
            Debug.Assert(disk is not null);
            Debug.Assert(partition is not null);
            this._logger.LogTrace("Creating GPT partition {Partition} of "
                + "type {Type} at offset {Offset} with length {Size}.",
                partition.Name, partition.Type.Name, partition.Offset,
                partition.Size);

            try {
                await disk.CreatePartitionAsync(partition.Offset,
                    partition.Size,
                    new VDS_PARTITION_INFO_GPT {
                        PartitionType = (Guid) partition.Type.Gpt!,
                        PartitionId = Guid.NewGuid(),
                        Name = partition.Name
                    },
                    cancellationToken).ConfigureAwait(false);
            } catch (COMException ex) {
                this._logger.LogError(ex, "Creating GPT partition {Partition} "
                    + "failed with error code {Hresult}.", partition.Name,
                    ex.HResult);
                throw;
            }
        }

        /// <summary>
        /// Create the default partitioning scheme for UEFI systems.
        /// </summary>
        /// <param name="disk"></param>
        /// <returns></returns>
        private DiskPartitioningDefinition CreateUefiScheme(
                IAdvancedDisk disk) {
            Debug.Assert(disk is not null);
            var retval = new DiskPartitioningDefinition {
                ID = disk.ID,
                PartitionStyle = PartitionStyle.Gpt
            };

            var systemDrive = (from d in this._driveInfo.GetFreeDrives()
                               where d.StartsWith("c", true, null)
                               select d).FirstOrDefault()
                              ?? this._driveInfo.GetFreeDrive();
            var bootDrive = this._driveInfo.GetFreeDrive();

            var offset = 0UL;

            // Create the EFI system partition.
            {
                var size = this._options.EfiSize;
                size = this.AdjustSize(size, disk);
                retval.Partitions.Add(new PartitionDefinition {
                    Offset = offset,
                    Size = size,
                    Name = this._options.EfiLabel,
                    Label = this._options.EfiLabel,
                    FileSystem = FileSystem.Fat32,
                    Type = PartitionType.EfiSystem,
                    Mounts = [bootDrive],
                    Usage = PartitionUsage.Boot | PartitionUsage.System
                });
                offset += retval.Partitions.Last().Size;
            }

            // Create the Windows recovery partition if configured.
            if (this._options.RecoverySize > 0) {
                var size = this._options.RecoverySize;
                size = this.AdjustSize(size, disk);
                retval.Partitions.Add(new PartitionDefinition {
                    Offset = offset,
                    Size = size,
                    Name = this._options.RecoveryLabel,
                    Label = this._options.RecoveryLabel,
                    Type = PartitionType.WindowsRe
                });
                offset += retval.Partitions.Last().Size;
            }

            // Create the OS partition.
            {
                var size = disk.Size - offset;
                size = this.AdjustSize(size, disk);
                retval.Partitions.Add(new PartitionDefinition {
                    Offset = offset,
                    Size = size,
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

        #region Private fields
        private readonly IDiskManagement _diskManagement;
        private readonly IDriveInfo _driveInfo;
        private readonly PartitioningOptions _options;
        private readonly ISystemInformation _systemInformation;
        #endregion
    }
}
