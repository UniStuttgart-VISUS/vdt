// <copyright file="DismPackageFeatureState.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>


namespace Visus.DeploymentToolkit.Dism {

    /// <summary>
    /// Specifies the state of a package or a feature.
    /// </summary>
    public enum DismPackageFeatureState {

        /// <summary>
        /// The package or feature is not present.
        /// </summary>
        NotPresent = 0,

        /// <summary>
        /// An uninstall process for the package or feature is pending.
        /// Additional processes are pending and must be completed before
        /// the package or feature is successfully uninstalled.
        /// </summary>
        UninstallPending = 1,

        /// <summary>
        /// The package or feature is staged.
        /// </summary>
        Staged = 2,

        /// <summary>
        /// Metadata about the package or feature has been added to the system,
        /// but the package or feature is not present.
        /// </summary>
        Removed = 3,

        /// <summary>
        /// The package or feature is installed.
        /// </summary>
        Installed = 4,

        /// <summary>
        /// The install process for the package or feature is pending.
        /// Additional processes are pending and must be completed before the
        /// package or feature is successfully installed.
        /// </summary>
        InstallPending = 5,

        /// <summary>
        /// The package or feature has been superseded by a more recent package
        /// or feature.
        /// </summary>
        Superseded = 6,

        /// <summary>
        /// The package or feature is partially installed. Some parts of the
        /// package or feature have not been installed.
        /// </summary>
        PartiallyInstalled = 7
    }
}
