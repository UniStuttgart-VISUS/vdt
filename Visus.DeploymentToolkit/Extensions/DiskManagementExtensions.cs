// <copyright file="DiskManagementExtensions.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.DiskManagement;
using Visus.DeploymentToolkit.Services;


namespace Visus.DeploymentToolkit.Extensions {

    /// <summary>
    /// Extensions for the <see cref="IDiskManagement"/> service.
    /// </summary>
    public static class DiskManagementExtensions {

        /// <summary>
        /// Gets the disk with the specified unique ID.
        /// </summary>
        /// <param name="that">The disk management service to get the drive
        /// from.</param>
        /// <param name="id">The unique ID of the disk to retrieve.</param>
        /// <param name="cancellationToken">A cancellation token for aborting
        /// the operation.</param>
        /// <returns>An object representing the requested disk or <c>null</c>
        /// if no such disk was found.</returns>
        public static async Task<IDisk?> GetDiskAsync(
                this IDiskManagement that,
                Guid id,
                CancellationToken cancellationToken) {
            if (that is null) {
                return null;
            }

            var disks = await that.GetDisksAsync(cancellationToken)
                .ConfigureAwait(false);
            return disks.Where(d => d.ID == id).SingleOrDefault();
        }

        /// <summary>
        /// Gets all disks that have at least one partition of the specified
        /// type.
        /// </summary>
        /// <param name="that">The disk management service to get the drives
        /// from.</param>
        /// <param name="partitionType">The type of partition to be searched.
        /// </param>
        /// <param name="cancellationToken">A cancellation token for aborting
        /// the operation.</param>
        /// <returns>A list of disks with matching partitions.</returns>
        /// <exception cref="ArgumentNullException">If the
        /// <paramref name="partitionType"/> is <see langword="null"/>.
        /// </exception>
        public static async Task<IEnumerable<IDisk>> GetDisksAsync(
                this IDiskManagement that,
                PartitionType partitionType,
                CancellationToken cancellationToken) {
            if (that is null) {
                return Enumerable.Empty<IDisk>();
            }

            ArgumentNullException.ThrowIfNull(partitionType);
            var disks = await that.GetDisksAsync(cancellationToken)
                .ConfigureAwait(false);
            return disks.Where(d => d.Partitions.Any(
                p => partitionType.Equals(p.Type)));
        }
    }
}
