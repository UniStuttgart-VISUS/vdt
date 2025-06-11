// <copyright file="CREATE_PARTITION_PARAMETERS.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Runtime.InteropServices;


namespace Visus.DeploymentToolkit.Vds {

    /// <summary>
    /// Defines the partition parameters of a create request.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct CREATE_PARTITION_PARAMETERS {

        /// <summary>
        /// Determines the style of the partition, which must either be
        /// <see cref="VDS_PARTITION_STYLE.MBR"/> or
        /// <see cref="VDS_PARTITION_STYLE.GPT"/>.
        /// </summary>
        [FieldOffset(0)]
        public VDS_PARTITION_STYLE Style;

        /// <summary>
        /// Parameters for a Master Boot Record (MBR) disk. Used if
        /// <see cref="Style"/> is <see cref="VDS_PARTITION_STYLE.MBR"/>.
        /// </summary>
        /// <remarks>
        /// Alignment of 8 bytes comes from Intellisense in C++. It is not clear
        /// how reliable this is, but using a 4-byte alignment causes the type
        /// loader to fail for this structure.
        /// </remarks>
        [FieldOffset(8)]
        public MbrPartitionParameters MbrPartInfo;

        /// <summary>
        /// Parameters for a GUID Partition Table (GPT) disk. Used if
        /// <see cref="Style"/> is <see cref="VDS_PARTITION_STYLE.GPT"/>.
        /// </summary>
        /// <remarks>
        /// Alignment of 8 bytes comes from Intellisense in C++. It is not clear
        /// how reliable this is, but using a 4-byte alignment causes the type
        /// loader to fail for this structure.
        /// </remarks>
        [FieldOffset(8)]
        public VDS_PARTITION_INFO_GPT GptPartInfo;

    }
}
