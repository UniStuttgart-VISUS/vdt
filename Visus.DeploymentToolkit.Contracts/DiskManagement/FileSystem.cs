// <copyright file="FileSystem.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>


namespace Visus.DeploymentToolkit.DiskManagement {

    /// <summary>
    /// Enumerates all known types of file systems.
    /// </summary>
    /// <remarks>
    /// This enumeration is value-compatible with <c>VDS_FILE_SYSTEM_TYPE</c>.
    /// </remarks>
    public enum FileSystem : uint {

        /// <summary>
        /// The file system is not known.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// The file system is an unformatted raw partition.
        /// </summary>
        Raw,

        /// <summary>
        /// FAT.
        /// </summary>
        Fat,

        /// <summary>
        /// 32-bit FA:
        /// </summary>
        Fat32,

        /// <summary>
        /// NTFS.
        /// </summary>
        Ntfs,

        /// <summary>
        /// CDFS.
        /// </summary>
        Cdfs,

        /// <summary>
        /// Universal Disk Format (DVD).
        /// </summary>
        Udf,

        /// <summary>
        /// Extended FAT.
        /// </summary>
        ExFat,

        /// <summary>
        /// Windows cluster file system.
        /// </summary>
        Csvfs,

        /// <summary>
        /// Resilient file system.
        /// </summary>
        Refs
    }
}
