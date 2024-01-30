// <copyright file="VDS_DISK_STATUS.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>


namespace Visus.DeploymentToolkit.Vds {

    /// <summary>
    /// Defines the set of object status values for a disk.
    /// </summary>
    public enum VDS_DISK_STATUS : uint {

        /// <summary>
        /// The provider failed to get the disk properties from the driver
        /// (unknown status, unknown health), or the provider cannot access
        /// the disk (unknown status, healthy).
        /// </summary>
        UNKNOWN = 0,

        /// <summary>
        /// The disk is available. The disk status value can be
        /// <see cref="ONLINE"/>, even if the status of the containing pack
        /// is VDS_PS_OFFLINE.
        /// </summary>
        ONLINE = 1,

        /// <summary>
        /// The disk is currently not ready to use. For example, if you use ACPI
        /// Power Management to request that a disk hibernate (spin down), the
        /// disk becomes temporarily unavailable.
        /// </summary>
        NOT_READY = 2,

        /// <summary>
        /// The disk is removable media, such as a CD-ROM drive, or contains no
        /// media.
        /// </summary>
        NO_MEDIA = 3,

        /// <summary>
        /// The disk is unavailable and cannot be used.
        /// </summary>
        FAILED = 5,

        /// <summary>
        /// No physical device is present for the disk object, even though the
        /// pack configuration information lists the disk. This status value
        /// applies to dynamic disks only.
        /// </summary>
        MISSING = 6,

        /// <summary>
        /// The disk is offline.
        /// </summary>
        OFFLINE = 4
    }
}
