// <copyright file="DismImageBootable.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>


namespace Visus.DeploymentToolkit.Dism {

    /// <summary>
    /// Indicates whether an image is a bootable image type.
    /// </summary>
    public enum DismImageBootable {

        /// <summary>
        /// The image is bootable.
        /// </summary>
        Yes = 0,

        /// <summary>
        /// The image is not bootable.
        /// </summary>
        No = 1,

        /// <summary>
        /// The image type is unknown.
        /// </summary>
        Unknown = 2
    }
}
