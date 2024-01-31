// <copyright file="MbrPartitionParameters.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Runtime.InteropServices;


namespace Visus.DeploymentToolkit.Vds {

    /// <summary>
    /// The MBR parameters in <see cref="CREATE_PARTITION_PARAMETERS"/>.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct MbrPartitionParameters {

        /// <summary>
        /// Byte value indicating the partition type.
        /// </summary>
        [FieldOffset(0)]
        public MbrPartitionTypes PartitionType;

        /// <summary>
        /// If <c>true</c>, the partition is active and can be booted;
        /// otherwise, the partition cannot be used to boot the computer.
        /// </summary>
        [FieldOffset(1)]
        [MarshalAs(UnmanagedType.U1)]
        public bool BootIndicator;
    }
}
