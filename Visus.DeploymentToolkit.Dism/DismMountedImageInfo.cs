// <copyright file="DismMountedImageInfo.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Runtime.InteropServices;


namespace Visus.DeploymentToolkit.Dism {

    /// <summary>
    /// Describes the metadata of a mounted image.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct DismMountedImageInfo {

        /// <summary>
        /// A relative or absolute path to the mounted image.
        /// </summary>
        public string MountPath;

        /// <summary>
        /// A relative or absolute path to the image file.
        /// </summary>
        public string ImageFilePath;

        /// <summary>
        /// The index number of the image. Index numbering starts at 1.
        /// </summary>
        public uint ImageIndex;

        /// <summary>
        /// A <see cref="DismMountMode"/> enumeration value representing whether
        /// the image is <see cref="DismMountMode.ReadWrite"/> or
        /// <see cref="DismMountMode.ReadOnly"/> .
        /// </summary>
        public DismMountMode MountMode;

        /// <summary>
        /// A <see cref="DismMountStatus"/> enumeration value such as
        /// <see cref="DismMountStatus.OK"/>.
        /// </summary>
        public DismMountStatus MountStatus;
    }
}
