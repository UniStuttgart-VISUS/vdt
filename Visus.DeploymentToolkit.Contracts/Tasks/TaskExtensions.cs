// <copyright file="TaskExtensions.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;


namespace Visus.DeploymentToolkit.Tasks {

    /// <summary>
    /// Extension methods for <see cref="ITask"/>.
    /// </summary>
    public static class TaskExtensions {

        /// <summary>
        /// Asynchronously executes a task.
        /// </summary>
        /// <param name="that"></param>
        public static Task ExecuteAsync(this ITask that)
            => that.ExecuteAsync(CancellationToken.None);

        /// <summary>
        /// Collects the properties of the given <paramref name="task"/> forming
        /// parameters as a <see cref="IDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        public static IDictionary<string, object?> GetParameters(
                this ITask task) {
            var flags = BindingFlags.Public | BindingFlags.Instance;
            var type = task.GetType();
            return (from p in type.GetProperties(flags)
                    where p.CanRead && p.CanWrite
                    select p).ToDictionary(p => p.Name, p => p.GetValue(task));
        }
    }
}
