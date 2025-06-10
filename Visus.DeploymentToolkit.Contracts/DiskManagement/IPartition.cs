// <copyright file="IPartition.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>


namespace Visus.DeploymentToolkit.DiskManagement {

    /// <summary>
    /// The interface of a disk partition.
    /// </summary>
    public interface IPartition {

        /// <summary>
        /// Gets the flags of the partition.
        /// </summary>
        PartitionFlags Flags { get; }

        /// <summary>
        /// Gets the ordinal assigned to the partition.
        /// </summary>
        uint Index { get; }

        /// <summary>
        /// Gets the name of the partition.
        /// </summary>
        /// <remarks>
        /// Only GPT partitions can have a name.
        /// </remarks>
        string? Name { get; }

        /// <summary>
        /// Gets the offset of the partition in bytes.
        /// </summary>
        /// <remarks>
        /// When this property is used to create a partition, a value of zero
        /// indicates that the partition should start right after the last
        /// existing one.
        /// </remarks>
        ulong Offset { get; }

        /// <summary>
        /// Gets the size of the partition in bytes.
        /// </summary>
        /// <remarks>
        /// When this property is used to create a partition, a value of zero
        /// indicates that the partition should use all remaining space.
        /// </remarks>
        ulong Size { get; }

        /// <summary>
        /// Gets the partition style.
        /// </summary>
        /// <remarks>
        /// The partition style of the disk determines whether certain
        /// properties are applicable for a partition. Although the partition
        /// style is a property of the whole disk, we have it in the partition
        /// as well for convenience.
        /// </remarks>
        PartitionStyle Style { get; }

        /// <summary>
        /// Gets the type of the partition.
        /// </summary>
        PartitionType Type { get; }
    }
}
