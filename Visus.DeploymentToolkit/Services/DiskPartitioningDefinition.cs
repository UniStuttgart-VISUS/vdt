// <copyright file="DiskPartitioningDefinition.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Collections.Generic;


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// Describes the desired partitioning of a disk.
    /// </summary>
    public sealed class DiskPartitioningDefinition : IDisk {

        /// <inheritdoc />
        public StorageBusType BusType => StorageBusType.Unknown;

        /// <inheritdoc />
        public string FriendlyName { get; } = string.Empty;

        /// <summary>
        /// Gets or sets the ID of the disk to be partitioned.
        /// </summary>
        public Guid ID { get; set; } = Guid.Empty;

        /// <summary>
        /// Gets or sets the partitions to be created on the disk.
        /// </summary>
        public List<IPartition> Partitions { get; set; } = new();

        /// <inheritdoc />
        IEnumerable<IPartition> IDisk.Partitions => this.Partitions;
    }
}
