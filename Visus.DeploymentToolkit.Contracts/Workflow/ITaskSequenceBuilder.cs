// <copyright file="ITaskSequenceBuilder.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 - 2025 Visualisierungsinstitut der Universität Stuttgart.
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
        /// <param name="task">The task to be added.</param>
        /// <returns><c>this</c>.</returns>
        /// <exception cref="System.ArgumentNullException">If
        /// <paramref name="task"/> is <c>null</c>.</exception>
        ITaskSequenceBuilder Add(ITask task);

        /// <summary>
        /// Instantiates a <typeparamref name="TTask"/> from
        /// <paramref name="services"/>, possibly configures it and adds it
        /// to <paramref name="that"/>.
        /// </summary>
        /// <typeparam name="TTask">The type of the task to be added, which must
        /// have been registered with the <see cref="IServiceProvider"/> used by
        /// the application.</typeparam>
        /// <param name="configure">An optional configuration callback that is
        /// being executed for the newly created task before it is being added
        /// to the task sequence.</param>
        /// <returns><c>this</c>.</returns>
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
        /// <param name="index">The position at which the task is inserted into
        /// the sequence.</param>
        /// <param name="task">The task to be added.</param>
        /// <returns><c>this</c>.</returns>
        /// <exception cref="System.ArgumentNullException">If
        /// <paramref name="task"/> is <c>null</c>.</exception>
        ITaskSequenceBuilder Insert(int index, ITask task);

        /// <summary>
        /// Insert the given <paramref name="task"/> at the given position in
        /// the workflow, possibly after configuring it.
        /// </summary>
        /// <typeparam name="TTask">The type of the task to be added, which must
        /// have been registered with the <see cref="IServiceProvider"/> used by
        /// the application.</typeparam>
        /// <param name="index">The position at which the task is inserted into
        /// the sequence.</param>
        /// <param name="configure">An optional configuration callback that is
        /// being executed for the newly created task before it is being added
        /// to the task sequence.</param>
        /// <returns><c>this</c>.</returns>
        ITaskSequenceBuilder Insert<TTask>(int index,
            Action<TTask>? configure = null)
            where TTask : ITask;
    }
}
