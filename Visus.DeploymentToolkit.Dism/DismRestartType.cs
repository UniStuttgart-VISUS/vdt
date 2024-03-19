// <copyright file="DismRestartType.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>


namespace Visus.DeploymentToolkit.Dism {

    /// <summary>
    /// Specifies whether a restart is required after enabling a feature or
    /// installing a package.
    /// </summary>
    public enum DismRestartType {

        /// <summary>
        /// No restart is required.
        /// </summary>
        No = 0,

        /// <summary>
        /// This package or feature might require a restart.
        /// </summary>
        Possible = 1,

        /// <summary>
        /// This package or feature always requires a restart.
        /// </summary>
        Required = 2
    }
}
