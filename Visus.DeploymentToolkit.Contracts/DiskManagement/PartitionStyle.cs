// <copyright file="IPartition.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 - 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>


namespace Visus.DeploymentToolkit.DiskManagement {

    /// <summary>
    /// Defines the partition styles supported by <see cref="IPartition"/>.
    /// </summary>
    /// <remarks>
    /// The constants used here match the values of the Virtual Disk Service
    /// defined in the <c>VDS_PARTITION_STYLE</c> enumeration and what the WMI
    /// <c>MSFT_Disk</c> class uses.
    /// </remarks>
    public enum PartitionStyle {

        /// <summary>
        /// An uninitialized disk.
        /// </summary>
        /// <remarks>
        ///  New disks or newly cleaned disks have this partitioning type.
        /// </remarks>
        Unknown = 0,

        /// <summary>
        /// The style is master boot record (MBR).
        /// </summary>
        Mbr = 1,

        /// <summary>
        /// The style is GUID partition table (GPT).
        /// </summary>
        Gpt = 2
    }
}
