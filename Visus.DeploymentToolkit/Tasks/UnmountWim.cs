// <copyright file="UnmountWim.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Extensions;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Services;


namespace Visus.DeploymentToolkit.Tasks {

    /// <summary>
    /// If mounted, unmount the <see cref="IState.WimMount"/>.
    /// </summary>
    /// <param name="state"></param>
    /// <param name="logger"></param>
    public sealed class UnmountWim: TaskBase {

        #region Public constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="logger"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public UnmountWim(IState state,
                ILogger<UnmountWim> logger)
                : base(state, logger) {
            this.Name = Resources.UnmountWim;
        }
        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets whether the changes should be discarded.
        /// </summary>
        [FromState(nameof(DiscardImageChanges))]
        public bool DiscardImageChanges { get; set; } = false;
        #endregion

        #region Public methods
        /// <inheritdoc />
        public override Task ExecuteAsync(CancellationToken cancellationToken) {
            this.CopyFrom(this._state);
            this.Validate();
            cancellationToken.ThrowIfCancellationRequested();

            var mount = this._state.WimMount;

            if (mount != null) {
                return Task.Factory.StartNew(() => {
                    if (this.DiscardImageChanges) {
                        this._logger.LogInformation("Unmounting "
                            + "{MountPoint} discarding changes.",
                            mount.MountPoint);
                        mount.Dispose();

                    } else {
                        this._logger.LogInformation("Unmounting "
                            + "{MountPoint} committing changes to "
                            + "{Image}.",
                            mount.MountPoint,
                            mount.ImagePath);
                        mount.Commit();
                    }

                    this._state.WimMount = null;
                });

            } else {
                this._logger.LogWarning("No image mounted.");
                return Task.CompletedTask;
            }
        }
        #endregion

    }
}
