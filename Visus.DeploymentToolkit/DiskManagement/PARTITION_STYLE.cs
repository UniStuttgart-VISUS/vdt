// <copyright file="PARTITION_STYLE.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>


namespace Visus.DeploymentToolkit.DiskManagement {

    /// <summary>
    /// Represents the format of a partition.
    /// </summary>
    /// <remarks>
    /// This enumeration is compatible with the definition in winioctl.h. It is
    /// <i>not<i/> binary compatible with the definition of the VDS!
    /// </remarks>
    internal enum PARTITION_STYLE {

        /// <summary>
        /// Master boot record (MBR) format. This corresponds to standard
        /// AT-style MBR partitions.
        /// </summary>
        MBR,

        /// <summary>
        /// GUID Partition Table (GPT) format.
        /// </summary>
        GPT,

        /// <summary>
        /// Partition not formatted in either of the recognized formats - MBR
        /// or GPT.
        /// </summary>
        RAW
    }
}
