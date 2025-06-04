// <copyright file="VOLUME_DISK_EXTENTS.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Runtime.InteropServices;


namespace Visus.DeploymentToolkit.DiskManagement {

    /// <summary>
    /// Represents a physical location on a disk. It is the output buffer for
    /// the <see cref="Kernel32.IOCTL_VOLUME_GET_VOLUME_DISK_EXTENTS"/> control
    /// code.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct VOLUME_DISK_EXTENTS {

        /// <summary>
        /// The number of disks in the volume (a volume can span multiple
        /// disks).
        /// </summary>
        /// <remarks>
        /// An extent is a contiguous run of sectors on one disk. When the
        /// number of extents returned is greater than one, the error code 
        /// <c>ERROR_MORE_DATA</c> is returned. You should call
        /// <see cref="Kernel32.DeviceIoControl"/> again, allocating enough
        /// buffer space based on the value of <see cref="NumberOfDiskExtents"/>
        /// after the first <see cref="Kernel32.DeviceIoControl"/> call.
        /// </remarks>
        public uint NumberOfDiskExtents;

        /// <summary>
        /// An array of <see cref="DISK_EXTENT"/> structures.
        /// </summary>
        public DISK_EXTENT Extents;
    }
}
