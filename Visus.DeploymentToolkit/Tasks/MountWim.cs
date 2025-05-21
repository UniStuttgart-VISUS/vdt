// <copyright file="InjectDrivers.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using Serilog.Core;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Extensions;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Services;


namespace Visus.DeploymentToolkit.Tasks {

    /// <summary>
    /// A task that mounts a WIM image to a specified directory.
    /// </summary>
    public sealed class MountWim : TaskBase {

        #region Public constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="dism"></param>
        /// <param name="logger"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public MountWim(IState state,
                IDismScope dism,
                ILogger<MountWim> logger)
                : base(state, logger) {
            this._dism = dism
                ?? throw new ArgumentNullException(nameof(dism));
            this.Name = Resources.MountWim;
        }
        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets the path to the image to be mounted.
        /// </summary>
        [FromState(nameof(ImagePath))]
        public string ImagePath { get; set; } = null!;

        /// <summary>
        /// Gets or sets the index of the image to be mounted.
        /// </summary>
        [FromState(nameof(ImageIndex))]
        public int ImageIndex { get; set; } = 1;

        /// <summary>
        /// Gets or sets the path where the image is mounted.
        /// </summary>
        [FromState(nameof(MountPoint))]
        public string MountPoint { get; set; } = null!;
        #endregion

        #region Public methods
        /// <inheritdoc />
        public override Task ExecuteAsync(CancellationToken cancellationToken) {
            this.CopyFrom(this._state);

            if (string.IsNullOrEmpty(this.ImagePath)
                    && !File.Exists(this.ImagePath)) {
                throw new ArgumentException(string.Format(
                    Errors.InvalidImage, this.ImagePath));
            }

            if (string.IsNullOrEmpty(this.MountPoint)
                    && !File.Exists(this.MountPoint)) {
                throw new ArgumentException(string.Format(
                    Errors.InvalidMountPoint, this.MountPoint));
            }

            if (this._state.WimMount != null) {
                throw new InvalidOperationException(string.Format(
                    Errors.ImageAlreadyMounted,
                    this._state.WimMount.MountPoint));
            }

            this._logger.LogInformation("Mounting \"{Image}\" at "
                + "\"{MountPoint}\".", this.ImagePath, this.MountPoint);
            return Task.Factory.StartNew(() => {
                this._state.WimMount = new DismMount(this._dism,
                    this.ImagePath,
                    this.ImageIndex,
                    this.MountPoint);
                this._logger.LogInformation("\"{Image}\" mounted at "
                + "\"{MountPoint}\".", this.ImagePath, this.MountPoint);
            });
        }
        #endregion

        #region Private fields
        private readonly IDismScope _dism;
        #endregion
    }
}
