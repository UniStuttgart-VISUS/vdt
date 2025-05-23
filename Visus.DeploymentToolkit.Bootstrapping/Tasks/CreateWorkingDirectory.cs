// <copyright file="CreateWorkingDirectory.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Extensions;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Services;


namespace Visus.DeploymentToolkit.Tasks {

    /// <summary>
    /// A task for preparing an empty working directory.
    /// </summary>
    /// <remarks>
    /// As we assume that there can be only one working directory at a time,
    /// this task will also set the <see cref="IState.WorkingDirectory"/>
    /// property.
    /// </remarks>
    public sealed class CreateWorkingDirectory : TaskBase {

        #region Public constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="directoryService"></param>
        /// <param name="logger"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public CreateWorkingDirectory(IState state,
                IDirectory directoryService,
                ILogger<CreateWorkingDirectory> logger)
                : base(state, logger) {
            this._directoryService = directoryService
                ?? throw new ArgumentNullException(nameof(directoryService));
            this.Name = Resources.CreateWorkingDirectory;
        }
        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets the path of the working directory to be created.
        /// </summary>
        /// <remarks>
        /// If not valid path was specified, the task will take the working
        /// directory from the current <see cref="IState"/>.
        /// </remarks>
        [FromState(WellKnownStates.WorkingDirectory)]
        [Required]
        public string Path { get; set; } = null!;
        #endregion

        #region Public methods
        /// <inheritdoc />
        public override async Task ExecuteAsync(
                CancellationToken cancellationToken) {
            this.CopyFrom(this._state);
            this.Validate();

            this._logger.LogInformation("Creating working directory at "
                + "\"{WorkingDirectory}\".", this.Path);
            var dir = await this._directoryService.CreateAsync(this.Path)
                .ConfigureAwait(false);

            this._logger.LogTrace("Making sure that the working directory "
                + "is empty.");
            await this._directoryService.CleanAsync(dir.FullName)
                .ConfigureAwait(false);

            this._state.WorkingDirectory = dir.FullName;
        }
        #endregion

        #region Private fields
        private readonly IDirectory _directoryService;
        #endregion
    }
}
