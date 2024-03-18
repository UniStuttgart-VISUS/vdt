// <copyright file="Phase.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>


namespace Visus.DeploymentToolkit.Contracts {

    /// <summary>
    /// Defines the phases of the deployment process.
    /// </summary>
    public enum Phase {

        /// <summary>
        /// The pre-installed environment booted from TFTP.
        /// </summary>
        /// <remarks>
        /// In this phase, the system will be prepared (disks) and the OS files
        /// are copied to the disk.
        /// </remarks>
        PreinstalledEnvironment,

        Installation,

        PostInstallation
    }
}
