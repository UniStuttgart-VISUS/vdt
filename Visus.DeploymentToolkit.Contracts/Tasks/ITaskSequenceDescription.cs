// <copyright file="ITaskSequenceDescription.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.ComponentModel.DataAnnotations;


namespace Visus.DeploymentToolkit.Tasks {

    /// <summary>
    /// Describes a task sequence that can be retrieved from a
    /// <see cref="Services.ITaskSequenceStore"/>.
    /// </summary>
    public interface ITaskSequenceDescription {

        /// <summary>
        /// Gets or sets a description of the task sequence.
        /// </summary>
        string? Description { get; set; }

        /// <summary>
        /// Gets or sets the unique ID of the task sequence.
        /// </summary>
        [Required]
        string ID { get; set; }

        /// <summary>
        /// Gets or sets the name of the task sequence.
        /// </summary>
        [Required]
        string Name { get; set; }
    }
}
