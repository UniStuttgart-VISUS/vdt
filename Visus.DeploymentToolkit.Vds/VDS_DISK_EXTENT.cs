// <copyright file="VDS_DISK_EXTENT.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Runtime.InteropServices;


namespace Visus.DeploymentToolkit.Vds {

    /// <summary>
    /// Defines the properties of a disk extent.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct VDS_DISK_EXTENT {

        /// <summary>
        /// The GUID of the disk.
        /// </summary>
        public Guid DiskId;

        /// <summary>
        /// A <see cref="VDS_DISK_EXTENT_TYPE"/> enumeration value that
        /// specifies the type of the disk extent.
        /// </summary>
        public VDS_DISK_EXTENT_TYPE Type;

        /// <summary>
        /// The disk offset, in bytes.
        /// </summary>
        public ulong Offset;

        /// <summary>
        /// The size of the extent, in bytes.
        /// </summary>
        public ulong Size;

        /// <summary>
        /// The GUID of the volume to which the extent belongs.
        /// </summary>
        public Guid VolumeId;

        /// <summary>
        /// If the extent is from a volume, this member is the GUID of the
        /// plex to which the extent belongs.
        /// </summary>
        public Guid PlexId;

        /// <summary>
        /// If the extent is from a volume plex, this member is the zero-based
        /// index of the plex member to which the extent belongs.
        /// </summary>
        public uint MemberIdx;
    }
}
