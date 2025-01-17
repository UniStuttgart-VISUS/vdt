// <copyright file="MountNetworkShare.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 - 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Services;


namespace Visus.DeploymentToolkit.Tasks {

    /// <summary>
    /// A task for mounting a (generic) network share to a specific location.
    /// </summary>
    /// <remarks>
    /// This task cannot be configured via the <see cref="IState"/>. For the
    /// deployment share, which is configured and announced via the state, use
    /// the <see cref="MountDeploymentShare"/> task.
    /// </remarks>
    public sealed class MountNetworkShare : TaskBase {

        #region Public constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="logger"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public MountNetworkShare(IState state,
                ILogger<MountNetworkShare> logger)
                : base(state, logger) {
            this.Name = Resources.MountNetworkShare;
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
        public string MountPoint { get; set; } = null!;

        /// <summary>
        /// Gets or sets the path to the share.
        /// </summary>
        public string Path { get; set; } = null!;
        #endregion

        #region Public methods
        /// <inheritdoc />
        [SupportedOSPlatform("windows")]
        public override Task ExecuteAsync(CancellationToken cancellationToken) {
            this._logger.LogInformation("Mapping \"{NetworkPath}\" to "
                + "\"{MountPoint}\" as {User}.",
                this.Path,
                this.MountPoint,
                this.Credential?.UserName ?? Resources.CurrentUser);
            MprApi.Connect(this.MountPoint, this.Path, this.Credential,
                MprApi.ConnectionFlags.Temporary);
            return Task.CompletedTask;
        }
        #endregion
    }
}
