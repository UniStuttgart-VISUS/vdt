// <copyright file="InjectDrivers.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
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
    /// Injects drivers into a mounted Windows image or online servicing
    /// session.
    /// </summary>
    public sealed class InjectDrivers : ImageServicingTaskBase {

        #region Public constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="imageServicing"></param>
        /// <param name="logger"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public InjectDrivers(IState state,
                IImageServicing imageServicing,
                ILogger<InjectDrivers> logger)
                : base(state, imageServicing, logger) {
            this.Name = Resources.InjectDrivers;
            this.IsCritical = true;
        }
        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets whether the drivers in subdirectories of
        /// <see cref="Path"/> are all injected.
        /// </summary>
        public bool IsRecursive { get; set; }

        /// <summary>
        /// Gets or sets whether unsigned drivers are allowed to be injected.
        /// </summary>
        public bool IsUnsigned { get; set; }

        /// <summary>
        /// Gets or sets the location where the drivers are located.
        /// </summary>
        [Required]
        public string[] Path { get; set; } = Array.Empty<string>();
        #endregion

        #region Public methods
        /// <inheritdoc />
        public override Task ExecuteAsync(CancellationToken cancellationToken) {
            this.Validate();

            return Task.Run(() => {
                try {
                    foreach (var p in this.Path) {
                        if (!Directory.Exists(p)) {
                            this._logger.LogWarning("The driver path {Path} "
                                + "does not exist or does not designate a "
                                + "directory. It will therefore be skipped.",
                                p);
                            continue;
                        }

                        this._logger.LogInformation("Injecting drivers from "
                            + "{Path} " + "to {Image}.",
                            p, this.EffectiveInstallationPath);
                        this.Open().InjectDrivers(p, this.IsRecursive,
                            this.IsUnsigned);
                        this._logger.LogInformation("Drivers from {Path} "
                            + "successfully added to image {Image}.", p,
                            this.EffectiveInstallationPath);
                    }
                } finally {
                    this.Close();
                }
            }, cancellationToken);
        }
        #endregion
    }
}
