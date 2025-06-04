// <copyright file="InjectDrivers.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using Serilog.Core;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Extensions;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Services;
using Visus.DeploymentToolkit.Validation;


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
        /// Gets or sets the name of the image to be mounted.
        /// </summary>
        /// <remarks>
        /// If a name and a valid <see cref="ImageIndex"/> are specified,
        /// the name takes precedence.
        /// </remarks>
        [FromState(nameof(ImageName))]
        public string? ImageName { get; set; }

        /// <summary>
        /// Gets or sets the path to the image to be mounted.
        /// </summary>
        [FromState(nameof(ImagePath))]
        [FileExists]
        public string ImagePath { get; set; } = null!;

        /// <summary>
        /// Gets or sets the index of the image to be mounted.
        /// </summary>
        /// <remarks>
        /// The index is one-based. If an <see cref="ImageName"/> is specified,
        /// this parameter is ignored.
        /// </remarks>
        [FromState(nameof(ImageIndex))]
        public int ImageIndex { get; set; } = 1;

        /// <summary>
        /// Gets or sets the path where the image is mounted.
        /// </summary>
        [FromState(nameof(MountPoint))]
        [Required]
        [NonExistingFile]
        public string MountPoint { get; set; } = null!;
        #endregion

        #region Public methods
        /// <inheritdoc />
        public override Task ExecuteAsync(CancellationToken cancellationToken) {
            this.CopyFrom(this._state);
            this.Validate();
            cancellationToken.ThrowIfCancellationRequested();

            this._logger.LogInformation("Mounting {Image} at "
                + "{MountPoint}.", this.ImagePath, this.MountPoint);
            return Task.Factory.StartNew(() => {
                if (string.IsNullOrEmpty(this.ImageName)) {
                    this._logger.LogInformation("Mounting image index "
                        + "{ImageIndex}.", this.ImageIndex);
                    this._state.WimMount = new DismMount(this._dism,
                        this.ImagePath,
                        this.ImageIndex,
                        this.MountPoint);
                } else {
                    this._logger.LogInformation("Mounting image name "
                        + "{ImageName}.", this.ImageName);
                    this._state.WimMount = new DismMount(this._dism,
                        this.ImagePath,
                        this.ImageName,
                        this.MountPoint);
                }

                this._logger.LogInformation("The {Image} has been mounted "
                + "at {MountPoint}. Should the need arise to unmount this "
                + "image manually, you can do so by invoking "
                + "dism /Unmount-Image /MountDir:{MountPoint} /Discard",
                this.ImagePath, this.MountPoint, this.MountPoint);
            });
        }
        #endregion

        #region Protected methods
        /// <inheritdoc />
        protected override void Validate() {
            base.Validate();

            if (this._state.WimMount != null) {
                throw new ValidationException(string.Format(
                    Errors.ImageAlreadyMounted,
                    this._state.WimMount.MountPoint));
            }
        }
        #endregion

        #region Private fields
        private readonly IDismScope _dism;
        #endregion
    }
}
