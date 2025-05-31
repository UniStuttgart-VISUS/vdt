// <copyright file="FromEnvironmentAttribute.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;


namespace Visus.DeploymentToolkit.Extensions {

    /// <summary>
    /// Annotates a property of a class as being retrieved from an environment
    /// variable.
    /// </summary>
    /// <param name="variables">The name of the environment variables to check,
    /// sorted in descending preference, i.e. the first variable that was found
    /// non-empty will be set, otherwise the subsequent ones are checked.
    /// </param>
    [AttributeUsage(AttributeTargets.Property,
        AllowMultiple = true,
        Inherited = false)]
    public sealed class FromEnvironmentAttribute(params string[] variables)
            : Attribute {

        /// <summary>
        /// Gets or sets whether the environment variable should be expanded on
        /// retrieval.
        /// </summary>
        /// <remarks>
        /// This property defaults to <see langword="true"/>.
        /// </remarks>
        public bool Expand { get; set; } = true;

        /// <summary>
        /// Gets the name of the environment variables in the state that the
        /// annotated property should be retrieved from. If empty, the name of
        /// the source property should be used. Otherwise, the first property
        /// that was found should be used.
        /// </summary>
        public string[] Variables { get; } = variables;
    }
}
