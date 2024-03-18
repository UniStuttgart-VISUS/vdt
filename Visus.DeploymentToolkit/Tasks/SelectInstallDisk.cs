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
    /// This task determines the index of the disk where Windows is to be
    /// installed.
    /// </summary>
    public sealed class SelectInstallDisk : TaskBase<int> {

        public SelectInstallDisk(IDiskManagement diskManagement,
                ILogger<SelectInstallDisk> logger)
                : base(logger) {
            this._diskManagement = diskManagement
                ?? throw new ArgumentNullException(nameof(diskManagement));
            this.Name = Resources.SelectInstallDisk;
        }

        #region Public methods
        /// <inheritdoc />
        public override Task<int> ExecuteAsync() {
            throw new NotImplementedException();
        }
        #endregion

        #region Private fields
        private readonly IDiskManagement _diskManagement;
        #endregion
    }
}
