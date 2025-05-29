// <copyright file="TaskSequenceExtensions.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Workflow;


namespace Visus.DeploymentToolkit.Extensions {

    /// <summary>
    /// Extension methods for task sequences.
    /// </summary>
    public static class TaskSequenceExtensions {

        /// <summary>
        /// Name the task sequence and save to the specified location.
        /// </summary>
        /// <param name="that"></param>
        /// <param name="path"></param>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <returns>A task to wait for the operation to complete.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="that"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If <paramref name="path"/>
        /// is <c>null</c> or empty, or if <paramref name="id"/> is <c>null</c>
        /// or empty, or if <paramref name="name"/> is <c>null</c> or empty.
        /// </exception>
        public static Task SaveAsync(this ITaskSequence that,
                string path,
                string id,
                string name,
                string? description = null) {
            ArgumentException.ThrowIfNullOrWhiteSpace(path);
            ArgumentException.ThrowIfNullOrWhiteSpace(id);
            ArgumentException.ThrowIfNullOrWhiteSpace(name);

            var desc = TaskSequenceDescription.FromTaskSequence(that);
            desc.Description = description;
            desc.ID = id;
            desc.Name = name;

            return desc.SaveAsync(path);
        }
    }
}
