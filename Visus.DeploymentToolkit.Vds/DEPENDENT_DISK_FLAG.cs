// <copyright file="DEPENDENT_DISK_FLAG.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;


namespace Visus.DeploymentToolkit.Vds {

    [Flags]
    public enum DEPENDENT_DISK_FLAG : uint {

        /// <summary>
        /// No flags specified. Use system defaults.
        /// </summary>
        NONE = 0x00000000,

        /// <summary>
        /// Multiple files backing the virtual disk.
        /// </summary>
        MULT_BACKING_FILES = 0x00000001,

        /// <summary>
        /// Fully allocated virtual disk.
        /// </summary>
        FULLY_ALLOCATED = 0x00000002,

        /// <summary>
        /// Read-only virtual disk.
        /// </summary>
        READ_ONLY = 0x00000004,

        /// <summary>
        /// The backing file of the virtual disk is not on a local physical
        /// disk.
        /// </summary>
        REMOTE = 0x00000008,

        /// <summary>
        /// Reserved.
        /// </summary>
        SYSTEM_VOLUME = 0x00000010,

        /// <summary>
        /// The backing file of the virtual disk is on the system volume.
        /// </summary>
        SYSTEM_VOLUME_PARENT = 0x00000020,

        /// <summary>
        /// The backing file of the virtual disk is on a removable physical
        /// disk.
        /// </summary>
        REMOVABLE = 0x00000040,

        /// <summary>
        /// Drive letters are not automatically assigned to the volumes on
        /// the virtual disk.
        /// </summary>
        NO_DRIVE_LETTER = 0x00000080,

        /// <summary>
        /// The virtual disk is a parent of a differencing chain.
        /// </summary>
        PARENT = 0x00000100,

        /// <summary>
        /// The virtual disk is not attached to the local host. For example, it
        /// is attached to a guest virtual machine.
        /// </summary>
        NO_HOST_DISK = 0x00000200,

        /// <summary>
        /// The lifetime of the virtual disk is not tied to any application or
        /// process.
        /// </summary>
        PERMANENT_LIFETIME = 0x00000400,

        SUPPORT_COMPRESSED_VOLUMES,

        ALWAYS_ALLOW_SPARSE,

        SUPPORT_ENCRYPTED_FILES
    }

}
