// <copyright file="SelfConfiguringTask.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Services;
using Visus.DeploymentToolkit.Tasks;


namespace Visus.DeploymentToolkit.Workflow {

    /// <summary>
    /// A task wrapper that executes a configuration step before a task.
    /// </summary>
    /// <typeparam name="TTask">The task that is being wrapped.</typeparam>
    /// <param name="task">The task to be executed</param>
    /// <param name="configure">The configuration callback to be executed
    /// immediately before the task.</param>
    /// <param name="services">The service provider for retrieving the state.
    /// </param>
    public sealed class SelfConfiguringTask<TTask>(TTask task,
            Action<TTask, IState> configure,
            IServiceProvider services)
            : ITask where TTask: ITask {

        ///// <summary>
        ///// Converts a self-configuring task to the task it wraps.
        ///// </summary>
        ///// <param name="task">A self configuring task.</param>
        ///// <returns>The wrapped task.</returns>
        //public static implicit operator TTask(SelfConfiguringTask<TTask> task)
        //    => task._task;

        /// <inheritdoc />
        public bool IsCritical => this._task.IsCritical;

        /// <inheritdoc />
        public string Name => this._task.Name;

        /// <inheritdoc />
        public bool CanExecute(Phase phase) => this._task.CanExecute(phase);

        /// <inheritdoc />
        public Task ExecuteAsync(CancellationToken cancellationToken) {
            Debug.Assert(this._configure != null);
            Debug.Assert(this._task != null);
            Debug.Assert(this._services != null);
            var state = this._services.GetRequiredService<IState>();

            this._logger.LogTrace("Configuring task {Task}.",
                this._task.Name);
            this._configure(this._task, state);

            return this._task.ExecuteAsync(cancellationToken);
        }

        #region Private fields
        private readonly Action<TTask, IState> _configure = configure
            ?? throw new ArgumentNullException(nameof(configure));
        private readonly ILogger _logger = services.GetRequiredService<
                ILogger<SelfConfiguringTask<TTask>>>();
        private readonly IServiceProvider _services = services
            ?? throw new ArgumentNullException(nameof(services));
        private readonly TTask _task = task
            ?? throw new ArgumentNullException(nameof(task));
        #endregion
    }
}
