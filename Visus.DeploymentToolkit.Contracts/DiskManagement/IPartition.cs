// <copyright file="IPartition.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

namespace Visus.DeploymentToolkit.DiskManagement
{

    /// <summary>
    /// The interface of a disk partition.
    /// </summary>
    public interface IPartition {

        /// <summary>
        /// Gets the ordinal assigned to the partition.
        /// </summary>
        uint Index { get; }

        /// <summary>
        /// Gets whether the boot flag is set for the partition.
        /// </summary>
        /// <remarks>
        /// This property is only relevant for <see cref="PartitionStyle.Mbr"/>.
        /// For GPT disks, it is always <c>false</c>.
        /// </remarks>
        bool IsBoot { get; }

        /// <summary>
        /// Answer whether the partition is a system partition.
        /// </summary>
        bool IsSystem { get; }

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
        ulong Offset { get; }

        /// <summary>
        /// Gets the size of the partition in bytes.
        /// </summary>
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
