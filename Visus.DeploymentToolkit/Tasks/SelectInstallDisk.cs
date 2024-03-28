// <copyright file="SelectInstallDisk.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Services;


namespace Visus.DeploymentToolkit.Tasks {

    /// <summary>
    /// This task determines the index of the disk where Windows is to be
    /// installed.
    /// </summary>
    public sealed class SelectInstallDisk : TaskBase {

        #region Public constructors
        public SelectInstallDisk(IDiskManagement diskManagement,
                ILogger<SelectInstallDisk> logger)
                : base(logger) {
            this._diskManagement = diskManagement
                ?? throw new ArgumentNullException(nameof(diskManagement));
            this._logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
            this.Name = Resources.SelectInstallDisk;
        }
        #endregion

        #region Public methods
        /// <inheritdoc />
        public override async Task ExecuteAsync(IState state,
                CancellationToken cancellationToken) {
            _ = state ?? throw new ArgumentNullException(nameof(state));

            var disks = await this._diskManagement
                .GetDisksAsync(cancellationToken)
                .ConfigureAwait(false);


            throw new NotImplementedException();
        }
        #endregion

        #region Private fields
        private readonly IDiskManagement _diskManagement;
        private readonly ILogger _logger;
        #endregion
    }
}
