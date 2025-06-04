// <copyright file="DISK_EXTENT.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Diagnostics;
using System.Runtime.InteropServices;


namespace Visus.DeploymentToolkit.DiskManagement {

    /// <summary>
    /// Contains information about a device. This structure is used by the
    /// <see cref="Kernel32.IOCTL_STORAGE_GET_DEVICE_NUMBER"/> control code.
    /// </summary>
    [DebuggerDisplay("Device = {DeviceNumber}, Partition = {PartitionNumber}")]
    [StructLayout(LayoutKind.Sequential)]
    internal struct STORAGE_DEVICE_NUMBER {

        /// <summary>
        /// The type of device. Values from 0 through 32,767 are reserved for
        /// use by Microsoft. Values from 32,768 through 65,535 are reserved for
        /// use by other vendors.
        /// </summary>
        public uint DeviceType;

        /// <summary>
        /// The number of this device.
        /// </summary>
        public uint DeviceNumber;

        /// <summary>
        /// The partition number of the device, if the device can be partitioned.
        /// Otherwise, this member is <see cref="uint.MaxValue"/>.
        /// </summary>
        public uint PartitionNumber;
    }
}
