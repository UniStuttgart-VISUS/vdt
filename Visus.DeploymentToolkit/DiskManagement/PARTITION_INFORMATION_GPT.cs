// <copyright file="PARTITION_INFORMATION_GPT.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Runtime.InteropServices;


namespace Visus.DeploymentToolkit.DiskManagement {

    /// <summary>
    /// Contains partition information specific to master boot record (MBR)
    /// disks.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct PARTITION_INFORMATION_GPT {

        /// <summary>
        /// A GUID that identifies the partition type.
        /// </summary>
        public Guid PartitionType;

        /// <summary>
        /// The GUID of the partition.
        /// </summary>
        public Guid PartitionID;

        /// <summary>
        /// The Extensible Firmware Interface (EFI) attributes of the partition.
        /// </summary>
        public ulong Attributes;

        /// <summary>
        /// A wide-character string that describes the partition.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 36)]
        public string Name;
    }
}
