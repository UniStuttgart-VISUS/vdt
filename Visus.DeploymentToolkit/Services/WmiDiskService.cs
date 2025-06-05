// <copyright file="WmiDiskService.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.DiskManagement;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Vds;


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// Provides access to disk management via the WMI
    /// </summary>
    /// <param name="wmi">The basic WMI service for obtaining the management
    /// objects.</param>
    /// <param name="logger">A logger for the service.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="wmi"/> is
    /// <see langword="null"/>, or if <paramref name="logger"/> is
    /// <see langword="null"/>.</exception>
    [SupportedOSPlatform("windows")]
    internal sealed class WmiDiskService(
            IManagementService wmi,
            ILogger<WmiDiskService> logger)
            : IDiskManagement {

        #region Public methods
        /// <inheritdoc />
        public Task CleanAsync(IDisk disk,
                CleanFlags flags,
                CancellationToken cancellationToken) {
            ArgumentNullException.ThrowIfNull(disk);
            if (disk is not VdsDisk vds) {
                throw new InvalidOperationException(Errors.NoVdsDisk);
            }

            var force = ((flags & CleanFlags.Force) != 0);
            var forceOem = ((flags & CleanFlags.ForceOem) != 0);
            var fullClean = ((flags & CleanFlags.FullClean) != 0);

            cancellationToken.ThrowIfCancellationRequested();
            vds.AdvancedDisk.Clean(force, forceOem, fullClean, out var async);

            return async.WaitAsync(cancellationToken);
        }

        /// <inheritdoc />
        public Task CreatePartitionAsync(IDisk disk,
                IPartition partition,
                CancellationToken cancellationToken) {
            ArgumentNullException.ThrowIfNull(disk);
            ArgumentNullException.ThrowIfNull(partition);
            if (disk is not VdsDisk vds) {
                throw new ArgumentException(Errors.NoVdsDisk);
            }

            CREATE_PARTITION_PARAMETERS cpp = new() {
                Style = (VDS_PARTITION_STYLE) disk.PartitionStyle,
            };

            switch (cpp.Style) {
                case VDS_PARTITION_STYLE.MBR:
                    if (partition.Type.Mbr is null) {
                        throw new ArgumentException(
                            Errors.GptPartitionTypeMissing);
                    }

                    cpp.MbrPartInfo = new() {
                        BootIndicator = partition.IsBoot,
                        PartitionType = (MbrPartitionTypes) partition.Type.Mbr
                    };

                    this._logger.LogTrace("Creating MBR partition with type "
                        + "{Type} and boot indicator {Boot}.",
                        cpp.MbrPartInfo.PartitionType,
                        cpp.MbrPartInfo.BootIndicator);
                    break;

                case VDS_PARTITION_STYLE.GPT:
                    if (partition.Name is null) {
                        throw new ArgumentException(
                            Errors.GptPartitionNameMissing);
                    }
                    if (partition.Type.Gpt is null) {
                        throw new ArgumentException(
                            Errors.GptPartitionTypeMissing);
                    }

                    cpp.GptPartInfo = new() {
                        PartitionType = partition.Type.Gpt.Value,
                        PartitionId = Guid.NewGuid(),
                        Name = partition.Name
                    };

                    this._logger.LogTrace("Creating GPT partition {Name} "
                        + "({ID}) with type {Type}.", cpp.GptPartInfo.Name,
                        cpp.GptPartInfo.PartitionId,
                        cpp.GptPartInfo.PartitionType);
                    break;
            }

            cancellationToken.ThrowIfCancellationRequested();
            this._logger.LogTrace("Creating a partition on disk {Disk} at "
                + "offset {Offset} with size {Size}.", disk.ID,
                partition.Offset, partition.Size);
            vds.AdvancedDisk.CreatePartition(partition.Offset,
                partition.Size,
                ref cpp,
                out var async);
            return async.WaitAsync(cancellationToken);
        }

        /// <inheritdoc />
        public Task FormatAsync(IPartition partition,
                FileSystem fileSystem,
                string label,
                uint allocationUnitSize,
                FormatFlags flags,
                CancellationToken cancellationToken) {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task ConvertAsync(IDisk disk,
                PartitionStyle style,
                CancellationToken cancellationToken) {
            ArgumentNullException.ThrowIfNull(disk);
            if (disk is not VdsDisk vds) {
                throw new InvalidOperationException(Errors.NoVdsDisk);
            }

            try {
                vds.Disk.GetPack(out var _);
            } catch {
                this._logger.LogTrace("Disk {Disk} is not part of a pack, "
                    + "so we need to intialise it first.", disk.ID);
                // TODO: I really have no idea how to initialise the disk using VDS ...
            }

            this._logger.LogTrace("Converting disk {Disk} to partition style "
                + "from {OldStyle} to {NewStyle}.", disk.ID,
                disk.PartitionStyle, style);
            switch (style) {
                case PartitionStyle.Gpt:
                    vds.Disk.ConvertStyle(VDS_PARTITION_STYLE.GPT);
                    return Task.CompletedTask;

                case PartitionStyle.Mbr:
                    vds.Disk.ConvertStyle(VDS_PARTITION_STYLE.MBR);
                    return Task.CompletedTask;

                default:
                    throw new ArgumentException();
            }
        }

        /// <inheritdoc />
        public Task<IEnumerable<IDisk>> GetDisksAsync(
                CancellationToken cancellationToken)
            => Task.Run(() => {
                cancellationToken.ThrowIfCancellationRequested();
                this._logger.LogInformation("Retrieving disks via WMI.");
                return this._wmi
                    .GetInstancesOf(WmiDisk.Class, this._wmi.WindowsStorageScope)
                    .Select(d => new WmiDisk(d))
                    .AsEnumerable<IDisk>();
            }, cancellationToken);
        #endregion

        #region Private fields
        private readonly ILogger _logger = logger
            ?? throw new ArgumentNullException(nameof(logger));
        private readonly IManagementService _wmi = wmi
            ?? throw new ArgumentNullException(nameof(wmi));
        #endregion
    }
}
