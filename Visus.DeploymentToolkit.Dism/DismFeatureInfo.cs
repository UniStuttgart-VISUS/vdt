// <copyright file="DismFeatureInfo.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Runtime.InteropServices;


namespace Visus.DeploymentToolkit.Dism {

    /// <summary>
    /// Describes advanced feature information, such as installed state and
    /// whether a restart is required after installation.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct DismFeatureInfo {

        /// <summary>
        /// The name of the feature.
        /// </summary>
        public string FeatureName;

        /// <summary>
        /// A valid <see cref="DismPackageFeatureState"/> enumeration value such
        /// as <see cref="DismPackageFeatureState.Installed"/>.
        /// </summary>
        public DismPackageFeatureState FeatureState;

        /// <summary>
        /// The display name of the feature. This is not always unique across all
        /// features.
        /// </summary>
        public string DisplayName;

        /// <summary>
        /// The description of the feature.
        /// </summary>
        public string Description;

        /// <summary>
        /// A <see cref="DismRestartType"/> enumeration value such as
        /// <see cref="DismRestartType.Possible"/>.
        /// </summary>
        public DismRestartType RestartRequired;

        /// <summary>
        /// An array of <see cref="DismCustomProperty" /> structures.
        /// </summary>
        public IntPtr CustomProperty;

        /// <summary>
        /// The number of elements in the <see cref="CustomProperty" /> array.
        /// </summary>
        public uint CustomPropertyCount;
    }
}
