// <copyright file="DISK_EXTENT.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Runtime.InteropServices;


namespace Visus.DeploymentToolkit.DiskManagement {

    /// <summary>
    /// Represents a disk extent.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct DISK_EXTENT {

        /// <summary>
        /// The number of the disk that contains this extent.
        /// </summary>
        /// <remarks>
        /// This is the same number that is used to construct the name of the
        /// disk, for example, the X in &quot;\\?\PhysicalDriveX&quot; or
        /// &quot;\\?\\HarddiskX&quot;.
        /// </remarks>
        public uint DiskNumber;

        /// <summary>
        /// The offset from the beginning of the disk to the extent, in bytes.
        /// </summary>
        public long StartingOffset;

        /// <summary>
        /// The number of bytes in this extent.
        /// </summary>
        public long ExtentLength;
    }
}
