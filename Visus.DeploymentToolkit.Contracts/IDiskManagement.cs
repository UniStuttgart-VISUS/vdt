// <copyright file="IDiskManagement.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Contracts.DiskManagement;


namespace Visus.DeploymentToolkit.Contracts {

    /// <summary>
    /// Defines the interface of the disk management service.
    /// </summary>
    public interface IDiskManagement {

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
