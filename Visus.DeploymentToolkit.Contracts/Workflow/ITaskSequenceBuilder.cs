// <copyright file="ITaskSequenceBuilder.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using Visus.DeploymentToolkit.Tasks;


namespace Visus.DeploymentToolkit.Workflow {

    /// <summary>
    /// Defines the interface of a builder that can be used to create
    /// <see cref="ITaskSequence"/>s programmatically.
    /// </summary>
    public interface ITaskSequenceBuilder {

        /// <summary>
        /// Appends the given <paramref name="task"/> to the list of tasks in
        /// the sequence.
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        ITaskSequenceBuilder Add(ITask task);

        /// <summary>
        /// Instantiates a <typeparamref name="TTask"/> from
        /// <paramref name="services"/>, possibly configures it and adds it
        /// to <paramref name="that"/>.
        /// </summary>
        /// <typeparam name="TTask"></typeparam>
        /// <param name="configure"></param>
        /// <returns></returns>
        ITaskSequenceBuilder Add<TTask>(Action<TTask>? configure = null)
            where TTask : ITask;

        /// <summary>
        /// Sets the phase the task sequence can run in.
        /// </summary>
        /// <param name="phase">The phase the task sequence can run in.</param>
        /// <returns><c>this</c>.</returns>
        ITaskSequenceBuilder ForPhase(Phase phase);

        /// <summary>
        /// Builds the task sequence from the current state of the builder.
        /// </summary>
        /// <returns>A task sequence.</returns>
        ITaskSequence Build();

        /// <summary>
        /// Insert the given <paramref name="task"/> at the given position in
        /// the workflow.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="task"></param>
        /// <returns></returns>
        ITaskSequenceBuilder Insert(int index, ITask task);

        /// <summary>
        /// Insert the given <paramref name="task"/> at the given position in
        /// the workflow, possibly after configuring it.
        /// </summary>
        /// <typeparam name="TTask"></typeparam>
        /// <param name="index"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        ITaskSequenceBuilder Insert<TTask>(int index,
            Action<TTask>? configure = null)
            where TTask : ITask;
    }
}
