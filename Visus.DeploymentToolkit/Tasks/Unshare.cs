// <copyright file="Unshare.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Services;


namespace Visus.DeploymentToolkit.Tasks {

    /// <summary>
    /// Removes a resource shared to the network from this machine.
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed class Unshare : TaskBase {

        #region Public constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="logger"></param>
        public Unshare(IState state,
                ILogger<Unshare> logger)
                : base(state, logger) {
            this.Name = Resources.Unshare;
        }
        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets the name of the share.
        /// </summary>
        [Required]
        public string Share { get; set; } = null!;
        #endregion

        #region Public methods
        /// <inheritdoc />
        public override Task ExecuteAsync(CancellationToken cancellationToken) {
            this.Validate();
            cancellationToken.ThrowIfCancellationRequested();

            this._logger.LogInformation("Removing share \"{Share}\".",
                this.Share);
            NetApi.Unshare(null, this.Share);
            this._logger.LogInformation("The \"{Share}\" was removed "
                + "successfully.", this.Share);

            return Task.CompletedTask;
        }
        #endregion
    }
}
