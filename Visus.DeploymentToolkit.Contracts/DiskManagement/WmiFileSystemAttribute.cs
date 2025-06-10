// <copyright file="WmiFileSystemAttribute.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;


namespace Visus.DeploymentToolkit.DiskManagement {

    /// <summary>
    /// Annotates a <see cref="FileSystem"/> with the values used by the WMI
    /// objects.
    /// </summary>
    /// <param name="value">The numeric value used by WMI to represent the
    /// annotated file system.</param>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class WmiFileSystemAttribute(ushort value) : Attribute {

        /// <summary>
        /// Gets the value used by WMI for the annotated file system type.
        /// </summary>
        public ushort Value { get; } = value;
    }
}
