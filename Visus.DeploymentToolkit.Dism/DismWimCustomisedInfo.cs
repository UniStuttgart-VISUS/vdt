// <copyright file="DismWimCustomisedInfo.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Runtime.InteropServices;


namespace Visus.DeploymentToolkit.Dism {

    /// <summary>
    /// Describes a Windows Imaging Format (WIM) file.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct DismWimCustomisedInfo {

        /// <summary>
        /// The size of the <see cref="DismWimCustomisedInfo" /> structure.
        /// </summary>
        public uint Size;

        /// <summary>
        /// The number of directories in the image.
        /// </summary>
        public uint DirectoryCount;

        /// <summary>
        /// The number of files in the image.
        /// </summary>
        public uint FileCount;

        /// <summary>
        /// The time that the image file was created.
        /// </summary>
        public SystemTime CreatedTime;

        /// <summary>
        /// The time that the image file was last modified.
        /// </summary>
        public SystemTime ModifiedTime;
    }
}
