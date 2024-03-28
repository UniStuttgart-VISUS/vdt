// <copyright file="TaskExtensions.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Threading;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Services;


namespace Visus.DeploymentToolkit.Tasks {

    /// <summary>
    /// Extension methods for <see cref="ITask"/>.
    /// </summary>
    public static class TaskExtensions {

        /// <summary>
        /// Asynchronously executes a task.
        /// </summary>
        /// <param name="state">The global application state, which can be used
        /// to retrieve data from a previous <see cref="ITask"/> or leave data
        /// for a subsequent one. It can be assumet that the built-in task
        /// sequence will always provide a non-<c>null</c> global state.</param>
        /// <returns>A <see cref="Task"/> to wait for completion.</returns>
        /// <exception cref="System.ArgumentNullException">If
        /// <paramref name="state"/> is <c>null</c>.</exception>
        public static Task ExecuteAsync(this ITask that, IState state)
            => that.ExecuteAsync(state, CancellationToken.None);
    }
}
