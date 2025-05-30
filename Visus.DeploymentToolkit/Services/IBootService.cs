// <copyright file="IBootService.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.Management;


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// The interface of a service class that allows for modifying the BCD
    /// store, the boot sector, etc.
    /// </summary>
    public interface IBootService {

        /// <summary>
        /// Create a new BCD store at the specified location.
        /// </summary>
        /// <remarks>
        /// This is equivalent to &quot;bcdedit.exe /createstore
        /// <paramref name="path"/>&quot;.
        /// </remarks>
        /// <param name="path">The path to the BCD store to be created.</param>
        /// <returns>The WMI object representing the new store.</returns>
        ManagementBaseObject CreateBcdStore(string path);

        /// <summary>
        /// Opens a BCD store at the specified location.
        /// </summary>
        /// <param name="path">The path to the BCD store to be opened. If this
        /// path is empty or <see langword="null"/>, the system store will be
        /// opened.</param>
        /// <returns>A WMI object representing the opened store.</returns>
        ManagementObject OpenBcdStore(string? path);

    }
}
