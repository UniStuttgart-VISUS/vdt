// <copyright file="DiskImageIdentifier.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details
// </copyright>
// <author>Christoph Müller</author>


namespace Visus.DeploymentToolkit.Dism {

    /// <summary>
    /// Specifies whether an image is identified by name or by index.
    /// </summary>
    /// <remarks>
    /// Cf. https://learn.microsoft.com/en-us/windows-hardware/manufacture/desktop/dism/dismimageidentifier-enumeration?view=windows-11
    /// </remarks>
    public enum DiskImageIdentifier {

        /// <summary>
        /// Identify the image by index number.
        /// </summary>
        DismImageIndex = 0,

        /// <summary>
        /// Identify the image by name.
        /// </summary>
        DismImageName = 1
    }
}
