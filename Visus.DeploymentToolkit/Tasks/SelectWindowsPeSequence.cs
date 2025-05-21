// <copyright file="SelectWindowsPeSequence.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Services;
using Visus.DeploymentToolkit.Workflow;


namespace Visus.DeploymentToolkit.Tasks {

    /// <summary>
    /// A task that either restores the configured
    /// <see cref="IState.TaskSequence"/> or creates the default WinPE servicing
    /// task sequence.
    /// </summary>
    [SupportsPhase(Workflow.Phase.PreinstalledEnvironment)]
    public sealed class SelectWindowsPeSequence : SelectTaskSequenceBase {

        #region Public constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="store"></param>
        /// <param name="logger"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public SelectWindowsPeSequence(IState state,
                ITaskSequenceStore? store,
                ITaskSequenceFactory tasks,
                ILogger<SelectWindowsPeSequence> logger)
            : base(state, store, tasks, logger) { }

        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="logger"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public SelectWindowsPeSequence(IState state,
                ITaskSequenceFactory tasks,
                ILogger<SelectWindowsPeSequence> logger)
            : base(state, null, tasks, logger) { }
        #endregion

        /// <inheritdoc />
        public override async Task ExecuteAsync(
                CancellationToken cancellationToken) {
            if (this._state.TaskSequence is string id) {
                var desc = await this.GetTaskSequenceAsync(id);
                if (desc != null) {
                    this._state.TaskSequence = this._tasks.CreateBuilder()
                        .FromDescription(desc)
                        .Build();
                }
            }

            if (this.IsNoTaskSequence) {
                this._logger.LogInformation("Creating default servicing task "
                    + "sequence for Windows PE.");

                this._state.TaskSequence = this._tasks.CreateBuilder()
                    .ForPhase(Phase.PreinstalledEnvironment)
                    .Add<CopyWindowsPe>()
                    .Add<MountWim>()
                    .Add<UnmountWim>()
                    .Add<CreateWindowsPeIso>()
                    .Build();
            }

            CheckPhase(this._state.TaskSequence as ITaskSequence,
                Phase.PreinstalledEnvironment);
        }

    }
}
