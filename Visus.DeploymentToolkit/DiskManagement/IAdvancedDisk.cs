// <copyright file="VdsVolume.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 - 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Threading;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Vds;


namespace Visus.DeploymentToolkit.DiskManagement {

    /// <summary>
    /// An internal advanced disk interface that allows for modifying a disk
    /// rather than merely reading its properties.
    /// </summary>
    internal interface IAdvancedDisk : IDisk {

        /// <summary>
        /// Assigns the specified drive letter to the partition at the specified
        /// offset.
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="letter"></param>
        void AssignDriveLetter(ulong offset, char letter);

        /// <summary>
        /// Removes partition information and uninitializes basic or dynamic disks.
        /// </summary>
        /// <param name="flags">The flags customising the behaviour of the
        /// operation.</param>
        /// <param name="cancellationToken">A cancellation token for aborting
        /// the operation.</param>
        /// <returns>A task for waiting on the operation to complete.</returns>
        Task<VDS_ASYNC_OUTPUT> CleanAsync(CleanFlags flags,
            CancellationToken cancellationToken);

        /// <summary>
        /// Create an MBR partition on the disk.
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="size"></param>
        /// <param name="parameters"></param>
        /// <param name="cancellationToken">A cancellation token for aborting
        /// the operation.</param>
        /// <returns>A task for waiting on the operation to complete.</returns>
        Task<VDS_ASYNC_OUTPUT> CreatePartitionAsync(ulong offset,
            ulong size,
            MbrPartitionParameters parameters,
            CancellationToken cancellationToken);

        /// <summary>
        /// Create a GPT partition on the disk.
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="size"></param>
        /// <param name="parameters"></param>
        /// <param name="cancellationToken">A cancellation token for aborting
        /// the operation.</param>
        /// <returns>A task for waiting on the operation to complete.</returns>
        Task<VDS_ASYNC_OUTPUT> CreatePartitionAsync(ulong offset,
            ulong size,
            VDS_PARTITION_INFO_GPT parameters,
            CancellationToken cancellationToken);

        /// <summary>
        /// Removes the specified drive letter from the partition at the
        /// specified offset.
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="letter"></param>
        void DeleteDriveLetter(ulong offset, char letter);

        /// <summary>
        /// Formats the partition at the specified offset.
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="type"></param>
        /// <param name="label"></param>
        /// <param name="unitAllocationSize"></param>
        /// <param name="flags"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<VDS_ASYNC_OUTPUT> FormatPartitionAsync(ulong offset,
            VDS_FILE_SYSTEM_TYPE type,
            string label,
            uint unitAllocationSize,
            FormatFlags flags,
            CancellationToken cancellationToken);

        /// <summary>
        /// Gets the drive letter assignet to the partition at the specified
        /// offset.
        /// </summary>
        /// <param name="offset"></param>
        /// <returns></returns>
        char? GetDriveLetter(ulong offset);

    }
}
