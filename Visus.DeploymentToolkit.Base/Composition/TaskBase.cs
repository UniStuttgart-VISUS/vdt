// <copyright file="TaskBase.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details
// </copyright>
// <author>Christoph Müller</author>


namespace Visus.DeploymentToolkit.Composition {

    /// <summary>
    /// A basic implementation of <see cref="ITask"/> that uses attributes to
    /// implement as many methods as possible.
    /// </summary>
    public abstract class TaskBase : ITask {

        /// <inheritdoc />
        public virtual bool CanExecute(Phase phase) {
            var supported = SupportsPhaseAttribute.GetPhases(this.GetType());
            return (!supported.Any() || supported.Contains(phase));
        }

        /// <inheritdoc />
        public abstract Task ExecuteAsync();
    }


    /// <summary>
    /// A basic implementation of <see cref="ITask{TResult}"/> that uses
    /// attributes to implement as many methods as possible.
    /// </summary>
    public abstract class TaskBase<TResult> : ITask<TResult> {

        /// <inheritdoc />
        public virtual bool CanExecute(Phase phase) {
            var supported = SupportsPhaseAttribute.GetPhases(this.GetType());
            return (!supported.Any() || supported.Contains(phase));
        }

        /// <inheritdoc />
        public abstract Task<TResult> ExecuteAsync();
    }
}
