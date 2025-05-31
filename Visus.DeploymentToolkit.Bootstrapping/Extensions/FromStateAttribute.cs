// <copyright file="FromStateAttribute.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 - 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;


namespace Visus.DeploymentToolkit.Extensions {

    /// <summary>
    /// Annotates a property of a class as being retrieved from
    /// <see cref="Visus.DeploymentToolkit.Services.IState"/>.
    /// </summary>
    /// <param name="properties">The name of the properties in the state, sorted
    /// in descending preference, i.e. the first state that was found non-empty
    /// will be set, otherwise the subsequent ones are checked.</param>
    [AttributeUsage(AttributeTargets.Property,
        AllowMultiple = true,
        Inherited = false)]
    public sealed class FromStateAttribute(params string[] properties)
            : Attribute {

        /// <summary>
        /// Gets the name of the properties in the state that the annotated
        /// property should be retrieved from. If empty, the name of the source
        /// property should be used. Otherwise, the first property that was
        /// found should be used.
        /// </summary>
        public string[] Properties { get; } = properties;
    }
}
