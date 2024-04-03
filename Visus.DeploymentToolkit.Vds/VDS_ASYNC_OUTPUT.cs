// <copyright file="VDS_ASYNC_OUTPUT.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Runtime.InteropServices;


namespace Visus.DeploymentToolkit.Vds {

    /// <summary>
    /// Defines the output of an async object. Output elements vary
    /// depending on the operation type.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct VDS_ASYNC_OUTPUT {

        /// <summary>
        /// Discriminant for the union.
        /// </summary>
        [FieldOffset(0)]
        public VDS_ASYNC_OUTPUT_TYPE Type;

        /// <summary>
        /// Actual offset of a created partition.
        /// </summary>
        [FieldOffset(8)]
        public ulong Offset;

        /// <summary>
        /// A volume, target, portal group, or disk object.
        /// </summary>
        /// <remarks>
        /// The object can be retrieved via
        /// <see cref="Marshal.GetObjectForIUnknown(IntPtr)"/>. The pointer
        /// itself must be release using <see cref="Marshal.Release(IntPtr)"/>
        /// separately, as retrieving the <see cref="object"/> will increment
        /// the reference count.
        /// </remarks>
        [FieldOffset(8)]
        public IntPtr Unknown;

        /// <summary>
        /// The number of bytes that were reclaimed by a shrink operation.
        /// </summary>
        [FieldOffset(8)]
        public ulong ReclaimedBytes;

        /// <summary>
        /// The ID of the volume object associated with a created partition.
        /// </summary>
        [FieldOffset(16)]
        public Guid VolumeId;
    }
}
