// <copyright file="MountDeploymentShare.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
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
                ISessionSecurity sessionSecurity,
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
            this._sessionSecurity = sessionSecurity
                ?? throw new ArgumentNullException(nameof(sessionSecurity));
            this.Name = Resources.MountNetworkShare;
        }
        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets the path to the share.
        /// </summary>
        [FromState(WellKnownStates.DeploymentShare)]
        [Required]
        public string DeploymentShare { get; set; } = null!;

        /// <summary>
        /// Gets or sets the domain the deployment server is joined to.
        /// </summary>
        [FromState(WellKnownStates.DeploymentShareDomain)]
        public string? Domain { get; set; }

        /// <summary>
        /// Gets or sets whether the task should prompt for the deployment share
        /// and use any data provided only as suggestions.
        /// </summary>
        public bool Interactive { get; set; } = true;

        /// <summary>
        /// Gets or sets the mount point for the share, i.e. the drive letter.
        /// </summary>
        [FromState(WellKnownStates.DeploymentDirectory)]
        public string MountPoint { get; set; } = null!;

        /// <summary>
        /// Gets or sets the password used to connect to the deployment share.
        /// </summary>
        [FromState(WellKnownStates.DeploymentSharePassword)]
        public string? Password { get; set; }

        /// <summary>
        /// If <c>true</c>, the login data for the deployment share will be
        /// stored in the <see cref="IState"/> to reconnect automatically after
        /// a reboot.
        /// </summary>
        /// <remarks>
        /// Note that enabling this will leave traces of the encrypted password
        /// in the log files, which might be undesirable in production
        /// environments.
        /// </remarks>
        public bool PreserveConnection { get; set; }

        /// <summary>
        /// Gets or sets the name of the user connecting to the share.
        /// </summary>
        [FromState(WellKnownStates.DeploymentShareUser)]
        public string? User { get; set; }
        #endregion

        #region Public methods
        /// <inheritdoc />
        [SupportedOSPlatform("windows")]
        public override async Task ExecuteAsync(
                CancellationToken cancellationToken) {
            this.CopyFrom(this._state);

            if (string.IsNullOrWhiteSpace(this.MountPoint)) {
                var drives = this._driveInfo.GetFreeDrives();
                this.MountPoint = drives.Last();
                this._logger.LogInformation("Selected {MountPoint} to mount "
                    + "the deployment share.", this.MountPoint);
            }

            if (this.Interactive) {
                this.DeploymentShare = this._input.ReadInput(
                    Resources.PromptDeploymentShare,
                    this.DeploymentShare)!;
                this.MountPoint = this._input.ReadInput(
                    Resources.PromptMountPoint,
                    this.MountPoint)!;

                this.Domain = this._input.ReadInput(
                    Resources.PromptDomain,
                    this.Domain);
                this.User = this._input.ReadInput(
                    Resources.PromptUser,
                    this.User);
                this.Password = this._input.ReadPassword(
                    Resources.PromptPassword).ToInsecureString();
            }

            this._mount.Credential = new(this.User, this.Password, this.Domain);
            this._mount.Path = this.DeploymentShare;
            this._mount.MountPoint = this.MountPoint;

            cancellationToken.ThrowIfCancellationRequested();
            await this._mount.ExecuteAsync(cancellationToken);
            this._logger.LogInformation("Connection to deployment share "
                + "succeeded.");

            if (this.PreserveConnection) {
                this._logger.LogInformation("Preserving the settings in the "
                    + "state for subsequent phases.");
                this._state.DeploymentShare = this.DeploymentShare;
                this._state.DeploymentDirectory = this.MountPoint;

                this._state.DeploymentShareDomain = this.Domain;

                if (!string.IsNullOrEmpty(this.Password)) {
                    this._state.DeploymentSharePassword
                        = this._sessionSecurity.EncryptString(this.Password);
                } else {
                    this._state.DeploymentSharePassword = null;
                }

                this._state.DeploymentShareUser = this.User;
            }
        }
        #endregion

        #region Private fields
        private readonly IDriveInfo _driveInfo;
        private readonly IConsoleInput _input;
        private readonly MountNetworkShare _mount;
        private readonly ISessionSecurity _sessionSecurity;
        #endregion
    }
}
