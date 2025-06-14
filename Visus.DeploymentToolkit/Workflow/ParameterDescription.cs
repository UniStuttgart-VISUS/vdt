// <copyright file="ParameterDescription.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Visus.DeploymentToolkit.Extensions;
using Visus.DeploymentToolkit.Validation;


namespace Visus.DeploymentToolkit.Workflow {

    /// <summary>
    /// Describes a parameter by reflecting a property.
    /// </summary>
    internal sealed class ParameterDescription : IParameterDescription {

        #region Public constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="property">The property representing the parameter.
        /// </param>
        public ParameterDescription(PropertyInfo property) {
            ArgumentNullException.ThrowIfNull(property);
            this.Name = property.Name;
            this.IsRequired = property.HasCustomAttribute<RequiredAttribute>()
                || property.HasCustomAttribute<FileExistsAttribute>()
                || property.HasCustomAttribute<DirectoryExistsAttribute>();

            // Determine the potential implicit sources of the parameter.
            var sources = new List<IParameterSource>();

            if (property.TryGetCustomAttribute<DefaultValueAttribute>(
                    out var d)) {
                sources.Add(new ParameterSource {
                    Source = d.Value,
                    Type = ParameterSourceType.Default
                });
            }

            foreach (var a in property.GetCustomAttributes<
                    FromStateAttribute>()) {
                foreach (var p in a .Properties) {
                    sources.Add(new ParameterSource {
                        Source = p,
                        Type = ParameterSourceType.State
                    });
                }
            }

            foreach (var a in property.GetCustomAttributes<
                    FromEnvironmentAttribute>()) {
                foreach (var v in a.Variables) {
                    sources.Add(new ParameterSource {
                        Source = v,
                        Type = ParameterSourceType.Environment
                    });
                }
            }

            this.Sources = sources;
        }

        #endregion

        #region Public properties
        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        public bool IsRequired { get; }

        /// <inheritdoc />
        public IEnumerable<IParameterSource> Sources { get; }
        #endregion

        #region Nested class ParameterSource
        /// <summary>
        /// Implements the <see cref="IParameterSource"/> interface for
        /// populating <see cref="Sources"/>.
        /// </summary>
        private sealed class ParameterSource : IParameterSource {
            /// <inheritdoc />
            public object? Source { get; init; }

            /// <inheritdoc />
            public ParameterSourceType Type { get; init; }
        }
        #endregion
    }
}
