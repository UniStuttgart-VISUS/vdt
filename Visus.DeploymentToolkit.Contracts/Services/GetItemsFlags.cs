// <copyright file="GetItemsFlags.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// Customises the behaviour of the <see cref="IDirectory.GetItemsAsync"/>.
    /// </summary>
    [Flags]
    public enum GetItemsFlags {

        /// <summary>
        /// Retrieves everything that is directly in the selected directory.
        /// </summary>
        None = 0,

        /// <summary>
        /// Only return files, not directories.
        /// </summary>
        FilesOnly = 0x00000001,

        /// <summary>
        /// Recursively retrieves items in subdirectories, too.
        /// </summary>
        Recursive = 0x00000002,
    }
}
