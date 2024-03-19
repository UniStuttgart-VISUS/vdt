// <copyright file="DismMountStatus.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>


namespace Visus.DeploymentToolkit.Dism {

    /// <summary>
    /// Indicates whether a mounted image needs to be remounted.
    /// </summary>
    public enum DismMountStatus {

        /// <summary>
        /// Indicates that the mounted image is mounted and ready for servicing.
        /// </summary>
        OK = 0,

        /// <summary>
        /// Indicates that the mounted image needs to be remounted before being
        /// serviced.
        /// </summary>
        NeedsRemount = 1,

        /// <summary>
        /// Indicates that the mounted image is corrupt and is in an invalid
        /// state.
        /// </summary>
        Invalid = 2
    }
}
