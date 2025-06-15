// <copyright file="SelectImage.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.DeploymentShare;
using Visus.DeploymentToolkit.Extensions;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Services;
using Visus.DeploymentToolkit.Validation;


namespace Visus.DeploymentToolkit.Tasks {

    /// <summary>
    /// A task that allows users to interactively select a WIM image from the
    /// deployment share. The selected image will be stored in the
    /// <see cref="IState"/> as <see cref="WellKnownStates.InstallationImage"/>
    /// such that it will be implicitly accessible by <see cref="ApplyImage"/>.
    /// </summary>
    public sealed class SelectImage : TaskBase {

        #region Public constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="state">The current state of the task sequence.</param>
        /// <param name="directory">The directory service that allows for
        /// enumerating the contents of the deployment share.</param>
        /// <param name="input">The console input service that allows for
        /// prompting for the image.</param>
        /// <param name="logger">A logger for progress and error messages.
        /// </param>
        public SelectImage(IState state,
                IDirectory directory,
                IConsoleInput input,
                ILogger<SelectImage> logger)
            : base(state, logger) {
            this._directory = directory
                ?? throw new ArgumentNullException(nameof(directory));
            this._input = input
                ?? throw new ArgumentNullException(nameof(input));

            this.Name = Resources.SelectImage;
            this.IsCritical = true;
        }
        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets the location of the image directory in the deployment
        /// share. The deployment share itself can be set here if the file
        /// structure is following the default <see cref="Layout"/>.
        /// </summary>
        [Required]
        [DirectoryExists]
        [FromState(WellKnownStates.DeploymentDirectory,
            WellKnownStates.DeploymentShare)]
        public string ImageDirectory { get; set; } = null!;
        #endregion

        #region Public methods
        /// <inheritdoc />
        public override Task ExecuteAsync(CancellationToken cancellationToken) {
            this.CopyFrom(this._state);

            {
                var path = Path.Combine(this.ImageDirectory,
                    Layout.InstallImagePath);
                if (Path.Exists(path)) {
                    this._logger.LogInformation("{DeploymentShare} is the "
                        + "deployment share with its images located at "
                        + "{ImageDirectory}.", this.ImageDirectory, path);
                    this.ImageDirectory = path;
                }
            }

            this.Validate();

            this._logger.LogInformation("Selecting an image from {ImageDirectory}.",
                this.ImageDirectory);
            var wims = this._directory.GetItems(this.ImageDirectory,
                "*.wim",
                GetItemsFlags.FilesOnly | GetItemsFlags.Recursive);

            // TODO: select image names within WIMs.

            var image = this._input.Select(Resources.PromptInstallationImage,
                wims.Select(w => $"{w.FullName}"));

            cancellationToken.ThrowIfCancellationRequested();

            if (image < 0) {
                throw new InvalidOperationException(
                    Errors.NoInstallationImage);
            }

            this._state.InstallationImage = wims.Skip(image).First().FullName;
            this._logger.LogInformation("Selected installation image {Image}" +
                ":{Index}.", this._state.InstallationImage,
                this._state.InstallationImageIndex);

            return Task.CompletedTask;
        }
        #endregion

        #region Private fields
        private readonly IDirectory _directory;
        private readonly IConsoleInput _input;
        #endregion
    }
}
