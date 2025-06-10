// <copyright file="UnmountWim.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Dism;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
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

        /// <summary>
        /// Gets or sets the number of retries when committing the image.
        /// </summary>
        /// <remarks>
        /// <para>The operation will only be retried if the error are open
        /// handles on the image. Other errors are not retriable and will cause
        /// the operation to fail immediately.</para>
        /// <para>Retries should normally not be necessary as long as all tasks
        /// await asynchronous operations properly. This parameter might,
        /// however, be useful for debugging if you tinker around with the image
        /// while the task sequence is running.</para>
        /// </remarks>
        public uint RetryCount { get; set; } = 0;

        /// <summary>
        /// Gets or sets the time to wait between retries.
        /// </summary>
        /// <remarks>
        /// This property has only an effect if <see cref="RetryCount"/> is
        /// greater than zero. The time will be doubled after each failed
        /// retry.
        /// </remarks>
        public TimeSpan RetryWait { get; set; } = TimeSpan.FromSeconds(5);
        #endregion

        #region Public methods
        /// <inheritdoc />
        public override async Task ExecuteAsync(
                CancellationToken cancellationToken) {
            this.CopyFrom(this._state);
            this.Validate();
            cancellationToken.ThrowIfCancellationRequested();

            var mount = this._state.WimMount;

            if (mount != null) {
                Func<IDismMount, Task> action = this.DiscardImageChanges
                    ? this.DiscardAsync
                    : this.CommitAsync;
                var retry = true;

                while (retry) {
                    try {
                        await action(mount);

                        this._logger.LogTrace("Unmounted {MountPoint} "
                            + "successfully. Cleaning up the state now.",
                            mount.MountPoint);
                        this._state.WimMount = null;
                        retry = false;
                    } catch (DismException ex) when (ex.HResult
                            == ErrorOpenHandles) {
                        if (!(retry = this.RetryCount > 0)) {
                            throw;
                        }

                        this._logger.LogWarning("Failed to unmount "
                            + "{MountPoint} due to open handles. Retrying in "
                            + "{Timeout}", mount.MountPoint, this.RetryWait);
                        await Task.Delay(this.RetryWait, cancellationToken);

                        --this.RetryCount;
                        this.RetryWait *= 2;
                    }
                }

            } else {
                this._logger.LogWarning("No image mounted.");
            }
        }
        #endregion

        #region Private constants
        private const int ErrorOpenHandles = unchecked(
            (int) DismApi.DISMAPI_E_OPEN_HANDLES_UNABLE_TO_UNMOUNT_IMAGE_PATH);
        #endregion

        #region Private methods
        private Task CommitAsync(IDismMount mount) {
            Debug.Assert(mount is not null);
            return Task.Run(() => {
                this._logger.LogInformation("Unmounting {MountPoint} "
                    + "committing changes to {Image}.", mount.MountPoint,
                    mount.ImagePath);
                mount.Commit();
            });
        }

        private Task DiscardAsync(IDismMount mount) {
            Debug.Assert(mount is not null);
            return Task.Run(() => {
                this._logger.LogInformation("Unmounting {MountPoint} " +
                    "discarding changes.", mount.MountPoint);
                mount.Dispose();
            });
        }
        #endregion
    }
}
