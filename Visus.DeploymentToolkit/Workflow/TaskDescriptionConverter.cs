// <copyright file="TaskDescriptionConverter.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Visus.DeploymentToolkit.Properties;


namespace Visus.DeploymentToolkit.Workflow {

    /// <summary>
    /// Converts a <see cref="ITaskDescription" /> from and to JSON by using
    /// our <see cref="TaskDescription" /> class as an intermediate.
    /// </summary>
    internal sealed class TaskDescriptionConverter
            : JsonConverter<ITaskDescription> {

        /// <inheritdoc />
        public override ITaskDescription? Read(
                ref Utf8JsonReader reader,
                Type typeToConvert,
                JsonSerializerOptions options) {
            using var doc = JsonDocument.ParseValue(ref reader);
            var retval = new TaskDescription() {
                Parameters = new Dictionary<string, object?>()
            };

            // Restore the type of the task.
            var task = doc.RootElement.GetProperty(
                nameof(TaskDescription.Task));
            retval.Task = task.GetString()
                ?? throw new JsonException(Errors.MissingTaskType);

            // Restore the configuration.
            if (doc.RootElement.TryGetProperty(
                    nameof(TaskDescription.Parameters),
                    out var parameters)) {
                foreach (var p in TaskDescription.GetParameters(retval.Type)) {
                    if (parameters.TryGetProperty(p.Name, out var value)) {
                        var v = JsonSerializer.Deserialize(value,
                            p.PropertyType);
                        retval.Parameters[p.Name] = v;
                    }
                }
            }

            return retval;
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer,
                ITaskDescription value,
                JsonSerializerOptions options) {
            Debug.Assert(writer != null);
            Debug.Assert(value != null);
            Debug.Assert(options != null);
            JsonSerializer.Serialize(writer, value, value.GetType(), options);
        }
    }
}
