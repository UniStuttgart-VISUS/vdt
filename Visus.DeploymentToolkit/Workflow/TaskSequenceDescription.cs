// <copyright file="TaskSequenceDescription.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 - 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
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
        /// Creates a new task sequence description from an existing task
        /// sequence.
        /// </summary>
        /// <typeparam name="TTaskSequence"></typeparam>
        /// <param name="taskSequence"></param>
        /// <returns></returns>
        public static TaskSequenceDescription FromTaskSequence<TTaskSequence>(
                TTaskSequence taskSequence)
                where TTaskSequence : ITaskSequence {
            ArgumentNullException.ThrowIfNull(taskSequence);
            return new TaskSequenceDescription() {
                Phase = taskSequence.Phase,
                Tasks = taskSequence.Select(t => TaskDescription.FromTask(t))
            };
        }

        /// <summary>
        /// Parse a task sequence description from the given JSON file.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ValueTask<TaskSequenceDescription?> ParseAsync(
                string path) {
            using var file = File.OpenRead(path);
            return JsonSerializer.DeserializeAsync<TaskSequenceDescription>(
                file, JsonOptions);
        }
        #endregion

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

        /// <inheritdoc />
        [Required]
        public IEnumerable<ITaskDescription> Tasks { get; set; } = [ ];
        #endregion

        #region Public methods
        /// <summary>
        /// Saves the description to the file at the given location.
        /// </summary>
        /// <param name="path">The path to the file where the description should
        /// be stored.</param>
        /// <returns>A task for waiting to the serialisation to complete.
        /// </returns>
        public Task SaveAsync(string path) {
            using var file = File.OpenWrite(path);
            return JsonSerializer.SerializeAsync(file, this, JsonOptions);
        }
        #endregion

        #region Private constants
        /// <summary>
        /// The options for the JSON serialiser used to persist task sequences.
        /// </summary>
        private static readonly JsonSerializerOptions JsonOptions = new() {
            AllowTrailingCommas = false,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters = {
                new JsonStringEnumConverter<Architecture>(),
                new JsonStringEnumConverter<Phase>(),
                new TaskDescriptionConverter()
            },
            WriteIndented = true
        };
        #endregion

        #region Private methods
        /// <summary>
        /// Resolves interface types during deserialisation, which is required
        /// because the task sequence description refers to the task
        /// descriptions by their interface type.
        /// </summary>
        /// <returns></returns>
        public static Action<JsonTypeInfo> GetTypeResolvers() {
            return t => {
                if (t.Type == typeof(ITaskDescription)) {
                    t.CreateObject = () => new TaskDescription();
                }
            };
        }
        #endregion
    }
}
