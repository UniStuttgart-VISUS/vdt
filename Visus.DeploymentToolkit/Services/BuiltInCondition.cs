// <copyright file="BuiltInCondition.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Visus.DeploymentToolkit.DiskManagement;


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// Provides a set of built-in conditions for
    /// <see cref="DiskSelectionStep"/>s, which cannot be expressed as dynamic
    /// queries on <see cref="IDisk"/>s.
    /// </summary>
    public enum BuiltInCondition {

        /// <summary>
        /// No built-in condition is to be applied, use
        /// <see cref="DiskSelectionStep.Condition"/> instead.
        /// </summary>
        None = 0,

        /// <summary>
        /// Selects disks that have at least one partition with a type known to
        /// be used by Linux.
        /// </summary>
        HasLinuxPartition,

        /// <summary>
        /// Selects disks that have at least one partition with a type known to
        /// be used by Microsoft Windows.
        /// </summary>
        HasMicrosoftPartition,

        /// <summary>
        /// Selects disks with a FAT32-formatted EFI boot partition.
        /// </summary>
        IsEfiBootDisk,

        /// <summary>
        /// Selects the disks that have not partitions on them.
        /// </summary>
        IsEmpty,

        /// <summary>
        /// Selects MBR disks with the boot flag set.
        /// </summary>
        IsMbrBootDisk,

        /// <summary>
        /// Selects the disk with the largest overall capacity.
        /// </summary>
        IsLargest,

        /// <summary>
        /// Selects the disk with the smallest overall capacity.
        /// </summary>
        IsSmallest,
    }
}
