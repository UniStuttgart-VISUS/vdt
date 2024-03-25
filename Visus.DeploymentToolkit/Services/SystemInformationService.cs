// <copyright file="SystemInformationService.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.IO;
using Visus.DeploymentToolkit.Properties;


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// Implementation of <see cref="ISystemInformation"/>.
    /// </summary>
    internal sealed class SystemInformationService : ISystemInformation {

        #region Public constructors
        public SystemInformationService(IRegistry registry,
                ILogger<SystemInformationService> logger) {
            this._logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
            this._registry = registry
                ?? throw new ArgumentNullException(nameof(registry));

            // Check for WinPE and Server/Server Core from MDT.
            this.IsWinPE = this._registry.KeyExists(
                @"HKLM\System\CurrentControlSet\Control\MiniNT");

            try {
                var productType = this._registry.GetValue(
                    @"HKLM\SYSTEM\CurrentControlSet\Control\ProductOptions",
                    "ProductType");
                this.IsServer = productType switch {
                    "ServerNT" => true,
                    "LanmanNT" => true,
                    _ => false,
                };
            } catch (Exception ex) {
                this._logger.LogError(ex, Errors.CouldNotGetProductType);
            }

            {
                var explorerPath = Environment.ExpandEnvironmentVariables(
                    @"%WINDIR%\explorer.exe");
                this.IsServerCore = !this.IsWinPE && !File.Exists(explorerPath);
            }
        }
        #endregion

        #region Public properties
        /// <inheritdoc />
        public bool IsWinPE { get; }

        /// <inheritdoc />
        public bool IsServer { get; }

        /// <inheritdoc />
        public bool IsServerCore { get; }

        /// <inheritdoc />
        public PlatformID OperatingSystemPlatform
            => Environment.OSVersion.Platform;

        /// <inheritdoc />
        public Version OperatingSystemVersion => Environment.OSVersion.Version;
        #endregion

        #region Private fields
        private readonly ILogger _logger;
        private readonly IRegistry _registry;
        #endregion
    }
}
