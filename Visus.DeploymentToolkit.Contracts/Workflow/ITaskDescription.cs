// <copyright file="ITaskDescription.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Collections.Generic;


namespace Visus.DeploymentToolkit.Workflow {

    /// <summary>
    /// The description of an <see cref="Tasks.ITask"/> that needs to be
    /// instantiated to create an actual task sequence from a description.
    /// </summary>
    public interface ITaskDescription {

        /// <summary>
        /// Gets the fully qualified type name of the task to be instantiated.
        /// </summary>
        string Task { get; }

        /// <summary>
        /// Gets parameters that are to be assigned to the properties of the task
        /// when it is instantiated.
        /// </summary>
        IDictionary<string, object?> Parameters { get; }
    }
}
