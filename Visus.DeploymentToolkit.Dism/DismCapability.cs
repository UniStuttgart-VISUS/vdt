// <copyright file="DismCapability.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Runtime.InteropServices;


namespace Visus.DeploymentToolkit.Dism {

    /// <summary>
    /// Describes capability basic information.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct DismCapability {

        /// <summary>
        /// The manufacturer name of the driver.
        /// </summary>
        public string Name;

        /// <summary>
        /// A hardware description of the driver.
        /// </summary>
        public DismPackageFeatureState State;
    }
}
