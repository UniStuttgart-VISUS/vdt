// <copyright file="Phase.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details
// </copyright>
// <author>Christoph Müller</author>


namespace Visus.DeploymentToolkit {

    /// <summary>
    /// Defines the phases of the deployment process.
    /// </summary>
    public enum Phase {

        PreinstalledEnvironment,

        Installation,

        PostInstallation
    }
}
