// <copyright file="ShareFolder.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Services;
using Visus.DeploymentToolkit.Validation;


namespace Visus.DeploymentToolkit.Tasks {

    /// <summary>
    /// Creates a network share for a specific folder.
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed class ShareFolder : TaskBase {

        #region Public constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="logger"></param>
        public ShareFolder(IState state,
                ILogger<ShareFolder> logger)
                : base(state, logger) {
            this.Name = Resources.ShareFolder;
        }
        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets the path to the folder to share.
        /// </summary>
        [DirectoryExists]
        public string Folder { get; set; } = null!;

        /// <summary>
        /// Gets or sets the number of maximum concurrent users the share
        /// allows.
        /// </summary>
        /// <remarks>
        /// If <see cref="uint.MaxValue"/>, which is the default, the number of
        /// concurrent users is unlimited.
        /// </remarks>
        public uint MaxUsers { get; set; } = uint.MaxValue;

        /// <summary>
        /// Gets or sets an optional description of the share.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the name of the share.
        /// </summary>
        /// <remarks>
        /// If no share name is specified, the name of the folder is used.
        /// </remarks>
        public string? Share { get; set; }
        #endregion

        #region Public methods
        /// <inheritdoc />
        public override Task ExecuteAsync(CancellationToken cancellationToken) {
            this.Validate();
            cancellationToken.ThrowIfCancellationRequested();

            return Task.Run(() => {
                if (string.IsNullOrWhiteSpace(this.Share)) {
                    this.Share = System.IO.Path.GetFileName(this.Folder);
                }

                this._logger.LogInformation("Creating share \"{Share}\" for"
                    + " folder \"{Folder}\".", this.Share, this.Folder);
                NetApi.ShareFolder(null,
                    this.Share,
                    this.Folder,
                    this.MaxUsers);
                this._logger.LogInformation("The \"{Share}\" was created "
                    + "successfully.", this.Share);
            }, cancellationToken);
        }
        #endregion
    }
}
