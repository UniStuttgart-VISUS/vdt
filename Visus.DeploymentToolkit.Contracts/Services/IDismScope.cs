// <copyright file="IDismScope.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// Allows callers to have DISM being initialised for the calling process.
    /// </summary>
    /// <remarks>
    /// The DISM scope should be a singleton in the dependency injection
    /// container and remain active until the 
    /// </remarks>
    public interface IDismScope {

        /// <summary>
        /// Gets the options that have been used to initialise DISM.
        /// </summary>
        DismOptions Options { get; }
    }
}
