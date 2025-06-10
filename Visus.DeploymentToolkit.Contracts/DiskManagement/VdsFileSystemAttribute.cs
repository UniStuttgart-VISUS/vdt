// <copyright file="WmiFileSystemAttribute.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 VdsFileSysemAttribute der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;


namespace Visus.DeploymentToolkit.DiskManagement {

    /// <summary>
    /// Annotates a <see cref="FileSystem"/> with the values used by the VDS.
    /// </summary>
    /// <param name="value">The numeric value used by the VDS to represent
    /// the annotated file system.</param>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class VdsFileSystemAttribute(uint value) : Attribute {

        /// <summary>
        /// Gets the value used by the VDS for the annotated file system type.
        /// </summary>
        public uint Value { get; } = value;
    }
}
