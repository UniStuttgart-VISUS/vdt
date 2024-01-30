// <copyright file="VDS_PACK_PROP.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>


namespace Visus.DeploymentToolkit.Vds {

    /// <summary>
    /// Defines the set of valid flags for a pack object.
    /// </summary>
    [Flags]
    public enum VDS_PACK_FLAG : uint {
        FOREIGN = 0x1,
        NOQUORUM = 0x2,
        POLICY = 0x4,
        CORRUPTED = 0x8,
        ONLINE_ERROR = 0x10
    }
}
