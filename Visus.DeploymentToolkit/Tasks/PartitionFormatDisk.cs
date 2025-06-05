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
using Visus.DeploymentToolkit.Vds;


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

            cancellationToken.ThrowIfCancellationRequested();
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

            //// The documentation at https://learn.microsoft.com/en-us/windows/win32/api/vds/nf-vds-ivdspack-adddisk
            //// suggests implicitly that an uninitialised disk cannot be used
            //// unless it has been added to a pack: "To undo the effect of this
            //// method — that is, to remove the partitioning format and cause
            //// the disk to be a raw disk that is owned by the VDS service — use
            //// the IVdsAdvancedDisk::Clean method."
            //if (this._diskManagement is VdsService vds) {
            //    cancellationToken.ThrowIfCancellationRequested();
            //    this._logger.LogInformation("Adding disk {DiskID} to the VDS "
            //        + "service.", this.Disk.ID);
            //    var packs = await vds.GetPacksAsync(cancellationToken);

            //    foreach (var p in packs) {
            //        try {
            //            // TODO: the service complains about corrupted cache data, but refreshing it like this does not help ...
            //            //await vds.RefreshAsync(true, cancellationToken);
            //            //var id = this.Disk.ID;
            //            //this.Disk = await this._diskManagement.GetDiskAsync(
            //            //    id, cancellationToken)
            //            //    ?? throw new InvalidOperationException(
            //            //        string.Format(Errors.DiskLost, id));

            //            // Find out whether the pack we have here is acceptable,
            //            // which means that it is online...
            //            cancellationToken.ThrowIfCancellationRequested();
            //            p.GetProperties(out var props);
            //            this._logger.LogTrace("Considering pack {PackID}.",
            //                props.Id);
            //            if (props.Status != VDS_PACK_STATUS.ONLINE) {
            //                this._logger.LogTrace("Pack {PackID} is not "
            //                    + "online.", props.Id);
            //                continue;
            //            }

            //            // Try adding the disk to the pack. If that succeeded,
            //            // we are done. Otherwise, we will take note of the
            //            // failure and continue with the next pack.
            //            p.AddDisk(this.Disk.ID,
            //                (VDS_PARTITION_STYLE) this.PartitionStyle,
            //                false);
            //        } catch (COMException ex) {
            //            this._logger.LogWarning(ex, "The disk {DiskID} could "
            //                + "not be added to a pack. This might be "
            //                + "acceptable if the disk can be added to another "
            //                + "pack or is already part of a pack and can be "
            //                + "converted to the desired partition style in the "
            //                + "next step.", this.Disk.ID);
            //        }
            //    } /* foreach (var p in packs) */
            //} /* if (this._diskManagement is VdsService vds) */

            //cancellationToken.ThrowIfCancellationRequested();
            //this._logger.LogInformation("Making sure that disk {DiskID} has "
            //    + "the partition style {RequiredPartitionStyle} (is "
            //    + "currently {CurrentPartitionStyle}).", this.Disk.ID,
            //    this.PartitionScheme.PartitionStyle, this.Disk.PartitionStyle);
            //try {
            //    await disk.ConvertAsync(this.PartitionScheme.PartitionStyle);
            //} catch (COMException ex) when (ex.HResult == VDS_E_DISK_NOT_CONVERTIBLE) {
            //    if (this.PartitionStyle != this.Disk.PartitionStyle) {
            //        this._logger.LogError(ex, "Disk {DiskID} is not "
            //            + "convertible. Make sure that the selected disk is "
            //            + "not a read-only device. The task cannot continue "
            //            + "because the disk does not yet already have the "
            //            + "required partition style {RequiredPartitionStyle}."
            //            , this.Disk.ID, this.PartitionScheme.PartitionStyle);
            //        throw;
            //    } else {
            //        this._logger.LogWarning(ex, "Disk {DiskID} is not "
            //            + "convertible, however the disk already had the "
            //            + "required partition style.", this.Disk.ID);
            //    }
            //}

            cancellationToken.ThrowIfCancellationRequested();
            Debug.Assert(this.PartitionScheme is not null);
            this._logger.LogInformation("Creating new partitions on disk "
                + "{DiskID}.", this.Disk.ID);
            var partitions = from p in this.PartitionScheme.Partitions
                             orderby p.Offset
                             select p;
            foreach (var p in partitions) {
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
        /// Adjusts the size of a partition to be a multiple of the sector
        /// size of the configured <see cref="Disk"/>.
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private ulong AdjustSize(ulong size) {
            Debug.Assert(this.Disk is not null);
            var remainder = size % this.Disk.SectorSize;
            if (remainder != 0) {
                this._logger.LogTrace("Adjusting partition size {Size} to "
                    + "match the sector size {SectorSize}.",
                    size, this.Disk.SectorSize);
                size += this.Disk.SectorSize - remainder;
            }
            return size;
        }

        /// <summary>
        /// Create the default partitioning scheme for BIOS systems.
        /// </summary>
        /// <returns></returns>
        private DiskPartitioningDefinition CreateBiosScheme() {
            var retval = new DiskPartitioningDefinition {
                ID = this.Disk.ID,
                PartitionStyle = PartitionStyle.Mbr
            };

            var offset = 0UL;

            // If configured, create a sepearate boot partition.
            if (this._options.BiosSystemReservedSize > 0) {
                var size = this._options.BiosSystemReservedSize;
                size = this.AdjustSize(size);
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
                var size = this.Disk.Size
                    - this._options.BiosSystemReservedSize;
                size = this.AdjustSize(size);
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

            var offset = 0UL;

            // Create the EFI system partition.
            {
                var size = this._options.EfiSize;
                size = this.AdjustSize(size);
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
                size = this.AdjustSize(size);
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
                var size = this.Disk.Size - offset;
                size = this.AdjustSize(size);
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
