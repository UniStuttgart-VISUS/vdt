// <copyright file="BuiltInCondition.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>


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
        /// Selects the disk with the largest overall capacity.
        /// </summary>
        IsLargest,

        /// <summary>
        /// Selects the disk with the smallest overall capacity.
        /// </summary>
        IsSmallest,

        /// <summary>
        /// Selects disks with a FAT32-formatted EFI boot partition.
        /// </summary>
        IsEfiBootDisk,

        /// <summary>
        /// Selects MBR disks with the boot flag set.
        /// </summary>
        IsMbrBootDisk
    }
}
