// <copyright file="VDS_PARTITION_INFO_MBR.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Runtime.InteropServices;


namespace Visus.DeploymentToolkit.Vds {

    /// <summary>
    /// Defines the details of a master boot record (MBR) partition.
    /// </summary>
    /// <remarks>
    /// The layout of this structure must be explicit in order to be usable
    /// in <see cref="VDS_PARTITION_PROP"/>.
    /// </remarks>
    [StructLayout(LayoutKind.Explicit)]
    public struct VDS_PARTITION_INFO_MBR {

        /// <summary>
        /// Byte value indicating the partition type.
        /// </summary>
        [FieldOffset(0)]
        public byte PartitionType;

        /// <summary>
        /// If <c>true</c>, the partition is active and can be booted;
        /// otherwise, the partition cannot be used to boot the computer.
        /// </summary>
        [FieldOffset(1)]
        [MarshalAs(UnmanagedType.U1)]
        public bool BootIndicator;

        /// <summary>
        /// If <c>true</c>, the operating system recognises the partition style;
        /// otherwise, the partition style is unknown.
        /// </summary>
        [FieldOffset(2)]
        [MarshalAs(UnmanagedType.U1)]
        public bool RecognisedPartition;

        /// <summary>
        /// Reserved sectors.
        /// </summary>
        [FieldOffset(4)]
        public uint HiddenSectors;
    }
}
