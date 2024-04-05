// <copyright file="VdsDisk.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Services;
using Visus.DeploymentToolkit.Vds;


namespace Visus.DeploymentToolkit.DiskManagement {

    /// <summary>
    /// A <see cref="IDisk"/> as enumerated by the <see cref="VdsService"/>.
    /// </summary>
    internal sealed class VdsDisk : IDisk {

        #region Public properties
        /// <inheritdoc />
        public StorageBusType BusType
            => (StorageBusType) _properties.BusType;

        /// <inheritdoc />
        public string FriendlyName => _properties.FriendlyName;

        /// <inheritdoc />
        public Guid ID => _properties.Id;

        /// <inheritdoc />
        public IEnumerable<IPartition> Partitions => _partitions.Value;

        /// <inheritdoc />
        public PartitionStyle PartitionStyle
            => (PartitionStyle) _properties.PartitionStyle;

        /// <inheritdoc />
        public ulong Size => _properties.Size;

        /// <inheritdoc />
        public IEnumerable<IVolume> Volumes => _volumes.Value;
        #endregion

        #region Public methods
        /// <inheritdoc />
        public Task ConvertAsync(PartitionStyle style) {
            switch (style) {
                case PartitionStyle.Gpt:
                    this._disk.ConvertStyle(VDS_PARTITION_STYLE.GPT);
                    return Task.CompletedTask;

                case PartitionStyle.Mbr:
                    this._disk.ConvertStyle(VDS_PARTITION_STYLE.MBR);
                    return Task.CompletedTask;

                default:
                    throw new ArgumentException();
            }
        }
        #endregion

        #region Internal constructors
        internal VdsDisk(IVdsDisk disk) {
            this._disk = disk ?? throw new ArgumentNullException(nameof(disk));
            this._disk.GetProperties(out _properties);
            this._partitions = new(() => {
                if (this._disk is IVdsAdvancedDisk disk) {
                    disk.QueryPartitions(out var props, out var _);
                    this._disk.QueryExtents(out var exts, out var _);

                    return from p in props
                           //let e = exts.Where(ee => ee.Offset == p.Offset).Single()
                           select new VdsPartition(p);
                } else {
                    return Enumerable.Empty<IPartition>();
                }
            });
            this._volumes = new(() => {
                disk.GetPack(out var pack);
                pack.QueryVolumes(out var enumerator);
                return enumerator.Enumerate<IVdsVolume>()
                    .Select(v => new VdsVolume(v));
            });
        }
        #endregion

        #region Private fields
        private readonly IVdsDisk _disk;
        private readonly Lazy<IEnumerable<IPartition>> _partitions;
        private readonly VDS_DISK_PROP _properties;
        private readonly Lazy<IEnumerable<IVolume>> _volumes;
        #endregion
    }
}
