// <copyright file="PARTITION_INFORMATION_MBR.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Runtime.InteropServices;


namespace Visus.DeploymentToolkit.DiskManagement {

    /// <summary>
    /// Contains partition information specific to master boot record (MBR)
    /// disks.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct PARTITION_INFORMATION_MBR {

        /// <summary>
        /// The type of partition.
        /// </summary>
        public byte PartitionType;

        /// <summary>
        /// If the member is non-zero, the partition is a boot partition.
        /// </summary>
        public uint BootIndicator;

        /// <summary>
        /// If the member is non-zero, the partition is of a recognised type.
        /// </summary>
        public uint RecognisedPartition;

        /// <summary>
        /// The number of hidden sectors to be allocated when the partition
        /// table is created.
        /// </summary>
        public uint HiddenSectors;

        /// <summary>
        /// The unique ID of the partition.
        /// </summary>
        public Guid PartitionID;
    }
}
