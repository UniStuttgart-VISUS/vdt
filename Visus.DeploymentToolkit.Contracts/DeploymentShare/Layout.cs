// <copyright file="Layout.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>


namespace Visus.DeploymentToolkit.DeploymentShare {

    /// <summary>
    /// Provides information about the layout of the deployment share, mainly
    /// constants for the folder structure.
    /// </summary>
    public static class Layout {

        /// <summary>
        /// The folder where the binaries of the tools are located.
        /// </summary>
        public const string BinaryPath = "Bin";

        /// <summary>
        /// The folder where the boot files are located.
        /// </summary>
        public const string BootFilePath = "Boot";

        /// <summary>
        /// The folder where the bootstrapper binaries are located that need to
        /// be embedded into the boot image.
        /// </summary>
        public const string BootstrapperPath = "Bootstrapper";

        /// <summary>
        /// The folder where the driver packages are stored, which can be
        /// injected into the images.
        /// </summary>
        public const string DriverPath = "Drivers";

        /// <summary>
        /// The folder where the operating system images are stored.
        /// </summary>
        public const string InstallImagePath = "Image";

        /// <summary>
        /// The folder where the task sequences are stored.
        /// </summary>
        public const string TaskSequencePath = "TaskSequences";
    }
}
