// <copyright file="PartitioningOptions.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>


namespace Visus.DeploymentToolkit.DiskManagement {

    /// <summary>
    /// An options class for configuring the default partitioning of
    /// installation disks.
    /// </summary>
    public sealed class PartitioningOptions {

        /// <summary>
        /// Gets the label assigned to the system-reserved partition on BIOS
        /// systems.
        /// </summary>
        public string BiosSystemReservedLabel { get; set; } = "System reserved";

        /// <summary>
        /// Gets or sets the size of the system-reserved parititionn on BIOS
        /// systems in bytes.
        /// </summary>
        public uint BiosSystemReservedSize { get; set; } = 499 * 1024 * 1024;

        /// <summary>
        /// Gets the label assigned to the EFI system partition.
        /// </summary>
        public string EfiLabel { get; set; } = "Boot";

        /// <summary>
        /// Gets or set the size of the EFI system partition in bytes.
        /// </summary>
        public uint EfiSize { get; set; } = 499 * 1024 * 1024;

        /// <summary>
        /// Gets or sets the label assigned to the recovery partition.
        /// </summary>
        public string RecoveryLabel { get; set; } = "MSR";

        /// <summary>
        /// Gets or sets the size of the recovery partition in bytes.
        /// </summary>
        public uint RecoverySize { get; set; } = 0;

        /// <summary>
        /// Gets or sets the label assigned to the system partition.
        /// </summary>
        public string SystemLabel { get; set; } = "Windows";
    }
}
