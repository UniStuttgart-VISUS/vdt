// <copyright file="CultureAttribute.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Globalization;


namespace Visus.DeploymentToolkit.SystemInformation {

    /// <summary>
    /// Annotates properties or fields with a specific culture.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field,
        AllowMultiple = false,
        Inherited = true)]
    internal sealed class CultureAttribute(string name) : Attribute {

        /// <summary>
        /// Gets the annotated culture.
        /// </summary>
        public CultureInfo Culture { get; } = new(name);
    }
}
