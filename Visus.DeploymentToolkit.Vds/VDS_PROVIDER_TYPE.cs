// <copyright file="VDS_PROVIDER_TYPE.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>


namespace Visus.DeploymentToolkit.Vds {

    /// <summary>
    /// Defines the set of valid types for a provider.
    /// </summary>
    public enum VDS_PROVIDER_TYPE {

        /// <summary>
        /// The provider type is unknown.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// The provider is a software provider.
        /// </summary>
        Software = 1,

        /// <summary>
        /// The provider is a hardware provider.
        /// </summary>
        Hardware = 2,

        /// <summary>
        /// The provider is a virtual disk provider.
        /// </summary>
        VirtualDisk = 3,

    }
}
