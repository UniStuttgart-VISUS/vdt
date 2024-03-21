// <copyright file="CopyFlags.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// Controls the behaviour of <see cref="ICopy"/>.
    /// </summary>
    [Flags]
    public enum CopyFlags {

        /// <summary>
        /// No special behaviour.
        /// </summary>
        None = 0x00000000,

        /// <summary>
        /// Overwrites any existing file at the destination location.
        /// </summary>
        Overwrite = 0x00000001,

        /// <summary>
        /// Recursively copies the whole directory structure from the source
        /// to the destination.
        /// </summary>
        Recursive = 0x00000002
    }
}
