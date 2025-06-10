// <copyright file="IDisk.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Collections.Generic;


namespace Visus.DeploymentToolkit.DiskManagement {

    /// <summary>
    /// The interface of a disk that can be used to install the operating system
    /// on.
    /// </summary>
    /// <remarks>
    /// This interface is used by the disk management service to provide
    /// management objects of disks. It is also used by a definition
    /// implementation that allows callers to describe the desired state of a
    /// disk. The latter might not implement all of the interface as it
    /// reasonably cannot know physical properties of the actual disk.
    /// </remarks>
    public interface IDisk {

        #region Public properties
        /// <summary>
        /// Gets the storage bus used by the disk.
        /// </summary>
        StorageBusType BusType { get; }

        /// <summary>
        /// Gets some properties about of the disk.
        /// </summary>
        DiskFlags Flags { get; }

        /// <summary>
        /// Gets the friendly name of the disk.
        /// </summary>
        string FriendlyName { get; }

        /// <summary>
        /// Gets the unique ID of the disk.
        /// </summary>
        Guid ID { get; }

        /// <summary>
        /// A path that can be used to open an operating system handle to the
        /// disk device.
        /// </summary>
        string Path { get; }

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
        /// Gets the size of a sector on the disk in bytes.
        /// </summary>
        uint SectorSize { get; }

        /// <summary>
        /// Gets the overall size of the disk in bytes.
        /// </summary>
        ulong Size { get; }

        /// <summary>
        /// Gets all partitions that can be mapped to a volume.
        /// </summary>
        IEnumerable<(IVolume, IPartition)> VolumePartitions { get; }

        /// <summary>
        /// Gets the volumes on the disk.
        /// </summary>
        IEnumerable<IVolume> Volumes { get; }
        #endregion
    }
}
