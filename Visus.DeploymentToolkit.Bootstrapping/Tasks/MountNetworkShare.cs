// <copyright file="MountNetworkShare.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Services;


namespace Visus.DeploymentToolkit.Tasks {

    /// <summary>
    /// A task for mounting a network share to a specific location.
    /// </summary>
    internal sealed class MountNetworkShare : TaskBase {

        #region Public constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="driveInfo"></param>
        /// <param name="logger"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public MountNetworkShare(IDriveInfo driveInfo,
                ILogger<MountNetworkShare> logger)
                : base(logger) {
            this._driveInfo = driveInfo
                ?? throw new ArgumentNullException(nameof(driveInfo));
        }
        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets the credential used to connect to the share.
        /// </summary>
        public NetworkCredential? Credential { get; set; }

        /// <summary>
        /// Gets or sets the mount point for the share, i.e. the drive letter.
        /// </summary>
        public string? MountPoint { get; set; }

        /// <summary>
        /// Gets or sets the path to the share.
        /// </summary>
        public string Path { get; set; }
        #endregion

        #region Public methods
        /// <inheritdoc />
        [SupportedOSPlatform("windows")]
        public override Task ExecuteAsync(IState state) {
            _ = state ?? throw new ArgumentNullException(nameof(state));

            if (this.MountPoint == null) {
                this.MountPoint = this._driveInfo.GetFreeDrives().First();
            }

            this._logger.LogTrace(Resources.MountingShare,
                this.Path,
                this.MountPoint,
                this.Credential?.UserName ?? Resources.CurrentUser);
            MprApi.Connect(this.MountPoint, this.Path, this.Credential,
                MprApi.ConnectionFlags.Temporary);

            return Task.CompletedTask;
        }
        #endregion

        #region Private fields
        private readonly IDriveInfo _driveInfo;
        #endregion
    }
}
