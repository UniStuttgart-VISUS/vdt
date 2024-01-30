// <copyright file="VDS_HEALTH.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>


namespace Visus.DeploymentToolkit.Vds {

    /// <summary>
    /// Defines the set of health state values for a VDS object.
    /// </summary>
    public enum VDS_HEALTH : uint {

        /// <summary>
        /// The health of the object cannot be determined.
        /// </summary>
        UNKNOWN = 0,

        /// <summary>
        /// The object indicates online status. This health state value means
        /// that the object is fully operational and is operating properly, but
        /// it does not imply that the object is available for use. For example,
        /// if the object is a disk, the disk is not missing, log and
        /// configuration files are synchronised, and the disk is free of I/O
        /// errors. If the object is a LUN or volume, all plexes (mirrored,
        /// simple, spanned, and striped) and columns (RAID-5) are available and
        /// free of I/O errors. The status value associated with this health
        /// state must not be <see cref="FAILED"/>, <see cref="UNKNOWN"/>, or
        /// <see cref="MISSING"/>.
        /// </summary>
        HEALTHY = 1,

        /// <summary>
        /// Either a mirrored LUN or volume is resynching all plexes, or a
        /// striped with parity (RAID-5) plex is regenerating the parity.
        /// </summary>
        REBUILDING = 2,

        /// <summary>
        /// The object configuration is stale. The status value must not be
        /// <see cref="FAILED"/> or <see cref="UNKNOWN"/>.
        /// </summary>
        STALE = 3,

        /// <summary>
        /// The object is failing, but still working. For example, a LUN or
        /// volume with failing health might be producing occasional
        /// input/output errors from which it is still able to recover. The
        /// status value must not be <see cref="FAILED"/> or
        /// <see cref="UNKNOWN"/>.
        /// </summary>
        FAILING = 4,

        /// <summary>
        /// One or more plexes have errors, but the object is working and all
        /// plexes are online. This value is valid only for volumes and LUNs.
        /// </summary>
        FAILING_REDUNDANCY = 5,

        /// <summary>
        /// One or more plexes have failed, but at least one plex is working.
        /// This value is valid only for volumes and LUNs.
        /// </summary>
        FAILED_REDUNDANCY = 6,

        /// <summary>
        /// The last working plex is failing. This value is valid only for
        /// volumes and LUNs.
        /// </summary>
        FAILED_REDUNDANCY_FAILING = 7,

        /// <summary>
        /// The object has failed. Any object with a failed health status also
        /// has a failed object status. Therefore, the status value must be
        /// <see cref="FAILED"/>.
        /// </summary>
        FAILED = 8,

        /// <summary>
        /// This value is reserved. Do not use it.
        /// </summary>
        REPLACED = 9,

        /// <summary>
        /// The object is not failing, but it is expected to fail according to
        /// analysis done on the object's attributes. For example, a disk may be
        /// set to <see cref="PENDING_FAILURE"/> based on S.M.A.R.T. data. The
        /// status value must not be <see cref="FAILED "/> or
        /// <see cref="UNKNOWN"/>.
        /// </summary>
        PENDING_FAILURE = 10,

        /// <summary>
        /// The object has not completely failed but is experiencing failures.
        /// If the object is a subsystem object, the firmware may be reporting
        /// errors, or the drive, controller, port, or path sub-object may have
        /// failed or be failing. If the object is a controller object, the
        /// firmware may be reporting errors, or the port or path sub-object may
        /// have failed or be failing. If the object is a storage pool object,
        /// one or more drives may have failed or be failing. The status value
        /// must not be <see cref="UNKNOWN"/>.
        /// </summary>
        DEGRADED = 11
    }
}
