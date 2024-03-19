// <copyright file="DismFullyOfflineInstallable.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>


namespace Visus.DeploymentToolkit.Dism {

    /// <summary>
    /// Specifies whether a package can be installed to an offline image without
    /// booting the image.
    /// </summary>
    public enum DismFullyOfflineInstallable {

        /// <summary>
        /// The package can be installed to an offline image without booting
        /// the image.
        /// </summary>
        Installable = 0,

        /// <summary>
        /// You must boot into the image in order to complete installation of
        /// this package.
        /// </summary>
        NotInstallable = 1,

        /// <summary>
        /// You may have to boot the image in order to complete the installation
        /// of this package.
        /// </summary>
        InstallableUndetermined = 2
    }
}
