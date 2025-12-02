// <copyright file="HashItemsFlags.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// Customises the behaviour of the <see cref="IDirectory.HashAsync"/>.
    /// </summary>
    [Flags]
    public enum HashItemsFlags {

        /// <summary>
        /// Retrieves everything that is directly in the selected directory and
        /// hashes the contents of the files.
        /// </summary>
        None = 0,

        /// <summary>
        /// Includes directory names if hashing contents. This flag has no
        /// effect if <see cref="NamesOnly"/> is set, too.
        /// </summary>
        IncludeDirectories = 0x00000001,

        /// <summary>
        /// Recursively retrieves items in subdirectories, too.
        /// </summary>
        Recursive = 0x00000002,

        /// <summary>
        /// Only hashes the names of the files and directories, not their
        /// contents. Setting this flags implies
        /// <see cref="IncludeDirectories"/>.
        /// </summary>
        NamesOnly = IncludeDirectories | 0x00000004,
    }
}
