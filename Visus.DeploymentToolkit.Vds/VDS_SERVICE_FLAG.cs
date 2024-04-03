// <copyright file="VDS_SERVICE_FLAG.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;


namespace Visus.DeploymentToolkit.Vds {

    /// <summary>
    /// Defines the set of valid flags for the service object.
    /// </summary>
    [Flags]
    public enum VDS_SERVICE_FLAG : uint {

        /// <summary>
        /// If set, the service supports dynamic disks.
        /// </summary>
        SUPPORT_DYNAMIC = 0x1,

        /// <summary>
        /// If set, the service supports fault-tolerant volumes.
        /// </summary>
        SUPPORT_FAULT_TOLERANT = 0x2,

        /// <summary>
        /// If set, the service supports GPT disks.
        /// </summary>
        SUPPORT_GPT = 0x4,

        /// <summary>
        /// If set, the service supports dynamic 1394 disks.
        /// </summary>
        SUPPORT_DYNAMIC_1394 = 0x8,

        /// <summary>
        /// If set, the host has the cluster service installed and configured,
        /// but not necessarily running.
        /// </summary>
        CLUSTER_SERVICE_CONFIGURED = 0x10,

        /// <summary>
        /// If set, the auto-mount operation is turned off for the computer to
        /// prevent the operating system from automatically mounting new
        /// partitions.
        /// </summary>
        /// <remarks>
        /// Beginning with Windows 8 and Windows Server 2012, this flag is
        /// deprecated.
        /// </remarks>
        AUTO_MOUNT_OFF = 0x20,

        /// <summary>
        /// If set, configuration changes to VDS have occurred. After a
        /// successful installation, the uninstall operation is valid only if 
        /// the configuration changes.
        /// </summary>
        OS_UNINSTALL_VALID = 0x40,

        /// <summary>
        /// If set, the machine boots from an EFI partition on a GPT disk.
        /// </summary>
        EFI = 0x80,

        /// <summary>
        /// The service supports mirrored volumes.
        /// </summary>
        SUPPORT_MIRROR = 0x100,

        /// <summary>
        /// The service supports RAID-5 volumes.
        /// </summary>
        SUPPORT_RAID5 = 0x200,

        /// <summary>
        /// The service supports ReFS volumes.
        /// </summary>
        SUPPORT_REFS = 0x400
    }
}
