// <copyright file="VDS_FILE_SYSTEM_PROP.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Runtime.InteropServices;


namespace Visus.DeploymentToolkit.Vds {

    /// <summary>
    /// Defines the properties of a file system.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct VDS_FILE_SYSTEM_PROP {

        /// <summary>
        /// The file-system type enumerated by
        /// <see cref="VDS_FILE_SYSTEM_TYPE" />.
        /// </summary>
        public VDS_FILE_SYSTEM_TYPE Type;

        /// <summary>
        /// The GUID of the volume object containing the file system.
        /// </summary>
        public Guid VolumeId;

        /// <summary>
        /// The file-system flags enumerated by
        /// <see cref="VDS_FILE_SYSTEM_PROP_FLAG"/>.
        /// </summary>
        public VDS_FILE_SYSTEM_PROP_FLAG Flags;

        /// <summary>
        /// The total number of allocation units.
        /// </summary>
        public ulong TotalAllocationUnits;

        /// <summary>
        /// The unused allocation units.
        /// </summary>
        public ulong AvailableAllocationUnits;

        /// <summary>
        /// The allocation unit size, in bytes, for the file system, which is
        /// usually between 512 and 4096.
        /// </summary>
        public uint AllocationUnitSize;

        /// <summary>
        /// A string containing the file-system label.
        /// </summary>
        public string Label;
    }
}
