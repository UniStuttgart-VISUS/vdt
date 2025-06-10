// <copyright file="PartitionFlags.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;


namespace Visus.DeploymentToolkit.DiskManagement {

    /// <summary>
    /// A set of Boolean properties that a <see cref="IPartition"/> can have.
    /// </summary>
    [Flags]
    public enum PartitionFlags : uint {

        /// <summary>
        /// No flags are set.
        /// </summary>
        None = 0x00000000,

        /// <summary>
        /// The active bit of the partition is set.
        /// </summary>
        /// <remarks>
        /// This flag is only relevant for MBR disks. It indicates that the
        /// the partition is active and can be booted from.
        /// </remarks>
        Active = 0x00000001,

        /// <summary>
        /// This is a boot partition.
        /// </summary>
        /// <remarks>
        /// This flag is only relevant for MBR disks.
        /// </remarks>
        Boot = 0x00000002,

        /// <summary>
        /// This is an EFI system partition.
        /// </summary>
        System = 0x00000004,

        /// <summary>
        /// The partition is hidden.
        /// </summary>
        Hidden = 0x00000008,

    }
}
