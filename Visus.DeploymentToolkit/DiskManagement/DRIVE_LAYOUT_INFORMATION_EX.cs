// <copyright file="DRIVE_LAYOUT_INFORMATION_EX.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Runtime.InteropServices;


namespace Visus.DeploymentToolkit.DiskManagement {

    /// <summary>
    /// Contains extended information about a drive's partitions.
    /// </summary>
    /// <remarks>
    /// This structure is followed by <see cref="PartitionCount"/> structures of
    /// <see cref="PARTITION_INFORMATION_EX" />.
    /// </remarks>
    [StructLayout(LayoutKind.Explicit)]
    internal struct DRIVE_LAYOUT_INFORMATION_EX {

        /// <summary>
        /// The style of the partitions on the drive enumerated by the
        /// <see cref="PARTITION_STYLE"/> enumeration.
        /// </summary>
        [FieldOffset(0)]
        public PARTITION_STYLE PartitionStyle;

        /// <summary>
        /// The number of partitions on the drive.
        /// </summary>
        [FieldOffset(4)]
        public uint PartitionCount;

        /// <summary>
        /// A <see cref="DRIVE_LAYOUT_INFORMATION_MBR"/> structure
        /// containing information about the master boot record type
        /// partitioning on the drive.
        /// </summary>
        [FieldOffset(8)]
        public DRIVE_LAYOUT_INFORMATION_MBR Mbr;

        /// <summary>
        /// A <see cref="DRIVE_LAYOUT_INFORMATION_GPT"/> structure containing
        /// information about the GUID disk partition type partitioning on the
        /// drive.
        /// </summary>
        [FieldOffset(8)]
        public DRIVE_LAYOUT_INFORMATION_GPT Gpt;
    }
}
