// <copyright file="VdsDisk.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Visus.DeploymentToolkit.DiskManagement;
using Visus.DeploymentToolkit.Vds;


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// A <see cref="IDisk"/> as enumerated by the <see cref="VdsService"/>.
    /// </summary>
    internal sealed class VdsDisk : IDisk {

        #region Public properties
        /// <inheritdoc />
        public StorageBusType BusType
            => (StorageBusType) this._properties.BusType;

        /// <inheritdoc />
        public string FriendlyName => this._properties.FriendlyName;

        /// <inheritdoc />
        public Guid ID => this._properties.Id;

        /// <inheritdoc />
        public IEnumerable<IPartition> Partitions => this._partitions.Value;

        /// <inheritdoc />
        public PartitionStyle PartitionStyle
            => (PartitionStyle) this._properties.PartitionStyle;

        /// <inheritdoc />
        public ulong Size => this._properties.Size;

        /// <inheritdoc />
        public IEnumerable<IVolume> Volumes => this._volumes.Value;
        #endregion

        #region Internal constructors
        internal VdsDisk(IVdsDisk disk) {
            this._disk = disk ?? throw new ArgumentNullException(nameof(disk));
            this._disk.GetProperties(out this._properties);
            this._partitions = new(() => {
                if (this._disk is IVdsAdvancedDisk disk) {
                    disk.QueryPartitions(out var props, out var cnt);
                    return from p in props
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
