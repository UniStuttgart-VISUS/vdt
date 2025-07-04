// <copyright file="ITaskSequence.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Collections.Generic;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Services;
using Visus.DeploymentToolkit.Tasks;


namespace Visus.DeploymentToolkit.Workflow {

    /// <summary>
    /// Defines the interface of a task sequence, which describes how the
    /// installation should be performed.
    /// </summary>
    public interface ITaskSequence : IReadOnlyCollection<ITask> {

        #region Public properties
        /// <summary>
        /// Gets the unique ID of the task sequence.
        /// </summary>
        string ID { get; }

        /// <summary>
        /// Gets the <see cref="Phase"/> the sequence is applicable to.
        /// </summary>
        Phase Phase { get; }
        #endregion

        #region Public methods
        /// <summary>
        /// Runs the <see cref="ITask"/>s for the given phase.
        /// </summary>
        /// <param name="state">The state that allows the task sequence to obtain
        /// critical information, most importantly its own progress.</param>
        /// <returns>A task to wait for the sequence to complete.</returns>
        Task ExecuteAsync(IState state);
        #endregion
    }
}
