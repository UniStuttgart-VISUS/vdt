// <copyright file="WimTaskBase.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using Microsoft.Wim;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Xml.XPath;
using Visus.DeploymentToolkit.Extensions;
using Visus.DeploymentToolkit.Properties;
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
        /// Gets or sets the path to the WIM file to work with.
        /// </summary>
        [FromState(WellKnownStates.InstallationImage, nameof(Image))]
        [FromEnvironment("DEIMOS_INSTALLATION_IMAGE", "DEIMOS_IMAGE")]
        [Required]
        [FileExists]
        public string Image { get; set; } = null!;

        /// <summary>
        /// Gets or sets the one-based index of the image in the WIM file to
        /// work with.
        /// </summary>
        /// <remarks>
        /// The image (typically the SKU) is either selected by its
        /// <see cref="ImageName"/> or by its <see cref="ImageIndex"/> in the WIM
        /// file. If both are set, the name will be checked first, and if that
        /// fails, the index will be used.
        /// </remarks>
        [FromState(WellKnownStates.InstallationImageIndex, nameof(ImageIndex))]
        [FromEnvironment("DEIMOS_INSTALLATION_IMAGE_INDEX",
            "DEIMOS_IMAGE_INDEX")]
        [Required]
        [DefaultValue(1)]
        public int ImageIndex { get; set; } = 1;

        /// <summary>
        /// Getr or sets the name of the image in the WIM file to work with.
        /// </summary>
        /// <remarks>
        /// The image (typically the SKU) is either selected by its
        /// <see cref="ImageName"/> or by its <see cref="ImageIndex"/> in the WIM
        /// file. If both are set, the name will be checked first, and if that
        /// fails, the index will be used.
        /// </remarks>
        public string? ImageName { get; set; }

        /// <summary>
        /// Gets or sets the temporary directory used by the WIMG API.
        /// </summary>
        /// <remarks>
        /// This directory is set by the <see cref="OpenFile"/> method if it
        /// exists.
        /// </remarks>
        public string? TemporaryDirectory { get; set; }
        #endregion

        #region Protected methods
        /// <summary>
        /// Loads the image identified by <see cref="ImageName"/> or
        /// <see cref="ImageIndex"/> from the WIM file identified by
        /// <paramref name="wim"/>.
        /// </summary>
        /// <param name="wim">The handle to the WIM file, typically obtained
        /// from <see cref="OpenFile"/>.</param>
        /// <returns></returns>
        protected WimHandle LoadImage(WimHandle wim) {
            ArgumentNullException.ThrowIfNull(wim);

            if (wim.IsInvalid || wim.IsClosed) {
                throw new ArgumentException(Errors.InvalidWimHandle);
            }

            if (!string.IsNullOrWhiteSpace(this.ImageName)) {
                this._logger.LogTrace("Searching for an image named "
                    + "{ImageName}.", this.ImageName);
                var info = WimgApi.GetImageInformationAsXDocument(wim);

                var p = info.XPathSelectElement(
                    $"//NAME[contains(text(), \"{this.ImageName}\")]");
                if (p is not null) {
                    var i = p.Parent?.Attribute("INDEX")?.Value;

                    if (i is not null) {
                        this.ImageIndex = int.Parse(i);
                        this._logger.LogTrace("Found image {ImageName} "
                            + "at index {ImageIndex}.", this.ImageName,
                            this.ImageIndex);
                    }
                }
            }

            this._logger.LogTrace("Loading image with index {ImageIndex}.",
                this.ImageIndex);
            return WimgApi.LoadImage(wim, this.ImageIndex);
        }

        /// <summary>
        /// Creates or opens the WIM file specified in <see cref="Image"/>.
        /// </summary>
        /// <returns>A handle for the WIM image.</returns>
        protected WimHandle OpenFile() {
            this._logger.LogTrace("Opening WIM image {Image} "
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
                    + "WIM operations on {Image} to"
                    + " {TemporaryDirectory}.", this.Image,
                    this.TemporaryDirectory);
                WimgApi.SetTemporaryPath(retval, this.TemporaryDirectory);
            }

            return retval;
        }
        #endregion
    }
}
