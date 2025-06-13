// <copyright file="IImageServicing.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Dism;
using System;
using System.Collections.Generic;


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// The interface of a service for modifying WIM images, for instance
    /// for injecting drivers and enabling features.
    /// </summary>
    public interface IImageServicing : IDisposable {

        #region Public properties
        /// <summary>
        /// Gets whether the service has an open session or not.
        /// </summary>
        bool IsOpen { get; }

        /// <summary>
        /// Gets a description of the installation that is bein services.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the path to the image that is being serviced or <c>null</c> if
        /// the current Windows installation is being serviced.
        /// </summary>
        string? Path { get; }
        #endregion

        #region Public methods
        /// <summary>
        /// Adds a single .cab or .msu file to an image.
        /// </summary>
        /// <param name="path">A relative or absolute path to the .cab or .msu
        /// file being added or a folder containing the expanded files of a
        /// single .cab file.</param>
        /// <param name="ignoreCheck">Specifies whether to ignore the internal
        /// applicability checks that are done when a package is added.</param>
        /// <param name="preventPending">Specifies whether to add a package if
        /// it has pending online actions.</param>
        void AddPackage(string path, bool ignoreCheck = false,
            bool preventPending = true);

        /// <summary>
        /// Adds an appx package that will install for each new user to a Windows
        /// image.
        /// </summary>
        /// <param name="appPath">Specifies the location of the appx package.
        /// </param>
        /// <param name="dependencyPackages">Specifies the location of
        /// dependency packages.</param>
        /// <param name="licencePath">Specifies the location of the .xml file
        /// containing your application licence.</param>
        /// <param name="customDataPath">Specifies the location of a custom data
        /// file. The custom data file will be renamed custom.data and saved in
        /// the app data store.</param>
        void AddProvisionedAppxPackage(string appPath,
            List<string>? dependencyPackages = null,
            string? licencePath = null,
            string? customDataPath = null);

        /// <summary>
        /// Applies the specified unattend.xml file to the image.
        /// </summary>
        /// <param name="path">A relative or absolute path to the answer file
        /// that will be applied to the image.</param>
        /// <param name="singleSession">Specifies whether the packages that are
        /// listed in an answer file will be processed in a single session or
        /// in multiple sessions.</param>
        void ApplyUnattend(string path, bool singleSession = true);

        /// <summary>
        /// Commits all changes to the image.
        /// </summary>
        void Commit();

        /// <summary>
        /// Enables a feature from from its name,
        /// </summary>
        /// <param name="feature">The name of the feature that is being enabled.
        /// To enable more than one feature, separate each feature name with a
        /// semicolon.</param>
        /// <param name="limitAccess">Specifies whether Windows Update should be
        /// contacted as a source location for downloading files if none are
        /// found in other specified locations.</param>
        /// <param name="enableAll">Specifies whether to enable all dependencies
        /// of the feature. If the specified feature or any one of its
        /// dependencies cannot be enabled, none of them will be changed from
        /// their existing state.</param>
        void EnableFeature(string feature, bool limitAccess, bool enableAll);

        /// <summary>
        /// Gets information about appx packages that will be installed for each
        /// new user.
        /// </summary>
        /// <returns>The list of provisioned appx packages.</returns>
        IEnumerable<DismAppxPackage> GetProvisionedAppxPackages();

        /// <summary>
        /// Injects all drivers in the specified folder into the image.
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="recursive"></param>
        /// <param name="forceUnsigned"></param>
        void InjectDrivers(string folder,
            bool recursive = false,
            bool forceUnsigned = false);

        /// <summary>
        /// Opens a WIM file at the specified location.
        /// </summary>
        /// <param name="path">The path to the mounted WIM image to service or
        /// <c>null</c> to service the current Windows we are running on.
        /// </param>
        void Open(string? path);

        /// <summary>
        /// Removes an appx package from a Windows image.
        /// </summary>
        /// <param name="packageName">Specifies the name of the appx package to
        /// remove.</param>
        void RemoveProvisionedAppxPackage(string packageName);

        /// <summary>
        /// Rolls back all changes.
        /// </summary>
        void RollBack();
        #endregion
    }
}
