// <copyright file="VDS_FILE_SYSTEM_TYPE_PROP.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Runtime.InteropServices;


namespace Visus.DeploymentToolkit.Vds {

    /// <summary>
    /// Defines the properties of a file system type.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct VDS_FILE_SYSTEM_TYPE_PROP {

        /// <summary>
        /// The file system types enumerated by
        /// <see cref="VDS_FILE_SYSTEM_TYPE"/>. Valid types are FAT, FAT32,
        /// NTFS, CDFS and UDF.
        /// </summary>
        public VDS_FILE_SYSTEM_TYPE Type;

        /// <summary>
        /// The file system name.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
        public string Name;

        /// <summary>
        /// The file system flags enumerated by
        /// <see cref="VDS_FILE_SYSTEM_FLAG"/>.
        /// </summary>
        public VDS_FILE_SYSTEM_FLAG Flags;

        /// <summary>
        /// The valid allocation unit sizes used for compression.
        /// </summary>
        public uint CompressionFlags;

        /// <summary>
        /// The maximum length for a label name.
        /// </summary>
        public uint MaxLableLength;

        /// <summary>
        /// A string containing all characters that are not valid for this file
        /// system type.
        /// </summary>
        public string IllegalLabelCharSet;
    }
}
