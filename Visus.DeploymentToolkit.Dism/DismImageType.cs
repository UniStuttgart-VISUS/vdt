// <copyright file="DismImageType.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>


namespace Visus.DeploymentToolkit.Dism {

    /// <summary>
    /// Specifies the file type of the Windows image container.
    /// </summary>
    public enum DismImageType {

        /// <summary>
        /// The file type is unsupported. The image must be in a .wim, .vhd, or
        /// .vhdx file.
        /// </summary>
        Unsupported = -1,

        /// <summary>
        /// The image is in a .wim file.
        /// </summary>
        Wim = 0,

        /// <summary>
        /// The image is in a .vhd or .vhdx file.
        /// </summary>
        Vhd = 1
    }
}
