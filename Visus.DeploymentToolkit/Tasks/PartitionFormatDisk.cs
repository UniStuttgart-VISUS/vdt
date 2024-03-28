// <copyright file="PartitionFormatDisk.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Services;


namespace Visus.DeploymentToolkit.Tasks {

    /// <summary>
    /// This task partitions and formats disks.
    /// </summary>
    public sealed class PartitionFormatDisk : TaskBase {

        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="diskManagement">The disk management abstraction that
        /// allows the task to access and modify partitions.</param>
        /// <param name="logger">A logger to report progress and problems.
        /// </param>
        /// <exception cref="ArgumentNullException">If
        /// <paramref name="diskManagement"/> is <c>null</c>, or if
        /// <paramref name="logger"/> is <c>null</c>.</exception>
        public PartitionFormatDisk(IDiskManagement diskManagement,
                ILogger<PartitionFormatDisk> logger)
                : base(logger) {
            this._diskManagement = diskManagement
                ?? throw new ArgumentNullException(nameof(diskManagement));
            this.Disks = Enumerable.Empty<DiskPartitioningDefinition>();
            this.Name = Resources.PartitionFormatDisk;
        }

        #region Public properties
        /// <summary>
        /// Gets or sets the description of the disk partitions to be created.
        /// </summary>
        public IEnumerable<DiskPartitioningDefinition> Disks { get; set; }
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
        #endregion
    }
}
