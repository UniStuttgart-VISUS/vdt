// <copyright file="IDisk.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Visus.DeploymentToolkit.DiskManagement {

    /// <summary>
    /// The interface of a disk that can be used to install the operating system
    /// on.
    /// </summary>
    public interface IDisk {

        #region Public properties
        /// <summary>
        /// Gets the storage bus used by the disk.
        /// </summary>
        StorageBusType BusType { get; }

        /// <summary>
        /// Gets the friendly name of the disk.
        /// </summary>
        string FriendlyName { get; }

        /// <summary>
        /// Gets the unique ID of the disk.
        /// </summary>
        Guid ID { get; }

        /// <summary>
        /// Gets the partitions on the disk.
        /// </summary>
        /// <remarks>
        /// This property can be an empty enumeration if the disk has not been
        /// formatted.
        /// </remarks>
        IEnumerable<IPartition> Partitions { get; }

        /// <summary>
        /// Gets the partition style of the disk.
        /// </summary>
        PartitionStyle PartitionStyle { get; }

        /// <summary>
        /// Gets the overall size of the disk in bytes.
        /// </summary>
        ulong Size { get; }

        /// <summary>
        /// Gets all partitions that can be mapped to a volume.
        /// </summary>
        IEnumerable<Tuple<IVolume, IPartition>> VolumePartitions { get; }

        /// <summary>
        /// Gets the volumes on the disk.
        /// </summary>
        IEnumerable<IVolume> Volumes { get; }
        #endregion

        #region Public methods
        /// <summary>
        /// Converts the partition style of the disk to the specified value.
        /// </summary>
        /// <param name="style">The new partition style of the disk.</param>
        /// <returns>A task to wait for the operation to complete.</returns>
        Task ConvertAsync(PartitionStyle style);
        #endregion
    }
}
