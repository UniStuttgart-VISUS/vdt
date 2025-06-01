// <copyright file="FriendlyNameAttribute.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;


namespace Visus.DeploymentToolkit.Bcd {

    /// <summary>
    /// Annotates an enumerated field with the GUID used for it in the BCD
    /// store.
    /// </summary>
    /// <param name="id">The GUID used in the BCD store for the annotated
    /// element.</param>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field,
        AllowMultiple = false,
        Inherited = true)]
    internal sealed class BcdIDAttribute(Guid id) : Attribute {

        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="id">The string representation of the GUID.</param>
        public BcdIDAttribute(string id) : this(Guid.Parse(id)) { }

        /// <summary>
        /// Gets the GUID used for the element in the BCD store.
        /// </summary>
        public Guid ID { get; } = id;}
}
