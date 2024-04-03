// <copyright file="IVolume.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Collections.Generic;


namespace Visus.DeploymentToolkit.DiskManagement {

    /// <summary>
    /// Represents a volume on an <see cref="IDisk"/>.
    /// </summary>
    public interface IVolume {

        /// <summary>
        /// Gets the file system of the volume.
        /// </summary>
        FileSystem FileSystem { get; }

        /// <summary>
        /// Gets the optional label of the volume.
        /// </summary>
        string? Label { get; }

        /// <summary>
        /// Gets the mount points of the volume, including drive letters.
        /// </summary>
        IEnumerable<string> Mounts { get; }

        /// <summary>
        /// Gets the name of the device that might be opened.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the size of the volume in bytes.
        /// </summary>
        ulong Size { get; }
    }
}
