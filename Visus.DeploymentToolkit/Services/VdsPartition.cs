// <copyright file="VdsPartition.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Visus.DeploymentToolkit.Vds;


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// Implementation of <see cref="IPartition"/> for the VDS.
    /// </summary>
    internal sealed class VdsPartition : IPartition {

        #region Public properties
        /// <inheritdoc />
        public uint Index => this._properties.PartitionNumber;

        /// <inheritdoc />
        public bool IsBoot => (this.Style == PartitionStyle.Mbr)
            ? this._properties.Mbr.BootIndicator
            : false;

        /// <inheritdoc />
        public bool IsSystem => (this._properties.Flags & 1u) == 1u;

        /// <inheritdoc />
        public string? Name => (this.Style == PartitionStyle.Gpt)
            ? this._properties.Gpt.name
            : null;

        /// <inheritdoc />
        public ulong Offset => this._properties.Offset;

        /// <inheritdoc />
        public ulong Size => this._properties.Size;

        /// <inheritdoc />
        public PartitionStyle Style
            => (PartitionStyle) this._properties.PartitionStyle;
        #endregion

        #region Internal constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="properties"></param>
        internal VdsPartition(VDS_PARTITION_PROP properties) {
            this._properties = properties;
        }
        #endregion

        #region Private fields
        private readonly VDS_PARTITION_PROP _properties;
        #endregion
    }
}
