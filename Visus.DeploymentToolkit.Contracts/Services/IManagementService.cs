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
        #endregion

        #region Public methods
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
