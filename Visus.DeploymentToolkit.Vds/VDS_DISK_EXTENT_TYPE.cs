// <copyright file="VDS_DISK_EXTENT.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>


namespace Visus.DeploymentToolkit.Vds {

    /// <summary>
    /// Defines the set of disk extents types. The type can be a partition,
    /// volume, or free space.
    /// </summary>
    public enum  VDS_DISK_EXTENT_TYPE : uint {

        /// <summary>
        /// An extent of any unknown partition.
        /// </summary>
        UNKNOWN = 0,

        /// <summary>
        /// An extent of free space, including free space inside an extended 
        /// partition.
        /// </summary>
        FREE = 1,

        /// <summary>
        /// An extent of any volume.
        /// </summary>
        DATA = 2,

        /// <summary>
        /// An extent of an OEM partition.
        /// </summary>
        OEM = 3,

        /// <summary>
        /// An extent of an ESP partition.
        /// </summary>
        ESP = 4,

        /// <summary>
        /// An extent of a MSR partition.
        /// </summary>
        MSR = 5,

        /// <summary>
        /// An extent of a LDM metadata partition.
        /// </summary>
        LDM = 6,

        /// <summary>
        /// An extent of a cluster metadata partition.
        /// </summary>
        CLUSTER = 7,

        /// <summary>
        /// An extent of unusable space on a disk. That is, space outside
        /// the four primary partitions (or three primary partitions plus one
        /// extended partition) on a basic MBR disk and space outside the
        /// dynamic disk public region.
        /// </summary>
        UNUSABLE = 0x7fff
    }
}
