// <copyright file="TaskSequence.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 - 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
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
        public int Length => this._tasks.Count;
        #endregion

        #region Public methods
        /// <inheritdoc />
        public async Task ExecuteAsync(IState state) {
            _ = state ?? throw new ArgumentNullException(nameof(state));

            if (state.Progress >= this.Length) {
                // Special case when a task sequence that has already completed
                // is executed again.
                this._logger.LogWarning("A task sequence that has already "
                    + "completed was called again. If this is intended, you "
                    + "must reset the progress in the state before restarting "
                    + "the sequence.");
                return;
            }

            for (int i = state.Progress; i < this.Length; i++) {
                var task = this._tasks[i];

                try {
                    this._logger.LogInformation("Task #{Progress} \"{Task}\" "
                        + "is starting.", i, task.Name);

                    await task.ExecuteAsync(state).ConfigureAwait(false);

                    this._logger.LogInformation("Task #{Progress} \"{Task}\" "
                        + "completed successfully.", i, task.Name);

                    state.Set(WellKnownStates.Progress, i + 1);
                } catch (Exception ex) {
                    this._logger.LogError(ex, "Task #{Progress} \"{Task}\" "
                        + "failed.", i, task.Name);

                    if (task.IsCritical) {
                        this._logger.LogError(ex, "Task #{Progress} \"{Task}\" "
                            + "is critical for the task sequence, which means "
                            + "that it cannot continue.", i, task.Name);
                        return;
                    }
                }
            }

            this._logger.LogInformation("The task sequence completed "
                + "successfully.");
        }
        #endregion

        #region Internal constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="loggers">A factory for creating the loggers used in
        /// the task sequence.</param>
        /// <param name="tasks"></param>
        /// <exception cref="ArgumentNullException"></exception>
        internal TaskSequence(ILoggerFactory loggers, IList<ITask> tasks) {
            this._loggers = loggers
                ?? throw new ArgumentNullException(nameof(loggers));
            this._logger = loggers.CreateLogger<TaskSequence>();
            this._tasks = tasks
                ?? throw new ArgumentNullException(nameof(tasks));
        }
        #endregion

        #region Private fields
        private readonly ILogger _logger;
        private readonly ILoggerFactory _loggers;
        private readonly IList<ITask> _tasks;
        #endregion
    }
}
