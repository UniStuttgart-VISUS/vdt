// <copyright file="Tools.cs" company="Visualisierungsinstitut der Universität Stuttgart">
// Copyright © 2025 Visualisierungsinstitut der Universität Stuttgart.
// Licensed under the MIT licence. See LICENCE file for details.
// </copyright>
// <author>Christoph Müller</author>

using System.IO;
using System.Runtime.InteropServices;
using Visus.DeploymentToolkit.Extensions;


namespace Visus.DeploymentToolkit.Waik {

    /// <summary>
    /// Provides methods for resolving the expected locations of specific tools
    /// in the WAIK installation folder.
    /// </summary>
    public static class Tools {

        /// <summary>
        /// Gets the path to the bcdboot tool for restoring the BCD store.
        /// </summary>
        /// <param name="basePath">The installation path of the WAIK.</param>
        /// <param name="architecture">The platform architecture to get the
        /// tools for.</param>
        /// <returns>The path of bcdboot.exe in the given WAIK installation.
        /// </returns>
        public static string GetBcdbootPath(string basePath,
                Architecture architecture)
            => Path.Combine(GetDeploymentToolsPath(basePath),
                architecture.GetFolder(),
                "BCDBoot",
                "bcdboot.exe");

        /// <summary>
        /// Gets the path to the bcdedit tool for editing the BCD store.
        /// </summary>
        /// <param name="basePath">The installation path of the WAIK.</param>
        /// <param name="architecture">The platform architecture to get the
        /// tools for.</param>
        /// <returns>The path of bcdedit.exe in the given WAIK installation.
        /// </returns>
        public static string GetBcdedit(string basePath,
                Architecture architecture)
            => Path.Combine(GetDeploymentToolsPath(basePath),
                architecture.GetFolder(),
                "BCDBoot",
                "bcdedit.exe");

        /// <summary>
        /// Gets the path to the bootsect tool.
        /// </summary>
        /// <param name="basePath">The installation path of the WAIK.</param>
        /// <param name="architecture">The platform architecture to get the
        /// tools for.</param>
        /// <returns>The path of bootsect.exe in the given WAIK installation.
        /// </returns>
        public static string GetBootsect(string basePath,
                Architecture architecture)
            => Path.Combine(GetDeploymentToolsPath(basePath),
                architecture.GetFolder(),
                "BCDBoot",
                "bootsect.exe");

        /// <summary>
        /// Gets the path where the deployment tools are installed, which
        /// includes tools for modifying the boot sector and creating
        /// images.
        /// </summary>
        /// <param name="basePath">The installation path of the WAIK.</param>
        /// <returns>The path where the deployment tools are located.</returns>
        public static string GetDeploymentToolsPath(string basePath)
            => Path.Combine(basePath, "Deployment Tools");

        /// <summary>
        /// Gets the path to the DISM tool.
        /// </summary>
        /// <param name="basePath">The installation path of the WAIK.</param>
        /// <param name="architecture">The platform architecture to get the
        /// tools for.</param>
        /// <returns>The path of dism.exe in the given WAIK installation.
        /// </returns>
        public static string GetDismPath(string basePath,
                Architecture architecture)
            => Path.Combine(GetDeploymentToolsPath(basePath),
                architecture.GetFolder(),
                "DISM",
                "dism.exe");

        /// <summary>
        /// Gets the path to the oscdimg tool for creating ISO images.
        /// </summary>
        /// <param name="basePath">The installation path of the WAIK.</param>
        /// <param name="architecture">The platform architecture to get the
        /// tools for.</param>
        /// <returns>The path of oscdimg.exe in the given WAIK installation.
        /// </returns>
        public static string GetOscdimgPath(string basePath,
                Architecture architecture)
            => Path.Combine(GetDeploymentToolsPath(basePath),
                architecture.GetFolder(),
                "Oscdimg",
                "oscdimg.exe");

        /// <summary>
        /// Gets the path to the Windows setup files.
        /// </summary>
        /// <param name="basePath">The installation path of the WAIK.</param>
        /// <param name="architecture">The platform architecture to get the
        /// tools for.</param>
        /// <returns>The path of the setup files.</returns>
        public static string GetSetupPath(string basePath,
                Architecture architecture)
            => Path.Combine(basePath,
                "Windows Setup",
                architecture.GetFolder());

        /// <summary>
        /// Gets the base path where the WinPE extension to WAIK is installed.
        /// </summary>
        /// <param name="basePath">The installation path of the WAIK.</param>
        /// <returns>The path where the WinPE templates are located.</returns>
        public static string GetWinPePath(string basePath)
            => Path.Combine(basePath, "Windows Preinstallation Environment");

        /// <summary>
        /// Gets the path where the optional components for Windows PE are
        /// located.
        /// </summary>
        /// <param name="basePath">The installation path of the WAIK.</param>
        /// <param name="architecture">The platform architecture to get the
        /// optional components for.</param>
        /// <returns>The path to the optional components.</returns>
        public static string GetWinPeOptionalComponentsPath(string basePath,
                Architecture architecture)
            => Path.Combine(GetWinPePath(basePath),
                architecture.GetFolder(),
                "WinPE_OCs");

        /// <summary>
        /// Gets the path where the optional components for Windows PE are
        /// located.
        /// </summary>
        /// <param name="architecture">The platform architecture to get the
        /// optional components for.</param>
        /// <returns>The path to the optional components.</returns>
        public static string GetWinPeOptionalComponentsPath(
                Architecture architecture)
            => GetWinPeOptionalComponentsPath(Defaults.BasePath, architecture);

    }
}
