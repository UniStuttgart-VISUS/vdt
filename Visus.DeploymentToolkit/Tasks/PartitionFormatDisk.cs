// <copyright file="SelectInstallDisk.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Contracts;
using Visus.DeploymentToolkit.Properties;


namespace Visus.DeploymentToolkit.Tasks {

    /// <summary>
    /// This task partitions and formats disks.
    /// </summary>
    public sealed class PartitionFormatDisk : TaskBase<int> {

        public PartitionFormatDisk(IDiskManagement diskManagement,
                ILogger<PartitionFormatDisk> logger)
                : base(logger) {
            this._diskManagement = diskManagement
                ?? throw new ArgumentNullException(nameof(diskManagement));
            this.Name = Resources.PartitionFormatDisk;
        }

        public override Task<int> ExecuteAsync() {
            throw new NotImplementedException();
        }

        #region Private fields
        private readonly IDiskManagement _diskManagement;
        #endregion
    }
}
