// <copyright file="ITaskSequenceDescription.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace Visus.DeploymentToolkit.Workflow {

    /// <summary>
    /// Describes a task sequence that can be retrieved from a
    /// <see cref="ITaskSequenceStore"/>.
    /// </summary>
    public interface ITaskSequenceDescription {

        /// <summary>
        /// Gets a description of the task sequence.
        /// </summary>
        string? Description { get; }

        /// <summary>
        /// Gets the unique ID of the task sequence.
        /// </summary>
        [Required]
        string ID { get; }

        /// <summary>
        /// Gets the name of the task sequence.
        /// </summary>
        [Required]
        string Name { get; }

        /// <summary>
        /// Gets the <see cref="Phase"/> the task sequence can run
        /// in. This typically includes
        /// <see cref="Phase.PreinstalledEnvironment"/> for a task sequence that
        /// creates a WinPE image and <see cref="Phase.Installation"/> for a task
        /// sequence performing a Windows installation.
        /// </summary>
        [Required]
        Phase Phase { get; }

        /// <summary>
        /// Gets the tasks to be executed in the order in which they should be
        /// run.
        /// </summary>
        [Required]
        IEnumerable<ITaskDescription> Tasks { get; }
    }
}
