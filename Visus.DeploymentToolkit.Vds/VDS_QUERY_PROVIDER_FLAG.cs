// <copyright file="VDS_QUERY_PROVIDER_FLAG.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;


namespace Visus.DeploymentToolkit.Vds {

    /// <summary>
    /// Defines the set of valid flags for provider query operations.
    /// </summary>
    [Flags]
    public enum VDS_QUERY_PROVIDER_FLAG : uint {

        /// <summary>
        /// If set, the operation queries for software providers.
        /// </summary>
        SOFTWARE_PROVIDERS = 0x1,

        /// <summary>
        /// If set, the operation queries for hardware providers.
        /// </summary>
        HARDWARE_PROVIDERS = 0x2,

        /// <summary>
        /// If set, the operation queries for virtual disk providers.
        /// </summary>
        VIRTUALDISK_PROVIDERS = 0x4
    }
}
