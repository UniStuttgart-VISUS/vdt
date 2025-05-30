// <copyright file="IManagementService.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Collections.Generic;
using System.Management;
using System.Runtime.Versioning;


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// Wrapper for the Common Information Model (WMI on Windows) classes.
    /// </summary>
    [SupportedOSPlatform("windows")]
    public interface IManagementService {

        #region Public properties
        /// <summary>
        /// Gets the default management scope on the local computer, which is
        /// &quot;\\.\root\cimv2&quot; on Windows.
        /// </summary>
        ManagementScope DefaultScope { get; }

        /// <summary>
        /// Gets the Windows storage management scope on the local computer,
        /// which provides access to the physical properties of installed disks.
        /// </summary>
        ManagementScope WindowsStorageScope { get; }

        /// <summary>
        /// Gets the management scope holding the BCD store of the local
        /// computer, which is &quot;\\.\root\wmi&quot; on Windows.
        /// </summary>
        ManagementScope WmiScope { get; }
        #endregion

        #region Public methods
        /// <summary>
        /// Gets a management class via its name.
        /// </summary>
        /// <param name="name">The name of the class to be retrieved.</param>
        /// <param name="scope">The management scope or <see langword="null"/>
        /// for the <see cref="DefaultScope"/>.</param>
        /// <param name="objectGetOptions">Optional options configuring the
        /// retrieval.</param>
        /// <returns>The management class with the specified name.</returns>
        ManagementClass GetClass(string name,
            ManagementScope? scope = null,
            ObjectGetOptions? objectGetOptions = null);

        /// <summary>
        /// Get all instances of the specified objec class.
        /// </summary>
        /// <param name="class"></param>
        /// <returns></returns>
        IEnumerable<ManagementObject> GetInstancesOf(string @class,
            ManagementScope? scope = null);

        /// <summary>
        /// Gets a management object via its path
        /// </summary>
        /// <param name="path"></param>
        /// <param name="scope"></param>
        /// <param name="objectGetOptions"></param>
        /// <returns></returns>
        ManagementObject GetObject(string path,
            ManagementScope? scope = null,
            ObjectGetOptions? objectGetOptions = null);

        /// <summary>
        /// Runs the given WQL query on the specified <paramref name="scope"/>
        /// or on <see cref="DefaultScope"/> if <paramref name="scope"/> is
        /// <c>null</c>.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        IEnumerable<ManagementObject> Query(string query,
            ManagementScope? scope = null);
        #endregion
    }
}
