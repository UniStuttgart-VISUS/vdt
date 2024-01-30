// <copyright file="ITask.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details
// </copyright>
// <author>Christoph Müller</author>


namespace Visus.DeploymentToolkit.Composition {

    /// <summary>
    /// Defines the interface of a deployment task that asynchronously performs
    /// some work, but does not return any result to be used.
    /// </summary>
    public interface ITask {

        /// <summary>
        /// Answer whether the task can be executed in its current state
        /// assuming the given deployment <paramref name="phase"/>.
        /// </summary>
        /// <param name="phase"></param>
        /// <returns></returns>
        bool CanExecute(Phase phase);

        /// <summary>
        /// Asynchronously executes a task.
        /// </summary>
        /// <returns>A <see cref="Task"/> to wait for completion.</returns>
        Task ExecuteAsync();
    }


    /// <summary>
    /// Defines the interface of a deployment task that asynchronously performs
    /// some work and returns a result to be used for subsequent tasks.
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public interface ITask<TResult> {

        /// <summary>
        /// Answer whether the task can be executed in its current state
        /// assuming the given deployment <paramref name="phase"/>.
        /// </summary>
        /// <param name="phase"></param>
        /// <returns></returns>
        bool CanExecute(Phase phase);

        /// <summary>
        /// Asynchronously executes the task.
        /// </summary>
        /// <returns>A <see cref="Task"/> to wait for the result.</returns>
        Task<TResult> ExecuteAsync();
    }
}
