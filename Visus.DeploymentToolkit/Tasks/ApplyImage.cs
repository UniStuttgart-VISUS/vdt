// <copyright file="ApplyImage.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using Microsoft.Wim;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.DeploymentShare;
using Visus.DeploymentToolkit.Extensions;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Services;
using Visus.DeploymentToolkit.Validation;
using SPath = System.IO.Path;


namespace Visus.DeploymentToolkit.Tasks {

    /// <summary>
    /// A task that applies a WIM image to the specified location.
    /// </summary>
    public sealed class ApplyImage : WimTaskBase {

        #region Public constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="state">The current state of the task sequence.</param>
        /// <param name="logger">A logger for progress and error messages.
        /// </param>
        public ApplyImage(IState state, ILogger<ApplyImage> logger)
            : base(state, logger) {
            this.Name = Resources.ApplyImage;
            this.IsCritical = true;
        }
        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets the path where the WIM image should be applied.
        /// </summary>
        [Required]
        [DirectoryExists]
        [FromState(WellKnownStates.InstallationDirectory)]
        [FromEnvironment("DEIMOS_INSTALLATION_DIRECTORY")]
        public string Path { get; set; } = null!;

        /// <summary>
        /// Gets or sets additional options for applying the WIM image.
        /// </summary>
        public WimApplyImageOptions Options {
            get;
            set;
        } = WimApplyImageOptions.None;
        #endregion

        #region Public methods
        /// <inheritdoc />
        public override Task ExecuteAsync(CancellationToken cancellationToken) {
            this.CopyFrom(this._state);
            this.CopyFromEnvironment();

            if (!File.Exists(this.Image)) {
                this.SearchImage(this.Image);
            }

            this.Validate();

            return Task.Run(() => {
                if (this.TemporaryDirectory is null) {
                    this._logger.LogTrace("Setting temporary directory for WIM "
                        + "operations to working curren tdirectory "
                        + " {WorkingDirectory}.",
                        this._state.WorkingDirectory);
                    this.TemporaryDirectory = this._state.WorkingDirectory;
                }

                using var wim = this.OpenFile();

                cancellationToken.ThrowIfCancellationRequested();
                using var img = this.LoadImage(wim);

                cancellationToken.ThrowIfCancellationRequested();
                this._logger.LogInformation("Applying {Image} to"
                    + " {Path} with options {Options}.",
                    this.Image, this.Path, this.Options);
                WimgApi.ApplyImage(img, this.Path, this.Options);
            }, cancellationToken);
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Searches the specified <paramref name="image"/> in the deployment
        /// share specified in the state.
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        private bool SearchImage(string image) {
            this._logger.LogInformation("Image was not found at {Image}. "
                + "Searching for it in the deployment share.", this.Image);

            if (this.SearchImage(image,
                    this._state.DeploymentDirectory,
                    Layout.InstallImagePath)) {
                return true;
            }

            if (this.SearchImage(image,
                    this._state.DeploymentShare,
                    Layout.InstallImagePath)) {
                return true;
            }

            this._logger.LogWarning("The image {Image} was not found in the "
                + "deployment share.", this.Image);
            return false;
        }

        /// <summary>
        /// Searches the specified <paramref name="image"/> in
        /// <paramref name="directory"/>, if the directory exists, and
        /// potentially in the specified <paramref name="subdirectories"/>
        /// of <paramref name="directory"/>.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="directory"></param>
        /// <param name="subdirectories"></param>
        /// <returns></returns>
        private bool SearchImage(string image,
                string? directory,
                params string[] subdirectories) {
            Debug.Assert(image is not null);
            if (!Directory.Exists(directory)) {
                this._logger.LogTrace("Directory {Directory} does not exist, "
                    + "ignoring it.", directory);
                return false;
            }

            this._logger.LogTrace("Searching for image {Image} in {Directory}.",
                image, directory);
            var path = SPath.Combine(directory, image);

            if (File.Exists(path)) {
                this._logger.LogTrace("Image was found at {Image}.", path);
                this.Image = path;
                return true;
            }

            if (Directory.Exists(path)) {
                var wim = SPath.Combine(path, "sources", "install.wim");
                this._logger.LogTrace("Image {Image} is a directory, so try "
                    + "to handle it as a Windows source directory with the "
                    + "installation image at {Path}.", path, wim);
                return this.SearchImage(directory, wim);
            }

            if (!SPath.HasExtension(image)) {
                this._logger.LogTrace("Image {Image} does not have an "
                    + "extension and is not a directory, so try the default "
                    + "WIM extension.", image);
                return this.SearchImage(directory, image + ".wim");
            }

            if (subdirectories is not null) {
                this._logger.LogTrace("Searching in specified subdirectories"
                    + " of {Directory}.", directory);
                foreach (var s in subdirectories) {
                    var d = SPath.Combine(directory, s);
                    if (this.SearchImage(image, d)) {
                        return true;
                    }
                }
            }

            this._logger.LogTrace("Image {Image} was not found in {Directory}.",
                image, directory);
            return false;
        }
        #endregion
    }
}
