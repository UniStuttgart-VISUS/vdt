// <copyright file="IDiskManagement.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 - 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.DiskManagement;


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// Defines the interface of the disk management service, which interfaces
    /// with the virtual disk service to provide enumeration capabilities for
    /// disks. Furthermore, the disk management service allows for partitioning
    /// and formatting disks as well.
    /// </summary>
    public interface IDiskManagement {

        /// <summary>
        /// Gets the disk with the specified unique ID.
        /// </summary>
        /// <param name="id">The unique ID of the disk to retrieve.</param>
        /// <param name="cancellationToken">A cancellation token for aborting
        /// the operation.</param>
        /// <returns>An object representing the requested disk or <c>null</c>
        /// if no such disk was found.</returns>
        Task<IDisk?> GetDiskAsync(Guid id, CancellationToken cancellationToken);

        /// <summary>
        /// Gets a list of all known disks in the system.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token for aborting
        /// the operation.</param>
        /// <returns>The list of disks in the system.</returns>
        Task<IEnumerable<IDisk>> GetDisksAsync(
            CancellationToken cancellationToken);
    }
}
