// <copyright file="VDS_VOLUME_TYPE.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>


namespace Visus.DeploymentToolkit.Vds {

    /// <summary>
    /// 
    /// </summary>
    public enum VDS_VOLUME_TYPE {
        UNKNOWN = 0,
        SIMPLE = 10,
        SPAN = 11,
        STRIPE = 12,
        MIRROR = 13,
        PARITY = 14
    }
}
