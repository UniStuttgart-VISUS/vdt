// <copyright file="TaskSequence.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 - 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Services;
using Visus.DeploymentToolkit.Tasks;


namespace Visus.DeploymentToolkit.Workflow {

    /// <summary>
    /// The default implementation of a <see cref="ITaskSequence"/>
    /// </summary>
    internal sealed class TaskSequence : ITaskSequence {

        #region Public properties
        /// <inheritdoc />
        public string ID { get; }

        /// <inheritdoc />
        public int Count => this._tasks.Count;

        /// <inheritdoc />
        public Phase Phase { get; init; }
        #endregion

        #region Public methods
        /// <inheritdoc />
        public async Task ExecuteAsync(IState state) {
            _ = state ?? throw new ArgumentNullException(nameof(state));

            if (state.Progress >= this.Count) {
                // Special case when a task sequence that has already completed
                // is executed again.
                this._logger.LogWarning("A task sequence that has already "
                    + "completed was called again. If this is intended, you "
                    + "must reset the progress in the state before restarting "
                    + "the sequence.");
                return;
            }

            for (; state.Progress < this.Count; ++state.Progress) {
                var task = this._tasks[state.Progress];

                try {
                    this._logger.LogInformation("Task #{Progress} {Task} "
                        + "is starting.", state.Progress, task.Name);

                    await task.ExecuteAsync().ConfigureAwait(false);

                    this._logger.LogInformation("Task #{Progress} {Task} "
                        + "completed successfully.", state.Progress, task.Name);
                } catch (Exception ex) {
                    this._logger.LogError(ex, "Task #{Progress} {Task} "
                        + "failed.", state.Progress, task.Name);

                    if (task.IsCritical) {
                        this._logger.LogError(ex, "Task #{Progress} {Task} "
                            + "is critical for the task sequence, which means "
                            + "that it cannot continue.", state.Progress,
                            task.Name);
                        return;
                    }
                }
            }

            this._logger.LogInformation("The task sequence completed "
                + "successfully.");
        }

        /// <inheritdoc />
        public IEnumerator<ITask> GetEnumerator()
            => this._tasks.GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
        #endregion

        #region Internal constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="id">The unique ID of the task sequence.</param>
        /// <param name="logger">The logger for the task sequence.</param>
        /// <param name="phase"></param>
        /// <param name="tasks"></param>
        /// <exception cref="ArgumentNullException"></exception>
        internal TaskSequence(ILogger<TaskSequence> logger,
                string id,
                Phase phase,
                IList<ITask> tasks) {
            this.ID = id ?? throw new ArgumentNullException(nameof(id));
            this.Phase = phase;
            this._logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
            this._tasks = tasks
                ?? throw new ArgumentNullException(nameof(tasks));
        }
        #endregion

        #region Private fields
        private readonly ILogger _logger;
        private readonly IList<ITask> _tasks;
        #endregion
    }
}
