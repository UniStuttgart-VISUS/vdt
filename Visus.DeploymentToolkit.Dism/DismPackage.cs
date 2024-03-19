// <copyright file="DismPackage.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Runtime.InteropServices;


namespace Visus.DeploymentToolkit.Dism {

    /// <summary>
    /// Describes basic information about a package, including the date and time
    /// that the package was installed.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct DismPackage {

        /// <summary>
        /// The package name.
        /// </summary>
        public string PackageName;

        /// <summary>
        /// A <see cref="DismPackageFeatureState"/> enumeration value, for
        /// example, <see cref="DismPackageFeatureState.Removed"/>.
        /// </summary>
        public DismPackageFeatureState PackageState;

        /// <summary>
        /// A <see cref="DismReleaseType"/> Enumeration value, for example,
        /// <see cref="DismReleaseType.Driver"/>.
        /// </summary>
        public DismReleaseType ReleaseType;

        /// <summary>
        /// The date and time that the package was installed. This field is
        /// local time relative to the servicing host computer.
        /// </summary>
        public SystemTime InstallTime;
    }
}
