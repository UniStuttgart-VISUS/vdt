// <copyright file="DismFeature.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Runtime.InteropServices;


namespace Visus.DeploymentToolkit.Dism {

    /// <summary>
    /// Describes basic information about a feature, such as the feature name
    /// and feature state.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct DismFeature {

        /// <summary>
        /// The name of the feature.
        /// </summary>
        public string FeatureName;

        /// <summary>
        /// A valid <see cref="DismPackageFeatureState"> enumeration value
        /// such as <see cref="DismPackageFeatureState.Installed">.
        /// </summary>
        public DismPackageFeatureState State;
    }
}
