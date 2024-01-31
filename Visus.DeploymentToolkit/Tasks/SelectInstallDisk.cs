// <copyright file="SelectInstallDisk.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;


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
        }

        public override Task<int> ExecuteAsync() {
            throw new NotImplementedException();
        }

        IDiskManagement _diskManagement;
    }
}
