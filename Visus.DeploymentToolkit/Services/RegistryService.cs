// <copyright file="RegistryService.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 - 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security.AccessControl;
using Visus.DeploymentToolkit.Bcd;
using Visus.DeploymentToolkit.Properties;
using Visus.DeploymentToolkit.Security;


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// Implementation of the <see cref="IRegistry"/> service.
    /// </summary>
    [SupportedOSPlatform("windows")]
    internal sealed class RegistryService : IRegistry {

        #region Public constructors
        /// <summary>
        /// Initialises a new instance.
        /// </summary>
        /// <param name="logger"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public RegistryService(ILogger<RegistryService> logger) {
            this._logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
        }
        #endregion

        #region Public methods
        /// <inheritdoc />
        public void DeleteValue(string key, string name) {
            using var k = this.OpenKey(key, RegistryRights.ReadKey);
            k?.DeleteValue(name, false);
        }

        /// <inheritdoc />
        public object? GetValue(string key, string? name, object? fallback) {
            using var k = this.OpenKey(key, RegistryRights.ReadKey);
            return k?.GetValue(name, fallback);
        }

        /// <inheritdoc />
        public MountedHive LoadHive(string path, string mountPoint) {
            ArgumentException.ThrowIfNullOrWhiteSpace(mountPoint);

            var keyPath = Path.GetDirectoryName(mountPoint)!;
            var keyName = Path.GetFileName(mountPoint)!;

            using var key = this.OpenKey(keyPath, RegistryRights.CreateSubKey);
            if (key == null) {
                throw new ArgumentException(string.Format(
                    Errors.InvalidHiveMountPoint, mountPoint));
            }

            this._logger.LogTrace("Loading the registry hive from the file "
                + "{File} into the key {MountPoint}.", path,
                    Path.Combine(key.Name, keyName));
            return new MountedHive(key, keyName, path);
        }

        /// <inheritdoc />
        public bool KeyExists(string key) {
            try {
                using var k = this.OpenKey(key, RegistryRights.ReadPermissions);
                return (k != null);
            } catch (Exception ex) {
                this._logger.LogDebug(ex, Errors.OpenRegistryFailed, key);
                return false;
            }
        }

        /// <summary>
        /// Sets the given registry value.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="kind"></param>
        /// <exception cref="ArgumentException"></exception>
        public void SetValue(string key,
                string? name,
                object value,
                RegistryValueKind kind) {
            using var k = this.OpenKey(key,
                RegistryRights.ReadKey | RegistryRights.SetValue);

            if (k == null) {
                throw new ArgumentException(string.Format(
                    Errors.MissingRegistryKey, key));
            }

            k.SetValue(name, value, kind);
        }

        /// <inheritdoc />
        public bool ValueExists(string key, string? value) {
            try {
                using var k = this.OpenKey(key, RegistryRights.ReadKey);
                return (k?.GetValue(value, null) != null);
            } catch (Exception ex) {
                this._logger.LogDebug(ex, Errors.OpenRegistryFailed, key);
                return false;
            }
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Parses the registry hive from the given <paramref name="key"/> path
        /// and opens the hive.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        private RegistryKey OpenHive(string key, out string path) {
            _ = key ?? throw new ArgumentNullException(nameof(key));

            this._logger.LogTrace("Parsing the registry hive from the given "
                + "path {RegistryPath}.", key);

            // Out first assumption is that 'key' is the hive itself.
            var hive = key;

            // Separate the hive from the path.
            var end = hive.IndexOf('\\');
            if (end >= 0) {
                hive = key.Substring(0, end);
                path = key.Substring(end + 1);
            } else {
                path = string.Empty;
            }
            hive = hive.ToUpperInvariant();

            this._logger.LogTrace("Registry hive is {Hive}.", hive);
            this._logger.LogTrace("The remaining registry path is "
                + "{RegistryPath}.", path);

            return hive switch {
                "HKEY_CLASSES_ROOT" => Registry.ClassesRoot,
                "HKCR" => Registry.ClassesRoot,
                "HKEY_CURRENT_USER" => Registry.CurrentUser,
                "HKCU" => Registry.CurrentUser,
                "HKEY_LOCAL_MACHINE" => Registry.LocalMachine,
                "HKLM" => Registry.LocalMachine,
                "HKEY_USERS" => Registry.Users,
                "HKU" => Registry.Users,
                "HKEY_PERFORMANCE_DATA" => Registry.PerformanceData,
                "HKEY_CURRENT_CONFIG" => Registry.CurrentConfig,
                "HKCC" => Registry.CurrentConfig,
                _ => throw new ArgumentException(string.Format(
                    Errors.NoRegistryHive, key))
            };
        }

        /// <summary>
        /// Opens the specified registry key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="permissions"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        private RegistryKey? OpenKey(string key, RegistryRights permissions) {
            var hive = this.OpenHive(key, out var path);

            if (path.Length == 0) {
                // If the path is empty, this was a request for the hive
                // itself.
                return hive;
            }

            return hive.OpenSubKey(path, permissions);
        }
        #endregion

        #region Private fields
        private readonly ILogger _logger;
        #endregion
    }
}
