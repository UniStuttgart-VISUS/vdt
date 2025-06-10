// <copyright file="DRIVE_LAYOUT_INFORMATION_MBR.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Runtime.InteropServices;


namespace Visus.DeploymentToolkit.DiskManagement {

    /// <summary>
    /// Provides information about a drive's master boot record (MBR) partitions.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct DRIVE_LAYOUT_INFORMATION_MBR {

        /// <summary>
        /// The signature of the drive.
        /// </summary>
        public uint Signature;

        /// <summary>
        /// From winoctl.h: ABRACADABRA_WIN10_RS1
        /// </summary>
        public uint CheckSum;
    }
}
