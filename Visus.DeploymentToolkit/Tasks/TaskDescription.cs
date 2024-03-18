// <copyright file="TaskSequenceDescription.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Collections.Generic;


namespace Visus.DeploymentToolkit.Tasks {

    /// <summary>
    /// The in-memory representation of a task description in JSON.
    /// </summary>
    internal sealed class TaskDescription {

        #region Public properties
        /// <summary>
        /// Gets or sets the properties to be configured on the task.
        /// </summary>
        public Dictionary<string, object> Properties { get; set; } = new();

        /// <summary>
        /// Gets or sets the type name of the task.
        /// </summary>
        public string Type { get; set; }
        #endregion
    }
}
