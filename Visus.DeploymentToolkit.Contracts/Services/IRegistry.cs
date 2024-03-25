// <copyright file="IRegistry.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// The interface of the registry service, which allows retrieving data from
    /// and manipulating the registry.
    /// </summary>
    public interface IRegistry {

        /// <summary>
        /// Gets the given value from the registry.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="name"></param>
        /// <param name="fallback"></param>
        /// <returns></returns>
        object? GetValue(string key, string name, object? fallback = null);

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
    }
}
