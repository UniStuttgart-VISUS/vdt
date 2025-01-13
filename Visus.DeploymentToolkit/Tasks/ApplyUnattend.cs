// <copyright file="ApplyUnattend.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 - 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Dism;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
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
        /// <param name="state"></param>
        /// <param name="dism"></param>
        /// <param name="logger">The logger used to report results of the
        /// operation.</param>
        public ApplyUnattend(IState state,
                IDismScope dism,
                ILogger<ApplyUnattend> logger)
                : base(state,logger) {
            this._dism = dism ?? throw new ArgumentNullException(nameof(dism));
        }
        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets the path to the Windows installation being modified.
        /// </summary>
        /// <remarks>
        /// If this path is <c>null</c>, an online servicing session for the
        /// current Windows installation is opened.
        /// </remarks>
        public string? InstallationPath { get; set; }

        /// <summary>
        /// Gets or sets the path to the unattend file to apply.
        /// </summary>
        [Required]
        public string UnattendFile { get; set; } = null!;
        #endregion

        #region Public methods
        /// <inheritdoc />
        public override Task ExecuteAsync(CancellationToken cancellationToken) {
            this._logger.LogInformation("Opening a DISM servicing session "
                + "for \"{Path}\" to apply unattend settings.",
                this.InstallationPath);
            using var session = (this.InstallationPath is null)
                ? DismApi.OpenOnlineSession()
                : DismApi.OpenOfflineSession(this.InstallationPath);

            this._logger.LogInformation("Applying unattend file "
                + "\"{UnattendFile}\" to \"{Path}\".",
                this.UnattendFile, this.InstallationPath);
            DismApi.ApplyUnattend(session, this.UnattendFile, true);

            this._logger.LogTrace("Committing changes to \"{Path}\".",
                this.InstallationPath);
            DismApi.CommitImage(session, false);
            DismApi.CloseSession(session);

            return Task.CompletedTask;
        }
        #endregion

        #region Private fields
        private readonly IDismScope _dism;
        #endregion
    }
}
