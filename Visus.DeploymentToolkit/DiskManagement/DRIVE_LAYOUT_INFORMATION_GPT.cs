// <copyright file="DRIVE_LAYOUT_INFORMATION_GPT.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Runtime.InteropServices;


namespace Visus.DeploymentToolkit.DiskManagement {

    /// <summary>
    /// Contains information about a drive's GUID partition table (GPT) partitions.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct DRIVE_LAYOUT_INFORMATION_GPT {

        /// <summary>
        /// The GUID of the disk.
        /// </summary>
        public Guid DiskID;

        /// <summary>
        /// The starting byte offset of the first usable block.
        /// </summary>
        public long StartingUsableOffset;

        /// <summary>
        /// The size of the usable blocks on the disk, in bytes.
        /// </summary>
        public long UsableLength;

        /// <summary>
        /// The maximum number of partitions that can be defined in the usable block.
        /// </summary>
        public uint MaxPartitionCount;
    }
}
