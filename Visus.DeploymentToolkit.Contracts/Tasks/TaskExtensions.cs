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
        public static Task ExecuteAsync(this ITask that)
            => that.ExecuteAsync(CancellationToken.None);
    }
}
