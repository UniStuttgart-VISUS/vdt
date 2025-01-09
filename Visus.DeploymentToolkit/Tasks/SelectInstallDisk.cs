// <copyright file="SelectInstallDisk.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 - 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.DiskManagement;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Services;


namespace Visus.DeploymentToolkit.Tasks {

    /// <summary>
    /// This task determines the index of the disk where Windows is to be
    /// installed.
    /// </summary>
    public sealed class SelectInstallDisk : TaskBase {

        #region Public constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="diskManagement">The disk management service that allows
        /// the task to retrieve the disks available on the system.</param>
        /// <param name="state">The state where the selected disk will be stored
        /// in as <see cref="WellKnownStates.InstallationDisk"/>.</param>
        /// <param name="logger">A logger to persist errors.</param>
        /// <exception cref="ArgumentNullException">If
        /// <paramref name="diskManagement"/> is <c>null</c>, or if
        /// <paramref name="state"/> is <c>null</c>.</exception>
        public SelectInstallDisk(IDiskManagement diskManagement,
                IState state,
                ILogger<SelectInstallDisk> logger)
                : base(logger) {
            this._diskManagement = diskManagement
                ?? throw new ArgumentNullException(nameof(diskManagement));
            this._state = state
                ?? throw new ArgumentNullException(nameof(state));
            this.Name = Resources.SelectInstallDisk;
        }
        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets the selection steps that are performed in this order to
        /// find out the installation disk.
        /// </summary>
        public IEnumerable<DiskSelectionStep> Steps {
            get;
            set;
        } = Enumerable.Empty<DiskSelectionStep>();
        #endregion

        #region Public methods
        /// <inheritdoc />
        public override async Task ExecuteAsync(IState state,
                CancellationToken cancellationToken) {
            _ = state ?? throw new ArgumentNullException(nameof(state));

            var disks = await this._diskManagement
                .GetDisksAsync(cancellationToken)
                .ConfigureAwait(false);

            foreach (var s in this.Steps) {
                disks = await s.ApplyAsync(disks,
                    this._diskManagement,
                    this._logger,
                    cancellationToken);
            }

            if (!disks.Any()) {
                this._logger.LogCritical("No suitable installation disks have "
                    + "been found.");
            }

            // Note: First() will throw if there was no disk left.
            this._state.Set(WellKnownStates.InstallationDisk, disks.First());
        }
        #endregion

        #region Private fields
        private readonly IDiskManagement _diskManagement;
        private readonly IState _state;
        #endregion
    }
}
