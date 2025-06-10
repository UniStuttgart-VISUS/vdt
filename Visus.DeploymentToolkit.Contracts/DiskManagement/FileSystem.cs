// <copyright file="FileSystem.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 - 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>


namespace Visus.DeploymentToolkit.DiskManagement {

    /// <summary>
    /// Enumerates all known types of file systems.
    /// </summary>
    public enum FileSystem : uint {

        /// <summary>
        /// The Apple File System (AFS).
        /// </summary>
        [WmiFileSystem(10)]
        Afs,

        /// <summary>
        /// The file system of CD-ROMs.
        /// </summary>
        [VdsFileSystem(5)]
        Csfs,

        /// <summary>
        /// Cluster-shared volume based on NTFS.
        /// </summary>
        [VdsFileSystem(8), WmiFileSystem(0x8000)]
        CsvfsNtfs,

        /// <summary>
        /// Cluster-shared volume based on NTFS.
        /// </summary>
        [VdsFileSystem(8), WmiFileSystem(0x8001)]
        CsvfsRefs,

        /// <summary>
        /// The FAT32 successor exFAT.
        /// </summary>
        [VdsFileSystem(7)]
        ExFat,

        /// <summary>
        /// EXT2 file system (Linux).
        /// </summary>
        [WmiFileSystem(11)]
        Ext2,

        /// <summary>
        /// EXT3 file system (Linux).
        /// </summary>
        [WmiFileSystem(12)]
        Ext3,

        /// <summary>
        /// FAT.
        /// </summary>
        [VdsFileSystem(2), WmiFileSystem(4)]
        Fat,

        /// <summary>
        /// FAT-16.
        /// </summary>
        [WmiFileSystem(5)]
        Fat16,

        /// <summary>
        /// 32-bit FAT
        /// </summary>
        [VdsFileSystem(3), WmiFileSystem(6)]
        Fat32,

        /// <summary>
        /// The file system is a Hierarchical File System (HFS).
        /// </summary>
        [WmiFileSystem(3)]
        Hfs,

        /// <summary>
        /// The Windows NT file system (NTFS).
        /// </summary>
        [VdsFileSystem(4), WmiFileSystem(14)]
        Ntfs,

        /// <summary>
        /// NTFS v4.
        /// </summary>
        [WmiFileSystem(7)]
        Ntfs4,

        /// <summary>
        /// NTFS v5.
        /// </summary>
        [WmiFileSystem(8)]
        Ntfs5,

        /// <summary>
        /// The file system is an unformatted raw partition.
        /// </summary>
        [VdsFileSystem(1), WmiFileSystem(1)]
        Raw,

        /// <summary>
        /// Resilient file system (ReFS).
        /// </summary>
        [VdsFileSystem(9), WmiFileSystem(15)]
        Refs,

        /// <summary>
        /// Reiser file system (Linux).
        /// </summary>
        [WmiFileSystem(13)]
        ReiserFs,

        /// <summary>
        /// The universal disk format (UDF) file system used by DVDs.
        /// </summary>
        [VdsFileSystem(6)]
        Udf,

        /// <summary>
        /// The file system is a Unix file system (UFS).
        /// </summary>
        [WmiFileSystem(2)]
        Ufs,

        /// <summary>
        /// The file system is not known.
        /// </summary>
        [VdsFileSystem(0), WmiFileSystem(0)]
        Unknown,

        /// <summary>
        /// XFS (Linux).
        /// </summary>
        [WmiFileSystem(9)]
        Xfs
    }
}
