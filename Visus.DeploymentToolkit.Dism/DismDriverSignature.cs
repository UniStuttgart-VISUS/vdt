// <copyright file="DismDriverSignature.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>


namespace Visus.DeploymentToolkit.Dism {

    /// <summary>
    /// Specifies the signature status of a driver.
    /// </summary>
    public enum DismDriverSignature {

        /// <summary>
        /// The signature status of the driver is unknown.
        /// </summary>
        /// <remarks>
        /// DISM only checks for a valid signature for boot-critical drivers.
        /// </remarks>
        Unknown = 0,

        /// <summary>
        /// The driver is unsigned.
        /// </summary>
        Unsigned = 1,

        /// <summary>
        /// The driver is signed.
        /// </summary>
        Signed = 2
    }
}
