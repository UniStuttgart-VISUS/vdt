// <copyright file="BcdEditNameAttribute.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;


namespace Visus.DeploymentToolkit.Bcd {

    /// <summary>
    /// Annotates an enumerated field with the friendly name used for it in
    /// the bcdedit.exe application.
    /// </summary>
    /// <param name="name">The friendly name used in BCD edit for the annotated
    /// element.</param>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field,
        AllowMultiple = true,
        Inherited = true)]
    internal sealed class BcdEditNameAttribute(string name) : Attribute {

        /// <summary>
        /// Gets or sets the major version for which the name is applicable.
        /// </summary>
        /// <remarks>
        /// The minimum version defaults to 6.0.
        /// </remarks>
        public uint Major { get; set; } = 6;

        /// <summary>
        /// Gets or sets the minor version for which the name is applicable.
        /// </summary>
        /// <remarks>
        /// The minimum version defaults to 6.0.
        /// </remarks>
        public uint Minor { get; set; } = 0;

        /// <summary>
        /// Gets the friendly name used in BCD edit for the annotated element.
        /// </summary>
        public string Name {
            get;
        } = name ?? throw new ArgumentNullException(nameof(name));
    }
}
