// <copyright file="TaskSequence.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Services;
using Visus.DeploymentToolkit.Tasks;


namespace Visus.DeploymentToolkit.Workflow
{

    /// <summary>
    /// The default implementation of a <see cref="ITaskSequence"/>
    /// </summary>
    internal sealed class TaskSequence : ITaskSequence {

        #region Public methods
        /// <inheritdoc />
        public async Task ExecuteAsync(Phase phase) {
            var state = new State(this._loggers.CreateLogger<State>());

            foreach (var t in this[phase]) {
                try {
                    this._logger.LogInformation(Resources.TaskStarting,
                        t.Name, phase);
                    await t.ExecuteAsync(state).ConfigureAwait(false);
                    this._logger.LogInformation(Resources.TaskFinished,
                        t.Name, phase);
                } catch (Exception ex) {
                    this._logger.LogError(Errors.TaskFailed, t.Name,
                        phase, ex);
                    if (t.IsCritical) {
                        this._logger.LogCritical(Errors.CriticalTaskFailed,
                            t.Name);
                        return;
                    }
                }
            }
        }
        #endregion

        #region Public indexers
        /// <inheritdoc />
        public IEnumerable<ITask> this[Phase phase] {
            get {
                if (this._tasks.TryGetValue(phase, out var retval)) {
                    return retval;
                } else {
                    return Enumerable.Empty<ITask>();
                }
            }
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
        internal TaskSequence(ILoggerFactory loggers,
                Dictionary<Phase, List<ITask>> tasks) {
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
        private readonly Dictionary<Phase, List<ITask>> _tasks;
        #endregion
    }
}
