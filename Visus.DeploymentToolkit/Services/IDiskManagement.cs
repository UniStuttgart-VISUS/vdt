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
using Visus.DeploymentToolkit.Vds;


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// Defines the interface of the disk management service, which interfaces
    /// with the virtual disk service to provide enumeration capabilities for
    /// disks. Furthermore, the disk management service allows for partitioning
    /// and formatting disks as well.
    /// </summary>
    public interface IDiskManagement {

        ///// <summary>
        ///// Assigns the specified drive letter to the partition at the specified
        ///// offset.
        ///// </summary>
        ///// <param name="offset"></param>
        ///// <param name="letter"></param>
        //void AssignDriveLetter(ulong offset, char letter);

        ///// <summary>
        ///// Removes the specified drive letter from the partition at the
        ///// specified offset.
        ///// </summary>
        ///// <param name="offset"></param>
        ///// <param name="letter"></param>
        //void DeleteDriveLetter(ulong offset, char letter);

        ///// <summary>
        ///// Gets the drive letter assignet to the partition at the specified
        ///// offset.
        ///// </summary>
        ///// <param name="offset"></param>
        ///// <returns></returns>
        //char? GetDriveLetter(ulong offset);


        /// <summary>
        /// Removes partition information and uninitializes basic or dynamic disks.
        /// </summary>
        /// <param name="disk">The disk to be cleaned.</param>
        /// <param name="flags">The flags customising the behaviour of the
        /// operation.</param>
        /// <param name="cancellationToken">A cancellation token for aborting
        /// the operation.</param>
        /// <returns>A task for waiting on the operation to complete.</returns>
        Task CleanAsync(IDisk disk,
            CleanFlags flags,
            CancellationToken cancellationToken);

        /// <summary>
        /// Converts the partition style of the disk to the specified value.
        /// </summary>
        /// <param name="disk">The disk to be converted.</param>
        /// <param name="style">The new partition style of the disk.</param>
        /// <param name="cancellationToken">A cancellation token for aborting
        /// the operation.</param>
        /// <returns>A task to wait for the operation to complete.</returns>
        Task ConvertAsync(IDisk disk,
            PartitionStyle style,
            CancellationToken cancellationToken);

        /// <summary>
        /// Create a new partition on the disk.
        /// </summary>
        /// <param name="disk"></param>
        /// <param name="partition"></param>
        /// <param name="cancellationToken">A cancellation token for aborting
        /// the operation.</param>
        /// <returns>A task for waiting on the operation to complete.</returns>
        Task CreatePartitionAsync(IDisk disk,
            IPartition partition,
            CancellationToken cancellationToken);

        /// <summary>
        /// Formats the given <paramref name="partition"/> with the specified
        /// <paramref name="fileSystem"/>.
        /// </summary>
        /// <param name="partition"></param>
        /// <param name="fileSystem"></param>
        /// <param name="label"></param>
        /// <param name="allocationUnitSize"></param>
        /// <param name="flags"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task FormatAsync(IPartition partition,
            FileSystem fileSystem,
            string label,
            uint allocationUnitSize,
            FormatFlags flags,
            CancellationToken cancellationToken);

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
