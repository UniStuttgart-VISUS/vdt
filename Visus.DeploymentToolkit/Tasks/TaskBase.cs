// <copyright file="TaskBase.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;


namespace Visus.DeploymentToolkit.Tasks {

    /// <summary>
    /// A basic implementation of <see cref="ITask"/> that uses attributes to
    /// implement as many methods as possible.
    /// </summary>
    public abstract class TaskBase : ITask {

        /// <inheritdoc />
        public virtual bool CanExecute(Phase phase) {
            return SupportsPhaseAttribute.Check(GetType(), phase);
        }

        /// <inheritdoc />
        public abstract Task ExecuteAsync();

        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="logger">The logger for writing progress and
        /// error notes.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="logger"/>
        /// is <c>null</c>.</exception>
        protected TaskBase(ILogger logger) {
            this._logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// A logger for writing progress and error notes.
        /// </summary>
        protected readonly ILogger _logger;
    }


    /// <summary>
    /// A basic implementation of <see cref="ITask{TResult}"/> that uses
    /// attributes to implement as many methods as possible.
    /// </summary>
    public abstract class TaskBase<TResult> : ITask<TResult> {

        /// <inheritdoc />
        public virtual bool CanExecute(Phase phase) {
            return SupportsPhaseAttribute.Check(GetType(), phase);
        }

        /// <inheritdoc />
        public abstract Task<TResult> ExecuteAsync();

        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="logger">The logger for writing progress and
        /// error notes.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="logger"/>
        /// is <c>null</c>.</exception>
        protected TaskBase(ILogger logger) {
            this._logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// A logger for writing progress and error notes.
        /// </summary>
        protected readonly ILogger _logger;
    }
}
