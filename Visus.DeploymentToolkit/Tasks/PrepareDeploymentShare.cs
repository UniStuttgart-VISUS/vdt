// <copyright file="PrepareDeploymentShare.cs" company="Visualisierungsinstitut der Universität Stuttgart">
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
    /// This task builds the deployment share file system structure.
    /// </summary>
    public sealed class PrepareDeploymentShare : TaskBase {

        #region Public constructors
        public PrepareDeploymentShare(IState state,
                IDirectory directory,
                ILogger<PrepareDeploymentShare> logger)
                : base(state, logger) {
            this._directory = directory
                ?? throw new ArgumentNullException(nameof(directory));
            this.Name = Resources.PrepareDeploymentShare;
        }
        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets the location of the deployment share.
        /// </summary>
        [Required]
        [EmptyDirectory]
        public string Path { get; set; } = null!;
        #endregion

        #region Public methods
        /// <inheritdoc />
        public override async Task ExecuteAsync(
                CancellationToken cancellationToken) {
            this.CopyFrom(this._state);
            cancellationToken.ThrowIfCancellationRequested();

            if (!Directory.Exists(this.Path)) {
                this._logger.LogInformation("Creating deployment share at "
                    + "\"{Path}\".", this.Path);
                await this._directory.CreateAsync(this.Path)
                    .ConfigureAwait(false);
            }

            // Note: this validation mainly checks that the share did not exist
            // before, but was either created or an empty folder.
            this.Validate();
            cancellationToken.ThrowIfCancellationRequested();

            {
                var p = System.IO.Path.Combine(this.Path, Layout.BinaryPath);
                this._logger.LogInformation("Creating directory \"{Path}\" for "
                    + "binaries.", p);
                await this._directory.CreateAsync(p).ConfigureAwait(false);
                cancellationToken.ThrowIfCancellationRequested();
            }

            {
                var p = System.IO.Path.Combine(this.Path, Layout.BootFilePath);
                this._logger.LogInformation("Creating directory \"{Path}\" for "
                    + "boot files.", p);
                await this._directory.CreateAsync(p).ConfigureAwait(false);
                cancellationToken.ThrowIfCancellationRequested();
            }

            {
                var p = System.IO.Path.Combine(this.Path, Layout.BootstrapperPath);
                this._logger.LogInformation("Creating directory \"{Path}\" for "
                    + "bootstrapper binaries.", p);
                await this._directory.CreateAsync(p).ConfigureAwait(false);
                cancellationToken.ThrowIfCancellationRequested();
            }

            {
                var p = System.IO.Path.Combine(this.Path, Layout.DriverPath);
                this._logger.LogInformation("Creating directory \"{Path}\" for "
                    + "drivers.", p);
                await this._directory.CreateAsync(p).ConfigureAwait(false);
                cancellationToken.ThrowIfCancellationRequested();
            }

            {
                var p = System.IO.Path.Combine(this.Path, Layout.InstallImagePath);
                this._logger.LogInformation("Creating directory \"{Path}\" for "
                    + "image files.", p);
                await this._directory.CreateAsync(p).ConfigureAwait(false);
                cancellationToken.ThrowIfCancellationRequested();
            }

            {
                var p = System.IO.Path.Combine(this.Path, Layout.TaskSequencePath);
                this._logger.LogInformation("Creating directory \"{Path}\" for "
                    + "task sequences.", p);
                await this._directory.CreateAsync(p).ConfigureAwait(false);
                cancellationToken.ThrowIfCancellationRequested();
            }

            {
                var p = System.IO.Path.Combine(this.Path, Layout.TemplatesPath);
                this._logger.LogInformation("Creating directory \"{Path}\" for "
                    + "template files.", p);
                await this._directory.CreateAsync(p).ConfigureAwait(false);
                cancellationToken.ThrowIfCancellationRequested();
            }
        }
        #endregion

        #region Private fields
        private readonly IDirectory _directory;
        #endregion
    }
}
