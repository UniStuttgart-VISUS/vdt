// <copyright file="DiskPartitioningDefinition.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Visus.DeploymentToolkit.DiskManagement {

    /// <summary>
    /// Describes the desired partitioning of a disk.
    /// </summary>
    public sealed class DiskPartitioningDefinition : IDisk {

        #region Public properties
        /// <inheritdoc />
        public StorageBusType BusType => StorageBusType.Unknown;

        /// <inheritdoc />
        public DiskFlags Flags => DiskFlags.None;

        /// <inheritdoc />
        public string FriendlyName => string.Empty;

        /// <summary>
        /// Gets or sets the ID of the disk to be partitioned.
        /// </summary>
        public Guid ID { get; set; } = Guid.Empty;

        /// <summary>
        /// Gets or sets the partitions to be created on the disk.
        /// </summary>
        public List<PartitionDefinition> Partitions { get; set; } = new();

        /// <inheritdoc />
        IEnumerable<IPartition> IDisk.Partitions => this.Partitions;

        /// <summary>
        /// Gets or sets the parition style of the disk, which must be either
        /// <see cref="PartitionStyle.Gpt"/> or <see cref="PartitionStyle.Mbr"/>
        /// when applying the partitioning to a disk.
        /// </summary>
        public PartitionStyle PartitionStyle { get; set; }

        /// <inheritdoc />
        public uint SectorSize => 0;

        /// <inheritdoc />
        public ulong Size => 0;

        /// <inheritdoc />
        public IEnumerable<Tuple<IVolume, IPartition>> VolumePartitions
            => this.Partitions.Select(p => new Tuple<IVolume, IPartition>(p, p));

        /// <inheritdoc />
        public IEnumerable<IVolume> Volumes => this.Partitions;
        #endregion

        #region Public methods
        /// <summary>
        /// Fails unconditionally.
        /// </summary>
        /// <param name="style"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public Task ConvertAsync(PartitionStyle style) {
            throw new InvalidOperationException();
        }
        #endregion
    }
}
