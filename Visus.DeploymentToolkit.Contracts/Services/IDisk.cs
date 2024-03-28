// <copyright file="IDisk.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Collections.Generic;


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// The interface of a disk that can be used to install the operating system
    /// on.
    /// </summary>
    public interface IDisk {

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
    }
}
