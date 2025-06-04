// <copyright file="ApplyImage.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using Microsoft.Wim;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Extensions;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Services;
using Visus.DeploymentToolkit.Validation;


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
            this.Validate();

            return Task.Run(() => {
                if (this.TemporaryDirectory is null) {
                    this._logger.LogTrace("Setting temporary directory for WIM "
                        + "operations to working curren tdirectory "
                        + " \"{WorkingDirectory}\".",
                        this._state.WorkingDirectory);
                    this.TemporaryDirectory = this._state.WorkingDirectory;
                }

                using var wim = this.OpenFile();

                cancellationToken.ThrowIfCancellationRequested();
                using var img = this.LoadImage(wim);

                cancellationToken.ThrowIfCancellationRequested();
                this._logger.LogInformation("Applying \"{Image}\" to"
                    + " \"{Path}\" with options {Options}.",
                    this.Image, this.Path, this.Options);
                WimgApi.ApplyImage(img, this.Path, this.Options);
            }, cancellationToken);
        }
        #endregion
    }
}
