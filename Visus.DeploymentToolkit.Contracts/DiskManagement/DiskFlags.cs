// <copyright file="DiskFlags.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;


namespace Visus.DeploymentToolkit.DiskManagement {

    /// <summary>
    /// A set of Boolean properties that a <see cref="IDisk"/> can have.
    /// </summary>
    [Flags]
    public enum DiskFlags : uint {

        /// <summary>
        /// No flags are set.
        /// </summary>
        None = 0x00000000,

        /// <summary>
        /// The disk is unitialised.
        /// </summary>
        Uninitialised = 0x00000001,

        /// <summary>
        /// The disk is offline.
        /// </summary>
        Offline = 0x00000002,

        /// <summary>
        /// The disk is read-only.
        /// </summary>
        ReadOnly = 0x00000004,

    }
}
