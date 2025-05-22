// <copyright file="IRegistry.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>


using System.Security.AccessControl;

namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// The interface of the registry service, which allows retrieving data from
    /// and manipulating the registry.
    /// </summary>
    public interface IRegistry {

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
        /// Unloads a registry hive mounted under the specified key.
        /// </summary>
        /// <remarks>
        /// Calling this method requires administrative permissions.
        /// </remarks>
        /// <param name="key">The registry key where the hive is mounted.
        /// </param>
        void UnloadHive(string key);
    }
}
