// <copyright file="IDirectory.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.IO;
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
        /// <param name="recursive">If <c>true</c>, files and directories in
        /// <paramref name="path"/> will be deleted, too. Otherwise, the
        /// operation will fail if the directory is not empty.</param>
        /// <returns>A task to wait for the operation to complete.</returns>
        Task DeleteAsync(string path, bool recursive = false);

        /// <summary>
        /// Indicates whether <paramref name="path"> exists and is a directory.
        /// </summary>
        /// <param name="path">The path to check.</param>
        /// <returns><c>true</c> if <paramref name="path"/> designates an
        /// existing directory, <c>false</c> otherwise.</returns>
        bool Exists(string? path);
    }
}
