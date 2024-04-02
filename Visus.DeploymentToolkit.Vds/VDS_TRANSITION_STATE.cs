// <copyright file="VDS_TRANSITION_STATE.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>


namespace Visus.DeploymentToolkit.Vds {

    /// <summary>
    /// 
    /// </summary>
    public enum VDS_TRANSITION_STATE {
        UNKNOWN = 0,
        STABLE = 1,
        EXTENDING = 2,
        SHRINKING = 3,
        RECONFIGING = 4,
        RESTRIPING = 5
    }
}
