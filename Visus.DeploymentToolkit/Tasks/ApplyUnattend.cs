// <copyright file="ApplyUnattend.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Dism;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Services;
using Visus.DeploymentToolkit.Workflow;


namespace Visus.DeploymentToolkit.Tasks {

    /// <summary>
    /// Applies an unattend file to a mounted Windows installation.
    /// </summary>
    [SupportsPhase(Phase.Installation)]
    internal sealed class ApplyUnattend : TaskBase {

        #region Public constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="dism"></param>
        /// <param name="logger">The logger used to report results of the
        /// operation.</param>
        public ApplyUnattend(IDismScope dism,
            ILogger<ApplyUnattend> logger) : base(logger) { }
        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets the path to the Windows installation being modified.
        /// </summary>
        [Required]
        public string InstallationPath { get; set; }

        /// <summary>
        /// Gets or sets the path to the unattend file to apply.
        /// </summary>
        [Required]
        public string UnattendFile { get; set; }
        #endregion

        #region Public methods
        /// <inheritdoc />
        public override Task ExecuteAsync(IState state,
                CancellationToken cancellationToken) {
            this._logger.LogTrace(Resources.DismOpenOffline,
                this.InstallationPath);
            var session = DismApi.OpenOfflineSession(this.InstallationPath);

            this._logger.LogInformation(Resources.DismApplyUnattend,
                this.UnattendFile, this.InstallationPath);
            DismApi.ApplyUnattend(session, this.UnattendFile, true);

            this._logger.LogTrace(Resources.DismCommit);
            DismApi.CommitImage(session, false);
            DismApi.CloseSession(session);

            return Task.CompletedTask;
        }
        #endregion
    }
}
