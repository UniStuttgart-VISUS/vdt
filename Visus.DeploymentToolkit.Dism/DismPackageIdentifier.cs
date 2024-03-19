// <copyright file="DismPackageIdentifier.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>


namespace Visus.DeploymentToolkit.Dism {

    /// <summary>
    /// Specifies whether a package is identified by name or by file path.
    /// </summary>
    public enum DismPackageIdentifier {

        /// <summary>
        /// No package is specified.
        /// </summary>
        None = 0,

        /// <summary>
        /// The package is identified by its name.
        /// </summary>
        Name = 1,

        /// <summary>
        /// The package is specified by its path.
        /// </summary>
        Path = 2
    }
}
