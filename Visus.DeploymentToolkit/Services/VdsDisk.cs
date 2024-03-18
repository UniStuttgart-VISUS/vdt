// <copyright file="VdsDisk.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Contracts.DiskManagement;
using Visus.DeploymentToolkit.Vds;


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// A <see cref="IDisk"/> as enumerated by the <see cref="VdsService"/>.
    /// </summary>
    internal sealed class VdsDisk : IDisk {

        /// <inheritdoc />
        public string FriendlyName => this._properties.FriendlyName;

        /// <inheritdoc />
        public int Index => throw new NotImplementedException();

        /// <inheritdoc />
        public IEnumerable<IPartition> Partitions {
            get {
                if (this._partitions == null) {
                    if (this._disk is IVdsAdvancedDisk disk) {
                        disk.QueryPartitions(out var props, out var cnt);
                        this._partitions = from p in props
                                           select new VdsPartition(p);
                    } else {
                        this._partitions = Enumerable.Empty<IPartition>();
                    }
                }

                return this._partitions;
            }
        }

        #region Internal constructors
        internal VdsDisk(IVdsDisk disk) {
            this._disk = disk ?? throw new ArgumentNullException(nameof(disk));
            this._disk.GetProperties(out this._properties);

            /*
            var disk = unknown as IVdsDisk;
            Assert.IsNotNull(disk, "Have IVdsDisk");

            VDS_DISK_PROP diskProp;
            disk.GetProperties(out diskProp);

            var advDisk = disk as IVdsAdvancedDisk;
            Assert.IsNotNull(advDisk, "Have IVdsAdvancedDisk");

            advDisk.QueryPartitions(out var partitionProps, out var cntPartitions);

            advDisk.GetPartitionProperties(partitionProps[0].Offset, out var partitionProp);
            Assert.AreEqual(partitionProps[0].PartitionNumber, partitionProp.PartitionNumber, "PartitionNumber matches");
            return default;*/
        }
        #endregion

        #region Private fields
        private readonly IVdsDisk _disk;
        private IEnumerable<IPartition>? _partitions;
        private readonly VDS_DISK_PROP _properties;
        #endregion
    }
}
