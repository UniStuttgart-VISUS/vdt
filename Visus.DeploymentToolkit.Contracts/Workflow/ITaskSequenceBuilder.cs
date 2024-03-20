// <copyright file="ITaskSequenceBuilder.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Visus.DeploymentToolkit.Tasks;


namespace Visus.DeploymentToolkit.Workflow {

    /// <summary>
    /// Defines the interface of a builder that can be used to create
    /// <see cref="ITaskSequence"/>s programmatically.
    /// </summary>
    public interface ITaskSequenceBuilder {

        /// <summary>
        /// Appends the given <paramref name="task"/> to the list of tasks for
        /// the given <paramref name="phase"/>.
        /// </summary>
        /// <param name="phase"></param>
        /// <param name="task"></param>
        /// <returns></returns>
        ITaskSequenceBuilder Add(Phase phase, ITask task);

        /// <summary>
        /// Builds the task sequence from the current state of the builder.
        /// </summary>
        /// <returns>A task sequence.</returns>
        ITaskSequence Build();

        /// <summary>
        /// Insert the given <paramref name="task"/> at the given position in
        /// the workflow of the specified <paramref name="phase"/>.
        /// </summary>
        /// <param name="phase"></param>
        /// <param name="index"></param>
        /// <param name="task"></param>
        /// <returns></returns>
        ITaskSequenceBuilder Insert(Phase phase, int index, ITask task);
    }
}
