// <copyright file="DismMountMode.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>


namespace Visus.DeploymentToolkit.Dism {

    /// <summary>
    /// Specifies whether an image is mounted as read-only or as read-write.
    /// </summary>
    public enum DismMountMode {

        /// <summary>
        /// Mounts an image in read-write mode.
        /// </summary>
        ReadWrite = 0,

        /// <summary>
        /// Mounts an image in read-only mode.
        /// </summary>
        ReadOnly = 1
    }
}
