// <copyright file="VDS_PARTITION_PROP.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Runtime.InteropServices;


namespace Visus.DeploymentToolkit.Vds {

    /// <summary>
    /// Defines the properties of a partition.
    /// </summary>
    /// <remarks>
    /// This structure needs to use <see cref="LayoutKind.Sequential"/>, because
    /// <see cref="VDS_PARTITION_PROP.Mbr"/> and
    /// <see cref="VDS_PARTITION_PROP.Gpt"/> form a union.
    /// </remarks>
    [StructLayout(LayoutKind.Explicit)]
    public struct VDS_PARTITION_PROP {

        /// <summary>
        /// he styles enumerated by <see cref="VDS_PARTITION_STYLE"/>. The
        /// style is either master boot record
        /// (<see cref="VDS_PARTITION_STYLE.MBR"/>) or GUID partition table
        /// (<see cref="VDS_PARTITION_STYLE.GPT"/>). This member is the
        /// discriminant for the union.
        /// </summary>
        [FieldOffset(0)]
        public VDS_PARTITION_STYLE PartitionStyle;

        /// <summary>
        /// The partition flags enumerated by <see cref="VDS_PARTITION_FLAG"/>.
        /// </summary>
        [FieldOffset(4)]
        public uint Flags;

        /// <summary>
        /// The number assigned to the partition.
        /// </summary>
        [FieldOffset(8)]
        public uint PartitionNumber;

        /// <summary>
        /// The partition offset.
        /// </summary>
        [FieldOffset(16)]
        public ulong Offset;

        /// <summary>
        /// The size of the partition in bytes.
        /// </summary>
        [FieldOffset(24)]
        public ulong Size;

        /// <summary>
        /// If <see cref="PartitionStyle"/> is
        /// <see cref="VDS_PARTITION_STYLE.MBR"/>, MBR-specific partition
        /// details ar available in <see cref="PartitionInfo.Mbr"/>.
        /// </summary>
        [FieldOffset(32)]
        public VDS_PARTITION_INFO_MBR Mbr;

        /// <summary>
        /// <see cref="VDS_PARTITION_STYLE.GPT"/>, MBR-specific partition
        /// details ar available in <see cref="PartitionInfo.Gpt"/>.
        /// </summary>
        [FieldOffset(32)]
        public VDS_PARTITION_INFO_GPT Gpt;
    }
}
