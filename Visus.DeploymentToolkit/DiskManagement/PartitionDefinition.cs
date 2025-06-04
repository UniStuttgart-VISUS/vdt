// <copyright file="PartitionDefinition.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Collections.Generic;


namespace Visus.DeploymentToolkit.DiskManagement {

    /// <summary>
    /// Generic implementation of <see cref="IPartition"/>, which can be used to
    /// define the partitioning of a disk. As we only create primary partitions,
    /// each partition is a <see cref="IVolume"/> at the same time.
    /// </summary>
    /// <remarks>
    /// Objects of this class are used in the JSON file defining the installation
    /// task sequence to specify how the disk should be configured.
    /// </remarks>
    public sealed class PartitionDefinition : IPartition, IVolume {

        /// <summary>
        /// Gets or sets the file system used to format the parititon.
        /// </summary>
        public FileSystem FileSystem { get; set; } = FileSystem.Unknown;

        /// <inheritdoc />
        public uint Index => 0;

        /// <inheritdoc />
        public bool IsBoot => false;

        /// <inheritdoc />
        public bool IsSystem => false;

        /// <summary>
        /// Gets or sets the optional volume label to be assigned to
        /// the volume created on the partition.
        /// </summary>
        public string? Label { get; set; }

        /// <summary>
        /// Gets or sets the mount points of the volume create on the partition.
        /// </summary>
        public IEnumerable<string> Mounts { get; set; } = [];

        /// <summary>
        /// Gets or sets the name of a GPT partition.
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Gets or sets the offset of the partition from the start of the disk
        /// in bytes.
        /// </summary>
        public ulong Offset { get; set; }

        /// <summary>
        /// Gets or sets the size of the partition in bytes.
        /// </summary>
        public ulong Size { get; set; }

        /// <summary>
        /// Gets the partition style.
        /// </summary>
        /// <remarks>
        /// This property is not used when partitioning as the disk as a whole
        /// needs to have the same parition style.
        /// </remarks>
        public PartitionStyle Style => PartitionStyle.Unknown;

        /// <summary>
        /// Gets or sets the type of the partition to be created.
        /// </summary>
        public PartitionType Type {
            get;
            set;
        } = PartitionType.MicrosoftBasicData;

        /// <summary>
        /// Gets or sets how the partition will be used.
        /// </summary>
        public PartitionUsage Usage { get; set; } = PartitionUsage.Other;
    }
}
