// <copyright file="ITaskSequenceBuilder.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>


namespace Visus.DeploymentToolkit.Contracts {

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

        ITaskSequenceBuilder Insert(Phase phase, int index, ITask task);
    }
}
