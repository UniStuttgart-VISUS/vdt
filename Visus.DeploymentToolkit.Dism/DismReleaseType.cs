// <copyright file="DismReleaseType.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>


namespace Visus.DeploymentToolkit.Dism {

    /// <summary>
    /// Specifies the release type of a package.
    /// </summary>
    public enum DismReleaseType {

        /// <summary>
        /// The package is a critical update.
        /// </summary>
        CriticalUpdate = 0,

        /// <summary>
        /// The package is a driver.
        /// </summary>
        Driver = 1,

        /// <summary>
        /// The package is a feature pack.
        /// </summary>
        FeaturePack = 2,

        /// <summary>
        /// The package is a hotfix.
        /// </summary>
        Hotfix = 3,

        /// <summary>
        /// The package is a security update.
        /// </summary>
        SecurityUpdate = 4,

        /// <summary>
        /// The package is a software update.
        /// </summary>
        SoftwareUpdate = 5,

        /// <summary>
        /// The package is a general update.
        /// </summary>
        Update = 6,

        /// <summary>
        /// The package is an update rollup.
        /// </summary>
        UpdateRollup = 7,

        /// <summary>
        /// The package is a language pack.
        /// </summary>
        LanguagePack = 8,

        /// <summary>
        /// The package is a foundation package.
        /// </summary>
        Foundation = 9,

        /// <summary>
        /// The package is a service pack.
        /// </summary>
        ServicePack = 10,

        /// <summary>
        /// The package is a product release.
        /// </summary>
        Product = 11,

        /// <summary>
        /// The package is a local pack.
        /// </summary>
        LocalPack = 12,

        /// <summary>
        /// The package is another type of release.
        /// </summary>
        Other = 13,

        /// <summary>
        /// This package is a feature on demand.
        /// </summary>
        OnDemandPack = 14
    }
}
