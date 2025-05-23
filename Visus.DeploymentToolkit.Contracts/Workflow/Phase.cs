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
        /// The phase is unknown.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Servicing of the pre-installed environment that the computers will
        /// boot into from TFTP.
        /// </summary>
        /// <remarks>
        /// This phase runs on a full Windows installation with the WAIK
        /// installed, from which it gets a WinPE template that is customised
        /// by the task sequence.
        /// </remarks>
        PreinstalledEnvironment = 100,

        /// <summary>
        /// We are running in the boostrapper preparing the deployment agent.
        /// </summary>
        /// <remarks>
        /// This phase uses the stuff from the bootstrapping library to mount
        /// the deployment share, get the actual deployment agent and start
        /// other task sequences using the agent.
        /// </remarks>
        Bootstrapping = 200,

        /// <summary>
        /// The installation phase which is running from the preinstalled
        /// environment that has been prepared in the
        /// <see cref="PreinstalledEnvironment"/> phase and loaded from TFTP to
        /// the computer being installed.
        /// </summary>
        Installation = 300,

        /// <summary>
        /// The first boot into the actual system that has been deployed in the
        /// <see cref="Installation"/> phase.
        /// </summary>
        PostInstallation = 400,

        /// <summary>
        /// This phase runs on a full Windows installation that represents the
        /// gold image to be captured. It performs the generalisation of the
        /// image using sysprep.
        /// </summary>
        PrepareImage = 10000,

        /// <summary>
        /// This phase runs in WinPE like <see cref="Installation"/>, but it
        /// does not deploy an image, but captures the system disk.
        /// </summary>
        CaptureImage = 10100
    }
}
