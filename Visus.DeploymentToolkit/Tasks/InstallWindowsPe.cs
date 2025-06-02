// <copyright file="InstallWindowsPe.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Services;
using Visus.DeploymentToolkit.Workflow;


namespace Visus.DeploymentToolkit.Tasks {

    /// <summary>
    /// 
    /// </summary>
    [SupportsPhase(Phase.PreinstalledEnvironment)]
    public sealed class InstallWindowsPe : TaskBase {

        #region Public constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="tasks"></param>
        /// <param name="logger"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public InstallWindowsPe(IState state,
                ITaskSequenceFactory tasks,
                ILogger<InstallWindowsPe> logger)
                : base(state, logger) {
            this._tasks = tasks
                ?? throw new ArgumentNullException(nameof(tasks));
        }
        #endregion

        #region Public methods
        /// <inheritdoc />
        public override Task ExecuteAsync(
                CancellationToken cancellationToken) {
            this._logger.LogInformation("Creating a task sequence for "
                + "installing Windows PE on a system.");

            this._state.TaskSequence = this._tasks.CreateBuilder()
                .ForPhase(Phase.PreinstalledEnvironment)
                .Add<SelectInstallDisk>(t => {
                })
                .Add<PartitionFormatDisk>()
                .Add<ApplyImage>((t, s) => {
                    ArgumentNullException.ThrowIfNull(s.InstallationDisk);
                    t.Image = Path.Combine("sources", "boot.wim");
                })
                //.Add<Bootse>
                .Build();

            return Task.CompletedTask;
        }
        #endregion

        #region Private fields
        private ITaskSequenceFactory _tasks;
        #endregion
    }

}
