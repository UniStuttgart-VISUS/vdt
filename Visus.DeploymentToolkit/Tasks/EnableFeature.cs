// <copyright file="EnableFeature.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Services;


namespace Visus.DeploymentToolkit.Tasks {

    /// <summary>
    /// Adds a feature by its name to a Windows installation using DISM.
    /// </summary>
    public sealed class EnableFeature : ImageServicingTaskBase {

        #region Public constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="state">The current state of the task sequence.</param>
        /// <param name="imageServicing">A new image servicing implementation
        /// to perform the work.</param>
        /// <param name="logger">The logger used to report results of the
        /// operation.</param>
        public EnableFeature(IState state,
                IImageServicing imageServicing,
                ILogger<EnableFeature> logger)
                : base(state,imageServicing, logger) {
            this.Name = Resources.EnableFeature;
        }
        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets  whether to enable all dependencies of the feature. If
        /// the specified feature or any one of its dependencies cannot be
        /// enabled, none of them will be changed from their existing state.
        /// </summary>
        public bool EnableAll { get; set; } = true;

        /// <summary>
        /// Gets or sets the name of the feature to enable.
        /// </summary>
        [Required]
        public string Feature { get; set; } = null!;

        /// <summary>
        /// Gets or sets whether Windows Update should be contacted as a source
        /// location for downloading files if none are found in other specified
        /// locations.
        /// </summary>
        public bool LimitAccess { get; set; } = false;
        #endregion

        #region Public methods
        /// <inheritdoc />
        public override Task ExecuteAsync(CancellationToken cancellationToken) {
            this.Validate();

            return Task.Run(() => {
                try {
                    this._logger.LogInformation("Enabling feature {Feature} in "
                        + "{Image}.", this.Feature,
                        this.EffectiveInstallationPath);
                    this.Open().EnableFeature(this.Feature, this.LimitAccess,
                        this.EnableAll);
                } finally {
                    this.Close();
                }
            }, cancellationToken);
        }
        #endregion
    }
}
