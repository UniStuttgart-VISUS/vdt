// <copyright file="ITaskSequence.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Collections.Generic;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Tasks;


namespace Visus.DeploymentToolkit.Workflow {

    /// <summary>
    /// Defines the interface of a task sequence, which describes how the
    /// installation should be performed.
    /// </summary>
    public interface ITaskSequence {

        /// <summary>
        /// Runs the <see cref="ITask"/>s for the given phase.
        /// </summary>
        /// <param name="phase">The phase to run.</param>
        /// <returns>A <see cref="Task"/> to wait for the tasks in the phase to
        /// complete.</returns>
        Task ExecuteAsync(Phase phase);

        /// <summary>
        /// Gets the <see cref="ITask"/>s to be performed in the given phase.
        /// </summary>
        /// <param name="phase">The phase to retrieve the tasks for.</param>
        /// <returns>The list of tasks for the requested phase.</returns>
        IEnumerable<ITask> this[Phase phase] {
            get;
        }
    }
}
