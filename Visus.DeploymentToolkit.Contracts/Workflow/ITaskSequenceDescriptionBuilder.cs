// <copyright file="ITaskSequenceDescriptionBuilder.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Collections.Generic;
using Visus.DeploymentToolkit.Tasks;


namespace Visus.DeploymentToolkit.Workflow {

    /// <summary>
    /// Defines the interface of a builder that can be used to create
    /// <see cref="ITaskSequenceDescription"/>s programmatically.
    /// </summary>
    public interface ITaskSequenceDescriptionBuilder {

        /// <summary>
        /// Appends the <paramref name="task"/> with the specified
        /// fully-qualified type name to the sequence and optionally sets the
        /// given <paramref name="parameters"/>.
        /// </summary>
        /// <param name="task"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        ITaskSequenceDescriptionBuilder Add(string task,
            IDictionary<string, object?>? parameters);

        /// <summary>
        /// Appends the given <paramref name="task"/> to the sequence.
        /// </summary>
        /// <param name="task"></param>
        /// <returns></returns>
        ITaskSequenceDescriptionBuilder Add(ITaskDescription task);

        /// <summary>
        /// Appends the given <paramref name="task"/> to the list of tasks in
        /// the sequence.
        /// </summary>
        /// <param name="task">The task to be added.</param>
        /// <returns><c>this</c>.</returns>
        /// <exception cref="System.ArgumentNullException">If
        /// <paramref name="task"/> is <c>null</c>.</exception>
        ITaskSequenceDescriptionBuilder Add(ITask task) {
            _ = task ?? throw new ArgumentNullException(nameof(task));
            return this.Add(task.GetType().FullName!, task.GetParameters());
        }

        /// <summary>
        /// Appends a task of type <typeparamref name="TTask"/> to the list of
        /// tasks in the sequence.
        /// </summary>
        /// <typeparam name="TTask"></typeparam>
        /// <param name="parameters"></param>
        /// <returns></returns>
        ITaskSequenceDescriptionBuilder Add<TTask>(
                IDictionary<string, object?>? parameters)
                where TTask : ITask
            => this.Add(typeof(TTask).FullName!, parameters);

        /// <summary>
        /// Sets the phase the task sequence can run in.
        /// </summary>
        /// <param name="phase">The phase the task sequence can run in.</param>
        /// <returns><c>this</c>.</returns>
        /// <exception cref="InvalidOperationException">If the phase has already
        /// been set.</exception>
        ITaskSequenceDescriptionBuilder ForPhase(Phase phase);

        ///// <summary>
        ///// Creates a description for the given <see cref="ITaskSequence"/>.
        ///// </summary>
        ///// <param name="sequence">The task sequence to build the description
        ///// for.</param>
        ///// <returns></returns>
        ///// <exception cref="ArgumentNullException">If the
        ///// <paramref name="sequence"/> is <c>null</c>.</exception>
        ///// <exception cref="InvalidOperationException">If the phase has already
        ///// been set.</exception>
        //ITaskSequenceDescriptionBuilder FromTaskSequence(ITaskSequence sequence);

        /// <summary>
        /// Builds the task sequence from the current state of the builder.
        /// </summary>
        /// <returns>The description of a task sequence.</returns>
        ITaskSequenceDescription Build();

        /// <summary>
        /// Intersts the <paramref name="task"/> with the specified
        /// fully-qualified type name to the sequence and optionally sets the
        /// given <paramref name="parameters"/> at the specified position.
        /// </summary>
        /// <param name="index">The position at which the task is inserted into
        /// the sequence.</param>
        /// <param name="task"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        ITaskSequenceDescriptionBuilder Insert(int index,
            string task,
            IDictionary<string, object?>? parameters);

        /// <summary>
        /// Inserts the the given <paramref name="task"/> at the apecified
        /// position in the task sequence
        /// </summary>
        /// <param name="index">The position at which the task is inserted into
        /// the sequence.</param>
        /// <param name="task"></param>
        /// <returns></returns>
        ITaskSequenceDescriptionBuilder Insert(int index,
            ITaskDescription task);

        /// <summary>
        /// Inserts the given <paramref name="task"/> at the specified position
        /// in the task sequence.
        /// </summary>
        /// <param name="index">The position at which the task is inserted into
        /// the sequence.</param>
        /// <param name="task">The task to be added.</param>
        /// <returns><c>this</c>.</returns>
        /// <exception cref="System.ArgumentNullException">If
        /// <paramref name="task"/> is <c>null</c>.</exception>
        ITaskSequenceDescriptionBuilder Insert(int index, ITask task) {
            _ = task ?? throw new ArgumentNullException(nameof(task));
            return this.Insert(index,
                task.GetType().FullName!,
                task.GetParameters());
        }

        /// <summary>
        /// Inserts a task of type <typeparamref name="TTask"/> at the specified
        /// position in the task sequence and optionally assigns the specified
        /// <paramref name="parameters"/>.
        /// </summary>
        /// <typeparam name="TTask"></typeparam>
        /// <param name="index">The position at which the task is inserted into
        /// the sequence.</param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        ITaskSequenceDescriptionBuilder Insert<TTask>(int index,
                IDictionary<string, object?>? parameters)
                where TTask : ITask
            => this.Insert(index, typeof(TTask).FullName!, parameters);
    }
}
