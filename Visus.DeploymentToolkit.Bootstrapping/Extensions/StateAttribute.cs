﻿// <copyright file="StateAttribute.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;


namespace Visus.DeploymentToolkit.Extensions {

    /// <summary>
    /// Annotates a property of a class as mapping to a member of
    /// <see cref="Visus.DeploymentToolkit.Services.IState"/>.
    /// </summary>
    /// <param name="property">The name of the property in the state.</param>
    [AttributeUsage(AttributeTargets.Property,
        AllowMultiple = true,
        Inherited = false)]
    public sealed class StateAttribute(string? property = null) : Attribute {

        /// <summary>
        /// Gets the name of the property in the state that the annotated
        /// property should be mapped to. If <c>null</c>, the name of the
        /// source property should be used.
        /// </summary>
        public string? Property { get; } = property;
    }
}
