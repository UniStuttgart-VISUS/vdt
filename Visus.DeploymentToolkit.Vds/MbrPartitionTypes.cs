// <copyright file="MbrPartitionTypes.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>


namespace Visus.DeploymentToolkit.Vds {

    /// <summary>
    /// Possible types of MBR partitions.
    /// </summary>
    public enum MbrPartitionTypes : byte {

        /// <summary>
        /// Unused entry.
        /// </summary>
        Unused = 0x00,

        /// <summary>
        /// Specifies a partition with 12-bit FAT entries.
        /// </summary>
        Fat12 = 0x01,

        /// <summary>
        /// Specifies a XENIX Type 1 partition.
        /// </summary>
        Xenix1 = 0x02,

        /// <summary>
        /// Specifies a XENIX Type 2 partition.
        /// </summary>
        Xenix2 = 0x03,

        /// <summary>
        /// Specifies a partition with 16-bit FAT entries.
        /// </summary>
        Fat16 = 0x04,

        /// <summary>
        /// Specifies an MS-DOS V4 extended partition.
        /// </summary>
        Extended = 0x05,

        /// <summary>
        /// Specifies an MS-DOS V4 huge partition.
        /// </summary>
        /// <remarks>
        /// This value indicates that there is no Microsoft file system on the
        /// partition. Use this value when creating a logical volume.
        /// </remarks>
        Huge = 0x06,

        /// <summary>
        /// Specifies an NTFS or ExFAT partition.
        /// </summary>
        Ifs = 0x07,

        /// <summary>
        /// Specifies an OS/2 Boot Manager, OPUS, or Coherent swap partition.
        /// </summary>
        Os2BootManager = 0x0A,

        /// <summary>
        /// Specifies a FAT32 partition.
        /// </summary>
        Fat32 = 0x0B,

        /// <summary>
        /// This value is not supported.
        /// </summary>
        Fat32Xint13 = 0x0C,

        /// <summary>
        /// This value is not supported.
        /// </summary>
        Xint13 = 0x0E,

        /// <summary>
        /// This value is not supported.
        /// </summary>
        Xint13Extended = 0x0F,

        /// <summary>
        /// Specifies a PowerPC Reference Platform partition.
        /// </summary>
        Prep = 0x41,

        /// <summary>
        /// Specifies a logical disk manager partition.
        /// </summary>
        Ldm = 0x42,

        /// <summary>
        /// Specifies a UNIX partition.
        /// </summary>
        Unix = 0x63,

        /// <summary>
        /// Specifies an NTFT partition.
        /// </summary>
        /// <remarks>
        /// This value is used in combination (that is, bitwise logically ORed)
        /// with the other values in this table.
        ///  </remarks>
        Ntft = 0x80
    }
}
