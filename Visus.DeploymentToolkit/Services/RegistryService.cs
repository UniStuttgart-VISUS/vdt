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
using Visus.DeploymentToolkit.Properties;


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
        public object? GetValue(string key, string? name, object? fallback) {
            using var k = this.OpenKey(key, RegistryRights.ReadKey);
            return k?.GetValue(name, fallback);
        }

        /// <inheritdoc />
        public void LoadHive(string path, string mountPoint) {
            ArgumentException.ThrowIfNullOrWhiteSpace(mountPoint);

            var keyPath = Path.GetDirectoryName(mountPoint)!;
            var keyName = Path.GetFileName(mountPoint)!;

            var key = this.OpenKey(keyPath, RegistryRights.CreateSubKey);
            if (key == null) {
                throw new ArgumentException(string.Format(
                    Errors.InvalidHiveMountPoint, mountPoint));
            }

            this.LoadHive(key, keyName, path);
        }

        /// <inheritdoc />
        public bool KeyExists(string key) {
            try {
                var k = this.OpenKey(key, RegistryRights.ReadPermissions);
                return (k != null);
            } catch (Exception ex) {
                this._logger.LogDebug(ex, Errors.OpenRegistryFailed, key);
                return false;
            }
        }

        /// <inheritdoc />
        public void UnloadHive(string mountPoint) {
            ArgumentException.ThrowIfNullOrWhiteSpace(mountPoint);

            var keyPath = Path.GetDirectoryName(mountPoint)!;
            var keyName = Path.GetFileName(mountPoint)!;

            var key = this.OpenKey(keyPath, RegistryRights.CreateSubKey);
            if (key == null) {
                throw new ArgumentException(string.Format(
                    Errors.InvalidHiveMountPoint, mountPoint));
            }

            this.UnloadHive(key, keyName);
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Loads a registry hive from the given file into the specified key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="mountPoint"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        [DllImport("advapi32.dll")]
        static extern int RegLoadKey(IntPtr key, string mountPoint, string file);

        /// <summary>
        /// Unloads a registry hive mounted at <paramref name="mountPoint"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="mountPoint"></param>
        /// <returns></returns>
        [DllImport("advapi32.dll")]
        static extern int RegUnLoadKey(IntPtr key, string mountPoint);

        /// <summary>
        /// Loads a registry hive from the given file to the specified key and
        /// returns this key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="mountPoint"></param>
        /// <param name="path"></param>
        /// <param name="permissions"></param>
        /// <returns></returns>
        /// <exception cref="Win32Exception"></exception>
        private void LoadHive(RegistryKey key, string mountPoint, string path) {
            Debug.Assert(key != null);
            Debug.Assert(!string.IsNullOrWhiteSpace(mountPoint));
            Debug.Assert(!string.IsNullOrWhiteSpace(path));

            this._logger.LogTrace("Loading the registry hive from the file "
                + "\"{File}\" into the key \"{MountPoint}\".", path,
                Path.Combine(key.Name, mountPoint));

            var status = RegLoadKey(key.Handle.DangerousGetHandle(),
                mountPoint,
                path);
            if (status != 0) {
                this._logger.LogError("RegLoadKey(\"{Key}\","
                    + " \"{MountPoint}\", \"{File}\") failed with error "
                    + "code {Error}.", key.Name, mountPoint, path, status);
                throw new Win32Exception(status);
            }
        }

        /// <summary>
        /// Unmounts a previously mounted registry hive.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="mountPoint"></param>
        /// <exception cref="Win32Exception"></exception>
        private void UnloadHive(RegistryKey key, string mountPoint) {
            Debug.Assert(key != null);
            Debug.Assert(!string.IsNullOrWhiteSpace(mountPoint));

            this._logger.LogTrace("Unloading the hive mounted at "
                + "\"{MountPoint}\".", Path.Combine(key.Name, mountPoint));

            var status = RegUnLoadKey(key.Handle.DangerousGetHandle(),
                mountPoint);
            if (status != 0) {
                this._logger.LogError("RegUnloadKey(\"{Key}\","
                    + " \"{MountPoint}\") failed with error code {Error}.",
                    key.Name, mountPoint, status);
                throw new Win32Exception(status);
            }
        }

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
                + "\"path {RegistryPath}\".", key);

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

            this._logger.LogTrace("Registry hive is \"{Hive}\".", hive);
            this._logger.LogTrace("The remaining registry path is "
                + "\"{RegistryPath}\".", path);

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
