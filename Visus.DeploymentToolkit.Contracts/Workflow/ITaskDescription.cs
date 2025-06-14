// <copyright file="ITaskDescription.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Collections.Generic;
using Visus.DeploymentToolkit.Tasks;


namespace Visus.DeploymentToolkit.Workflow {

    /// <summary>
    /// The description of an <see cref="Tasks.ITask"/> that needs to be
    /// instantiated to create an actual task sequence from a description.
    /// </summary>
    /// <remarks>
    /// The serialised version of a task description typically contains more
    /// properties that map to the properties of the task they describe. A
    /// custom serialiser is used to restore this information. This is, however,
    /// not relevant for the public interface, so we only provide the ability
    /// to restore the task here.
    /// </remarks>
    public interface ITaskDescription {

        /// <summary>
        /// Gets a description of the parameters of the task.
        /// </summary>
        IEnumerable<IParameterDescription> DeclaredParameters { get; }

        /// <summary>
        /// Gets the type name of the task to be instantiated.
        /// </summary>
        string Task { get; }

        /// <summary>
        /// Gets the type resolved from <see cref="Task"/>.
        /// </summary>
        Type Type { get; }

        /// <summary>
        /// Restores a task instance from the description.
        /// </summary>
        /// <param name="services">A service provider that can resolve the
        /// dependencies for the task. The description needs this information
        /// as it cannot know how potential dependencies of a task can be
        /// resolved otherwise.</param>
        /// <returns>The task that has been restored from this description.
        /// </returns>
        /// <exception cref="ArgumentNullException">If
        /// <paramref name="services"/> is <see langword="null"/>.</exception>
        /// <exception cref="InvalidOperationException">If the
        /// <see cref="Task"/> does not designate a valid task type.</exception>
        ITask ToTask(IServiceProvider services);
    }
}
