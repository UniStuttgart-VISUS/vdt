// <copyright file="VDS_PARTITION_STYLE.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>


namespace Visus.DeploymentToolkit.Vds {

    /// <summary>
    /// Defines the set of partition style values.
    /// </summary>
    public enum VDS_PARTITION_STYLE : uint {

        /// <summary>
        /// An uninitialized disk. New disks or newly cleaned disks have this
        /// partitioning type.
        /// </summary>
        UNKNOWN = 0,

        /// <summary>
        /// The style is master boot record (MBR).
        /// </summary>
        MBR = 1,

        /// <summary>
        /// The style is GUID partition table (GPT).
        /// </summary>
        GPT = 2
    }
}
