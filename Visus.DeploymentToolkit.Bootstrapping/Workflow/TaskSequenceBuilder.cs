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
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="loggerFactory"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public TaskSequenceBuilder(ILoggerFactory loggerFactory) {
            this._loggerFactory = loggerFactory
                ?? throw new ArgumentNullException(nameof(loggerFactory));
            this._logger = this._loggerFactory
                .CreateLogger<TaskSequenceBuilder>();
        }
        #endregion

        #region Public methods
        /// <inheritdoc />
        public ITaskSequenceBuilder Add(ITask task) {
            this.CheckTask(task);
            this._logger.LogDebug("Adding task \"{Task}\" at the end of the"
                + " sequence.", task.Name);
            this._tasks.Add(task);
            return this;
        }

        /// <inheritdoc />
        public ITaskSequenceBuilder ForPhase(Phase phase) {
            if (this._phase != Phase.Unknown) {
                throw new InvalidOperationException(string.Format(
                    Errors.PhaseAlreadySet, this._phase));
            }

            this._phase = phase;
            return this;
        }

        /// <inheritdoc />
        public ITaskSequence Build() {
            return new TaskSequence(this._loggerFactory, this._tasks);
        }

        /// <inheritdoc />
        public ITaskSequenceBuilder Insert(int index, ITask task) {
            this.CheckTask(task);
            this._logger.LogDebug("Inserting task \"{Task}\" at position "
                + "{Index} in the sequence.", task.Name, index);
            this._tasks.Insert(index, task);
            return this;
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Checks whether the <paramref name="task"/> is valid and can run in
        /// the given <see cref="_phase"/>.
        /// </summary>
        /// <param name="task"></param>
        private void CheckTask(ITask task) {
            _ = task ?? throw new ArgumentNullException(nameof(task));

            if (this._phase == Phase.Unknown) {
                throw new InvalidOperationException(Errors.PhaseNotSet);
            }

            if (!task.CanExecute(this._phase)) {
                throw new ArgumentException(
                    string.Format(Errors.TaskCannotExecute, this._phase),
                    nameof(task));
            }
        }
        #endregion

        #region Private fields
        private readonly ILogger _logger;
        private readonly ILoggerFactory _loggerFactory;
        private Phase _phase = Phase.Unknown;
        private readonly List<ITask> _tasks = new();
        #endregion
    }
}
