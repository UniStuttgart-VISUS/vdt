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
    [StructLayout(LayoutKind.Sequential)]
    public struct VDS_PARTITION_PROP {

        /// <summary>
        /// Represents a union of <see cref="VDS_PARTITION_INFO_GPT"/> and
        /// <see cref="VDS_PARTITION_INFO_MBR"/>.
        /// </summary>
        /// <remarks>
        /// This indirection is required in C#, because we do not have anonymous
        /// unions and it avoids adding a <see cref="FieldOffsetAttribute"/> to
        /// all fields of <see cref="VDS_PARTITION_PROP"/>.
        /// </remarks>
        [StructLayout(LayoutKind.Explicit)]
        public struct PartitionInfo {

            [FieldOffset(0)]
            VDS_PARTITION_INFO_GPT Gpt;

            [FieldOffset(0)]
            VDS_PARTITION_INFO_MBR Mbr;
        }

        /// <summary>
        /// he styles enumerated by <see cref="VDS_PARTITION_STYLE"/>. The
        /// style is either master boot record
        /// (<see cref="VDS_PARTITION_STYLE.MBR"/>) or GUID partition table
        /// (<see cref="VDS_PARTITION_STYLE.GPT"/>). This member is the
        /// discriminant for the union.
        /// </summary>
        public VDS_PARTITION_STYLE PartitionStyle;

        /// <summary>
        /// The partition flags enumerated by <see cref="VDS_PARTITION_FLAG"/>.
        /// </summary>
        public uint Flags;

        /// <summary>
        /// The number assigned to the partition.
        /// </summary>
        public uint PartitionNumber;

        /// <summary>
        /// The partition offset.
        /// </summary>
        public ulong Offset;

        /// <summary>
        /// The size of the partition in bytes.
        /// </summary>
        public ulong Size;

        /// <summary>
        /// If <see cref="PartitionStyle"/> is
        /// <see cref="VDS_PARTITION_STYLE.MBR"/>, MBR-specific partition
        /// details ar available in <see cref="PartitionInfo.Mbr"/>.
        /// If <see cref="PartitionStyle"/> is
        /// <see cref="VDS_PARTITION_STYLE.GPT"/>, MBR-specific partition
        /// details ar available in <see cref="PartitionInfo.Gpt"/>.
        /// </summary>
        PartitionInfo Info;
    }
}
