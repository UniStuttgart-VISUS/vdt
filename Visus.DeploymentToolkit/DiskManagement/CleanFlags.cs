// <copyright file="CleanFlags.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;


namespace Visus.DeploymentToolkit.DiskManagement {

    /// <summary>
    /// Allows for customising the behaviour of the
    /// <see cref="Tasks.PartitionFormatDisk"/> task and the
    /// <see cref="IAdvancedDisk.CleanAsync"/> method.
    /// </summary>
    [Flags]
    public enum CleanFlags : uint {

        /// <summary>
        /// Perform a quick clean of non-protected disks.
        /// </summary>
        None = 0x00000000,

        /// <summary>
        /// Cleans a disk containing data volumes or ESP partitions.
        /// </summary>
        Force = 0x00000001,

        /// <summary>
        /// Cleans an MBR-based disk containing the known OEM partitions in
        /// the following table or cleans a GPT-based disk containing any OEM
        /// partition. An OEM partition has the
        /// <see cref="Vds.GptPartitionAttributes.PlatformRequired"/> flag set
        /// on a GPT-based disk.
        /// </summary>
        ForceOem = 0x00000002,

        /// <summary>
        /// Cleans the entire disk by replacing the data on each sector with
        /// zeros; otherwise, this method cleans only the first and the last
        /// megabytes on the disk.
        /// </summary>
        FullClean = 0x00000004,

        /// <summary>
        /// If this flag is set, the <see cref="Tasks.PartitionFormatDisk"/>
        /// will not consider failing to clean the disk as an error. This flag
        /// has no effect on the <see cref="IAdvancedDisk.CleanAsync"/>
        /// method.
        /// </summary>
        IgnoreErrors = 0x00000008,
    }
}
