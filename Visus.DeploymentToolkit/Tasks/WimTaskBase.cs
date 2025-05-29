// <copyright file="WimTaskBase.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using Microsoft.Wim;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using Visus.DeploymentToolkit.Extensions;
using Visus.DeploymentToolkit.Services;
using Visus.DeploymentToolkit.Validation;


namespace Visus.DeploymentToolkit.Tasks {

    /// <summary>
    /// The base class using the native WIMG API for working with WIM images.
    /// </summary>
    /// <param name="state">The current state of the task sequence.</param>
    /// <param name="logger">A logger for progress and error messages.</param>
    public abstract class WimTaskBase(IState state, ILogger logger)
            : TaskBase(state, logger) {

        #region Public properties
        /// <summary>
        /// Gets or sets the desired compression of a new WIM file.
        /// </summary>
        public WimCompressionType Compression {
            get;
            set;
        } = WimCompressionType.None;

        /// <summary>
        /// Gets or sets whether an existing WIM file should be opened or a new
        /// one should be created.
        /// </summary>
        public WimCreationDisposition CreationDisposition {
            get;
            set;
        } = WimCreationDisposition.OpenExisting;

        /// <summary>
        /// Gets or sets additional options for creating or opening a WIM file.
        /// </summary>
        public WimCreateFileOptions CreateFileOptions {
            get;
            set;
        } = WimCreateFileOptions.None;

        /// <summary>
        /// Gets or sets the desired access permissions for the WIM file.
        /// </summary>
        public WimFileAccess DesiredAccess { get; set; } = WimFileAccess.Read;

        /// <summary>
        /// Gets or sets the name of the WIM image to work with.
        /// </summary>
        [Required]
        [FileExists]
        public string Image { get; set; } = null!;

        /// <summary>
        /// Gets or sets the temporary directory used by the WIMG API.
        /// </summary>
        /// <remarks>
        /// This directory is set by the <see cref="OpenImage"/> method if it
        /// exists.
        /// </remarks>
        public string? TemporaryDirectory { get; set; }
        #endregion

        #region Protected methods
        /// <summary>
        /// Creates or opens the WIM file specified in <see cref="Image"/>.
        /// </summary>
        /// <returns>A handle for the WIM image.</returns>
        protected WimHandle OpenImage() {
            this._logger.LogTrace("Opening WIM image \"{Image}\" "
                + "with access {DesiredAccess}, "
                + "creation disposition {CreationDisposition}, "
                + "options {CreateFileOptions}"
                + "compression {Compression}.",
                this.Image,
                this.DesiredAccess,
                this.CreationDisposition,
                this.CreateFileOptions,
                this.Compression);

            var retval = WimgApi.CreateFile(this.Image,
                this.DesiredAccess,
                this.CreationDisposition,
                this.CreateFileOptions,
                this.Compression);

            if (Directory.Exists(this.TemporaryDirectory)) {
                this._logger.LogInformation("Setting temporary directory for "
                    + "WIM operations on \"{Image}\" to"
                    + " \"{TemporaryDirectory}\".", this.Image,
                    this.TemporaryDirectory);
                WimgApi.SetTemporaryPath(retval, this.TemporaryDirectory);
            }

            return retval;
        }
        #endregion
    }
}
