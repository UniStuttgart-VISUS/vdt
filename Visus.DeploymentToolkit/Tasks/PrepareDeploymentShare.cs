// <copyright file="PrepareDeploymentShare.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.DeploymentShare;
using Visus.DeploymentToolkit.Extensions;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Services;
using Visus.DeploymentToolkit.Validation;
using SysPath = System.IO.Path;


namespace Visus.DeploymentToolkit.Tasks {

    /// <summary>
    /// This task builds the deployment share file system structure.
    /// </summary>
    public sealed class PrepareDeploymentShare : TaskBase {

        #region Public constructors
        public PrepareDeploymentShare(IState state,
                IDirectory directory,
                ICopy copy,
                ILogger<PrepareDeploymentShare> logger)
                : base(state, logger) {
            this._copy = copy
                ?? throw new ArgumentNullException(nameof(copy));
            this._directory = directory
                ?? throw new ArgumentNullException(nameof(directory));
            this.Name = Resources.PrepareDeploymentShare;
        }
        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets the location of the agent binaries to be deployed to
        /// the share.
        /// </summary>
        /// <remarks>
        /// The path to the directory is expected.
        /// </remarks>
        public string? Agent { get; set; }

        /// <summary>
        /// Gets or sets the location of the bootstrapper binaries to be
        /// deployed to the share.
        /// </summary>
        /// <remarks>
        /// The path to the directory is expected.
        /// </remarks>
        public string? Bootstrapper { get; set; }

        /// <summary>
        /// Gets or sets the location of the deployment share.
        /// </summary>
        [Required]
        [EmptyDirectory]
        public string Path { get; set; } = null!;

        /// <summary>
        /// Gets or sets the location of the template files.
        /// </summary>
        public string? Templates { get; set; }

        #endregion

        #region Public methods
        /// <inheritdoc />
        public override async Task ExecuteAsync(
                CancellationToken cancellationToken) {
            this.CopyFrom(this._state);
            cancellationToken.ThrowIfCancellationRequested();

            if (!Directory.Exists(this.Path)) {
                this._logger.LogInformation("Creating deployment share at "
                    + "{Path}.", this.Path);
                await this._directory.CreateAsync(this.Path)
                    .ConfigureAwait(false);
            }

            // Note: this validation mainly checks that the share did not exist
            // before, but was either created or an empty folder.
            this.Validate();
            cancellationToken.ThrowIfCancellationRequested();

            {
                var p = SysPath.Combine(this.Path, Layout.BinaryPath);
                this._logger.LogInformation("Creating directory {Path} for "
                    + "binaries.", p);
                await this._directory.CreateAsync(p).ConfigureAwait(false);
                cancellationToken.ThrowIfCancellationRequested();
            }

            {
                var p = SysPath.Combine(this.Path, Layout.BootFilePath);
                this._logger.LogInformation("Creating directory {Path} for "
                    + "boot files.", p);
                await this._directory.CreateAsync(p).ConfigureAwait(false);
                cancellationToken.ThrowIfCancellationRequested();
            }

            {
                var p = SysPath.Combine(this.Path, Layout.BootstrapperPath);
                this._logger.LogInformation("Creating directory {Path} for "
                    + "bootstrapper binaries.", p);
                await this._directory.CreateAsync(p).ConfigureAwait(false);
                cancellationToken.ThrowIfCancellationRequested();
            }

            {
                var p = SysPath.Combine(this.Path, Layout.DriverPath);
                this._logger.LogInformation("Creating directory {Path} for "
                    + "drivers.", p);
                await this._directory.CreateAsync(p).ConfigureAwait(false);
                cancellationToken.ThrowIfCancellationRequested();
            }

            {
                var p = SysPath.Combine(this.Path, Layout.InstallImagePath);
                this._logger.LogInformation("Creating directory {Path} for "
                    + "image files.", p);
                await this._directory.CreateAsync(p).ConfigureAwait(false);
                cancellationToken.ThrowIfCancellationRequested();
            }

            {
                var p = SysPath.Combine(this.Path, Layout.TaskSequencePath);
                this._logger.LogInformation("Creating directory {Path} for "
                    + "task sequences.", p);
                await this._directory.CreateAsync(p).ConfigureAwait(false);
                cancellationToken.ThrowIfCancellationRequested();
            }

            {
                var p = SysPath.Combine(this.Path, Layout.TemplatesPath);
                this._logger.LogInformation("Creating directory {Path} for "
                    + "template files.", p);
                await this._directory.CreateAsync(p).ConfigureAwait(false);
                cancellationToken.ThrowIfCancellationRequested();
            }

            if (Directory.Exists(this.Agent)) {
                var dst = SysPath.Combine(this.Path, Layout.BinaryPath);
                this._logger.LogInformation("Copying agent binaries from "
                    + "{Source} to {Destination} on the deployment share.",
                    this.Agent, dst);
                await this._copy.CopyAsync(this.Agent,
                    dst,
                    CopyFlags.Recursive | CopyFlags.Overwrite);
                cancellationToken.ThrowIfCancellationRequested();
            }

            if (Directory.Exists(this.Bootstrapper)) {
                var dst = SysPath.Combine(this.Path, Layout.BootstrapperPath);
                this._logger.LogInformation("Copying bootstrapper binaries "
                    + "from {Source} to {Destination} on the deployment share.",
                    this.Agent, dst);
                await this._copy.CopyAsync(this.Bootstrapper,
                    dst,
                    CopyFlags.Recursive | CopyFlags.Overwrite);
                cancellationToken.ThrowIfCancellationRequested();
            }

            if (Directory.Exists(this.Templates)) {
                var dst = SysPath.Combine(this.Path, Layout.TemplatesPath);
                this._logger.LogInformation("Copying template files from "
                    + "{Source} to {Destination} on the deployment share.",
                    this.Agent, dst);
                await this._copy.CopyAsync(this.Templates,
                    dst,
                    CopyFlags.Recursive | CopyFlags.Overwrite);
                cancellationToken.ThrowIfCancellationRequested();
            }
        }
        #endregion

        #region Private fields
        private readonly ICopy _copy;
        private readonly IDirectory _directory;
        #endregion
    }
}
