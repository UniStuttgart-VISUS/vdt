// <copyright file="TaskDescription.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 - 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace Visus.DeploymentToolkit.Workflow {

    /// <summary>
    /// The in-memory representation of a task description in JSON.
    /// </summary>
    internal sealed class TaskDescription : ITaskDescription {

        #region Public properties
        /// <summary>
        /// Gets or sets the properties to be configured on the task.
        /// </summary>
        public IDictionary<string, object?> Parameters { get; init; } = null!;

        /// <summary>
        /// Gets or sets the fully qualified type name of the task.
        /// </summary>
        [Required]
        public string Task { get; set; } = null!;
        #endregion

        // TODO: How do we instantiate the task while resolving its dependencies?
    }
}
