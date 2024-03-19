// <copyright file="DismCapabilityInfo.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Runtime.InteropServices;


namespace Visus.DeploymentToolkit.Dism {

    /// <summary>
    /// Describes information about a capability.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct DismCapabilityInfo {

        /// <summary>
        /// The name of the capability.
        /// </summary>
        public string Name;

        /// <summary>
        /// The state of the capability.
        /// </summary>
        public DismPackageFeatureState State;

        /// <summary>
        /// The display name of the capability.
        /// </summary>
        public string DisplayName;

        /// <summary>
        /// The description of the capability.
        /// </summary>
        public string Description;

        /// <summary>
        /// The download size of the capability in bytes.
        /// </summary>
        public uint DownloadSize;

        /// <summary>
        /// The install size of the capability in bytes.
        /// </summary>
        public uint InstallSize;
    }
}
