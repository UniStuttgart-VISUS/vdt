// <copyright file="IVdsVDisk.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Runtime.InteropServices;


namespace Visus.DeploymentToolkit.Vds {

    /// <summary>
    /// Defines methods for managing a virtual disk.
    /// </summary>
    [ComImport]
    [Guid("1e062b84-e5e6-4b4b-8a25-67b81e8f13e8")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IVdsVDisk {

        /**********************************************************************\
         * WARNING: It is important that the methods are in the same order as *
         * in the native interface!                                           *
        \**********************************************************************/

        /// <summary>
        /// Opens a handle to the specified virtual disk file and returns an
        /// <see cref="IVdsOpenVDisk"/> interface pointer to the object that
        /// represents the opened handle.
        /// </summary>
        [Obsolete("This is not correctly mapped")]
        void Open(/*VIRTUAL_DISK_ACCESS_MASK accessMask,
            OPEN_VIRTUAL_DISK_FLAG flags,
            uint readWriteDepth,
            out IVdsOpenVDisk openVDisk*/);

        /// <summary>
        /// Returns disk property information for the volume where the virtual
        /// disk resides.
        /// </summary>
        /// <param name="diskProperties"></param>
        void GetProperties(out VDS_VDISK_PROPERTIES diskProperties);

        /// <summary>
        /// Returns an interface pointer to the volume object for the volume
        /// where the virtual disk resides.
        /// </summary>
        [Obsolete("This is not correctly mapped")]
        void GetHostVolume(/* out IVdsVolume volume */);

        /// <summary>
        /// Returns the device name for the volume where the virtual disk
        /// resides.
        /// </summary>
        /// <param name="deviceName"></param>
        void GetDeviceName(out string deviceName);
    }
}
