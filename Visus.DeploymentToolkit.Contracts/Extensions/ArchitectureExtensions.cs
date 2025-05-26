// <copyright file="ArchitectureExtensions.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Runtime.InteropServices;
using Visus.DeploymentToolkit.Properties;


namespace Visus.DeploymentToolkit.Extensions {

    /// <summary>
    /// Extensions for <see cref="Architecture"/>.
    /// </summary>
    public static class ArchitectureExtensions {

        /// <summary>
        /// Gets the architecture string used in the WinPE paths, which is
        /// derived from <see cref="Architecture"/>.
        /// </summary>
        /// <param name="that">The architecture to get the folder name
        /// for.</param>
        /// <returns>The name of teh subfolder for the specified architecture.
        /// </returns>
        /// <exception cref="ArgumentException">The architecture is not
        /// supported by the WAIK.</exception>
        public static string GetFolder(this Architecture that)
            => that switch {
                Architecture.X64 => "amd64",
                Architecture.X86 => "x86",
                Architecture.Arm => "arm",
                Architecture.Arm64 => "arm64",
                _ => throw new ArgumentException(string.Format(
                    Errors.UnsupportedWaikArchitecture, that))
            };
    }
}
