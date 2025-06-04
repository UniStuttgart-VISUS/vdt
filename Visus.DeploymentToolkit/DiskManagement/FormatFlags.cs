// <copyright file="FormatFlags.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;


namespace Visus.DeploymentToolkit.DiskManagement {

    /// <summary>
    /// Allows for customising the behaviour of the
    /// <see cref="IAdvancedDisk.FormatPartitionAsync"/>.
    /// </summary>
    [Flags]
    internal enum FormatFlags : uint {

        /// <summary>
        /// Perform a standard format.
        /// </summary>
        None = 0x00000000,

        /// <summary>
        /// Force the partition to be formatted.
        /// </summary>
        Force = 0x00000001,

        /// <summary>
        /// Perform a quick format.
        /// </summary>
        Quick = 0x00000002,

        /// <summary>
        /// Enable compression on the partition.
        /// </summary>
        EnableCompression = 0x00000004,
    }
}
