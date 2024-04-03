// <copyright file="VDS_VDISK_PROPERTIES.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Runtime.InteropServices;


namespace Visus.DeploymentToolkit.Vds {

    /// <summary>
    /// Defines the properties of a virtual disk.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct VDS_VDISK_PROPERTIES {

        /// <summary>
        /// Unique VDS-specific session identifier of the disk.
        /// </summary>
        public Guid Id;

        /// <summary>
        /// A <see cref="VDS_VDISK_STATE "/> enumeration value that specifies 
        /// the virtual disk state.
        /// </summary>
        public VDS_VDISK_STATE State;

        /// <summary>
        /// A pointer to a <see cref="VIRTUAL_STORAGE_TYPE"/> structure that
        /// specifies the storage device type of the virtual disk.
        /// </summary>
        public VIRTUAL_STORAGE_TYPE VirtualDeviceType;

        /// <summary>
        /// The size, in bytes, of the virtual disk.
        /// </summary>
        public ulong VirtualSize;

        /// <summary>
        /// The on-disk size, in bytes, of the virtual disk backing file.
        /// </summary>
        public ulong PhysicalSize;

        /// <summary>
        /// A <c>NULL</c>-terminated wide-character string containing the
        /// name and directory path of the backing file for the virtual disk.
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pPath;

        /// <summary>
        /// A <c>NULL</c>-terminated wide-character string containing the name
        /// and device path of the disk device object for the volume where the
        /// virtual disk resides.
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pDeviceName;

        /// <summary>
        /// A bitmask of <see cref="DEPENDENT_DISK_FLAG"/> enumeration values
        /// that specify disk dependency information.
        /// </summary>
        public DEPENDENT_DISK_FLAG DiskFlag;

        /// <summary>
        /// <c>true</c> if the virtual disk is a child virtual disk,
        /// or <c>false </c>otherwise.
        /// </summary>
        public bool bIsChild;

        /// <summary>
        /// A <c>NULL</c>-terminated wide-character string that contains an
        /// optional path to a parent virtual disk object.
        /// </summary>
        [MarshalAs(UnmanagedType.LPWStr)]
        public string pParentPath;
    }
}
