// <copyright file="TaskDescriptionConverter.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;


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
            var doc = JsonDocument.ParseValue(ref reader);
            return JsonSerializer.Deserialize<TaskDescription>(doc, options);
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
