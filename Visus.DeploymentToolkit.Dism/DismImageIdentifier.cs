// <copyright file="DismImageIdentifier.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>


namespace Visus.DeploymentToolkit.Dism {

    /// <summary>
    /// Specifies whether an image is identified by name or by index number.
    /// </summary>
    public enum DismImageIdentifier {

        /// <summary>
        /// Identify the image by index number.
        /// </summary>
        Index = 0,

        /// <summary>
        /// Identify the image by name.
        /// </summary>
        Name = 1
    }
}
