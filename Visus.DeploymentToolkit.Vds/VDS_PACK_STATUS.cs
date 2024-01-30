// <copyright file="VDS_PACK_STATUS.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>


namespace Visus.DeploymentToolkit.Vds {

    /// <summary>
    /// Defines the set of object status values for a pack.
    /// </summary>
    public enum VDS_PACK_STATUS : uint {
        UNKNOWN = 0,
        ONLINE = 1,
        OFFLINE = 4
    }
}
