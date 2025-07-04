// <copyright file="CustomisationDescriptionConverter.cs" company="Visualisierungsinstitut der Universität Stuttgart">
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
using Visus.DeploymentToolkit.Unattend;


namespace Visus.DeploymentToolkit.Workflow {

    /// <summary>
    /// Converts a <see cref="ICustomisation" /> from and to JSON by using
    /// our <see cref="CustomisationDescription" /> class as an intermediate.
    /// </summary>
    internal sealed class CustomisationDescriptionConverter
            : JsonConverter<ICustomisation> {

        /// <inheritdoc />
        public override ICustomisation? Read(
                ref Utf8JsonReader reader,
                Type typeToConvert,
                JsonSerializerOptions options) {
            using var doc = JsonDocument.ParseValue(ref reader);

            // Restore the type of the customisation.
            var typeProperty = doc.RootElement.GetProperty(
                nameof(CustomisationDescription.Type));
            var type = typeProperty.GetString()
                ?? throw new JsonException(Errors.MissingTaskType);

            //// Restore the configuration.
            //if (doc.RootElement.TryGetProperty(
            //        nameof(TaskDescription.Parameters),
            //        out var parameters)) {
            //    foreach (var p in TaskDescription.GetParameters(retval.Type)) {
            //        if (parameters.TryGetProperty(p.Name, out var value)) {
            //            var v = JsonSerializer.Deserialize(value,
            //                p.PropertyType);
            //            retval.Parameters[p.Name] = v;
            //        }
            //    }
            //}

            //return retval;
            throw new NotImplementedException("TODO");
        }

        /// <inheritdoc />
        public override void Write(Utf8JsonWriter writer,
                ICustomisation value,
                JsonSerializerOptions options) {
            Debug.Assert(writer != null);
            Debug.Assert(value != null);
            Debug.Assert(options != null);
            throw new NotImplementedException("TODO");
            JsonSerializer.Serialize(writer, value, value.GetType(), options);
        }
    }
}
