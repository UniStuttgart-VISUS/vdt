// <copyright file="Defaults.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System;
using System.IO;


namespace Visus.DeploymentToolkit.Waik {

    /// <summary>
    /// Provides default locations for the Windows Assessment and Deployment
    /// Kit.
    /// </summary>
    public static class Defaults {

        /// <summary>
        /// Gets the WAIK base installation path.
        /// </summary>
        /// <remarks>
        /// This is where the Windows 10 WAIK is installed by default.
        /// </remarks>
        public static string BasePath => Path.Combine(ProgrammeFiles,
            "Windows Kits",
            "10",
            "Assessment and Deployment Kit");

        /// <summary>
        /// Gets the path where the deployment tools are installed, which
        /// includes tools for modifying the boot sector and creating
        /// images.
        /// </summary>
        public static string DeploymentToolsPath
            => Tools.GetDeploymentToolsPath(BasePath);

        /// <summary>
        /// Gets the base path where the WinPE extension to WAIK is installed.
        /// </summary>
        public static string WinPePath => Tools.GetWinPePath(BasePath);

        /// <summary>
        /// The folder where we expect the WAIK to be installed, which currently
        /// us the 32-bit programme files folder.
        /// </summary>
        private static string ProgrammeFiles => Environment.GetFolderPath(
            Environment.SpecialFolder.ProgramFilesX86);
    }
}
