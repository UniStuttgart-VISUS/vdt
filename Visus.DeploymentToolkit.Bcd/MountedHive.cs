// <copyright file="MountedHive.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Security.AccessControl;


namespace Visus.DeploymentToolkit.Bcd {

    /// <summary>
    /// Represents a registry hive from a file mounted in the registry of the
    /// running system.
    /// </summary>
    /// <remarks>
    /// As the BCD is actually a registry hive mounted by the boot manager, it
    /// is easiest for us to manipulate stores by mounting the hive into the
    /// active registry and edit it there.
    /// </remarks>
    [SupportedOSPlatform("windows")]
    public sealed class MountedHive : IDisposable {

        #region Public casts
        /// <summary>
        /// Converts the give to a <see cref="RegistryKey"/> with all access
        /// permissions.
        /// </summary>
        /// <param name="hive">The hive to be converted.</param>
        public static implicit operator RegistryKey?(MountedHive hive)
            => hive?.Open(RegistryKeyPermissionCheck.ReadWriteSubTree,
                RegistryRights.FullControl);
        #endregion

        #region Public constructors
        /// <summary>
        /// Mounts the registry hive in <paramref name="path"/> to the specified
        /// <paramref name="subKey"/> of <paramref name="key"/>.
        /// </summary>
        /// <remarks>
        /// <para>This constructor requires the code to run with administrative
        /// privileges. The process must have &quot;SeBackupPrivilege&quot; and
        /// &quot;SeRestorePrivilege&quot;. Callers can achieve this by holding
        /// a <see cref="Visus.DeploymentToolkit.Security.TokenPrivilege"/>
        /// object for each privilege.</para>
        /// </remarks>
        /// <param name="key">The registry key where the hive should be mounted.
        /// This must be either <see cref="Registry.Users"/> or
        /// <see cref="Registry.LocalMachine"/>.</param>
        /// <param name="subKey">The name of a non-existent sub-key of 
        /// <paramref name="key"/> where the hive is mounted.</param>
        /// <param name="path">The path to the hive file.</param>
        /// <exception cref="Win32Exception">In case the operation did not
        /// succeed. This is tyically the case if the caller is not running
        /// as administrator or the required privileges have not been acquired
        /// for the calling process.</exception>
        public MountedHive(RegistryKey key, string subKey, string path) {
            ArgumentNullException.ThrowIfNull(key);
            ArgumentNullException.ThrowIfNull(subKey);
            ArgumentNullException.ThrowIfNull(path);

            var status = RegLoadKey(key.Handle.DangerousGetHandle(),
                subKey, path);
            if (status != 0) {
                throw new Win32Exception(status);
            }

            this._key = key;
            this._subKey = subKey;
        }

        /// <summary>
        /// Mounts the registry hive in <paramref name="path"/> to the specified
        /// <paramref name="subKey"/> of <paramref name="hive"/>.
        /// </summary>
        /// <remarks>
        /// <para>This constructor requires the code to run with administrative
        /// privileges. The process must have &quot;SeBackupPrivilege&quot; and
        /// &quot;SeRestorePrivilege&quot;. Callers can achieve this by holding
        /// a <see cref="Visus.DeploymentToolkit.Security.TokenPrivilege"/>
        /// object for each privilege.</para>
        /// </remarks>
        /// <param name="key">The registry key where the hive should be mounted.
        /// This must be either <see cref="RegistryHive.Users"/> or
        /// <see cref="RegistryHive.LocalMachine"/>.</param>
        /// <param name="subKey">The name of a non-existent sub-key of 
        /// <paramref name="key"/> where the hive is mounted.</param>
        /// <param name="path">The path to the hive file.</param>
        /// <exception cref="Win32Exception">In case the operation did not
        /// succeed. This is tyically the case if the caller is not running
        /// as administrator or the required privileges have not been acquired
        /// for the calling process.</exception>
        public MountedHive(RegistryHive hive, string subKey, string path)
            : this(RegistryKey.OpenBaseKey(hive, RegistryView.Default),
                subKey,
                path) { }
        #endregion

        #region Finaliser
        /// <summary>
        /// Finalises the instance.
        /// </summary>
        ~MountedHive() => this.Dispose(false);
        #endregion

        #region Public methods
        /// <inheritdoc />
        public void Dispose() {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Opens the mounted registry hive.
        /// </summary>
        /// <param name="permissionCheck"></param>
        /// <param name="rights"></param>
        /// <returns></returns>
        /// <exception cref="ObjectDisposedException"></exception>
        public RegistryKey? Open(RegistryKeyPermissionCheck permissionCheck,
                RegistryRights rights) {
            ObjectDisposedException.ThrowIf(this._subKey is null, this);
            return this._key.OpenSubKey(this._subKey, permissionCheck, rights);
        }

        /// <summary>
        /// Opens the mounted registry hive.
        /// </summary>
        /// <param name="permissionCheck"></param>
        /// <returns></returns>
        /// <exception cref="ObjectDisposedException"></exception>
        public RegistryKey? Open(RegistryKeyPermissionCheck permissionCheck) {
            ObjectDisposedException.ThrowIf(this._subKey is null, this);
            return this._key.OpenSubKey(this._subKey, permissionCheck);
        }

        /// <summary>
        /// Opens the mounted registry hive.
        /// </summary>
        /// <param name="writeable"></param>
        /// <returns></returns>
        /// <exception cref="ObjectDisposedException"></exception>
        public RegistryKey? Open(bool writeable = false) {
            ObjectDisposedException.ThrowIf(this._subKey is null, this);
            return this._key.OpenSubKey(this._subKey, writeable);
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Creates a subkey under <c>HKEY_USERS</c> or
        /// <c>HKEY_LOCAL_MACHINE</c> and loads the data from the specified
        /// registry hive into that subkey.
        /// </summary>
        /// <param name="key">A handle to the key where the subkey will be
        /// created.</param>
        /// <param name="subKey">The name of the key to be created under
        /// <paramref name="key"/>. This subkey is where the registration
        /// information from the file will be loaded.</param>
        /// <param name="file">The name of the file containing the registry
        /// data. If this file does not exist, a file is created with the
        /// specified name.</param>
        /// <returns>If the function succeeds, the return value is 0.</returns>
        [DllImport("advapi32.dll")]
        static extern int RegLoadKey(nint key, string subKey, string file);

        /// <summary>
        /// Unloads a registry hive mounted at <paramref name="subKey"/>.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="subKey"></param>
        /// <returns></returns>
        [DllImport("advapi32.dll")]
        static extern int RegUnLoadKey(nint key, string subKey);

        /// <summary>
        /// Unloads the hive if there is any.
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing) {
            if (this._subKey is not null) {
                Debug.Assert(this._key is not null);
                var status = RegUnLoadKey(this._key.Handle.DangerousGetHandle(),
                    this._subKey);
                if (status != 0) {
                    throw new Win32Exception(status);
                }

                if (disposing) {
                    this._key.Dispose();
                }

                this._subKey = null;
            }
        }
        #endregion

        #region Private fields
        private RegistryKey _key;
        private string? _subKey;
        #endregion
    }
}
