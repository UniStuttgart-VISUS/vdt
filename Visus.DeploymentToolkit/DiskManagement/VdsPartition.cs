// <copyright file="VdsPartition.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Linq;
using Visus.DeploymentToolkit.Vds;


namespace Visus.DeploymentToolkit.DiskManagement {

    /// <summary>
    /// Implementation of <see cref="IPartition"/> for the VDS.
    /// </summary>
    internal sealed class VdsPartition : IPartition {

        #region Public properties
        /// <inheritdoc />
        public uint Index => _properties.PartitionNumber;

        /// <inheritdoc />
        public bool IsBoot => Style == PartitionStyle.Mbr
            ? _properties.Mbr.BootIndicator
            : false;

        /// <inheritdoc />
        public bool IsSystem => (_properties.Flags & 1u) == 1u;

        /// <inheritdoc />
        public string? Name => Style == PartitionStyle.Gpt
            ? _properties.Gpt.name
            : null;

        /// <inheritdoc />
        public ulong Offset => _properties.Offset;

        /// <inheritdoc />
        public ulong Size => _properties.Size;

        /// <inheritdoc />
        public PartitionStyle Style
            => (PartitionStyle) _properties.PartitionStyle;

        /// <inheritdoc />
        public PartitionType Type {
            get {
                if (Style == PartitionStyle.Gpt) {
                    var id = _properties.Gpt.PartitionType;
                    var types = PartitionType.FromGpt(id);
                    // If we have NTFS in the list, we also have a lot of legacy
                    // chunk which maps on the same partition type. Therefore,
                    // we force this to NTFS. Ideally, we would check the volume
                    // instead, but we do not have access to this from here.
                    return types.Contains(PartitionType.Ntfs)
                        ? PartitionType.Ntfs
                        : types.First();

                } else {
                    var id = (byte) _properties.Mbr.PartitionType;
                    return PartitionType.FromMbr(id).First();
                }
            }
        }
        #endregion

        #region Internal constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="properties"></param>
        internal VdsPartition(VDS_PARTITION_PROP properties) {
            _properties = properties;
        }
        #endregion

        #region Private fields
        private readonly VDS_PARTITION_PROP _properties;
        #endregion
    }
}
