// <copyright file="TaskSequenceDescription.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Workflow;


namespace Visus.DeploymentToolkit.Tasks {

    /// <summary>
    /// Represents the JSON description of a task sequence.
    /// </summary>
    internal sealed class TaskSequenceDescription {

        #region Public constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        public TaskSequenceDescription() {
            this.ID = Guid.NewGuid().ToString("N");
            this.Name = string.Format(Resources.TaskSequenceName, this.ID);
        }
        #endregion

        #region Public properties
        /// <summary>
        /// Gets or sets a description of the task sequence.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the unique ID of the task sequence.
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// Gets or sets the name of the task sequence.
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the steps in the task sequence.
        /// </summary>
        public Dictionary<Phase, TaskDescription[]> Steps { get; set; } = new();
        #endregion
    }
}
