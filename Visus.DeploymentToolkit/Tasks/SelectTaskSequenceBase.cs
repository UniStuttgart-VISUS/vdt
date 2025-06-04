// <copyright file="SelectTaskSequenceBase.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Services;
using Visus.DeploymentToolkit.Workflow;


namespace Visus.DeploymentToolkit.Tasks {

    /// <summary>
    /// A base class for tasks that select a task sequence to be executed.
    /// </summary>
    public abstract class SelectTaskSequenceBase : TaskBase {

        #region Protected constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="state">The current state of the task sequence.</param>
        /// <param name="store">The task sequence store that provides the task
        /// selected that can be selected.</param>
        /// <param name="tasks">A task sequence factory that allows for creating
        /// new task sequences.</param>
        /// <param name="logger">A logger for progress and error messages.
        /// </param>
        /// <exception cref="ArgumentNullException">If <paramref name="tasks"/>
        /// is <c>null</c>.</exception>
        public SelectTaskSequenceBase(IState state,
                ITaskSequenceStore? store,
                ITaskSequenceFactory tasks,
                ILogger logger)
                : base(state, logger) {
            this._store = store;
            this._tasks = tasks
                ?? throw new ArgumentNullException(nameof(tasks));
        }
        #endregion

        #region Protected properties
        /// <summary>
        /// Gets whether there is no task sequence or reference to a task
        /// sequence in the state.
        /// </summary>
        protected bool IsNoTaskSequence
            => (this._state.TaskSequence == null)
            || ((this._state.TaskSequence is string n)
            && string.IsNullOrWhiteSpace(n));
        #endregion

        #region Protected methods
        /// <summary>
        /// Check whether the given task sequence is for the
        /// <paramref name="expected"/> phase or throw.
        /// </summary>
        /// <param name="taskSequence"></param>
        /// <param name="expected"></param>
        /// <exception cref="ArgumentException"></exception>
        protected static void CheckPhase(ITaskSequence? taskSequence,
                Phase expected) {
            if (taskSequence == null) {
                throw new ArgumentException(string.Format(
                    Errors.UnexpectedPhase,
                    Phase.Unknown,
                    expected));
            }
            if (taskSequence.Phase != expected) {
                throw new ArgumentException(string.Format(
                    Errors.UnexpectedPhase,
                    Phase.Unknown,
                    expected));
            }
        }

        /// <summary>
        /// Searches the task sequence with the specified ID in the store.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        protected Task<ITaskSequenceDescription?> GetTaskSequenceAsync(
                string id) {
            this._logger.LogInformation("Searching task sequence {ID}.", id);
            return (this._store == null)
                ? Task.FromResult<ITaskSequenceDescription?>(null)
                : this._store.GetTaskSequenceAsync(id);
        }
        #endregion

        #region Protected fields
        protected readonly ITaskSequenceFactory _tasks;
        #endregion

        #region Private fields
        private readonly ITaskSequenceStore? _store;
        #endregion
    }
}
