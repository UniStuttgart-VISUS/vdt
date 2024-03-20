// <copyright file="ServiceCollectionExtensions.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Tasks;


namespace Visus.DeploymentToolkit.Workflow {

    /// <summary>
    /// Default implementation of the <see cref="ITaskSequenceBuilder"/>.
    /// </summary>
    internal sealed class TaskSequenceBuilder : ITaskSequenceBuilder {

        #region Public constructors
        public TaskSequenceBuilder(ILogger<TaskSequenceBuilder> logger) {
            this._logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
        }
        #endregion

        #region Public methods
        /// <inheritdoc />
        public ITaskSequenceBuilder Add(Phase phase, ITask task) {
            CheckTask(phase, task);
            this._logger.LogDebug(Resources.AddTask, task, phase);
            this.GetTasks(phase).Add(task);
            return this;
        }

        /// <inheritdoc />
        public ITaskSequence Build() {
            // TODO: should we deep copy here?
            //return new TaskSequence(this._phases);
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public ITaskSequenceBuilder Insert(Phase phase, int index, ITask task) {
            CheckTask(phase, task);
            this._logger.LogDebug(Resources.InsertTask, task, phase, index);
            this.GetTasks(phase).Insert(index, task);
            return this;
        }
        #endregion

        #region Private class methods
        /// <summary>
        /// Checks whether the <paramref name="task"/> is valid and can run in
        /// the given <paramref name="phase"/>.
        /// </summary>
        /// <param name="phase"></param>
        /// <param name="task"></param>
        private static void CheckTask(Phase phase, ITask task) {
            _ = task ?? throw new ArgumentNullException(nameof(task));

            if (!task.CanExecute(phase)) {
                throw new ArgumentException(
                    string.Format(Errors.TaskCannotExecute, phase),
                    nameof(task));
            }
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Gets or creates the task list for the given phase.
        /// </summary>
        /// <param name="phase"></param>
        /// <returns></returns>
        private IList<ITask> GetTasks(Phase phase) {
            if (!this._phases.TryGetValue(phase, out var retval)) {
                this._logger.LogDebug(Resources.CreatingTaskListForPhase,
                    phase);
                this._phases.Add(phase, retval = new List<ITask>());
            }

            return retval;
        }
        #endregion

        #region Private fields
        private readonly ILogger _logger;
        private readonly Dictionary<Phase, List<ITask>> _phases = new();
        #endregion
    }
}
