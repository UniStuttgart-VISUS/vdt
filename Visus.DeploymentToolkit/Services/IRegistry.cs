// <copyright file="IRegistry.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using Microsoft.Win32;
using System.Runtime.Versioning;


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// The interface of the registry service, which allows retrieving data from
    /// and manipulating the registry.
    /// </summary>
    [SupportedOSPlatform("windows")]
    public interface IRegistry {

        /// <summary>
        /// Delete the given value from <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The path to the key holding the value to be
        /// deleted.</param>
        /// <param name="name">The name of the value to be deleted.</param>
        void DeleteValue(string key, string name);

        /// <summary>
        /// Gets the given value from the registry.
        /// </summary>
        /// <param name="key">The full path to the registry key.</param>
        /// <param name="name">The name of the value to open or <c>null</c> for
        /// the default value.</param>
        /// <param name="fallback">The default value being returned if the
        /// requested value was not in the registry. This parameter defaults
        /// to <c>null</c>.</param>
        /// <returns></returns>
        object? GetValue(string key, string? name, object? fallback = null);

        /// <summary>
        /// Mounts a registry hive from the given file into the specified key.
        /// </summary>
        /// <remarks>
        /// Calling this method requires administrative permissions.
        /// </remarks>
        /// <param name="path">The path to the registry file to be mounted.
        /// </param>
        /// <param name="mountPoint">The location where the registry hive should
        /// be mounted. Note that you should not provide an existing key here,
        /// but the name of a new key within an existing location.</param>
        public void LoadHive(string path, string mountPoint);

        /// <summary>
        /// Answer whether the given registry key exists.
        /// </summary>
        /// <remarks>
        /// This method never throws. Any error indicating failure to open the
        /// registry will yield <c>false</c>.
        /// </remarks>
        /// <param name="key"></param>
        /// <returns></returns>
        bool KeyExists(string key);

        /// <summary>
        /// Sets the given value in the registry.
        /// </summary>
        /// <param name="key">The path to the key where the value should be
        /// added.</param>
        /// <param name="name">The name of the value. This can be <c>null</c>
        /// for the default value.</param>
        /// <param name="value">The value to be set.</param>
        /// <param name="kind">The type used for stor <paramref name="value"/>.
        /// </param>
        void SetValue(string key, string? name, object value,
            RegistryValueKind kind);

        /// <summary>
        /// Sets the given value in the registry.
        /// </summary>
        /// <param name="key">The path to the key where the value should be
        /// added.</param>
        /// <param name="name">The name of the value. This can be <c>null</c>
        /// for the default value.</param>
        /// <param name="value">The value to be set.</param>
        /// <param name="expand">If <c>true</c>, the string will be marked
        /// that variables are to be expanded.</param>
        void SetValue(string key,
                string? name,
                string value,
                bool expand = false)
            => this.SetValue(key,
                name,
                value,
                expand ? RegistryValueKind.ExpandString : RegistryValueKind.String);

        /// <summary>
        /// Sets the given value in the registry.
        /// </summary>
        /// <param name="key">The path to the key where the value should be
        /// added.</param>
        /// <param name="name">The name of the value. This can be <c>null</c>
        /// for the default value.</param>
        /// <param name="value">The value to be set.</param>
        void SetValue(string key, string? name, string[] value)
            => this.SetValue(key, name, value, RegistryValueKind.MultiString);

        /// <summary>
        /// Sets the given value in the registry.
        /// </summary>
        /// <param name="key">The path to the key where the value should be
        /// added.</param>
        /// <param name="name">The name of the value. This can be <c>null</c>
        /// for the default value.</param>
        /// <param name="value">The value to be set.</param>
        void SetValue(string key, string? name, int value)
            => this.SetValue(key, name, value, RegistryValueKind.DWord);

        /// <summary>
        /// Sets the given value in the registry.
        /// </summary>
        /// <param name="key">The path to the key where the value should be
        /// added.</param>
        /// <param name="name">The name of the value. This can be <c>null</c>
        /// for the default value.</param>
        /// <param name="value">The value to be set.</param>
        void SetValue(string key, string? name, long value)
            => this.SetValue(key, name, value, RegistryValueKind.QWord);

        /// <summary>
        /// Unloads a registry hive mounted under the specified key.
        /// </summary>
        /// <remarks>
        /// Calling this method requires administrative permissions.
        /// </remarks>
        /// <param name="key">The registry key where the hive is mounted.
        /// </param>
        void UnloadHive(string key);

        /// <summary>
        /// Answer whether the given registry key value.
        /// </summary>
        /// <remarks>
        /// This method never throws. Any error indicating failure to open the
        /// registry will yield <c>false</c>.
        /// </remarks>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool ValueExists(string key, string? value);
    }
}
