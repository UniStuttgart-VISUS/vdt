// <copyright file="ISystemInformation.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2024 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Visus.DeploymentToolkit.Services {

    /// <summary>
    /// A service for retrieving information about the system we are running.
    /// </summary>
    public interface ISystemInformation {

        /// <summary>
        /// Gets whether the Windows version is a preinstalled environment.
        /// </summary>
        bool IsWinPE { get; }

        /// <summary>
        /// Gets whether the Windows version is a server SKU.
        /// </summary>
        bool IsServer { get; }

        /// <summary>
        /// Gets whether the Windows version is a UI-less server core
        /// installation.
        /// </summary>
        bool IsServerCore { get; }

        /// <summary>
        /// Gets the OS platform.
        /// </summary>
        PlatformID OperatingSystemPlatform { get; }

        /// <summary>
        /// Gets the version of the operating system we are running on.
        /// </summary>
        Version OperatingSystemVersion { get; }
    }
}
