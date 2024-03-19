// <copyright file="DismImageHealthState.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>


namespace Visus.DeploymentToolkit.Dism {

    /// <summary>
    /// Specifies whether an image is corrupted.
    /// </summary>
    public enum DismImageHealthState {

        /// <summary>
        /// The image is not corrupted.
        /// </summary>
        Healthy = 0,

        /// <summary>
        /// The image is corrupted but can be repaired.
        /// </summary>
        Repairable = 1,

        /// <summary>
        /// The image is corrupted and cannot be repaired. Discard the image
        /// and start again.
        /// </summary>
        NonRepairable = 2
    }
}
