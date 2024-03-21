// <copyright file="ICopy.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Threading.Tasks;


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// Provides services for copying files.
    /// </summary>
    public interface ICopy {

        /// <summary>
        /// Copies one or more files from <paramref name="source"/> to
        /// <paramref name="destination"/>.
        /// </summary>
        /// <param name="source">The source to be copied, which can be a file
        /// or a directory.</param>
        /// <param name="destination">The destination of the copy, which can be
        /// a file only if <paramref name="source"/> is a file as well.
        /// Otherwise, it must be a directory.</param>
        /// <param name="flags">Allows for controlling the behaviour, eg whether
        /// existing files are overwritten or whether the directory structure
        /// is copied recursively.</param>
        /// <returns>A task for waiting for the opration to complete.</returns>
        Task CopyAsync(string source, string destination, CopyFlags flags);
    }
}
