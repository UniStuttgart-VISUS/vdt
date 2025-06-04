// <copyright file="PartitionUsage.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;


namespace Visus.DeploymentToolkit.DiskManagement {

    /// <summary>
    /// Annotates a <see cref="PartitionDefinition"/> such that the
    /// partitioning task can handle special partitions correctly.
    /// </summary>
    [Flags]
    public enum PartitionUsage : uint {

        /// <summary>
        /// The partition has no specific usage assigned that would require
        /// special handling by the partitioning task.
        /// </summary>
        Other = 0,

        /// <summary>
        /// The partition is the one where the boot loader will be installed.
        /// </summary>
        Boot = 0x00000001,

        /// <summary>
        /// The partition is the one where the EFI system partition is
        /// installed.
        /// </summary>
        System = 0x00000002,

        /// <summary>
        /// The partition is the one where the operating system will be
        /// installed.
        /// </summary>
        Installation = 0x00000004,
    }
}
