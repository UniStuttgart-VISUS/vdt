// <copyright file="CreateDirectory.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Services;


namespace Visus.DeploymentToolkit.Tasks {

    /// <summary>
    /// A task for preparing an empty directory.
    /// </summary>
    public sealed class CreateDirectory : TaskBase {

        #region Public constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="state">The current state of the task sequence.
        /// </param>
        /// <param name="directoryService">The directory service to create the
        /// requested path.</param>
        /// <param name="logger">A logger for recording progress and errors.
        /// </param>
        /// <exception cref="ArgumentNullException">If any of the parameters
        /// is <c>null</c>.</exception>
        public CreateDirectory(IState state,
                IDirectory directoryService,
                ILogger<CreateDirectory> logger)
                : base(state, logger) {
            this._directoryService = directoryService
                ?? throw new ArgumentNullException(nameof(directoryService));
            this.Name = Resources.CreateDirectory;
        }
        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets whether the task should make sure that the directory
        /// is empty. If <c>true</c>, the task will delete all existing
        /// contents.
        /// </summary>
        public bool Clean { get; set; } = false;

        /// <summary>
        /// Gets or sets whether the desired directory already existing
        /// indicates a failure.
        /// </summary>
        public bool MustNotExist { get; set; } = false;

        /// <summary>
        /// Gets or sets the path of the directory to be created.
        /// </summary>
        [Required]
        public string Path { get; set; } = null!;

        /// <summary>
        /// Gets or sets the name of a state variable to which the path is
        /// propagated on success.
        /// </summary>
        public string? State { get; set; }
        #endregion

        #region Public methods
        /// <inheritdoc />
        public override async Task ExecuteAsync(
                CancellationToken cancellationToken) {
            this.Validate();
            cancellationToken.ThrowIfCancellationRequested();

            this._logger.LogInformation("Creating a directory at {Path}.",
                this.Path);
            this.Path = (await this._directoryService.CreateAsync(this.Path)
                .ConfigureAwait(false))
                .FullName;

            if (this.Clean) {
                cancellationToken.ThrowIfCancellationRequested();
                this._logger.LogTrace("Making sure that {Path} is empty.",
                    this.Path);
                await this._directoryService.CleanAsync(this.Path)
                    .ConfigureAwait(false);
            }

            if (!string.IsNullOrWhiteSpace(this.State)) {
                this._logger.LogTrace("Storing directory path to {State}.",
                    this.State);
                this._state[this.State] = this.Path;
            }
        }
        #endregion

        #region Protected methods
        /// <inheritdoc />
        protected override void Validate() {
            base.Validate();

            if (this.MustNotExist) {
                throw new ValidationException(string.Format(
                    Errors.DirectoryExists,
                    this.Path));
            }
        }
        #endregion

        #region Private fields
        private readonly IDirectory _directoryService;
        #endregion
    }
}
