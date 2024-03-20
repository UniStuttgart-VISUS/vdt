// <copyright file="Phase.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>


namespace Visus.DeploymentToolkit.Workflow {

    /// <summary>
    /// Defines the phases of the deployment process.
    /// </summary>
    public enum Phase {

        /// <summary>
        /// Servicing of the pre-installed environment that the computers will
        /// boot into from TFTP.
        /// </summary>
        PreinstalledEnvironment,

        /// <summary>
        /// The installation phase which is running from the preinstalled
        /// environment that has been prepared in the
        /// <see cref="PreinstalledEnvironment"/> phase and loaded from TFTP to
        /// the computer being installed.
        /// </summary>
        Installation,

        /// <summary>
        /// The first boot into the actual system that has been deployed in the
        /// <see cref="Installation"/> phase.
        /// </summary>
        PostInstallation
    }
}
