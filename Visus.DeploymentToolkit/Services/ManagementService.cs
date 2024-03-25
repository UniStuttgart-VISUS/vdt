// <copyright file="ManagementService.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Runtime.Versioning;
using Visus.DeploymentToolkit.Properties;


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
            this._windowsStoageScope = new(() => {
                var retval = new ManagementScope(
                    @"\\.\root\Microsoft\Windows\Storage");
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
            => this._windowsStoageScope.Value;
        #endregion

        #region Public methods
        /// <inheritdoc />
        public IEnumerable<ManagementObject> GetInstancesOf(string @class,
                ManagementScope? scope = null) {
            return this.Query($"SELECT * FROM {@class}", scope);
        }

        /// <inheritdoc />
        public IEnumerable<ManagementObject> Query(string query,
                ManagementScope? scope) {
            this._logger.LogTrace(Resources.IssuingWmiQuery, query);
            var search = new ManagementObjectSearcher(
                scope ?? this.DefaultScope,
                 new ObjectQuery(query));
            return search.Get().Cast<ManagementObject>();
        }
        #endregion

        #region Private fields
        private readonly Lazy<ManagementScope> _defaultScope;
        private readonly ILogger _logger;
        private readonly Lazy<ManagementScope> _windowsStoageScope;
        #endregion
    }
}
