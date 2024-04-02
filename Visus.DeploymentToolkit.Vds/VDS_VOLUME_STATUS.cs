// <copyright file="VDS_VOLUME_STATUS.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>


namespace Visus.DeploymentToolkit.Vds {

    /// <summary>
    /// 
    /// </summary>
    public enum VDS_VOLUME_STATUS {
        UNKNOWN = 0,
        ONLINE = 1,
        NO_MEDIA = 3,
        FAILED = 5,
        OFFLINE = 4
    }
}
