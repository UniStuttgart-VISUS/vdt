// <copyright file="TaskSequenceDescriptionBuilder.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using Visus.DeploymentToolkit.Properties;


namespace Visus.DeploymentToolkit.Workflow {

    /// <summary>
    /// Implementation of the <see cref="ITaskSequenceDescriptionBuilder"/>.
    /// </summary>
    internal sealed class TaskSequenceDescriptionBuilder
            : ITaskSequenceDescriptionBuilder {

        #region Public constructors
        public TaskSequenceDescriptionBuilder(
                ILogger<TaskSequenceDescriptionBuilder> logger) {
            this._logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
        }
        #endregion

        /// <inheritdoc />
        public ITaskSequenceDescriptionBuilder Add(string task,
                IDictionary<string, object?>? parameters) {
            ArgumentNullException.ThrowIfNull(task, nameof(task));
            this._tasks.Add(new TaskDescription() {
                Task = task,
                Parameters = parameters ?? new Dictionary<string, object?>()
            });
            return this;
        }

        /// <inheritdoc />
        public ITaskSequenceDescriptionBuilder Add(ITaskDescription task) {
            ArgumentNullException.ThrowIfNull(task, nameof(task));
            this._tasks.Add(task);
            return this;
        }

        /// <inheritdoc />
        public ITaskSequenceDescription Build() {
            return new TaskSequenceDescription() {
                Phase = this._phase,
                Tasks = this._tasks,
            };
        }

        /// <inheritdoc />
        public ITaskSequenceDescriptionBuilder ForPhase(Phase phase) {
            if (this._phase != Phase.Unknown) {
                throw new InvalidOperationException(string.Format(
                    Errors.PhaseAlreadySet, this._phase));
            }

            this._phase = phase;
            return this;
        }

        /// <inheritdoc />
        public ITaskSequenceDescriptionBuilder Insert(int index,
                string task,
                IDictionary<string, object?>? parameters) {
            ArgumentNullException.ThrowIfNull(task, nameof(task));
            this._tasks.Insert(index, new TaskDescription() {
                Task = task,
                Parameters = parameters ?? new Dictionary<string, object?>()
            });
            return this;
        }

        /// <inheritdoc />
        public ITaskSequenceDescriptionBuilder Insert(int index,
                ITaskDescription task) {
            ArgumentNullException.ThrowIfNull(task, nameof(task));
            this._tasks.Insert(index, task);
            return this;
        }

        #region Private fields
        private readonly ILogger _logger;
        private Phase _phase = Phase.Unknown;
        private readonly List<ITaskDescription> _tasks = new();
        #endregion
    }
}
