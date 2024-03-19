// <copyright file="DismPackageInfo.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Runtime.InteropServices;


namespace Visus.DeploymentToolkit.Dism {

    /// <summary>
    /// Describes detailed package information such as the client used to
    /// install the package, the date and time that the package was installed,
    /// and support information.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct DismPackageInfo {

        /// <summary>
        /// The name of the package.
        /// </summary>
        public string PackageName;

        /// <summary>
        /// A <see cref="DismPackageFeatureState"/> enumeration value such as
        /// <see cref="DismPackageFeatureState.Removed"/>.
        /// </summary>
        public DismPackageFeatureState PackageState;

        /// <summary>
        /// A <see cref="DismReleaseType"/> enumeration value such as
        /// <see cref="DismReleaseType.Update"/>.
        /// </summary>
        public DismReleaseType ReleaseType;

        /// <summary>
        /// The date and time that the package was installed. This field is
        /// local time, based on the servicing host computer.
        /// </summary>
        public SystemTime InstallTime;

        /// <summary>
        /// <c>true</c> if the package is applicable to the image, otherwise
        /// <c>false</c>.
        /// </summary>
        public bool Applicable;

        /// <summary>
        /// The copyright information of the package.
        /// </summary>
        public string Copyright;

        /// <summary>
        /// The company that released the package.
        /// </summary>
        public string Company;

        /// <summary>
        /// The date and time that the package was created. This field is local
        /// time, based on the time zone of the computer that created the
        /// package.
        /// </summary>
        public SystemTime CreationTime;

        /// <summary>
        /// The display name of the package.
        /// </summary>
        public string DisplayName;

        /// <summary>
        /// A description of the purpose of the package.
        /// </summary>
        public string Description;

        /// <summary>
        /// The client that installed this package.
        /// </summary>
        public string InstallClient;

        /// <summary>
        /// The client that installed this package.
        /// </summary>
        public string InstallPackageName;

        /// <summary>
        /// The date and time when this package was last updated. This field is
        /// local time, based on the servicing host computer.
        /// </summary>
        public SystemTime LastUpdateTime;

        /// <summary>
        /// The product name for this package.
        /// </summary>
        public string ProductName;

        /// <summary>
        /// The product version for this package.
        /// </summary>
        public string ProductVersion;

        /// <summary>
        /// A <see cref="DismRestartType"/>  enumeration value describing
        /// whether a restart is required after installing the package on an
        /// online image.
        /// </summary>
        public DismRestartType RestartRequired;

        /// <summary>
        /// A <see cref="DismFullyOfflineInstallable"/> enumeration value
        /// describing whether a package can be installed offline without
        /// booting the image.
        /// </summary>
        public DismFullyOfflineInstallable FullyOffline;

        /// <summary>
        /// A string listing additional support information for this package.
        /// </summary>
        public string SupportInformation;

        /// <summary>
        /// An array of <see cref="DismCustomProperty"/> structures representing
        /// the custom properties of the package.
        /// </summary>
        public IntPtr CustomProperties;

        /// <summary>
        /// The number of elements in the <see cref="CustomProperties"/> array.
        /// </summary>
        public uint CustomPropertyCount;

        /// <summary>
        /// An array of <see cref="DismFeature"> structures representing the
        /// features in the package.
        /// </summary>
        public IntPtr Features;

        /// <summary>
        /// The number of elements in the <see cref="Features" /> array.
        /// </summary>
        public uint FeatureCount;
    }
}
