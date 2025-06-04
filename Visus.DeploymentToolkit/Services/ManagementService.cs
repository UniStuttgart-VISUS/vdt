// <copyright file="ManagementService.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Runtime.Versioning;


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// Implementation of the WMI abstraction.
    /// </summary>
    [SupportedOSPlatform("windows")] 
    internal sealed class ManagementService : IManagementService {

        #region Public constructors
        public ManagementService(ILogger<ManagementService> logger) {
            this._logger = logger
                ?? throw new ArgumentNullException(nameof(logger));

            this._defaultScope = new(() => {
                var retval = new ManagementScope(@"\\.\root\cimv2");
                retval.Connect();
                return retval;
            });
            this._windowsStorageScope = new(() => {
                var retval = new ManagementScope(
                    @"\\.\root\Microsoft\Windows\Storage");
                retval.Connect();
                return retval;
            });
            this._wmiScope = new(() => {
                var retval = new ManagementScope(@"\\.\root\wmi");
                retval.Connect();
                return retval;
            });
        }
        #endregion

        #region Public properties
        /// <inheritdoc />
        public ManagementScope DefaultScope => this._defaultScope.Value;

        /// <inheritdoc />
        public ManagementScope WindowsStorageScope
            => this._windowsStorageScope.Value;

        /// <inheritdoc />
        public ManagementScope WmiScope => this._wmiScope.Value;
        #endregion

        #region Public methods
        /// <inheritdoc />
        public ManagementClass GetClass(string name,
                ManagementScope? scope,
                ObjectGetOptions? objectGetOptions) {
            this._logger.LogTrace("Getting management class {Path}.",
                name);
            return new ManagementClass(scope ?? this.DefaultScope,
                new ManagementPath(name),
                objectGetOptions);
        }

        /// <inheritdoc />
        public IEnumerable<ManagementObject> GetInstancesOf(string @class,
                ManagementScope? scope) {
            return this.Query($"SELECT * FROM {@class}", scope);
        }

        /// <inheritdoc />
        public ManagementObject GetObject(string path,
                ManagementScope? scope,
                ObjectGetOptions? objectGetOptions) {
            ArgumentNullException.ThrowIfNull(path);
            this._logger.LogTrace("Selecting a WMI object via its path "
                + "{Path}.", path);
            return new ManagementObject(scope ?? this.DefaultScope,
                new ManagementPath(path),
                objectGetOptions);
        }

        /// <inheritdoc />
        public IEnumerable<ManagementObject> Query(string query,
                ManagementScope? scope) {
            this._logger.LogTrace("Issuing WMI query {Query}.", query);
            var search = new ManagementObjectSearcher(
                scope ?? this.DefaultScope,
                new ObjectQuery(query));
            return search.Get().Cast<ManagementObject>();
        }
        #endregion

        #region Private fields
        private readonly Lazy<ManagementScope> _defaultScope;
        private readonly ILogger _logger;
        private readonly Lazy<ManagementScope> _windowsStorageScope;
        private readonly Lazy<ManagementScope> _wmiScope;
        #endregion
    }
}
