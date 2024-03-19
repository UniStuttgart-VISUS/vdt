// <copyright file="DismCapabilityInfo.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Runtime.InteropServices;


namespace Visus.DeploymentToolkit.Dism {

    /// <summary>
    /// Describes the custom properties of a package. Custom properties are any
    /// properties that are not found in <see cref="DismPackage "/> or 
    /// <see cref="DismFeature"/> structures.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct DismCustomProperty {

        /// <summary>
        /// The name of the custom property.
        /// </summary>
        public string Name;

        /// <summary>
        /// The value of the custom property.
        /// </summary>
        public string Value;

        /// <summary>
        /// The path of the custom property.
        /// </summary>
        public string Path;
    }
}
