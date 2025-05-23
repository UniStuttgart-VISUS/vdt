// <copyright file="EmptyDirectoryAttribute.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;


namespace Visus.DeploymentToolkit.Validation {

    /// <summary>
    /// Validates a property to be a string that that refers to an existing
    /// directory that is empty.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property
        | AttributeTargets.Field
        | AttributeTargets.Parameter,
        AllowMultiple = false)]
    public sealed class EmptyDirectoryAttribute : ValidationAttribute {

        /// <inheritdoc />
        public override bool IsValid(object? value) => value is string path
            && Directory.Exists(path)
            && !Directory.EnumerateFileSystemEntries(path).Any();
    }
}
