// <copyright file="TaskSequenceBuilder.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 - 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Services;
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
        /// <param name="serviceProvider">The service provider that allows the
        /// builder instancing new tasks. All tasks that should be added to a
        /// task sequence must have been registered before.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public TaskSequenceBuilder(IServiceProvider serviceProvider) {
            this._services = serviceProvider
                ?? throw new ArgumentNullException(nameof(serviceProvider));
            this._logger = this.CreateLogger<TaskSequenceBuilder>();
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
        public ITaskSequenceBuilder Add<TTask>(
                Action<TTask>? configure = null) where TTask : ITask {
            var task = this._services.GetRequiredService<TTask>();

            if (configure != null) {
                configure(task);
            }

            return this.Add(task);
        }

        /// <inheritdoc />
        public ITaskSequenceBuilder Add<TTask>(Action<TTask, IState> configure)
                where TTask : ITask {
            ArgumentNullException.ThrowIfNull(configure);
            var t = this._services.GetRequiredService<TTask>();
            var task = new SelfConfiguringTask<TTask>(t, configure,
                this._services);
            return this.Add(task);
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
        public ITaskSequenceBuilder FromDescription(
                ITaskSequenceDescription desc) {
            _ = desc ?? throw new ArgumentNullException(nameof(desc));

            this.ForPhase(desc.Phase);

            foreach (var t in desc.Tasks) {
                this.Add(t.ToTask());
            }

            return this;
        }

        /// <inheritdoc />
        public ITaskSequence Build() {
            return new TaskSequence(
                this._services.GetRequiredService<ILogger<TaskSequence>>(),
                this._phase,
                this._tasks);
        }

        /// <inheritdoc />
        public ITaskSequenceBuilder Insert(int index, ITask task) {
            this.CheckTask(task);
            this._logger.LogDebug("Inserting task \"{Task}\" at position "
                + "{Index} in the sequence.", task.Name, index);
            this._tasks.Insert(index, task);
            return this;
        }

        /// <inheritdoc />
        public ITaskSequenceBuilder Insert<TTask>(int index,
                Action<TTask>? configure = null) where TTask : ITask {
            var task = this._services.GetRequiredService<TTask>();

            if (configure != null) {
                configure(task);
            }

            return this.Insert(index, task);
        }

        /// <inheritdoc />
        public ITaskSequenceBuilder Insert<TTask>(int index,
                Action<TTask, IState> configure)
                where TTask : ITask {
            ArgumentNullException.ThrowIfNull(configure);
            var t = this._services.GetRequiredService<TTask>();
            var task = new SelfConfiguringTask<TTask>(t, configure,
                this._services);
            return this.Insert(index, task);
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

        /// <summary>
        /// Instantiate a new logger using <see cref="_services"/>.
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <returns></returns>
        private ILogger<TType> CreateLogger<TType>() {
            Debug.Assert(this._services != null);
            var factory = this._services.GetRequiredService<ILoggerFactory>();
            return factory.CreateLogger<TType>();
        }
        #endregion

        #region Private fields
        private readonly ILogger _logger;
        private Phase _phase = Phase.Unknown;
        private readonly IServiceProvider _services;
        private readonly List<ITask> _tasks = new();
        #endregion
    }
}
