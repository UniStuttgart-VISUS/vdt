// <copyright file="PartitionFormatDisk.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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

        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="diskManagement">The disk management abstraction that
        /// allows the task to access and modify partitions.</param>
        /// <param name="systemInformation">The system information service that
        /// allows the task to find out whether we are running UEFI or BIOS.
        /// </param>
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
                IOptions<PartitioningOptions> options,
                ILogger<PartitionFormatDisk> logger)
                : base(state, logger) {
            this._diskManagement = diskManagement
                ?? throw new ArgumentNullException(nameof(diskManagement));
            this._options = options?.Value
                ?? throw new ArgumentNullException(nameof(options));
            this._systemInformation = systemInformation
                ?? throw new ArgumentNullException(nameof(systemInformation));
            this.Name = Resources.PartitionFormatDisk;
        }

        #region Public properties
        /// <summary>
        /// Gets or sets the disk to work with.
        /// </summary>
        [Required]
        [FromState(WellKnownStates.InstallationDisk)]
        public IDisk Disk { set; get; } = null!;

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
                throw new ArgumentException(Errors.UnsupportedInstallationDisk);
            }

            if (this.PartitionScheme is null) {
                cancellationToken.ThrowIfCancellationRequested();
                this._logger.LogInformation("No partition scheme provided, so "
                    + "create the default partitioning scheme.");
                this.PartitionScheme = this._systemInformation.Firmware switch {
                    FirmwareType.Bios => this.CreateBiosScheme(this.Disk),
                    FirmwareType.Uefi => this.CreateUefiScheme(this.Disk),
                    _ => throw new NotSupportedException(string.Format(
                        Errors.UnexpectedFirmware,
                        this._systemInformation.Firmware))
                };
            }

            cancellationToken.ThrowIfCancellationRequested();

            //this._diskManagement.
            throw new NotImplementedException("TODO: implement disk partitioning steps");
        }
        #endregion


        #region Private methods
        /// <summary>
        /// Create the default partitioning scheme for BIOS systems.
        /// </summary>
        /// <param name="disk"></param>
        /// <returns></returns>
        private DiskPartitioningDefinition CreateBiosScheme(IDisk disk) {
            var retval = new DiskPartitioningDefinition {
                ID = disk.ID,
                PartitionStyle = PartitionStyle.Mbr
            };

            if (this._options.BiosSystemReservedSize > 0) {
                retval.Partitions.Add(new PartitionDefinition {
                    Size = this._options.BiosSystemReservedSize,
                    Name = this._options.BiosSystemReservedLabel,
                    Label = this._options.BiosSystemReservedLabel,
                    FileSystem = FileSystem.Ntfs,
                    Type = PartitionType.Ntfs
                });
            }

            retval.Partitions.Add(new PartitionDefinition {
                Offset = this._options.BiosSystemReservedSize,
                Size = disk.Size - this._options.BiosSystemReservedSize,
                Name = this._options.SystemLabel,
                Label = this._options.SystemLabel,
                FileSystem = FileSystem.Ntfs,
                Type = PartitionType.Ntfs
            });

            return retval;
        }

        /// <summary>
        /// Create the default partitioning scheme for UEFI systems.
        /// </summary>
        /// <param name="disk"></param>
        /// <returns></returns>
        private DiskPartitioningDefinition CreateUefiScheme(IDisk disk) {
            var retval = new DiskPartitioningDefinition {
                ID = disk.ID,
                PartitionStyle = PartitionStyle.Gpt
            };

            var offset = 0UL;

            retval.Partitions.Add(new PartitionDefinition {
                Offset = offset,
                Size = this._options.EfiSize,
                Name = this._options.EfiLabel,
                Label = this._options.EfiLabel,
                FileSystem = FileSystem.Fat32,
                Type = PartitionType.EfiSystem
            });
            offset += retval.Partitions.Last().Size;

            if (this._options.RecoverySize > 0) {
                retval.Partitions.Add(new PartitionDefinition {
                    Offset = offset,
                    Size = this._options.RecoverySize,
                    Name = this._options.RecoveryLabel,
                    Label = this._options.RecoveryLabel,
                    Type = PartitionType.WindowsRe
                });
                offset += retval.Partitions.Last().Size;
            }

            retval.Partitions.Add(new PartitionDefinition {
                Offset = offset,
                Size = disk.Size - offset,
                Name = this._options.SystemLabel,
                Label = this._options.SystemLabel,
                FileSystem = FileSystem.Ntfs,
                Type = PartitionType.MicrosoftBasicData
            });

            return retval;
        }
        #endregion

        #region Private fields
        private readonly IDiskManagement _diskManagement;
        private readonly PartitioningOptions _options;
        private readonly ISystemInformation _systemInformation;
        #endregion
    }
}
