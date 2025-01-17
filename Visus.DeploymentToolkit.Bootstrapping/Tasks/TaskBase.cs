// <copyright file="TaskBase.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 - 2025 Visualisierungsinstitut der Universität Stuttgart.
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
    /// A basic implementation of <see cref="ITask"/> that uses attributes to
    /// implement as many methods as possible.
    /// </summary>
    public abstract class TaskBase : ITask {

        #region Public properties
        /// <inheritdoc />
        public bool IsCritical { get; set; } = true;

        /// <inheritdoc />
        public string Name { get; init; }
        #endregion

        #region Public methods
        /// <inheritdoc />
        public virtual bool CanExecute(Phase phase) {
            return SupportsPhaseAttribute.Check(GetType(), phase);
        }

        /// <inheritdoc />
        public abstract Task ExecuteAsync(CancellationToken cancellationToken);
        #endregion

        #region Protected constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="logger">The logger for writing progress and
        /// error notes.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="logger"/>
        /// is <c>null</c>, or if <paramref name="state"/> is <c>null</c>.
        /// </exception>
        protected TaskBase(IState state, ILogger logger) {
            this._logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
            this._state = state
                ?? throw new ArgumentNullException(nameof(state));
            this.Name = $"{this.GetType().Name} {Guid.NewGuid().ToString("N")}";
        }
        #endregion

        #region Protected fields
        /// <summary>
        /// A logger for writing progress and error notes.
        /// </summary>
        protected readonly ILogger _logger;

        /// <summary>
        /// The global application state tracking progress through the workflow.
        /// </summary>
        protected readonly IState _state;
        #endregion
    }
}
