// <copyright file="PARTITION_INFORMATION_EX.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Runtime.InteropServices;


namespace Visus.DeploymentToolkit.DiskManagement {

    /// <summary>
    /// Contains partition information for standard AT-style master boot record
    /// (MBR) and Extensible Firmware Interface (EFI) disks.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    internal struct PARTITION_INFORMATION_EX {

        /// <summary>
        /// The format of the partition. For a list of values, see
        /// <see cref="PARTITION_STYLE"/>.
        /// </summary>
        [FieldOffset(0)]
        public PARTITION_STYLE PartitionStyle;

        /// <summary>
        /// The starting offset of the partition.
        /// </summary>
        [FieldOffset(4)]
        public ulong StartingOffset;

        /// <summary>
        /// The size of the partition, in bytes.
        /// </summary>
        [FieldOffset(12)]
        public ulong PartitionLength;

        /// <summary>
        /// The number of the partition (1-based).
        /// </summary>
        [FieldOffset(20)]
        public uint PartitionNumber;

        /// <summary>
        /// If this member is non-zero, the partition is rewritable. The value
        /// of this parameter should be set to 1.
        /// </summary>
        [FieldOffset(24)]
        public uint RewritePartition;

        [FieldOffset(28)]
        public uint IsServicePartition;

        ///// <summary>
        ///// A <see cref="PARTITION_INFORMATION_MBR "/> structure that specifies
        ///// partition information specific to master boot record (MBR) disks.
        ///// The MBR partition format is the standard AT-style format.
        ///// </summary>
        //[FieldOffset(32)]
        //public PARTITION_INFORMATION_MBR Mbr;

        ///// <summary>
        ///// A <see cref="PARTITION_INFORMATION_GPT"/> structure that specifies
        ///// partition information specific to GUID partition table (GPT) disks.
        ///// The GPT format corresponds to the EFI partition format.
        ///// </summary>
        //[FieldOffset(32)]
        //public PARTITION_INFORMATION_GPT Gpt;
    }
}
