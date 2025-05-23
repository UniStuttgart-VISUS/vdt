// <copyright file="NonExistingFileAttribute.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.ComponentModel.DataAnnotations;
using System.IO;


namespace Visus.DeploymentToolkit.Validation {

    /// <summary>
    /// Validates a property to be a string that does not refer to an existing
    /// file.
    /// </summary>
    /// <remarks>
    /// Note that the path may refer to an existing directory, it only does not
    /// refer to an existing file if the validation succeeds.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property
        | AttributeTargets.Field
        | AttributeTargets.Parameter,
        AllowMultiple = false)]
    public sealed class NonExistingFileAttribute : ValidationAttribute {

        /// <inheritdoc />
        public override bool IsValid(object? value)
            => value is string path && !File.Exists(path);
    }
}
