// <copyright file="VDS_PARTITION_INFO_GPT.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Runtime.InteropServices;


namespace Visus.DeploymentToolkit.Vds {

    /// <summary>
    /// Defines details of a GUID partition table (GPT) partition.
    /// </summary>
    /// <remarks>
    /// The layout of this structure must be explicit in order to be usable
    /// in <see cref="VDS_PARTITION_PROP"/>.
    /// </remarks>
    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
    public struct VDS_PARTITION_INFO_GPT {

        /// <summary>
        /// GUID for the partition type.
        /// </summary>
        [FieldOffset(0)]
        public Guid PartitionType;

        /// <summary>
        /// GUID for the partition.
        /// </summary>
        [FieldOffset(16)]
        public Guid PartitionId;

        /// <summary>
        /// Attributes of the partition.
        /// </summary>
        [FieldOffset(32)]
        public GptPartitionAttributes Attributes;

        /// <summary>
        /// Name of the partition.
        /// </summary>
        [FieldOffset(40)]
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 36)]
        public string name;
    }
}
