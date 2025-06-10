// <copyright file="AddPackage.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Services;


namespace Visus.DeploymentToolkit.Tasks {

    /// <summary>
    /// Adds a component to a windows installation using DISM.
    /// </summary>
    public sealed class AddPackage : ImageServicingTaskBase {

        #region Public constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="state">The current state of the task sequence.</param>
        /// <param name="imageServicing">A new image servicing implementation
        /// to perform the work.</param>
        /// <param name="logger">The logger used to report results of the
        /// operation.</param>
        public AddPackage(IState state,
                IImageServicing imageServicing,
                ILogger<ApplyUnattend> logger)
                : base(state,imageServicing, logger) {
            this.Name = Resources.AddPackage;
        }
        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets an optional base path that is prepended to
        /// <see cref="Path"/> if the latter is not found.
        /// </summary>
        /// <remarks>
        /// This can be used to inject a package directory such that callers
        /// only need to specify the package name.
        /// </remarks>
        public string? BasePath { get; set; }

        /// <summary>
        /// Gets or sets wheter to ignore the internal applicability checks
        /// that are done when a package is added.
        /// </summary>
        public bool IgnoreCheck { get; set; } = false;

        /// <summary>
        /// Gets or sets the relative or absolute path to the .cab or .msu file
        /// being added or a folder containing the expanded files of a single
        /// .cab file.
        /// </summary>
        [Required]
        public string Path { get; set; } = null!;

        /// <summary>
        /// Gets or sets whether to add a package if it has pending online
        /// actions.
        /// </summary>
        public bool PreventPending { get; set; } = true;
        #endregion

        #region Public methods
        /// <inheritdoc />
        public override Task ExecuteAsync(CancellationToken cancellationToken) {
            this.Validate();

            if (!File.Exists(this.Path)
                    && !Directory.Exists(this.Path)
                    && Directory.Exists(this.BasePath)) {
                this.Path = System.IO.Path.Combine(this.BasePath, this.Path);
                this._logger.LogTrace("Combining specified package path with "
                    + "base path {BasePath} to {Path}.", this.BasePath,
                    this.Path);
            }

            if (!Directory.Exists(this.Path) && !File.Exists(this.Path)) {
                var p = this.Path + ".cab";
                if (File.Exists(p)) {
                    this._logger.LogTrace("Adding implicit .cab extension to "
                        + "{Path}.", p);
                    this.Path = p;
                }

                p = this.Path + ".msu";
                if (File.Exists(p)) {
                    this._logger.LogTrace("Adding implicit .msu extension to "
                        + "{Path}.", p);
                    this.Path = p;
                }
            }

            return Task.Run(() => {
                try {
                    this._logger.LogInformation("Adding package {Path} to "
                        + "{Image}.", this.Path,
                        this.EffectiveInstallationPath);
                    this.Open().AddPackage(this.Path,
                        this.IgnoreCheck,
                        this.PreventPending);
                    this._logger.LogInformation("Package {Path} successfully "
                        + "added to {Image}.", this.Path,
                        this.EffectiveInstallationPath);
                } finally {
                    this.Close();
                }
            }, cancellationToken);
        }
        #endregion
    }
}
