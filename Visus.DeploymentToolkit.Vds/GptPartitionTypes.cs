// <copyright file="GptPartitionTypes.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>


using System;

namespace Visus.DeploymentToolkit.Vds {

    /// <summary>
    /// Enumerates som well-known GPT partition types.
    /// </summary>
    public static class GptPartitionTypes {

        /// <summary>
        /// The data partition type that is created and recognised by Windows.
        /// </summary>
        /// <remarks>
        /// <para>Only partitions of this type can be assigned drive letters,
        /// receive volume GUID paths, host mounted folders (also called volume
        /// mount points) and be enumerated by calls to FindFirstVolume and
        /// FindNextVolume.</para>
        /// <para>This value can be set only for basic disks, with one
        /// exception. If both PARTITION_BASIC_DATA_GUID and
        /// GPT_ATTRIBUTE_PLATFORM_REQUIRED are set for a partition on a basic
        /// disk that is subsequently converted to a dynamic disk, the partition
        /// remains a basic partition, even though the rest of the disk is a
        /// dynamic disk. This is because the partition is considered to be an
        /// OEM partition on a GPT disk.</para>
        /// </remarks>
        public static readonly Guid BasicData
            = new Guid("ebd0a0a2-b9e5-4433-87c0-68b6b72699c7");

        /// <summary>
        /// The partition is an LDM data partition on a dynamic disk.
        /// </summary>
        /// <remarks>
        /// This value can be set only for dynamic disks.
        /// </remarks>
        public static readonly Guid LdmData
            = new Guid("af9b60a0-1431-4f62-bc68-3311714a69ad");

        /// <summary>
        /// The partition is a Logical Disk Manager (LDM) metadata partition on
        /// a dynamic disk.
        /// </summary>
        /// <remarks>
        /// This value can be set only for dynamic disks.
        /// </remarks>
        public static readonly Guid LdmMetadata
            = new Guid("5808c8aa-7e8f-42e0-85d2-e1e90434cfb3");

        /// <summary>
        /// The partition is a Microsoft recovery partition.
        /// </summary>
        /// <remarks>
        /// This attribute can be set for basic and dynamic disks.
        /// </remarks>
        public static readonly Guid MicrosoftRecovery
            = new Guid("de94bba4-06d1-4d40-a16a-bfd50179d6ac");

        /// <summary>
        /// The partition is a Microsoft reserved partition.
        /// </summary>
        /// <remarks>
        /// This attribute can be set for basic and dynamic disks.
        /// </remarks>
        public static readonly Guid MicrosoftReserved
            = new Guid("e3c9e316-0b5c-4db8-817d-f92df00215ae");

        /// <summary>
        /// The partition is an EFI system partition.
        /// </summary>
        /// <remarks>
        /// This attribute can be set for basic and dynamic disks.
        /// </remarks>
        public static readonly Guid System
            = new Guid("c12a7328-f81f-11d2-ba4b-00a0c93ec93b");

        /// <summary>
        /// There is no partition.
        /// </summary>
        /// <remarks>
        /// This attribute can be set for basic and dynamic disks.
        /// </remarks>
        public static readonly Guid Unused
            = new Guid("00000000-0000-0000-0000-000000000000");

    }
}
