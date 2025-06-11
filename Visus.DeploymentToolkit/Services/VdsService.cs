// <copyright file="VdsService.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.DiskManagement;
using Visus.DeploymentToolkit.Extensions;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Vds;


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// Provides access to disk management via the Virtual Disk Service VDS.
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal sealed class VdsService : IDiskManagement {

        #region Public constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="logger"></param>
        /// <exception cref="ArgumentNullException">If <paramref name="logger"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="Exception">If the <see cref="VdsServiceLoader"/>
        /// does nto implement <see cref="IVdsServiceLoader"/>, which should
        /// never happen.</exception>
        public VdsService(ILogger<VdsService> logger) {
            _logger = logger
                ?? throw new ArgumentNullException(nameof(logger));

            try {
                var loader = new VdsServiceLoader() as IVdsServiceLoader
                    ?? throw new Exception(Errors.NoVdsServiceLoader);
                loader.LoadService(null, out _service);
            } catch (Exception ex) {
                _logger.LogError(ex.Message, ex);
                throw;
            }
        }
        #endregion

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
            this._logger.LogInformation("Clearing disk {Disk} ({Name}) "
                + "with flags {Flags}.", disk.ID, disk.FriendlyName, flags);
            vds.AdvancedDisk.Clean(force, forceOem, fullClean, out var async);

            return async.WaitAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IPartition> CreatePartitionAsync(IDisk disk,
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
                        BootIndicator = partition.Flags.HasFlag(PartitionFlags.Boot),
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
            this._logger.LogTrace("User-provided partition offset is {Offset} "
                + "bytes.", partition.Offset);
            var offset = partition.Offset.CeilingAlign(vds.GetBytesPerTrack());

            if (offset <= 0) {
                offset = (from p in vds.GetPartitions()
                          orderby p.Offset descending
                          select p.Offset + p.Size).FirstOrDefault();
                this._logger.LogTrace("Appending at an offset of {Offset} "
                    + "bytes.", offset);
                // https://github.com/pbatard/rufus/blob/688f011f317492225ded10b51edc19b39d62fbd4/src/drive.c#L2290-L2306
                if ((offset == 0)
                        && (vds.PartitionStyle == PartitionStyle.Gpt)) {
                    offset = 1 * 1024 * 1024;
                    this._logger.LogTrace("Adjusting offset of first partition "
                        + "to {Offset}.", offset);
                }
            }

            // Align to track and cluster size.
            // https://github.com/pbatard/rufus/blob/688f011f317492225ded10b51edc19b39d62fbd4/src/drive.c#L2317C2-L2321
            offset = offset.CeilingAlign(vds.GetBytesPerTrack());
            if (vds.GetNtfsClusterSize() % vds.SectorSize == 0) { 
                offset = offset.FloorAlign(vds.GetNtfsClusterSize());
            }
            this._logger.LogTrace("Aligned offset of partition is at {Offset} "
                + "bytes.", offset);

            var size = partition.Size;
            this._logger.LogTrace("User-provided partition size is {Size} "
                + "bytes.", size);
            if (size <= 0) {
                size = disk.Size - offset;
                this._logger.LogTrace("Adjusting partition size to be the "
                    + "remaining {Size} of {TotalSize} bytes.", size,
                    disk.Size);
            }

            size = size.FloorAlign(vds.GetBytesPerTrack());
            this._logger.LogTrace("Aligned partition size is {Size} bytes.",
                size);

            cancellationToken.ThrowIfCancellationRequested();
            this._logger.LogTrace("Creating a partition on disk {Disk} at "
                + "offset {Offset} with size {Size}.", disk.ID, offset, size);
            vds.AdvancedDisk.CreatePartition(offset, size, ref cpp,
                out var async);
            await async.WaitAsync(cancellationToken).ConfigureAwait(false);

            cancellationToken.ThrowIfCancellationRequested();
            this._logger.LogTrace("Getting partition properties for the newly "
                + "created partition at offset {Offset}.", offset);
            await this.RefreshAsync(false, cancellationToken);
            vds.AdvancedDisk.GetPartitionProperties(offset, out var properties);
            return new VdsPartition(vds, properties);
        }

        /// <inheritdoc />
        public Task FormatAsync(IPartition partition,
                FileSystem fileSystem,
                string label,
                uint allocationUnitSize,
                FormatFlags flags,
                CancellationToken cancellationToken) {
            ArgumentNullException.ThrowIfNull(partition);
            if (partition is not VdsPartition vds) {
                throw new ArgumentException(Errors.NoVdsPartition);
            }

            var disk = vds.Disk.AdvancedDisk;
            Debug.Assert(disk is not null);

            this._logger.LogTrace("Formatting partition {Partition} "
                + "on disk {Disk} with file system {FileSystem}, label "
                + "{Label}, allocation unit size {AllocationUnitSize} "
                + "and flags {Flags}.", partition.Index, vds.Disk.ID,
                fileSystem, label, allocationUnitSize, flags);
            disk.FormatPartition(vds.Offset,
                fileSystem.ToVds(),
                label,
                allocationUnitSize,
                flags.HasFlag(FormatFlags.Force),
                flags.HasFlag(FormatFlags.Quick),
                flags.HasFlag(FormatFlags.EnableCompression),
                out var async);

            return async.WaitAsync(cancellationToken);
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
                vds.Disk.GetPack(out var pack);
                pack.GetProperties(out var props);
                this._logger.LogTrace("Converting disk {Disk} (pack {Pack}) to "
                    + "partition style from {OldStyle} to {NewStyle}.", disk.ID,
                    props.Name, disk.PartitionStyle, style);
                switch (style) {
                    case PartitionStyle.Gpt:
                        vds.Disk.ConvertStyle(VDS_PARTITION_STYLE.GPT);
                        return Task.CompletedTask;

                    case PartitionStyle.Mbr:
                        vds.Disk.ConvertStyle(VDS_PARTITION_STYLE.MBR);
                        return Task.CompletedTask;

                    default:
                        throw new ArgumentException(string.Format(
                            Errors.InvalidPartitionStyle, style));
                }
            } catch {
                this._logger.LogTrace("Disk {Disk} is not part of a pack, "
                    + "so we need to intialise it first.", disk.ID);
                var prov = this.GetBasicSoftwareProvider();
                prov.CreatePack(out var pack);
                pack.GetProperties(out var props);

                this._logger.LogTrace("Adding disk {Disk} to a newly created "
                    + "pack {Pack} with partition style {Style}.", disk.ID,
                    props.Name, style);
                pack.AddDisk(disk.ID, (VDS_PARTITION_STYLE) style, false);
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task<IEnumerable<IDisk>> GetDisksAsync(
                CancellationToken cancellationToken) {
            return Task<IEnumerable<IDisk>>.Factory.StartNew(
                () => GetDisks(cancellationToken));
        }

        /// <summary>
        /// Refresh disk ownership and layout information.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task RefreshAsync(bool reenumerate,
                CancellationToken cancellationToken)
            => Task.Run(() => {
                cancellationToken.ThrowIfCancellationRequested();
                this.WaitForVds();

                cancellationToken.ThrowIfCancellationRequested();
                this._logger.LogTrace("Refreshing the VDS service.");
                this._service.Refresh();

                if (reenumerate) {
                    cancellationToken.ThrowIfCancellationRequested();
                    this._logger.LogTrace("Re-enumerating the disks.");
                    this._service.Reenumerate();
                }
            }, cancellationToken);
        #endregion

        #region Private methods
        /// <summary>
        /// Enumerate all disks in the given <paramref name="pack"/>.
        /// </summary>
        /// <param name="pack"></param>
        /// <param name="logger"></param>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        private IEnumerable<VdsDisk> GetDisks(
                IVdsPack pack,
                CancellationToken cancellation) {
            ArgumentNullException.ThrowIfNull(pack);
            foreach (var d in pack.QueryDisks()) {
                cancellation.ThrowIfCancellationRequested();
                yield return new VdsDisk(d);
            }
        }

        /// <summary>
        /// Gets the disks for all providers registered with the given
        /// <paramref name="service"/>.
        /// </summary>
        /// <remarks>
        /// Currently, we only support the Windows software provider.
        /// </remarks>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="COMException"></exception>
        private IEnumerable<VdsDisk> GetDisks(
                CancellationToken cancellation) {
            cancellation.ThrowIfCancellationRequested();

            // First of all, make sure that the VDS is ready.
            this.WaitForVds();

            cancellation.ThrowIfCancellationRequested();

            foreach (var p in this.GetProviders()) {
                cancellation.ThrowIfCancellationRequested();
                p.GetProperties(out var props);
                //this._logger.LogTrace("Provider {Provider} ({ID}) has type "
                //    + "{Type} and flags {Flags}.", props.Name, props.ID,
                //    props.Type, props.Flags);

                if ((p is IVdsSwProvider sw) && (sw != null)) {
                    this._logger.LogTrace("Querying disks from provider "
                        + "{Provider}", props.Name);
                    foreach (var d in GetDisks(sw, cancellation)) {
                        yield return d;
                    }

                } else if ((p is IVdsVdProvider vd) && (vd != null)) {
                    this._logger.LogTrace("Querying virtual disks from "
                        + "provider {Provider}", props.Name);
                    foreach (var d in GetDisks(vd, cancellation)) {
                        yield return d;
                    }
                }
            }

            // Unallocated disks need the be enumerated separately. These
            // are the most important ones for our scenario.
            {
                this._logger.LogTrace("Querying unallocated disks.");
                this._service.QueryUnallocatedDisks(out var enumerator);
                foreach (var d in enumerator.Enumerate<IVdsDisk>()) {
                    cancellation.ThrowIfCancellationRequested();
                    yield return new VdsDisk(d, DiskFlags.Uninitialised);
                }
            }
        }

        /// <summary>
        /// Gets all disks from the specified software provider.
        /// </summary>
        /// <param name="provider">The software provider for disks.</param>
        /// <param name="cancellation"></param>
        /// <returns>All disks managed by the provider.</returns>
        /// <exception cref="ArgumentNullException">If
        /// <paramref name="provider"/> is <c>null</c>, or if
        /// <paramref name="logger"/> is <c>null</c>.</exception>
        private IEnumerable<VdsDisk> GetDisks(
                IVdsSwProvider provider,
                CancellationToken cancellation) {
            _ = provider ?? throw new ArgumentNullException(nameof(provider));
            cancellation.ThrowIfCancellationRequested();

            foreach (var pack in provider.QueryPacks()) {
                cancellation.ThrowIfCancellationRequested();

                //pack.GetProperties(out var properties);
                //this._logger.LogTrace("Querying disks from pack {Pack} "
                //    + "({PackID}, status {Status}, flags {Flags}).",
                //    properties.Name, properties.Id, properties.Status,
                //    properties.Flags);
                foreach (var d in GetDisks(pack, cancellation)) {
                    cancellation.ThrowIfCancellationRequested();
                    yield return d;
                }
            }
        }

        /// <summary>
        /// Enumerates all disks from the specified virtual disk provider.
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        private IEnumerable<VdsDisk> GetDisks(
                IVdsVdProvider provider,
                CancellationToken cancellation) {
            _ = provider ?? throw new ArgumentNullException(nameof(provider));
            cancellation.ThrowIfCancellationRequested();
            foreach (var d in  provider.QueryVDisks()) {
                cancellation.ThrowIfCancellationRequested();
                yield return new VdsDisk(d);
            }
        }

        /// <summary>
        /// Answer the basic software provider.
        /// </summary>
        /// <returns></returns>
        private IVdsSwProvider GetBasicSoftwareProvider() {
            foreach (var p in this.GetProviders()) {
                p.GetProperties(out var props);
                if (props.Type != VDS_PROVIDER_TYPE.Software) {
                    continue;
                }
                if (props.Flags.HasFlag(VDS_PROVIDER_FLAG.Dynamic)) {
                    continue;
                }

                return (IVdsSwProvider) p;
            }

            throw new InvalidOperationException(Errors.NoBasicSwProvider);
        }

        /// <summary>
        /// Enumerates all providers.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<IVdsProvider> GetProviders() {
            var types = VDS_QUERY_PROVIDER_FLAG.SOFTWARE_PROVIDERS
                | VDS_QUERY_PROVIDER_FLAG.HARDWARE_PROVIDERS
                | VDS_QUERY_PROVIDER_FLAG.VIRTUALDISK_PROVIDERS;

            foreach (var unknown in this._service.QueryProviders(types)) {
                if ((unknown is IVdsProvider p) && (p is not null)) {
                    yield return p;
                }
            }
        }

        /// <summary>
        /// Blocks the calling thread until the Virtual Disk Service is ready.
        /// </summary>
        /// <exception cref="COMException"></exception>
        private void WaitForVds() {
            this._logger.LogTrace("Waiting for the Virtual Disk "
                + "Service to become ready.");
            var status = this._service.WaitForServiceReady();
            if (status != 0) {
                this._logger.LogWarning("The Virtual Disk Service did not "
                    + "become ready with status: {Status}", status);
                throw new COMException(Errors.WaitVdsFailed, (int) status);
            }
        }
        #endregion

        #region Private fields
        private readonly ILogger _logger;
        private readonly IVdsService _service;
        #endregion
    }
}
