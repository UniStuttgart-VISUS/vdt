// <copyright file="IDirectory.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Collections.Generic;
using System.IO;
using System.IO.Enumeration;
using System.Threading.Tasks;


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// Provides services around manipulating directories.
    /// </summary>
    public interface IDirectory {

        /// <summary>
        /// Deletes all content in the specified directory, but does not delete
        /// the directory itself.
        /// </summary>
        /// <param name="path">The path to the directory to be cleaned.</param>
        /// <returns>A task to wait for the operation to complete.</returns>
        Task CleanAsync(string path);

        /// <summary>
        /// Creates a directory at the given location, including any necessary
        /// subdirectories, unless they already exist.
        /// </summary>
        /// <param name="path">The path to the directory to be created.</param>
        /// <returns>An object that represents the directory at the specified
        /// path. This object is returned regardless of whether a directory at
        /// the specified path already exists.</returns>
        Task<DirectoryInfo> CreateAsync(string path);

        /// <summary>
        /// Deletes the specified directory and, if indicated, any
        /// subdirectories and files in the directory.
        /// </summary>
        /// <param name="path">The path of the directory to be removed.</param>
        /// <param name="recursive">If <see langword="true"/>, files and
        /// directories in <paramref name="path"/> will be deleted, too.
        /// Otherwise, theoperation will fail if the directory is not empty.
        /// </param>
        /// <returns>A task to wait for the operation to complete.</returns>
        Task DeleteAsync(string path, bool recursive = false);

        /// <summary>
        /// Indicates whether <paramref name="path"> exists and is a directory.
        /// </summary>
        /// <param name="path">The path to check.</param>
        /// <returns><see langword="true"/> if <paramref name="path"/>
        /// designates an existing directory, <see langword="false"/> otherwise.
        /// </returns>
        bool Exists(string? path);

        /// <summary>
        /// Gets all contents of the specified directory.
        /// </summary>
        /// <param name="path">The path of the directory to enumerate the
        /// contents of. if this is <see langword="null"/>, the current working
        /// directory is assumed.</param>
        /// <param name="pattern">An optional pattern that the entries to be
        /// returned must match.</param>
        /// <param name="flags">Customises the behaviour of the method.</param>
        /// <returns>The items in the given directory.</returns>
        IEnumerable<FileSystemInfo> GetItems(string? path,
            string? pattern= null,
            GetItemsFlags flags = GetItemsFlags.None);
    }
}
