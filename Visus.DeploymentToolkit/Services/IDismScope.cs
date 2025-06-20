﻿// <copyright file="IDismScope.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 - 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// Allows callers to have DISM being initialised for the calling process.
    /// </summary>
    /// <remarks>
    /// The DISM scope should be a singleton service that can be obtained from
    /// the DI container. It will be initialised when the first task needs
    /// DISM. While not stricly necessary (the scope is a singleton), classes
    /// working with DISM hold a reference to this service in order to make sure
    /// that the DISM library stays open as long as the users live. The options
    /// stored in the DISM service also allow for finding the log files and
    /// other stuff.
    /// </remarks>
    public interface IDismScope : IDisposable {

        /// <summary>
        /// Gets the options that have been used to initialise DISM.
        /// </summary>
        DismOptions Options { get; }
    }
}
