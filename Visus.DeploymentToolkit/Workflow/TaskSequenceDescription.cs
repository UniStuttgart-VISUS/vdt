// <copyright file="TaskSequenceDescription.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 - 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Visus.DeploymentToolkit.Properties;


namespace Visus.DeploymentToolkit.Workflow {

    /// <summary>
    /// Represents the JSON description of a task sequence, which is the stuff
    /// that a user would author to configure the installation process.
    /// </summary>
    internal sealed class TaskSequenceDescription : ITaskSequenceDescription {

        #region Factory methods
        /// <summary>
        /// Parse a task sequence description from the given JSON file.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ValueTask<TaskSequenceDescription?> ParseAsync(
                string path) {
            using var file = File.OpenRead(path);
            return JsonSerializer.DeserializeAsync<TaskSequenceDescription>(
                file);
        }
        #endregion

        #region Public constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        public TaskSequenceDescription() {
            ID = Guid.NewGuid().ToString("N");
            Name = string.Format(Resources.TaskSequenceName, ID);
        }
        #endregion

        #region Public properties
        /// <inheritdoc />
        public string? Description { get; set; }

        /// <inheritdoc />
        [Required]
        public string ID { get; set; }

        /// <inheritdoc />
        [Required]
        public string Name { get; set; }

        /// <inheritdoc />
        public Phase Phase { get; set; } = Phase.Installation;

        /// <summary>
        /// Gets or sets the steps in the task sequence.
        /// </summary>
        public Dictionary<Phase, TaskDescription[]> Steps { get; set; } = new();
        #endregion

        #region Public methods
        /// <summary>
        /// Saves the description to the file at the given location.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Task SaveAsync(string path) {
            using var file = File.OpenWrite(path);
            return JsonSerializer.SerializeAsync(file, this,
                new JsonSerializerOptions() {
                    AllowTrailingCommas = false,
                    WriteIndented = true
                });
        }
        #endregion
    }
}
