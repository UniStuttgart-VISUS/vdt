// <copyright file="ITaskSequenceStore.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Collections.Generic;
using System.Threading.Tasks;


namespace Visus.DeploymentToolkit.Workflow {

    /// <summary>
    /// Provides access to the store of task sequences.
    /// </summary>
    public interface ITaskSequenceStore {

        /// <summary>
        /// Gets a task sequence from a specific file or looks it up in the store
        /// by its ID.
        /// </summary>
        /// <param name="taskSequence">The path to a task sequence file or the
        /// ID of a task sequence in teh store.</param>
        /// <returns>A description for the specified task sequence,
        /// or <c>null</c> if no matching one was found.</returns>
        /// <exception cref="System.ArgumentNullException">If
        /// <paramref name="taskSequence"/> is <c>null</c>.</exception>
        Task<ITaskSequenceDescription?> GetTaskSequenceAsync(
            string taskSequence);

        /// <summary>
        /// Retrieves all task sequences from the store.
        /// </summary>
        /// <returns>The task sequences in the store.</returns>
        Task<IEnumerable<ITaskSequenceDescription>> GetTaskSequencesAsync();
    }
}
