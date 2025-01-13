// <copyright file="ITask.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 - 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Threading;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Services;
using Visus.DeploymentToolkit.Workflow;


namespace Visus.DeploymentToolkit.Tasks {

    /// <summary>
    /// Defines the interface of a deployment task that asynchronously performs
    /// some work, but does not return any result to be used.
    /// </summary>
    public interface ITask {

        #region Public properties
        /// <summary>
        /// Inidicates whether the task is critical.
        /// </summary>
        /// <remarks>
        /// If a <see cref="ITask"/> is marked critical, the
        /// <see cref="ITaskSequence"/> cannot continue if
        /// <see cref="ExecuteAsync(IState)"/> throws an exception.
        /// Otherwise, this exception will be ignored and the task sequence
        /// continues with the next task.
        /// </remarks>
        bool IsCritical { get; }

        /// <summary>
        /// Gets the name of the task.
        /// </summary>
        string Name { get; }
        #endregion

        #region Public methods
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
        /// <param name="cancellationToken">A cancellation token for aborting
        /// the task.</param>
        /// <returns>A <see cref="Task"/> to wait for completion.</returns>
        Task ExecuteAsync(CancellationToken cancellationToken);
        #endregion
    }
}
