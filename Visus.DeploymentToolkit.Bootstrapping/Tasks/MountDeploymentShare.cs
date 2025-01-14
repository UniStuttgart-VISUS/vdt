// <copyright file="MountDeploymentShare.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Extensions;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Services;


namespace Visus.DeploymentToolkit.Tasks {

    /// <summary>
    /// A task for mounting the deployment share.
    /// </summary>
    public sealed class MountDeploymentShare : TaskBase {

        #region Public constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="mount"></param>
        /// <param name="input"></param>
        /// <param name="driveInfo"></param>
        /// <param name="logger"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public MountDeploymentShare(IState state,
                MountNetworkShare mount,
                IConsoleInput input,
                IDriveInfo driveInfo,
                ILogger<MountNetworkShare> logger)
                : base(state, logger) {
            this._driveInfo = driveInfo
                ?? throw new ArgumentNullException(nameof(driveInfo));
            this._input = input
                ?? throw new ArgumentNullException(nameof(input));
            this._mount = mount
                ?? throw new ArgumentNullException(nameof(mount));
            this.Name = Resources.MountNetworkShare;
        }
        #endregion

        #region Public properties
        /// <summary>
        /// Gets the options configuring the deployment share.
        /// </summary>
        public DeploymentShareOptions Options { get; } = new();
        #endregion

        #region Public methods
        /// <inheritdoc />
        [SupportedOSPlatform("windows")]
        public override Task ExecuteAsync(CancellationToken cancellationToken) {
            if (string.IsNullOrWhiteSpace(this.Options.MountPoint)) {
                var drives = this._driveInfo.GetFreeDrives();
                this.Options.MountPoint = drives.Last();
                this._logger.LogInformation("Selected {MountPoint} to mount "
                    + "the deployment share.", this.Options.MountPoint);
            }

            if (this.Options.Interactive) {
                this.Options.Path = this._input.ReadInput(
                    Resources.PromptDeploymentShare,
                    this.Options.Path)!;
                this.Options.MountPoint = this._input.ReadInput(
                    Resources.PromptMountPoint,
                    this.Options.MountPoint);

                this.Options.Domain = this._input.ReadInput(
                    Resources.PromptDomain,
                    this.Options.Domain);
                this.Options.User = this._input.ReadInput(
                    Resources.PromptUser,
                    this.Options.User);
                this.Options.Password = this._input.ReadPassword(
                    Resources.PromptPassword).ToInsecureString();
            }

            this._mount.Credential = this.Options.Credential;
            this._mount.Path = this.Options.Path;
            this._mount.MountPoint = this.Options.MountPoint!;

            return this._mount.ExecuteAsync(cancellationToken);
        }
        #endregion

        #region Private fields
        private readonly IDriveInfo _driveInfo;
        private readonly IConsoleInput _input;
        private readonly MountNetworkShare _mount;
        #endregion
    }
}
