// <copyright file="PartitionDefinition.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// Generic implementation of <see cref="IPartition"/>, which can be used to
    /// define the partitioning of a disk.
    /// </summary>
    /// <remarks>
    /// Objects of this class are used in the JSON file defining the installation
    /// task sequence to specify how the disk should be configured.
    /// </remarks>
    public sealed class PartitionDefinition : IPartition {

        /// <inheritdoc />
        public uint Index => 0;

        /// <inheritdoc />
        public bool IsBoot => false;

        /// <inheritdoc />
        public bool IsSystem => false;

        /// <summary>
        /// Gets or sets the name of a GPT partition.
        /// </summary>
        public string? Name { get; set; }

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
    }
}
