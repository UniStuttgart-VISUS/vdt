// <copyright file="CustomisationDescriptionConverter.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Visus.DeploymentToolkit.Extensions;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Unattend;


namespace Visus.DeploymentToolkit.Workflow {

    /// <summary>
    /// Converts a <see cref="CustomisationDescription" /> from and to JSON.
    /// </summary>
    internal sealed class CustomisationDescriptionConverter
            : JsonConverter<CustomisationDescription> {

        /// <inheritdoc />
        public override CustomisationDescription? Read(
                ref Utf8JsonReader reader,
                Type typeToConvert,
                JsonSerializerOptions options) {
            using var doc = JsonDocument.ParseValue(ref reader);
            var retval = new CustomisationDescription() {
                Parameters = new Dictionary<string, object?>()
            };

            // Restore the type of the customisation.
            var customisation = doc.RootElement.GetProperty(
                nameof(CustomisationDescription.Customisation));
            retval.Customisation = customisation.GetString()
                ?? throw new JsonException(Errors.MissingCustomisationType);
            var type = Type.GetType(retval.Customisation)
                ?? throw new JsonException(string.Format(
                    Errors.UnknownCustomisationType, retval.Customisation));

            // Restore the parameters.
            if (doc.RootElement.TryGetProperty(
                    nameof(CustomisationDescription.Parameters),
                    out var parameters)) {
                foreach (var p in type.GetPublicReadWriteInstanceProperties()) {
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
                CustomisationDescription value,
                JsonSerializerOptions options) {
            Debug.Assert(writer != null);
            Debug.Assert(value != null);
            Debug.Assert(options != null);
            JsonSerializer.Serialize(writer, value, value.GetType(), options);
        }
    }
}
