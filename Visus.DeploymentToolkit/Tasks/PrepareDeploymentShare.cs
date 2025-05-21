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


namespace Visus.DeploymentToolkit.Tasks {

    /// <summary>
    /// This task builds the deployment share file system structure.
    /// </summary>
    public sealed class PrepareDeploymentShare : TaskBase {

        #region Public constructors
        public PrepareDeploymentShare(IState state,
                ILogger<PrepareDeploymentShare> logger)
                : base(state, logger) {
            this.Name = Resources.PrepareDeploymentShare;
        }
        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets the location of the deployment share.
        /// </summary>
        [Required]
        public string Path { get; set; } = null!;
        #endregion

        #region Public methods
        /// <inheritdoc />
        public override Task ExecuteAsync(
                CancellationToken cancellationToken) {
            this.CopyFrom(this._state);

            if (string.IsNullOrWhiteSpace(this.Path)) {
                throw new ArgumentException(Errors.InvalidDeploymentShare);
            }

            if (File.Exists(this.Path)) {
                throw new ArgumentException(string.Format(
                    Errors.DeploymentShareAlreadyExists,
                    this.Path));
            }

            if (Directory.Exists(this.Path)) {
                if (Directory.GetFileSystemEntries(this.Path).Any()) {
                    throw new ArgumentException(string.Format(
                        Errors.DeploymentShareNotEmpty,
                        this.Path));
                }
            } else {
                this._logger.LogInformation("Creating deployment share at "
                    + "\"{Path}\".", this.Path);
                Directory.CreateDirectory(this.Path);
            }

            {
                var p = System.IO.Path.Combine(this.Path, Layout.BinaryPath);
                this._logger.LogInformation("Creating directory \"{Path}\" for "
                    + "binaries.", p);
                Directory.CreateDirectory(p);
            }

            {
                var p = System.IO.Path.Combine(this.Path, Layout.BootFilePath);
                this._logger.LogInformation("Creating directory \"{Path}\" for "
                    + "boot files.", p);
                Directory.CreateDirectory(p);
            }

            {
                var p = System.IO.Path.Combine(this.Path, Layout.BootstrapperPath);
                this._logger.LogInformation("Creating directory \"{Path}\" for "
                    + "bootstrapper binaries.", p);
                Directory.CreateDirectory(p);
            }

            {
                var p = System.IO.Path.Combine(this.Path, Layout.DriverPath);
                this._logger.LogInformation("Creating directory \"{Path}\" for "
                    + "drivers.", p);
                Directory.CreateDirectory(p);
            }

            {
                var p = System.IO.Path.Combine(this.Path, Layout.InstallImagePath);
                this._logger.LogInformation("Creating directory \"{Path}\" for "
                    + "image files.", p);
                Directory.CreateDirectory(p);
            }

            {
                var p = System.IO.Path.Combine(this.Path, Layout.TaskSequencePath);
                this._logger.LogInformation("Creating directory \"{Path}\" for "
                    + "task sequences.", p);
                Directory.CreateDirectory(p);
            }

            return Task.CompletedTask;
        }
        #endregion
    }
}
