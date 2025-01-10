// <copyright file="ITaskSequenceStore.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Collections.Generic;
using Visus.DeploymentToolkit.Tasks;


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// Provides access to the store of task sequences.
    /// </summary>
    public interface ITaskSequenceStore {

        /// <summary>
        /// Retrieves all task sequences from the store.
        /// </summary>
        /// <returns>The task sequences in the store.</returns>
        IEnumerable<ITaskSequenceDescription> GetTaskSequences();
    }
}
