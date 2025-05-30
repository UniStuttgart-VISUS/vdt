// <copyright file="BootService.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Management;
using System.Runtime.Versioning;
using Visus.DeploymentToolkit.Properties;


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// Implementation of the startup modification service.
    /// </summary>
    /// <remarks>
    /// The BCD-related stuff is shamelessly stolen from
    /// https://github.com/mattifestation/BCD/ - and from Copilot.
    /// </remarks>
    /// <param name="wmi"></param>
    /// <param name="logger"></param>
    internal sealed class BootService(IManagementService wmi,
            ILogger<BootService> logger) : IBootService {

        #region Public methods
        /// <inheritdoc />
        [SupportedOSPlatform("windows")]
        public ManagementBaseObject CreateBcdStore(string path) {
            ArgumentException.ThrowIfNullOrWhiteSpace(path);
            path = Path.GetFullPath(path);

            var bcd = this._wmi.GetClass(BcdStoreClass, this._wmi.WmiScope);

            var args = bcd.GetMethodParameters("CreateStore");
            args["File"] = path;

            this._logger.LogTrace("Creating new BCD store at \"{Path}\".",
                path);
            var result = bcd.InvokeMethod("CreateStore", args, null);
            if (!(bool) result["ReturnValue"]) {
                this._logger.LogError("The WMI call to create the BCD "
                    + "store \"{Path}\" succeeded, but the methor indicated "
                    + "failure. Most likely, a file at the specified location "
                    + "already exists.", path);
                throw new InvalidOperationException(string.Format(
                    Errors.FailedCreateBcdStore, path));
            }

            return (ManagementBaseObject) result["Store"];
        }

        /// <inheritdoc />
        [SupportedOSPlatform("windows")]
        public ManagementObject OpenBcdStore(string? path) {
            if (string.IsNullOrWhiteSpace(path)) {
                this._logger.LogTrace("Opening BCD system store.");
                return this._wmi.GetObject($"{BcdStoreClass}.FilePath=''",
                    this._wmi.WmiScope);

            } else {
                path = Path.GetFullPath(path);
                this._logger.LogTrace("Opening BCD store at \"{Path}\".", path);
                return this._wmi.GetObject(
                    $"{BcdStoreClass}.FilePath='{path}'",
                    this._wmi.WmiScope);
            }
        }
        #endregion

        #region Private constants
        /// <summary>
        /// The WMI class representing a BCD store.
        /// </summary>
        private const string BcdStoreClass = "BcdStore";
        #endregion

        #region Private fields
        private readonly ILogger<BootService> _logger = logger
            ?? throw new ArgumentNullException(nameof(logger));
        private readonly IManagementService _wmi = wmi
            ?? throw new ArgumentNullException(nameof(wmi));
        #endregion
    }
}
