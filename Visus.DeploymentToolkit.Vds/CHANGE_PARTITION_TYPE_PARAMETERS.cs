// <copyright file="CHANGE_PARTITION_TYPE_PARAMETERS.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Runtime.InteropServices;


namespace Visus.DeploymentToolkit.Vds {

    /// <summary>
    /// Defines the partition parameters of a create request.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct CHANGE_PARTITION_TYPE_PARAMETERS {

        /// <summary>
        /// Determines the style of the partition, which must either be
        /// <see cref="VDS_PARTITION_STYLE.MBR"/> or
        /// <see cref="VDS_PARTITION_STYLE.GPT"/>.
        /// </summary>
        [FieldOffset(0)]
        public VDS_PARTITION_STYLE Style;

        /// <summary>
        /// Byte value indicating the partition type to which to change the
        /// partition.
        /// </summary>
        [FieldOffset(4)]
        public byte MbrType;

        /// <summary>
        /// GUID indicating the partition type to which to change the partition.
        /// </summary>
        [FieldOffset(4)]
        public Guid GptType;
    }
}
