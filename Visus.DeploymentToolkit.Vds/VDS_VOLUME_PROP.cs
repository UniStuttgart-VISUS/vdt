// <copyright file="VDS_VOLUME_PROP.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Runtime.InteropServices;


namespace Visus.DeploymentToolkit.Vds {

    /// <summary>
    /// Defines the properties of a volume object.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct VDS_VOLUME_PROP {

        /// <summary>
        /// The GUID of the volume.
        /// </summary>
        public Guid ID;

        /// <summary>
        /// A <see cref="VDS_VOLUME_TYPE"/> enumeration value that specifies the
        /// type of the volume. Volume types are simple, spanned, striped
        /// (RAID-0), mirrored, or striped with parity (RAID-5).
        /// </summary>
        public VDS_VOLUME_TYPE Type;

        /// <summary>
        /// A <see cref="VDS_VOLUME_STATUS"/> enumeration value that specifies
        /// the status of the volume.
        /// </summary>
        public VDS_VOLUME_STATUS Status;

        /// <summary>
        /// A <see cref="VDS_HEALTH"/> enumeration value that specifies the
        /// health state of the volume.
        /// </summary>
        public VDS_HEALTH Health;

        /// <summary>
        /// A <see cref="VDS_TRANSITION_STATE"/> enumeration value that
        /// specifies the transition state of the volume.
        /// </summary>
        public VDS_TRANSITION_STATE TransitionState;

        /// <summary>
        /// The size of the volume, in bytes.
        /// </summary>
        public ulong Size;

        /// <summary>
        /// A bitmask of<see cref="VDS_VOLUME_FLAG"/> enumeration values that
        /// describe the volume.
        /// </summary>
        public VDS_VOLUME_FLAG Flags;

        /// <summary>
        /// A <see cref="VDS_FILE_SYSTEM_TYPE"/> enumeration value that
        /// specifies the preferred file system for the volume. Must be one of
        /// the following: VDS_FST_NTFS, VDS_FST_FAT, VDS_FST_FAT32,
        /// VDS_FST_UDF, VDS_FST_CDFS, or VDS_FST_UNKNOWN.
        /// </summary>
        public VDS_FILE_SYSTEM_TYPE RecommendedFileSystemType;

        /// <summary>
        /// The name used to open a handle for the volume with the CreateFile
        /// function. For example, \?\GLOBALROOT\Device\HarddiskVolume1.
        /// </summary>
        public string Name;
    }
}
