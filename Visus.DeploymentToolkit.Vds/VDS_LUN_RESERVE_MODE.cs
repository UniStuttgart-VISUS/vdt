// <copyright file="VDS_LUN_RESERVE_MODE.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>


namespace Visus.DeploymentToolkit.Vds {

    /// <summary>
    /// This enumeration is reserved for future use.
    /// </summary>
    public enum VDS_LUN_RESERVE_MODE : uint {
        NONE = 0,
        EXCLUSIVE_RW = 1,
        EXCLUSIVE_RO = 2,
        SHARED_RO = 3,
        SHARED_RW = 4
    }
}
